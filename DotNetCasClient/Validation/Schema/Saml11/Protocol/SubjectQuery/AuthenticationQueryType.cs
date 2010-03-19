using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol.SubjectQuery
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:protocol")]
    [XmlRoot("AuthenticationQuery", Namespace="urn:oasis:names:tc:SAML:1.0:protocol", IsNullable=false)]
    public class AuthenticationQueryType : SubjectQueryAbstractType {
        [XmlAttribute(DataType="anyURI")]
        public string AuthenticationMethod
        {
            get;
            set;
        }
    }
}