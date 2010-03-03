<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TicketManager.ascx.cs" Inherits="Controls.TicketManager" %>
<asp:GridView ID="OutstandingTickets" DataKeyNames="NetId,ServiceTicket" runat="server" AutoGenerateColumns="false" Width="100%" OnRowCommand="OutstandingTickets_RowCommand" CssClass="block">
    <Columns>
        <asp:BoundField DataField="NetId" HeaderText="NetId" ItemStyle-Width="15%" />
        <asp:BoundField DataField="ServiceTicket" HeaderText="Ticket" ItemStyle-Width="30%" />
        <asp:BoundField DataField="ValidFrom" HeaderText="From" ItemStyle-Width="20%" />
        <asp:BoundField DataField="ValidUntil" HeaderText="Until" ItemStyle-Width="20%" />
        <asp:BoundField DataField="Expired" HeaderText="Expired" ItemStyle-Width="5%" />
        <asp:ButtonField ButtonType="Link" Text="Revoke" CommandName="Revoke" />
        <asp:ButtonField ButtonType="Link" Text="Single Signout" CommandName="SSO" />
    </Columns>
</asp:GridView>
<asp:Label runat="server" ID="NoTicketManagerLabel" />
<asp:Label ID="ResponseField" runat="server" />
