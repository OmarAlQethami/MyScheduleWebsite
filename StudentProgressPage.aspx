<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentProgressPage.aspx.cs" Inherits="MyScheduleWebsite.StudentProgressPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/StudentStyles.css">
    

    <div class="main-container">
        <div class="info">
            <asp:Label ID="lblGreeting" class="lbl-greeting" runat="server" Text="Hello, N/A"></asp:Label>
            <div class="label-center">
                <asp:Label ID="lblCurrentLevel" class="label-center-labels" runat="server" Text="Current Level: N/A"></asp:Label>
            </div>
            <div class="hours-taken-container">
                <asp:Label ID="lblHoursSelected" class="label-hours" runat="server" Text="Compulsory Hours Selected: N/A of N/A"></asp:Label>
                <asp:Label ID="lblElectiveUniversityHoursSelected" class="label-hours" runat="server" Text="Elective University Hours Selected: N/A of N/A"></asp:Label>
                <asp:Label ID="lblElectiveCollegeHoursSelected" class="label-hours" runat="server" Text="Elective College Hours Selected: N/A of N/A"></asp:Label>
            </div>
        </div>
        <div class="choose-labels-progress">
            <div class="choose-label-wrapper">
                <asp:Label ID="lblChoose1" runat="server" CssClass="labels" Text="Select your previously taken subjects:"></asp:Label>
            </div>
            <div class="lbl-output">
                <asp:Label ID="lblOutput" runat="server" CssClass="labels" Text=""></asp:Label>
            </div>
        </div>
        <div id="subjectsContainer" class="subjects-container" runat="server">

        </div>
        
        <div class="buttons">
            <asp:Button ID="btnBack" runat="server" Text="Back" PostBackUrl="~/Default.aspx" CssClass="custom-button"/>
            <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CssClass="custom-button" OnClick="btnConfirm_Click" OnClientClick="return onConfirmClick();"/>
        </div>
        <asp:HiddenField ID="hdnSelectedSubjects" runat="server" ClientIDMode="Static" />
    </div>

    <script>
        var lblOutputClientId = '<%= lblOutput.ClientID %>';
    </script>

    <script src="/Scripts/studentProgress.js" type="text/javascript"></script>

</asp:Content>
