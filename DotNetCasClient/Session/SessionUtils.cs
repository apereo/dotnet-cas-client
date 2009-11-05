using System;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using DotNetCasClient.Security;
using log4net;

namespace DotNetCasClient.Session
{
  /// <summary>
  /// Utility methods for session state maintenance for the Jasig CAS Client.
  /// </summary>
  public sealed class SessionUtils
  {
    /// <summary>
    /// Access to the log file
    /// </summary>
    static ILog LOG =
      LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// The name for the session attribute whose value is the Assertion populated
    /// with the data from the CAS ticket validation response.
    /// </summary>
    public const string CONST_CAS_PRINCIPAL = "_const_cas_principal_";

    /// <summary>
    /// Declare private constructor to prevent instance creation.
    /// </summary>
    private SessionUtils()
    {
        // prevent instance creation
    }

    /// <summary>
    /// Retrieves the session state object using HttpContext.Current.
    /// </summary>
    /// <returns>
    /// the session state object, if available, otherwise <code>null</code>
    /// is returned.
    /// </returns>
    public static HttpSessionState GetSession()
    {
      HttpSessionState session = null;
      if (HttpContext.Current.Handler is IRequiresSessionState ||
          HttpContext.Current.Handler is IReadOnlySessionState)
      {
        session = HttpContext.Current.Session;
      }
      return session;
    }

    /// <summary>
    /// Removes the Cas Principal from the session, if the session is
    /// available in the current context.
    /// </summary>
    /// <param name="session">
    /// the session state object, which may be null.
    /// </param>
    public static void RemoveCasPrincipal(HttpSessionState session)
    {
      if (session != null) {
        session.Remove(CONST_CAS_PRINCIPAL);
      }
    }

    /// <summary>
    /// Gets the Cas Principal from the session, if the session is
    /// available in the current context.
    /// </summary>
    /// <param name="session">
    /// the session state object, which may be null
    /// </param>
    /// <returns>
    /// the Cas Principal that is stored in the session, if available,
    /// otherwise <code>null</code> is returned.
    /// </returns>
    public static ICasPrincipal GetCasPrincipal(HttpSessionState session)
    {
      ICasPrincipal casPrincipal = session != null ?
        (ICasPrincipal)session[CONST_CAS_PRINCIPAL] : null;
      return casPrincipal;
    }

    /// <summary>
    /// Sets the Cas Principal in the session, if the session is
    /// available in the current context.
    /// </summary>
    /// <param name="session">
    /// the session state object, which may be null
    /// </param>
    /// <param name="casPrincipal">
    /// the Cas Principal to be stored
    /// </param>
    public static void SetCasPrincipal(HttpSessionState session,
                                       ICasPrincipal casPrincipal)
    {
      session[CONST_CAS_PRINCIPAL] = casPrincipal;
    }


    /// <summary>
    /// Gets the Principal from the session, if the session is
    /// available in the current context.
    /// </summary>
    /// <param name="session">
    /// the session state object, which may be null
    /// </param>
    /// <returns>
    /// the Principal that is stored in the session, if available,
    /// otherwise <code>null</code> is returned.
    /// </returns>
    public static IPrincipal GetPrincipal(HttpSessionState session)
    {
      IPrincipal principal = session != null ?
        (IPrincipal)session[CONST_CAS_PRINCIPAL] : null;
      return principal;
    }
  }
}
