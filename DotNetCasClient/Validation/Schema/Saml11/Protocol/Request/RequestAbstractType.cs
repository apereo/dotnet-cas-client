/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol.Request
{
    [XmlInclude(typeof(RequestType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:protocol")]
    public abstract class RequestAbstractType {
        [XmlElement("RespondWith")]
        public XmlQualifiedName[] RespondWith
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

        [XmlAttribute("RequestID", DataType="ID")]
        public string RequestId
        {
            get;
            set;
        }

        [XmlAttribute(DataType="integer")]
        public string MajorVersion
        {
            get;
            set;
        }

        [XmlAttribute(DataType="integer")]
        public string MinorVersion
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
    }
}