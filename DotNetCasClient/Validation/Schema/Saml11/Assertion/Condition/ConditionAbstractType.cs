using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion
{
    [XmlInclude(typeof(DoNotCacheConditionType))]
    [XmlInclude(typeof(AudienceRestrictionConditionType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("Condition", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public abstract class ConditionAbstractType {
    }
}