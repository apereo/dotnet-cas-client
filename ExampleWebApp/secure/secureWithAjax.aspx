<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="secureWithAjax.aspx.cs" 
    Inherits="CasDotNetExampleWebapp.secure.secureWithAjax" %>

<%@ Import Namespace="DotNetCasClient.Utils" %>
<%@ Import Namespace="DotNetCasClient.Security" %>
<%@ Import Namespace="DotNetCasClient.Session" %>
<%@ Import Namespace="System.Security.Principal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
  <head runat="server">
    <title>Protected Resource</title>
  </head>
  <body>
    <h1>Protected Resource</h1>
    <h2> -- Ajax Control --</h2>      
    <form id="form1" runat="server">
      <asp:ScriptManager EnablePartialRendering="true" ID="ScriptManager" runat="server"/>    
      <div>
        <ul>
          <li><a href="../default.aspx">Public Resource - Basic</a></li>
          <li><a href="../nonsecure/withAjax.aspx">Public Resource - With Ajax Control</a></li>
          <li><a href="default.aspx">Protected Resource - Basic</a></li>              
        </ul>
        <br />
        <br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
          <ContentTemplate>
            <asp:Button ID="Button1" runat="server" Text="Click Me" OnClick="Button1_Click"/>&nbsp;&nbsp;
            <asp:Label ID="Label1" runat="server" Text="No click yet!"></asp:Label>
          </ContentTemplate>
        </asp:UpdatePanel>           
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
