﻿using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite
{
    public partial class DepartmentHeadPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                GetFacultyUniversityAndMajor();
                BindFacultyInfo();
                BindMajorPlan();
                BindCurriculum();
                BindWaitlists();
            }
            BindStudents();
            LoadDashboard();
        }

        private int curriculumId = 1;
        private void BindMajorPlan()
        {
            if (ViewState["UniversityId"] == null || ViewState["MajorId"] == null) return;

            CRUD myCrud = new CRUD();
            string sql = @"SELECT s.subjectLevel, s.subjectCode, s.subjectEnglishName, 
                 s.creditHours, st.subjectTypeEnglishName, s.prerequisites,
                 s.subjectTypeId 
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

        protected void gvMajorPlan_RowEditing(object sender, GridViewEditEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("RowEditing event fired");
            gvMajorPlan.EditIndex = e.NewEditIndex;
            BindMajorPlan();
        }

        protected void gvMajorPlan_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Update started");

            GridViewRow row = gvMajorPlan.Rows[e.RowIndex];

            
            string subjectCode = gvMajorPlan.DataKeys[e.RowIndex].Value.ToString();
            System.Diagnostics.Debug.WriteLine($"Updating record with code: {subjectCode}");

            TextBox txtCode = (TextBox)row.FindControl("txtSubjectCode");
            TextBox txtLevel = (TextBox)row.FindControl("txtSubjectLevel");
            TextBox txtName = (TextBox)row.FindControl("txtSubjectEnglishName");
            TextBox txtHours = (TextBox)row.FindControl("txtCreditHours");
            TextBox txtPre = (TextBox)row.FindControl("txtPrerequisites");
            DropDownList ddlType = (DropDownList)row.FindControl("ddlSubjectType");

            
            

            string selectedTypeId = ddlType.SelectedValue;
            System.Diagnostics.Debug.WriteLine($"Selected Subject Type ID: {selectedTypeId}");
            try
            {
                CRUD myCrud = new CRUD();
                string sql = @"UPDATE subjects SET 
                      subjectEnglishName = @name,
                      subjectCode = @newCode,
                      subjectLevel = @level,
                      creditHours = @hours,
                      prerequisites = @pre,
                      subjectTypeId = (SELECT subjectTypeId FROM subjectType WHERE subjectTypeEnglishName = @type)
                      WHERE subjectCode = @code
                      AND universityId = @univId
                      AND majorId = @majorId";

                var parameters = new Dictionary<string, object>
        {
            { "@name", txtName.Text },
            { "@newCode", txtCode.Text.Trim() },
            { "@level", txtLevel.Text.Trim() },
            { "@hours", txtHours.Text },
            { "@pre", txtPre?.Text ?? "" },
            { "@type", ddlType.SelectedValue },
            { "@code", subjectCode },
            { "@univId", ViewState["UniversityId"] },
            { "@majorId", ViewState["MajorId"] }
        };

                int rowsAffected = myCrud.InsertUpdateDelete(sql, parameters);
                System.Diagnostics.Debug.WriteLine($"Rows affected: {rowsAffected}");

                if (rowsAffected > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert",
                        "alert('Updated Successfully');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert",
                        "alert('Record Not Updated, Check Data');", true);
                }

                gvMajorPlan.EditIndex = -1;
                BindMajorPlan();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.ToString()}");
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Error: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        protected void gvMajorPlan_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvMajorPlan.EditIndex = -1;
            BindMajorPlan();
        }

        protected void gvMajorPlan_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string subjectCode = gvMajorPlan.DataKeys[e.RowIndex].Value.ToString();

            CRUD myCrud = new CRUD();
            string sql = @"DELETE FROM subjects 
                  WHERE subjectCode = @subjectCode
                  AND universityId = @UniversityId
                  AND majorId = @MajorId";

            Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "@subjectCode", subjectCode },
        { "@UniversityId", ViewState["UniversityId"] },
        { "@MajorId", ViewState["MajorId"] }
    };

            myCrud.InsertUpdateDelete(sql, parameters);
            BindMajorPlan();
        }

        protected void gvMajorPlan_PreRender(object sender, EventArgs e)
        {
            if (gvMajorPlan.Rows.Count > 0)
            {
                gvMajorPlan.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected string GetSafeSubjectType(object subjectType)
        {
            if (subjectType == null) return "Core"; 

            string type = subjectType.ToString().Trim();

            
            if (type.Equals("Core", StringComparison.OrdinalIgnoreCase) ||
                type.Equals("Elective", StringComparison.OrdinalIgnoreCase))
            {
                return type;
            }

            return "Core"; 
        }

        protected void gvMajorPlan_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                {
                    DropDownList ddlType = (DropDownList)e.Row.FindControl("ddlSubjectType");
                    if (ddlType != null)
                    {
                       
                        string sql = "SELECT subjectTypeId, subjectTypeEnglishName FROM subjectType";
                        DataTable dt = new CRUD().getDT(sql);

                        ddlType.DataSource = dt;
                        ddlType.DataBind();

                        
                        DataRowView drv = (DataRowView)e.Row.DataItem;
                        string currentTypeId = drv["subjectTypeId"].ToString();
                        ddlType.SelectedValue = currentTypeId;
                    }
                }
            }
        }


        protected void btnAddSubject_Click(object sender, EventArgs e)
        {
            try
            {
                
                Response.Redirect("AddSubject.aspx?univId=" + ViewState["UniversityId"] + "&majorId=" + ViewState["MajorId"]);

                
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Error: {ex.Message.Replace("'", "\\'")}');", true);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {

                string subjectCode = txtSubjectCode.Text.Trim();
                string subjectLevel = ddlSubjectLevel.SelectedValue;
                string englishName = txtSubjectEnglishName.Text.Trim();
                string arabicName = txtSubjectArabicName.Text.Trim();
                string creditHours = txtCreditHours.Text.Trim();
                string subjectTypeId = ddlSubjectType.SelectedValue;
                string prerequisites = txtPrerequisites.Text.Trim();


                if (string.IsNullOrEmpty(subjectCode) || string.IsNullOrEmpty(englishName) ||
                    string.IsNullOrEmpty(creditHours) || string.IsNullOrEmpty(subjectTypeId))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                       "alert('Please fill in all required fields.');", true);
                    return;
                }


                if (!int.TryParse(creditHours, out int hours) || hours < 1 || hours > 10)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('Credit hours must be a number between 1 and 10.');", true);
                    return;
                }


                CRUD myCrud = new CRUD();
                string sql = @"INSERT INTO subjects 
                      (subjectCode, subjectLevel, subjectEnglishName, subjectArabicName, 
                       creditHours, subjectTypeId, prerequisites, universityId, majorId)
                      VALUES 
                      (@code, @level, @engName, @arName, 
                       @hours, @typeId, @prerequisites, @univId, @majorId)";

                Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "@code", subjectCode },
            { "@level", subjectLevel },
            { "@engName", englishName },
            { "@arName", arabicName },
            { "@hours", creditHours },
            { "@typeId", subjectTypeId },
            { "@prerequisites", string.IsNullOrEmpty(prerequisites) ? DBNull.Value : (object)prerequisites },
            { "@univId", ViewState["UniversityId"] ?? DBNull.Value },
            { "@majorId", ViewState["MajorId"] ?? DBNull.Value }
        };

                int rowsAffected = myCrud.InsertUpdateDelete(sql, parameters);


                if (rowsAffected > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "success",
                        "alert('The subject has been saved successfully');", true);

                    BindMajorPlan();
                    return;
                }

                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        "alert('Data not saved, please try again');", true);
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Error saving subject: {ex}");


                string errorMsg = ex.Message.Contains("UNIQUE KEY constraint")
                    ? "The subject code is already registered, please use another code"
                    : "An error occurred while trying to save data.";

                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('{errorMsg}');", true);
            }
            BindMajorPlan();
        }

        protected void gvCurriculum_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCurriculum.EditIndex = e.NewEditIndex;
            BindCurriculum(); 
        }

        protected void gvCurriculum_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCurriculum.EditIndex = -1;
            BindCurriculum();
        }

        protected void gvCurriculum_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            
            int sectionNumber = Convert.ToInt32(gvCurriculum.DataKeys[e.RowIndex].Value);

            CRUD myCrud = new CRUD();

           
            string sql = @"DELETE FROM Sections 
                   WHERE sectionNumber = @sectionNumber 
                   AND universityId = @UniversityId 
                   AND majorId = @MajorId";

            
            var parameters = new Dictionary<string, object>
    {
        { "@sectionNumber", sectionNumber },
        { "@UniversityId", ViewState["UniversityId"] },
        { "@MajorId", ViewState["MajorId"] }
    };

           
            myCrud.InsertUpdateDelete(sql, parameters);

            BindCurriculum();
        }

        protected void gvCurriculum_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Update started");

            GridViewRow row = gvCurriculum.Rows[e.RowIndex];

            int sectionNumber = Convert.ToInt32(gvCurriculum.DataKeys[e.RowIndex].Value);


            string subjectCode = ((TextBox)row.Cells[1].Controls[0]).Text;
            string subjectName = ((TextBox)row.Cells[2].Controls[0]).Text;
            int creditHours = int.Parse(((TextBox)row.Cells[3].Controls[0]).Text);
            int capacity = int.Parse(((TextBox)row.Cells[4].Controls[0]).Text);

            string day = ((TextBox)row.FindControl("txtDay")).Text;
            string startTime = ((TextBox)row.FindControl("txtStartTime")).Text;
            string endTime = ((TextBox)row.FindControl("txtEndTime")).Text;
            string location = ((TextBox)row.FindControl("txtLocation")).Text;
            string instructor = ((TextBox)row.Cells[7].Controls[0]).Text;

            CRUD myCrud = new CRUD();
            string sql = @"UPDATE Sections SET 
                    subjectCode = @subjectCode,
                    subjectEnglishName = @subjectName,
                    creditHours = @creditHours,
                    capacity = @capacity,
                    day = @day, 
                    startTime = @startTime, 
                    endTime = @endTime, 
                    location = @location,
                    instuctorArabicName = @instructor
                    WHERE sectionNumber = @sectionNumber";

            var parameters = new Dictionary<string, object>
    {
        { "@subjectCode", subjectCode },
        { "@subjectName", subjectName },
        { "@creditHours", creditHours },
        { "@capacity", capacity },
        { "@day", day },
        { "@startTime", startTime },
        { "@endTime", endTime },
        { "@location", location },
        { "@instructor", instructor },
        { "@sectionNumber", sectionNumber }
    };

            try
            {
                int rowsAffected = myCrud.InsertUpdateDelete(sql, parameters);

                if (rowsAffected > 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert",
                        "alert('Updated Successfully');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert",
                        "alert('No changes made, check data');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.ToString()}");
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Error: {ex.Message.Replace("'", "\\'")}');", true);
            }

            gvCurriculum.EditIndex = -1;
            BindCurriculum();
        }

        protected void btnSaveSection_Click(object sender, EventArgs e)
        {
            string subjectCode = txtPopupSubjectCode.Text.Trim();
            string registeredStudents = txtPopupregisteredStudents.Text.Trim();
            int capacity = int.Parse(txtPopupCapacity.Text.Trim());
            int day = ConvertDayToNumber(txtPopupDay.Text.Trim());
            string startTime = txtPopupStartTime.Text.Trim();
            string endTime = txtPopupEndTime.Text.Trim();
            string location = txtPopupLocation.Text.Trim();
            string instructor = txtPopupInstructor.Text.Trim();

            
            string sectionSql = @"
    INSERT INTO Sections (subjectCode, capacity, registeredStudents, instructorArabicName)
    OUTPUT INSERTED.sectionId
    VALUES (@subjectCode, @capacity, @registeredStudents, @instructor)";

        
            string sectionDetailsSql = @"
    INSERT INTO SectionDetails (sectionId, day, startTime, endTime, location)
    VALUES (@sectionId, @day, @startTime, @endTime, @location)";

            CRUD myCrud = new CRUD();

            var sectionParams = new Dictionary<string, object>
    {
        { "@subjectCode", subjectCode },
        { "@capacity", capacity },
        { "@registeredStudents", registeredStudents },
        { "@instructor", instructor }
    };

            try
            {
                
                int sectionId = myCrud.InsertAndReturnId(sectionSql, sectionParams);

                if (sectionId > 0)
                {
                 
                    var sectionDetailsParams = new Dictionary<string, object>
            {
                { "@sectionId", sectionId },
                { "@day", day },
                { "@startTime", startTime },
                { "@endTime", endTime },
                { "@location", location }
            };

                   
                    int result = myCrud.InsertUpdateDelete(sectionDetailsSql, sectionDetailsParams);

                    if (result > 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Section Added Successfully');", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to add section details');", true);
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to add section');", true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error: " + ex.Message + "');", true);
            }

            BindCurriculum();
        }



        private int ConvertDayToNumber(string day)
        {
            switch (day.Trim())
            {
               
                case "sunday":
                    return 1;
                
                case "monday":
                    return 2;
               
                case "tuesday":
                    return 3;
               
                case "wednesday":
                    return 4;
                
                case "thursday":
                    return 5;
                default:
                    return 0; 
            }
        }


        private void BindFacultyInfo()
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT 
                    f.facultyEnglishFirstName,
                    f.facultyEnglishLastName,
                    f.email,
                    u.universityEnglishName,
                    m.majorEnglishName
                   FROM faculty f
                   INNER JOIN universities u ON f.universityId = u.universityId
                   INNER JOIN majors m ON f.majorId = m.majorId";

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            gvFacultyInfo.DataSource = dt;
            gvFacultyInfo.DataBind();
        }

        protected string GetFullName(object firstName, object lastName)
        {
            string first = firstName?.ToString() ?? "";
            string last = lastName?.ToString() ?? "";
            return $"{first} {last}".Trim();
        }

        protected void gvFacultyInfo_PreRender(object sender, EventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (gridView.Rows.Count > 0)
            {
                gridView.HeaderRow.TableSection = TableRowSection.TableHeader;
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
            string facultyFullName = ViewState["FacultyName"]?.ToString() ?? "Faculty";

            string updateSql = @"UPDATE w 
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
                    { "@ChangedBy", facultyFullName },
                    { "@SubjectId", subjectId },
                    { "@Semester", semester }
                };

            int rowsUpdated = myCrud.InsertUpdateDelete(updateSql, parameters);

            if (rowsUpdated > 0)
            {
                string subjectName = GetSubjectName(subjectId);

                string studentSql = @"SELECT s.email, s.studentEnglishFirstName, s.studentEnglishLastName
                            FROM waitlist w
                            INNER JOIN students s ON w.studentId = s.studentId
                            WHERE w.subjectId = @SubjectId
                              AND w.requestedSemester = @Semester";

                DataTable students = myCrud.getDTPassSqlDic(studentSql, parameters);

                foreach (DataRow student in students.Rows)
                {
                    string email = student["email"].ToString();
                    string fullName = $"{student["studentEnglishFirstName"]} {student["studentEnglishLastName"]}";

                    if (!string.IsNullOrEmpty(email))
                    {
                        SendStatusEmail(email, fullName, subjectName, facultyFullName, newStatus);
                    }
                }
                BindWaitlists();
            }
        }

        private string GetSubjectName(int subjectId)
        {
            CRUD myCrud = new CRUD();
            DataTable dt = myCrud.getDTPassSqlDic(
                "SELECT subjectEnglishName FROM subjects WHERE subjectId = @id",
                new Dictionary<string, object> { { "@id", subjectId } }
            );
            return dt.Rows[0]["subjectEnglishName"].ToString();
        }

        private void SendStatusEmail(string studentEmail, string studentName, string subjectName, string facultyName, string status)
        {
            try
            {
                mailMgr email = new mailMgr
                {
                    myTo = studentEmail,
                    mySubject = $"Update on your request for {subjectName}",
                    myBody = $@"
Dear {studentName},

We're writing to inform you about the status of your request for {subjectName}.

Your request has been {status} by {facultyName}.


------------------
This is an automated notification. Please do not reply directly to this email.",
                    myIsBodyHtml = false
                };

                email.sendEmailViaGmail();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Email failed for {studentEmail}: {ex.Message}");
            }
        }
        private void GetFacultyUniversityAndMajor()
        {
            Guid userId = GetUserId();
            CRUD myCrud = new CRUD();

            string sql = @"SELECT d.departmentHeadEnglishFirstName, d.departmentHeadEnglishLastName, 
                          d.universityId, d.majorId, 
                          m.majorEnglishName, u.universityEnglishName 
                   FROM departmentHead d
                   INNER JOIN majors m ON d.majorId = m.majorId
                   INNER JOIN universities u ON d.universityId = u.universityId
                   WHERE d.UserId = @UserId";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                string firstName = dt.Rows[0]["departmentHeadEnglishFirstName"].ToString();
                string lastName = dt.Rows[0]["departmentHeadEnglishLastName"].ToString();
                string fullName = $"{firstName} {lastName}".Trim();

                lblFacultyName.Text = $"Hello, {dt.Rows[0]["departmentHeadEnglishFirstName"]}";
                lblMajor.Text = $"Major: {dt.Rows[0]["majorEnglishName"]}";
                lblUniversity.Text = $"University: {dt.Rows[0]["universityEnglishName"]}";

                ViewState["UniversityId"] = Convert.ToInt32(dt.Rows[0]["universityId"]);
                ViewState["MajorId"] = Convert.ToInt32(dt.Rows[0]["majorId"]);
                ViewState["DepartmentHeadName"] = fullName;
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

        private Dictionary<string, object> GetUniversityParams()
        {
            return new Dictionary<string, object>
            {
                { "@UniversityId", ViewState["UniversityId"] },
                { "@MajorId", ViewState["MajorId"] }
            };
        }

        public class DashboardData
        {
            public int TotalStudents { get; set; }
            public int StudentsWithOrders { get; set; }
            public int StudentsWithoutOrders { get; set; }
            public Dictionary<string, int> OrderStatuses { get; set; } = new Dictionary<string, int>
            {
                ["Approved"] = 0,
                ["Pending"] = 0,
                ["Denied"] = 0,
                ["Not Ordered"] = 0
            };

            public Dictionary<string, int> WaitlistStatuses { get; set; } = new Dictionary<string, int>
            {
                ["Approved"] = 0,
                ["Pending"] = 0,
                ["Denied"] = 0
            };
            public DataTable TopWaitlisted { get; set; }
            public DataTable PopularSubjects { get; set; }
        }

        private DashboardData GetDashboardData()
        {
            var data = new DashboardData();

            var orderStatuses = GetOrderStatusDistribution();
            foreach (var kvp in orderStatuses)
            {
                data.OrderStatuses[kvp.Key] = kvp.Value;
            }

            var waitlistStatuses = GetWaitlistStatusDistribution();
            foreach (var kvp in waitlistStatuses)
            {
                data.WaitlistStatuses[kvp.Key] = kvp.Value;
            }

            data.TotalStudents = GetTotalStudentsCount();
            data.StudentsWithOrders = GetStudentsWithOrdersCount();
            data.StudentsWithoutOrders = GetStudentsWithoutOrdersCount();
            data.TopWaitlisted = GetTopWaitlistedSubjects();
            data.PopularSubjects = GetPopularSubjects();

            return data;
        }

        private Dictionary<string, int> GetOrderStatusDistribution()
        {
            var statuses = new Dictionary<string, int>
            {
                ["Approved"] = 0,
                ["Pending"] = 0,
                ["Denied"] = 0,
                ["Not Ordered"] = 0
            };

            CRUD myCrud = new CRUD();
            string sql = @"SELECT ISNULL(o.status, 'Not Ordered') AS Status, 
                          COUNT(DISTINCT s.studentId) AS Count
                   FROM students s
                   LEFT JOIN orders o ON s.studentId = o.studentId
                   WHERE s.universityId = @UniversityId 
                     AND s.majorId = @MajorId
                   GROUP BY o.status";

            DataTable dt = myCrud.getDTPassSqlDic(sql, GetUniversityParams());

            foreach (DataRow row in dt.Rows)
            {
                string status = row["Status"].ToString();
                int count = Convert.ToInt32(row["Count"]);

                if (statuses.ContainsKey(status))
                {
                    statuses[status] = count;
                }
            }

            return statuses;
        }

        private DataTable GetTopWaitlistedSubjects()
        {
            string sql = @"SELECT TOP 5 s.subjectCode, s.subjectEnglishName AS SubjectName, 
                          COUNT(*) AS WaitCount
                   FROM waitlist w
                   INNER JOIN subjects s ON w.subjectId = s.subjectId
                   WHERE s.universityId = @UniversityId AND s.majorId = @MajorId
                   GROUP BY s.subjectCode, s.subjectEnglishName
                   ORDER BY WaitCount DESC";

            return new CRUD().getDTPassSqlDic(sql, GetUniversityParams());
        }

        private int GetStudentsWithOrdersCount()
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT COUNT(DISTINCT s.studentId)
                   FROM students s
                   INNER JOIN orders o ON s.studentId = o.studentId
                   WHERE s.universityId = @UniversityId 
                     AND s.majorId = @MajorId";

            return Convert.ToInt32(myCrud.getDTPassSqlDic(sql, GetUniversityParams()).Rows[0][0]);
        }

        private int GetStudentsWithoutOrdersCount()
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT COUNT(*)
                   FROM students
                   WHERE universityId = @UniversityId 
                     AND majorId = @MajorId
                     AND studentId NOT IN (SELECT studentId FROM orders)";

            return Convert.ToInt32(myCrud.getDTPassSqlDic(sql, GetUniversityParams()).Rows[0][0]);
        }

        private Dictionary<string, int> GetWaitlistStatusDistribution()
        {
            var statuses = new Dictionary<string, int>
            {

                ["Approved"] = 0,
                ["Pending"] = 0,
                ["Denied"] = 0
            };

            CRUD myCrud = new CRUD();
            string sql = @"SELECT w.status, COUNT(*) AS Count
                   FROM waitlist w
                   INNER JOIN subjects s ON w.subjectId = s.subjectId
                   WHERE s.universityId = @UniversityId 
                     AND s.majorId = @MajorId
                   GROUP BY w.status";

            DataTable dt = myCrud.getDTPassSqlDic(sql, GetUniversityParams());

            foreach (DataRow row in dt.Rows)
            {
                string status = row["status"].ToString();
                int count = Convert.ToInt32(row["Count"]);

                if (statuses.ContainsKey(status))
                {
                    statuses[status] = count;
                }
            }

            return statuses;
        }

        private DataTable GetPopularSubjects()
        {
            string sql = @"SELECT TOP 5 sub.subjectCode, 
                          sub.subjectEnglishName AS SubjectName,
                          COUNT(*) AS RequestCount
                   FROM orderDetails od
                   INNER JOIN orders o ON od.orderId = o.orderId
                   INNER JOIN sections sec ON od.sectionId = sec.sectionId
                   INNER JOIN subjects sub ON sec.subjectCode = sub.subjectCode
                   WHERE o.status = 'Approved'
                     AND sub.universityId = @UniversityId
                     AND sub.majorId = @MajorId
                   GROUP BY sub.subjectCode, sub.subjectEnglishName
                   ORDER BY RequestCount DESC";

            return new CRUD().getDTPassSqlDic(sql, GetUniversityParams());
        }

        public Dictionary<string, int> OrderStatuses { get; set; }
        public Dictionary<string, int> WaitlistStatuses { get; set; }

        protected void LoadDashboard()
        {
            var dashboardData = GetDashboardData();
            EnsureKeyExists(dashboardData.OrderStatuses, "Approved", "Pending", "Not Ordered");
            EnsureKeyExists(dashboardData.WaitlistStatuses, "Pending", "Approved", "Denied");

            string orderData = $"{dashboardData.OrderStatuses["Approved"]}, {dashboardData.OrderStatuses["Pending"]}, {dashboardData.OrderStatuses["Not Ordered"]}";
            ClientScript.RegisterArrayDeclaration("orderStatusData", orderData);

            string waitlistData = $"{dashboardData.WaitlistStatuses["Approved"]}, {dashboardData.WaitlistStatuses["Pending"]}, {dashboardData.WaitlistStatuses["Denied"]}";
            ClientScript.RegisterArrayDeclaration("waitlistStatusData", waitlistData);

            OrderStatuses = dashboardData.OrderStatuses ?? new Dictionary<string, int>();
            WaitlistStatuses = dashboardData.WaitlistStatuses ?? new Dictionary<string, int>();


            lblTotalStudents.Text = "Total Students: " + GetTotalStudentsCount().ToString("N0");
            lblApprovedOrders.Text = $"Approved Orders: {dashboardData.OrderStatuses["Approved"]:N0}";
            lblPendingActions.Text = $"Pending Actions: {dashboardData.WaitlistStatuses["Pending"]:N0}";

            gvTopWaitlists.DataSource = dashboardData.TopWaitlisted;
            gvTopWaitlists.DataBind();

            gvPopularSubjects.DataSource = dashboardData.PopularSubjects;
            gvPopularSubjects.DataBind();
        }

        private void EnsureKeyExists(Dictionary<string, int> dict, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (!dict.ContainsKey(key))
                {
                    dict[key] = 0;
                }
            }
        }

        protected int GetOrderStatusValue(string key)
        {
            if (OrderStatuses != null && OrderStatuses.TryGetValue(key, out int value))
                return value;
            return 0;
        }

        protected int GetWaitlistStatusValue(string key)
        {
            if (WaitlistStatuses != null && WaitlistStatuses.TryGetValue(key, out int value))
                return value;
            return 0;
        }
    }
}