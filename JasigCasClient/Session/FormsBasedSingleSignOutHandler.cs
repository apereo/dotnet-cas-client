using System;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.SessionState;
using JasigCasClient.Configuration;
using JasigCasClient.Utils;
using log4net;

namespace JasigCasClient.Session
{
  /// <summary>
  /// Provides CAS single sign out functionality for an implementation using
  /// Forms Authentication.
  /// </summary>
  /// <author>Catherine D. Winfrey (.Net)</author>
  sealed class FormsBasedSingleSignOutHandler : AbstractSingleSignOutHandler
  {
    /// <summary>
    /// Constructs a SessionBasedSingleSignOutHandler, initializing it with the supplied
    /// configuration data.
    /// </summary>
    /// <param name="config">
    /// ConfigurationManager to be used to obtain the settings needed by this
    /// single sign out handler
    /// </param>
    public FormsBasedSingleSignOutHandler(CasClientConfiguration config)
      : base(config) { }

    /// <summary>
    /// Performs single sign out processing of the received HttpRequest.
    /// <remarks>
    /// See interface class for details.
    /// </remarks>
    public override bool ProcessRequest(HttpApplication application)
    {
      bool logoutRequestProcessed = base.ProcessRequest(application);
      if (logoutRequestProcessed) {
        // Establish an authenticated security context for this 
        // request / response discussion with the CAS server.
        // Needed because end request event always fires.
        HttpContext context = application.Context;
        context.User =  new GenericPrincipal(new GenericIdentity("CasSsorProcessed",
          CommonUtils.CAS_SERVER_AUTH_TYPE), new string[0]);
        System.Threading.Thread.CurrentPrincipal = context.User;
        application.CompleteRequest();
      }
      return logoutRequestProcessed;
    }
  }
}
