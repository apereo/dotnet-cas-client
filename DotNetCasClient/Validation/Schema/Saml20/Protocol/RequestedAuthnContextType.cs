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
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("RequestedAuthnContext", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class RequestedAuthnContextType {
        [XmlElement("AuthnContextClassRef", typeof(string), Namespace="urn:oasis:names:tc:SAML:2.0:assertion", DataType="anyURI")]
        [XmlElement("AuthnContextDeclRef", typeof(string), Namespace="urn:oasis:names:tc:SAML:2.0:assertion", DataType="anyURI")]
        [XmlChoiceIdentifier("ItemsElementName")]
        public string[] Items
        {
            get;
            set;
        }

        [XmlElement("ItemsElementName"), XmlIgnore]
        public ItemsElementNames[] ItemsElementName
        {
            get;
            set;
        }

        [XmlAttribute]
        public AuthnContextComparisonType Comparison
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool ComparisonSpecified
        {
            get;
            set;
        }

        [Serializable]
        [XmlType(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextClassRef")]
            AuthnContextClassRef,

            [XmlEnum("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextDeclRef")]
            AuthnContextDeclRef,
        }
    }
}

#pragma warning restore 1591