using MyScheduleWebsite.App_Code;
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
    public partial class StudentProgressPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Default.aspx");
            }
            else if (!IsPostBack)
            {
                lblGreeting.Text = "Hello " + User.Identity.Name + "!";
                Guid userId = GetUserId();
                lblCurrentLevel.Text = "Current Level: " + getCurrentLevel(userId);

                int universityId = getUniversityId(userId);
                int majorId = getMajorId(userId);
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

        private Guid GetUserId()
        {
            if (Membership.GetUser() is MembershipUser user)
            {
                return (Guid)user.ProviderUserKey;
            }
            return Guid.Empty;
        }

        private int getCurrentLevel(Guid userId)
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

        private int getUniversityId(Guid userId)
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

        private int getMajorId(Guid userId)
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

        private class BindSubjectsData
        {
            public List<Subject> Subjects { get; set; } = new List<Subject>();
            public List<string> AutoSelectedSubjects { get; set; } = new List<string>();
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

        private void BindSubjects(int universityId, int majorId)
        {
            int currentLevel = getCurrentLevel(GetUserId());
            DataTable subjectsTable = GetSubjects(universityId, majorId);

            BindSubjectsData data = ProcessSubjectsData(subjectsTable, currentLevel);

            SubjectsRenderResult renderResult = RenderSubjectsHtmlAndJson(data);

            RegisterClientScripts(data, renderResult, currentLevel);
        }

        private BindSubjectsData ProcessSubjectsData(DataTable subjectsTable, int currentLevel)
        {
            var data = new BindSubjectsData();

            foreach (DataRow row in subjectsTable.Rows)
            {
                var subject = new Subject
                {
                    Code = row["subjectCode"].ToString(),
                    Level = int.Parse(row["subjectLevel"].ToString()),
                    EnglishName = row["subjectEnglishName"]?.ToString() ?? "N/A",
                    ArabicName = row["subjectArabicName"]?.ToString() ?? "N/A",
                    CreditHours = decimal.Parse(row["creditHours"].ToString()),
                    TypeId = int.Parse(row["subjectTypeId"].ToString())
                };

                if (subject.IsCompulsory)
                    data.TotalCompulsoryHours += subject.CreditHours;
                else if (subject.IsElectiveUniversity)
                    data.TotalElectiveUniversityHours += subject.CreditHours;
                else if (subject.IsElectiveCollege)
                    data.TotalElectiveCollegeHours += subject.CreditHours;

                if (row["prerequisites"] != null &&
                    !string.IsNullOrWhiteSpace(row["prerequisites"].ToString()))
                {
                    var prerequisites = row["prerequisites"].ToString().Split(',');
                    subject.Prerequisites = new List<string>(prerequisites);
                }
                else
                {
                    subject.Prerequisites = new List<string>();
                }

                data.Subjects.Add(subject);

                if (subject.Level < currentLevel)
                {
                    data.AutoSelectedSubjects.Add(subject.Code);
                }
            }

            return data;
        }

        private SubjectsRenderResult RenderSubjectsHtmlAndJson(BindSubjectsData data)
        {
            var subjectGroups = data.Subjects.GroupBy(s => s.Level).OrderBy(g => g.Key);
            subjectsContainer.InnerHtml = string.Empty;

            string prerequisitesJson = "var subjectPrerequisites = {";
            string subjectNameMapJson = "var subjectNameMap = {";
            string subjectLevelMapJson = "var subjectLevelMap = {";
            string subjectCreditHoursMapJson = "var subjectCreditHoursMap = {";
            string subjectTypeMapJson = "var subjectTypeMap = {";

            lblHoursSelected.Text = $"Compulsory Hours Selected: 0 of {data.TotalCompulsoryHours}";
            lblElectiveUniversityHoursSelected.Text = $"Elective University Hours Selected: 0 of {data.TotalElectiveUniversityHours}";
            lblElectiveCollegeHoursSelected.Text = $"Elective College Hours Selected: 0 of {data.TotalElectiveCollegeHours}";

            int electivePlaceholderCounter = 1;

            foreach (var group in subjectGroups)
            {
                subjectsContainer.InnerHtml += "<div class='subjects-row'>";

                var CompulsorySubjects = group.Where(s => !s.IsElectiveCollege && !s.IsElectiveUniversity);
                var electiveSubjectsInLevel = group.Where(s => s.IsElectiveCollege || s.IsElectiveUniversity).ToList();

                foreach (var subject in CompulsorySubjects)
                {
                    string subjectCode = subject.Code;
                    string subjectName = subject.EnglishName;
                    subjectsContainer.InnerHtml += $"<div class='subject history' id='{subjectCode}' onclick='SubjectClicked(this)'><span>{subjectName}</span></div>";

                    prerequisitesJson += $"'{subjectCode}': [{string.Join(",", subject.Prerequisites.Select(p => $"'{p}'"))}],";
                    subjectNameMapJson += $"'{subjectCode}': '{subjectName}',";
                    subjectLevelMapJson += $"'{subjectCode}': {subject.Level},";
                    subjectCreditHoursMapJson += $"'{subjectCode}': {subject.CreditHours},";
                    subjectTypeMapJson += $"'{subjectCode}': {subject.TypeId},";
                }

                if (electiveSubjectsInLevel.Any())
                {
                    string placeholderId = $"elective_placeholder_{electivePlaceholderCounter}";
                    subjectsContainer.InnerHtml +=
                        $"<div class='subject elective-slot' id='{placeholderId}' data-level='{group.Key}' onclick='showElectivePopup({group.Key}, \"{placeholderId}\")'>" +
                            $"<span>Elective ({electivePlaceholderCounter})</span>" +
                        $"</div>";
                    foreach (var subject in electiveSubjectsInLevel)
                    {
                        string subjectCode = subject.Code;
                        string subjectName = subject.EnglishName;
                        prerequisitesJson += $"'{subjectCode}': [{string.Join(",", subject.Prerequisites.Select(p => $"'{p}'"))}],";
                        subjectNameMapJson += $"'{subjectCode}': '{subjectName}',";
                        subjectLevelMapJson += $"'{subjectCode}': {subject.Level},";
                        subjectCreditHoursMapJson += $"'{subjectCode}': {subject.CreditHours},";
                        subjectTypeMapJson += $"'{subjectCode}': {subject.TypeId},";
                    }
                    electivePlaceholderCounter++;
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

        private void RegisterClientScripts(BindSubjectsData data, SubjectsRenderResult renderResult, int currentLevel)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PrerequisitesScript", $"<script>{renderResult.PrerequisitesJson}</script>", false);
            ClientScript.RegisterStartupScript(this.GetType(), "SubjectNameMapScript", $"<script>{renderResult.SubjectNameMapJson}</script>", false);
            ClientScript.RegisterStartupScript(this.GetType(), "SubjectLevelMapScript", $"<script>{renderResult.SubjectLevelMapJson}</script>", false);
            ClientScript.RegisterStartupScript(this.GetType(), "SubjectCreditHoursMapScript", $"<script>{renderResult.SubjectCreditHoursMapJson}</script>", false);
            ClientScript.RegisterStartupScript(this.GetType(), "SubjectTypeMapScript", $"<script>{renderResult.SubjectTypeMapJson}</script>", false);

            var electivesByLevel = data.Subjects
                .Where(s => s.IsElectiveCollege || s.IsElectiveUniversity)
                .GroupBy(s => s.Level)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Code).ToList());

            string electiveOptionsJson = "var electiveOptions = {";
            foreach (var kvp in electivesByLevel)
            {
                electiveOptionsJson += $"'{kvp.Key}': [{string.Join(",", kvp.Value.Select(code => $"'{code}'"))}],";
            }
            electiveOptionsJson = electiveOptionsJson.TrimEnd(',') + "};";
            ClientScript.RegisterStartupScript(this.GetType(), "ElectiveOptionsScript", $"<script>{electiveOptionsJson}</script>", false);

            string totalHoursScript = $@"
            var totalCompulsoryHours = {data.TotalCompulsoryHours};
            var totalElectiveUniversityHours = {data.TotalElectiveUniversityHours};
            var totalElectiveCollegeHours = {data.TotalElectiveCollegeHours};
            var lblHoursSelectedId = '{lblHoursSelected.ClientID}';
            var lblElectiveUniversityHoursSelectedId = '{lblElectiveUniversityHoursSelected.ClientID}';
            var lblElectiveCollegeHoursSelectedId = '{lblElectiveCollegeHoursSelected.ClientID}';
            ";
            ClientScript.RegisterStartupScript(this.GetType(), "TotalHoursScript", $"<script>{totalHoursScript}</script>", false);

            string selectedSubjectsJsArray = $"var preSelectedSubjects = {Newtonsoft.Json.JsonConvert.SerializeObject(data.AutoSelectedSubjects)};";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AutoSelectSubjects", $"<script>{selectedSubjectsJsArray}</script>", false);

            string currentLevelJs = $"var currentStudentLevel = {currentLevel};";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CurrentStudentLevel", $"<script>{currentLevelJs}</script>", false);
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {

        }
    }
}