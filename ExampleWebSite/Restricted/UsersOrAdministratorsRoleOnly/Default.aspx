<%@ Page Title="Users or Administrators Only" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Restricted.UsersOrAdministratorsRoleOnly.RestrictedAuthenticatedUsersOnlyDefault" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    CasProxyHyperLink: <cas:CasProxyHyperLink ID="CasProxyHyperLink1" runat="server" NavigateUrl="http://www.google.com/" Text="Proxy to Google" />
    <br /><br />
    You are only able to see this section because you are in the Users or Administrators role 
    (User.IsInRole("Users") == <%= User.IsInRole("Users") %> || User.IsInRole("Administrators") == <%= User.IsInRole("Administrators") %>)
    <br /><br />    
    <asp:TextBox ID="DebugField" runat="Server" Rows="35" Columns="90" TextMode="MultiLine" Font-Size="Small" />
    <strong>ClickOnce URL:</strong>
    <asp:TextBox ID="ClickOnceUrl" runat="server" Columns="64" />
    <asp:Button ID="GetProxyTicketButton" runat="Server" Text="Launch" OnClick="GetProxyTicketButton_Click" />
</asp:Content>