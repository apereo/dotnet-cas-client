<%@ Page Title="Users or Administrators Only" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Restricted.UsersOrAdministratorsRoleOnly.RestrictedAuthenticatedUsersOnlyDefault" %>
<%--

    Licensed to Jasig under one or more contributor license
    agreements. See the NOTICE file distributed with this work
    for additional information regarding copyright ownership.
    Jasig licenses this file to you under the Apache License,
    Version 2.0 (the "License"); you may not use this file
    except in compliance with the License. You may obtain a
    copy of the License at:

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing,
    software distributed under the License is distributed on
    an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
    KIND, either express or implied. See the License for the
    specific language governing permissions and limitations
    under the License.

--%>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
    You are only able to see this section because you are in the Users or Administrators role 
    (User.IsInRole("Users") == <%= User.IsInRole("Users") %> || User.IsInRole("Administrators") == <%= User.IsInRole("Administrators") %>)
    <br /><br />    
    <asp:TextBox ID="DebugField" runat="Server" Rows="35" Columns="90" TextMode="MultiLine" Font-Size="Small" />
    <strong>ClickOnce URL:</strong>
    <asp:TextBox ID="ClickOnceUrl" runat="server" Columns="64" />
    <asp:Button ID="GetProxyTicketButton" runat="Server" Text="Launch" OnClick="GetProxyTicketButton_Click" />
</asp:Content>