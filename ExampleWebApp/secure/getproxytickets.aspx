<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="getproxytickets.aspx.cs" 
    Inherits="CasDotNetExampleWebapp.secure.getproxytickets" %>

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
    </form> 
  </body>
</html>
