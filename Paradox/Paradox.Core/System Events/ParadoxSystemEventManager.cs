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

namespace Paradox.Events
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represent the Paradox events manager
    /// </summary>
    public class ParadoxSystemEventManager
    {
        #region Paradox Events

        /// <summary>
        /// Occurs when zone changes.
        /// </summary>
        [ParadoxGroupEvent(0, 3)]
        public event EventHandler<ZoneEventArgs> ZoneChanged;

        /// <summary>
        /// Occurs when non reportable event is raised.
        /// </summary>
        [ParadoxGroupEvent(4)]
        public event EventHandler<NonReportableEventArgs> NonReportableEvent;

        /// <summary>
        /// Occurs when user code is entered on keypad.
        /// </summary>
        [ParadoxGroupEvent(5)]
        public event EventHandler<UserEventArgs> UserCodeEnteredOnKeypad;

        /// <summary>
        /// Occurs when user arming area.
        /// </summary>
        [ParadoxGroupEvent(9, 10)]
        public event EventHandler<UserEventArgs> Arming;

        /// <summary>
        /// Occurs during a special arming .
        /// </summary>
        [ParadoxGroupEvent(12)]
        public event EventHandler<SpecialArmingEventArgs> SpecialArming;

        /// <summary>
        /// Occurs when user desarming area.
        /// </summary>
        [ParadoxGroupEvent(new int[] { 13, 14, 16, 17 })]
        public event EventHandler<UserEventArgs> Disarming;

        /// <summary>
        /// Occurs when alarm is cancelled.
        /// </summary>
        [ParadoxGroupEvent(19, 20)]
        public event EventHandler<UserEventArgs> AlarmCancelled;

        /// <summary>
        /// Occurs during a special disarming.
        /// </summary>
        [ParadoxGroupEvent(22)]
        public event EventHandler<SpecialDesarmingEventArgs> SpecialDisarming;

        /// <summary>
        /// Occurs when zone is bypassed.
        /// </summary>
        [ParadoxGroupEvent(23)]
        public event EventHandler<ZoneEventArgs> ZoneBypassed;

        /// <summary>
        /// Occurs when zone is in alarm.
        /// </summary>
        [ParadoxGroupEvent(24)]
        public event EventHandler<ZoneEventArgs> ZoneInAlarm;

        /// <summary>
        /// Occurs during a fire alarm.
        /// </summary>
        [ParadoxGroupEvent(25)]
        public event EventHandler<ZoneEventArgs> FireAlarm;

        /// <summary>
        /// Occurs when zone alarm is restored.
        /// </summary>
        [ParadoxGroupEvent(26)]
        public event EventHandler<ZoneEventArgs> ZoneAlarmRestored;

        /// <summary>
        /// Occurs when fire alarm is restored.
        /// </summary>
        [ParadoxGroupEvent(27)]
        public event EventHandler<ZoneEventArgs> FireAlarmRestored;

        /// <summary>
        /// Occurs when early to disarm by user.
        /// </summary>
        [ParadoxGroupEvent(28)]
        public event EventHandler<UserEventArgs> EarlyToDisarmByUser;

        /// <summary>
        /// Occurs when late to disarm by user.
        /// </summary>
        [ParadoxGroupEvent(29)]
        public event EventHandler<UserEventArgs> LateToDisarmByUser;

        /// <summary>
        /// Occurs during a special alarm.
        /// </summary>
        [ParadoxGroupEvent(30)]
        public event EventHandler<SpecialAlarmEventArgs> SpecialAlarm;

        /// <summary>
        /// Occurs when zone shutdown.
        /// </summary>
        [ParadoxGroupEvent(32)]
        public event EventHandler<ZoneEventArgs> ZoneShutdown;

        /// <summary>
        /// Occurs when zone is tampered.
        /// </summary>
        [ParadoxGroupEvent(33)]
        public event EventHandler<ZoneEventArgs> ZoneTampered;

        /// <summary>
        /// Occurs when zone tamper is restored.
        /// </summary>
        [ParadoxGroupEvent(34)]
        public event EventHandler<ZoneEventArgs> ZoneTamperRestored;

        /// <summary>
        /// Occurs when keypad lockout.
        /// </summary>
        [ParadoxGroupEvent(35)]
        public event EventHandler<AreaEventArgs> KeypadLockout;

        /// <summary>
        /// Occurs when system has trouble.
        /// </summary>
        [ParadoxGroupEvent(36)]
        public event EventHandler<TroubleEventArgs> Trouble;

        /// <summary>
        /// Occurs when trouble is restored.
        /// </summary>
        [ParadoxGroupEvent(37)]
        public event EventHandler<TroubleEventArgs> TroubleRestored;

        /// <summary>
        /// Occurs when module has trouble.
        /// </summary>
        [ParadoxGroupEvent(38)]
        public event EventHandler<ModuleTroubleEventArgs> ModuleTrouble;

        /// <summary>
        /// Occurs when module trouble is restored.
        /// </summary>
        [ParadoxGroupEvent(39)]
        public event EventHandler<ModuleTroubleEventArgs> ModuleTroubleRestored;

        /// <summary>
        /// Occurs during a special event.
        /// </summary>
        [ParadoxGroupEvent(45)]
        public event EventHandler<SpecialEventArgs> SpecialEvent;

        /// <summary>
        /// Occurs when early to arm by user.
        /// </summary>
        [ParadoxGroupEvent(46)]
        public event EventHandler<UserEventArgs> EarlyToArmByUser;

        /// <summary>
        /// Occurs when late to arm by user.
        /// </summary>
        [ParadoxGroupEvent(47)]
        public event EventHandler<UserEventArgs> LateToArmByUser;

        /// <summary>
        /// Occurs when intellizone is triggered.
        /// </summary>
        [ParadoxGroupEvent(55)]
        public event EventHandler<ZoneEventArgs> IntellizoneTriggered;

        /// <summary>
        /// Occurs when new module is assigned on combus.
        /// </summary>
        [ParadoxGroupEvent(58)]
        public event EventHandler<ModuleEventArgs> NewModuleAssignedOnCombus;

        /// <summary>
        /// Occurs when module is manually removed from combus.
        /// </summary>
        [ParadoxGroupEvent(59)]
        public event EventHandler<ModuleEventArgs> ModuleManuallyRemovedFromCombus;

        /// <summary>
        /// Occurs when access is granted.
        /// </summary>
        [ParadoxGroupEvent(62)]
        public event EventHandler<UserEventArgs> AccessGranted;

        /// <summary>
        /// Occurs when access is denied.
        /// </summary>
        [ParadoxGroupEvent(63)]
        public event EventHandler<UserEventArgs> AccessDenied;

        /// <summary>
        /// Occurs when the Status1 changes.
        /// </summary>
        [ParadoxGroupEvent(64)]
        public event EventHandler<StatusEventArgs<Status1EventType>> Status1Changed;

        /// <summary>
        /// Occurs when the Status2 changes.
        /// </summary>
        [ParadoxGroupEvent(65)]
        public event EventHandler<StatusEventArgs<Status2EventType>> Status2Changed;

        /// <summary>
        /// Occurs when the Status3 changes.
        /// </summary>
        [ParadoxGroupEvent(66)]
        public event EventHandler<StatusEventArgs<Status3EventType>> Status3Changed;

        #endregion

        #region Events manager logic

        /// <summary>
        /// Occurs when new event occurred.
        /// </summary>
        public event EventHandler NewEventOccurred;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxSystemEventManager"/> class.
        /// </summary>
        /// <param name="manager">The Paradox manager.</param>
        internal ParadoxSystemEventManager(ParadoxManager manager)
        {
            manager.SystemEventReceived += OnSystemEvent;
        }

        /// <summary>
        /// Handles when SystemEvent is received from the Paradox manager.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ParadoxSystemEventArgs"/> instance containing the event data.</param>
        private void OnSystemEvent(object sender, ParadoxSystemEventArgs e)
        {
            // Get the event to raise from the event group
            var eventToRaise = this.GetType().GetEvents()
                    .Select(ev => new
                    {
                        Description = ev.GetCustomAttribute<ParadoxGroupEventAttribute>(),
                        EventInfo = ev
                    })
                    .Where(ev => ev.Description != null && ev.Description.IsMatch((int)e.EventGroup) && ev.EventInfo != null)
                    .Select(ev => new
                    {
                        EventName = ev.EventInfo.Name,
                        EventArgsType = ev.EventInfo.EventHandlerType.GenericTypeArguments.FirstOrDefault()
                    })
                    .SingleOrDefault();

            // If the event exist
            if (eventToRaise != null && eventToRaise.EventArgsType != null)
            {
                // Create the custom event args from the original event
                ParadoxSystemEventArgs eventArgs = (ParadoxSystemEventArgs)Activator.CreateInstance(eventToRaise.EventArgsType);
                eventArgs.CopyFrom(e);
                // Raise the event handler(s)
                MulticastDelegate eventDelegate = (MulticastDelegate)this.GetType().GetField(eventToRaise.EventName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
                if (eventDelegate != null)
                {
                    foreach (var handler in eventDelegate.GetInvocationList())
                    {
                        handler.Method.Invoke(handler.Target, new object[] { this, eventArgs });
                    }
                }
                // Raise the generic NewEventOccurred event
                if (this.NewEventOccurred != null)
                {
                    this.NewEventOccurred(this, EventArgs.Empty);
                }
            }
        }

        private class ParadoxGroupEventAttribute : Attribute
        {
            public int[] GroupEventIds { get; set; }

            public ParadoxGroupEventAttribute(int groupEventId)
            {
                this.GroupEventIds = new[] { groupEventId };
            }

            public ParadoxGroupEventAttribute(int startGroupEventId, int endGroupEventId)
            {
                int count = endGroupEventId - startGroupEventId + 1;
                this.GroupEventIds = new int[count];
                for (int i = 0; i < count; i++)
                {
                    this.GroupEventIds[i] = startGroupEventId + i;
                }
            }

            public ParadoxGroupEventAttribute(int[] groupEventIdList)
            {
                this.GroupEventIds = groupEventIdList;
            }

            public bool IsMatch(int groupEventId)
            {
                return this.GroupEventIds.Contains(groupEventId);
            }
        }
        
        #endregion
    }
}
