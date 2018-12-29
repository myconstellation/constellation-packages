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
    using System.Xml.Serialization;

    [StateObject, XmlRoot("response")]
    public class SessionInformations
    {
        [XmlElement("SesInfo")]
        public string SessionId { get; set; }

        [XmlElement("TokInfo")]
        public string TokenId { get; set; }
    }

    [StateObject, XmlRoot("response")]
    public class TokenInformations
    {
        [XmlElement("token")]
        public string TokenId { get; set; }
    }

    [XmlRoot("response")]
    public class ChallengeResponse
    {
        [XmlElement("servernonce")]
        public string ServerNonce { get; set; }
        [XmlElement("modeselected")]
        public int ModeSelected { get; set; }
        [XmlElement("salt")]
        public string Salt { get; set; }
        [XmlElement("iterations")]
        public int Iterations { get; set; }
    }

    [XmlRoot("response")]
    public class AuthentificationResponse
    {
        [XmlElement("rsan")]
        public string Rsan { get; set; }
        [XmlElement("rsae")]
        public string Rsae { get; set; }
        [XmlElement("serversignature")]
        public string ServerSignature { get; set; }
        [XmlElement("rsapubkeysignature")]
        public string RsaPublicKeySignature { get; set; }
    }

    [XmlRoot("response")]
    public class LoginState
    {
        [XmlElement("username")]
        public string Username { get; set; }
        [XmlElement("password_type")]
        public int PasswordType { get; set; }
        [XmlElement("firstlogin")]
        public int FirstLogin { get; set; }
        [XmlElement("history_login_flag")]
        public int HistoryLoginFlag { get; set; }
        [XmlElement("extern_password_type")]
        public int ExternPasswordType { get; set; }
        [XmlElement("State")]
        public int State { get; set; }
        [XmlElement("userlevel")]
        public string UserLevel { get; set; }
    }
}