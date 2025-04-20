<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="FacultyPage.aspx.cs" 
    Inherits="MyScheduleWebsite.FacultyPage" ClientIDMode="Static" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/FacultyStyles.css">
    <asp:HiddenField ID="hdnActiveTab" runat="server" Value="students" />

    <div class="faculty-container">
        <div class="faculty-header">
            <div class="faculty-info">
                <asp:Label ID="lblFacultyName" runat="server" CssClass="faculty-name"></asp:Label>
                <asp:Label ID="lblMajor" runat="server" CssClass="faculty-major"></asp:Label>
                <asp:Label ID="lblUniversity" runat="server" CssClass="faculty-university"></asp:Label>
            </div>
    
            <div class="stats-bar">
                <asp:Label ID="lblTotalStudents" runat="server" CssClass="stat-badge total-students">Total Students: N/A</asp:Label>
                <asp:Label ID="lblApprovedOrders" runat="server" CssClass="stat-badge approved-orders">Approved Orders: N/A</asp:Label>
                <asp:Label ID="lblPendingActions" runat="server" CssClass="stat-badge pending-actions">Pending Actions: N/A</asp:Label>
            </div>
        </div>

        <div class="tabs-container">
            <div class="tab-nav">
                <asp:LinkButton ID="btnStudents" runat="server" 
                    CssClass="tab-link" 
                    OnClientClick="setActiveTab('students'); return false;" 
                    data-target="students">
                    Students
                </asp:LinkButton>

                <asp:LinkButton ID="btnWaitlists" runat="server" 
                    CssClass="tab-link" 
                    OnClientClick="setActiveTab('waitlists'); return false;" 
                    data-target="waitlists">
                    Waitlists
                </asp:LinkButton>
                
                <asp:LinkButton ID="btnDashboard" runat="server" 
                    CssClass="tab-link" 
                    OnClientClick="setActiveTab('dashboard'); return false;" 
                    data-target="dashboard">
                    Dashboard
                </asp:LinkButton>

                <asp:LinkButton ID="btnMajorPlan" runat="server" 
                    CssClass="tab-link" 
                    OnClientClick="setActiveTab('majorPlan'); return false;" 
                    data-target="majorPlan">
                    Major Plan
                </asp:LinkButton>
                
                <asp:LinkButton ID="btnCurriculum" runat="server" 
                    CssClass="tab-link" 
                    OnClientClick="setActiveTab('curriculum'); return false;" 
                    data-target="curriculum">
                    Curriculum
                </asp:LinkButton>
            </div>

            <div id="students" class="tab-content active">
                <div class="filters">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" 
                        placeholder="Search students..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged" AutoComplete="off"></asp:TextBox>
                </div>

                <asp:GridView ID="gvStudents" runat="server" CssClass="students-table"
                    AutoGenerateColumns="false" OnPreRender="gvStudents_PreRender">
                    <Columns>
                        <asp:BoundField DataField="StudentUniId" HeaderText="Student ID" />
                        <asp:BoundField DataField="CurrentLevel" HeaderText="Level" />
                        <asp:TemplateField HeaderText="Student Name">
                            <ItemTemplate>
                                <%# Eval("studentEnglishFirstName") + " " + Eval("studentEnglishLastName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Order Status">
                            <ItemTemplate>
                                <span class='status-badge <%# GetStatusClass(Eval("status")) %>'>
                                    <%# GetOrderStatus(Eval("status")) %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="OrderDate" HeaderText="Order Date" 
                            DataFormatString="{0:dd MMM yyyy hh:mm tt}" HtmlEncode="false" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnViewOrder" runat="server" CssClass="action-btn view" 
                                    Text="View" CommandArgument='<%# Eval("StudentId") %>' 
                                    OnClick="btnViewOrder_Click" />
                                <asp:Button ID="btnExport" runat="server" CssClass="action-btn export" 
                                    Text="Export" CommandArgument='<%# Eval("OrderId") %>' 
                                    OnClick="btnExport_Click" Visible='<%# Eval("OrderId") != DBNull.Value %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div id="waitlists" class="tab-content">
                <asp:GridView ID="gvWaitlists" runat="server" CssClass="waitlist-table" 
                    AutoGenerateColumns="false" OnPreRender="gvWaitlists_PreRender">
                    <Columns>
                        <asp:BoundField DataField="subjectCode" HeaderText="Subject Code" />
                        <asp:BoundField DataField="subjectEnglishName" HeaderText="Subject Name" />
                        <asp:BoundField DataField="totalStudents" HeaderText="Total Waitlisted" />
                        <asp:TemplateField HeaderText="Highest Priority Students">
                            <ItemTemplate>
                                <%# FormatStudentList(Eval("topStudents")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="requestedSemester" HeaderText="Semester" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='status-badge <%# Eval("status").ToString().ToLower() %>'>
                                    <%# Eval("status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div id="dashboard" class="tab-content">
                <div class="chart-card">
                    <h3>Order Status Distribution</h3>
                    <canvas id="orderChart" width="100" height="100"></canvas>
                </div>
            </div>

            <div id="majorPlan" class="tab-content">
                <asp:GridView ID="gvMajorPlan" runat="server" CssClass="major-plan-table" 
                    AutoGenerateColumns="false" OnPreRender="gvMajorPlan_PreRender">
                    <Columns>
                        <asp:BoundField DataField="subjectLevel" HeaderText="Level" />
                        <asp:BoundField DataField="subjectCode" HeaderText="Subject Code" />
                        <asp:BoundField DataField="subjectEnglishName" HeaderText="Subject Name" />
                        <asp:BoundField DataField="creditHours" HeaderText="Credit Hours" />
                        <asp:TemplateField HeaderText="Subject Type">
                            <ItemTemplate>
                                <%# Eval("subjectTypeEnglishName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Prerequisites">
                            <ItemTemplate>
                                <%# string.IsNullOrEmpty(Eval("prerequisites").ToString()) ? "" : Eval("prerequisites") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div id="curriculum" class="tab-content">
                <asp:GridView ID="gvCurriculum" runat="server" CssClass="curriculum-table" 
                    AutoGenerateColumns="false" OnPreRender="gvCurriculum_PreRender">
                    <Columns>
                        <asp:BoundField DataField="sectionNumber" HeaderText="Section #" />
                        <asp:BoundField DataField="subjectCode" HeaderText="Subject Code" />
                        <asp:BoundField DataField="subjectEnglishName" HeaderText="Subject Name" />
                        <asp:BoundField DataField="creditHours" HeaderText="Credits" />
                        <asp:BoundField DataField="capacity" HeaderText="Capacity" />
                        <asp:BoundField DataField="registeredStudents" HeaderText="Registered" />
                        <asp:TemplateField HeaderText="Schedule">
                            <ItemTemplate>
                                <%# FormatScheduleDetails(Eval("day"), Eval("startTime"), Eval("endTime"), Eval("location")) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="instuctorArabicName" HeaderText="Instructor" 
                            NullDisplayText="TBA" />
                    </Columns>
                </asp:GridView>
            </div>

            
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            const activeTab = document.getElementById('<%= hdnActiveTab.ClientID %>').value;
        setActiveTab(activeTab);
        
        const ctx = document.getElementById('orderChart').getContext('2d');
        new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['Approved', 'Pending', 'Not Ordered'],
                datasets: [{
                    data: [85, 5, 30],
                    backgroundColor: ['#4CAF50', '#FFC107', '#E91E63']
                }]
            }
            });
        });

        function setActiveTab(tabId) {
            document.getElementById('<%= hdnActiveTab.ClientID %>').value = tabId;
                document.querySelectorAll('.tab-link').forEach(link => link.classList.remove('active'));
                document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));
                document.querySelector(`[data-target="${tabId}"]`).classList.add('active');
                document.getElementById(tabId).classList.add('active');
            }
    </script>

</asp:Content>