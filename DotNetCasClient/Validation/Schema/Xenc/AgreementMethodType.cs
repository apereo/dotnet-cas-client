/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.XmlDsig;
    
namespace DotNetCasClient.Validation.Schema.Xenc {
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [XmlRoot("AgreementMethod", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public class AgreementMethodType {
        [XmlElement("KA-Nonce", DataType="base64Binary")]
        public byte[] KaNonce
        {
            get;
            set;
        }

        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any
        {
            get;
            set;
        }

        public KeyInfoType OriginatorKeyInfo
        {
            get;
            set;
        }

        public KeyInfoType RecipientKeyInfo
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Algorithm
        {
            get;
            set;
        }
    }            
}
