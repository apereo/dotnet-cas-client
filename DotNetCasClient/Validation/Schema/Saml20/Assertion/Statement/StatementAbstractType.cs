using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion.Statement
{
    [XmlInclude(typeof(AttributeStatementType))]
    [XmlInclude(typeof(AuthzDecisionStatementType))]
    [XmlInclude(typeof(AuthnStatementType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("Statement", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public abstract class StatementAbstractType {
    }
}