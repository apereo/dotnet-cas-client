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

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:protocol")]
    [XmlRoot("StatusCode", Namespace="urn:oasis:names:tc:SAML:1.0:protocol", IsNullable=false)]
    public class StatusCodeType {
        [XmlElement]
        public StatusCodeType StatusCode
        {
            get;
            set;
        }

        [XmlAttribute]
        public XmlQualifiedName Value
        {
            get;
            set;
        }
    }
}