/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Xenc;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("EncryptedID", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class EncryptedElementType {
        [XmlElement(Namespace="http://www.w3.org/2001/04/xmlenc#")]
        public EncryptedDataType EncryptedData
        {
            get;
            set;
        }

        [XmlElement("EncryptedKey", Namespace="http://www.w3.org/2001/04/xmlenc#")]
        public EncryptedKeyType[] EncryptedKey
        {
            get;
            set;
        }
    }
}