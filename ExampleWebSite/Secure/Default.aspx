<%@ Page Title="Secure Page 1" MasterPageFile="~/MasterPage.master" Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Secure_Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <cas:TicketManager runat="server" />
</asp:Content>
