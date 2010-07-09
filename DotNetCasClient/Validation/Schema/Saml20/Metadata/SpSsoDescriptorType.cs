/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("SPSSODescriptor", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class SpSsoDescriptorType : SsoDescriptorType {
        [XmlElement("AssertionConsumerService")]
        public IndexedEndpointType[] AssertionConsumerService
        {
            get;
            set;
        }

        [XmlElement("AttributeConsumingService")]
        public AttributeConsumingServiceType[] AttributeConsumingService
        {
            get;
            set;
        }

        [XmlAttribute]
        public bool AuthnRequestsSigned
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool AuthnRequestsSignedSpecified
        {
            get;
            set;
        }

        [XmlAttribute]
        public bool WantAssertionsSigned
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool WantAssertionsSignedSpecified
        {
            get;
            set;
        }
    }
}