using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol.Response
{
    [XmlInclude(typeof(ResponseType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:protocol")]
    public abstract class ResponseAbstractType {
        [XmlElement(Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get;
            set;
        }

        [XmlAttribute("ResponseID", DataType = "ID")]
        public string ResponseId
        {
            get;
            set;
        }

        [XmlAttribute(DataType="NCName")]
        public string InResponseTo
        {
            get;
            set;
        }

        [XmlAttribute(DataType="integer")]
        public string MajorVersion
        {
            get;
            set;
        }

        [XmlAttribute(DataType="integer")]
        public string MinorVersion
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime IssueInstant
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Recipient
        {
            get;
            set;
        }
    }
}