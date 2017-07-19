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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.SoapEnvelope
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRoot(Namespace="http://schemas.xmlsoap.org/soap/envelope/", IsNullable=false)]
    public class Fault {
        [XmlElement("faultcode", Form = XmlSchemaForm.Unqualified)]
        public XmlQualifiedName FaultCode
        {
            get;
            set;
        }

        [XmlElement("faultstring", Form = XmlSchemaForm.Unqualified)]
        public string FaultString
        {
            get;
            set;
        }

        [XmlElement("faultactor", Form = XmlSchemaForm.Unqualified, DataType = "anyURI")]
        public string FaultActor
        {
            get;
            set;
        }

        [XmlElement("detail", Form = XmlSchemaForm.Unqualified)]
        public Detail Detail
        {
            get;
            set;
        }
    }
}

#pragma warning restore 1591