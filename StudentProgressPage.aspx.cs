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
            string mySql = @"SELECT * FROM subjects WHERE universityId = 1 AND majorId = 1 AND (subjectTypeId = 1 OR subjectTypeId = 2)";

            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@universityId", universityId);
            myPara.Add("@majorId", majorId);

            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            return dt;
        }

        private void BindSubjects(int universityId, int majorId)
        {
            int currentLevel = getCurrentLevel(GetUserId());

            CRUD crud = new CRUD();
            DataTable subjectsTable = GetSubjects(universityId, majorId);

            List<Subject> subjects = new List<Subject>();
            List<string> autoSelectedSubjects = new List<string>();

            Dictionary<string, string> subjectNameMap = new Dictionary<string, string>();

            Dictionary<string, int> subjectLevelMap = new Dictionary<string, int>();

            decimal totalCompulsoryHours = 0;
            decimal totalElectiveUniversityHours = 0;
            decimal totalElectiveCollegeHours = 0;

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

                if (subject.IsCompulsory) totalCompulsoryHours += subject.CreditHours;
                else if (subject.IsElectiveUniversity) totalElectiveUniversityHours += subject.CreditHours;
                else if (subject.IsElectiveCollege) totalElectiveCollegeHours += subject.CreditHours;

                if (row["prerequisites"] != null && !string.IsNullOrWhiteSpace(row["prerequisites"].ToString()))
                {
                    var prerequisites = row["prerequisites"].ToString().Split(',');
                    subject.Prerequisites = new List<string>(prerequisites);
                }
                else
                {
                    subject.Prerequisites = new List<string>();
                }

                subjects.Add(subject);

                subjectNameMap[subject.Code] = subject.EnglishName;

                subjectLevelMap[subject.Code] = subject.Level;

                if (subject.Level < currentLevel)
                {
                    autoSelectedSubjects.Add(subject.Code);
                }
            }

            var subjectGroups = subjects.GroupBy(s => s.Level);
            subjectsContainer.InnerHtml = string.Empty;

            string prerequisitesJson = "var subjectPrerequisites = {";
            string subjectNameMapJson = "var subjectNameMap = {";
            string subjectLevelMapJson = "var subjectLevelMap = {";
            string subjectCreditHoursMapJson = "var subjectCreditHoursMap = {";
            string subjectTypeMapJson = "var subjectTypeMap = {";

            lblHoursSelected.Text = $"Compulsory Hours Selected: 0 of {totalCompulsoryHours}";
            lblElectiveUniversityHoursSelected.Text = $"Elective University Hours Selected: 0 of {totalElectiveUniversityHours}";
            lblElectiveCollegeHoursSelected.Text = $"Elective College Hours Selected: 0 of {totalElectiveCollegeHours}";

            foreach (var group in subjectGroups)
            {
                subjectsContainer.InnerHtml += "<div class='subjects-row'>";

                foreach (var subject in group)
                {
                    string subjectCode = subject.Code;
                    string subjectName = subject.EnglishName;
                    string prerequisites = string.Join(",", subject.Prerequisites);

                    subjectsContainer.InnerHtml += $"<div class='subject history' id='{subjectCode}' onclick='SubjectClicked(this)'><span>{subjectName}</span></div>";

                    prerequisitesJson += $"'{subjectCode}': [{string.Join(",", subject.Prerequisites.Select(p => $"'{p}'"))}],";
                    subjectNameMapJson += $"'{subjectCode}': '{subjectName}',";
                    subjectLevelMapJson += $"'{subjectCode}': {subject.Level},";
                    subjectCreditHoursMapJson += $"'{subject.Code}': {subject.CreditHours},";
                    subjectTypeMapJson += $"'{subject.Code}': {subject.TypeId},";
                }

                subjectsContainer.InnerHtml += "</div>";
            }

            prerequisitesJson = prerequisitesJson.TrimEnd(',') + "};";
            subjectNameMapJson = subjectNameMapJson.TrimEnd(',') + "};";
            subjectLevelMapJson = subjectLevelMapJson.TrimEnd(',') + "};";
            subjectCreditHoursMapJson = subjectCreditHoursMapJson.TrimEnd(',') + "};";
            subjectTypeMapJson = subjectTypeMapJson.TrimEnd(',') + "};";

            ClientScript.RegisterStartupScript(this.GetType(), "PrerequisitesScript", $"<script>{prerequisitesJson}</script>", false);
            ClientScript.RegisterStartupScript(this.GetType(), "SubjectNameMapScript", $"<script>{subjectNameMapJson}</script>", false);
            ClientScript.RegisterStartupScript(this.GetType(), "SubjectLevelMapScript", $"<script>{subjectLevelMapJson}</script>", false);
            ClientScript.RegisterStartupScript(this.GetType(), "SubjectCreditHoursMapScript", $"<script>{subjectCreditHoursMapJson}</script>", false);
            ClientScript.RegisterStartupScript(this.GetType(), "SubjectTypeMapScript", $"<script>{subjectTypeMapJson}</script>", false);
            string totalHoursScript = $@"
            var totalCompulsoryHours = {totalCompulsoryHours};
            var totalElectiveUniversityHours = {totalElectiveUniversityHours};
            var totalElectiveCollegeHours = {totalElectiveCollegeHours};
            var lblHoursSelectedId = '{lblHoursSelected.ClientID}';
            var lblElectiveUniversityHoursSelectedId = '{lblElectiveUniversityHoursSelected.ClientID}';
            var lblElectiveCollegeHoursSelectedId = '{lblElectiveCollegeHoursSelected.ClientID}';
            ";
            ClientScript.RegisterStartupScript(this.GetType(), "TotalHoursScript", $"<script>{totalHoursScript}</script>", false);

            string selectedSubjectsJsArray = $"var preSelectedSubjects = {Newtonsoft.Json.JsonConvert.SerializeObject(autoSelectedSubjects)};";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AutoSelectSubjects", $"<script>{selectedSubjectsJsArray}</script>", false);

            string currentLevelJs = $"var currentStudentLevel = {currentLevel};";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CurrentStudentLevel", $"<script>{currentLevelJs}</script>", false);
        }
    }
}