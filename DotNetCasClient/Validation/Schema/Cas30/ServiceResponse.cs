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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Cas30
{
    /// <summary>
    /// Represents a response from the CAS server's ticket validation endpoint, which could be any
    /// of the following types: authentication failure, authentication success, proxy failure, or
    /// proxy success.
    /// </summary>
    /// <authors>Blair Allen</authors>
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.yale.edu/tp/cas")]
    [XmlRoot("serviceResponse", Namespace = "http://www.yale.edu/tp/cas", IsNullable = false)]
    public class ServiceResponse
    {
        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal ServiceResponse() { }

        /// <summary>
        /// Parses the specified XML response from the CAS server and deserializes it into one of:
        /// <see cref="AuthenticationFailure"/>, <see cref="AuthenticationSuccess"/>,
        /// <see cref="ProxyFailure"/>, or <see cref="ProxySuccess"/>.
        /// </summary>
        /// <param name="responseXml">The XML response from the CAS server</param>
        /// <returns>
        /// An instance of <see cref="ServiceResponse"/> that contains deserialized XML response
        /// </returns>
        public static ServiceResponse ParseResponse(string responseXml)
        {
            var xs = new XmlSerializer(typeof(ServiceResponse));
            using (var sr = new StringReader(responseXml))
            {
                // Deserialize the XML response
                return (ServiceResponse) xs.Deserialize(sr);
            }
        }

        /// <summary>
        /// The deserialized XML response from the CAS server, which could be an instance of:
        /// <see cref="AuthenticationFailure"/>, <see cref="AuthenticationSuccess"/>,
        /// <see cref="ProxyFailure"/>, or <see cref="ProxySuccess"/>.
        /// </summary>
        [XmlElement("authenticationFailure", typeof(AuthenticationFailure))]
        [XmlElement("authenticationSuccess", typeof(AuthenticationSuccess))]
        [XmlElement("proxyFailure", typeof(ProxyFailure))]
        [XmlElement("proxySuccess", typeof(ProxySuccess))]
        public object Item
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether or not the <see cref="Item"/> property is an instance of <see
        /// cref="AuthenticationFailure"/>.
        /// </summary>
        [XmlIgnore]
        public bool IsAuthenticationFailure
        {
            get
            {
                return (Item != null && Item is AuthenticationFailure);
            }
        }

        /// <summary>
        /// Indicates whether or not the <see cref="Item"/> property is an instance of <see
        /// cref="AuthenticationSuccess"/>.
        /// </summary>
        [XmlIgnore]
        public bool IsAuthenticationSuccess
        {
            get
            {
                return (Item != null && Item is AuthenticationSuccess);
            }
        }

        /// <summary>
        /// Indicates whether or not the <see cref="Item"/> property is an instance of <see
        /// cref="ProxyFailure"/>.
        /// </summary>
        [XmlIgnore]
        public bool IsProxyFailure
        {
            get
            {
                return (Item != null && Item is ProxyFailure);
            }
        }

        /// <summary>
        /// Indicates whether or not the <see cref="Item"/> property is an instance of <see
        /// cref="ProxySuccess"/>.
        /// </summary>
        [XmlIgnore]
        public bool IsProxySuccess
        {
            get
            {
                return (Item != null && Item is ProxySuccess);
            }
        }
    }
}

#pragma warning restore 1591
