/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion.Attribute;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion.Statement
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("AttributeStatement", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class AttributeStatementType : StatementAbstractType {
        [XmlElement("Attribute", typeof(AttributeType)), XmlElement("EncryptedAttribute", typeof(EncryptedElementType))]
        public object[] Items
        {
            get;
            set;
        }
    }
}