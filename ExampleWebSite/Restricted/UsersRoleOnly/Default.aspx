<%@ Page Title="Users Only" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Restricted_AuthenticatedUsersOnly_Default" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    You are only able to see this section because you are in the Users role 
    (User.IsInRole("Users") == <%= User.IsInRole("Users") %>)
    <br /><br />
    <strong>You have the following roles:</strong><br />
    <asp:CheckBoxList runat="server" ID="RoleList" />
</asp:Content>