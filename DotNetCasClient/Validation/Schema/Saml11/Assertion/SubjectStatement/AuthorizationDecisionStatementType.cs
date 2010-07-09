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
    [XmlRoot("AuthorizationDecisionStatement", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class AuthorizationDecisionStatementType : SubjectStatementAbstractType {
        [XmlElement("Action")]
        public ActionType[] Action
        {
            get;
            set;
        }

        [XmlArray, XmlArrayItem("Assertion", typeof(AssertionType), IsNullable=false), XmlArrayItem("AssertionIDReference", typeof(string), DataType="NCName", IsNullable=false)]
        public object[] Evidence
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