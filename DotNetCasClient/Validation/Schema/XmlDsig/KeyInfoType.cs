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
    [XmlRoot("KeyInfo", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class KeyInfoType {
        [XmlAnyElement]
        [XmlElement("KeyName", typeof(string))]
        [XmlElement("KeyValue", typeof(KeyValueType))]
        [XmlElement("MgmtData", typeof(string))]
        [XmlElement("PGPData", typeof(PgpDataType))]
        [XmlElement("RetrievalMethod", typeof(RetrievalMethodType))]
        [XmlElement("SPKIData", typeof(SpkiDataType))]
        [XmlElement("X509Data", typeof(X509DataType))]
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

        [XmlText]
        public string[] Text
        {
            get;
            set;
        }

        [XmlAttribute(DataType="ID")]
        public string Id
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

            [XmlEnum("KeyName")]
            KeyName,

            [XmlEnum("KeyValue")]
            KeyValue,

            [XmlEnum("MgmtData")]
            MgmtData,

            [XmlEnum("PGPData")]
            PgpData,

            [XmlEnum("RetrievalMethod")]
            RetrievalMethod,

            [XmlEnum("SPKIData")]
            SpkiData,

            [XmlEnum("X509Data")]
            X509Data,
        }
    }
}

#pragma warning restore 1591