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
    [XmlRoot("KeyInfo", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class KeyInfoType {
        [XmlAnyElement]
        [XmlElement("KeyName", typeof(string))]
        [XmlElement("KeyValue", typeof(KeyValueType))]
        [XmlElement("MgmtData", typeof(string))]
        [XmlElement("PGPData", typeof(PgpDataType))]
        [XmlElement("RetrievalMethod", typeof(RetrievalMethodType))]
        [XmlElement("SPKIData", typeof(SpkiDataType))]
        [XmlElement("X509Data", typeof(X509DataType))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
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

        [XmlText]
        public string[] Text
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

        [Serializable]
        [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("##any:")]
            Item,

            [XmlEnum("KeyName")]
            KeyName,

            [XmlEnum("KeyValue")]
            KeyValue,

            [XmlEnum("MgmtData")]
            MgmtData,

            [XmlEnum("PGPData")]
            PgpData,

            [XmlEnum("RetrievalMethod")]
            RetrievalMethod,

            [XmlEnum("SPKIData")]
            SpkiData,

            [XmlEnum("X509Data")]
            X509Data,
        }
    }
}