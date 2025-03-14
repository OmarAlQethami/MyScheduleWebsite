using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite
{
    public partial class StudentPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Default.aspx");
            }

            Guid userId = GetUserId();
            int universityId = GetUniversityId(userId);
            int majorId = GetMajorId(userId);
            int studentId = GetStudentId(userId);

            List<string> takenSubjects = GetTakenSubjectCodes(studentId, universityId, majorId);
            if (takenSubjects.Count == 0)
            {
                Response.Redirect("~/StudentProgressPage.aspx");
                return;
            }
            else if (!IsPostBack)
            {
                lblGreeting.Text = "Hello " + User.Identity.Name + "!";
                lblCurrentLevel.Text = "Current Level: " + GetCurrentLevel(userId);

                BindSubjects(universityId, majorId);
            }
        }

        private class Subject
        {
            public string Code { get; set; }
            public int Level { get; set; }
            public string EnglishName { get; set; }
            public string ArabicName { get; set; }
            public decimal CreditHours { get; set; }
            public int TypeId { get; set; }
            public List<string> Prerequisites { get; set; } = new List<string>();
            public bool IsCompulsory => TypeId == 1 || TypeId == 2;
            public bool IsElectiveCollege => TypeId == 3;
            public bool IsElectiveUniversity => TypeId == 4;
        }

        private class BindSubjectsData
        {
            public List<Subject> Subjects { get; set; } = new List<Subject>();
            public decimal TotalCompulsoryHours { get; set; }
            public decimal TotalElectiveUniversityHours { get; set; }
            public decimal TotalElectiveCollegeHours { get; set; }
        }

        private class SubjectsRenderResult
        {
            public string PrerequisitesJson { get; set; }
            public string SubjectNameMapJson { get; set; }
            public string SubjectLevelMapJson { get; set; }
            public string SubjectCreditHoursMapJson { get; set; }
            public string SubjectTypeMapJson { get; set; }
        }

        private Guid GetUserId()
        {
            if (Membership.GetUser() is MembershipUser user)
            {
                return (Guid)user.ProviderUserKey;
            }
            return Guid.Empty;
        }

        private int GetCurrentLevel(Guid userId)
        {
            int currentLevel = 0;

            CRUD myCrud = new CRUD();
            string mySql = @"SELECT currentLevel FROM students WHERE UserId = @UserId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@UserId", userId);

            SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara);
            if (dr.Read())
            {
                currentLevel = dr.GetInt32(0);
            }
            dr.Close();
            return currentLevel;
        }

        private int GetUniversityId(Guid userId)
        {
            int universityId = 0;

            CRUD myCrud = new CRUD();
            string mySql = @"SELECT universityId FROM students WHERE UserId = @UserId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@UserId", userId);

            SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara);
            if (dr.Read())
            {
                universityId = dr.GetInt32(0);
            }
            dr.Close();
            return universityId;
        }

        private int GetMajorId(Guid userId)
        {
            int majorId = 0;

            CRUD myCrud = new CRUD();
            string mySql = @"SELECT majorId FROM students WHERE UserId = @UserId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@UserId", userId);

            SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara);
            if (dr.Read())
            {
                majorId = dr.GetInt32(0);
            }
            dr.Close();
            return majorId;
        }

        private int GetStudentId(Guid userId)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"SELECT studentId FROM students WHERE UserId = @UserId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@UserId", userId);
            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["studentId"]) : 0;
        }
        
        private List<string> GetTakenSubjectCodes(int studentId, int universityId, int majorId)
        {
            List<string> takenSubjects = new List<string>();
            CRUD myCrud = new CRUD();
            string mySql = @"
                SELECT s.subjectCode 
                FROM studentsProgress sp 
                INNER JOIN subjects s ON sp.subjectId = s.subjectId 
                WHERE sp.studentId = @studentId 
                  AND s.universityId = @universityId 
                  AND s.majorId = @majorId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@studentId", studentId);
            myPara.Add("@universityId", universityId);
            myPara.Add("@majorId", majorId);
            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            foreach (DataRow row in dt.Rows)
            {
                takenSubjects.Add(row["subjectCode"].ToString());
            }
            return takenSubjects;
        }

        protected DataTable GetSubjects(int universityId, int majorId)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"SELECT * FROM subjects WHERE universityId = @universityId AND majorId = @majorId";

            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@universityId", universityId);
            myPara.Add("@majorId", majorId);

            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            return dt;
        }

        protected List<string> selectedSubjects = new List<string>();

        private void BindSubjects(int universityId, int majorId)
        {
            Guid userId = GetUserId();
            int studentId = GetStudentId(userId);
            int currentLevel = GetCurrentLevel(userId);

            var subjects = ProcessSubjects(universityId, majorId);
            var takenSubjects = GetTakenSubjectCodes(studentId, universityId, majorId);

            var data = ProcessSubjectsData(subjects);
            var renderResult = RenderSubjectsHtmlAndJson(data, takenSubjects, currentLevel);

            RegisterClientScripts(data, renderResult, currentLevel);
            UpdateProgressLabels(subjects);
        }

        private List<Subject> ProcessSubjects(int universityId, int majorId)
        {
            DataTable subjectsTable = GetSubjects(universityId, majorId);
            List<Subject> subjects = new List<Subject>();

            foreach (DataRow row in subjectsTable.Rows)
            {
                subjects.Add(new Subject
                {
                    Code = row["subjectCode"].ToString(),
                    Level = Convert.ToInt32(row["subjectLevel"]),
                    EnglishName = row["subjectEnglishName"].ToString(),
                    ArabicName = row["subjectArabicName"].ToString(),
                    CreditHours = Convert.ToDecimal(row["creditHours"]),
                    TypeId = Convert.ToInt32(row["subjectTypeId"]),
                    Prerequisites = GetPrerequisites(row["prerequisites"].ToString())
                });
            }
            return subjects;
        }

        private BindSubjectsData ProcessSubjectsData(List<Subject> subjects)
        {
            var data = new BindSubjectsData();

            foreach (var subject in subjects)
            {
                if (subject.IsCompulsory)
                    data.TotalCompulsoryHours += subject.CreditHours;
                else if (subject.IsElectiveUniversity)
                    data.TotalElectiveUniversityHours += subject.CreditHours;
                else if (subject.IsElectiveCollege)
                    data.TotalElectiveCollegeHours += subject.CreditHours;

                data.Subjects.Add(subject);
            }
            return data;
        }

        private SubjectsRenderResult RenderSubjectsHtmlAndJson(BindSubjectsData data, List<string> takenSubjectCodes, int currentLevel)
        {
            subjectsContainer.InnerHtml = string.Empty;
            int electivePlaceholderCounter = 1;

            string prerequisitesJson = "var subjectPrerequisites = {";
            string subjectNameMapJson = "var subjectNameMap = {";
            string subjectLevelMapJson = "var subjectLevelMap = {";
            string subjectCreditHoursMapJson = "var subjectCreditHoursMap = {";
            string subjectTypeMapJson = "var subjectTypeMap = {";

            var groupedSubjects = data.Subjects.GroupBy(s => s.Level).OrderBy(g => g.Key);

            foreach (var group in groupedSubjects)
            {
                subjectsContainer.InnerHtml += "<div class='subjects-row'>";

                foreach (var subject in group)
                {
                    AddSubjectToJsonMaps(subject,
                        ref prerequisitesJson,
                        ref subjectNameMapJson,
                        ref subjectLevelMapJson,
                        ref subjectCreditHoursMapJson,
                        ref subjectTypeMapJson);
                }

                var compulsorySubjects = group.Where(s => s.IsCompulsory);
                foreach (var subject in compulsorySubjects)
                {
                    RenderSubjectHtml(subject, takenSubjectCodes);
                }

                var electiveSubjects = group.Where(s => s.IsElectiveCollege || s.IsElectiveUniversity).ToList();

                if (electiveSubjects.Any())
                {
                    string slotText = $"Elective ({electivePlaceholderCounter})";
                    var takenElective = electiveSubjects.FirstOrDefault(s => takenSubjectCodes.Contains(s.Code));
                    if (takenElective != null)
                    {
                        subjectsContainer.InnerHtml += $@"
                            <div class='subject taken elective' id='{takenElective.Code}'>
                                <span>{slotText} - {takenElective.EnglishName}</span>
                            </div>";
                    }
                    else
                    {
                        string placeholderId = $"elective_placeholder_{electivePlaceholderCounter}";
                        bool isSlotUnavailable = group.Key > currentLevel;
                        string slotClass = isSlotUnavailable ? "subject elective-slot unavailable-slot" : "subject elective-slot";

                        subjectsContainer.InnerHtml +=
                            $"<div class='{slotClass}' id='{placeholderId}' data-level='{group.Key}' data-current-level='{currentLevel}' " +
                            $"onclick='showElectivePopup({group.Key}, \"{placeholderId}\", {currentLevel})'>" +
                            "<span>Elective (" + electivePlaceholderCounter + ")</span></div>";

                    }
                    electivePlaceholderCounter++;

                    // Note to self, this condition down here is wrong, it shouldn't be hardcoded, but we'll keep it for now until it is implemented in the DB.
                    if (electivePlaceholderCounter == 6)
                    {
                        string placeholderId = $"elective_placeholder_{electivePlaceholderCounter}";
                        bool isSlotUnavailable = group.Key > currentLevel;
                        string slotClass = isSlotUnavailable ? "subject elective-slot unavailable-slot" : "subject elective-slot";

                        subjectsContainer.InnerHtml +=
                            $"<div class='{slotClass}' id='{placeholderId}' data-level='{group.Key}' " +
                                $"onclick='showElectivePopup({group.Key}, \"{placeholderId}\", {currentLevel})'>" +
                                "<span>Elective (" + electivePlaceholderCounter + ")</span></div>";
                    }
                }
                subjectsContainer.InnerHtml += "</div>";
            }

            prerequisitesJson = prerequisitesJson.TrimEnd(',') + "};";
            subjectNameMapJson = subjectNameMapJson.TrimEnd(',') + "};";
            subjectLevelMapJson = subjectLevelMapJson.TrimEnd(',') + "};";
            subjectCreditHoursMapJson = subjectCreditHoursMapJson.TrimEnd(',') + "};";
            subjectTypeMapJson = subjectTypeMapJson.TrimEnd(',') + "};";

            return new SubjectsRenderResult
            {
                PrerequisitesJson = prerequisitesJson,
                SubjectNameMapJson = subjectNameMapJson,
                SubjectLevelMapJson = subjectLevelMapJson,
                SubjectCreditHoursMapJson = subjectCreditHoursMapJson,
                SubjectTypeMapJson = subjectTypeMapJson
            };
        }

        private void AddSubjectToJsonMaps(Subject subject,
            ref string prerequisitesJson,
            ref string subjectNameMapJson,
            ref string subjectLevelMapJson,
            ref string subjectCreditHoursMapJson,
            ref string subjectTypeMapJson)
        {
            prerequisitesJson += $"'{subject.Code}': [{string.Join(",", subject.Prerequisites.Select(p => $"'{p}'"))}],";
            subjectNameMapJson += $"'{subject.Code}': '{subject.EnglishName.Replace("'", "\\'")}',";
            subjectLevelMapJson += $"'{subject.Code}': {subject.Level},";
            subjectCreditHoursMapJson += $"'{subject.Code}': {subject.CreditHours},";
            subjectTypeMapJson += $"'{subject.Code}': {subject.TypeId},";
        }

        private void RenderSubjectHtml(Subject subject, List<string> takenSubjectCodes)
        {
            string statusClass = GetSubjectStatusClass(subject, takenSubjectCodes);
            bool isClickable = statusClass == "available";
            string onClickHandler = isClickable ? "onclick='SubjectClicked(this)'" : "";

            subjectsContainer.InnerHtml += $@"
                <div class='subject {statusClass}' id='{subject.Code}' {onClickHandler}>
                    <span>{subject.EnglishName}</span>
                </div>";
        }

        private List<string> GetPrerequisites(string prerequisites)
        {
            return string.IsNullOrWhiteSpace(prerequisites)
                ? new List<string>()
                : prerequisites.Split(',').Select(p => p.Trim()).ToList();
        }

        private void UpdateProgressLabels(List<Subject> subjects)
        {
            decimal totalCompulsoryHours = subjects.Where(s => s.IsCompulsory).Sum(s => s.CreditHours);
            decimal totalElectiveCollegeHours = subjects.Where(s => s.IsElectiveCollege).Sum(s => s.CreditHours);
            decimal totalElectiveUniversityHours = subjects.Where(s => s.IsElectiveUniversity).Sum(s => s.CreditHours);

            lblHoursTaken.Text = $"Compulsory Hours Selected: 0 of {totalCompulsoryHours}";
            lblElectiveCollegeHoursTaken.Text = $"Elective College Hours Selected: 0 of {totalElectiveCollegeHours}";
            lblElectiveUniversityHoursTaken.Text = $"Elective University Hours Selected: 0 of {totalElectiveUniversityHours}";
        }

        private string GetSubjectStatusClass(Subject subject, List<string> takenSubjectCodes)
        {
            if (takenSubjectCodes.Contains(subject.Code))
                return "taken";

            if (subject.Level > GetCurrentLevel(GetUserId()))
                return "unavailable";

            if (subject.Prerequisites.Any() && !subject.Prerequisites.All(p => takenSubjectCodes.Contains(p)))
            {
                return "unavailable";
            }

            // I need later to alter this to have "unoffered" based on the curriculum

            return "available";
        }

        private void RegisterClientScripts(BindSubjectsData data, SubjectsRenderResult renderResult, int currentLevel)
        {
            ClientScript.RegisterStartupScript(GetType(), "Prerequisites", renderResult.PrerequisitesJson, true);
            ClientScript.RegisterStartupScript(GetType(), "SubjectNameMap", renderResult.SubjectNameMapJson, true);
            ClientScript.RegisterStartupScript(GetType(), "SubjectLevelMap", renderResult.SubjectLevelMapJson, true);
            ClientScript.RegisterStartupScript(GetType(), "CreditHoursMap", renderResult.SubjectCreditHoursMapJson, true);
            ClientScript.RegisterStartupScript(GetType(), "SubjectTypeMap", renderResult.SubjectTypeMapJson, true);

            var electivesByLevel = data.Subjects
                .Where(s => (s.IsElectiveCollege || s.IsElectiveUniversity))
                .GroupBy(s => s.Level)
                .ToDictionary(g => g.Key, g => g.Select(s => s.Code).ToList());

            string electiveOptionsJson = "var electiveOptions = " +
                Newtonsoft.Json.JsonConvert.SerializeObject(electivesByLevel) + ";";
            ClientScript.RegisterStartupScript(GetType(), "ElectiveOptions", electiveOptionsJson, true);

            string hoursScript = $@"
                var totalCompulsoryHours = {data.TotalCompulsoryHours};
                var totalElectiveUniversityHours = {data.TotalElectiveUniversityHours};
                var totalElectiveCollegeHours = {data.TotalElectiveCollegeHours};
                var lblHoursTakenId = '{lblHoursTaken.ClientID}';
                var lblElectiveUniversityHoursTakenId = '{lblElectiveUniversityHoursTaken.ClientID}';
                var lblElectiveCollegeHoursTakenId = '{lblElectiveCollegeHoursTaken.ClientID}';
            ";
            ClientScript.RegisterStartupScript(GetType(), "TotalHours", hoursScript, true);
        }
    }
}