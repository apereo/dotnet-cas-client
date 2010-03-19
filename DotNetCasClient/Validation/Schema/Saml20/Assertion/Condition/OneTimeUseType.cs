using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion.Condition
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("OneTimeUse", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class OneTimeUseType : ConditionAbstractType {
    }
}