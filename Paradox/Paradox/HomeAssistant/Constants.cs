namespace Paradox.HomeAssistant
{
    /// <summary>
    /// HomeAssistant MQTT availability states
    /// </summary>
    public class AvailabilityState
    {
        /// <summary>
        /// When sensor is online
        /// </summary>
        public static readonly string Online = "online";

        /// <summary>
        /// When sensor is offline
        /// </summary>
        public static readonly string Offline = "offline";
    }

    /// <summary>
    /// HomeAssistant binary states
    /// </summary>
    public class BinaryState
    {
        /// <summary>
        /// When sensor is on
        /// </summary>
        public static readonly string On = "ON";

        /// <summary>
        /// When sensor is off
        /// </summary>
        public static readonly string Off = "OFF";
    }

    /// <summary>
    /// HomeAssistant alarm sensors
    /// </summary>
    public class HomeAssistantAlarmSensor
    {
        /// <summary>
        /// Trouble sensor
        /// </summary>
        public static readonly string Trouble = nameof(Trouble);
        /// <summary>
        /// Strobe sensor
        /// </summary>
        public static readonly string Strobe = nameof(Strobe);
        /// <summary>
        /// Area Threat sensor
        /// </summary>
        public static readonly string AreaThreat = nameof(AreaThreat);
        /// <summary>
        /// Alarm In Memory sensor
        /// </summary>
        public static readonly string AlarmInMemory = nameof(AlarmInMemory);
        /// <summary>
        /// Programming sensor
        /// </summary>
        public static readonly string Programming = nameof(Programming);
        /// <summary>
        /// AC Failure sensor
        /// </summary>
        public static readonly string ACFailure = nameof(ACFailure);
        /// <summary>
        /// Battery Failure sensor
        /// </summary>
        public static readonly string BatteryFailure = nameof(BatteryFailure);
        /// <summary>
        /// Clock Trouble sensor
        /// </summary>
        public static readonly string ClockTrouble = nameof(ClockTrouble);
        /// <summary>
        /// Bell Absent sensor
        /// </summary>
        public static readonly string BellAbsent = nameof(BellAbsent);
        /// <summary>
        /// Combus Fault sensor
        /// </summary>
        public static readonly string CombusFault = nameof(CombusFault);
    }

    /// <summary>
    /// HomeAssistant zone sensors
    /// </summary>
    public class HomeAssistantZoneSensor
    {
        /// <summary>
        /// Status sensor
        /// </summary>
        public static readonly string Status = nameof(Status);
        /// <summary>
        /// Alarm sensor
        /// </summary>
        public static readonly string Alarm = nameof(Alarm);
        /// <summary>
        /// Tamper sensor
        /// </summary>
        public static readonly string Tamper = nameof(Tamper);
        /// <summary>
        /// Supervision sensor
        /// </summary>
        public static readonly string Supervision = nameof(Supervision);
        /// <summary>
        /// FireAlarm sensor
        /// </summary>
        public static readonly string FireAlarm = nameof(FireAlarm);
        /// <summary>
        /// BatteryLow sensor
        /// </summary>
        public static readonly string BatteryLow = nameof(BatteryLow);
    }

    /// <summary>
    /// HomeAssistant alarm features
    /// </summary>
    public class HomeAssistantAlarmFeature
    {
        /// <summary>
        /// Arm with home mode
        /// </summary>
        public static readonly string ArmHome = "arm_home";
        /// <summary>
        /// Arm with away mode
        /// </summary>
        public static readonly string ArmAway = "arm_away";
        /// <summary>
        /// Arm with night mode
        /// </summary>
        public static readonly string ArmNight = "arm_night";
        /// <summary>
        /// Arm with vacation mode
        /// </summary>
        public static readonly string ArmVacation = "arm_vacation";
        /// <summary>
        /// Arm with custom bypass mode
        /// </summary>
        public static readonly string ArmCustomBypass = "arm_custom_bypass";
        /// <summary>
        /// Trigger
        /// </summary>
        public static readonly string Trigger = "trigger";
    }

    /// <summary>
    /// HomeAssistant alarm states
    /// </summary>
    public class HomeAssistantAlarmState
    {
        /// <summary>
        /// System disarmed
        /// </summary>
        public static readonly string Disarmed = "disarmed";
        /// <summary>
        /// System armed with home mode
        /// </summary>
        public static readonly string ArmedHome = "armed_home";
        /// <summary>
        /// System armed with away mode
        /// </summary>
        public static readonly string ArmedAway = "armed_away";
        /// <summary>
        /// System armed with night mode
        /// </summary>
        public static readonly string ArmedNight = "armed_night";
        /// <summary>
        /// System armed with vacation mode
        /// </summary>
        public static readonly string ArmedVacation = "armed_vacation";
        /// <summary>
        /// System armed with custom bypass mode
        /// </summary>
        public static readonly string ArmedForced = "armed_custom_bypass";
        /// <summary>
        /// System pending (entry delay)
        /// </summary>
        public static readonly string Pending = "pending";
        /// <summary>
        /// System triggered
        /// </summary>
        public static readonly string Triggered = "triggered";
        /// <summary>
        /// System arming (exit delay)
        /// </summary>
        public static readonly string Arming = "arming";
        /// <summary>
        /// System disarming
        /// </summary>
        public static readonly string Disarming = "disarming";
    }

    /// <summary>
    /// HomeAssistant alarm commands
    /// </summary>
    public class HomeAssistantAlarmCommand
    {
        /// <summary>
        /// The payload to disarm the Alarm Panel.
        /// </summary>
        public static readonly string Disarm = nameof(Disarm);
        /// <summary>
        /// The payload to set armed-home mode on the Alarm Panel (equivalent to Stay arm).
        /// </summary>
        public static readonly string ArmHome = ArmingMode.StayArm.ToString();
        /// <summary>
        /// The payload to set armed-away mode on the Alarm Panel (equivalent to Regular arm).
        /// </summary>
        public static readonly string ArmAway = ArmingMode.RegularArm.ToString();
        /// <summary>
        /// The payload to set armed-custom-bypass mode on the Alarm Panel (equivalent to Force arm).
        /// </summary>
        public static readonly string ArmBypass = ArmingMode.ForceArm.ToString();
    }

    /// <summary>
    /// HomeAssistant Entity Category
    /// See: https://developers.home-assistant.io/docs/core/entity/#generic-properties
    /// </summary>
    public class HomeAssistantEntityCategory
    {
        /// <summary>
        /// Generic on/off.
        /// </summary>
        public static readonly string None = null;

        /// <summary>
        /// Generic on/off.
        /// </summary>
        public static readonly string Configuration = "config";

        /// <summary>
        /// Generic on/off.
        /// </summary>
        public static readonly string Diagnostic = "diagnostic";
    }

    /// <summary>
    /// HomeAssistant BinarySensor Device Class
    /// See: https://www.home-assistant.io/integrations/binary_sensor/#device-class
    /// </summary>
    public class HomeAssistantBinarySensorClass
    {
        /// <summary>
        /// Generic on/off.
        /// </summary>
        public static readonly string None = null;

        /// <summary>
        /// on means low, off means normal.
        /// </summary>
        public static readonly string Battery = "battery";

        /// <summary>
        /// on means connected, off means disconnected.
        /// </summary>
        public static readonly string Connectivity = "connectivity";

        /// <summary>
        /// on means open, off means closed.
        /// </summary>
        public static readonly string Door = "door";

        /// <summary>
        /// on means open, off means closed.
        /// </summary>
        public static readonly string GarageDoor = "garage_door";

        /// <summary>
        /// on means light detected, off means no light.
        /// </summary>
        public static readonly string Light = "light";

        /// <summary>
        /// on means open (unlocked), off means closed (locked).
        /// </summary>
        public static readonly string Lock = "lock";

        /// <summary>
        /// on means motion detected, off means no motion (clear).
        /// </summary>
        public static readonly string Motion = "motion";

        /// <summary>
        /// on means moving, off means not moving (stopped).
        /// </summary>
        public static readonly string Moving = "moving";

        /// <summary>
        /// on means occupied (detected), off means not occupied (clear).
        /// </summary>
        public static readonly string Occupancy = "occupancy";

        /// <summary>
        /// on means open, off means closed.
        /// </summary>
        public static readonly string Opening = "opening";

        /// <summary>
        /// on means home, off means away.
        /// </summary>
        public static readonly string Presence = "presence";

        /// <summary>
        /// on means problem detected, off means no problem (OK).
        /// </summary>
        public static readonly string Problem = "problem";

        /// <summary>
        /// on means device is plugged in, off means device is unplugged.
        /// </summary>
        public static readonly string Plug = "Plug";

        /// <summary>
        /// on means power detected, off means no power.
        /// </summary>
        public static readonly string Power = "power";

        /// <summary>
        /// on means unsafe, off means safe.
        /// </summary>
        public static readonly string Safety = "safety";

        /// <summary>
        /// on means smoke detected, off means no smoke (clear).
        /// </summary>
        public static readonly string Smoke = "smoke";

        /// <summary>
        /// on means tampering detected, off means no tampering (clear).
        /// </summary>
        public static readonly string Tamper = "tamper";

        /// <summary>
        /// on means vibration detected, off means no vibration (clear).
        /// </summary>
        public static readonly string Vibration = "vibration";

        /// <summary>
        /// on means open, off means closed.
        /// </summary>
        public static readonly string Window = "window";
    }
}