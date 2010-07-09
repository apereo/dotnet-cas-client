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
using DotNetCasClient.Validation.Schema.Saml20.Metadata;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion.Attribute
{
    [XmlInclude(typeof(RequestedAttributeType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("Attribute", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class AttributeType {
        [XmlElement("AttributeValue", IsNullable=true)]
        public object[] AttributeValue
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string NameFormat
        {
            get;
            set;
        }

        [XmlAttribute]
        public string FriendlyName
        {
            get;
            set;
        }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get;
            set;
        }
    }
}