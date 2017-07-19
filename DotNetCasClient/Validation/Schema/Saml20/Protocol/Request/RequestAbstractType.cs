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
using DotNetCasClient.Validation.Schema.Saml20.Assertion;
using DotNetCasClient.Validation.Schema.Saml20.Protocol.SubjectQuery;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.Request
{
    [XmlInclude(typeof(NameIdMappingRequestType))]
    [XmlInclude(typeof(LogoutRequestType))]
    [XmlInclude(typeof(ManageNameIdRequestType))]
    [XmlInclude(typeof(ArtifactResolveType))]
    [XmlInclude(typeof(AuthnRequestType))]
    [XmlInclude(typeof(SubjectQueryAbstractType))]
    [XmlInclude(typeof(AuthzDecisionQueryType))]
    [XmlInclude(typeof(AttributeQueryType))]
    [XmlInclude(typeof(AuthnQueryType))]
    [XmlInclude(typeof(AssertionIdRequestType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    public abstract class RequestAbstractType {
        [XmlElement(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public NameIdType Issuer
        {
            get;
            set;
        }

        [XmlElement(Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get;
            set;
        }

        public Extensions.ExtensionsType Extensions
        {
            get;
            set;
        }

        [XmlAttribute("ID", DataType="ID")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Version
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime IssueInstant
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Destination
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Consent
        {
            get;
            set;
        }
    }
}

#pragma warning restore 1591