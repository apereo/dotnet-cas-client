/*
 * Licensed to Jasig under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Jasig licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Cas20
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.yale.edu/tp/cas")]
    public class AuthenticationSuccess
    {
        internal AuthenticationSuccess() { }

        [XmlElement("user")]
        public string User
        {
            get;
            set;
        }

        [XmlElement("attributes")]
        public Object AttributesNodes { get; set; }

        [XmlIgnore]
        public Dictionary<String, IList<String>> Attributes
        {
            get
            {
                var result = new Dictionary<String, IList<String>>();
                foreach (var element in AttributesNodes as IEnumerable<XmlNode>)
                {
                    if (!result.ContainsKey(element.Name))
                    {
                        result[element.Name] = new List<string>();
                    }
                    result[element.Name].Add(element.InnerText);
                }
                return result;
            }
        }

        [XmlElement("proxyGrantingTicket")]
        public string ProxyGrantingTicket
        {
            get;
            set;
        }

        [XmlArray("proxies")]
        [XmlArrayItem("proxy", IsNullable = false)]
        public string[] Proxies
        {
            get;
            set;
        }
    }
}

#pragma warning restore 1591