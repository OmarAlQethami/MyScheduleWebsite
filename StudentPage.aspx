<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentPage.aspx.cs" Inherits="MyScheduleWebsite.StudentPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="top-container center">
        Steps will be here - TODO
    </div>
    <div class="main-container">
         <div class="info">
             <asp:Label ID="lblGreeting" runat="server" Text="Hello, name"></asp:Label>
             <asp:Label ID="lblCurrentLevel" runat="server" Text="Current Level:"></asp:Label>
             <asp:Label ID="lblHoursTaken" runat="server" Text="Hours Taken: TODO"></asp:Label>
         </div>
        <div class="subjects-container" id="subjectsContainer" runat="server">
            <div class="subjects-row">
                
            </div>
        </div>
    </div>
</asp:Content>
