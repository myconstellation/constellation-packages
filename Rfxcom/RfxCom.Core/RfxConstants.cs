/*
 *	 RFXCom Core library
 *	 Author: Sébastien Warin
 *	 Web site: http://sebastien.warin.fr
 *	 Copyright (C) 2014-2016 - Sebastien Warin <http://sebastien.warin.fr>	   	  
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

namespace RfxCom.Core
{
    using System.ComponentModel;

    public enum RfxPacketType : byte
    {
        InterfaceControl = 0x00,
        InterfaceMessage = 0x01,
        ReceiverTransmitterMessage = 0x02,
        UndecodedRFMessage = 0x03,
        Lighting1 = 0x10,
        Lighting2 = 0x11,
        Lighting3 = 0x12,
        Lighting4 = 0x13,
        Lighting5 = 0x14,
        Lighting6 = 0x15,
        Chime = 0x16,
        Curtain1 = 0x18,
        Blinds1 = 0x19,
        RTS = 0x1A,
        Security1 = 0x20,
        Camera1 = 0x28,
        RemoteControlAndIR = 0x30,
        Thermostat1 = 0x40,
        Thermostat2 = 0x41,
        Thermostat3 = 0x42,
        TemperatureSensors = 0x50,
        HumiditySensors = 0x51,
        TemperatureHumiditySensors = 0x52,
        Barometricsensors = 0x53,
        TemperatureHumidityBarometricSensors = 0x54,
        RainSensors = 0x55,
        WindSensors = 0x56,
        UVSensors = 0x57,
        DateTimeSensors = 0x58,
        CurrentSensors = 0x59,
        EnergyUsageSensors = 0x5A,
        CurrentEnergySensors = 0x5B,
        PowerSensors = 0x5C,
        WeightingScale = 0x5D,
        GasUsageSensors = 0x5E,
        WaterUsageSensors = 0x5F,
        RFXSensor = 0x70,
        RFXMeter = 0x71,
        FS20 = 0x72,
    }

    public enum RfxProtocol
    {
        [Description("Display undecoded")]
        DisplayUndecoded,
        [Description("RFU6")]
        RFU6,
        [Description("Byron SX")]
        ByronSX,
        [Description("RSL")]
        RSL,
        [Description("Lighting4")]
        Lighting4,
        [Description("FineOffset/Viking")]
        FineOffsetViking,
        [Description("Rubicson")]
        Rubicson,
        [Description("AE Blyss")]
        AEBlyss,
        [Description("BlindsT1/T2/T3/T4")]
        BlindsT1to4,
        [Description("BlindsT0")]
        BlindsT0,
        [Description("ProGuard")]
        ProGuard,
        [Description("FS20")]
        FS20,
        [Description("La Crosse")]
        LaCrosse,
        [Description("Hideki/UPM")]
        HidekiUPM,
        [Description("AD LightwaveRF")]
        ADLightwaveRF,
        [Description("Mertik")]
        Mertik,
        [Description("Visonic")]
        Visonic,
        [Description("ATI")]
        ATI,
        [Description("Oregon Scientific")]
        OregonScientific,
        [Description("Meiantech")]
        Meiantech,
        [Description("HomeEasy EU")]
        HomeEasyEU,
        [Description("AC")]
        AC,
        [Description("ARC")]
        ARC,
        [Description("X10")]
        X10,
    };

    public enum RFYCommand
    {
        Stop = 0x0,
        Up = 0x1,
        UpStop = 0x2,
        Down = 0x3,
        DownStop = 0x4,
        UpDown = 0x5,
        Program = 0x7,
        Program2seconds = 0x8,
        Program7seconds = 0x9,
        Stop2seconds = 0xA,
        Stop5seconds = 0xB,
        UpDown5seconds = 0xC,
        EraseRTSremoteFromRFXtrx = 0xD,
        EraseAllRTSremotesFromRFXtrx = 0xE
    }
}
