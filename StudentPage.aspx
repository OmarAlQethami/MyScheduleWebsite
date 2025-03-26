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

        <asp:MultiView ID="mvSteps" runat="server" ActiveViewIndex="0">
            <asp:View ID="viewSubjects" runat="server">
                <!-- Subjects Page -->
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
        
                <div id="subjectTooltip" class="subject-tooltip" style="display: none;">
                    <div class="tt-section">
                        <h4 class="tt-header"><span id="ttSubjectName"></span></h4>
                    </div>

                    <div class="tt-section">
                        <div class="tt-item">
                            <span class="tt-label">Level</span>
                            <span class="tt-level" id="ttLevel"></span>
                        </div>
                    </div>

                    <div class="tt-section">
                        <div class="tt-item">
                            <span class="tt-label">Credit Hours</span>
                            <span class="tt-hours" id="ttCredits"></span>
                        </div>
                    </div>

                    <div class="tt-section">
                        <div class="tt-item">
                            <span class="tt-label">Status</span>
                            <span class="tt-status" id="ttStatus"></span>
                        </div>
                    </div>

                    <div class="tt-section">
                        <div class="tt-item">
                            <span class="tt-label">Type</span>
                            <span class="tt-value" id="ttType"></span>
                        </div>
                    </div>
    
                    <div class="tt-section">
                        <div class="tt-item">
                            <span class="tt-label">Prerequisites</span>
                            <ul class="tt-prerequisites" id="ttPrerequisites"></ul>
                        </div>
                    </div>
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
            </asp:View>

            <asp:View ID="viewSections" runat="server">
                <!-- Sections Page -->
                <div class="choose-labels">
                    <div class="choose-label-wrapper">
                        <asp:Label ID="lblChoose2" runat="server" CssClass="labels" Text="Choose your desired sections:"></asp:Label>
                    </div>
                    <div class="lbl-output">
                        <asp:Label ID="lblOutput2" runat="server" CssClass="labels" Text=""></asp:Label>
                    </div>
                </div>

                <div class="sections-body" id="sectionsBody">
                    <div class="subjects-in-sections-container" id="subjectsinSectionsContainer" runat="server">
                        <!-- Selected subjects will be dynamically added here -->
                    </div>
                    <div class="sections-container" id="sectionsContainer" runat="server" ClientIDMode="Static">
                        <!-- Sections will be dynamically added here -->
                    </div>
                </div>
                
                <div class="schedule-container">
                    <div class="schedule-grid" id="scheduleGrid">

                    </div>
                </div>
            </asp:View>
        </asp:MultiView>

        <div class="buttons">
            <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click" CssClass="custom-button"/>
            <asp:Button ID="btnNext" runat="server" Text="Next" OnClick="btnNext_Click" CssClass="custom-button"/>
        </div>
    </div>

    <asp:HiddenField ID="hdnSelectedSubjects" runat="server" ClientIDMode="Static" />

    <script>
        var lblOutputClientId = '<%= lblOutput.ClientID %>';
        var lblOutput2ClientId = '<%= lblOutput2.ClientID %>';
        var progressBarId = '<%= progressBar.ClientID %>';
        var hoursLabelId = '<%= hoursChosenLabel.ClientID %>';
    </script>

    <script src="/Scripts/studentPage.js" type="text/javascript"></script>

</asp:Content>
