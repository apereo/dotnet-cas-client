/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.SessionState;
using DotNetCasClient.Security;
using log4net;

namespace DotNetCasClient.Utils
{
    public sealed class DebugUtils
    {
        /// <summary>
        /// Access to the log file
        /// </summary>
        static readonly ILog Log = LogManager.GetLogger("DebugUtils");

        public static string IPrincipalToString(IPrincipal principal)
        {
            if (principal == null)
            {
                return "NULL";
            }

            if (typeof(CasPrincipal) == principal.GetType())
            {
                ICasPrincipal casPrincipal = (ICasPrincipal)principal;

                return string.Format("Type>{0}< Identity[{1}] Assertion[{2}]",
                    principal.GetType().Name,
                    IIdentityToString(casPrincipal.Identity),
                    AssertionToString(casPrincipal.Assertion)
                );
            }

            return string.Format("Type>{0}< Identity[{1}]",
                principal.GetType().Name,
                IIdentityToString(principal.Identity)
            );
        }

        public static string IIdentityToString(IIdentity identity)
        {
            string identityDisplay = "NULL";
            if (identity != null)
            {
                identityDisplay = string.Format("Name>{0}< AuthenticationType>{1}< IsAuthenticated>{2}",
                    identity.Name, identity.AuthenticationType,
                    identity.IsAuthenticated
                );
            }
            return identityDisplay;
        }

        public static string AssertionToString(IAssertion assertion)
        {
            string assertionDisplay = "NULL";

            if (assertion != null)
            {
                assertionDisplay = string.Format("ValidFromDate>{0}< ValidUntilDate>{1}< PrincipalName>{2}< Attributes Count>{3}<",
                    assertion.ValidFromDate,
                    assertion.ValidUntilDate,
                    assertion.PrincipalName,
                    assertion.Attributes.Count
                );
            }

            return assertionDisplay;
        }

        public static string ContextToString(HttpContext context)
        {
            string contextDisplay = "NULL";
            if (context != null)
            {
                contextDisplay = IPrincipalToString(context.User);
            }

            return string.Format("Context.User[{0}]{1}             Thread.CurrentPrincipal[{2}]",
                  contextDisplay,
                  Environment.NewLine,
                  IPrincipalToString(System.Threading.Thread.CurrentPrincipal)
            );
        }

        public static string FormsAuthEventArgsToString(FormsAuthenticationEventArgs faa)
        {
            string principalDisplay = "UNDEFINED";
            if (faa != null && faa.User != null)
            {
                principalDisplay = IPrincipalToString(faa.User);
            }
            return string.Format("User[{0}]", principalDisplay);
        }

        public static string HttpResponseToString(HttpContext context)
        {
            string responseDisplay = "NULL";
            if (context != null)
            {
                HttpResponse response = context.Response;
                responseDisplay = string.Format("statusCode>{0}<", response.StatusCode);
            }
            return responseDisplay;
        }

        public static string HttpRequestToString(HttpContext context)
        {
            string requestDisplay = "NULL";
            if (context != null)
            {
                string fatCookieDisplay = FatCookieToString(context);
                requestDisplay = string.Format("{0}", fatCookieDisplay);
            }
            return requestDisplay;
        }

        public static string HttpSessionToString(HttpApplication application)
        {
            string contextSessionDisplay = "NULL";
            string applSessionDisplay;

            HttpContext context = application.Context;
            if (context != null)
            {
                try
                {
                    HttpSessionState session = context.Session;
                    contextSessionDisplay = string.Format("available--{0}", session.SessionID);
                }
                catch (Exception)
                {
                    contextSessionDisplay = "unavailable";
                }
            }

            try
            {
                HttpSessionState session = application.Session;
                applSessionDisplay = string.Format("available--{0}", session.SessionID);
            }
            catch (Exception)
            {
                applSessionDisplay = "unavailable";
            }

            return string.Format("contextSession[{0}] -- applicationSession[{1}]",
                contextSessionDisplay,
                applSessionDisplay
            );
        }

        public static string FatCookieToString(HttpContext context)
        {
            AuthenticationSection config = (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");

            string fatDisplay = "NULL";
            string cookieDisplay = "NULL";
            if (context != null && context.Request != null)
            {
                HttpCookie cookie = context.Request.Cookies[config.Forms.Name];
                cookieDisplay = CookieToString(cookie);
                if (cookie != null)
                {
                    try
                    {
                        FormsAuthenticationTicket fat = FormsAuthentication.Decrypt(cookie.Value);
                        fatDisplay = FormsAuthTicketToString(fat);
                    }
                    catch (ArgumentException)
                    {
                        fatDisplay = "DECRYPT EXCEPTION";
                    }
                }
            }

            return string.Format("authcookie [{0}] authticket [{1}]",
                cookieDisplay,
                fatDisplay
            );
        }

        public static string CookieToString(HttpCookie cookie)
        {
            string cookieDisplay = "NULL";
            if (cookie != null)
            {
                cookieDisplay = string.Format("Name>{0}< Expires>{1}<", cookie.Name, cookie.Expires);
            }
            return cookieDisplay;
        }

        public static string FormsAuthTicketToString(FormsAuthenticationTicket fat)
        {
            string fatDisplay = "NULL";
            if (fat != null)
            {
                fatDisplay = string.Format("name>{0}< userdata>{1}< issuedate>{2}< expiration>{3}< expired>{4}< ispersistent>{5}<",
                    fat.Name,
                    fat.UserData,
                    fat.IssueDate,
                    fat.Expiration,
                    fat.Expired,
                    fat.IsPersistent
                );
            }
            return fatDisplay;
        }

        public static string FormsAuthSummaryToString(HttpApplication app)
        {
            if (app == null)
            {
                return "NULL";
            }

            return string.Format("{0}Session: {1}  {2}",
                Environment.NewLine,
                HttpSessionToString(app),
                FormsAuthSummaryToString(app.Context)
            );
        }


        public static string FormsAuthSummaryToString(HttpContext context)
        {
            return string.Format("{0}Response: {1}{2}Context: {1}",
                Environment.NewLine,
                HttpResponseToString(context),
                ContextToString(context)
            );
        }

        public static string FormsAuthRequestSummaryToString(HttpApplication app)
        {
            if (app == null)
            {
                return "NULL";
            }

            return string.Format("{0}Session: {1}{2}",
                Environment.NewLine,
                HttpSessionToString(app),
                FormsAuthRequestSummaryToString(app.Context)
            );
        }

        public static string FormsAuthRequestSummaryToString(HttpContext context)
        {
            return string.Format("{0}Request: {1}{0}Response: {2}{0}Context: {3}",
                Environment.NewLine,
                HttpRequestToString(context),
                HttpResponseToString(context),
                ContextToString(context)
            );
        }

        public static string FormsAuthenticationToString()
        {
            return string.Format("{0}DefaultUrl>{1}<{0}FormsCookieName>{2}< {0}FormsCookiePath>{3}<{0}LoginUrl>{4}<{0}SlidingExpiration>{5}<",
                Environment.NewLine,
                FormsAuthentication.DefaultUrl,
                FormsAuthentication.FormsCookieName,
                FormsAuthentication.FormsCookiePath,
                FormsAuthentication.LoginUrl,
                FormsAuthentication.SlidingExpiration
            );
        }

        public static string DataNullEmptyDisplay(string source, string dataDelim, bool useDelimAlways)
        {
            string dataDelimStart = dataDelim;
            string dataDelimEnd = dataDelim;

            if (dataDelimStart.Equals(">"))
            {
                dataDelimEnd = "<";
            }

            if (source == null)
            {
                return dataDelim + "NULL" + dataDelimEnd;
            }
            else if (source.Length < 1)
            {
                return dataDelim + "EMPTY" + dataDelimEnd;
            }
            else if (useDelimAlways)
            {
                return dataDelim + source + dataDelimEnd;
            }
            else
            {
                return source;
            }
        }

        private static string GenerateLineIndent(string lineIndent, int lineIndentCount)
        {
            StringBuilder sb = new StringBuilder();
            string lineIndentToUse = lineIndent;
            if (String.IsNullOrEmpty(lineIndent) && lineIndentCount > 0)
            {
                lineIndentToUse = " ";
            }
            for (int i = 0; i < lineIndentCount; i++)
            {
                sb.Append(lineIndentToUse);
            }
            return sb.ToString();
        }

        public static string IPrincipalToString(IPrincipal principal, string newLineDelim, string initialLineIndent, int initialLineIndentCount, string dataDelim, bool useDelimAlways)
        {
            StringBuilder sb = new StringBuilder();
            string lineIndent = GenerateLineIndent(initialLineIndent, initialLineIndentCount);

            sb.AppendFormat("{0}User Information:{1}",
                lineIndent,
                newLineDelim
            );

            int memberLineIndentCount = initialLineIndentCount + 2;
            string memberLineIndent = GenerateLineIndent(initialLineIndent, memberLineIndentCount);

            if (principal == null)
            {
                sb.AppendFormat("{0}UNDEFINED{1}",
                    memberLineIndent,
                    newLineDelim
                );
            }
            else
            {
                sb.AppendFormat("{0}Type: {1}{2}", memberLineIndent, DataNullEmptyDisplay(principal.GetType().Name, dataDelim, useDelimAlways), newLineDelim);
                sb.Append(IIdentityToString(principal.Identity, newLineDelim, initialLineIndent, memberLineIndentCount, dataDelim, useDelimAlways));
                if (typeof(CasPrincipal) == principal.GetType())
                {
                    memberLineIndentCount += 2;
                    sb.Append(AssertionToString(((CasPrincipal)principal).Assertion, newLineDelim, initialLineIndent, memberLineIndentCount, dataDelim, useDelimAlways));
                }
            }

            return sb.ToString();
        }


        public static string IIdentityToString(IIdentity identity, string newLineDelim, string initialLineIndent, int initialLineIndentCount, string dataDelim, bool useDelimAlways)
        {
            StringBuilder sb = new StringBuilder();
            string lineIndent = GenerateLineIndent(initialLineIndent, initialLineIndentCount);
            sb.AppendFormat("{0}Identity:{1}", lineIndent, newLineDelim);
            int memberLineIndentCount = initialLineIndentCount + 2;
            string memberLineIndent = GenerateLineIndent(initialLineIndent, memberLineIndentCount);

            if (identity == null)
            {
                sb.AppendFormat("{0}UNDEFINED{1}", memberLineIndent, newLineDelim);
            }
            else
            {
                sb.AppendFormat("{0}Name: {1}{2}", memberLineIndent, DataNullEmptyDisplay(identity.Name, dataDelim, useDelimAlways), newLineDelim);
                sb.AppendFormat("{0}AuthenticationType: {1}{2}", memberLineIndent, DataNullEmptyDisplay(identity.AuthenticationType, dataDelim, useDelimAlways), newLineDelim);
                sb.AppendFormat("{0}IsAuthenticated: {1}{2}", memberLineIndent, identity.IsAuthenticated, newLineDelim);
            }
            return sb.ToString();
        }

        public static string AssertionToString(IAssertion assertion, string newLineDelim, string initialLineIndent, int initialLineIndentCount, string dataDelim, bool useDelimAlways)
        {
            StringBuilder sb = new StringBuilder();
            string lineIndent = GenerateLineIndent(initialLineIndent, initialLineIndentCount);
            sb.AppendFormat("{0}Assertion:{1}", lineIndent, newLineDelim);
            int memberLineIndentCount = initialLineIndentCount + 2;
            string memberLineIndent = GenerateLineIndent(initialLineIndent, memberLineIndentCount);

            if (assertion == null)
            {
                sb.AppendFormat("{0}UNDEFINED{1}",
                    memberLineIndent,
                    newLineDelim
                );
            }
            else
            {
                sb.AppendFormat("{0}ValidFromDate: {1}{2}",
                    memberLineIndent,
                    DataNullEmptyDisplay(assertion.ValidFromDate.ToString(), dataDelim, useDelimAlways),
                    newLineDelim
                );

                sb.AppendFormat("{0}ValidUntilDate: {1}{2}",
                    memberLineIndent,
                    DataNullEmptyDisplay(assertion.ValidUntilDate.ToString(), dataDelim, useDelimAlways),
                    newLineDelim
                );

                sb.AppendFormat("{0}PrincipalName: {1}{2}",
                    memberLineIndent,
                    DataNullEmptyDisplay(assertion.PrincipalName, dataDelim, useDelimAlways),
                    newLineDelim
                );

                sb.Append(AttributesToString(assertion.Attributes, newLineDelim, initialLineIndent, memberLineIndentCount, dataDelim, useDelimAlways));
            }
            return sb.ToString();
        }

        public static string AttributesToString(Dictionary<string, IList<string>> attributes, string newLineDelim, string initialLineIndent, int initialLineIndentCount, string dataDelim, bool useDelimAlways)
        {
            StringBuilder sb = new StringBuilder();
            string lineIndent = GenerateLineIndent(initialLineIndent, initialLineIndentCount);
            sb.AppendFormat("{0}Attributes:{1}", lineIndent, newLineDelim);
            string memberLineIndent = GenerateLineIndent(initialLineIndent, initialLineIndentCount + 2);

            if (attributes == null)
            {
                sb.AppendFormat("{0}UNDEFINED{1}", memberLineIndent, newLineDelim);
            }
            else
            {
                sb.AppendFormat("{0}Attributes Count: {1}{2}", memberLineIndent, attributes.Count, newLineDelim);
                foreach (KeyValuePair<string, IList<string>> entry in attributes)
                {
                    sb.AppendFormat("{0}Attribute Name: {1}{2}", memberLineIndent, entry.Key, newLineDelim);
                    int count = 0;
                    string valueLineIndent = GenerateLineIndent(initialLineIndent, initialLineIndentCount + 4);
                    foreach (string value in entry.Value)
                    {
                        count++;
                        sb.AppendFormat("{0}Value[{1}]: {2}{3}", valueLineIndent, count, value, newLineDelim);
                    }
                }
            }
            return sb.ToString();
        }

        public static void LogPrincipals(HttpApplication application)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}:context.user principal: {1}",
                    CommonUtils.ParentMethodName,
                    IPrincipalToString(application.Context.User, Environment.NewLine, "", 0, ">", true));
                
                Log.DebugFormat("{0}:thread principal: {1}",
                    CommonUtils.ParentMethodName,
                    IPrincipalToString(System.Threading.Thread.CurrentPrincipal, Environment.NewLine, "", 0, ">", true));
            }
        }

        public static string DictionaryToString(IDictionary<string, Object> dict)
        {
            string dictDisplay = "NULL";
            if (dict != null)
            {
                if (dict.Count < 1)
                {
                    dictDisplay = "EMPTY";
                }
                else
                {
                    StringBuilder sb = new StringBuilder("[");
                    int i = 0;
                    foreach (KeyValuePair<string, Object> kvp in dict)
                    {
                        sb.AppendFormat("{0}{1}={2}",
                            (i++ == 0 ? "" : ","), 
                            kvp.Key, 
                            kvp.Value
                        );
                    }
                    sb.Append("]");
                    dictDisplay = sb.ToString();
                }
            }
            return dictDisplay;
        }

        public static string DictionaryToString(IDictionary<string, string> dict)
        {
            string dictDisplay = "NULL";
            if (dict != null)
            {
                if (dict.Count < 1)
                {
                    dictDisplay = "EMPTY";
                }
                else
                {
                    StringBuilder sb = new StringBuilder("[");
                    int i = 0;
                    foreach (KeyValuePair<string, string> kvp in dict)
                    {
                        sb.AppendFormat("{0}{1}={2}",
                            (i++ == 0 ? "" : ","), 
                            kvp.Key, 
                            kvp.Value
                        );
                    }
                    sb.Append("]");
                    dictDisplay = sb.ToString();
                }
            }
            return dictDisplay;
        }

        public static string CookieSessionId(HttpApplication application)
        {
            string sessionId = null;
            HttpCookieCollection cookies = application.Request.Cookies;
            
            if (cookies != null)
            {
                HttpCookie sessionIdCookie = cookies[CommonUtils.ASP_NET_COOKIE_NAME_SESSION_ID];
                if (sessionIdCookie != null)
                {
                    sessionId = sessionIdCookie.Value;
                }
            }
            
            return sessionId;
        }

        public static string CookieSessionIdToString(HttpApplication application)
        {
            string sessionIdDisplay = "NULL";
            string sessionId = CookieSessionId(application);
            
            if (sessionId != null)
            {
                sessionIdDisplay = sessionId;
            }

            return string.Format("cookie[{0}]={1}", CommonUtils.ASP_NET_COOKIE_NAME_SESSION_ID, sessionIdDisplay);
        }
    }
}