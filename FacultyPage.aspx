<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="FacultyPage.aspx.cs" 
    Inherits="MyScheduleWebsite.FacultyPage" ClientIDMode="Static" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/FacultyStyles.css">
    <asp:HiddenField ID="hdnActiveTab" runat="server" Value="students" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdnSelectedOrder" runat="server" />

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
                    AutoGenerateColumns="false" 
                    OnPreRender="gvStudents_PreRender" 
                    DataKeyNames="OrderId">
                    <HeaderStyle CssClass="modal-grid-header" />
                    <RowStyle CssClass="modal-grid-row" />
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
                        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="180px">
                            <ItemTemplate>
                                <asp:Button ID="btnView" runat="server" CssClass="action-btn view" 
                                    Text="View" CommandArgument='<%# Eval("OrderId") %>' 
                                    OnClick="btnView_Click"
                                    Enabled='<%# Eval("OrderId") != DBNull.Value %>' />
                                </button>
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
                    AutoGenerateColumns="false" OnPreRender="gvWaitlists_PreRender"
                    OnRowDataBound="gvWaitlists_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="subjectCode" HeaderText="Subject Code" />
                        <asp:BoundField DataField="subjectEnglishName" HeaderText="Subject Name" />
                        <asp:BoundField DataField="totalStudents" HeaderText="Total Waitlisted" />
                        <asp:TemplateField HeaderText="Interested Students">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlStudents" runat="server" CssClass="student-dropdown">
                                </asp:DropDownList>
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

            <div class="modal-overlay" id="orderModal" onclick="hideModal()">
    <div class="modal-content" onclick="event.stopPropagation()">
        <button class="modal-close" onclick="hideModal()">&times;</button>
        
        <div class="student-info-header">
            <h3 style="margin: 0 0 10px 0;">Student Information</h3>
            <div style="display: flex; gap: 15px;">
                <div class="info-badge">
                    <span class="info-label">Name:</span>
                    <asp:Label ID="lblModalStudentName" runat="server" Text="N/A" />
                </div>
                <div class="info-badge">
                    <span class="info-label">Level:</span>
                    <asp:Label ID="lblModalStudentLevel" runat="server" Text="N/A" />
                </div>
            </div>
        </div>

        <asp:Literal ID="litModalError" runat="server" Visible="false" />
        <asp:GridView ID="gvOrderDetails" runat="server" CssClass="students-table"
            AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="SubjectCode" HeaderText="Subject Code" />
                    <asp:BoundField DataField="SubjectName" HeaderText="Subject Name" />
                    <asp:BoundField DataField="SectionNumber" HeaderText="Section" />
                    <asp:BoundField DataField="CreditHours" HeaderText="Credits" />
                    <asp:BoundField DataField="Schedule" HeaderText="Schedule" />
                    <asp:BoundField DataField="Instructor" HeaderText="Instructor" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    
    <script src="scripts/facultyPage.js"></script>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
            try {
                const activeTabControl = document.getElementById('hdnActiveTab');
                const activeTab = activeTabControl ? activeTabControl.value : 'students';
                setActiveTab(activeTab);

                const ctx = document.getElementById('orderChart')?.getContext('2d');
                if (ctx) {
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
                }
            } catch (e) {
                console.error('Initialization error:', e);
            }
        });

        function setActiveTab(tabId) {
            document.getElementById('<%= hdnActiveTab.ClientID%>').value = tabId;
            document.querySelectorAll('.tab-link').forEach(link => link.classList.remove('active'));
            document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));
            document.querySelector(`[data-target="${tabId}"]`).classList.add('active');
            document.getElementById(tabId).classList.add('active');
        }

        function toggleDetails(orderId) {
            const detailsDiv = document.getElementById(`details_${orderId}`);
            if (!detailsDiv) return;

            detailsDiv.style.display = detailsDiv.style.display === 'none' ? 'block' : 'none';

            const btn = document.querySelector(`[data-orderid="${orderId}"]`);
            btn.classList.toggle('active');
        }

        function showOrderDetails(orderId) {
            fetch('FacultyPage.aspx/GetOrderDetails', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ orderId: orderId }),
                credentials: 'same-origin'
            })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        throw new Error(`HTTP error! status: ${response.status} - ${text}`);
                    });
                }
                return response.json();
            })
            .then(data => {
                const errorContainer = document.getElementById('modalError');
                errorContainer.style.display = 'none';
        
                if (data.error) {
                    errorContainer.textContent = data.error + (data.details ? ` (${data.details})` : '');
                    errorContainer.style.display = 'block';
                    return;
                }   
        
                const details = data.d || [];
                const tbody = document.getElementById('modalTableBody');
                tbody.innerHTML = '';

                if (details.length === 0) {
                    tbody.innerHTML = '<tr><td colspan="6">No order details found</td></tr>';
                    return;
                }

                details.forEach(item => {
                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <td>${item.SubjectCode || ''}</td>
                        <td>${item.SubjectName || ''}</td>
                        <td>${item.SectionNumber || ''}</td>
                        <td>${item.CreditHours || ''}</td>
                        <td>${item.Schedule || ''}</td>
                        <td>${item.Instructor || 'TBA'}</td>
                    `;
                    tbody.appendChild(row);
                });

                document.getElementById('orderDetailsModal').style.display = 'block';
            })
            .catch(error => {
                console.error('Error:', error);
                const tbody = document.getElementById('modalTableBody');
                tbody.innerHTML = '<tr><td colspan="6">Error loading details</td></tr>';
                document.getElementById('orderDetailsModal').style.display = 'block';
            });
        }

        function showModal() {
            document.getElementById('orderModal').style.display = 'flex';
        }

        function hideModal() {
            document.getElementById('orderModal').style.display = 'none';
        }

        // Close modal when pressing ESC
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') hideModal();
        });
    </script>
</asp:Content>