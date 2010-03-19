using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion;
using DotNetCasClient.Validation.Schema.Saml20.Protocol.StatusResponse;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.Response
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(TypeName="ResponseType", Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("Response", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class ResponseType : StatusResponseType {
        [XmlElement("Assertion", typeof(AssertionType), Namespace="urn:oasis:names:tc:SAML:2.0:assertion"), XmlElement("EncryptedAssertion", typeof(EncryptedElementType), Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public object[] Items
        {
            get;
            set;
        }
    }
}