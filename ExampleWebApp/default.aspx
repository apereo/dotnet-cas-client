<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" 
    Inherits="CasDotNetExampleWebapp._Default" %>

<%@ Import Namespace="JasigCasClient.Utils" %>
<%@ Import Namespace="JasigCasClient.Security" %>
<%@ Import Namespace="JasigCasClient.Session" %>
<%@ Import Namespace="System.Security.Principal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
  <head runat="server">
    <title>CAS Client Test</title>
  </head>
  <body>
    <h1>Public Resource</h1>
    <h2>-- Basic --</h2>
    <p>Test application for .Net JasigCasClient</p>
    <form id="form1" runat="server">
      <div>
        <ul>
          <li><a href="nonsecure/withAjax.aspx">Public Resource - With Ajax Control</a></li>  
          <li><a href="secure/default.aspx">Protected Resource - Basic</a></li>
          <li><a href="secure/secureWithAjax.aspx">Protected Resource - With Ajax Control</a></li>
          <!--
          <li><a href="secure/getproxytickets.aspx">Protected Resource - With Proxy Tickets</a></li>
          <li><asp:LoginStatus ID="LoginStatus1" runat="server" /></li>
          -->
        </ul>
        <br />
        <br />
        <%
          string contextPrincipalDisplay = "  UNDEFINED<br/>";
          if (HttpContext.Current.User != null) {
            contextPrincipalDisplay = DebugUtils.IPrincipalToString(HttpContext.Current.User,
              DebugUtils.BR, "&nbsp;&nbsp;", 2, "|", false);
          }
          string sessionPrincipalDisplay = DebugUtils.IPrincipalToString(
            (ICasPrincipal)Session[SessionUtils.CONST_CAS_PRINCIPAL],
              DebugUtils.BR, "&nbsp;&nbsp;", 2, "|", false);
        %>
        Context Information:<br />
        <%=contextPrincipalDisplay%>
        <br />
        <br />
        Session Information:<br />
        <%=sessionPrincipalDisplay %>    

      </div>
    </form>
  </body>
</html>
