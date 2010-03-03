<%@ Page Title="Administrators Only" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="SingleSignOut" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <cas:TicketManager runat="server" />
</asp:Content>