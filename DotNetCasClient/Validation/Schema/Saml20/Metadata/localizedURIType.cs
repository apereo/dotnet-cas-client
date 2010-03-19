using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("OrganizationURL", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class LocalizedUriType {
        [XmlAttribute("lang", Form=XmlSchemaForm.Qualified, Namespace="http://www.w3.org/XML/1998/namespace")]
        public string Lang
        {
            get;
            set;
        }

        [XmlText(DataType="anyURI")]
        public string Value
        {
            get;
            set;
        }
    }
}