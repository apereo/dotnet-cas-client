/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Xenc
{
    [XmlInclude(typeof(EncryptedKeyType))]
    [XmlInclude(typeof(EncryptedDataType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    public abstract class EncryptedType {
        public EncryptionMethodType EncryptionMethod
        {
            get;
            set;
        }

        [XmlElement(Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfoType KeyInfo
        {
            get;
            set;
        }

        public CipherDataType CipherData
        {
            get;
            set;
        }

        public EncryptionPropertiesType EncryptionProperties
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

        [XmlAttribute(DataType="anyURI")]
        public string Type
        {
            get;
            set;
        }

        [XmlAttribute]
        public string MimeType
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Encoding
        {
            get;
            set;
        }
    }
}