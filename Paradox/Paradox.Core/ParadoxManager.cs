/*
 *	 Paradox .NET library
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2017 - Sebastien Warin <http://sebastien.warin.fr>	   	  
 *	
 *	 Licensed to Sebastien Warin under one or more contributor
 *	 license agreements. Sebastien Warin licenses this file to you under
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

namespace Paradox
{
    using Paradox.Events;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /*
     *   Not implemented :
     *      - Virtual Inputs & Virtual PGM Events
     *      - Emergency Panic, Medical Panic, Fire Panic & Smoke Reset
     *      - Utility Keys
     */

    /// <summary>
    /// Represent the Paradox manager
    /// </summary>
    public class ParadoxManager
    {
        #region Singleton

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static ParadoxManager instance = null;

        /// <summary>
        /// Gets the Paradox manager instance.
        /// </summary>
        /// <value>
        /// The Paradox manager instance.
        /// </value>
        public static ParadoxManager Instance
        {
            get { return instance; }
        }

        #endregion

        #region Paradox base events

        /// <summary>
        /// Occurs when area is armed.
        /// </summary>
        [Description("AA")]
        public event EventHandler<AreaArmingEventArgs> AreaArmed;
        /// <summary>
        /// Occurs when area is quick armed.
        /// </summary>
        [Description("AQ")]
        public event EventHandler<AreaArmingEventArgs> AreaQuickArmed;
        /// <summary>
        /// Occurs when area is disarmed.
        /// </summary>
        [Description("AD")]
        public event EventHandler<AreaArmingEventArgs> AreaDisarmed;
        /// <summary>
        /// Occurs when area label is received.
        /// </summary>
        [Description("AL")]
        public event EventHandler<AreaLabelEventArgs> AreaLabelReceived;
        /// <summary>
        /// Occurs when area status changes.
        /// </summary>
        [Description("RA")]
        public event EventHandler<AreaStatusEventArgs> AreaStatusChanged;
        /// <summary>
        /// Occurs when zone label is received.
        /// </summary>
        [Description("ZL")]
        public event EventHandler<ZoneLabelEventArgs> ZoneLabelReceived;
        /// <summary>
        /// Occurs when zone status changes.
        /// </summary>
        [Description("RZ")]
        public event EventHandler<ZoneStatusEventArgs> ZoneStatusChanged;
        /// <summary>
        /// Occurs when user label is received.
        /// </summary>
        [Description("UL")]
        public event EventHandler<UserLabelEventArgs> UserLabelReceived;
        /// <summary>
        /// Occurs when system event is received.
        /// </summary>
        [Description("G")]
        public event EventHandler<ParadoxSystemEventArgs> SystemEventReceived;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Paradox module interface.
        /// </summary>
        /// <value>
        /// The Paradox module interface.
        /// </value>
        public ParadoxModuleInterface Interface { get; private set; }

        /// <summary>
        /// Gets the Paradox event manager.
        /// </summary>
        /// <value>
        /// The Paradox event manager.
        /// </value>
        public ParadoxSystemEventManager EventManager { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxManager"/> class.
        /// </summary>
        /// <param name="systemPortCom">The module serial port name.</param>
        public ParadoxManager(string systemPortCom)
            : this(new ParadoxModuleInterface(systemPortCom))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxManager" /> class.
        /// </summary>
        /// <param name="systemPortCom">The module serial port name.</param>
        /// <param name="baudRate">The module serial baud rate.</param>
        public ParadoxManager(string systemPortCom, int baudRate)
            : this(new ParadoxModuleInterface(systemPortCom, baudRate))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxManager"/> class.
        /// </summary>
        /// <param name="moduleInferface">The Paradox module interface.</param>
        internal ParadoxManager(ParadoxModuleInterface moduleInferface)
        {
            if (instance == null)
            {
                instance = this;
                this.Interface = moduleInferface;
                this.Interface.MessageReceived += OnMessageReceived;
                this.EventManager = new ParadoxSystemEventManager(this);
                if (!this.Interface.IsConnected)
                {
                    this.Interface.Connect();
                }
            }
            else
            {
                throw new InvalidOperationException("ParadoxManager is a singleton object. Use the existing instance from 'ParadoxManager.Instance'");
            }
        }

        #endregion

        #region Paradox commands

        /// <summary>
        /// Requests area status.
        /// </summary>
        /// <param name="area">The area.</param>
        public void RequestArea(Area area)
        {
            this.Interface.SendCommand("RA" + Utils.GetStringId(area));
        }

        /// <summary>
        /// Requests zone status.
        /// </summary>
        /// <param name="zone">The zone.</param>
        public void RequestZone(Zone zone)
        {
            this.Interface.SendCommand("RZ" + Utils.GetStringId(zone));
        }

        /// <summary>
        /// Requests zone label.
        /// </summary>
        /// <param name="zone">The zone.</param>
        public void RequestZoneLabel(Zone zone)
        {
            this.Interface.SendCommand("ZL" + Utils.GetStringId(zone));
        }

        /// <summary>
        /// Requests area label.
        /// </summary>
        /// <param name="area">The area.</param>
        public void RequestAreaLabel(Area area)
        {
            this.Interface.SendCommand("AL" + Utils.GetStringId(area));
        }

        /// <summary>
        /// Requests user label.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public void RequestUserLabel(int userId)
        {
            this.Interface.SendCommand("UL" + Utils.GetStringId(userId));
        }

        /// <summary>
        /// Arms area
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="pinCode">The pin code.</param>
        /// <returns></returns>
        public async Task<bool> AreaArm(Area area, ArmingMode mode, string pinCode)
        {
            return await this.Interface.SendCommandWithAcknowledgement("AA" + Utils.GetStringId(area) + Utils.GetDescription(mode) + pinCode);
        }

        /// <summary>
        /// Quick arms area
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public async Task<bool> AreaQuickArm(Area area, ArmingMode mode)
        {
            return await this.Interface.SendCommandWithAcknowledgement("AQ" + Utils.GetStringId(area) + Utils.GetDescription(mode));
        }

        /// <summary>
        /// Disarms area
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="pinCode">The pin code.</param>
        /// <returns></returns>
        public async Task<bool> AreaDisarm(Area area, string pinCode)
        {
            return await this.Interface.SendCommandWithAcknowledgement("AD" + Utils.GetStringId(area) + pinCode);
        }

        #endregion

        #region Incoming message handler

        /// <summary>
        /// Handles when message is received from the PRT3 module.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ParadoxMessageEventArgs"/> instance containing the raw message.</param>
        private void OnMessageReceived(object sender, ParadoxMessageEventArgs e)
        {
            // Find the message handler
            var eventToRaise = this.GetType().GetEvents()
                    .Select(ev => new
                    {
                        Description = ev.GetCustomAttribute<DescriptionAttribute>(),
                        EventInfo = ev
                    })
                    .Where(ev => ev.Description != null && e.Message.StartsWith(ev.Description.Description) && ev.EventInfo != null)
                    .Select(ev => new
                    {
                        EventName = ev.EventInfo.Name,
                        EventArgsType = ev.EventInfo.EventHandlerType.GenericTypeArguments.FirstOrDefault()
                    })
                    .SingleOrDefault();
            // If the event exist
            if (eventToRaise != null && eventToRaise.EventArgsType != null)
            {
                try
                {
                    // Process the message
                    var eventArgs = (ParadoxBaseEventArgs)Activator.CreateInstance(eventToRaise.EventArgsType);
                    eventArgs.ProcessMessage(e.Message);
                    // Invoke handler(s)
                    var eventDelegate = (MulticastDelegate)this.GetType().GetField(eventToRaise.EventName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
                    if (eventDelegate != null)
                    {
                        foreach (var handler in eventDelegate.GetInvocationList())
                        {
                            handler.Method.Invoke(handler.Target, new object[] { this, eventArgs });
                        }
                    }
                }
                catch { }
            }
        }

        #endregion
    }
}
