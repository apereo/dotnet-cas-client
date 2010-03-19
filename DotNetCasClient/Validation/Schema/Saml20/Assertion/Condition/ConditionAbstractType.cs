using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion.Condition
{
    [XmlInclude(typeof(ProxyRestrictionType))]
    [XmlInclude(typeof(OneTimeUseType))]
    [XmlInclude(typeof(AudienceRestrictionType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("Condition", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public abstract class ConditionAbstractType {
    }
}