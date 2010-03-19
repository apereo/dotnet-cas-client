using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Xenc
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true, Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [XmlRoot(Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public class ReferenceList {
        [XmlElement("DataReference", typeof(ReferenceType))]
        [XmlElement("KeyReference", typeof(ReferenceType))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public ReferenceType[] Items
        {
            get;
            set;
        }

        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsElementNames[] ItemsElementName
        {
            get;
            set;
        }

        [Serializable]
        [XmlType(Namespace = "http://www.w3.org/2001/04/xmlenc#", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            DataReference,
            KeyReference,
        }
    }
}