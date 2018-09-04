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
    using System.Linq;

    /// <summary>
    /// Simple media player (support file and HTTP media uri)
    /// </summary>
    /// <seealso cref="SSound.Core.PlayerBase{System.String}" />
    public class MediaPlayer : PlayerBase<string>
    {
        private string file = null;
        private MediaFoundationReader reader = null;

        /// <summary>
        /// Gets the wave provider.
        /// </summary>
        /// <value>
        /// The wave provider.
        /// </value>
        public override IWaveProvider WaveProvider
        {
            get { return this.reader; }
        }

        /// <summary>
        /// Configures this player.
        /// </summary>
        public override void Configure()
        {
            if (this.Arguments.EndsWith(".m3u", System.StringComparison.OrdinalIgnoreCase))
            {
                this.file = M3UReader.Read(this.Arguments).FirstOrDefault();
                PackageHost.WriteInfo("Resolving file uri : {0}", this.file);
            }
            else
            {
                this.file = this.Arguments;
            }
        }

        /// <summary>
        /// Loads this player.
        /// </summary>
        /// <returns></returns>
        public override bool Load()
        {
            // Create the Reader
            this.reader = new MediaFoundationReader(this.file);
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (this.reader != null)
            {
                this.reader.Dispose();
                this.reader = null;
            }
        }
    }
}
