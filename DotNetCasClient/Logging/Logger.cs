using System;
using System.Diagnostics;

namespace DotNetCasClient.Logging
{
    /// <summary>
    /// Simple logger implementation that insulates application from details of logging framework/strategy.
    /// </summary>
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
