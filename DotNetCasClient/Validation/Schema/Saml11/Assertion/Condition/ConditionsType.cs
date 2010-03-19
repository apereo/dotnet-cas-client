using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("Conditions", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class ConditionsType {
        [XmlElement("AudienceRestrictionCondition", typeof(AudienceRestrictionConditionType)), XmlElement("Condition", typeof(ConditionAbstractType)), XmlElement("DoNotCacheCondition", typeof(DoNotCacheConditionType))]
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