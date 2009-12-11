<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs"
     Inherits="CasDotNetExampleWebapp.login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
  <head runat="server">
    <title>Login Form</title>
  </head>
  <body>
    <h1>Standard Forms Authentication Login Screen</h1>
    <p>
      This page is for testing standard Forms Authentication behavior without DotNetCasClient.
      We should be able to demonstrate switching to DotNetCasClient by a simple Web.config change.
    </p>
    <form id="form1" runat="server">
      <div style="width: 257px">
        <asp:Login ID="Login1" runat="server" DisplayRememberMe="False">
        </asp:Login>
      </div>
    </form>
  </body>
</html>
