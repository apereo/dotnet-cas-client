/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;

namespace DotNetCasClient.Validation
{
    /// <summary>
    /// Generic exception to be thrown when ticket validation fails.
    /// </summary>
    /// <remarks>
    /// This is the .Net port of org.jasig.cas.client.validation.TicketValidationException
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    /// <author>Scott Holodak (.Net)</author>
    public class TicketValidationException : Exception
    {       
        /// <summary>
        /// The error code contained in the CAS service response.
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs an exception with the supplied message.
        /// </summary>
        /// <param name="message">the message</param>
        public TicketValidationException(string message) : base(message) { }
        
        /// <summary>
        /// Constructs an exception with the supplied message and chained exception
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="exception">the original exception</param>
        public TicketValidationException(string message, Exception exception) : base(message, exception) { }

        /// <summary>
        /// Constructs an exception with the supplied message.
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="code">the CAS service response error code</param>
        public TicketValidationException(string message, string code)
            : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// Constructs an exception with the supplied message and chained exception
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="code">the CAS service response error code</param>
        /// <param name="exception">the original exception</param>
        public TicketValidationException(string message, string code, Exception exception) : base(message, exception)
        {
            Code = code;
        }
    }
}
