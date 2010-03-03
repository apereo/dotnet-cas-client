<%@ Page Title="Non-Anonymous Users Only" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Restricted_AuthenticatedUsersOnly_Default" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    You are only able to see this section because you are not here anonymously
    (User.Identity.IsAuthenticated == <%= User.Identity.IsAuthenticated %>)
    <br /><br />
    <table border="1" class="block">
        <tr><th colspan="2">Code Access Security... The other CAS</th></tr>
        <tr><td style="font-weight: bold;">TestUserIsAuthenticatedMethod</td><td><asp:Label ID="TestUserIsAuthenticatedMethodResult" runat="server" /><br /></td></tr>
        <tr><td style="font-weight: bold;">TestUsersRoleMethod</td><td><asp:Label ID="TestUsersRoleMethodResult" runat="server" /><br /></td></tr>
        <tr><td style="font-weight: bold;">TestAdministratorsRoleMethod</td><td><asp:Label ID="TestAdministratorsRoleMethodResult" runat="server" /><br /></td></tr>
        <tr><td style="font-weight: bold;">TestUsersOrAdministratorsRoleMethod</td><td><asp:Label ID="TestUsersOrAdministratorsRoleMethodResult" runat="server" /><br /></td></tr>
        <tr><td style="font-weight: bold;">TestUserIsBobMethod</td><td><asp:Label ID="TestUserIsBobMethodResult" runat="server" /><br /></td></tr>
    </table>
</asp:Content>