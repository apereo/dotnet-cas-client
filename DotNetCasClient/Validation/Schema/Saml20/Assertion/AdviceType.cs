/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("Advice", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class AdviceType {
        [XmlAnyElement]
        [XmlElement("Assertion", typeof(AssertionType))]
        [XmlElement("AssertionIDRef", typeof(string), DataType="NCName")]
        [XmlElement("AssertionURIRef", typeof(string), DataType="anyURI")]
        [XmlElement("EncryptedAssertion", typeof(EncryptedElementType))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get;
            set;
        }

        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsElementNames[] ItemsElementName
        {
            get;
            set;
        }

        [Serializable]
        [XmlType(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("##any:")]
            Item,

            [XmlEnum("Assertion")]
            Assertion,

            [XmlEnum("AssertionIDRef")]
            AssertionIdRef,

            [XmlEnum("AssertionURIRef")]
            AssertionUriRef,

            [XmlEnum("EncryptedAssertion")]
            EncryptedAssertion,
        }
    }
}