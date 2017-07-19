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
using System.Diagnostics;

namespace DotNetCasClient.Logging
{
    /// <summary>
    /// Simple logger implementation that insulates application from details of logging framework/strategy.
    /// </summary>
    /// <author>Marvin S. Addison</author>
    public class Logger
    {
        private const int EVENT_ID = 0xCA5;
        private Category category;

        /// <summary>
        /// Creates a new logger for the given category.
        /// </summary>
        /// <param name="category">Logger category.</param>
        public Logger(Category category)
        {
            this.category = category;
        }

        /// <summary>
        /// Writes a debug/verbose message for the category owned by this logger.
        /// </summary>
        /// <param name="message">Log message; could be a formatted message.</param>
        /// <param name="parameters">Optional message format parameters.</param>
        public void Debug(string message, params object[] parameters)
        {
            Trace(TraceEventType.Verbose, message, parameters);
        }

        /// <summary>
        /// Writes an informative message for the category owned by this logger.
        /// </summary>
        /// <param name="message">Log message; could be a formatted message.</param>
        /// <param name="parameters">Optional message format parameters.</param>
        public void Info(string message, params object[] parameters)
        {
            Trace(TraceEventType.Information, message, parameters);
        }

        /// <summary>
        /// Writes a warning message for the category owned by this logger.
        /// </summary>
        /// <param name="message">Log message; could be a formatted message.</param>
        /// <param name="parameters">Optional message format parameters.</param>
        public void Warn(string message, params object[] parameters)
        {
            Trace(TraceEventType.Warning, message, parameters);
        }

        /// <summary>
        /// Writes an error message for the category owned by this logger.
        /// </summary>
        /// <param name="message">Log message; could be a formatted message.</param>
        /// <param name="parameters">Optional message format parameters.</param>
        public void Error(string message, params object[] parameters)
        {
            Trace(TraceEventType.Error, message, parameters);
        }

        private void Trace(TraceEventType type, string message, params object[] parameters)
        {
            if (category.Source.Switch.ShouldTrace(type))
            {
                category.Source.TraceEvent(type, EVENT_ID, message, parameters);
            }
        }
    }
}
