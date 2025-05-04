using MyScheduleWebsite.App_Code;
using Newtonsoft.Json;
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

            CheckExistingOrder(studentId);

            SqlConnection.ClearAllPools();

            ViewState["UniversityId"] = universityId;
            ViewState["MajorId"] = majorId;

            List<string> takenSubjects = GetTakenSubjectCodes(studentId, universityId, majorId);
            if (takenSubjects.Count == 0)
            {
                Response.Redirect("~/StudentProgressPage.aspx");
                return;
            }
            else if (!IsPostBack)
            {
                mvSteps.ActiveViewIndex = 0;

                lblGreeting.Text = "Hello " + User.Identity.Name + "!";
                lblCurrentLevel.Text = "Current Level: " + GetCurrentLevel(userId);
            }
            BindSubjects(universityId, majorId);

            if (mvSteps.ActiveViewIndex == 1)
            {
                BindSelectedSubjects();
                PreloadSectionsData();
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
            public double Score { get; set; }
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
            public string SubjectOfferedMapJson { get; set; }
        }
        private class Section
        {
            public int SectionId { get; set; }
            public string SubjectCode { get; set; }
            public string SubjectEnglishName { get; set; }
            public int SectionNumber { get; set; }
            public int Capacity { get; set; }
            public int RegisteredStudents { get; set; }
            public string InstructorArabicName { get; set; }
            public List<SectionDetail> Details { get; set; } = new List<SectionDetail>();
        }

        private class SectionDetail
        {
            public int Day { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public string Location { get; set; }
        }

        int curriculumId = 2;

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
        private void CheckExistingOrder(int studentId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT COUNT(*) FROM orders WHERE studentId = @studentId";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@studentId", studentId }
                };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            if (dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0][0]) > 0)
            {
                Response.Redirect("~/OrderSuccessfulPage.aspx");
            }
        }

        private List<string> GetTakenSubjectCodes(int studentId, int universityId, int majorId)
        {
            List<string> takenSubjects = new List<string>();
            var allSubjects = ProcessSubjects(universityId, majorId) ?? new List<Subject>();

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
                string code = row["subjectCode"].ToString().Trim();
                takenSubjects.Add(code);

                var subject = allSubjects.FirstOrDefault(s =>
                    s.Code.Trim().Equals(code, StringComparison.OrdinalIgnoreCase)
                );

                if (subject != null && (subject.IsElectiveCollege || subject.IsElectiveUniversity))
                {
                    var slotId = $"{subject.Level}-{(subject.IsElectiveCollege ? "College" : "University")}";
                    takenSubjects.Add(slotId);
                }
            }

            return takenSubjects.Distinct().ToList();
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

            RegisterClientScripts(data, renderResult);
            UpdateProgressLabels(subjects);

            if (subjects.Any())
            {
                int maxLevel = subjects.Max(s => s.Level);
                int remainingSemesters = maxLevel - currentLevel;

                if (remainingSemesters <= 0)
                {
                    lblGraduation.Text = "Graduation In: This Semester";
                }
                else
                {
                    lblGraduation.Text = $"Graduation In: {remainingSemesters} " +
                                       $"{(remainingSemesters == 1 ? "Semester" : "Semesters")}";
                }
            }

            var subjectOfferedMap = subjects.ToDictionary(
                s => s.Code,
                s => IsSubjectOffered(s.Code, universityId, majorId, curriculumId)
            );

            hdnCurrentLevel.Value = GetCurrentLevel(GetUserId()).ToString();

            var recommended = CalculateRecommendedSubjects(subjects, takenSubjects, currentLevel)
                           .Take(10)
                           .Select(s => new {
                               Code = s.Code,
                               EnglishName = s.EnglishName,
                               Level = s.Level,
                               CreditHours = s.CreditHours,
                               TypeId = s.TypeId,
                               Score = s.Score
                           })
                           .ToList();


            hdnRecommendedSubjects.Value = JsonConvert.SerializeObject(recommended.Select(r => r.Code));

            lblRecommendations.Text = @"
                <table class='recommendation-table'>
                    <tr>
                        <th>Subject Name</th>
                        <th>Level</th>
                        <th>Credit Hours</th>
                        <th>Type</th>
                        <th>Score</th>
                    </tr>
                    " + string.Join("", recommended.Select(s => $@"
                    <tr>
                        <td>{s.EnglishName}</td>
                        <td>{s.Level}</td>
                        <td>{s.CreditHours}</td>
                        <td>{(s.TypeId == 1 || s.TypeId == 2 ? "Compulsory" :
                     s.TypeId == 3 ? "Elective College" : "Elective University")}</td>
                        <td class='score-cell'>{s.Score:0.00}</td>
                    </tr>")) + @"
                </table>";
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
            SqlConnection.ClearAllPools();
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
            string subjectOfferedMapJson = "var subjectOfferedMap = {";

            Guid userId = GetUserId();
            int universityId = GetUniversityId(userId);
            int majorId = GetMajorId(userId);

            var offeredSubjects = data.Subjects.ToDictionary(
                s => s.Code,
                s => IsSubjectOffered(s.Code, universityId, majorId, curriculumId)
            );

            var groupedSubjects = data.Subjects.GroupBy(s => s.Level).OrderBy(g => g.Key);

            foreach (var group in groupedSubjects)
            {
                subjectsContainer.InnerHtml += "<div class='subjects-row'>";

                foreach (var subject in group)
                {
                    AddSubjectToJsonMaps(subject, offeredSubjects,
                        ref prerequisitesJson,
                        ref subjectNameMapJson,
                        ref subjectLevelMapJson,
                        ref subjectCreditHoursMapJson,
                        ref subjectTypeMapJson,
                        ref subjectOfferedMapJson);
                }

                foreach (var subject in group.Where(s => s.IsCompulsory))
                {
                    RenderSubjectHtml(subject, takenSubjectCodes);
                }

                var electives = group.Where(s => s.IsElectiveCollege || s.IsElectiveUniversity).ToList();
                CreateElectiveSlots(electives, group.Key, ref electivePlaceholderCounter, currentLevel, takenSubjectCodes);

                subjectsContainer.InnerHtml += "</div>";
            }

            prerequisitesJson = prerequisitesJson.TrimEnd(',') + "};";
            subjectNameMapJson = subjectNameMapJson.TrimEnd(',') + "};";
            subjectLevelMapJson = subjectLevelMapJson.TrimEnd(',') + "};";
            subjectCreditHoursMapJson = subjectCreditHoursMapJson.TrimEnd(',') + "};";
            subjectTypeMapJson = subjectTypeMapJson.TrimEnd(',') + "};";
            subjectOfferedMapJson = subjectOfferedMapJson.TrimEnd(',') + "};";

            return new SubjectsRenderResult
            {
                PrerequisitesJson = prerequisitesJson,
                SubjectNameMapJson = subjectNameMapJson,
                SubjectLevelMapJson = subjectLevelMapJson,
                SubjectCreditHoursMapJson = subjectCreditHoursMapJson,
                SubjectTypeMapJson = subjectTypeMapJson,
                SubjectOfferedMapJson = subjectOfferedMapJson
            };
        }

        private void CreateElectiveSlots(List<Subject> electives, int level, ref int counter, int currentLevel, List<string> takenSubjectCodes)
        {
            if (!electives.Any()) return;

            var takenElective = electives.FirstOrDefault(s => takenSubjectCodes.Contains(s.Code));
            var slotNumber = counter++;

            if (takenElective != null)
            {
                subjectsContainer.InnerHtml += $@"
            <div class='subject taken elective' id='{takenElective.Code}'
                 data-level='{level}',
                 data-current-level='{currentLevel}'>
                <span>Elective ({slotNumber}) - {takenElective.EnglishName}</span>
            </div>";
            }
            else
            {
                bool isSlotUnavailable = level > currentLevel;
                string slotClass = isSlotUnavailable ?
                    "subject elective-slot unavailable-slot" :
                    "subject elective-slot";

                subjectsContainer.InnerHtml += $@"
                    <div class='{slotClass}' 
                         id='elective_placeholder_{slotNumber}'
                         data-level='{level}'
                         data-current-level='{currentLevel}'
                         data-max-selections='{(level == 10 ? 2 : 1)}'
                         onclick='showElectivePopup({level}, this)'>
                        <span>Elective ({slotNumber})</span>
                    </div>";
            }

            // Note to self, this condition down here is wrong, it shouldn't be hardcoded, but we'll keep it for now until it is implemented in the DB.
            if (counter == 6)
            {
                string placeholderId = $"elective_placeholder_{counter}";
                bool isSlotUnavailable = level > currentLevel;
                string slotClass = isSlotUnavailable ?
                    "subject elective-slot unavailable-slot" :
                    "subject elective-slot";

                subjectsContainer.InnerHtml += $@"
                    <div class='{slotClass}' 
                         id='elective_placeholder_{slotNumber}'
                         data-level='{level}'
                         data-current-level='{currentLevel}'
                         data-max-selections='{(level == 10 ? 2 : 1)}'
                         onclick='showElectivePopup({level}, this)'>
                        <span>Elective ({slotNumber + 1})</span>
                    </div>";
            }
        }

        private void AddSubjectToJsonMaps(Subject subject,
            Dictionary<string, bool> offeredSubjects,
            ref string prerequisitesJson,
            ref string subjectNameMapJson,
            ref string subjectLevelMapJson,
            ref string subjectCreditHoursMapJson,
            ref string subjectTypeMapJson,
            ref string subjectOfferedMapJson)
        {
                    prerequisitesJson += $"'{subject.Code}': [{string.Join(",", subject.Prerequisites.Select(p => $"'{p}'"))}],";
                    subjectNameMapJson += $"'{subject.Code}': '{subject.EnglishName.Replace("'", "\\'")}',";
                    subjectLevelMapJson += $"'{subject.Code}': {subject.Level},";
                    subjectCreditHoursMapJson += $"'{subject.Code}': {subject.CreditHours},";
                    subjectTypeMapJson += $"'{subject.Code}': {subject.TypeId},";
                    bool isOffered = offeredSubjects[subject.Code];
                    subjectOfferedMapJson += $"'{subject.Code}': {isOffered.ToString().ToLower()},";
        }

        private void RenderSubjectHtml(Subject subject, List<string> takenSubjectCodes)
        {
            string statusClass = GetSubjectStatusClass(subject, takenSubjectCodes);
            bool isClickable = statusClass == "available" || statusClass == "unoffered";
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

            if (subject.IsElectiveCollege || subject.IsElectiveUniversity)
            {
                var slotIdentifier = $"{subject.Level}-{(subject.IsElectiveCollege ? "College" : "University")}";
                if (takenSubjectCodes.Any(t => t.StartsWith(slotIdentifier)))
                    return "unavailable";
            }

            if (subject.Level > GetCurrentLevel(GetUserId()))
                return "unavailable";

            var missingPrerequisites = subject.Prerequisites
                .Where(p => !takenSubjectCodes.Contains(p))
                .ToList();

            if (missingPrerequisites.Any())
            {
                return "unavailable";
            }

            bool isOffered = IsSubjectOffered(
                subject.Code,
                (int)ViewState["UniversityId"],
                (int)ViewState["MajorId"],
                curriculumId
            );

            if (!isOffered)
                return "unoffered";

            return "available";
        }

        private bool IsSubjectOffered(string subjectCode, int universityId, int majorId, int curriculumId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"
                SELECT COUNT(*) 
                FROM sections s
                INNER JOIN subjects sub ON s.subjectCode = sub.subjectCode
                WHERE s.curriculumId = @curriculumId
                AND sub.subjectCode = @subjectCode
                AND sub.universityId = @universityId
                AND sub.majorId = @majorId";

                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@curriculumId", curriculumId },
                    { "@subjectCode", subjectCode },
                    { "@universityId", universityId },
                    { "@majorId", majorId }
                };

            DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);
            return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        private void RegisterClientScripts(BindSubjectsData data, SubjectsRenderResult renderResult)
        {
            ClientScript.RegisterStartupScript(GetType(), "Prerequisites", renderResult.PrerequisitesJson, true);
            ClientScript.RegisterStartupScript(GetType(), "SubjectNameMap", renderResult.SubjectNameMapJson, true);
            ClientScript.RegisterStartupScript(GetType(), "SubjectLevelMap", renderResult.SubjectLevelMapJson, true);
            ClientScript.RegisterStartupScript(GetType(), "CreditHoursMap", renderResult.SubjectCreditHoursMapJson, true);
            ClientScript.RegisterStartupScript(GetType(), "SubjectTypeMap", renderResult.SubjectTypeMapJson, true);
            ClientScript.RegisterStartupScript(GetType(), "SubjectOfferedMap", renderResult.SubjectOfferedMapJson, true);

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

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (mvSteps.ActiveViewIndex == 0)
            {
                var selectedSubjects = hdnSelectedSubjects.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (!ValidateSubjects(selectedSubjects))
                {
                    return;
                }

                ViewState["SelectedSubjects"] = selectedSubjects;
                Session["SelectedSubjects"] = selectedSubjects;
                mvSteps.ActiveViewIndex = 1;
                btnNext.Text = "Confirm Order";

                BindSelectedSubjects();
                PreloadSectionsData();
            }
            else if (mvSteps.ActiveViewIndex == 1)
            {
                List<string> errors = ValidateSections();
                if (errors.Count > 0)
                {
                    lblOutput2.Text = string.Join("<br />", errors);
                    lblOutput2.ForeColor = Color.Red;
                }
                else if (ConfirmOrder())
                {
                    Response.Redirect("~/OrderSuccessfulPage.aspx");
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (mvSteps.ActiveViewIndex == 1)
            {
                mvSteps.ActiveViewIndex = 0;
                btnNext.Text = "Next";
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        private bool ValidateSubjects(List<string> selectedCodes)
        {
            List<string> errors = new List<string>();

            if (selectedCodes == null || selectedCodes.Count == 0)
            {
                errors.Add("No subjects selected.");
                lblOutput.Text = string.Join("<br />", errors);
                return false;
            }

            Guid userId = GetUserId();
            int studentId = GetStudentId(userId);
            int universityId = (int)ViewState["UniversityId"];
            int majorId = (int)ViewState["MajorId"];
            List<string> takenSubjects = GetTakenSubjectCodes(studentId, universityId, majorId);
            List<Subject> allSubjects = ProcessSubjects(universityId, majorId);
            List<Subject> selectedSubjects = new List<Subject>();

            foreach (string code in selectedCodes)
            {
                Subject subject = allSubjects.FirstOrDefault(s => s.Code == code);
                if (subject == null)
                {
                    errors.Add($"Subject with code {code} does not exist.");
                    continue;
                }
                selectedSubjects.Add(subject);

                string status = GetSubjectStatusClass(subject, takenSubjects);
                if (status != "available" && status != "unoffered")
                {
                    errors.Add($"Subject {subject.EnglishName} ({code}) is not available for selection.");
                }
            }

            decimal totalHours = selectedSubjects.Sum(s => s.CreditHours);
            if (totalHours < 12)
            {
                errors.Add($"Total credit hours ({totalHours}) is less than the minimum required 12.");
            }
            else if (totalHours > 20)
            {
                errors.Add($"Total credit hours ({totalHours}) exceeds the maximum allowed 20.");
            }

            foreach (Subject subject in selectedSubjects)
            {
                foreach (string prereq in subject.Prerequisites)
                {
                    if (!takenSubjects.Contains(prereq))
                    {
                        string prereqName = allSubjects.FirstOrDefault(s => s.Code == prereq)?.EnglishName ?? prereq;
                        errors.Add($"Subject {subject.EnglishName} requires {prereqName} which has not been taken.");
                    }
                }
            }

            if (errors.Count > 0)
            {
                lblOutput.Text = string.Join("<br />", errors);
                lblOutput.ForeColor = Color.Red;
                return false;
            }
            return true;
        }

        private List<Subject> CalculateRecommendedSubjects(List<Subject> allSubjects, List<string> takenSubjects, int currentLevel)
        {
            var availableSubjects = allSubjects
                .Where(s => GetSubjectStatusClass(s, takenSubjects) == "available" ||
                           GetSubjectStatusClass(s, takenSubjects) == "unoffered")
                .ToList();

            if (!availableSubjects.Any()) return availableSubjects;

            var prerequisiteImpact = new Dictionary<string, int>();
            foreach (var subject in allSubjects)
            {
                foreach (var prereq in subject.Prerequisites)
                {
                    prerequisiteImpact[prereq] = prerequisiteImpact.ContainsKey(prereq)
                        ? prerequisiteImpact[prereq] + 1
                        : 1;
                }
            }

            var prereqMax = prerequisiteImpact.Values.DefaultIfEmpty(0).Max();
            var maxValues = new
            {
                LevelDiff = Math.Max(availableSubjects.Max(s => currentLevel - s.Level), 1),
                PrereqImpact = prereqMax > 0 ? prereqMax : 1,
                CreditHours = availableSubjects.Max(s => s.CreditHours)
            };

            var maxCreditHours = Math.Max(maxValues.CreditHours, 1m);
            bool finalYear = allSubjects.Any() && (allSubjects.Max(s => s.Level) - currentLevel) <= 1;

            foreach (var subject in availableSubjects)
            {
                prerequisiteImpact.TryGetValue(subject.Code, out int prereqCount);

                var scores = new
                {
                    LevelPriority = (double)(currentLevel - subject.Level) / maxValues.LevelDiff,
                    Compulsory = subject.IsCompulsory ? 1.0 : 0.0,
                    PrerequisiteCriticality = (double)prereqCount / maxValues.PrereqImpact,
                    CreditHours = (double)subject.CreditHours / (double)maxCreditHours,
                    //GraduationUrgency = (finalYear && subject.IsCompulsory) ? 1.0 : 0.0
                };

                subject.Score =
                    (scores.LevelPriority * 0.3d) +
                    (scores.Compulsory * 0.3d) +
                    (scores.PrerequisiteCriticality * 0.25d) +
                    (scores.CreditHours * 0.15d);// +
                    //(scores.GraduationUrgency * 0.1d)
            }

            var pastLevelSubjects = availableSubjects
                .Where(s => s.Level < currentLevel)
                .OrderByDescending(s => s.Score)
                .ToList();

            var currentLevelSubjects = availableSubjects
                .Where(s => s.Level == currentLevel)
                .ToList();

            bool isLate = pastLevelSubjects.Any();
            int targetHours = isLate ? 20 : 18;

            var recommendation = new List<Subject>();

            AddToRecommendation(pastLevelSubjects, recommendation, targetHours);

            var currentCompulsory = currentLevelSubjects
                .Where(s => s.IsCompulsory)
                .OrderByDescending(s => s.Score)
                .ToList();

            var electiveSlots = currentLevelSubjects
                .Where(s => s.IsElectiveCollege || s.IsElectiveUniversity)
                .GroupBy(s => new { s.Level, Type = s.IsElectiveCollege ? "College" : "University" })
                .Select(g => g.OrderByDescending(s => s.Score).First())
                .ToList();

            var mandatoryCurrent = currentCompulsory
                .Concat(electiveSlots)
                .OrderByDescending(s => s.Score)
                .ToList();

            AddToRecommendation(mandatoryCurrent, recommendation, targetHours);

            var remainingSubjects = availableSubjects
                .Except(pastLevelSubjects)
                .Except(mandatoryCurrent)
                .OrderByDescending(s => s.Score)
                .ToList();

            AddToRecommendation(remainingSubjects, recommendation, targetHours);

            if (GetTotalHours(recommendation) < 12)
            {
                var fallback = availableSubjects
                    .OrderByDescending(s => s.Level < currentLevel)
                    .ThenByDescending(s => s.Score)
                    .ToList();

                recommendation.Clear();
                AddToRecommendation(fallback, recommendation, 12);
            }

            return recommendation.OrderByDescending(s => s.Score).ToList();
        }

        private void AddToRecommendation(List<Subject> source, List<Subject> destination, int targetHours)
        {
            int currentTotal = GetTotalHours(destination);
            int currentLevel = GetCurrentLevel(GetUserId());
            var orderedSource = source
                .OrderByDescending(s => s.Level < currentLevel)
                .ThenByDescending(s => s.Score)
                .ToList();

            foreach (var subject in orderedSource)
            {
                if (currentTotal >= targetHours) break;
                if (currentTotal + subject.CreditHours > 20) continue;
                if (IsElectiveSlotFilled(subject, destination)) continue;

                if ((currentTotal + subject.CreditHours < 12) &&
                    (currentTotal + subject.CreditHours + (12 - currentTotal - subject.CreditHours) > 20))
                {
                    continue;
                }

                destination.Add(subject);
                currentTotal += (int)subject.CreditHours;
            }
        }

        private bool IsElectiveSlotFilled(Subject subject, List<Subject> selected)
        {
            if (!subject.IsElectiveCollege && !subject.IsElectiveUniversity) return false;

            // Note to self, this is wrong, change it later.
            if (subject.Level == 10)
            {
                int count = selected.Count(s => s.IsElectiveCollege || s.IsElectiveUniversity);
                return count >= 2;
            }

            var slotKey = $"{subject.Level}-{(subject.IsElectiveCollege ? "College" : "University")}";
            return selected.Any(s =>
                (s.IsElectiveCollege || s.IsElectiveUniversity) &&
                $"{s.Level}-{(s.IsElectiveCollege ? "College" : "University")}" == slotKey);
        }

        private int GetTotalHours(List<Subject> subjects) =>
            subjects.Sum(s => (int)Math.Ceiling(s.CreditHours));

        private void BindSelectedSubjects()
        {
            subjectsinSectionsContainer.InnerHtml = "<div class='subjects-content-wrapper'>";

            var selectedCodes = Session["SelectedSubjects"] as List<string>;
            if (selectedCodes == null || selectedCodes.Count == 0)
            {
                subjectsinSectionsContainer.InnerHtml += "<div class='labels'>No subjects selected</div></div>";
                return;
            }

            Guid userId = GetUserId();
            int universityId = GetUniversityId(userId);
            int majorId = GetMajorId(userId);
            int studentId = GetStudentId(userId);

            var allSubjects = ProcessSubjects(universityId, majorId);
            List<string> takenSubjects = GetTakenSubjectCodes(studentId, universityId, majorId);

            var subjects = allSubjects
                .Where(s => selectedCodes.Contains(s.Code))
                .Select(s => new {
                    Subject = s,
                    Status = GetSubjectStatusClass(s, takenSubjects)
                })
                .OrderBy(x => x.Status == "unoffered")
                .ToList();

            string subjectsHtml = "";
            foreach (var subject in subjects)
            {
                subjectsHtml += $@"
            <div class='subject subject-in-sections {(subject.Status == "unoffered" ? "unoffered" : "")}' 
                 id='{subject.Subject.Code}' 
                 onclick='SubjectInSectionsClicked(this)'>
                <span>{subject.Subject.EnglishName}</span>
            </div>";
            }

            subjectsinSectionsContainer.InnerHtml += subjectsHtml + "</div>";
        }
        
        private List<Section> GetSectionsForSubjects(List<string> subjectCodes)
        {
            if (subjectCodes.Count == 0) return new List<Section>();

            CRUD myCrud = new CRUD();
            string sql = @"
                SELECT s.sectionId, s.subjectCode, sub.subjectEnglishName, 
                       s.sectionNumber, s.capacity, s.registeredStudents, 
                       s.instuctorArabicName, sd.day, sd.startTime, 
                       sd.endTime, sd.location
                       FROM sections s
                       INNER JOIN sectionDetails sd ON s.sectionId = sd.sectionId
                       INNER JOIN subjects sub ON s.subjectCode = sub.subjectCode
                       WHERE s.curriculumId = @curriculumId
                       AND s.subjectCode IN ({0})";

            string[] parameters = subjectCodes.Select((_, i) => $"@p{i}").ToArray();
            sql = string.Format(sql, string.Join(",", parameters));

            Dictionary<string, object> para = new Dictionary<string, object>();
            para.Add("@curriculumId", curriculumId);
            for (int i = 0; i < subjectCodes.Count; i++)
            {
                para.Add($"@p{i}", subjectCodes[i]);
            }

            DataTable dt = myCrud.getDTPassSqlDic(sql, para);

            var sections = dt.AsEnumerable()
                .GroupBy(r => r.Field<int>("sectionId"))
                .Select(g => new Section
                {
                    SectionId = g.Key,
                    SubjectCode = g.First().Field<string>("subjectCode"),
                    SubjectEnglishName = g.First().Field<string>("subjectEnglishName"),
                    SectionNumber = g.First().Field<int>("sectionNumber"),
                    Capacity = g.First().Field<int>("capacity"),
                    RegisteredStudents = g.First().Field<int>("registeredStudents"),
                    InstructorArabicName = g.First().Field<string>("instuctorArabicName"),
                    Details = g.Select(r => new SectionDetail
                    {
                        Day = r.Field<int>("day"),
                        StartTime = r.Field<TimeSpan>("startTime"),
                        EndTime = r.Field<TimeSpan>("endTime"),
                        Location = r.Field<string>("location")
                    }).ToList()
                }).ToList();

            SqlConnection.ClearAllPools();

            return sections;
        }
        private void PreloadSectionsData()
        {
            var selectedCodes = Session["SelectedSubjects"] as List<string> ?? new List<string>();
            var sections = GetSectionsForSubjects(selectedCodes);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(
                sections,
                Newtonsoft.Json.Formatting.None,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                }
            );

            SqlConnection.ClearAllPools();

            string script = $@"
                window.allSections = {json};";

            ClientScript.RegisterStartupScript(
                GetType(),
                "SectionsData",
                script,
                true
            );
        }
        private List<string> ValidateSections()
        {
            List<string> errors = new List<string>();
            var selectedSubjects = Session["SelectedSubjects"] as List<string> ?? new List<string>();
            var selectedSections = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(hdnSelectedSections.Value);

            int universityId = (int)ViewState["UniversityId"];
            int majorId = (int)ViewState["MajorId"];

            foreach (var subjectCode in selectedSubjects)
            {
                bool isOffered = IsSubjectOffered(subjectCode, universityId, majorId, curriculumId);
                if (isOffered && !selectedSections.ContainsKey(subjectCode))
                {
                    errors.Add($"{GetSubjectName(subjectCode)} ({subjectCode}) requires a section selection");
                }
            }

            List<Section> validSections = new List<Section>();
            foreach (var kvp in selectedSections)
            {
                var section = GetSectionDetails(kvp.Key, kvp.Value);
                if (section == null)
                {
                    errors.Add($"{kvp.Key}-{kvp.Value}: Invalid section");
                    continue;
                }

                if (section.RegisteredStudents >= section.Capacity)
                {
                    errors.Add($"{section.SubjectEnglishName} Section {section.SectionNumber} is full");
                }
                else
                {
                    validSections.Add(section);
                }
            }

            for (int i = 0; i < validSections.Count; i++)
            {
                for (int j = i + 1; j < validSections.Count; j++)
                {
                    if (SectionsConflict(validSections[i], validSections[j]))
                    {
                        errors.Add($"Schedule conflict between {validSections[i].SubjectEnglishName} " +
                                   $"(Section {validSections[i].SectionNumber}) and " +
                                   $"{validSections[j].SubjectEnglishName} (Section {validSections[j].SectionNumber})");
                    }
                }
            }

            return errors;
        }

        private string GetSubjectName(string subjectCode)
        {
            CRUD crud = new CRUD();
            DataTable dt = crud.getDTPassSqlDic(
                "SELECT subjectEnglishName FROM subjects WHERE subjectCode = @code",
                new Dictionary<string, object> { { "@code", subjectCode } }
            );
            return dt.Rows.Count > 0 ? dt.Rows[0]["subjectEnglishName"].ToString() : subjectCode;
        }

        private Section GetSectionDetails(string subjectCode, string sectionNumber)
        {
            int universityId = (int)ViewState["UniversityId"];
            int majorId = (int)ViewState["MajorId"];

            CRUD crud = new CRUD();
            DataTable dt = crud.getDTPassSqlDic(
                        @"SELECT s.sectionId, s.instuctorArabicName, 
                         sub.subjectEnglishName, s.capacity, 
                         s.registeredStudents, sd.day, 
                         sd.startTime, sd.endTime, sd.location
                          FROM sections s
                          INNER JOIN subjects sub ON s.subjectCode = sub.subjectCode
                          INNER JOIN sectionDetails sd ON s.sectionId = sd.sectionId
                          WHERE s.subjectCode = @code 
                          AND s.sectionNumber = @num
                          AND sub.universityId = @universityId 
                          AND sub.majorId = @majorId",
                        new Dictionary<string, object> {
                            { "@code", subjectCode },
                            { "@num", sectionNumber },
                            { "@universityId", universityId },
                            { "@majorId", majorId }
                        });

            if (dt.Rows.Count == 0) return null;

            return new Section
            {
                SubjectCode = subjectCode,
                SectionNumber = Convert.ToInt32(sectionNumber),
                SubjectEnglishName = dt.Rows[0]["subjectEnglishName"].ToString(),
                Capacity = Convert.ToInt32(dt.Rows[0]["capacity"]),
                RegisteredStudents = Convert.ToInt32(dt.Rows[0]["registeredStudents"]),
                InstructorArabicName = dt.Rows[0]["instuctorArabicName"]?.ToString() ?? "TBA",
                Details = dt.AsEnumerable().Select(row => new SectionDetail
                {
                    Day = Convert.ToInt32(row["day"]),
                    StartTime = (TimeSpan)row["startTime"],
                    EndTime = (TimeSpan)row["endTime"],
                    Location = row["location"]?.ToString()?.Trim() ?? "TBA"
                }).ToList()
            };
        }

        private bool SectionsConflict(Section a, Section b)
        {
            foreach (var aDetail in a.Details)
            {
                foreach (var bDetail in b.Details)
                {
                    if (aDetail.Day == bDetail.Day &&
                        TimesOverlap(aDetail.StartTime, aDetail.EndTime, bDetail.StartTime, bDetail.EndTime))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TimesOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            return start1 < end2 && end1 > start2;
        }

        private bool ConfirmOrder()
        {
            try
            {
                Guid userId = GetUserId();
                int studentId = GetStudentId(userId);
                int universityId = (int)ViewState["UniversityId"];
                int majorId = (int)ViewState["MajorId"];

                var selectedSubjects = Session["SelectedSubjects"] as List<string> ?? new List<string>();
                List<string> waitlistSubjects = new List<string>();
                decimal totalHours = 0;

                foreach (string subjectCode in selectedSubjects)
                {
                    CRUD creditCrud = new CRUD();
                    DataTable dtCredit = creditCrud.getDTPassSqlDic(
                        @"SELECT creditHours FROM subjects 
                        WHERE subjectCode = @code 
                        AND universityId = @univId 
                        AND majorId = @majorId",
                        new Dictionary<string, object>
                        {
                            { "@code", subjectCode },
                            { "@univId", universityId },
                            { "@majorId", majorId }
                        });

                    if (dtCredit.Rows.Count > 0 && dtCredit.Rows[0]["creditHours"] != DBNull.Value)
                    {
                        totalHours += Convert.ToDecimal(dtCredit.Rows[0]["creditHours"]);
                    }
                }

                CRUD orderCrud = new CRUD();
                int orderId = Convert.ToInt32(orderCrud.InsertUpdateDeleteViaSqlDicRtnIdentity(
                    @"INSERT INTO orders (curriculumId, studentId, totalCreditHours, orderDate, status)
                    OUTPUT INSERTED.orderId
                    VALUES (@curriculumId, @studentId, @totalHours, GETDATE(), 'Approved')",
                    new Dictionary<string, object>
                    {
                        { "@curriculumId", curriculumId },
                        { "@studentId", studentId },
                        { "@totalHours", totalHours }
                    }));

                var selectedSections = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    hdnSelectedSections.Value);

                foreach (var kvp in selectedSections)
                {
                    string subjectCode = kvp.Key;
                    string sectionNumber = kvp.Value;

                    CRUD subjectCrud = new CRUD();
                    DataTable dtSubject = subjectCrud.getDTPassSqlDic(
                        @"SELECT subjectId FROM subjects 
                        WHERE subjectCode = @code 
                        AND universityId = @univId 
                        AND majorId = @majorId",
                        new Dictionary<string, object>
                        {
                            { "@code", subjectCode },
                            { "@univId", universityId },
                            { "@majorId", majorId }
                        });

                    if (dtSubject.Rows.Count == 0)
                        throw new Exception($"Invalid subject: {subjectCode}");

                    int subjectId = Convert.ToInt32(dtSubject.Rows[0]["subjectId"]);

                    CRUD sectionCrud = new CRUD();
                    string sectionSql = @"
                        SELECT s.sectionId 
                        FROM sections s
                        INNER JOIN subjects sub 
                        ON s.subjectCode = sub.subjectCode
                        WHERE s.subjectCode = @code 
                        AND s.sectionNumber = @num 
                        AND sub.universityId = @univId 
                        AND sub.majorId = @majorId";

                    DataTable dtSection = sectionCrud.getDTPassSqlDic(sectionSql,
                        new Dictionary<string, object>
                        {
                            { "@code", subjectCode },
                            { "@num", sectionNumber },
                            { "@univId", universityId },
                            { "@majorId", majorId }
                        });

                    if (dtSection.Rows.Count == 0)
                        throw new Exception($"Invalid section: {subjectCode}-{sectionNumber}");

                    int sectionId = Convert.ToInt32(dtSection.Rows[0]["sectionId"]);

                    CRUD detailCrud = new CRUD();
                    detailCrud.InsertUpdateDelete(
                        @"INSERT INTO orderDetails (orderId, subjectId, sectionId) 
                        VALUES (@orderId, @subjectId, @sectionId)",
                        new Dictionary<string, object>
                        {
                            { "@orderId", orderId },
                            { "@subjectId", subjectId },
                            { "@sectionId", sectionId }
                        });
                }

                string requestedSemester = curriculumId == 1 ? "Fall 2024" : "Spring 2025";
                foreach (string subjectCode in selectedSubjects)
                {
                    bool isOffered = IsSubjectOffered(subjectCode, universityId, majorId, curriculumId);
                    if (!isOffered)
                    {
                        CRUD subjectCrud = new CRUD();
                        DataTable dtSubject = subjectCrud.getDTPassSqlDic(
                            @"SELECT subjectId FROM subjects 
                                WHERE subjectCode = @code 
                                AND universityId = @univId 
                                AND majorId = @majorId",
                            new Dictionary<string, object>
                            {
                                { "@code", subjectCode },
                                { "@univId", universityId },
                                { "@majorId", majorId }
                            });

                        if (dtSubject.Rows.Count == 0)
                            continue;

                        int subjectId = Convert.ToInt32(dtSubject.Rows[0]["subjectId"]);

                        CRUD waitlistCrud = new CRUD();
                        waitlistCrud.InsertUpdateDelete(
                            @"INSERT INTO waitlist (studentId, subjectId, requestedSemester, priority, requestDate, status)
                                VALUES (@studentId, @subjectId, @requestedSemester, 1, GETDATE(), 'Pending')",
                            new Dictionary<string, object>
                            {
                                { "@studentId", studentId },
                                { "@subjectId", subjectId },
                                { "@requestedSemester", requestedSemester }
                            });
                    }
                }
                var studentDetails = GetStudentDetails(studentId);
                if (string.IsNullOrEmpty(studentDetails.Email))
                {
                    throw new Exception("Student email not found");
                }

                if (selectedSections.Count > 0)
                {
                    SendOrderConfirmationEmail(studentDetails.Email, studentDetails.Name, selectedSections);
                }

                foreach (string subjectCode in selectedSubjects)
                {
                    bool isOffered = IsSubjectOffered(subjectCode, universityId, majorId, curriculumId);
                    if (!isOffered)
                    {
                        waitlistSubjects.Add(subjectCode);
                    }
                }

                foreach (var subjectCode in waitlistSubjects)
                {
                    SendWaitlistEmail(studentDetails.Email, studentDetails.Name, subjectCode);
                }

                return true;
            }
            catch (Exception ex)
            {
                lblOutput2.Text = $"Order failed: {ex.Message}";
                lblOutput2.ForeColor = Color.Red;
                return false;
            }
        }

        private (string Email, string Name) GetStudentDetails(int studentId)
        {
            CRUD crud = new CRUD();
            string sql = @"SELECT email, studentEnglishFirstName, studentEnglishLastName 
                   FROM students 
                   WHERE studentId = @studentId";
            DataTable dt = crud.getDTPassSqlDic(sql, new Dictionary<string, object> { { "@studentId", studentId } });
            if (dt.Rows.Count > 0)
            {
                string email = dt.Rows[0]["email"].ToString();
                string firstName = dt.Rows[0]["studentEnglishFirstName"].ToString();
                string lastName = dt.Rows[0]["studentEnglishLastName"].ToString();
                return (email, $"{firstName} {lastName}");
            }
            return (null, null);
        }

        private void SendOrderConfirmationEmail(string studentEmail, string studentName, Dictionary<string, string> selectedSections)
        {
            try
            {
                if (string.IsNullOrEmpty(studentEmail))
                {
                    throw new ArgumentException("Student email cannot be null");
                }

                List<string> tableRows = new List<string>();
                foreach (var kvp in selectedSections)
                {
                    string subjectCode = kvp.Key;
                    string sectionNumber = kvp.Value;

                    Section section = GetSectionDetails(subjectCode, sectionNumber);
                    if (section != null)
                    {
                        string schedule = FormatSchedule(section.Details);
                        string instructor = section.InstructorArabicName?.Trim() ?? "TBA";

                        tableRows.Add($"{section.SubjectEnglishName} | Section {sectionNumber} | {schedule} | {instructor}");
                    }
                }

                string body = $@"
Dear {studentName},

Thank you for confirming your order. Below are the details of your registered subjects:

{string.Join("\r\n\r\n", tableRows)}


------------------
This is an automated notification. Please do not reply directly to this email.";

                mailMgr email = new mailMgr
                {
                    myTo = studentEmail,
                    mySubject = "Order Confirmation",
                    myBody = body,
                    myIsBodyHtml = false
                };

                email.sendEmailViaGmail();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Confirmation email failed: {ex.Message}");
            }
        }

        private string FormatSchedule(List<SectionDetail> details)
        {
            if (details == null || details.Count == 0) return "Schedule not available";

            return string.Join("\n", details.Select(d =>
            {
                int adjustedDay = d.Day - 1;
                DateTime start = DateTime.Today.Add(d.StartTime);
                DateTime end = DateTime.Today.Add(d.EndTime);

                return $"{GetDayName(adjustedDay)} | " +
                       $"{start.ToString("hh:mm tt")}-{end.ToString("hh:mm tt")} | " +
                       $"{d.Location}";
            }));
        }

        private string GetDayName(int day)
        {
            if (day >= 0 && day <= 6)
            {
                return ((DayOfWeek)day).ToString();
            }
            return "TBA";
        }

        private void SendWaitlistEmail(string studentEmail, string studentName, string subjectCode)
        {
            try
            {
                string subjectName = GetSubjectName(subjectCode);
                string body = $@"
Dear {studentName},

As {subjectName} is currently unoffered this semester, your request has been placed on the waitlist for this subject.
Your current status is Pending.


------------------
This is an automated notification. Please do not reply directly to this email.";

                mailMgr email = new mailMgr
                {
                    myTo = studentEmail,
                    mySubject = $"Waitlist Notification for {subjectName}",
                    myBody = body,
                    myIsBodyHtml = false
                };
                email.sendEmailViaGmail();
            }
            catch (Exception ex)
            {
                lblOutput2.Text = $"Waitlist email failed: {ex.Message}";
                lblOutput2.ForeColor = Color.Red;
            }

        }
    }
}