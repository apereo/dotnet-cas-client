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