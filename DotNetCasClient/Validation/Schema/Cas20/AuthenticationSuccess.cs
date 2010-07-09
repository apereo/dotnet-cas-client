/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Cas20
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.yale.edu/tp/cas")]
    public class AuthenticationSuccess
    {
        internal AuthenticationSuccess() { }

        [XmlElement("user")]
        public string User
        {
            get;
            set;
        }

        [XmlElement("proxyGrantingTicket")]
        public string ProxyGrantingTicket
        {
            get;
            set;
        }

        [XmlArray("proxies")]
        [XmlArrayItem("proxy", IsNullable = false)]
        public string[] Proxies
        {
            get;
            set;
        }
    }
}


