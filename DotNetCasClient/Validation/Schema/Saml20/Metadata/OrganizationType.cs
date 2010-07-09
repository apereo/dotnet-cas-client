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

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("Organization", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class OrganizationType {
        public ExtensionsType Extensions
        {
            get;
            set;
        }

        [XmlElement("OrganizationName")]
        public LocalizedNameType[] OrganizationName
        {
            get;
            set;
        }

        [XmlElement("OrganizationDisplayName")]
        public LocalizedNameType[] OrganizationDisplayName
        {
            get;
            set;
        }

        [XmlElement("OrganizationURL")]
        public LocalizedUriType[] OrganizationUrl
        {
            get;
            set;
        }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get;
            set;
        }
    }
}