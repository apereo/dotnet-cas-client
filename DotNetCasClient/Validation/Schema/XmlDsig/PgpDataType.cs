/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

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
    [XmlRoot("PGPData", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class PgpDataType {
        [XmlAnyElement]
        [XmlElement("PGPKeyID", typeof(byte[]), DataType="base64Binary")]
        [XmlElement("PGPKeyPacket", typeof(byte[]), DataType="base64Binary")]
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
        [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("##any:")]
            Item,

            [XmlEnum("PGPKeyID")]
            PgpKeyId,

            [XmlEnum("PGPKeyPacket")]
            PgpKeyPacket,
        }
    }
}