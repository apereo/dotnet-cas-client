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
    [XmlRoot("Subject", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class SubjectType {
        [XmlElement("NameIdentifier", typeof(NameIdentifierType)), XmlElement("SubjectConfirmation", typeof(SubjectConfirmationType))]
        public object[] Items
        {
            get;
            set;
        }
    }
}