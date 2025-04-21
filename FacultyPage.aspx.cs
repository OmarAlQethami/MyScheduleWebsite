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

        private void BindWaitlists()
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT 
                s.subjectCode,
                s.subjectEnglishName,
                COUNT(w.studentId) AS totalStudents,
                STRING_AGG(CONCAT(stu.studentEnglishFirstName, ' ', stu.studentEnglishLastName, 
                           ' - ', stu.studentUniId),  -- Changed here
                           ', ') WITHIN GROUP (ORDER BY w.priority DESC, w.requestDate) AS topStudents,
                w.requestedSemester,
                w.status
           FROM waitlist w
           INNER JOIN subjects s ON w.subjectId = s.subjectId
           INNER JOIN students stu ON w.studentId = stu.studentId
           WHERE s.universityId = @UniversityId
             AND s.majorId = @MajorId
           GROUP BY s.subjectCode, s.subjectEnglishName, w.requestedSemester, w.status
           ORDER BY totalStudents DESC";

            Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "@UniversityId", ViewState["UniversityId"] },
        { "@MajorId", ViewState["MajorId"] }
    };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            gvWaitlists.DataSource = dt;
            gvWaitlists.DataBind();
        }

        protected void gvWaitlists_PreRender(object sender, EventArgs e)
        {
            if (gvWaitlists.Rows.Count > 0)
            {
                gvWaitlists.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
        protected void gvWaitlists_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                string topStudents = rowView["topStudents"].ToString();

                DropDownList ddlStudents = (DropDownList)e.Row.FindControl("ddlStudents");
                if (ddlStudents != null)
                {
                    string[] students = topStudents.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

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
        private void LoadDashboard()
        {

        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string requestId = btn.CommandArgument;
        }
        protected void btnDeny_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string requestId = btn.CommandArgument;
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

        protected void BindStudents()
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT s.studentId, s.studentUniId, s.currentLevel, 
                            s.studentEnglishFirstName, s.studentEnglishLastName,
                            o.orderId, o.orderDate, o.status
                            FROM students s
                            LEFT JOIN orders o ON s.studentId = o.studentId
                            WHERE s.universityId = @UniversityId 
                            AND s.majorId = @MajorId
                            AND (s.studentEnglishFirstName + ' ' + s.studentEnglishLastName LIKE '%' + @Search + '%' 
                            OR s.studentUniId LIKE '%' + @Search + '%')";

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

        [System.Web.Services.WebMethod(EnableSession = true)]
        private DataTable GetOrderDetails(int orderId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT 
        s.subjectCode AS SubjectCode,
        sub.subjectEnglishName AS SubjectName,
        s.sectionNumber AS SectionNumber,
        sub.creditHours AS CreditHours,
        STRING_AGG(CONVERT(nvarchar, sd.day), ', ') + ' | ' +
        STRING_AGG(CONVERT(nvarchar, sd.startTime, 108), ', ') + ' | ' +
        STRING_AGG(CONVERT(nvarchar, sd.endTime, 108), ', ') + ' | ' +
        STRING_AGG(sd.location, ', ') AS Schedule,
        s.instructorName AS Instructor
    FROM orderDetails od
    INNER JOIN sections s ON od.sectionId = s.sectionId
    INNER JOIN subjects sub ON s.subjectCode = sub.subjectCode
    INNER JOIN sectionDetails sd ON s.sectionId = sd.sectionId
    WHERE od.orderId = @OrderId
    GROUP BY s.subjectCode, sub.subjectEnglishName, s.sectionNumber, 
            sub.creditHours, s.instructorName";

            Dictionary<string, object> parameters = new Dictionary<string, object> {
        { "@OrderId", orderId }
    };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters) as DataTable;
            return dt ?? new DataTable();
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
            GridViewRow row = (GridViewRow)btn.NamingContainer;

            string studentName = $"{DataBinder.Eval(row.DataItem, "studentEnglishFirstName")} {DataBinder.Eval(row.DataItem, "studentEnglishLastName")}";

            lblModalStudentName.Text = studentName;

            int orderId = Convert.ToInt32(btn.CommandArgument);
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

        protected void btnExport_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int orderId = Convert.ToInt32(btn.CommandArgument);
        }
    }
}