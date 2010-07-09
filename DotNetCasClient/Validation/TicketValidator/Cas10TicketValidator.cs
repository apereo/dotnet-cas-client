/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System.IO;
using DotNetCasClient.Security;

namespace DotNetCasClient.Validation.TicketValidator
{
    /// <summary>
    /// CAS 1.0 Ticket Validator
    /// </summary>
    /// <remarks>
    /// This is the .Net port of org.jasig.cas.client.validation.Cas10TicketValidator
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    /// <author>Marvin S. Addison</author>
    class Cas10TicketValidator : AbstractUrlTicketValidator
    {
        /// <summary>
        /// The default name of the request parameter whose value is the artifact
        /// for the CAS 1.0 protocol.
        /// </summary>
        protected override string DefaultArtifactParameterName
        {
            get
            {
                return "ticket";
            }
        }

        /// <summary>
        /// The default name of the request parameter whose value is the service
        /// for the CAS 1.0 protocol.
        /// </summary>
        protected override string DefaultServiceParameterName
        {
            get
            {
                return "service";
            }
        }

        /// <summary>
        /// The endpoint of the validation URL.  Should be relative (i.e. not start with a "/").
        /// i.e. validate or serviceValidate.
        /// </summary>
        public override string UrlSuffix
        {
            get
            {
                return "validate";
            }
        }

        public override void Initialize()
        {
            // Do nothing
        }

        /// <summary>
        /// Parses the response from the server into a CAS Assertion and includes this in
        /// a CASPrincipal.
        /// </summary>
        /// <param name="response">the response from the server, in any format.</param>
        /// <param name="ticket">The ticket used to generate the validation response</param>
        /// <returns>
        /// a Principal backed by a CAS Assertion, if one could be created from the response.
        /// </returns>
        /// <exception cref="TicketValidationException">
        /// Thrown if creation of the Assertion fails.
        /// </exception>
        protected override ICasPrincipal ParseResponseFromServer(string response, string ticket)
        {
            if (response == null || !response.StartsWith("yes"))
            {
                throw new TicketValidationException("CAS Server could not validate ticket.");
            }

            try
            {
                StringReader reader = new StringReader(response);
                reader.ReadLine();
                string name = reader.ReadLine();
                return new CasPrincipal(new Assertion(name));
            }
            catch (IOException e)
            {
                throw new TicketValidationException("CAS Server response could not be parsed.", e);
            }
        }

    }
}


