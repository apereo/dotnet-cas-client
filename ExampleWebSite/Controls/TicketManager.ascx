<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TicketManager.ascx.cs" Inherits="Controls.TicketManager" %>
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
<asp:GridView ID="OutstandingTickets" DataKeyNames="NetId,ServiceTicket" runat="server" AutoGenerateColumns="false" Width="100%" OnRowCommand="OutstandingTickets_RowCommand" CssClass="block">
    <Columns>
        <asp:BoundField DataField="NetId" HeaderText="NetId" ItemStyle-Width="15%" />
        <asp:BoundField DataField="ServiceTicket" HeaderText="Ticket" ItemStyle-Width="30%" />
        <asp:BoundField DataField="ValidFrom" HeaderText="From" ItemStyle-Width="20%" />
        <asp:BoundField DataField="ValidUntil" HeaderText="Until" ItemStyle-Width="20%" />
        <asp:BoundField DataField="Expired" HeaderText="Expired" ItemStyle-Width="5%" />
        <asp:ButtonField ButtonType="Link" Text="Revoke" CommandName="Revoke" />
        <asp:ButtonField ButtonType="Link" Text="Single SignOut" CommandName="SSO" />
    </Columns>
</asp:GridView>
<asp:Label runat="server" ID="NoTicketManagerLabel" />
<asp:Label ID="ResponseField" runat="server" />
