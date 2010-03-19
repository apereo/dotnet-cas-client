using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.XmlDsig
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2000/09/xmldsig#")]
    [XmlRoot("RetrievalMethod", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class RetrievalMethodType {
        [XmlArray]
        [XmlArrayItem("Transform", IsNullable=false)]
        public TransformType[] Transforms
        {
            get;
            set;
        }

        [XmlAttribute("URI", DataType="anyURI")]
        public string Uri
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Type
        {
            get;
            set;
        }
    }
}