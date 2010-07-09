/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.StatusResponse
{
    [XmlInclude(typeof(NameIdMappingResponseType))]
    [XmlInclude(typeof(ArtifactResponseType))]
    [XmlInclude(typeof(Response.ResponseType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("ManageNameIDResponse", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class StatusResponseType {
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

        public StatusType Status
        {
            get;
            set;
        }

        [XmlAttribute("Id", DataType="ID")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttribute(DataType="NCName")]
        public string InResponseTo
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