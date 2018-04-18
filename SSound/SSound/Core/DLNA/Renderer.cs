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

namespace SSound.Core.Dlna
{
    using OpenSource.UPnP.AV;
    using OpenSource.UPnP.AV.RENDERER.Device;

    /// <summary>
    /// DLNA Media Renderer
    /// </summary>
    public class Renderer
    {
        private static object syncLock = new object();

        /// <summary>
        /// Gets the AV connection.
        /// </summary>
        /// <value>
        /// The AV connection.
        /// </value>
        public AVConnection AVConnection { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Renderer(AVConnection connection)
        {
            this.AVConnection = connection;
            this.AVConnection.OnPlay += new AVConnection.PlayHandler(PlaySink);
            this.AVConnection.OnPause += new AVConnection.StopPauseRecordHandler(PauseSink);
            this.AVConnection.OnStop += new AVConnection.StopPauseRecordHandler(StopSink);
            this.AVConnection.OnMuteChanged += new AVConnection.MuteChangedHandler(MuteSink);
            this.AVConnection.OnVolumeChanged += new AVConnection.VolumeChangedHandler(VolumeSink);
            //this.avConnection.OnCurrentURIChanged += new AVConnection.VariableChangedHandler(UriChangeSink);
            //this.avConnection.OnSeek += new AVConnection.SeekHandler(SeekSink);
            this.AVConnection.SetVolume(DvRenderingControl.Enum_A_ARG_TYPE_Channel.MASTER, 100);
            this.AVConnection.SetVolume(DvRenderingControl.Enum_A_ARG_TYPE_Channel.LF, 100);
            this.AVConnection.SetVolume(DvRenderingControl.Enum_A_ARG_TYPE_Channel.RF, 100);
        }

        private void UriChangeSink(AVConnection sender)
        {
            if (sender.ID == AVConnection.ID)
            {
                if (sender.CurrentURI != null)
                {
                    // if(sender.InfoString.MimeType.ToLower()=="audio/mpegurl" ||
                    // 	sender.CurrentURI.PathAndQuery.ToLower().EndsWith(".m3u"))
                    // {
                    // }
                }
            }
        }

        private void SeekSink(AVConnection sender, DvAVTransport.Enum_A_ARG_TYPE_SeekMode SeekMode, string Target)
        {
        }

        private void PauseSink(AVConnection sender)
        {
            Manager.Instance.Pause();
        }

        private void StopSink(AVConnection sender)
        {
            Manager.Instance.Stop();
        }

        private void VolumeSink(AVConnection sender, DvRenderingControl.Enum_A_ARG_TYPE_Channel Channel, System.UInt16 DesiredVolume)
        {
            Manager.Instance.SetVolume((float)DesiredVolume / (float)100);
        }

        private void MuteSink(AVConnection sender, DvRenderingControl.Enum_A_ARG_TYPE_Channel Channel, bool NewMute)
        {
            Manager.Instance.SetMute(NewMute);
        }

        private void PlaySink(AVConnection sender, DvAVTransport.Enum_TransportPlaySpeed Speed)
        {
            if (sender.CurrentURI == null)
            {
                Manager.Instance.Stop();
                sender.CurrentTransportState = DvAVTransport.Enum_TransportState.STOPPED;
                return;
            }

            if (sender.CurrentTransportState == DvAVTransport.Enum_TransportState.PAUSED_PLAYBACK)
            {
                Manager.Instance.Play();
            }
            else
            {
                if (sender.CurrentTransportState != DvAVTransport.Enum_TransportState.PLAYING)
                {

                    lock (syncLock)
                    {
                        sender.CurrentTransportState = DvAVTransport.Enum_TransportState.TRANSITIONING;
                        if (sender.CurrentURI.LocalPath.EndsWith(".m3u", System.StringComparison.OrdinalIgnoreCase))
                        {
                            Manager.Instance.PlayM3UList(sender.CurrentURI.ToString());
                        }
                        else
                        {
                            Manager.Instance.PlayMediaRessource(sender.CurrentURI.ToString());
                        }
                    }
                }
            }
        }
    }
}
