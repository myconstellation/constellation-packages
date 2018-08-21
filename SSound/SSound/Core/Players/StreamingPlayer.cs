/*
 *	 S-Sound - Multi-room audio system for Constellation
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2018 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Constellation under one or more contributor
 *	 license agreements. Constellation licenses this file to you under
 *	 the Apache License, Version 2.0 (the "License"); you may
 *	 not use this file except in compliance with the License.
 *	 You may obtain a copy of the License at
 *	
 *	 http://www.apache.org/licenses/LICENSE-2.0
 *	
 *	 Unless required by applicable law or agreed to in writing,
 *	 software distributed under the License is distributed on an
 *	 "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 *	 KIND, either express or implied. See the License for the
 *	 specific language governing permissions and limitations
 *	 under the License.
 */

namespace SSound.Core.Players
{
    using Constellation.Package;
    using NAudio.Wave;
    using SSound.Core.Utils;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;

    /// <summary>
    /// HTTP/MP3 streaming player
    /// </summary>
    /// <seealso cref="SSound.Core.PlayerBase{System.String}" />
    public class StreamingPlayer : PlayerBase<string>
    {
        private enum StreamingPlaybackState { Stopped, Playing, Buffering, Paused }

        private BufferedWaveProvider bufferedWaveProvider;
        private volatile StreamingPlaybackState playbackState;
        private volatile bool fullyDownloaded;
        private HttpWebRequest webRequest;

        private bool IsBufferNearlyFull
        {
            get
            {
                return this.bufferedWaveProvider != null &&
                    this.bufferedWaveProvider.BufferLength - this.bufferedWaveProvider.BufferedBytes < this.bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4;
            }
        }

        /// <summary>
        /// Gets the wave provider.
        /// </summary>
        /// <value>
        /// The wave provider.
        /// </value>
        public override IWaveProvider WaveProvider
        {
            get { return this.bufferedWaveProvider; }
        }

        /// <summary>
        /// Configures this player.
        /// </summary>
        /// <exception cref="Exception">Use a new instance of this player !</exception>
        public override void Configure()
        {
            if (this.webRequest == null)
            {
                string mp3StreamUri = this.Arguments;
                if (this.Arguments.EndsWith(".m3u", StringComparison.OrdinalIgnoreCase))
                {
                    mp3StreamUri = M3UReader.Read(this.Arguments).FirstOrDefault();
                    PackageHost.WriteInfo("Resolving stream uri : {0}", mp3StreamUri);
                }

                this.webRequest = (HttpWebRequest)WebRequest.Create(mp3StreamUri);
            }
            else
            {
                throw new Exception("Use a new instance of this player !");
            }
        }

        /// <summary>
        /// Loads this player.
        /// </summary>
        /// <returns></returns>
        public override bool Load()
        {
            this.SetStatus("Loading");
            this.playbackState = StreamingPlaybackState.Buffering;
            this.bufferedWaveProvider = null;

            ThreadPool.QueueUserWorkItem(StreamMp3);

            // Initializing...
            while (bufferedWaveProvider == null && this.playbackState != StreamingPlaybackState.Stopped)
            {
                // Wait !
                Thread.Sleep(1000);
            }

            if (this.playbackState != StreamingPlaybackState.Stopped)
            {
                this.SetStatus("Playing");
                this.playbackState = StreamingPlaybackState.Playing;
                ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
                {
                    Thread.Sleep(2000);
                    do
                    {
                        if (this.bufferedWaveProvider != null)
                        {
                            var bufferedSeconds = this.bufferedWaveProvider.BufferedDuration.TotalSeconds;

                            // make it stutter less if we buffer up a decent amount before playing
                            if (bufferedSeconds < 0.5 && this.playbackState == StreamingPlaybackState.Playing && !this.fullyDownloaded)
                            {
                                this.SetStatus("Buffering");
                                this.playbackState = StreamingPlaybackState.Buffering;
                                Manager.Instance.Pause();
                            }
                            else if (bufferedSeconds > 10.0 && playbackState == StreamingPlaybackState.Buffering)
                            {
                                this.SetStatus("Playing");
                                this.playbackState = StreamingPlaybackState.Playing;
                                Manager.Instance.Play();
                            }
                            else if (fullyDownloaded && bufferedSeconds == 0)
                            {
                                this.SetStatus("Stop");
                                this.playbackState = StreamingPlaybackState.Stopped;
                                Manager.Instance.Stop();
                            }
                        }

                        Thread.Sleep(1000);
                    }
                    while (this.playbackState != StreamingPlaybackState.Stopped);
                }), null);

                return true;
            }
            else
            { 
                return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (playbackState != StreamingPlaybackState.Stopped)
            {
                if (!fullyDownloaded)
                {
                    webRequest.Abort();
                }

                playbackState = StreamingPlaybackState.Stopped;
                Thread.Sleep(500);
            }
        }

        private void StreamMp3(object state)
        {
            this.fullyDownloaded = false;
            HttpWebResponse resp = null;
            try
            {
                resp = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status != WebExceptionStatus.RequestCanceled)
                {
                    PackageHost.WriteError("Error HTTP: " + e.Message);
                    Manager.Instance.Stop();
                }
                return;
            }
            var buffer = new byte[16384 * 4]; // needs to be big enough to hold a decompressed frame

            IMp3FrameDecompressor decompressor = null;
            try
            {
                using (var responseStream = resp.GetResponseStream())
                {
                    var readFullyStream = new ReadFullyStream(responseStream);
                    do
                    {
                        if (IsBufferNearlyFull)
                        {
                            System.Diagnostics.Debug.WriteLine("Buffer getting full, taking a break");
                            Thread.Sleep(500);
                        }
                        else
                        {
                            Mp3Frame frame;
                            try
                            {
                                frame = Mp3Frame.LoadFromStream(readFullyStream);
                            }
                            catch (EndOfStreamException)
                            {
                                fullyDownloaded = true;
                                // reached the end of the MP3 file / stream
                                break;
                            }
                            catch (WebException)
                            {
                                // probably we have aborted download from the GUI thread
                                break;
                            }
                            if (decompressor == null)
                            {
                                // don't think these details matter too much - just help ACM select the right codec
                                // however, the buffered provider doesn't know what sample rate it is working at
                                // until we have a frame
                                decompressor = CreateFrameDecompressor(frame);
                                bufferedWaveProvider = new BufferedWaveProvider(decompressor.OutputFormat);
                                bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(20); // allow us to get well ahead of ourselves
                                //this.bufferedWaveProvider.BufferedDuration = 250;
                            }
                            int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                            //Debug.WriteLine(String.Format("Decompressed a frame {0}", decompressed));
                            bufferedWaveProvider.AddSamples(buffer, 0, decompressed);
                        }

                    }
                    while (playbackState != StreamingPlaybackState.Stopped);

                    // ######## Exiting ########

                    // was doing this in a finally block, but for some reason
                    // we are hanging on response stream .Dispose so never get there
                    decompressor.Dispose();
                }
            }
            finally
            {
                if (decompressor != null)
                {
                    decompressor.Dispose();
                }
            }
        }

        private static IMp3FrameDecompressor CreateFrameDecompressor(Mp3Frame frame)
        {
            WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2, frame.FrameLength, frame.BitRate);
            return new AcmMp3FrameDecompressor(waveFormat);
        }

        #region ReadFullyStream nested class

        private class ReadFullyStream : Stream
        {
            private readonly Stream sourceStream;
            private long pos; // psuedo-position
            private readonly byte[] readAheadBuffer;
            private int readAheadLength;
            private int readAheadOffset;

            public ReadFullyStream(Stream sourceStream)
            {
                this.sourceStream = sourceStream;
                readAheadBuffer = new byte[4096];
            }
            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void Flush()
            {
                throw new InvalidOperationException();
            }

            public override long Length
            {
                get { return pos; }
            }

            public override long Position
            {
                get
                {
                    return pos;
                }
                set
                {
                    throw new InvalidOperationException();
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int bytesRead = 0;
                while (bytesRead < count)
                {
                    int readAheadAvailableBytes = readAheadLength - readAheadOffset;
                    int bytesRequired = count - bytesRead;
                    if (readAheadAvailableBytes > 0)
                    {
                        int toCopy = Math.Min(readAheadAvailableBytes, bytesRequired);
                        Array.Copy(readAheadBuffer, readAheadOffset, buffer, offset + bytesRead, toCopy);
                        bytesRead += toCopy;
                        readAheadOffset += toCopy;
                    }
                    else
                    {
                        readAheadOffset = 0;
                        readAheadLength = sourceStream.Read(readAheadBuffer, 0, readAheadBuffer.Length);
                        //Debug.WriteLine(String.Format("Read {0} bytes (requested {1})", readAheadLength, readAheadBuffer.Length));
                        if (readAheadLength == 0)
                        {
                            break;
                        }
                    }
                }
                pos += bytesRead;
                return bytesRead;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new InvalidOperationException();
            }

            public override void SetLength(long value)
            {
                throw new InvalidOperationException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion
    }
}
