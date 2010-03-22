<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CookiesRequired.aspx.cs" Inherits="CookiesRequired" Title="Session Cookies Disabled" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">    
    The application has detected that your web browser is not configured to 
    accept session cookies.  CAS Authentication cannot function properly 
    without session cookie support.  You may need to add an exception to 
    your web browser for this site.  CAS does not require the ability to 
    store persistent cookies.
</asp:Content>