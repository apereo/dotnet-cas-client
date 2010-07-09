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

namespace DotNetCasClient.Validation.Schema.Xenc
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    public class EncryptionMethodType {
        [XmlElement(DataType="integer")]
        public string KeySize
        {
            get;
            set;
        }

        [XmlElement("OAEPparams", DataType="base64Binary")]
        public byte[] OaepParams
        {
            get;
            set;
        }

        [XmlText, XmlAnyElement]
        public XmlNode[] Any
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