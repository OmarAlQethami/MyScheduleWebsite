<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DepartmentHeadPage.aspx.cs" Inherits="MyScheduleWebsite.DepartmentHeadPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/DepartmentHeadStyles.css">
     <asp:HiddenField ID="hdnActiveTab" runat="server" Value="students" ClientIDMode="Static"/>
    <asp:HiddenField ID="hdnSelectedOrder" runat="server" />
    <asp:HiddenField ID="hdnOrderStatusData" runat="server" />
    <asp:HiddenField ID="hdnWaitlistStatusData" runat="server" />

    <div class="faculty-container">
        <div class="faculty-header">
            <div class="faculty-info">
                <asp:Label ID="lblFacultyName" runat="server" CssClass="faculty-name"></asp:Label>
                <asp:Label ID="lblMajor" runat="server" CssClass="faculty-major"></asp:Label>
                <asp:Label ID="lblUniversity" runat="server" CssClass="faculty-university"></asp:Label>
            </div>
    
            <div class="stats-bar">
                <asp:Label ID="lblTotalStudents" runat="server" CssClass="stat-badge total-students"> Total Students: N/A</asp:Label>
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

                <asp:LinkButton ID="btnFaculties" runat="server" 
                CssClass="tab-link" 
                OnClientClick="setActiveTab('facultyInfoTab'); return false;" 
                data-target="facultyInfoTab">
                    Faculty
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
                    DataKeyNames="OrderId"
                    AllowSorting="true"
                    OnSorting="gvStudents_Sorting">
                    <Columns>
                        <asp:BoundField DataField="studentUniId" HeaderText="Student ID" SortExpression="studentUniId" />
                        <asp:BoundField DataField="currentLevel" HeaderText="Level" SortExpression="currentLevel" />
        
                        <asp:TemplateField HeaderText="Student Name" SortExpression="studentEnglishFirstName">
                            <ItemTemplate>
                                <%# Eval("studentEnglishFirstName") + " " + Eval("studentEnglishLastName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
        
                        <asp:TemplateField HeaderText="Order Status" SortExpression="status">
                            <ItemTemplate>
                                <span class='status-badge <%# GetStatusClass(Eval("status")) %>'>
                                    <%# GetOrderStatus(Eval("status")) %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
        
                        <asp:BoundField DataField="OrderDate" HeaderText="Order Date" 
                            DataFormatString="{0:dd MMM yyyy hh:mm tt}" HtmlEncode="false" SortExpression="OrderDate" />
        
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnView" runat="server" CssClass="action-btn view" 
                                    Text="View" CommandArgument='<%# Eval("OrderId") %>' 
                                    OnClick="btnView_Click"
                                    ClientIDMode="Predictable" 
                                    Visible='<%# Eval("OrderId") != DBNull.Value %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

           <div id="facultyInfoTab" class="tab-content">
                 <asp:GridView ID="gvFacultyInfo" runat="server" 
                    CssClass="students-table"
                    AutoGenerateColumns="false" 
                    OnPreRender="gvFacultyInfo_PreRender"
                    EmptyDataText="No records found">
            <Columns>
                <asp:TemplateField HeaderText="Full Name">
                    <ItemTemplate>
                           <%# $"{Eval("facultyEnglishFirstName")} {Eval("facultyEnglishLastName")}".Trim() %>
                    </ItemTemplate>
                </asp:TemplateField>
            
                 <asp:BoundField DataField="email" HeaderText="Email" />
                 <asp:BoundField DataField="universityEnglishName" HeaderText="University" />
                 <asp:BoundField DataField="majorEnglishName" HeaderText="Major" />
            </Columns>
                 </asp:GridView>
            </div>

            <div id="waitlists" class="tab-content">
                <asp:GridView ID="gvWaitlists" runat="server" CssClass="waitlist-table" 
                    AutoGenerateColumns="false" 
                    OnPreRender="gvWaitlists_PreRender"
                    OnRowDataBound="gvWaitlists_RowDataBound"
                    ClientIDMode="AutoID"
                    AllowSorting="true"
                    OnSorting="gvWaitlists_Sorting">
                    <Columns>
                        <asp:BoundField DataField="subjectCode" HeaderText="Subject Code" SortExpression="subjectCode" />
                        <asp:BoundField DataField="subjectEnglishName" HeaderText="Subject Name" SortExpression="subjectEnglishName" />
                        <asp:BoundField DataField="requestedSemester" HeaderText="Semester" SortExpression="requestedSemester" />
                        <asp:BoundField DataField="totalStudents" HeaderText="Waitlisted Students" SortExpression="totalStudents" />
    
                        <asp:TemplateField HeaderText="Students">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlStudents" runat="server" CssClass="student-dropdown" ClientIDMode="Predictable">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
    
                        <asp:TemplateField HeaderText="Status" SortExpression="status">
                            <ItemTemplate>
                                <span class='status-badge <%# Eval("status").ToString().ToLower() %>'>
                                    <%# Eval("status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
    
                        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="180px">
                            <ItemTemplate>
                                <asp:Button ID="btnApprove" runat="server" Text="Approve" 
                                    CssClass="action-btn approve" 
                                    ClientIDMode="Predictable"
                                    CommandArgument='<%# Eval("subjectId") + "|" + Eval("requestedSemester") %>' 
                                    OnClick="btnApprove_Click" 
                                    Visible='<%# Eval("status").ToString() == "Pending" %>' />
        
                                <asp:Button ID="btnDeny" runat="server" Text="Deny" 
                                    CssClass="action-btn deny" 
                                    ClientIDMode="Predictable"
                                    CommandArgument='<%# Eval("subjectId") + "|" + Eval("requestedSemester") %>' 
                                    OnClick="btnDeny_Click" 
                                    Visible='<%# Eval("status").ToString() == "Pending" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div id="dashboard" class="tab-content">
                <div class="dashboard-container">
                    <div class="dashboard-stats-container">
                        <div class="stats-section">
                            <h3 class="section-header">Orders Statistics</h3>
                            <div class="stats-row">
                                <div class="stat-item">
                                    <div class="stat-value"><%= OrderStatuses["Approved"] %></div>
                                    <div class="stat-label">Approved</div>
                                </div>
                                <div class="stat-item">
                                    <div class="stat-value"><%= OrderStatuses["Pending"] %></div>
                                    <div class="stat-label">Pending</div>
                                </div>
                                <div class="stat-item">
                                    <div class="stat-value"><%= OrderStatuses["Not Ordered"] %></div>
                                    <div class="stat-label">Not Ordered</div>
                                </div>
                            </div>
                            <div class="chart-card">
                                <canvas id="orderChart"></canvas>
                            </div>
                        </div>

                        <div class="stats-section">
                            <h3 class="section-header">Waitlists Statistics</h3>
                            <div class="stats-row">
                                <div class="stat-item">
                                    <div class="stat-value"><%= WaitlistStatuses["Approved"] %></div>
                                    <div class="stat-label">Approved</div>
                                </div>
                                <div class="stat-item">
                                    <div class="stat-value"><%= WaitlistStatuses["Pending"] %></div>
                                    <div class="stat-label">Pending</div>
                                </div>
                                <div class="stat-item">
                                    <div class="stat-value"><%= WaitlistStatuses["Denied"] %></div>
                                    <div class="stat-label">Denied</div>
                                </div>
                            </div>
                            <div class="chart-card">
                                <canvas id="waitlistChart"></canvas>
                            </div>
                        </div>
                    </div>


                    <div class="data-tables">
                        <div class="table-card">
                            <h4>Most Ordered Subjects</h4>
                            <asp:GridView ID="gvPopularSubjects" runat="server" CssClass="dashboard-table"
                                AutoGenerateColumns="false">
                                <Columns>
                                    <asp:BoundField DataField="SubjectCode" HeaderText="Code" />
                                    <asp:BoundField DataField="SubjectName" HeaderText="Subject Name" />
                                    <asp:BoundField DataField="RequestCount" HeaderText="Orders" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="table-card">
                            <h4>Top Waitlisted Subjects</h4>
                            <asp:GridView ID="gvTopWaitlists" runat="server" CssClass="dashboard-table" 
                                AutoGenerateColumns="false">
                                <Columns>
                                    <asp:BoundField DataField="SubjectCode" HeaderText="Code" />
                                    <asp:BoundField DataField="SubjectName" HeaderText="Subject Name" />
                                    <asp:BoundField DataField="WaitCount" HeaderText="Waitlists" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <div id="majorPlan" class="tab-content">
                <div class="mb-3">
    <asp:Button ID="btnOpenSubjectPopup" runat="server" Text="Add Subject"
        CssClass="btn btn-primary mb-3"
        OnClientClick="openSubjectPopup(); return false;" />
</div>

    <asp:GridView ID="gvMajorPlan" runat="server" CssClass="major-plan-table" 
        AutoGenerateColumns="false" 
        OnRowDeleting="gvMajorPlan_RowDeleting"
    OnRowEditing="gvMajorPlan_RowEditing"
    OnRowUpdating="gvMajorPlan_RowUpdating"
    OnRowCancelingEdit="gvMajorPlan_RowCancelingEdit"
    OnRowDataBound="gvMajorPlan_RowDataBound"
    DataKeyNames="subjectCode"> 
        <Columns>
            <asp:TemplateField HeaderText="Level">
    <ItemTemplate>
        <%# Eval("subjectLevel") %>
    </ItemTemplate>
    <EditItemTemplate>
        <asp:TextBox ID="txtSubjectLevel" runat="server" 
            Text='<%# Bind("subjectLevel") %>' 
            CssClass="form-control" />
    </EditItemTemplate>
</asp:TemplateField>

<asp:TemplateField HeaderText="Subject Code">
    <ItemTemplate>
        <%# Eval("subjectCode") %>
    </ItemTemplate>
    <EditItemTemplate>
        <asp:TextBox ID="txtSubjectCode" runat="server" 
            Text='<%# Bind("subjectCode") %>' 
            CssClass="form-control" />
    </EditItemTemplate>
</asp:TemplateField>
            
            <asp:TemplateField HeaderText="Subject Name">
                <ItemTemplate>
                    <%# Eval("subjectEnglishName") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtSubjectEnglishName" runat="server" Text='<%# Bind("subjectEnglishName") %>' CssClass="form-control" />
                </EditItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Credit Hours">
                <ItemTemplate>
                    <%# Eval("creditHours") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtCreditHours" runat="server" Text='<%# Bind("creditHours") %>' CssClass="form-control" />
                </EditItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Subject Type">
    <ItemTemplate>
        <%# Eval("subjectTypeEnglishName") %>
    </ItemTemplate>
    <EditItemTemplate>
        <asp:DropDownList ID="ddlSubjectType" runat="server" 
                          CssClass="form-control"
                          DataTextField="subjectTypeEnglishName"
                          DataValueField="subjectTypeId">
        </asp:DropDownList>
    </EditItemTemplate>
</asp:TemplateField>

            <asp:TemplateField HeaderText="Prerequisites">
                <ItemTemplate>
                    <%# Eval("prerequisites") %>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtPrerequisites" runat="server" 
                        Text='<%# Bind("prerequisites") %>' 
                        CssClass="form-control" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="Edit" CssClass="btn btn-primary btn-edit" />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete this subject?');" CssClass="btn btn-danger btn-delete" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="Update" CssClass="btn btn-success btn-update" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="Cancel" CssClass="btn btn-secondary btn-cancel" />
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>

            <div id="curriculum" class="tab-content">
               <div class="mb-3">
                <asp:Button ID="btnAddSection" runat="server" Text="Add New Section" CssClass="btn btn-primary" OnClientClick="openPopup(); return false;" />
              </div>
                <asp:GridView ID="gvCurriculum" runat="server" CssClass="curriculum-table" 
    AutoGenerateColumns="false" OnPreRender="gvCurriculum_PreRender"
    OnRowEditing="gvCurriculum_RowEditing"
    OnRowUpdating="gvCurriculum_RowUpdating"
    OnRowCancelingEdit="gvCurriculum_RowCancelingEdit"
    OnRowDeleting="gvCurriculum_RowDeleting"
    DataKeyNames="sectionNumber">
    <Columns>
        <asp:BoundField DataField="sectionNumber" HeaderText="Section #" ReadOnly="true" />
        <asp:BoundField DataField="subjectCode" HeaderText="Subject Code" />
        <asp:BoundField DataField="subjectEnglishName" HeaderText="Subject Name" />
        <asp:BoundField DataField="creditHours" HeaderText="Credits" />
        <asp:BoundField DataField="capacity" HeaderText="Capacity" />
        <asp:BoundField DataField="registeredStudents" HeaderText="Registered" ReadOnly="true" />
        
        <asp:TemplateField HeaderText="Schedule">
            <ItemTemplate>
                <%# FormatScheduleDetails(Eval("day"), Eval("startTime"), Eval("endTime"), Eval("location")) %>
            </ItemTemplate>
            <EditItemTemplate>
                Day: <asp:TextBox ID="txtDay" runat="server" Text='<%# Bind("day") %>' Width="50" />
                <br />
                From: <asp:TextBox ID="txtStartTime" runat="server" Text='<%# Bind("startTime") %>' Width="50" />
                To: <asp:TextBox ID="txtEndTime" runat="server" Text='<%# Bind("endTime") %>' Width="50" />
                <br />
                Location: <asp:TextBox ID="txtLocation" runat="server" Text='<%# Bind("location") %>' Width="70" />
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:BoundField DataField="instuctorArabicName" HeaderText="Instructor" NullDisplayText="TBA" />

        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="Edit" CssClass="btn btn-primary btn-edit" />
                <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete this subject?');" CssClass="btn btn-danger btn-delete"  />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="Update" CssClass="btn btn-success btn-update" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="Cancel" CssClass="btn btn-secondary btn-cancel"/>
            </EditItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

            </div>

            <div id="sectionPopup" style="display:none; position:fixed; top:20%; left:30%; width:40%; background:#fff; padding:20px; box-shadow:0 0 10px gray; z-index:1000;">
    <h3>Add New Section</h3>
    <asp:TextBox ID="txtPopupSubjectCode" runat="server" placeholder="Subject Code" CssClass="form-control" /><br />
    <asp:TextBox ID="txtPopupregisteredStudents" runat="server" placeholder="Number Of Students Registered" CssClass="form-control" /><br />
    <asp:TextBox ID="txtPopupCapacity" runat="server" placeholder="Capacity" CssClass="form-control" /><br />
    <asp:TextBox ID="txtPopupDay" runat="server" placeholder="Day" CssClass="form-control" /><br />
    <asp:TextBox ID="txtPopupStartTime" runat="server" placeholder="Start Time" CssClass="form-control" /><br />
    <asp:TextBox ID="txtPopupEndTime" runat="server" placeholder="End Time" CssClass="form-control" /><br />
    <asp:TextBox ID="txtPopupLocation" runat="server" placeholder="Location" CssClass="form-control" /><br />
    <asp:TextBox ID="txtPopupInstructor" runat="server" placeholder="Instructor Arabic Name" CssClass="form-control" /><br />

    <asp:Button ID="btnSaveSection" runat="server" Text="Save Section" OnClick="btnSaveSection_Click" CssClass="btn btn-success" />
    <input type="button" value="Cancel" class="btn btn-secondary" onclick="closePopup()" />
</div>


           
<div id="addSubjectModal" style="display:none; position:fixed; top:50%; left:50%; transform:translate(-50%, -50%);
    background-color:white; border:1px solid #ccc; padding:20px; z-index:1000; width:600px; max-width:95%; box-shadow:0 0 10px rgba(0,0,0,0.3);">
    
    <h3>Add Subject</h3>
    <hr />

    <label>Subject Code:</label><br />
    <asp:TextBox ID="txtSubjectCode" runat="server" CssClass="form-control" /><br /><br />

    <label>Subject Level:</label><br />
    <asp:DropDownList ID="ddlSubjectLevel" runat="server" CssClass="form-control">
        <asp:ListItem Text="1" Value="1" />
        <asp:ListItem Text="2" Value="2" />
        <asp:ListItem Text="3" Value="3" />
        <asp:ListItem Text="4" Value="4" />
        <asp:ListItem Text="5" Value="5" />
        <asp:ListItem Text="6" Value="6" />
        <asp:ListItem Text="7" Value="7" />
        <asp:ListItem Text="8" Value="8" />
        <asp:ListItem Text="9" Value="9" />
        <asp:ListItem Text="10" Value="10" />
    </asp:DropDownList><br /><br />

    <label>English Name:</label><br />
    <asp:TextBox ID="txtSubjectEnglishName" runat="server" CssClass="form-control" /><br /><br />

    <label>Arabic Name:</label><br />
    <asp:TextBox ID="txtSubjectArabicName" runat="server" CssClass="form-control" /><br /><br />

    <label>Credit Hours:</label><br />
    <asp:TextBox ID="txtCreditHours" runat="server" CssClass="form-control" TextMode="Number" /><br /><br />

    <label>Subject Type:</label><br />
    <asp:DropDownList ID="ddlSubjectType" runat="server" CssClass="form-control">
        <asp:ListItem Text="Select Type" Value="" Selected="True" Disabled="True" />
        <asp:ListItem Text="Compulsory College" Value="1" />
        <asp:ListItem Text="Compulsory University" Value="2" />
        <asp:ListItem Text="Elective College" Value="3" />
        <asp:ListItem Text="Elective University" Value="4" />
    </asp:DropDownList><br /><br />

    <label>Prerequisites:</label><br />
    <asp:TextBox ID="txtPrerequisites" runat="server" CssClass="form-control" Placeholder="Example: 101,102" /><br /><br />

    <asp:Button ID="btnSubmit" runat="server" Text="Save Subject" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
    <input type="button" value="Cancel" class="btn btn-secondary" onclick="closeSubjectPopup()" />

</div>



            <div class="modal-overlay" id="orderModal" onclick="hideModal()">
                <div class="modal-content" onclick="event.stopPropagation()">
                    <button class="modal-close" onclick="hideModal()">&times;</button>
        
                    <div class="student-info-header">
                        <div style="display: flex; gap: 15px;">
                            <div class="info-badge">
                                <span class="info-label"></span>
                                <asp:Label ID="lblModalStudentName" runat="server" cssclass="labels"
                                    Text=""  />
                            </div>
        
                            <div class="info-badge">
                                <span class="info-label"></span>
                                <asp:Label ID="lblModalStudentLevel" runat="server" cssclass="labels"
                                    Text="" />
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
                            <asp:BoundField DataField="creditHours" HeaderText="Credits" />
                            <asp:TemplateField HeaderText="Schedule">
                                <ItemTemplate>
                                    <%# Eval("FormattedSchedule") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Instructor" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <div class="instructor-cell">
                                        <%# Eval("Instructor") %>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>



   <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    
    <script src="scripts/facultyPage.js"></script>

   
    <script type="text/javascript">
    function openPopup() {
        document.getElementById('sectionPopup').style.display = 'block';
    }

    function closePopup() {
        document.getElementById('sectionPopup').style.display = 'none';
        }
    </script>

      <script type="text/javascript">
          function openSubjectPopup() {
              document.getElementById('addSubjectModal').style.display = 'block';
          }

          function closeSubjectPopup() {
              document.getElementById('addSubjectModal').style.display = 'none';
          }
      </script>





    <script>
        let orderChart = null;
        let waitlistChart = null;
        let chartsInitialized = false;
        let chartInitializationPending = false;
        let resizeObserver = null;
        let tabHandlerRegistered = false;

        document.addEventListener("DOMContentLoaded", function () {
            if (!tabHandlerRegistered) {
                document.querySelectorAll('.tab-link').forEach(link => {
                    link.addEventListener('click', handleTabClick);
                });
                tabHandlerRegistered = true;
            }

            const initialTab = document.getElementById('hdnActiveTab')?.value || 'students';
            setActiveTab(initialTab);
        });

        function handleTabClick(e) {
            const targetTab = this.dataset.target;

            setActiveTab(targetTab);

            if (targetTab === 'dashboard') {
                setTimeout(() => {
                    if (!chartsInitialized) {
                        initializeCharts();
                    }
                }, 1);
            }
        }

        function setActiveTab(tabId) {

            const tabControl = document.getElementById('<%= hdnActiveTab.ClientID%>');
            if (tabControl) {
                tabControl.value = tabId;
            }

            document.querySelectorAll('.tab-link').forEach(link => {
                link.classList.toggle('active', link.dataset.target === tabId);
            });

            document.querySelectorAll('.tab-content').forEach(content => {
                content.classList.toggle('active', content.id === tabId);
            });
        }

        function initializeCharts() {
            if (chartInitializationPending || chartsInitialized) return;
            chartInitializationPending = true;

            const orderCanvas = document.getElementById('orderChart');
            const waitlistCanvas = document.getElementById('waitlistChart');

            try {
                orderChart = new Chart(orderCanvas.getContext('2d'), {
                    type: 'doughnut',
                    data: {
                        labels: ['Approved', 'Pending', 'Not Ordered'],
                        datasets: [{
                            data: window.orderStatusData || [0, 0, 0],
                            backgroundColor: ['#4CAF50', '#FFC107', '#E91E63'],
                            borderWidth: 0
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        animation: { duration: 0 },
                        plugins: {
                            legend: {
                                position: 'bottom',
                                labels: { boxWidth: 12, padding: 20 }
                            }
                        }
                    }
                });

                waitlistChart = new Chart(waitlistCanvas.getContext('2d'), {
                    type: 'doughnut',
                    data: {
                        labels: ['Approved', 'Pending', 'Denied'],
                        datasets: [{
                            data: window.waitlistStatusData || [0, 0, 0],
                            backgroundColor: ['#4CAF50', '#FFC107', '#E91E63'],
                            borderWidth: 0
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        animation: { duration: 0 },
                        plugins: {
                            legend: {
                                position: 'bottom',
                                labels: { boxWidth: 12, padding: 20 }
                            }
                        }
                    }
                });

                if (!resizeObserver) {
                    resizeObserver = new ResizeObserver(entries => {
                        orderChart?.resize();
                        waitlistChart?.resize();
                    });
                    resizeObserver.observe(orderCanvas.parentElement);
                    resizeObserver.observe(waitlistCanvas.parentElement);
                }

                chartsInitialized = true;
            } catch (error) {
                console.error('Chart error:', error);
            }

            chartInitializationPending = false;
        }

        function showModal() {
            document.getElementById('orderModal').style.display = 'flex';
        }

        function hideModal() {
            document.getElementById('orderModal').style.display = 'none';
        }

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') hideModal();
        });

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
    </script>
</asp:Content>
