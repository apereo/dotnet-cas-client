/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;

namespace DotNetCasClient.Configuration
{
    /// <summary>
    /// Generic exception to be thrown when Cas Client configuration fails.
    /// </summary>
    /// <author>Catherine Winfrey</author>
    public class CasConfigurationException : Exception
    {
        /// <summary>
        /// Constructs an exception with the supplied message.
        /// </summary>
        /// <param name="message">the message</param>
        public CasConfigurationException(string message) : base(message) { }

        /// <summary>
        /// Constructs an exception with the supplied message and chained exception
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="exception">the original exception</param>
        public CasConfigurationException(string message, Exception exception) : base(message, exception) { }
    }
}
