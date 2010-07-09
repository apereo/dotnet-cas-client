/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml11.Assertion.Statement;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion.SubjectStatement
{
    [XmlInclude(typeof(AttributeStatementType))]
    [XmlInclude(typeof(AuthorizationDecisionStatementType))]
    [XmlInclude(typeof(AuthenticationStatementType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("SubjectStatement", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public abstract class SubjectStatementAbstractType : StatementAbstractType {
        [XmlElement]
        public SubjectType Subject
        {
            get;
            set;
        }
    }
}