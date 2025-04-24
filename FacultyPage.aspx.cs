using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite
{
    public partial class FacultyPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetFacultyUniversityAndMajor();
                LoadDashboard();
                BindStudents();
                BindMajorPlan();
                BindCurriculum();
                BindWaitlists();
            }
        }

        private int curriculumId = 1;

        private void BindMajorPlan()
        {
            if (ViewState["UniversityId"] == null || ViewState["MajorId"] == null) return;

            CRUD myCrud = new CRUD();
            string sql = @"SELECT s.subjectLevel, s.subjectCode, s.subjectEnglishName, 
                          s.creditHours, st.subjectTypeEnglishName, s.prerequisites
                   FROM subjects s
                   INNER JOIN subjectType st ON s.subjectTypeId = st.subjectTypeId
                   WHERE s.universityId = @UniversityId 
                     AND s.majorId = @MajorId
                   ORDER BY s.subjectLevel, s.subjectCode";

            Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "@UniversityId", ViewState["UniversityId"] },
        { "@MajorId", ViewState["MajorId"] }
    };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            gvMajorPlan.DataSource = dt;
            gvMajorPlan.DataBind();
        }

        protected void gvMajorPlan_PreRender(object sender, EventArgs e)
        {
            if (gvMajorPlan.Rows.Count > 0)
            {
                gvMajorPlan.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        private void BindCurriculum()
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT s.sectionNumber, s.subjectCode, sub.subjectEnglishName, 
                          sub.creditHours, s.capacity, s.registeredStudents,
                          STRING_AGG(CONVERT(nvarchar, sd.day), ', ') AS day,
                          STRING_AGG(CONVERT(nvarchar, sd.startTime, 108), ', ') AS startTime,
                          STRING_AGG(CONVERT(nvarchar, sd.endTime, 108), ', ') AS endTime,
                          STRING_AGG(sd.location, ', ') AS location,
                          s.instuctorArabicName
                   FROM sections s
                   INNER JOIN sectionDetails sd ON s.sectionId = sd.sectionId
                   INNER JOIN subjects sub ON s.subjectCode = sub.subjectCode
                   WHERE s.curriculumId = @CurriculumId
                   GROUP BY s.sectionNumber, s.subjectCode, sub.subjectEnglishName, 
                            sub.creditHours, s.capacity, s.registeredStudents, 
                            s.instuctorArabicName";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@CurriculumId", curriculumId }
                };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            gvCurriculum.DataSource = dt;
            gvCurriculum.DataBind();
        }

        protected void gvCurriculum_PreRender(object sender, EventArgs e)
        {
            if (gvCurriculum.Rows.Count > 0)
            {
                gvCurriculum.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        private string GetDayName(object dayNumber)
        {
            if (dayNumber == null) return "TBA";

            int dayValue = Convert.ToInt32(dayNumber);

            if (dayValue >= 0 && dayValue <= 4)
            {
                return ((DayOfWeek)dayValue).ToString();
            }

            return "TBA";
        }

        public string FormatScheduleDetails(object day, object startTime, object endTime, object location)
        {
            int academicDay = Convert.ToInt32(day) - 1;

            DateTime start = DateTime.Parse(startTime.ToString());
            DateTime end = DateTime.Parse(endTime.ToString());

            string formattedStart = start.ToString("hh:mmtt");
            string formattedEnd = end.ToString("hh:mmtt");

            return $"{GetDayName(academicDay)} | {formattedStart}-{formattedEnd} | {location}";
        }

        protected void gvWaitlists_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortDirection = ViewState["WaitlistSortDirection"]?.ToString() ?? "ASC";

            if (ViewState["WaitlistSortExpression"]?.ToString() == e.SortExpression)
            {
                sortDirection = (sortDirection == "ASC") ? "DESC" : "ASC";
            }
            else
            {
                sortDirection = "ASC";
            }

            ViewState["WaitlistSortExpression"] = e.SortExpression;
            ViewState["WaitlistSortDirection"] = sortDirection;

            BindWaitlists(e.SortExpression, sortDirection);
        }

        private void BindWaitlists(string sortExpression = "subjectCode", string sortDirection = "ASC")
        {
            CRUD myCrud = new CRUD();
            string sql = $@"SELECT 
                s.subjectId,
                s.subjectCode,
                s.subjectEnglishName,
                w.requestedSemester,
                COUNT(w.studentId) AS totalStudents,
                STRING_AGG(CONCAT(stu.studentEnglishFirstName, ' ', stu.studentEnglishLastName, 
                           ' (', stu.studentUniId, ')'), '|') WITHIN GROUP (ORDER BY w.priority DESC) AS studentList,
                w.status
            FROM waitlist w
            INNER JOIN subjects s ON w.subjectId = s.subjectId
            INNER JOIN students stu ON w.studentId = stu.studentId
            WHERE s.universityId = @UniversityId
              AND s.majorId = @MajorId
            GROUP BY s.subjectId, s.subjectCode, s.subjectEnglishName, w.requestedSemester, w.status
            ORDER BY {ValidateWaitlistSortColumn(sortExpression)} {sortDirection}";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UniversityId", ViewState["UniversityId"] },
                    { "@MajorId", ViewState["MajorId"] }
                };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            gvWaitlists.DataSource = dt;
            gvWaitlists.DataBind();
        }

        private string ValidateWaitlistSortColumn(string columnName)
        {
            var allowedColumns = new List<string>
                {
                    "subjectCode",
                    "subjectEnglishName",
                    "requestedSemester",
                    "totalStudents",
                    "status"
                };

            return allowedColumns.Contains(columnName) ? columnName : "subjectCode";
        }

        protected void gvWaitlists_PreRender(object sender, EventArgs e)
        {
            if (gvWaitlists.Rows.Count > 0)
            {
                gvWaitlists.HeaderRow.TableSection = TableRowSection.TableHeader;

                string currentSort = ViewState["WaitlistSortExpression"]?.ToString();
                string sortDirection = ViewState["WaitlistSortDirection"]?.ToString();

                foreach (DataControlFieldHeaderCell header in gvWaitlists.HeaderRow.Cells
                    .OfType<DataControlFieldHeaderCell>())
                {
                    if (header.ContainingField.SortExpression == currentSort)
                    {
                        header.CssClass += sortDirection == "ASC" ? " sort-asc" : " sort-desc";
                    }
                }
            }
        }
        protected void gvWaitlists_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                string studentList = rowView["studentList"].ToString();

                DropDownList ddlStudents = (DropDownList)e.Row.FindControl("ddlStudents");
                if (ddlStudents != null)
                {
                    string[] students = studentList.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var student in students)
                    {
                        ddlStudents.Items.Add(new ListItem(student));
                    }

                    if (ddlStudents.Items.Count == 0)
                    {
                        ddlStudents.Items.Add("No students in waitlist");
                    }
                }
            }
        }

        public string FormatStudentList(object studentList)
        {
            if (studentList == DBNull.Value) return "No students in waitlist";

            var students = studentList.ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            return "<ul class='student-list'>" +
                   string.Join("", students.Select(s => $"<li>{s}</li>")) +
                   "</ul>";
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            UpdateWaitlistStatus(sender, "Approved");
        }

        protected void btnDeny_Click(object sender, EventArgs e)
        {
            UpdateWaitlistStatus(sender, "Denied");
        }

        private void UpdateWaitlistStatus(object sender, string newStatus)
        {
            Button btn = (Button)sender;
            string[] args = btn.CommandArgument.ToString().Split('|');
            int subjectId = Convert.ToInt32(args[0]);
            string semester = args[1];

            CRUD myCrud = new CRUD();
            string sql = @"UPDATE w
                  SET w.status = @Status,
                      w.statusChangedBy = @ChangedBy,
                      w.statusChangedDate = GETDATE()
                  FROM waitlist w
                  INNER JOIN subjects s ON w.subjectId = s.subjectId
                  WHERE s.subjectId = @SubjectId
                    AND w.requestedSemester = @Semester";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Status", newStatus },
                    { "@ChangedBy", User.Identity.Name },
                    { "@SubjectId", subjectId },
                    { "@Semester", semester }
                };

            int rowsAffected = myCrud.InsertUpdateDelete(sql, parameters);

            if (rowsAffected > 0)
            {
                BindWaitlists();
            }
            else
            {
                
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string requestId = btn.CommandArgument;
        }
        private void GetFacultyUniversityAndMajor()
        {
            Guid userId = GetUserId();
            CRUD myCrud = new CRUD();

            string sql = @"SELECT f.facultyEnglishFirstName, f.universityId, f.majorId, 
                          m.majorEnglishName, u.universityEnglishName 
                           FROM faculty f
                           INNER JOIN majors m ON f.majorId = m.majorId
                           INNER JOIN universities u ON f.universityId = u.universityId
                           WHERE f.UserId = @UserId";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                lblFacultyName.Text = $"Hello, {dt.Rows[0]["facultyEnglishFirstName"]}";
                lblMajor.Text = $"Major: {dt.Rows[0]["majorEnglishName"]}";
                lblUniversity.Text = $"University: {dt.Rows[0]["universityEnglishName"]}";

                ViewState["UniversityId"] = Convert.ToInt32(dt.Rows[0]["universityId"]);
                ViewState["MajorId"] = Convert.ToInt32(dt.Rows[0]["majorId"]);
            }
        }

        private Guid GetUserId()
        {
            if (Membership.GetUser() is MembershipUser user)
            {
                return (Guid)user.ProviderUserKey;
            }
            return Guid.Empty;
        }

        protected void gvStudents_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortDirection = ViewState["SortDirection"]?.ToString() ?? "ASC";

            if (ViewState["SortExpression"]?.ToString() == e.SortExpression)
            {
                sortDirection = (sortDirection == "ASC") ? "DESC" : "ASC";
            }
            else
            {
                sortDirection = "ASC";
            }

            ViewState["SortExpression"] = e.SortExpression;
            ViewState["SortDirection"] = sortDirection;

            BindStudents(e.SortExpression, sortDirection);
        }

        private void BindStudents(string sortExpression = "studentUniId", string sortDirection = "ASC")
        {
            CRUD myCrud = new CRUD();
            string sql = $@"SELECT 
                s.studentId, s.studentUniId, s.currentLevel, 
                s.studentEnglishFirstName, s.studentEnglishLastName,
                o.orderId, o.orderDate, o.status
                FROM students s
                LEFT JOIN orders o ON s.studentId = o.studentId
                WHERE s.universityId = @UniversityId 
                  AND s.majorId = @MajorId
                ORDER BY {ValidateSortColumn(sortExpression)} {sortDirection}";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UniversityId", ViewState["UniversityId"] },
                    { "@MajorId", ViewState["MajorId"] },
                    { "@Search", txtSearch.Text.Trim() }
                };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            gvStudents.DataSource = dt;
            gvStudents.DataBind();
        }

        private string ValidateSortColumn(string columnName)
        {
            var allowedColumns = new List<string>
                {
                    "studentUniId",
                    "currentLevel",
                    "studentEnglishFirstName",
                    "status",
                    "OrderDate"
                };

            return allowedColumns.Contains(columnName) ? columnName : "studentUniId";
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        private DataTable GetOrderDetails(int orderId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT 
                s.subjectCode AS SubjectCode,
                sub.subjectEnglishName AS SubjectName,
                s.sectionNumber AS SectionNumber,
                sub.creditHours AS CreditHours,
                STRING_AGG(CONVERT(nvarchar, sd.day), ', ') AS Days,
                STRING_AGG(CONVERT(nvarchar, sd.startTime, 108), ', ') AS StartTimes,
                STRING_AGG(CONVERT(nvarchar, sd.endTime, 108), ', ') AS EndTimes,
                STRING_AGG(sd.location, ', ') AS Locations,
                s.instuctorArabicName AS Instructor
            FROM orderDetails od
            INNER JOIN sections s ON od.sectionId = s.sectionId
            INNER JOIN subjects sub ON s.subjectCode = sub.subjectCode
            INNER JOIN sectionDetails sd ON s.sectionId = sd.sectionId
            WHERE od.orderId = @OrderId
            GROUP BY s.subjectCode, sub.subjectEnglishName, s.sectionNumber, 
                    sub.creditHours, s.instuctorArabicName";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@OrderId", orderId }
            };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters) as DataTable ?? new DataTable();

            dt.Columns.Add("FormattedSchedule", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                string[] days = row["Days"].ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                string[] startTimes = row["StartTimes"].ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                string[] endTimes = row["EndTimes"].ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                string[] locations = row["Locations"].ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                List<string> scheduleEntries = new List<string>();
                for (int i = 0; i < days.Length; i++)
                {
                    scheduleEntries.Add(FormatScheduleDetails(
                        days[i],
                        startTimes.ElementAtOrDefault(i),
                        endTimes.ElementAtOrDefault(i),
                        locations.ElementAtOrDefault(i)
                    ));
                }

                row["FormattedSchedule"] = string.Join("<br/>", scheduleEntries);
            }

            return dt;
        }

        public static string FormatScheduleDetails(string day, string startTime, string endTime, string location)
        {
            if (!int.TryParse(day, out int academicDay))
            {
                academicDay = 0;
            }
            academicDay -= 1;

            if (!DateTime.TryParse(startTime, out DateTime start))
            {
                start = DateTime.MinValue;
            }
            if (!DateTime.TryParse(endTime, out DateTime end))
            {
                end = DateTime.MinValue;
            }

            string formattedStart = start.ToString("hh:mmtt");
            string formattedEnd = end.ToString("hh:mmtt");

            return $"{GetDayName(academicDay)} | {formattedStart}-{formattedEnd} | {location}";
        }

        private static string GetDayName(int academicDay)
        {
            if (academicDay >= 0 && academicDay <= 4)
            {
                return ((DayOfWeek)academicDay).ToString();
            }
            return "TBA";
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            int orderId = Convert.ToInt32(btn.CommandArgument);

            CRUD myCrud = new CRUD();
            string studentSql = @"
                SELECT s.studentEnglishFirstName, s.studentEnglishLastName, s.currentLevel 
                FROM orders o
                INNER JOIN students s ON o.studentId = s.studentId
                WHERE o.orderId = @OrderId";

            Dictionary<string, object> studentParams = new Dictionary<string, object>
                {
                    { "@OrderId", orderId }
                };

            DataTable studentDt = myCrud.getDTPassSqlDic(studentSql, studentParams);

            if (studentDt.Rows.Count > 0)
            {
                DataRow student = studentDt.Rows[0];
                lblModalStudentName.Text = $"Student Name: {student["studentEnglishFirstName"]} {student["studentEnglishLastName"]}";
                lblModalStudentLevel.Text = $"Level: {student["currentLevel"]}";
            }
            else
            {
                lblModalStudentName.Text = "Student Not Found";
                lblModalStudentLevel.Text = "N/A";
            }

            DataTable dtOrderDetails = GetOrderDetails(orderId);
            gvOrderDetails.DataSource = dtOrderDetails;
            gvOrderDetails.DataBind();

            ClientScript.RegisterStartupScript(this.GetType(), "ShowModal", "showModal();", true);
        }
        public string GetOrderStatus(object status)
        {
            if (status == DBNull.Value)
            {
                return "Not Ordered";
            }
            return status.ToString();
        }

        public string GetStatusClass(object status)
        {
            if (status == DBNull.Value)
            {
                return "not-ordered";
            }
            return status.ToString().ToLower();
        }

        protected void gvStudents_PreRender(object sender, EventArgs e)
        {
            if (gvStudents.Rows.Count > 0)
            {
                gvStudents.HeaderRow.TableSection = TableRowSection.TableHeader;

                string currentSort = ViewState["SortExpression"]?.ToString();
                string sortDirection = ViewState["SortDirection"]?.ToString();

                foreach (DataControlFieldHeaderCell header in gvStudents.HeaderRow.Cells
                    .OfType<DataControlFieldHeaderCell>())
                {
                    if (header.ContainingField.SortExpression == currentSort)
                    {
                        header.CssClass += sortDirection == "ASC" ? " sort-asc" : " sort-desc";
                    }
                }
            }
        }

        protected void ddlLevelFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindStudents();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            BindStudents();
        }

        protected void btnViewOrder_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int studentId = Convert.ToInt32(btn.CommandArgument);
        }

        private void LoadDashboard()
        {
            lblTotalStudents.Text = "Total Students: " + GetTotalStudentsCount().ToString("N0");

            lblApprovedOrders.Text = "Approved Orders: " + GetApprovedOrdersCount().ToString("N0");

            lblPendingActions.Text = "Pending Actions: " + GetPendingWaitlistActionsCount().ToString("N0");
        }

        private int GetTotalStudentsCount()
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT COUNT(*) 
                 FROM students
                 WHERE universityId = @UniversityId 
                   AND majorId = @MajorId";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UniversityId", ViewState["UniversityId"] },
                    { "@MajorId", ViewState["MajorId"] }
                };

            return Convert.ToInt32(myCrud.getDTPassSqlDic(sql, parameters).Rows[0][0]);
        }

        private int GetApprovedOrdersCount()
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT COUNT(*) 
                 FROM orders o
                 INNER JOIN students s ON o.studentId = s.studentId
                 WHERE s.universityId = @UniversityId 
                   AND s.majorId = @MajorId
                   AND o.status = 'Approved'";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UniversityId", ViewState["UniversityId"] },
                    { "@MajorId", ViewState["MajorId"] }
                };

            return Convert.ToInt32(myCrud.getDTPassSqlDic(sql, parameters).Rows[0][0]);
        }

        private int GetPendingWaitlistActionsCount()
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT COUNT(DISTINCT CONCAT(w.subjectId, '|', w.requestedSemester))
                 FROM waitlist w
                 INNER JOIN subjects s ON w.subjectId = s.subjectId
                 WHERE s.universityId = @UniversityId
                   AND s.majorId = @MajorId
                   AND w.status = 'Pending'";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UniversityId", ViewState["UniversityId"] },
                    { "@MajorId", ViewState["MajorId"] }
                };

            return Convert.ToInt32(myCrud.getDTPassSqlDic(sql, parameters).Rows[0][0]);
        }
    }
}