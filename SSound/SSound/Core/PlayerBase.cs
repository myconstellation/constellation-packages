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

namespace SSound.Core
{
    using Constellation.Package;
    using NAudio.Wave;
    using System;

    /// <summary>
    /// Represent a S-Sound base player
    /// </summary>
    /// <typeparam name="TArgs">The type of the arguments.</typeparam>
    /// <seealso cref="SSound.Core.PlayerBase" />
    public abstract class PlayerBase<TArgs> : PlayerBase
    {
        /// <summary>
        /// Gets or sets the player's arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public TArgs Arguments { get; set; }

        /// <summary>
        /// Configures this player with arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="useNewWaveOut">if set to <c>true</c> to use a new wave out.</param>
        /// <param name="volume">The default volume.</param>
        /// <returns></returns>
        public PlayerBase<TArgs> ConfigureWith(TArgs args, bool useNewWaveOut = false, float? volume = null)
        {
            this.Arguments = args;
            this.UseNewWaveOut = useNewWaveOut;
            this.Configure();
            PackageHost.WriteInfo("{0}: ConfigureWith {1}", this.ToString(), args.ToString());
            return this;
        }

        /// <summary>
        /// Sets the player's status.
        /// </summary>
        /// <param name="status">The status.</param>
        internal override void SetStatus(string status)
        {
            PackageHost.WriteInfo("{0}: {1}", this.ToString(), status);
            PackageHost.PushStateObject("CurrentPlayerInfo", new
            {
                Arguments = this.Arguments,
                Type = this.GetType().Name.ToString(),
                Status = status
            });
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

    /// <summary>
    /// Represent a S-Sound base player
    /// </summary>
    /// <seealso cref="SSound.Core.PlayerBase" />
    public abstract class PlayerBase : IDisposable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the player use a new wave out.
        /// </summary>
        /// <value>
        ///   <c>true</c> if use a new wave out; otherwise, <c>false</c>.
        /// </value>
        [Newtonsoft.Json.JsonIgnore]
        public bool UseNewWaveOut { get; set; }
        
        /// <summary>
        /// Gets the wave provider.
        /// </summary>
        /// <value>
        /// The wave provider.
        /// </value>
        [Newtonsoft.Json.JsonIgnore]
        public abstract IWaveProvider WaveProvider { get; }

        /// <summary>
        /// Configures this player.
        /// </summary>
        public virtual void Configure()
        {
        }

        /// <summary>
        /// Loads this player.
        /// </summary>
        /// <returns></returns>
        public virtual bool Load()
        {
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Sets the player's status.
        /// </summary>
        /// <param name="status">The status.</param>
        internal abstract void SetStatus(string status);
    }
}
