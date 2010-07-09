/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion.Condition;
using DotNetCasClient.Validation.Schema.Saml20.Assertion.Statement;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("Assertion", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class AssertionType {
        public NameIdType Issuer
        {
            get;
            set;
        }

        [XmlElement(Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get;
            set;
        }

        public SubjectType Subject
        {
            get;
            set;
        }

        public ConditionsType Conditions
        {
            get;
            set;
        }

        public AdviceType Advice
        {
            get;
            set;
        }

        [XmlElement("AttributeStatement", typeof(AttributeStatementType))]
        [XmlElement("AuthnStatement", typeof(AuthnStatementType))]
        [XmlElement("AuthzDecisionStatement", typeof(AuthzDecisionStatementType))]
        [XmlElement("Statement", typeof(StatementAbstractType))]
        public StatementAbstractType[] Items
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Version
        {
            get;
            set;
        }

        [XmlAttribute("ID", DataType="ID")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime IssueInstant
        {
            get;
            set;
        }
    }
}