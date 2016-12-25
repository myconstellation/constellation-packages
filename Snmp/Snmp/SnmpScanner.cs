/*
 *	 SNMP Package for Constellation
 *	 Web site: http://www.myConstellation.io
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

namespace Snmp
{
    using global::System;
    using global::System.Collections;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Net;
    using global::System.Reflection;
    using SnmpSharpNet;

    /// <summary>
    /// Internal SNMP scanner
    /// </summary>
    internal class SnmpScanner
    {
        /// <summary>
        /// The SNMP default community
        /// </summary>
        public const string DEFAULT_COMMUNITY = "public";

        /// <summary>
        /// The root MIB-OID
        /// </summary>
        private const string ROOT_MIB_OID = ".1.3.6.1.2.1";

        /// <summary>
        /// Gets or sets the SNMP client.
        /// </summary>
        private SimpleSnmp Client { get; set; }

        /// <summary>
        /// Gets or sets the SNMP version.
        /// </summary>
        private SnmpVersion Version { get; set; }

        /// <summary>
        /// Checks the SNMP agent for the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="community">The community.</param>
        /// <returns><c>true</c> if the SNMP agent is valid, otherwise, <c>false</c></returns>
        public static bool CheckAgent(string host, string community = null)
        {
            return new SimpleSnmp(host, community ?? DEFAULT_COMMUNITY).Valid;
        }

        /// <summary>
        /// Scans the specified SNMP device.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="community">The community.</param>
        /// <param name="throwException">if set to <c>true</c> throw exception on error.</param>
        /// <returns>
        /// SNMP Device
        /// </returns>
        /// <exception cref="System.Exception">SNMP agent host name/ip address is invalid.</exception>
        public static SnmpDevice ScanDevice(string host, string community = null, bool throwException = true)
        {
            try
            {
                return Scan<SnmpDevice>(host, community);
            }
            catch
            {
                if (throwException)
                {
                    throw;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Scans the specified host.
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="host">The host.</param>
        /// <param name="community">The community.</param>
        /// <param name="oidRoot">The root OID.</param>
        /// <param name="instance">The result instance.</param>
        /// <returns>The T instance.</returns>
        /// <exception cref="System.Exception">SNMP agent host name/ip address is invalid.</exception>
        public static T Scan<T>(string host, string community = null, string oidRoot = null, T instance = null) where T : class, new()
        {
            return Scan<T>(new SimpleSnmp(host, community ?? DEFAULT_COMMUNITY), oidRoot, instance);
        }

        /// <summary>
        /// Scans the specified host.
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="snmpClient">The SNMP client.</param>
        /// <param name="oidRoot">The root OID.</param>
        /// <param name="instance">The result instance.</param>
        /// <param name="version">The SNMP version.</param>
        /// <returns>
        /// The T instance.
        /// </returns>
        /// <exception cref="System.Exception">SNMP agent host name/ip address is invalid.</exception>
        public static T Scan<T>(SimpleSnmp snmpClient, string oidRoot = null, T instance = null, SnmpVersion version = SnmpVersion.Ver1) where T : class, new()
        {
            if (!snmpClient.Valid)
            {
                throw new Exception("SNMP agent host name/ip address is invalid.");
            }
            if (oidRoot == null)
            {
                oidRoot = ROOT_MIB_OID;
            }
            if (instance == null)
            {
                instance = Activator.CreateInstance(typeof(T)) as T;
            }
            var scanner = new SnmpScanner()
            {
                Client = snmpClient,
                Version = version
            };
            scanner.FeedObject(instance, new Oid(oidRoot));
            return instance;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SnmpScanner"/> class from being created.
        /// </summary>
        private SnmpScanner()
        {
        }

        /// <summary>
        /// Feeds the object.
        /// </summary>
        /// <param name="targetInstance">The target instance.</param>
        /// <param name="rootId">The OID root identifier.</param>
        /// <param name="datas">The datas.</param>
        private void FeedObject(object targetInstance, Oid rootId, Dictionary<Oid, AsnType> datas = null)
        {
            bool snmpWalk = false;
            foreach (PropertyInfo property in targetInstance.GetType().GetProperties())
            {
                OIDAttribute propertyAttribute = property.GetCustomAttribute<OIDAttribute>();
                if (propertyAttribute != null)
                {
                    if (datas == null && !snmpWalk)
                    {
                        snmpWalk = true;
                        datas = this.Client.Walk(this.Version, propertyAttribute.ObjectId.ToString());
                    }
                    if (datas != null && datas.Count > 0)
                    {
                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Sequence<>))
                        {
                            object obj = property.GetValue(targetInstance);
                            if (obj == null)
                            {
                                obj = Activator.CreateInstance(property.PropertyType);
                                property.SetValue(targetInstance, obj);
                            }
                            FeedSequence(obj, property.PropertyType.GenericTypeArguments[0], propertyAttribute.ObjectId, datas.Where(i => propertyAttribute.ObjectId.IsRootOf(i.Key)).ToDictionary(k => k.Key, v => v.Value));
                        }
                        else if (property.PropertyType.GetCustomAttribute<SnmpObjectAttribute>() != null && property.PropertyType.IsClass)
                        {
                            object obj = property.GetValue(targetInstance);
                            if (obj == null)
                            {
                                obj = Activator.CreateInstance(property.PropertyType);
                                property.SetValue(targetInstance, obj);
                            }
                            FeedObject(obj, propertyAttribute.ObjectId, datas.Where(i => propertyAttribute.ObjectId.IsRootOf(i.Key)).ToDictionary(k => k.Key, v => v.Value));
                        }
                        else
                        {
                            AsnType asnValue = datas.Where(t => propertyAttribute.ObjectId == t.Key || propertyAttribute.ObjectId.IsRootOf(t.Key)).Select(p => p.Value).SingleOrDefault();
                            if (asnValue != null)
                            {
                                property.SetValue(targetInstance, GetValue(asnValue, property.PropertyType));
                            }
                            else if (property.GetCustomAttribute<RequiredAttribute>() != null)
                            {
                                throw new MissingMemberException(property.ReflectedType.Name, property.Name);
                            }
                        }
                    }
                    if (property.GetCustomAttribute<RequiredAttribute>() != null && property.PropertyType.IsClass && property.GetValue(targetInstance) == null)
                    {
                        throw new MissingMemberException(property.ReflectedType.Name, property.Name);
                    }
                }
                if (snmpWalk)
                {
                    snmpWalk = false;
                    datas = null;
                }
            }
        }

        /// <summary>
        /// Feeds the sequence.
        /// </summary>
        /// <param name="targetInstance">The target instance.</param>
        /// <param name="childrenType">Type of the children.</param>
        /// <param name="rootId">The OID root identifier.</param>
        /// <param name="datas">The datas.</param>
        private void FeedSequence(object targetInstance, Type childrenType, Oid rootId, Dictionary<Oid, AsnType> datas)
        {
            if (datas != null && datas.Count > 0)
            {
                foreach (var obj in datas)
                {
                    string instanceId = string.Join(".", Oid.GetChildIdentifiers(rootId, obj.Key).Skip(1));
                    IDictionary objects = targetInstance as IDictionary;
                    if (objects[instanceId] == null && childrenType.IsClass)
                    {
                        objects[instanceId] = Activator.CreateInstance(childrenType);
                    }
                    if (childrenType.IsClass)
                    {
                        FeedObject(objects[instanceId], obj.Key, datas.Where(i => obj.Key.IsRootOf(i.Key)).ToDictionary(k => k.Key, v => v.Value));
                    }
                    else
                    {
                        objects[instanceId] = GetValue(obj.Value, childrenType);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="asnValue">The ASN value.</param>
        /// <param name="targetType">Type of the value.</param>
        /// <returns>.NET value compliant</returns>
        private object GetValue(AsnType asnValue, Type targetType)
        {
            object value = asnValue;
            switch (SnmpConstants.GetTypeName(asnValue.Type))
            {
                case "Integer32":
                    value = ((Integer32)value).Value;
                    break;
                case "Gauge32":
                    value = ((Gauge32)value).Value;
                    break;
                case "Counter32":
                    value = ((Counter32)value).Value;
                    break;
                case "TimeTicks":
                    value = TimeSpan.FromMilliseconds(((TimeTicks)value).Milliseconds);
                    break;
                case "IPAddress":
                    value = IPAddress.Parse(((IpAddress)value).ToString());
                    break;
                case "OctetString":
                    value = asnValue.ToString();
                    if (targetType == typeof(DateTime))
                    {
                        value = ParseDateAndTime((string)value);
                    }
                    break;
            }
            return value;
        }

        /// <summary>
        /// Parses the SNMP DateTime.
        /// </summary>
        /// <param name="strValue">The string value.</param>
        /// <returns>.NET DateTime</returns>
        private DateTime ParseDateAndTime(string strValue)
        {
            string[] data = strValue.Split(' ');
            DateTime date = DateTime.MinValue;
            for (int idx = 1; idx < data.Length; idx++)
            {
                switch (idx)
                {
                    case 1:
                        date = date.AddYears(Convert.ToInt32(data[0] + data[1], 16) - 1);
                        break;
                    case 2:
                        date = date.AddMonths(Convert.ToInt32(data[idx], 16) - 1);
                        break;
                    case 3:
                        date = date.AddDays(Convert.ToInt32(data[idx], 16) - 1);
                        break;
                    case 4:
                        date = date.AddHours(Convert.ToInt32(data[idx], 16));
                        break;
                    case 5:
                        date = date.AddMinutes(Convert.ToInt32(data[idx], 16));
                        break;
                    case 6:
                        date = date.AddSeconds(Convert.ToInt32(data[idx], 16));
                        break;
                }
            }
            return date;
        }
    }
}
