/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion.Statement
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("AuthnStatement", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class AuthnStatementType : StatementAbstractType {
        public SubjectLocalityType SubjectLocality
        {
            get;
            set;
        }

        public AuthnContextType AuthnContext
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime AuthnInstant
        {
            get;
            set;
        }

        [XmlAttribute]
        public string SessionIndex
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime SessionNotOnOrAfter
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool SessionNotOnOrAfterSpecified
        {
            get;
            set;
        }
    }
}