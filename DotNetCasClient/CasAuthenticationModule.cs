using System;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Security;
using System.Security.Principal;
using DotNetCasClient.Configuration;
using DotNetCasClient.Utils;
using DotNetCasClient.Security;
using DotNetCasClient.Session;
using DotNetCasClient.Validation;
using System.Web.SessionState;
using log4net;

namespace DotNetCasClient
{
  /// <summary>
  /// HttpModule implementation to intercept requests and perform authentication via CAS.
  /// </summary>
  public sealed class CasAuthenticationModule : AbstractCasModule, IHttpModule
  {
    /// <summary>
    /// Performs initializations / startup functionality when an instance of this HttpModule
    /// is being created.
    /// </summary>
    /// <param name="application">the current HttpApplication</param>
    public override void Init(HttpApplication application)
    {
      base.Init(application);

      if (this.Gateway) {
        throw new CasConfigurationException(
          string.Format("Gateway functionality not yet implemented."));
      }

      if (this.SingleSignOut) {
        this.singleSignOutHandler = new FormsBasedSingleSignOutHandler(config);
      }

      // Register our event handlers.  These are fired on every HttpRequest.
      application.AuthenticateRequest += new EventHandler(this.OnAuthenticateRequest);
      application.AcquireRequestState +=
        (new EventHandler(this.OnAcquireRequestState));
      application.EndRequest += new EventHandler(this.OnEndRequest);
    }


    /// <summary>
    /// AuthenticateRequest event fires on every HttpRequest in order to (re)establish the security
    /// context (i.e. IPrincipal and IIdentity) available as HttpContext.User.  Security context
    /// will come from an encrypted cookie (e.g. FormsAuthenticationTicket) if the user has
    /// already been authenticated via CAS, or from the validation of a CAS Ticket if one is
    /// available, otherwise an anonymous security context is created.
    /// 
    /// FormsAuthenticationTicket maintenance is also done.
    /// </summary>
    /// <param name="sender">the HttpApplication that fired the event</param>
    /// <param name="args">the event data</param>
    void OnAuthenticateRequest(object sender, EventArgs args)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting:Summary:{1}", CommonUtils.MethodName,
          DebugUtils.FormsAuthRequestSummaryToString((HttpApplication)sender)));
        log.Debug(string.Format("{0}:starting with {1} and {2}", CommonUtils.MethodName,
          DebugUtils.CookieSessionIdToString((HttpApplication)sender),
          DebugUtils.SessionSessionIdToString((HttpApplication)sender)));
      }
      AuthenticationSection config =
        (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");

      // Make sure we are configured for Forms Authentication Mode
      if (config == null || config.Mode != AuthenticationMode.Forms) {
        // ASP.NET does this so they can have all the default AuthN Modules deployed.
        // I'm thinking we should throw an exception here to alert the deployer that
        // the config is not right, otherwise they should have this module deployed. No?
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:not configured for Forms Authentication - just return",
            CommonUtils.MethodName));
        }
        return;
      }
      HttpApplication app = (HttpApplication)sender;
      if (this.SingleSignOut) {
        if (this.singleSignOutHandler.ProcessRequest(app)) {
          if (log.IsDebugEnabled) {
            log.Debug(string.Format("{0}:SingleSignOut returned true --> " +
              " processed CAS logoutRequest", CommonUtils.MethodName));
          }
          return;
        } else if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:SingleSignOut returned false --> " +
            " received client request", CommonUtils.MethodName));
        }
      }

      HttpContext context = app.Context;
      string cookiePath = config.Forms.Path;
      string casLoginUrl = config.Forms.LoginUrl;
      bool slidingExpiration = config.Forms.SlidingExpiration;

      // Construct Service URL and save as request Item in case it is needed.
      Uri serviceUrl = this.ConstructServiceUri(app.Request);
      context.Items[CommonUtils.CAS_KEY_REDIRECT_URI] = serviceUrl;
            
      // Authenticate is a no-op unless a ticket exists.
      // When a ticket exists, ticket validation is performed
      //     success --> a new security context is created and information needed for the
      //       AcquireRequestState event is stored in the HttpRequest cache.  Note that the
      //       redirect is DELAYED until after the AcquireRequestState processing because
      //       of the need for Http session access to complete processing before the redirect
      //     failure --> TicketValidationException is thrown
      // For ticket validation failure, want to invalidate the FAT, action patterned after the
      // Jasig Java client where presence of a ticket "trumps" authentication based on session
      // attribute (which would be authentication based on FAT here).
      FormsAuthenticationEventArgs faa = new FormsAuthenticationEventArgs(context);
      try {
        if (Authenticate(faa)) {
          return;
        }
      } catch (TicketValidationException te) {
        log.Warn(te.Message, te);
        //throw new HttpException(403, "Unauthorized - CAS ticket validation failed!");
        // TODO do we want this behavior?
        //if (this.exceptionOnValidationFailure)
        //{
        //    throw new HttpException(te.Message);
        //}
        return;
      }

      // No initial security context via CAS ticket, how about FAT Cookie?
      FormsAuthenticationTicket faTicket = this.FindValidFat(context);
      if (faTicket == null) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:return(no valid fat)", CommonUtils.MethodName));
        }
        return;
      }

      // FAT valid, do we need to extend expiration?
      FormsAuthenticationTicket faTicketOld = faTicket;
      if (config.Forms.SlidingExpiration) {
        faTicket = FormsAuthentication.RenewTicketIfOld(faTicket);
      }

      // FAT valid and up to date, create security context.
      // Since the session is not available yet, the CAS Assertion from the previous
      // authentication is not available in order to create a CasPrincipal.  A
      // GenericPrincipal with authentication type 'Jasig CAS' will be created and
      // stored in the security context, to be updated with the Assertion by the
      // an event handler method that has session access.
      context.User =  new GenericPrincipal(new GenericIdentity(faTicket.Name,
        CommonUtils.CAS_AUTH_TYPE), new string[0]);
      System.Threading.Thread.CurrentPrincipal = context.User;

      // Do we need to reset (renew) the FAT cookie?
      // If cookie is persistent and fat wasn't renewed, no
      HttpCookie cookie = context.Request.Cookies[config.Forms.Name];
      if (cookie.Expires == DateTime.MinValue && faTicketOld == faTicket) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:return(no cookie reset needed):Summary:{1}",
            CommonUtils.MethodName,
            DebugUtils.FormsAuthSummaryToString((HttpApplication)sender)));
        }
        return;
      }

      // otherwise, yes, create new FAT Cookie
      cookie.Value = FormsAuthentication.Encrypt(faTicket);
      cookie.Path = config.Forms.Path;
      if (faTicket.IsPersistent) {
        cookie.Expires = faTicket.Expiration;
      }
      context.Response.Cookies.Add(cookie);
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:ending:Summary:{1}", CommonUtils.MethodName,
          DebugUtils.FormsAuthSummaryToString((HttpApplication)sender)));
      }
    }

    /// <summary>
    /// Performs CAS state information maintenance that requires access to the
    /// Http session.
    /// </summary>
    /// <param name="sender">the HttpApplication that fired the event</param>
    /// <param name="args">the event data</param>
    void OnAcquireRequestState(object sender, EventArgs args)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting with {1} and {2}", CommonUtils.MethodName,
          DebugUtils.CookieSessionIdToString((HttpApplication)sender),
          DebugUtils.SessionSessionIdToString((HttpApplication)sender)));
      }
      HttpApplication app = (HttpApplication)sender;
      HttpContext context = app.Context;
      HttpRequest request = app.Request;
      HttpSessionState session = SessionUtils.GetSession();
          
      // If ticket was received, even if not validated, update the single sign out
      // information.
      if (this.SingleSignOut) {
        string ticket = (string)context.Items[CommonUtils.CAS_KEY_TICKET];
        if (CommonUtils.IsNotBlank(ticket)) {
          if (session != null) {
            try {
              this.singleSignOutHandler.StoreState(app, ticket, session.SessionID);
            } catch (Exception ex) {
              if (log.IsWarnEnabled) {
                log.Warn(string.Format("{0}: FAILURE: " +
                  "Storing single sign out information for sessionID={1} and ticket={2}",
                  CommonUtils.MethodName, session.SessionID, ticket), ex);
              }
            }
          } 
        }
      }

      // Do maintenance on the CasPrincipal stored in the session.  The presence of
      // this session attribute is the "final authority" in terms of authentication.
      IPrincipal contextUserPrincipal = context.User;
      if (contextUserPrincipal != null) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:starting context.User={1}",
            CommonUtils.MethodName, DebugUtils.IPrincipalToString(contextUserPrincipal)));
        }
        if (typeof(CasPrincipal) == contextUserPrincipal.GetType()) {
          // Ticket validation occurred successfully.  Store the CasPrincipal in the
          // session.
          if (log.IsDebugEnabled) {
            log.Debug(string.Format("{0}:store CasPrincipal from context.User in session.",
              CommonUtils.MethodName));
           }
          SessionUtils.SetCasPrincipal(session, (ICasPrincipal)contextUserPrincipal);

        } else {
          // GenericPrincipal so authentication occurred from the FAT -- or -- no valid FAT
           // and are accessing a publicly available page.
          if (CommonUtils.CAS_AUTH_TYPE.Equals(contextUserPrincipal.Identity.AuthenticationType)) {
            // Authentication was based on FAT.  Still might be a publicly available page
            // but don't know how to determine this.
            // A session attribute must exist with the CASPrincipal for the authentication to
            // be acceptable. Otherwise, the authentication information will be cleared and
            // a redirect to the original URL performed to force the authentication step to
            // be performed with no FAT available to solve the public versus secure issue.
            IPrincipal storedPrincipal = SessionUtils.GetPrincipal(session);
            if (storedPrincipal != null) {
              if (log.IsDebugEnabled) {
                log.Debug(string.Format("{0}:starting session Principal={1}",
                  CommonUtils.MethodName, 
                  DebugUtils.IPrincipalToString(storedPrincipal)));
              }
              if (typeof(CasPrincipal) == storedPrincipal.GetType()) {
                ICasPrincipal storedCasPrincipal = (ICasPrincipal)storedPrincipal;
                if (log.IsDebugEnabled) {
                  log.Debug(string.Format("{0}:store CasPrincipal from session in context.User.",
                    CommonUtils.MethodName));
                 }
                context.User = storedPrincipal;
                System.Threading.Thread.CurrentPrincipal = storedPrincipal;
              } else {
                if (log.IsWarnEnabled) {
                  log.Warn(string.Format(
                    "{0}: incorrect type for Principal in session --> fail authentication from FAT",
                    CommonUtils.MethodName));
                }
                contextUserPrincipal = null;
              }
            } else {
              if (log.IsInfoEnabled) {
                log.Info(string.Format(
                  "{0}: no IPrincipal in session --> fail authentication from FAT",
                  CommonUtils.MethodName));
              }
              contextUserPrincipal = null;
            }
            if (contextUserPrincipal == null) {
              if (log.IsDebugEnabled) {
                log.Debug(string.Format("{0}:FAT authentication with missing session IPrincipal" +
                  " --> force redirect to CAS login",
                  CommonUtils.MethodName));
              }
              context.User = null;
              System.Threading.Thread.CurrentPrincipal = null;
              SessionUtils.RemoveCasPrincipal(session);
              FormsAuthentication.SignOut();
              context.Items[CommonUtils.CAS_KEY_REDIRECT_REQUIRED] = "true";
            }
          } else {
            // Public page access with no valid FAT.
            SessionUtils.RemoveCasPrincipal(session);
          }
        }
      }
          
      // Issue any delayed redirect
      string redirectUrl = context.Items[CommonUtils.CAS_KEY_REDIRECT_URI].ToString();
      if (CommonUtils.IsNotBlank(redirectUrl) &&
        "true".Equals(context.Items[CommonUtils.CAS_KEY_REDIRECT_REQUIRED]))
      {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:process delayed redirect to {1}",
            CommonUtils.MethodName, redirectUrl));
        }
        context.Response.Redirect(redirectUrl, true);
      }

      // throw any delayed exception
      Exception delayedEx = (Exception)context.Items[CommonUtils.CAS_KEY_EXCEPTION_TO_THROW];
      if (delayedEx != null) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:process delayed exception {1}",
            CommonUtils.MethodName, delayedEx.Message));
        }
        throw delayedEx;
      }
    }


    /// <summary>
    /// EndRequest fires on every HttpRequest.  If we get this far and have an HTTP 401, we redirect
    /// the user to CAS.  An authenticated user via FAT that lacks authorization would get kicked
    /// out of the pipeline at AuthorizeRequest???, so we assume we need to go to CAS, unless we 
    /// already have a ticket.
    /// </summary>
    /// <param name="sender">the HttpApplication that fired the event</param>
    /// <param name="args">the event arguments</param>
    void OnEndRequest(object sender, EventArgs args)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting:Summary:{1}", CommonUtils.MethodName,
          DebugUtils.FormsAuthSummaryToString((HttpApplication)sender)));
      }
      // Make sure we are configured for Forms Authentication Mode, do we really have 
      // to keep doing this? isn't once enough? should we have an initConfig()?
      AuthenticationSection config = 
        (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");
      if (config == null || config.Mode != AuthenticationMode.Forms) {
        // I'm thinking we should throw an expection here to alert the deployer that
        // the config is not right, otherwise they should have this module deployed. No?
        return;
      }

      HttpApplication app = (HttpApplication)sender;
      HttpRequest request = app.Context.Request;
      HttpResponse response = app.Context.Response;
      HttpContext context = app.Context;

      // If we got an HTTP 401 Error (Unauthorized) and don't have a CAS ticket,
      // redirect to CAS.
      if ((response.StatusCode == 401 &&
             request.QueryString[this.ArtifactParameterName] == null))
      {
        // construct CASRedirectURL and do redirect
        response.Redirect(this.ConstructRedirectUri(request, config.Forms.LoginUrl));
      }
    }

    // TODO do want to fire Authenticate events?  Do we need this for ASP.NET?
    //public event FormsAuthenticationEventHandler Authenticate;
    /// <summary>
    /// Validates a ticket if received in the request.
    /// <para>
    /// For successful ticket validation, the HttpRequest cache is updated with the
    /// redirect URL.  The redirect needs to be delayed until the post-Authenticate
    /// processing that must be performed in an event handler that has session access.
    /// </para>
    /// </summary>
    /// <param name="args">access to the data needed for Forms Authentication</param>
    /// <returns>
    /// <code>true</code> if a ticket was successfully validated; otherwise returns
    /// <code>false</code> to indicate no ticket was received
    /// </returns>
    /// <exception cref="TicketValidationException">
    /// Thrown if a ticket was received and failed validation.
    /// </exception>
    bool Authenticate(FormsAuthenticationEventArgs args)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:starting:Summary:{1}", CommonUtils.MethodName,
          DebugUtils.FormsAuthSummaryToString(args.Context)));
      }

      HttpContext context = args.Context;
      HttpRequest request = context.Request;

      // This looks for the ticket in HttpRequest.Cookies, HttpRequest.Form,
      // HttpRequest.QueryString, and System.Web.HttpRequest.ServerVariables collections.
      // TODO: is this what we want?  which one wins???
      string ticket = request[this.ArtifactParameterName];

      if (CommonUtils.IsNotBlank(ticket)) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:Ticket found for validation: {1}",
            CommonUtils.MethodName, ticket));
        }
        context.Items[CommonUtils.CAS_KEY_TICKET] = ticket;
        ICasPrincipal principal = this.ticketValidator.Validate(ticket,
          (Uri)context.Items[CommonUtils.CAS_KEY_REDIRECT_URI]);

        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:Successfully authenticated user {1}",
            CommonUtils.MethodName, principal.Identity.Name));
        }

        // Setup .Net User object and Forms Authentication Cookie
        args.Context.User = principal;
        System.Threading.Thread.CurrentPrincipal = args.Context.User;
        FormsAuthentication.SetAuthCookie(principal.Identity.Name, false);
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:before Redirect to serviceURL:Summary:{1}",
            CommonUtils.MethodName,
            DebugUtils.FormsAuthSummaryToString(args.Context)));
        }

        // Store information in HttpRequest cache needed during the 
        // AcquireRequestState event handler method, including the
        // need for the redirect to the original URL but without the ticket.
        // The redirect URL has already been stored in the cache.
        context.Items[CommonUtils.CAS_KEY_REDIRECT_REQUIRED] = "true";
        return true;
      } else {
        return false;
      }
    }


    /// <summary>
    /// Checks for presence of a valid FAT.
    /// </summary>
    /// <param name="context">current HttpContext that may contain an existing FAT</param>
    /// <returns>the Forms Authentication Ticket(FAT) <i>if</i> a Forms Authentication Cookie
    /// containing a valid FAT exists; otherwise <code>null</code>.
    /// </returns>
    FormsAuthenticationTicket FindValidFat(HttpContext context)
    {
      AuthenticationSection config =
        (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");
      // Is FAT cookie still valid?
      HttpCookie cookie = context.Request.Cookies[config.Forms.Name];
      if (cookie == null  ||
          cookie.Expires != DateTime.MinValue && cookie.Expires < DateTime.Now)
      {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:no valid cookie --> return null", CommonUtils.MethodName));
        }
        return null;
      }

      // got a cookie, does it contain a FAT?
      if (CommonUtils.IsBlank(cookie.Value)) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:no FAT in cookie --> return null", CommonUtils.MethodName));
          return null;
        }
      }
      FormsAuthenticationTicket faTicket = null;
      try {
        faTicket = FormsAuthentication.Decrypt(cookie.Value);
      } catch (ArgumentException) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:no decryptable FAT in cookie --> return null",
            CommonUtils.MethodName));
          return null;
        }
      }
      if (faTicket == null  || faTicket.Expired) {
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:no valid FAT --> return null", CommonUtils.MethodName));
          return null;
        }
      }
      return faTicket;
    }
  }
}