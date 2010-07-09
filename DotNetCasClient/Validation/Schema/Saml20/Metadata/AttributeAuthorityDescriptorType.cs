/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion.Attribute;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("AttributeAuthorityDescriptor", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class AttributeAuthorityDescriptorType : RoleDescriptorType {
        [XmlElement("AttributeService")]
        public EndpointType[] AttributeService
        {
            get;
            set;
        }

        [XmlElement("AssertionIDRequestService")]
        public EndpointType[] AssertionIdRequestService
        {
            get;
            set;
        }

        [XmlElement("NameIDFormat", DataType="anyURI")]
        public string[] NameIdFormat
        {
            get;
            set;
        }

        [XmlElement("AttributeProfile", DataType="anyURI")]
        public string[] AttributeProfile
        {
            get;
            set;
        }

        [XmlElement("Attribute", Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public AttributeType[] Attribute
        {
            get;
            set;
        }
    }
}