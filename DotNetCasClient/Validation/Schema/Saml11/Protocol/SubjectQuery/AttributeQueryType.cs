/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml11.Assertion.Attribute;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol.SubjectQuery
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:protocol")]
    [XmlRoot("AttributeQuery", Namespace="urn:oasis:names:tc:SAML:1.0:protocol", IsNullable=false)]
    public class AttributeQueryType : SubjectQueryAbstractType {
        [XmlElement("AttributeDesignator", Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
        public AttributeDesignatorType[] AttributeDesignator
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Resource
        {
            get;
            set;
        }
    }
}