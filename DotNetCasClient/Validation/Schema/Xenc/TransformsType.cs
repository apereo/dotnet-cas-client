using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Xenc
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    public class TransformsType {
        [XmlElement("Transform", Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public TransformType[] Transform
        {
            get;
            set;
        }
    }
}