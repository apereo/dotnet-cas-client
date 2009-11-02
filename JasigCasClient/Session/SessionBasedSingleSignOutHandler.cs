using System;
using System.Web;
using System.Web.SessionState;
using JasigCasClient.Configuration;
using JasigCasClient.Utils;
using log4net;

namespace JasigCasClient.Session
{
  /// <summary>
  /// Provides CAS single sign out functionality for an implementation using session-base
  /// authentication.
  /// </summary>
  /// <author>Catherine D. Winfrey (.Net)</author>
  class SessionBasedSingleSignOutHandler : AbstractSingleSignOutHandler
  {
     /// <summary>
    /// Constructs a SessionBasedSingleSignOutHandler, initializing it with the supplied
    /// configuration data.
    /// </summary>
    /// <param name="config">
    /// ConfigurationManager to be used to obtain the settings needed by this
    /// single sign out handler
    /// </param>
    public SessionBasedSingleSignOutHandler(CasClientConfiguration config)
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
        application.CompleteRequest();
      }
      return logoutRequestProcessed;
    }
  }
}
