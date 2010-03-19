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
    [XmlRoot("SubjectLocality", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class SubjectLocalityType {
        [XmlAttribute]
        public string Address
        {
            get;
            set;
        }

        [XmlAttribute("DNSName")]
        public string DnsName
        {
            get;
            set;
        }
    }
}