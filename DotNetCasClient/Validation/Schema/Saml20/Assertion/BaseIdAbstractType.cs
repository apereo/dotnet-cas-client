using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("BaseID", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public abstract class BaseIdAbstractType {
        [XmlAttribute]
        public string NameQualifier
        {
            get;
            set;
        }

        [XmlAttribute("SPNameQualifier")]
        public string SpNameQualifier
        {
            get;
            set;
        }
    }
}