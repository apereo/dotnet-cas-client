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
    public class TicketValidationException : Exception
    {
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
    }
}
