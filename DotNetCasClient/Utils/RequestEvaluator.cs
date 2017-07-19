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
using System.Globalization;
using System.Web;

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// A utility class for evaluating the type of request 
    /// </summary>
    /// <author>Scott Holodak</author>
    internal static class RequestEvaluator
    {
        /// <summary>
        /// Determines whether the request has a CAS ticket in the URL
        /// </summary>
        /// <returns>True if the request URL contains a CAS ticket, otherwise False</returns>
        internal static bool GetRequestHasCasTicket()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            bool result =
            (
                request[CasAuthentication.TicketValidator.ArtifactParameterName] != null &&
                !String.IsNullOrEmpty(request[CasAuthentication.TicketValidator.ArtifactParameterName])
            );

            return result;
        }

        /// <summary>
        /// Determines whether the request is a return request from the 
        /// CAS server containing a CAS ticket
        /// </summary>
        /// <returns>True if the request URL contains a CAS ticket, otherwise False</returns>
        internal static bool GetRequestIsCasAuthenticationResponse()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            int artifactIndex = request.Url.AbsoluteUri.IndexOf(CasAuthentication.TicketValidator.ArtifactParameterName);

            bool result =
            (
                GetRequestHasCasTicket() && 
                artifactIndex > 0 && 
                (
                    request.Url.AbsoluteUri[artifactIndex - 1] == '?' || 
                    request.Url.AbsoluteUri[artifactIndex - 1] == '&'
                )
            );

            return result;
        }

        /// <summary>
        /// Determines whether the request contains the GatewayParameterName defined in 
        /// web.config or the default value 'gatewayResponse'
        /// </summary>
        /// <returns>True if the request contains the GatewayParameterName, otherwise False</returns>
        internal static bool GetRequestHasGatewayParameter()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            bool requestContainsGatewayParameter = !String.IsNullOrEmpty(request.QueryString[CasAuthentication.GatewayParameterName]);
            bool gatewayParameterValueIsTrue = (request.QueryString[CasAuthentication.GatewayParameterName] == "true");

            bool result =
            (
               requestContainsGatewayParameter &&
               gatewayParameterValueIsTrue
            );

            return result;
        }

        /// <summary>
        /// Determines whether the request is an inbound proxy callback verifications 
        /// from the CAS server
        /// </summary>
        /// <returns>True if the request is a proxy callback verificiation, otherwise False</returns>
        internal static bool GetRequestIsProxyResponse()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            bool requestContainsProxyCallbackParameter = !String.IsNullOrEmpty(request.QueryString[CasAuthentication.ProxyCallbackParameterName]);
            bool proxyCallbackParameterValueIsTrue = (request.QueryString[CasAuthentication.ProxyCallbackParameterName] == "true");

            bool result =
            (
               requestContainsProxyCallbackParameter &&
               proxyCallbackParameterValueIsTrue
            );

            return result;            
        }

        /// <summary>
        /// Determines whether the current request requires a Gateway authentication redirect
        /// </summary>
        /// <returns>True if the request requires Gateway authentication, otherwise False</returns>
        internal static bool GetRequestRequiresGateway()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            GatewayStatus status = CasAuthentication.GetGatewayStatus();

            bool gatewayEnabled = CasAuthentication.Gateway;
            bool gatewayWasNotAttempted = (status == GatewayStatus.NotAttempted);
            bool requestDoesNotHaveGatewayParameter = !GetRequestHasGatewayParameter();
            bool cookiesRequiredUrlIsDefined = !string.IsNullOrEmpty(CasAuthentication.CookiesRequiredUrl);
            bool requestIsNotCookiesRequiredUrl = !GetRequestIsCookiesRequiredUrl();
            bool notAuthorizedUrlIsDefined = !String.IsNullOrEmpty(CasAuthentication.NotAuthorizedUrl);
            bool requestIsNotAuthorizedUrl = notAuthorizedUrlIsDefined && request.RawUrl.StartsWith(UrlUtil.ResolveUrl(CasAuthentication.NotAuthorizedUrl), true, CultureInfo.InvariantCulture);

            bool result =
            (
                gatewayEnabled &&
                gatewayWasNotAttempted &&
                requestDoesNotHaveGatewayParameter &&
                cookiesRequiredUrlIsDefined &&
                requestIsNotCookiesRequiredUrl &&
                !requestIsNotAuthorizedUrl
            );

            return result;
        }

        /// <summary>
        /// Determines whether the user's browser refuses to accept session cookies
        /// </summary>
        /// <returns>True if the browser does not allow session cookies, otherwise False</returns>
        internal static bool GetUserDoesNotAllowSessionCookies()
        {
            CasAuthentication.Initialize();

            // If the request has a gateway parameter but the cookie does not
            // reflect the fact that gateway was attempted, then cookies must
            // be disabled.
            GatewayStatus status = CasAuthentication.GetGatewayStatus();

            bool gatewayEnabled = CasAuthentication.Gateway;
            bool gatewayWasNotAttempted = (status == GatewayStatus.NotAttempted);
            bool requestHasGatewayParameter = GetRequestHasGatewayParameter();
            bool cookiesRequiredUrlIsDefined = !string.IsNullOrEmpty(CasAuthentication.CookiesRequiredUrl);
            bool requestIsNotCookiesRequiredUrl = cookiesRequiredUrlIsDefined && !GetRequestIsCookiesRequiredUrl();

            bool result =
            (
                gatewayEnabled &&
                gatewayWasNotAttempted &&
                requestHasGatewayParameter &&
                requestIsNotCookiesRequiredUrl
            );

            return result;
        }

        /// <summary>
        /// Determines whether the current request is unauthorized
        /// </summary>
        /// <returns>True if the request is unauthorized, otherwise False</returns>
        internal static bool GetRequestIsUnauthorized()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            bool responseIsBeingRedirected = (response.StatusCode == 302);
            bool userIsAuthenticated = GetUserIsAuthenticated();
            bool responseIsCasLoginRedirect = GetResponseIsCasLoginRedirect();

            bool result =
            (
               responseIsBeingRedirected &&
               userIsAuthenticated &&
               responseIsCasLoginRedirect
            );

            return result;
        }

        /// <summary>
        /// Determines whether the current request is unauthenticated
        /// </summary>
        /// <returns>True if the request is unauthenticated, otherwise False</returns>
        internal static bool GetRequestIsUnAuthenticated()
        {
            CasAuthentication.Initialize();

            bool userIsNotAuthenticated = !GetUserIsAuthenticated();
            bool responseIsCasLoginRedirect = GetResponseIsCasLoginRedirect();

            bool result =
            (
                userIsNotAuthenticated &&
                responseIsCasLoginRedirect
            );

            return result;
        }

        /// <summary>
        /// Determines whether the current request will be redirected to the 
        /// CAS login page
        /// </summary>
        /// <returns>True if the request will be redirected, otherwise False.</returns>
        private static bool GetResponseIsCasLoginRedirect()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            bool requestDoesNotHaveCasTicket = !GetRequestHasCasTicket();
            bool responseIsBeingRedirected = (response.StatusCode == 302);
            bool responseRedirectsToFormsLoginUrl = !String.IsNullOrEmpty(response.RedirectLocation) && response.RedirectLocation.StartsWith(CasAuthentication.FormsLoginUrl);

            bool result =
            (
               requestDoesNotHaveCasTicket &&
               responseIsBeingRedirected &&
               responseRedirectsToFormsLoginUrl
            );

            return result;
        }

        /// <summary>
        /// Determines whether the request is a CAS Single Sign Out request
        /// </summary>
        /// <returns>True if the request is a CAS Single Sign Out request, otherwise False</returns>
        internal static bool GetRequestIsCasSingleSignOut()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            bool requestIsFormPost = (request.RequestType == "POST");
            bool haveLogoutRequest = !string.IsNullOrEmpty(request.Params["logoutRequest"]);

            bool result =
            (
                requestIsFormPost &&
                haveLogoutRequest
            );

            return result;
        }

        /// <summary>
        /// Determines whether the User associated with the request has been 
        /// defined and is authenticated.
        /// </summary>
        /// <returns>True if the request has an authenticated User, otherwise False</returns>
        private static bool GetUserIsAuthenticated()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;

            bool result =
            (
               context.User != null &&
               context.User.Identity.IsAuthenticated
            );

            return result;
        }

        /// <summary>
        /// Determines whether the request is for the CookiesRequiredUrl defined in web.config
        /// </summary>
        /// <returns>True if the request is to the CookiesRequiredUrl, otherwise False</returns>
        private static bool GetRequestIsCookiesRequiredUrl()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            bool cookiesRequiredUrlIsDefined = !String.IsNullOrEmpty(CasAuthentication.CookiesRequiredUrl);
            bool requestIsCookiesRequiredUrl = cookiesRequiredUrlIsDefined && request.RawUrl.StartsWith(UrlUtil.ResolveUrl(CasAuthentication.CookiesRequiredUrl), true, CultureInfo.InvariantCulture);

            bool result =
            (
                requestIsCookiesRequiredUrl
            );

            return result;
        }

        /// <summary>
        /// Determines whether the request is appropriate for CAS authentication.
        /// Generally, this is true for most requests except those for images,
        /// style sheets, javascript files and anything generated by the built-in
        /// ASP.NET handlers (i.e., web resources, trace handler).
        /// </summary>
        /// <returns>True if the request is appropriate for CAS authentication, otherwise False</returns>
        internal static bool GetRequestIsAppropriateForCasAuthentication()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            string contentType = response.ContentType;
            string fileName = request.Url.Segments[request.Url.Segments.Length - 1];

            bool contentTypeIsEligible = false;
            bool fileNameIsEligible = true;

            if (string.IsNullOrEmpty(contentType) && CasAuthentication.RequireCasForMissingContentTypes)
            {
                contentTypeIsEligible = true;
            }

            if (!contentTypeIsEligible)
            {
                foreach (string appropriateContentType in CasAuthentication.RequireCasForContentTypes)
                {
                    if (string.Compare(contentType, appropriateContentType, true, CultureInfo.InvariantCulture) == 0)
                    {
                        contentTypeIsEligible = true;
                        break;
                    }
                }
            }

            foreach (string builtInHandler in CasAuthentication.BypassCasForHandlers)
            {
                if (string.Compare(fileName, builtInHandler, true, CultureInfo.InvariantCulture) == 0)
                {
                    fileNameIsEligible = false;
                    break;
                }
            }

            return (contentTypeIsEligible && fileNameIsEligible);
        }        
    }
}
