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
using DotNetCasClient.Validation.Schema.Saml11.Assertion.Statement;
using DotNetCasClient.Validation.Schema.Saml11.Assertion.SubjectStatement;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("Assertion", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class AssertionType {
        [XmlElement]
        public ConditionsType Conditions
        {
            get;
            set;
        }

        [XmlArray, XmlArrayItem("", typeof(XmlElement), IsNullable=false), XmlArrayItem("Assertion", typeof(AssertionType), IsNullable=false), XmlArrayItem("AssertionIDReference", typeof(string), DataType="NCName", IsNullable=false)]
        public object[] Advice
        {
            get;
            set;
        }

        [XmlElement("AttributeStatement", typeof(AttributeStatementType)), XmlElement("AuthenticationStatement", typeof(AuthenticationStatementType)), XmlElement("AuthorizationDecisionStatement", typeof(AuthorizationDecisionStatementType)), XmlElement("Statement", typeof(StatementAbstractType)), XmlElement("SubjectStatement", typeof(SubjectStatementAbstractType))]
        public StatementAbstractType[] Items
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

        [XmlAttribute(DataType="integer")]
        public string MajorVersion
        {
            get;
            set;
        }

        [XmlAttribute(DataType="integer")]
        public string MinorVersion
        {
            get;
            set;
        }

        [XmlAttribute("AssertionID", DataType="ID")]
        public string AssertionId
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Issuer
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