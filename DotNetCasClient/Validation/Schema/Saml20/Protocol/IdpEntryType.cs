/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("IDPEntry", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class IdpEntryType {
        [XmlAttribute("ProviderID", DataType="anyURI")]
        public string ProviderId
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Loc
        {
            get;
            set;
        }
    }
}