using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;
using JasigCasClient.Utils;
using JasigCasClient.Validation;
using System.Web.Security;


namespace JasigCasClient.Authentication
{
    /// <summary>
    /// HttpModule implemation that intercepts all requests for protected URLs and redirects
    /// to CAS server for authentication, unless the user already has a ticket.
    /// 
    /// This HttpModule allow you to specify the following parmeters.
    /// casServerLoginUrl - the url to log into CAS, e.g. https://cas.princeton.edu/login
    /// renew - true/false on whether to use renew.
    /// gateway - true/false on whether to use gateway.
    /// 
    /// See AbstractCasHttpModule for additional properties.
    /// </summary>
    class AuthenticationHttpModule : AbstractCasHttpModule
    {
        /// <summary>
        /// The URL of the CAS Server login
        /// </summary>
        string casServerLoginUrl;

        /// <summary>
        /// Whether to send the renew request or not
        /// </summary>
        bool renew;

        /// <summary>
        /// Whether to send the gateway request or not
        /// </summary>
        bool gateway;

        IGatewayResolver gatewayStorage = new DefaultGatewayResolver();

        protected override void InitInternal(HttpApplication application)
        {
            CasClientConfiguration config = CasClientConfiguration.Config;

            this.casServerLoginUrl = config.CasServerLoginUrl;
            CommonUtils.AssertNotNull(this.casServerLoginUrl, "casServerLoginUrl cannot be null.  Check Web.config settings");
            log.Info("Loaded " + CasClientConfiguration.CAS_SERVER_LOGIN_URL + " property: " + this.casServerLoginUrl);

            this.renew = config.Renew;
            log.Info("Loaded " + CasClientConfiguration.RENEW + " property: " + this.renew.ToString());

            this.gateway = config.Gateway;
            log.Info("Loaded " + CasClientConfiguration.GATEWAY + " property: " + this.gateway.ToString());

            // Config looks good, so regisiter our event handler
            // application.AuthenticateRequest += new EventHandler(application_AuthenticateRequest);
            application.EndRequest += new EventHandler(application_EndRequest);

            //string gatewayStorageClass = getPropertyFromInitParams("gatewayStorageClass", null);
            // TODO: gateway Class.forName via gatewayStorageClass parameter
            //if (gatewayStorageClass != null) {
            //    try {
            //        this.gatewayStorage = (GatewayResolver) Class.forName(gatewayStorageClass).newInstance();
            //    } catch (final Exception e) {
            //        log.error(e,e);
            //        throw new ServletException(e);
            //    }
            //}
        }

        public void application_EndRequest(object sender, EventArgs e) 
        {
            HttpApplication application = (HttpApplication)sender;

            // EventLogMessage("EndRequest. Status code: " + application.Response.StatusCode);

            // Check fo HTTP Error 401 - Unauthorized
            // This assumes that the user is not logged, rather than logged in and Unauthorized
            if (application.Response.StatusCode == 401)
            {
                string _login = systemWebSection.Authentication.Forms.LoginUrl;
                _login += "?service=" + HttpUtility.UrlEncode(application.Request.Url.AbsoluteUri);
                application.Response.StatusCode = 302;
                application.Response.RedirectLocation = _login;
                EventLogMessage("401to302: " + application.Response.RedirectLocation);
            }
            string redirectUrl = application.Response.RedirectLocation;
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                application.Response.RedirectLocation = Regex.Replace(redirectUrl, "ReturnUrl=(?'url'.*)", delegate(Match m)
                {
                    string url = HttpUtility.UrlDecode(m.Groups["url"].Value);
                    Uri u = new Uri(application.Request.Url, url);
                    return string.Format("service={0}", HttpUtility.UrlEncode(u.ToString()));
                }, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

                //if (ForceCASRenew)
                //{
                //    application.Response.RedirectLocation += "&renew=true";
                //}
            }
            EventLogMessage("heading to: " + application.Response.RedirectLocation);
        }
            


        }

        public void application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpResponse response = application.Response;
            HttpRequest request = application.Request;

            // Is the user already logged in?  Check for valid FormsAuthenticationTicket
            HttpCookie formsAuthCookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (formsAuthCookie != null) {
                FormsAuthenticationTicket fat = FormsAuthentication.Decrypt(formsAuthCookie.Value);
                if (fat.Expired) {
                    FormsAuthentication.SignOut();
                    // do we have to return? redirect default logout or back to cas?
                } 
                else
                {
                    // User had a valid fat
                    IIdentity identity = new FormsIdentity(fat);
                    application.Context.User = new AttributePrincipal(identity.Name);
                    return;
                }
            } else if (CommonUtils.IsBlank(ticket) 
            else 
            {


            }




                //FormsAuthentication.GetRedirectUrl

                /*************************
                 * CAS Login
                 * To get here the following conditions exist:
                 *    we are using FormsAuthentication
                 *    we do NOT have an ASP.NET FormsAuthenticationTicket (it expired)
                 *    and we do NOT have a valid CAS ServiceTicket
                 *************************/
                EventLogMessage("9.Authenticate - Login");
                //this is where we will redirect to CAS
                string redirect_url = CASLoginUrl;
                //we will be setting our service to the base URL of the application
                redirect_url += "?service=" + request.Url.AbsoluteUri.ToString();

                //only redirect if ForceCASRenew is set
                if (ForceCASRenew)
                {
                    redirect_url += "&renew=true";
                    application.Response.Redirect(redirect_url);
                }
                #endregion
            }


  
    }
}
