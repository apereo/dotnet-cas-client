/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Cas20
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.yale.edu/tp/cas")]
    public class AuthenticationFailure
    {
        internal AuthenticationFailure() { }

        [XmlAttribute("code")]
        public string Code
        {
            get;
            set;
        }

        [XmlText]
        public string Message
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool IsInvalidRequester
        {
            get
            {
                return String.Compare(Code, "INVALID_REQUEST", true, CultureInfo.InvariantCulture) == 0;
            }
        }

        [XmlIgnore]
        public bool IsInvalidTicket
        {
            get
            {
                return String.Compare(Code, "INVALID_TICKET", true, CultureInfo.InvariantCulture) == 0;
            }
        }

        [XmlIgnore]
        public bool IsInvalidService
        {
            get
            {
                return String.Compare(Code, "INVALID_SERVICE", true, CultureInfo.InvariantCulture) == 0;
            }
        }

        [XmlIgnore]
        public bool IsInternalError
        {
            get
            {
                return String.Compare(Code, "INTERNAL_ERROR", true, CultureInfo.InvariantCulture) == 0;
            }
        }
    }
}