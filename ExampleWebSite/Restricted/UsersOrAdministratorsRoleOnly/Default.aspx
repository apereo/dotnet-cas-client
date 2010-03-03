<%@ Page Title="Users or Administrators Only" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Restricted_AuthenticatedUsersOnly_Default" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    You are only able to see this section because you are in the Users or Administrators role 
    (User.IsInRole("Users") == <%= User.IsInRole("Users") %> || User.IsInRole("Administrators") == <%= User.IsInRole("Administrators") %>)
</asp:Content>