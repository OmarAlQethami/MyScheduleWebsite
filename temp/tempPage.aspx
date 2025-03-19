<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="tempPage.aspx.cs" Inherits="MyScheduleWebsite.temp.tempPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 20px;">
        <h3>Excel to SQL Converter</h3>
        <asp:FileUpload ID="fuExcel" runat="server" />
        <asp:Button ID="btnConvert" runat="server" Text="Convert to SQL" OnClick="btnConvert_Click" />
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
    </div>
</asp:Content>
