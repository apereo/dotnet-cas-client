using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.SubjectQuery
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("AuthnQuery", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class AuthnQueryType : SubjectQueryAbstractType {
        public RequestedAuthnContextType RequestedAuthnContext
        {
            get;
            set;
        }

        [XmlAttribute]
        public string SessionIndex
        {
            get;
            set;
        }
    }
}