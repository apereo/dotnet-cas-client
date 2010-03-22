<%@ Page Title="Users or Administrators Only" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Restricted.UsersOrAdministratorsRoleOnly.RestrictedAuthenticatedUsersOnlyDefault" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    You are only able to see this section because you are in the Users or Administrators role 
    (User.IsInRole("Users") == <%= User.IsInRole("Users") %> || User.IsInRole("Administrators") == <%= User.IsInRole("Administrators") %>)
    <br /><br />
    Target Service Url: <asp:TextBox ID="TargetUrl" runat="server" Columns="100" /><asp:Button ID="GetProxyTicketButton" runat="Server" Text="Get Proxy Ticket" OnClick="GetProxyTicketButton_Click" />
    <br /><br />
    <asp:Label ID="ProxyTicket" runat="server" />
</asp:Content>