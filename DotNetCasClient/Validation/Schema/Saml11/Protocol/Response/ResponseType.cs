using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml11.Assertion;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol.Response
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:protocol")]
    [XmlRoot("Response", Namespace="urn:oasis:names:tc:SAML:1.0:protocol", IsNullable=false)]
    public class ResponseType : ResponseAbstractType {
        [XmlElement]
        public StatusType Status
        {
            get;
            set;
        }

        [XmlElement("Assertion", Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
        public AssertionType[] Assertion
        {
            get;
            set;
        }
    }
}