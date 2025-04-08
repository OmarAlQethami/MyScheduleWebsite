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

            var missingPrerequisites = subject.Prerequisites
                .Where(p => !takenSubjectCodes.Contains(p))
                .ToList();

            if (missingPrerequisites.Any())
            {
                return "unavailable";
            }

            // I need later to alter this to have "unoffered" based on the curriculum

            return "available";
        }

        private void RegisterClientScripts(BindSubjectsData data, SubjectsRenderResult renderResult)
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

            var allSubjects = ProcessSubjects(universityId, majorId);

            var selectedSubjects = allSubjects
                .Where(s => selectedCodes.Contains(s.Code))
                .ToList();

            string subjectsHtml = "";
            foreach (var subject in selectedSubjects)
            {
                subjectsHtml += $@"
                    <div class='subject subject-in-sections' id='{subject.Code}' onclick='SubjectInSectionsClicked(this)'>
                        <span>{subject.EnglishName}</span>
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
                       WHERE s.curriculumId IN (1, 2)
                       AND s.subjectCode IN ({0})";

            string[] parameters = subjectCodes.Select((_, i) => $"@p{i}").ToArray();
            sql = string.Format(sql, string.Join(",", parameters));

            Dictionary<string, object> para = new Dictionary<string, object>();
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

            foreach (var subjectCode in selectedSubjects)
            {
                if (!selectedSections.ContainsKey(subjectCode))
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
                @"SELECT s.*, sub.subjectEnglishName, sd.day, sd.startTime, sd.endTime 
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
                    }
            );

            if (dt.Rows.Count == 0) return null;

            return new Section
            {
                SubjectCode = subjectCode,
                SectionNumber = Convert.ToInt32(sectionNumber),
                SubjectEnglishName = dt.Rows[0]["subjectEnglishName"].ToString(),
                Capacity = Convert.ToInt32(dt.Rows[0]["capacity"]),
                RegisteredStudents = Convert.ToInt32(dt.Rows[0]["registeredStudents"]),
                Details = dt.AsEnumerable().Select(row => new SectionDetail
                {
                    Day = Convert.ToInt32(row["day"]),
                    StartTime = (TimeSpan)row["startTime"],
                    EndTime = (TimeSpan)row["endTime"]
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
            CRUD myCrud = new CRUD();
            try
            {
                Guid userId = GetUserId();
                int studentId = GetStudentId(userId);
                int universityId = (int)ViewState["UniversityId"];
                int majorId = (int)ViewState["MajorId"];

                int curriculumId = 1;

                var selectedSubjects = Session["SelectedSubjects"] as List<string> ?? new List<string>();
                decimal totalHours = 0;

                foreach (string subjectCode in selectedSubjects)
                {
                    CRUD creditCrud = new CRUD();
                    try
                    {
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
                    finally
                    {
                        creditCrud.con?.Dispose();
                    }
                }

                int orderId;
                CRUD orderCrud = new CRUD();
                try
                {
                    orderId = Convert.ToInt32(orderCrud.InsertUpdateDeleteViaSqlDicRtnIdentity(
                        @"INSERT INTO orders (curriculumId, studentId, totalCreditHours, orderDate, status)
                        OUTPUT INSERTED.orderId
                        VALUES (@curriculumId, @studentId, @totalHours, GETDATE(), 'Approved')",
                        new Dictionary<string, object>
                        {
                            { "@curriculumId", curriculumId },
                            { "@studentId", studentId },
                            { "@totalHours", totalHours }
                        }));
                }
                finally
                {
                    orderCrud.con?.Dispose();
                }

                var selectedSections = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    hdnSelectedSections.Value);

                foreach (var kvp in selectedSections)
                {
                    string subjectCode = kvp.Key;
                    string sectionNumber = kvp.Value;

                    int subjectId;
                    CRUD subjectCrud = new CRUD();
                    try
                    {
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

                        subjectId = Convert.ToInt32(dtSubject.Rows[0]["subjectId"]);
                    }
                    finally
                    {
                        subjectCrud.con?.Dispose();
                    }

                    int sectionId;
                    CRUD sectionCrud = new CRUD();
                    try
                    {
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

                        sectionId = Convert.ToInt32(dtSection.Rows[0]["sectionId"]);
                    }
                    finally
                    {
                        sectionCrud.con?.Dispose();
                    }

                    CRUD detailCrud = new CRUD();
                    try
                    {
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
                    finally
                    {
                        detailCrud.con?.Dispose();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                lblOutput2.Text = $"Order failed: {ex.Message}";
                lblOutput2.ForeColor = Color.Red;
                return false;
            }
            finally
            {
                myCrud.con?.Dispose();
            }
        }
    }
}