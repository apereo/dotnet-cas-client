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
