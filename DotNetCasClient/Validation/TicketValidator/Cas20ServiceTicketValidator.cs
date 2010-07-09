/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.Web;
using DotNetCasClient.Security;
using DotNetCasClient.Utils;
using DotNetCasClient.Validation.Schema.Cas20;

namespace DotNetCasClient.Validation.TicketValidator
{
    /// <summary>
    /// CAS 2.0 Ticket Validator
    /// </summary>
    /// <remarks>
    /// This is the .Net port of org.jasig.cas.client.validation.Cas20ServiceTicketValidator.
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>Catherine D. Winfrey (.Net)</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    /// <author>Marvin S. Addison</author>
    /// <author>Scott Holodak (.Net)</author>
    class Cas20ServiceTicketValidator : AbstractUrlTicketValidator
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
        /// for the CAS 2.0 protocol.
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
                if (CasAuthentication.ProxyTicketManager != null)
                {
                    return "proxyValidate";
                }
                else
                {
                    return "serviceValidate";
                }
            }
        }

        public override void Initialize()
        {
            if (CasAuthentication.ProxyTicketManager != null)
            {
                CustomParameters.Add("pgtUrl", HttpUtility.UrlEncode(UrlUtil.ConstructProxyCallbackUrl()));
            }
        }

        /// <summary>
        /// Parses the response from the server into a CAS Assertion and includes this in
        /// a CASPrincipal.
        /// <remarks>
        /// Parsing of a &lt;cas:attributes&gt; element is <b>not</b> supported.  The official
        /// CAS 2.0 protocol does include this feature.  If attributes are needed,
        /// SAML must be used.
        /// </remarks>
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
            if (String.IsNullOrEmpty(response))
            {
                throw new TicketValidationException("CAS Server response was empty.");
            }

            ServiceResponse serviceResponse;
            try
            {
                serviceResponse = ServiceResponse.ParseResponse(response);
            }
            catch (InvalidOperationException)
            {
                throw new TicketValidationException("CAS Server response does not conform to CAS 2.0 schema");
            }
            
            if (serviceResponse.IsAuthenticationSuccess)
            {
                AuthenticationSuccess authSuccessResponse = (AuthenticationSuccess)serviceResponse.Item;

                if (String.IsNullOrEmpty(authSuccessResponse.User))
                {
                    throw new TicketValidationException(string.Format("CAS Server response parse failure: missing 'cas:user' element."));
                }

                string proxyGrantingTicketIou = authSuccessResponse.ProxyGrantingTicket;
                string proxyGrantingTicket = (CasAuthentication.ProxyTicketManager != null ? CasAuthentication.ProxyTicketManager.GetProxyGrantingTicket(proxyGrantingTicketIou) : null);
                
                if (CasAuthentication.ProxyTicketManager != null)
                {
                    CasAuthentication.ProxyTicketManager.InsertProxyGrantingTicketMapping(proxyGrantingTicketIou, proxyGrantingTicket);
                }

                if (authSuccessResponse.Proxies != null && authSuccessResponse.Proxies.Length > 0)
                {
                    return new CasPrincipal(new Assertion(authSuccessResponse.User), proxyGrantingTicketIou, authSuccessResponse.Proxies);
                } 
                else
                {
                    return new CasPrincipal(new Assertion(authSuccessResponse.User), proxyGrantingTicketIou);
                }
            }
            
            if (serviceResponse.IsAuthenticationFailure)
            {
                try
                {
                    AuthenticationFailure authFailureResponse = (AuthenticationFailure) serviceResponse.Item;
                    throw new TicketValidationException(authFailureResponse.Message, authFailureResponse.Code);
                }
                catch
                {
                    throw new TicketValidationException("CAS ticket could not be validated.");
                }
            }
            
            if (serviceResponse.IsProxySuccess)
            {
                throw new TicketValidationException("Unexpected service validate response: ProxySuccess");
            }

            if (serviceResponse.IsProxyFailure)
            {
                throw new TicketValidationException("Unexpected service validate response: ProxyFailure");
            }

            throw new TicketValidationException("Failed to validate CAS ticket.");
        }
    }
}