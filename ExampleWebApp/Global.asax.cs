using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using JasigCasClient.Utils;
using log4net;

namespace ExampleWebApp
{
  public class Global : System.Web.HttpApplication
  {
    /// <summary>
    /// Performs initializations needed as the ASP.NET application starts.
    /// 
    /// The configuration of log4net is done here, rather than waiting for it
    /// to be executed when the first logging call is made, so that the web
    /// application is in control of the configuration.  This seems to be
    /// needed to have the logging available in the JasigCasClient without
    /// requiring any log4net configuration in the client itself.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Application_Start(object sender, EventArgs e)
    {
      log4net.Config.XmlConfigurator.Configure();
      ILog log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}: configured log4net",
          new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name));
      }
    }

    protected void Session_Start(object sender, EventArgs e)
    {
      ILog log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:sessionID={1}",
          new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name,
          Session.SessionID));
      }
    }

    protected void Session_End(object sender, EventArgs e)
    {
      string sessionID = Session.SessionID;
      ILog log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:sessionID={1}",
          new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name,
          sessionID));
      }
      CommonUtils.RemoveState(sessionID);
    }
  }
}