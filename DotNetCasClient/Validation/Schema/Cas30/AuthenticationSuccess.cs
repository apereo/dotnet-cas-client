/*
 * Licensed to Apereo under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Apereo licenses this file to you under the Apache License,
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

namespace DotNetCasClient.Validation.Schema.Cas30
{
    /// <summary>
    /// Represents a successful authentication response from the CAS server's ticket validation endpoint.
    /// </summary>
    /// <author>Blair Allen</author>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.yale.edu/tp/cas")]
    public class AuthenticationSuccess
    {
        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal AuthenticationSuccess() { }

        /// <summary>
        /// The authenticated user's name/ID.
        /// </summary>
        [XmlElement("user")]
        public string User
        {
            get;
            set;
        }

        /// <summary>
        /// The identifier of the proxy granting ticket.
        /// </summary>
        [XmlElement("proxyGrantingTicket")]
        public string ProxyGrantingTicket
        {
            get;
            set;
        }

        /// <summary>
        /// A list of proxies that are available.
        /// </summary>
        [XmlArray("proxies")]
        [XmlArrayItem("proxy", IsNullable = false)]
        public string[] Proxies
        {
            get;
            set;
        }

        /// <summary>
        /// The &lt;cas:attributes&gt; XML element section from CAS server's service response.
        /// </summary>
        /// <remarks>
        /// This property should not be consumed outside of the CAS v3.0 service ticket validator.
        /// It is a director accessor to the XML element for CAS attributes that is needed for
        /// automatic XML deserialization. For access to the extra CAS attributes returned in the
        /// response, use the <see cref="Attributes"/> property instead.
        /// </remarks>
        [XmlAnyElement("attributes")]
        public XmlElement AttributesXmlElements { get; set; }

        /// <summary>
        /// The list of CAS attributes provided by the server for the user.
        /// </summary>
        [XmlIgnore]
        public IDictionary<string, IList<string>> Attributes
		{
            get
            {
                if (AttributesXmlElements == null)
                {
                    // No CAS attributes were found in the response XML, so return an empty
                    // dictionary
                    return new Dictionary<string, IList<string>>();
                }
                else
                {
                    // Build a new dictionary of key/values pairs
                    var attributes = new Dictionary<string, IList<string>>();
                    foreach (XmlNode child in AttributesXmlElements.ChildNodes)
                    {
                        string key = child.LocalName;
                        if (!attributes.ContainsKey(key))
                        {
                            // The dictionary currently does not have the attribute key, so add it
                            // using an empty list of values
                            attributes.Add(key, new List<string>());
                        }
                        // Add the new value to the list for the given attribute key
                        attributes[key].Add(child.InnerText);
                    }

                    return attributes;
                }
            }
		}
    }
}

#pragma warning restore 1591
