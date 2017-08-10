/*
 * Licensed to Apereo under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Apereo licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
