<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="getproxytickets.aspx.cs" 
    Inherits="CasDotNetExampleWebapp.secure.getproxytickets" %>
<%@ Import Namespace="DotNetCasClient.Security" %>
<%@ Import Namespace="DotNetCasClient.Session" %>
<%@ Import Namespace="DotNetCasClient.Utils" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
  <head runat="server">
    <title>Protected Resource with Proxy Tickets</title>
  </head>
  <body>
    <h1>Protected Resource with Proxy Tickets</h1>
    <form id="form1" runat="server">
      <div>
        <ul>
          <li><a href="../default.aspx">Public Resource - Basic</a></li>
          <li><a href="../nonsecure/withAjax.aspx">Public Resource - With Ajax Control</a></li>
          <li><a href="default.aspx">Protected Resource - Basic</a></li>
          <li><a href="secureWithAjax.aspx">Protected Resource - With Ajax Control</a></li>
          <li><a href="getproxytickets.aspx">get more Proxy Tickets</a></li>
          <li><asp:LoginStatus ID="LoginStatus2" runat="server" /></li>
        </ul>
      </div>
      <div>
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
        <%=sessionPrincipalDisplay%>
      </div>

    <%
        var casPrincipal = HttpContext.Current.User as ICasPrincipal;
    %>
      <div>
        <dl>
	        <dt>Principal:</dt>
	        <dd><%= casPrincipal.Identity.Name %></dd>	
	        <dt>Valid from:</dt>
	        <dd><%= casPrincipal.Assertion.ValidFromDate %></dd>
	        <dt>Valid until:</dt>
	        <dd><%= casPrincipal.Assertion.ValidUntilDate %></dd>
	        <dt>Attributes:</dt>
	        <dd>
	    <dl>
		<%
        if (casPrincipal.Assertion.Attributes != null)
        {
            foreach (var attribute in casPrincipal.Assertion.Attributes)
            {
                Response.Write("<dt>" + attribute.Key + "</dt>");
                Response.Write("<dd> " + attribute.Value + "</dd>");
                
            }
        }
		%>
		</dl>
	</dd>
    </dl>
    
    <% var targetService = new Uri("http://target/service"); %>
    <dl>
    <dt>ProxyTicket for <%= targetService %></dt>
    <dd><%= casPrincipal.GetProxyTicketFor(targetService) %></dd>
    </dl>
    </div>
        
    </form> 
  </body>
</html>
