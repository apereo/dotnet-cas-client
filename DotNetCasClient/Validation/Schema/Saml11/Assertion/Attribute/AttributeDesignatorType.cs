/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion.Attribute
{
    [XmlInclude(typeof(AttributeType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("AttributeDesignator", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class AttributeDesignatorType {
        [XmlAttribute]
        public string AttributeName
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string AttributeNamespace
        {
            get;
            set;
        }
    }
}