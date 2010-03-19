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
    [XmlRoot("Conditions", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class ConditionsType {
        [XmlElement("AudienceRestriction", typeof(AudienceRestrictionType)), XmlElement("Condition", typeof(ConditionAbstractType)), XmlElement("OneTimeUse", typeof(OneTimeUseType)), XmlElement("ProxyRestriction", typeof(ProxyRestrictionType))]
        public ConditionAbstractType[] Items
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime NotBefore
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool NotBeforeSpecified
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime NotOnOrAfter
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool NotOnOrAfterSpecified
        {
            get;
            set;
        }
    }
}