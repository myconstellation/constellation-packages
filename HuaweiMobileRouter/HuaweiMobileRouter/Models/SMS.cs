/*
 *	 Huawei Mobile Router Package for Constellation
 *	 Web site: http://www.myConstellation.io
 *	 Copyright (C) 2018 - Sebastien Warin <http://sebastien.warin.fr>
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

namespace HuaweiMobileRouter.Models
{
    using Constellation.Package;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// SMS list on the SIM
    /// </summary>
    [StateObject, XmlRoot(ElementName = "response")]
    public class SMSList
    {
        [XmlElement(ElementName = "Count")]
        public int Count { get; set; }

        [XmlElement(ElementName = "Messages")]
        public MessageList List { get; set; }
    }

    /// <summary>
    /// SMS Message list
    /// </summary>
    [XmlRoot(ElementName = "Messages")]
    public class MessageList
    {
        [XmlElement(ElementName = "Message")]
        public List<Message> Messages { get; set; }
    }

    /// <summary>
    /// SMS message
    /// </summary>
    [XmlRoot(ElementName = "Message")]
    public class Message
    {
        [XmlElement(ElementName = "Smstat")]
        public Smstat Status { get; set; }

        [XmlElement(ElementName = "Index")]
        public int Index { get; set; }

        [XmlElement(ElementName = "Phone")]
        public string Phone { get; set; }

        [XmlElement(ElementName = "Content")]
        public string Content { get; set; }

        [XmlElement(ElementName = "Date")]
        public string Date { get; set; }

        [XmlElement(ElementName = "Sca")]
        public string Sca { get; set; }

        [XmlElement(ElementName = "SaveType")]
        public int SaveType { get; set; }

        [XmlElement(ElementName = "Priority")]
        public int Priority { get; set; }

        [XmlElement(ElementName = "SmsType")]
        public int SmsType { get; set; }
        
        public enum Smstat
        {
            [XmlEnum(Name = "0")]
            Unread,
            [XmlEnum(Name = "1")]
            Read,
            [XmlEnum(Name = "2")]
            NotSent,
            [XmlEnum(Name = "3")]
            Sent,
            [XmlEnum(Name = "4")]
            Failed
        }
    }
}
