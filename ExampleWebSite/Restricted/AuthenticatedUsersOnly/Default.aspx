<%@ Page Title="Authenticated Users Only" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Restricted.AuthenticatedUsersOnly.Default" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    You are only able to see this page because you are an authenticated user (User.Identity.Name == <%= User.Identity.Name %>)
    <br /><br />
    <strong>Here are CAS the ticket(s) currently assigned to you:</strong><br />
    <asp:ListBox ID="YourTickets" runat="server" />
</asp:Content>