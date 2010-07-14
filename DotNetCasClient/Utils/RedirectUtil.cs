/*
 * Licensed to Jasig under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Jasig licenses this file to you under the Apache License,
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

using System.Web;

namespace DotNetCasClient.Utils
{
    internal static class RedirectUtil
    {
        public static void RedirectToLoginPage()
        {
            RedirectToLoginPage(CasAuthentication.Renew);
        }

        public static void RedirectToLoginPage(bool forceRenew)
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;
            HttpApplication application = context.ApplicationInstance;

            string redirectUrl = UrlUtil.ConstructLoginRedirectUrl(false, forceRenew);
            response.Redirect(redirectUrl, false);
            // application.CompleteRequest();
        }


        public static void RedirectToCookiesRequiredPage()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;
            HttpApplication application = context.ApplicationInstance;

            response.Redirect(UrlUtil.ResolveUrl(CasAuthentication.CookiesRequiredUrl), false);
            // application.CompleteRequest();
        }

        public static void RedirectToUnauthorizedPage()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;
            HttpApplication application = context.ApplicationInstance;

            response.Redirect(UrlUtil.ResolveUrl(CasAuthentication.NotAuthorizedUrl), false);
            // application.CompleteRequest();
        }

        internal static void RedirectFromLoginCallback()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            HttpApplication application = context.ApplicationInstance;

            if (RequestEvaluator.GetRequestHasGatewayParameter())
            {
                // TODO: Only set Success if request is authenticated?  Otherwise Failure.  
                // Doesn't make a difference from a security perspective, but may be clearer for users
                CasAuthentication.SetGatewayStatusCookie(GatewayStatus.Success);
            }

            response.Redirect(UrlUtil.RemoveCasArtifactsFromUrl(request.Url.AbsoluteUri), false);
            // application.CompleteRequest();
        }

        internal static void RedirectFromFailedGatewayCallback()
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            HttpApplication application = context.ApplicationInstance;

            CasAuthentication.SetGatewayStatusCookie(GatewayStatus.Failed);

            string urlWithoutCasArtifact = UrlUtil.RemoveCasArtifactsFromUrl(request.Url.AbsoluteUri);
            response.Redirect(urlWithoutCasArtifact, false);
            // application.CompleteRequest();
        }
    }
}
