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
    [XmlRoot("Transforms", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class TransformsType {
        [XmlElement("Transform")]
        public TransformType[] Transform
        {
            get;
            set;
        }
    }
}