using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml11.Assertion.SubjectStatement;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion.Statement
{
    [XmlInclude(typeof(SubjectStatementAbstractType))]
    [XmlInclude(typeof(AttributeStatementType))]
    [XmlInclude(typeof(AuthorizationDecisionStatementType))]
    [XmlInclude(typeof(AuthenticationStatementType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("Statement", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public abstract class StatementAbstractType {
    }
}