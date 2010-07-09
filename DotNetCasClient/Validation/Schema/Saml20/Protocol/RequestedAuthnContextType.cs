/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("RequestedAuthnContext", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class RequestedAuthnContextType {
        [XmlElement("AuthnContextClassRef", typeof(string), Namespace="urn:oasis:names:tc:SAML:2.0:assertion", DataType="anyURI")]
        [XmlElement("AuthnContextDeclRef", typeof(string), Namespace="urn:oasis:names:tc:SAML:2.0:assertion", DataType="anyURI")]
        [XmlChoiceIdentifier("ItemsElementName")]
        public string[] Items
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

        [XmlAttribute]
        public AuthnContextComparisonType Comparison
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool ComparisonSpecified
        {
            get;
            set;
        }

        [Serializable]
        [XmlType(Namespace = "urn:oasis:names:tc:SAML:2.0:protocol", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextClassRef")]
            AuthnContextClassRef,

            [XmlEnum("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextDeclRef")]
            AuthnContextDeclRef,
        }
    }
}