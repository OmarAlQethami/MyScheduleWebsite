<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentPage.aspx.cs" Inherits="MyScheduleWebsite.StudentPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/StudentStyles.css">

    <div class="main-container">
        <div class="info">
            <asp:Label ID="lblGreeting" runat="server" Text="Hello, name"></asp:Label>
            <div class="label-center">
                <asp:Label ID="lblCurrentLevel" runat="server" Text="Current Level: "></asp:Label>
            </div>
            <div class="hours-taken-container">
                <asp:Label ID="lblHoursTaken" runat="server" Text="Compulsory Hours Selected: N/A of N/A"></asp:Label>
                <asp:Label ID="lblElectiveUniversityHoursTaken" runat="server" Text="Elective University Hours Selected: N/A of N/A"></asp:Label>
                <asp:Label ID="lblElectiveCollegeHoursTaken" runat="server" Text="Elective College Hours Selected: N/A of N/A"></asp:Label>
            </div>
        </div>

        <div class="status-container">
            <div class="status-bar">
                <div id="progressBar" class="progress-bar-fill" runat="server"></div>
            </div>
            <label id="hoursChosenLabel" runat="server">Hours selected: 0</label>
        </div>

        <div class="choose-labels">
            <div class="choose-label-wrapper">
                <asp:Label ID="lblChoose1" runat="server" CssClass="labels" Text="Choose your new subjects:"></asp:Label>
            </div>
            <div class="lbl-output">
                <asp:Label ID="lblOutput" runat="server" CssClass="labels" Text=""></asp:Label>
            </div>
        </div>

        <div class="subjects-container" id="subjectsContainer" runat="server">
            <!-- Subjects will be dynamically added here -->
        </div>


        <asp:Label ID="lblSelectedSubjects" runat="server" Text=""></asp:Label>
        <div class="buttons">
            <asp:Button ID="btnBack" runat="server" Text="Back" PostBackUrl="~/Default.aspx" CssClass="custom-button"/>
            <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="custom-button"/>
        </div>

        <div class="floating-legend">
            <div class="legend-item">
                <div class="legend-box taken"></div>
                <span>Taken</span>
            </div>
            <div class="legend-item">
                <div class="legend-box available"></div>
                <span>Available</span>
            </div>
            <div class="legend-item">
                <div class="legend-box selected"></div>
                <span>Selected</span>
            </div>
            <div class="legend-item">
                <div class="legend-box unoffered"></div>
                <span>Unoffered</span>
            </div>
            <div class="legend-item">
                <div class="legend-box unavailable"></div>
                <span>Unavailable</span>
            </div>
        </div>
    </div>

    <script>
        var lblOutputClientId = '<%= lblOutput.ClientID %>';
        var progressBarId = '<%= progressBar.ClientID %>';
        var hoursLabelId = '<%= hoursChosenLabel.ClientID %>';
    </script>

    <script src="/Scripts/studentPage.js" type="text/javascript"></script>

</asp:Content>
