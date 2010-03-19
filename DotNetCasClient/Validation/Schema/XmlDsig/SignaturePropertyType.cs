using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.XmlDsig {
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2000/09/xmldsig#")]
    [XmlRoot("SignatureProperty", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class SignaturePropertyType {
        [XmlAnyElement]
        public XmlElement[] Items
        {
            get;
            set;
        }

        [XmlText]
        public string[] Text
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Target
        {
            get;
            set;
        }

        [XmlAttribute(DataType="ID")]
        public string Id
        {
            get;
            set;
        }
    }
}
