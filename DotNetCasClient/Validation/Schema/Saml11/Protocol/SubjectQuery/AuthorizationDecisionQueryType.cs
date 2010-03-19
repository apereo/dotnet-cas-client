using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml11.Assertion;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol.SubjectQuery
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:protocol")]
    [XmlRoot("AuthorizationDecisionQuery", Namespace="urn:oasis:names:tc:SAML:1.0:protocol", IsNullable=false)]
    public class AuthorizationDecisionQueryType : SubjectQueryAbstractType {
        [XmlElement("Action", Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
        public ActionType[] Action
        {
            get;
            set;
        }

        [XmlArray(Namespace="urn:oasis:names:tc:SAML:1.0:assertion"), XmlArrayItem("Assertion", typeof(AssertionType), IsNullable=false), XmlArrayItem("AssertionIDReference", typeof(string), DataType="NCName", IsNullable=false)]
        public object[] Evidence
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Resource
        {
            get;
            set;
        }
    }
}