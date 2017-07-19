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

namespace DotNetCasClient.Validation.Schema.XmlDsig
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2000/09/xmldsig#")]
    [XmlRoot("X509Data", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class X509DataType {
        [XmlAnyElement]
        [XmlElement("X509CRL", typeof(byte[]), DataType="base64Binary")]
        [XmlElement("X509Certificate", typeof(byte[]), DataType="base64Binary")]
        [XmlElement("X509IssuerSerial", typeof(X509IssuerSerialType))]
        [XmlElement("X509SKI", typeof(byte[]), DataType="base64Binary")]
        [XmlElement("X509SubjectName", typeof(string))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get;
            set;
        }

        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsElementNames[] ItemsElementName
        {
            get;
            set;
        }

        [Serializable]
        [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("##any:")]
            Item,

            [XmlEnum("X509CRL")]
            X509Crl,

            [XmlEnum("X509Certificate")]
            X509Certificate,

            [XmlEnum("X509IssuerSerial")]
            X509IssuerSerial,

            [XmlEnum("X509SKI")]
            X509Ski,

            [XmlEnum("X509SubjectName")]
            X509SubjectName,
        }
    }
}

#pragma warning restore 1591