using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol.SubjectQuery
{
    [XmlInclude(typeof(SubjectQueryAbstractType))]
    [XmlInclude(typeof(AuthorizationDecisionQueryType))]
    [XmlInclude(typeof(AttributeQueryType))]
    [XmlInclude(typeof(AuthenticationQueryType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:protocol")]
    [XmlRoot("Query", Namespace="urn:oasis:names:tc:SAML:1.0:protocol", IsNullable=false)]
    public abstract class QueryAbstractType {
    }
}