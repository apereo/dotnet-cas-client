using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml11.Protocol.SubjectQuery;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol.Request
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:oasis:names:tc:SAML:1.0:protocol")]
    [XmlRoot("Request", Namespace = "urn:oasis:names:tc:SAML:1.0:protocol", IsNullable = false)]
    public class RequestType : RequestAbstractType
    {
        [XmlElement("AssertionIDReference", typeof(string), Namespace = "urn:oasis:names:tc:SAML:1.0:assertion", DataType = "NCName")]
        [XmlElement("AssertionArtifact", typeof(string))]
        [XmlElement("AttributeQuery", typeof(AttributeQueryType))]
        [XmlElement("AuthenticationQuery", typeof(AuthenticationQueryType))]
        [XmlElement("AuthorizationDecisionQuery", typeof(AuthorizationDecisionQueryType))]
        [XmlElement("Query", typeof(QueryAbstractType))]
        [XmlElement("SubjectQuery", typeof(SubjectQueryAbstractType))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get;
            set;
        }

        [XmlElement("ItemsElementName"), XmlIgnore]
        public ItemsElementNames[] ItemsElementName
        {
            get;
            set;
        }

        [Serializable]
        [XmlType(Namespace = "urn:oasis:names:tc:SAML:1.0:protocol", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("urn:oasis:names:tc:SAML:1.0:assertion:AssertionIDReference")]
            AssertionIdReference,
            AssertionArtifact,
            AttributeQuery,
            AuthenticationQuery,
            AuthorizationDecisionQuery,
            Query,
            SubjectQuery,
        }
    }
}