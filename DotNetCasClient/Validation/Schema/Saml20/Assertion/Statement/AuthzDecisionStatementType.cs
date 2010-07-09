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
    [XmlRoot("AuthzDecisionStatement", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class AuthzDecisionStatementType : StatementAbstractType {
        [XmlElement("Action")]
        public ActionType[] Action
        {
            get;
            set;
        }

        public EvidenceType Evidence
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

        [XmlAttribute]
        public DecisionType Decision
        {
            get;
            set;
        }
    }
}