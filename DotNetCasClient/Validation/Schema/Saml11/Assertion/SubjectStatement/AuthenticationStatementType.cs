/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion.SubjectStatement
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("AuthenticationStatement", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class AuthenticationStatementType : SubjectStatementAbstractType {
        [XmlElement]
        public SubjectLocalityType SubjectLocality
        {
            get;
            set;
        }

        [XmlElement("AuthorityBinding")]
        public AuthorityBindingType[] AuthorityBinding
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string AuthenticationMethod
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime AuthenticationInstant
        {
            get;
            set;
        }
    }
}