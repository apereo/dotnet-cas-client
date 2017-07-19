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
using DotNetCasClient.Validation.Schema.Saml11.Protocol.SubjectQuery;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol.Request
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:oasis:names:tc:SAML:1.0:protocol")]
    [XmlRoot("Request", Namespace = "urn:oasis:names:tc:SAML:1.0:protocol", IsNullable = false)]
    public class RequestType : RequestAbstractType
    {
        [XmlElement("AssertionIDReference", typeof(string), Namespace = "urn:oasis:names:tc:SAML:1.0:assertion", DataType = "NCName")]
        [XmlElement("AssertionArtifact", typeof(string))]
        [XmlElement("AttributeQuery", typeof(AttributeQueryType))]
        [XmlElement("AuthenticationQuery", typeof(AuthenticationQueryType))]
        [XmlElement("AuthorizationDecisionQuery", typeof(AuthorizationDecisionQueryType))]
        [XmlElement("Query", typeof(QueryAbstractType))]
        [XmlElement("SubjectQuery", typeof(SubjectQueryAbstractType))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
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

        [Serializable]
        [XmlType(Namespace = "urn:oasis:names:tc:SAML:1.0:protocol", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("urn:oasis:names:tc:SAML:1.0:assertion:AssertionIDReference")]
            AssertionIdReference,
            AssertionArtifact,
            AttributeQuery,
            AuthenticationQuery,
            AuthorizationDecisionQuery,
            Query,
            SubjectQuery,
        }
    }
}

#pragma warning restore 1591