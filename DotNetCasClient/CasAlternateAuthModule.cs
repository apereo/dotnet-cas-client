using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using JasigCasClient.Authentication;
using JasigCasClient.Configuration;
using JasigCasClient.Security;
using JasigCasClient.Session;
using JasigCasClient.Utils;
using JasigCasClient.Validation;
using log4net;

namespace JasigCasClient
{
  /// <summary>
  /// HttpModule implementation to intercept requests and perform authentication via CAS.
  /// <para>
  /// This version does not use Windows Forms Authentication.  The event AcquireRequestState
  /// is used for the CAS authentication functionality because it is the first event
  /// that provides access to the HttpSession.
  /// </para>
  /// </summary>
  /// <author>Catherine Winfrey</author>
  public class CasAlternateAuthModule : AbstractCasModule, IHttpModule
  { 
    #region Properties
    /// Regex for selection of URIs which need to be protected by CAS.
    protected Regex SecureUriRegex { get; private set; }

    /// Regex for selection of URIs which match the SecureUriRegex but do
    /// not need to be protected by CAS.  This is needed for things such
    /// as the Ajax resource URIs.
    protected Regex SecureUriExceptionRegex { get; private set; }

    // Whether authentication based on the presence of the CAS
    // Assertion in the session is in effect, reducing the number of
    // round-trips to the CAS server.
    protected bool UseSession { get; private set; }
    #endregion

    /// <summary>
    /// Initializes the HttpModule and prepares it to handle requests.
    /// <para>
    /// Primary tasks are reading configuration settings and registering
    /// event handler(s).
    /// </para>
    /// </summary>
    /// <param name="application">the application context</param>
    public override void Init(HttpApplication application)
    {
      base.Init(application);

      this.InitCommon(application);
      this.InitInternalAuthentication(application);
      this.InitInternalValidation(application);

      application.AcquireRequestState +=
        (new EventHandler(this.Application_AcquireRequestState));

      if (log.IsDebugEnabled) {
        application.EndRequest +=
            (new EventHandler(this.Application_EndRequest));
      }
    }


    /// <summary>
    /// Performs initializations / startup functionality common to all the methods of
    /// this HttpModule.
    /// <param name="application">the application context</param>
    private void InitCommon(HttpApplication application)
    {

      this.UseSession = config.UseSession;
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded UseSession property: {1}",
          CommonUtils.MethodName, this.UseSession));
      }
      if (this.RedirectAfterValidation && !this.UseSession) {
        throw new CasConfigurationException(
          string.Format(
          "Redirect after ticket validation functionality supported ONLY" + 
          " when use session is enabled."));
      }

      this.SecureUriRegex = new Regex(config.SecureUriRegex);
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded SecureUriRegex property: {1}",
          CommonUtils.MethodName, this.SecureUriRegex));
      }

      this.SecureUriExceptionRegex = new Regex(config.SecureUriExceptionRegex);
      if (log.IsInfoEnabled) {
        log.Info(string.Format("{0}:Loaded SecureUriExceptionRegex property: {1}",
          CommonUtils.MethodName, this.SecureUriExceptionRegex));
      }
      if (this.SingleSignOut) {
        this.singleSignOutHandler = new SessionBasedSingleSignOutHandler(config);
      }
    }

    
    /// <summary>
    /// Performs initializations / startup functionality specific to the Authentication step.
    /// </summary>
    /// <param name="application">the application context</param>
    private void InitInternalAuthentication(HttpApplication application)
    {
      if (this.Gateway) {
        this.gatewayResolver = new SessionAttrGatewayResolver();
      }
    }


    /// <summary>
    /// Performs initializations / startup functionality specific to the Validation step.
    /// </summary>
    /// <param name="application">the application context</param>
    private void InitInternalValidation(HttpApplication application)
    {
    }

    /// <summary>
    /// Determines if the current URI represents a CAS protected resource using the
    /// configured Regex.
    /// </summary>
    /// <param name="application">the current application</param>
    /// <returns>true if this resource should be protected by CAS</returns>
    protected bool IsCasProtected(HttpApplication application)
    {
      bool isProtected = this.SecureUriRegex.IsMatch(application.Request.RawUrl);
      if (isProtected) {
        isProtected = !this.SecureUriExceptionRegex.IsMatch(application.Request.RawUrl);
      }
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:Checking {1} using secureUriRegex {2} and " +
                                "secureUriExceptionRegex {3} --> {4}",
                                CommonUtils.MethodName, application.Request.RawUrl,
                                this.SecureUriRegex,
                                this.SecureUriExceptionRegex,
                                isProtected));
      }
      return isProtected;
    }


    /// <summary>
    /// Executes CAS functionality as dictated by the current state of the request,
    /// which will be either authentication (which results in a redirect to CAS logon as the
    /// exit from this method) or validation.
    /// </summary>
    /// <param name="sender">the application context</param>
    /// <param name="e">event data</param>
    public void Application_AcquireRequestState(object sender, EventArgs e)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting with {1} and {2}", CommonUtils.MethodName,
          DebugUtils.CookieSessionIdToString((HttpApplication)sender),
          DebugUtils.SessionSessionIdToString((HttpApplication)sender)));
      }
      HttpApplication application = (HttpApplication)sender;
      if (this.SingleSignOut) {
        if (this.singleSignOutHandler.ProcessRequest(application)) {
          if (log.IsDebugEnabled) {
            log.Debug(string.Format("{0}:SingleSignOut returned true --> " +
              " received CAS logoutRequest", CommonUtils.MethodName));
          }
          return;
        }
      }
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:SingleSignOut returned false --> " +
          " received client request", CommonUtils.MethodName));
      }
      if (this.IsCasProtected(application)) {
        this.AuthenticateStep(application, e);
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:AuthenticateStep returned --> no redirect to CAS logon",
            CommonUtils.MethodName));
        }
        this.ValidateStep(application, e);
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:ValidateStep completed", CommonUtils.MethodName));
        }
      }
      HttpSessionState session = SessionUtils.GetSession();
      if (session != null) {
        ICasPrincipal sessionPrincipal = SessionUtils.GetCasPrincipal(session);
        if (sessionPrincipal != null) {
          if (log.IsDebugEnabled) {
            log.Debug(string.Format("{0}:copy CAS Principal({1}) to context",
              CommonUtils.MethodName,
              sessionPrincipal.Identity != null ? sessionPrincipal.Identity.Name : "unknown"));
          }
          application.Context.User = sessionPrincipal;
          System.Threading.Thread.CurrentPrincipal = sessionPrincipal;
        }
      }
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:ending", CommonUtils.MethodName));
        DebugUtils.LogPrincipals((HttpApplication)sender);
      }
    }

    /// <summary>
    /// Performs debug logging to aid in development.
    /// </summary>
    /// <param name="sender">the application context</param>
    /// <param name="e">event data</param>
    public void Application_EndRequest(object sender, EventArgs e)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting", CommonUtils.MethodName));
        DebugUtils.LogPrincipals((HttpApplication)sender);
      }
    }


    //TODO renew and gateway
    /// <summary>
    /// Redirects to CAS logon if either
    /// <list type="number">
    /// <item><description>no ticket exists -or-</description></item>
    /// <item><description>
    /// session CasPrincipal either does <i>not</i> exist or is not
    /// being used for re-authentication
    /// </description></item>
    /// </summary>
    /// <param name="application">the application context</param>
    /// <param name="e">event data</param>
    private void AuthenticateStep(HttpApplication application, EventArgs e)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting", CommonUtils.MethodName));
      }
      // set some helper objects
      HttpRequest request = application.Request;
      HttpResponse response = application.Response;
      HttpSessionState session = SessionUtils.GetSession();
      string ticket = request.QueryString[this.ArtifactParameterName];
      ICasPrincipal sessionPrincipal = SessionUtils.GetCasPrincipal(session);
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:ticket=>{1}<", CommonUtils.MethodName, ticket));
        log.Debug(string.Format("{0}:sessionPrincipal=>{1}<", CommonUtils.MethodName,
                                sessionPrincipal));
        log.Debug(string.Format("{0}:gatewayResolver={1} and IsGatewayed={2}",
          CommonUtils.MethodName,
          (this.gatewayResolver == null ? "null" : this.gatewayResolver.ToString()),
          (this.gatewayResolver == null ? "notApplicable" :
              this.gatewayResolver.IsGatewayed().ToString())));
      }
      if (CommonUtils.IsBlank(ticket)  &&
          (sessionPrincipal == null  || !this.UseSession)  &&
          (this.gatewayResolver == null  || !this.gatewayResolver.IsGatewayed()))
      {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:no ticket -and- no 'for auth' principal" +
             " -and- not redirect after gateway", CommonUtils.MethodName));
        }
        // Delete any existing CasPrincipal from the session -- it was stored for the purpose
        // of sending Assertion information to the web site and not for authentication on
        // subsequent page accesses.
        SessionUtils.RemoveCasPrincipal(session);

        // Update to 'is gatewayed' status if configured for gateway functionality
        if (this.gatewayResolver != null) {
          this.gatewayResolver.StoreGatewayInformation(
            this.ConstructServiceUri(request).ToString());
        }

        // redirect to CAS login authentication
        response.Redirect(this.ConstructRedirectUri(request, this.CasServerLoginUrl));
      }

      // make sure gateway status is 'not gatewayed'
      if (this.gatewayResolver != null) {
        this.gatewayResolver.WasGatewayed(this.ConstructServiceUri(request).ToString());
      }
    }

    /// <summary>
    /// Performs CAS ticket validation if an HttpRequest parameter containing a ticket
    /// exists.
    /// </summary>
    /// <remarks>
    /// For successful ticket validation, a redirect to the original URL <i>without</i> the
    /// ticket included in the URL is performed if the configuration included this setting.
    /// </remarks>
    /// <param name="sender">the application context</param>
    /// <param name="e">event data</param>
    /// <exception cref="HttpException">
    /// Thrown with a status code of 403 if ticket validation is attempted and fails.
    /// This will include the original TicketValidationException that was thrown during the
    /// ticket validation processing.
    /// </exception>
    private void ValidateStep(HttpApplication application, EventArgs e)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting",CommonUtils.MethodName));
      }
      // set some helper objects
      HttpRequest request = application.Request;
      HttpResponse response = application.Response;
      HttpSessionState session = SessionUtils.GetSession();
      string ticket = request.QueryString[this.ArtifactParameterName];
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:ticket=>{1}<", CommonUtils.MethodName, ticket));
      }
      if (CommonUtils.IsNotBlank(ticket)) {
        if (session != null) {
          try {
            this.singleSignOutHandler.StoreState(application, ticket, session.SessionID);
          } catch (Exception ex) {
            if (log.IsWarnEnabled) {
              log.Warn(string.Format("{0}: FAILURE: " +
                "Storing single sign out information for sessionID={1} and ticket={2}",
                CommonUtils.MethodName, session.SessionID, ticket), ex);
            }
          }
        } 
        try {
          // Delete any existing CasPrincipal from the session -- successful ticket validation
          // will return a new CasPrincipal to be stored and if ticket validation fails then
          // it is assumed that the old data is no longer correct and certainly shouldn't be
          // used for authentication purposes
          SessionUtils.RemoveCasPrincipal(session);

          Uri serviceUri = this.ConstructServiceUri(request);
          ICasPrincipal principal = this.ticketValidator.Validate(ticket, serviceUri);
          SessionUtils.SetCasPrincipal(session, principal);
          application.Context.User = principal;
          System.Threading.Thread.CurrentPrincipal = principal;
          if (log.IsDebugEnabled) {
            log.Debug(string.Format("{0}:Successfully authenticated user: {1}",
              CommonUtils.MethodName, principal.Assertion.PrincipalName));
          }
          if (this.RedirectAfterValidation) {
            response.Redirect(serviceUri.ToString(), true);
          }
        } catch (TicketValidationException te) {
          if (log.IsWarnEnabled) {
            log.Warn(string.Format("{0}:{1}", CommonUtils.MethodName, te.Message), te);
            throw new HttpException(403, te.Message);
          }
        }
      } else if (!UseSession) {
        // Delete any existing CasPrincipal from the session -- since authentication using the
        // existence of a session attribute is not enabled, reaching this point means that
        // this request is after a gateway with no SSO found by the CAS server.
        SessionUtils.RemoveCasPrincipal(session);
      }
    }
  }
}
