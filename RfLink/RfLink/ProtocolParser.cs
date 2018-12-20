using System;
using System.Collections.Generic;
using Constellation.Package;

namespace RFLinkNet
{
    /// <summary>
    /// Send or receive
    /// </summary>
    public enum Direction
    {
        None = 0,
        MasterToRF = 10,
        RFToMaster = 20,
        MasterToMaster = 11
    }

    /// <summary>
    /// Rflink commands
    /// </summary>
    public static class Commands
    {
        /// <summary>
        /// Request rflink status
        /// </summary>
        public static readonly string GetStatus = "status;";

        /// <summary>
        /// Request rflink version
        /// </summary>
        public static readonly string GetVersion = "version;";

        /// <summary>
        /// Build rflink command
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ConstructPacket(string request)
        {
            return $"{(int)Direction.MasterToRF};{request}";
        }
    }

    /// <summary>
    /// Describes a RF Data packet with helper functions
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class RFData : EventArgs
    {
        /// <summary>
        /// Time the data was processed
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// RFLink Gateway counter ID
        /// </summary>
        public string Counter { get; set; }

        /// <summary>
        /// Protocol used by the device
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// All fields returned by the gateway
        /// </summary>
        public Dictionary<string, string> Fields { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Ctor
        /// </summary>
        public RFData()
        {

        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = string.Empty;

            foreach (KeyValuePair<string, string> kvp in Fields)
            {
                output += string.Format("{0}:{1}:", kvp.Key, kvp.Value);
            }

            return output;
        }
    }

    /// <summary>
    /// Helper to read rf data
    /// </summary>
    public static class ProtocolParser
    {
        private static readonly char DataDelim = ';';
        private static readonly char FieldDelim = '=';

        internal static RFData ProcessData(string indata)
        {
            RFData rf = new RFData();
            string[] splitProtocol = indata.Split(DataDelim);
            
            if (splitProtocol.Length == 1 // RTS Record: 0 Address: FFFFFF RC: FFFF …
                || splitProtocol[1].Equals("PING")) // ping
                
            {
                rf.Fields.Add("LOG", indata);
                return rf;
            }

            // data sent
            if (indata.StartsWith(((int)Direction.MasterToRF).ToString()))
            {
                rf.Protocol = splitProtocol[1];
                rf.Fields.Add("Protocol", rf.Protocol);
                rf.Fields.Add("ID", splitProtocol[2]);
                rf.Fields.Add("SWITCH", splitProtocol[3]);
                rf.Fields.Add("CMD", splitProtocol[4]);
            }
            else // data received
            {

                rf.Counter = splitProtocol[1];
                rf.Protocol = splitProtocol[2];
                rf.Fields.Add("Protocol", rf.Protocol);

                ProcessesFields(splitProtocol, ref rf);
            }

            return rf;
        }

        internal static void ProcessesFields(string[] fields, ref RFData rf)
        {
            string[] labelSplit;

            // Skip pass "20;xx;
            for (int i = 3; i < fields.Length - 1; i++)
            {
                try
                {
                    labelSplit = fields[i].Split(FieldDelim);
                    rf.Fields.Add(labelSplit[0], labelSplit[1]);
                }
                catch { }//todo: remove?
            }
        }

        internal static bool ToBoolean(string stringbool)
        {
            if (stringbool == "ON")
            {
                return true;
            }
            else if (stringbool == "OFF")
            {
                return false;
            }
            else
            {
                throw new ArgumentException("Could not convert RF string to bool");
            }
        }
    }
}
