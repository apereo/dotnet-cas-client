using System;

namespace JasigCasClient.Configuration
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
        public CasConfigurationException(string message, Exception exception)
          : base(message, exception) { }
    }
}
