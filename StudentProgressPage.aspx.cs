using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
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
            else
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
                    int currentLevel = getCurrentLevel(GetUserId());
                    string placeholderId = $"elective_placeholder_{electivePlaceholderCounter}";
                    if (group.Key + 1 <= currentLevel)
                    {
                        var firstElective = electiveSubjectsInLevel.First();
                        subjectsContainer.InnerHtml +=
                            $"<div class='subject history history-selected' id='{firstElective.Code}' " +
                            $"data-slot-id='{placeholderId}' data-slot-level='{group.Key}' onclick='toggleElectiveSlot(this)'>" +
                            $"<span>Elective ({electivePlaceholderCounter}) - {firstElective.EnglishName}</span></div>";

                        data.AutoSelectedSubjects.Add(firstElective.Code);
                    }
                    else
                    {
                        // Render empty slot for higher levels
                        subjectsContainer.InnerHtml +=
                            $"<div class='subject elective-slot' id='{placeholderId}' data-level='{group.Key}' " +
                            $"onclick='showElectivePopup({group.Key}, \"{placeholderId}\")'>" +
                            $"<span>Elective ({electivePlaceholderCounter})</span></div>";
                    }
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
        private int GetStudentId(Guid userId)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"SELECT studentId FROM students WHERE UserId = @UserId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@UserId", userId);
            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["studentId"]) : 0;
        }

        private int GetSubjectId(string subjectCode, int universityId, int majorId)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"SELECT subjectId FROM subjects 
                   WHERE subjectCode = @subjectCode 
                     AND universityId = @universityId 
                     AND majorId = @majorId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@subjectCode", subjectCode);
            myPara.Add("@universityId", universityId);
            myPara.Add("@majorId", majorId);
            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["subjectId"]) : 0;
        }

        private string GetSubjectName(string subjectCode, int universityId, int majorId)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"SELECT subjectEnglishName FROM subjects 
                   WHERE subjectCode = @subjectCode 
                     AND universityId = @universityId 
                     AND majorId = @majorId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@subjectCode", subjectCode);
            myPara.Add("@universityId", universityId);
            myPara.Add("@majorId", majorId);
            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            return dt.Rows.Count > 0 ? dt.Rows[0]["subjectEnglishName"].ToString() : subjectCode;
        }
        private bool IsSubjectTaken(int studentId, int subjectId)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"SELECT COUNT(*) FROM studentsProgress WHERE studentId = @studentId AND subjectId = @subjectId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@studentId", studentId);
            myPara.Add("@subjectId", subjectId);
            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        private List<string> GetPrerequisiteCodes(int subjectId)
        {
            List<string> prerequisites = new List<string>();
            CRUD myCrud = new CRUD();
            string mySql = @"SELECT prerequisites FROM subjects WHERE subjectId = @subjectId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@subjectId", subjectId);
            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            if (dt.Rows.Count > 0 && dt.Rows[0]["prerequisites"] != DBNull.Value)
            {
                string prereqStr = dt.Rows[0]["prerequisites"].ToString();
                prerequisites = prereqStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(p => p.Trim()).ToList();
            }
            return prerequisites;
        }
        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            Guid userId = GetUserId();
            int studentId = GetStudentId(userId);
            int universityId = getUniversityId(userId);
            int majorId = getMajorId(userId);

            if (studentId == 0)
            {
                lblOutput.Text = "Student not found!";
                lblOutput.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string selectedSubjectsCSV = hdnSelectedSubjects.Value;
            List<string> selectedSubjectCodes = selectedSubjectsCSV.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (string.IsNullOrEmpty(selectedSubjectsCSV))
            {
                lblOutput.Text = "No subjects selected!";
                lblOutput.ForeColor = System.Drawing.Color.Red;
                return;
            }

            List<string> errors = new List<string>();
            Dictionary<string, int> validSubjects = new Dictionary<string, int>();
            List<string> reservedSubjects = new List<string>();

            foreach (string code in selectedSubjectCodes)
            {
                int subjectId = GetSubjectId(code, universityId, majorId);
                if (subjectId == 0)
                {
                    errors.Add($"Invalid subject code: {code}");
                    continue;
                }

                if (IsSubjectTaken(studentId, subjectId))
                {
                    reservedSubjects.Add(code);
                    continue;
                }

                List<string> prerequisiteCodes = GetPrerequisiteCodes(subjectId);
                List<string> missingPrerequisites = new List<string>();

                foreach (string prereqCode in prerequisiteCodes)
                {
                    int prereqSubjectId = GetSubjectId(prereqCode, universityId, majorId);
                    if (prereqSubjectId == 0)
                    {
                        errors.Add($"Invalid prerequisite '{prereqCode}'");
                        continue;
                    }

                    bool isTaken = IsSubjectTaken(studentId, prereqSubjectId);
                    bool isSelected = selectedSubjectCodes.Contains(prereqCode);

                    if (!isTaken && !isSelected)
                    {
                        missingPrerequisites.Add(GetSubjectName(prereqCode, universityId, majorId));
                    }
                }

                if (missingPrerequisites.Count > 0)
                {
                    errors.Add($"{GetSubjectName(code, universityId, majorId)} requires: {string.Join(", ", missingPrerequisites)}");
                }
                else
                {
                    validSubjects.Add(code, subjectId);
                }
            }

            if (errors.Count > 0)
            {
                lblOutput.Text = string.Join("<br />", errors);
                lblOutput.ForeColor = System.Drawing.Color.Red;
            }

            List<string> successfullyInserted = new List<string>();
            bool shouldRollback = false;
            Exception actualError = null;
            CRUD myCrud = null;

            try
            {
                foreach (var subject in validSubjects)
                {
                    myCrud = new CRUD();

                    string insertSql = @"INSERT INTO studentsProgress (studentId, subjectId) 
                                   VALUES (@studentId, @subjectId)";

                    Dictionary<string, object> insertParams = new Dictionary<string, object>
                    {
                        { "@studentId", studentId },
                        { "@subjectId", subject.Value }
                    };

                    int result = myCrud.InsertUpdateDelete(insertSql, insertParams);

                    if (result > 0)
                    {
                        successfullyInserted.Add(subject.Key);
                    }
                    else
                    {
                        shouldRollback = true;
                        actualError = new Exception($"Failed to insert subject {subject.Key}");
                        break;
                    }

                    myCrud = null;
                }

                if (shouldRollback)
                {
                    throw actualError;
                }

                if (successfullyInserted.Count > 0 && errors.Count == 0)
                {
                    Response.Redirect("StudentPage.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                shouldRollback = true;
                actualError = ex;
            }
            finally
            {
                try
                {
                    if (shouldRollback && successfullyInserted.Count > 0)
                    {
                        foreach (string subjectCode in successfullyInserted)
                        {
                            var deleteCrud = new CRUD();
                            try
                            {
                                int subjectId = GetSubjectId(subjectCode, universityId, majorId);
                                string deleteSql = @"DELETE FROM studentsProgress 
                                             WHERE studentId = @studentId 
                                               AND subjectId = @subjectId";

                                deleteCrud.InsertUpdateDelete(deleteSql, new Dictionary<string, object>
                                {
                                    { "@studentId", studentId },
                                    { "@subjectId", subjectId }
                                });
                            }
                            finally
                            {
                                deleteCrud = null;
                            }
                        }
                    }
                }
                finally
                {
                    hdnSelectedSubjects.Value = string.Join(",", selectedSubjectCodes);
                    ScriptManager.RegisterStartupScript(this, GetType(), "restoreSelection",
                        $"restoreSelection('{hdnSelectedSubjects.ClientID}');", true);

                    var reservedSubjectsJson = Newtonsoft.Json.JsonConvert.SerializeObject(reservedSubjects);
                    ScriptManager.RegisterStartupScript(this, GetType(), "markReservedSubjects",
                        $"markReservedSubjects({reservedSubjectsJson});", true);

                    myCrud = null;

                    if (shouldRollback)
                    {
                        lblOutput.Text = $"Error: {actualError.Message}. Changes rolled back.";
                        lblOutput.ForeColor = Color.Red;
                    }
                }
            }
        }

    }
}