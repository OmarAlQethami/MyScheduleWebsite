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
                    <div class="sections-container" id="sectionsContainer" runat="server">
                        <div class="section" onclick="SectionClicked(this)">
                            <div class="section-rows">
                                <div class="section-row">
                                    <div class="info-box">
                                        <span class="info-label">Section</span>
                                        <span class="info-value section-number">511</span>
                                    </div>
                                    <div class="info-box">
                                        <span class="info-label">Subject</span>
                                        <span class="info-value subject-name">Internet Technologies</span>
                                    </div>
                                    <div class="info-box">
                                        <span class="info-label">Capacity</span>
                                        <span class="info-value capacity">28/40</span>
                                    </div>
                                </div>
            
                                <div class="section-row">
                                    <div class="time-info">
                                        <span class="info-label">Day</span>
                                        <span class="info-value day">Wednesday</span>
                                    </div>
                                    <div class="time-info">
                                        <span class="info-label">From</span>
                                        <span class="info-value start-time">09:00 AM</span>
                                    </div>
                                    <div class="time-info">
                                        <span class="info-label">To</span>
                                        <span class="info-value end-time">11:00 AM</span>
                                    </div>
                                    <div class="location">
                                        <span class="info-label">Location</span>
                                        <span class="info-value location">29204</span>
                                    </div>
                                </div>
            
                                <div class="section-row">
                                    <div class="info-box" style="width: 100%;">
                                        <span class="info-label">Instructor</span>
                                        <span class="info-value instructor-name">Ibrahim Althomali</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="section" onclick="SectionClicked(this)">
                            <div class="section-rows">
                                <div class="section-row">
                                    <div class="info-box">
                                        <span class="info-label">Section</span>
                                        <span class="info-value section-number">434</span>
                                    </div>
                                    <div class="info-box">
                                        <span class="info-label">Subject</span>
                                        <span class="info-value subject-name">Internet Technologies</span>
                                    </div>
                                    <div class="info-box">
                                        <span class="info-label">Capacity</span>
                                        <span class="info-value capacity">36/40</span>
                                    </div>
                                </div>
            
                                <div class="section-row">
                                    <div class="time-info">
                                        <span class="info-label">Day</span>
                                        <span class="info-value day">Monday</span>
                                    </div>
                                    <div class="time-info">
                                        <span class="info-label">From</span>
                                        <span class="info-value start-time">08:00 AM</span>
                                    </div>
                                    <div class="time-info">
                                        <span class="info-label">To</span>
                                        <span class="info-value end-time">10:00 AM</span>
                                    </div>
                                    <div class="location">
                                        <span class="info-label">Location</span>
                                        <span class="info-value location">29102</span>
                                    </div>
                                </div>
            
                                <div class="section-row">
                                    <div class="info-box" style="width: 100%;">
                                        <span class="info-label">Instructor</span>
                                        <span class="info-value instructor-name">Ibrahim Althomali</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="section" onclick="SectionClicked(this)">
                            <div class="section-rows">
                                <div class="section-row">
                                    <div class="info-box">
                                        <span class="info-label">Section</span>
                                        <span class="info-value section-number">488</span>
                                    </div>
                                    <div class="info-box">
                                        <span class="info-label">Subject</span>
                                        <span class="info-value subject-name">Internet Technologies</span>
                                    </div>
                                    <div class="info-box">
                                        <span class="info-label">Capacity</span>
                                        <span class="info-value capacity">22/40</span>
                                    </div>
                                </div>
            
                                <div class="section-row">
                                    <div class="time-info">
                                        <span class="info-label">Day</span>
                                        <span class="info-value day">Sunday</span>
                                    </div>
                                    <div class="time-info">
                                        <span class="info-label">From</span>
                                        <span class="info-value start-time">02:00 PM</span>
                                    </div>
                                    <div class="time-info">
                                        <span class="info-label">To</span>
                                        <span class="info-value end-time">04:00 PM</span>
                                    </div>
                                    <div class="location">
                                        <span class="info-label">Location</span>
                                        <span class="info-value location">35303</span>
                                    </div>
                                </div>
            
                                <div class="section-row">
                                    <div class="info-box" style="width: 100%;">
                                        <span class="info-label">Instructor</span>
                                        <span class="info-value instructor-name">Ibrahim Althomali</span>
                                    </div>
                                </div>
                            </div>
                        </div>
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
        var progressBarId = '<%= progressBar.ClientID %>';
        var hoursLabelId = '<%= hoursChosenLabel.ClientID %>';
    </script>

    <script src="/Scripts/studentPage.js" type="text/javascript"></script>

</asp:Content>
