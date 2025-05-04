using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                helpBox.Style["display"] = "none";
                SetupMessageButtons();
            }
        }

        private void SetupMessageButtons()
        {
            if (!Request.IsAuthenticated)
            {
                btnMessageAdmin.Visible = true;
                return;
            }

            btnMessageAdmin.Visible = false;
            btnMessageDeptHead.Visible = false;
            btnMessageFaculty.Visible = false;
            btnMessageStudent.Visible = false;

            if (Roles.IsUserInRole("Admin"))
            {
                btnMessageDeptHead.Visible = true;
                btnMessageFaculty.Visible = true;
                btnMessageStudent.Visible = true;
            }
            else if (Roles.IsUserInRole("DepartmentHead"))
            {
                btnMessageAdmin.Visible = true;
                btnMessageFaculty.Visible = true;
                btnMessageStudent.Visible = true;
            }
            else if (Roles.IsUserInRole("Faculty"))
            {
                btnMessageAdmin.Visible = true;
                btnMessageDeptHead.Visible = true;
                btnMessageFaculty.Visible = true;
                btnMessageStudent.Visible = true;
            }
            else if (Roles.IsUserInRole("Student"))
            {
                btnMessageAdmin.Visible = true;
                btnMessageFaculty.Visible = true;
            }
        }

        protected void btnMessageRecipient_Click(object sender, EventArgs e)
        {
            var button = (LinkButton)sender;
            string recipientType = button.CommandArgument;

            pnlEmailForm.Visible = true;
            ddlRecipients.Visible = (recipientType != "Admin");

            if (ddlRecipients.Visible)
            {
                LoadRecipients(recipientType);
            }

            helpBox.Style["display"] = "flex";

            updHelp.Update();
            ScriptManager.RegisterStartupScript(this, GetType(), "KeepHelpOpen",
                $"document.getElementById('{helpBox.ClientID}').style.display = 'flex';", true);
        }

        private void LoadRecipients(string recipientType)
        {
            try
            {
                CRUD myCrud = new CRUD();
                string sql = "";
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                bool isAdmin = Roles.IsUserInRole("Admin");

                int universityId = 0;
                int majorId = 0;
                Guid userId = (Guid)Membership.GetUser().ProviderUserKey;

                if (!isAdmin)
                {
                    string contextSql = @"SELECT COALESCE(s.universityId, f.universityId, dh.universityId) AS universityId,
                                 COALESCE(s.majorId, f.majorId, dh.majorId) AS majorId
                                 FROM aspnet_Users u
                                 LEFT JOIN students s ON u.UserId = s.UserId
                                 LEFT JOIN faculty f ON u.UserId = f.UserId
                                 LEFT JOIN departmentHead dh ON u.UserId = dh.UserId
                                 WHERE u.UserId = @userId";

                    DataTable contextDt = myCrud.getDTPassSqlDic(contextSql,
                        new Dictionary<string, object> { { "@userId", userId } });

                    if (contextDt.Rows.Count > 0 && contextDt.Rows[0]["universityId"] != DBNull.Value)
                    {
                        universityId = Convert.ToInt32(contextDt.Rows[0]["universityId"]);
                        majorId = Convert.ToInt32(contextDt.Rows[0]["majorId"]);
                    }
                }

                switch (recipientType)
                {
                    case "Faculty":
                        sql = isAdmin ?
                            @"SELECT m.Email, 
                     f.facultyEnglishFirstName + ' ' + f.facultyEnglishLastName + ' (' + m.Email + ')' AS DisplayName
                     FROM aspnet_Membership m
                     INNER JOIN faculty f ON m.UserId = f.UserId"
                            :
                            @"SELECT m.Email, 
                     f.facultyEnglishFirstName + ' ' + f.facultyEnglishLastName + ' (' + m.Email + ')' AS DisplayName
                     FROM aspnet_Membership m
                     INNER JOIN faculty f ON m.UserId = f.UserId
                     WHERE f.universityId = @univId AND f.majorId = @majorId";
                        break;

                    case "Student":
                        sql = isAdmin ?
                            @"SELECT m.Email, 
                     s.studentEnglishFirstName + ' ' + s.studentEnglishLastName + ' (' + m.Email + ')' AS DisplayName
                     FROM aspnet_Membership m
                     INNER JOIN students s ON m.UserId = s.UserId"
                            :
                            @"SELECT m.Email, 
                     s.studentEnglishFirstName + ' ' + s.studentEnglishLastName + ' (' + m.Email + ')' AS DisplayName
                     FROM aspnet_Membership m
                     INNER JOIN students s ON m.UserId = s.UserId
                     WHERE s.universityId = @univId AND s.majorId = @majorId";
                        break;

                    case "DeptHead":
                        sql = isAdmin ?
                            @"SELECT m.Email, 
                     dh.departmentHeadEnglishFirstName + ' ' + dh.departmentHeadEnglishLastName + ' (' + m.Email + ')' AS DisplayName
                     FROM aspnet_Membership m
                     INNER JOIN departmentHead dh ON m.UserId = dh.UserId"
                            :
                            @"SELECT m.Email, 
                     dh.departmentHeadEnglishFirstName + ' ' + dh.departmentHeadEnglishLastName + ' (' + m.Email + ')' AS DisplayName
                     FROM aspnet_Membership m
                     INNER JOIN departmentHead dh ON m.UserId = dh.UserId
                     WHERE dh.universityId = @univId AND dh.majorId = @majorId";
                        break;

                    case "Admin":
                        sql = @"SELECT m.Email, 
                       COALESCE(
                           f.facultyEnglishFirstName + ' ' + f.facultyEnglishLastName,
                           s.studentEnglishFirstName + ' ' + s.studentEnglishLastName,
                           dh.departmentHeadEnglishFirstName + ' ' + dh.departmentHeadEnglishLastName,
                           m.Email
                       ) + ' (' + m.Email + ')' AS DisplayName
                       FROM aspnet_Membership m
                       LEFT JOIN faculty f ON m.UserId = f.UserId
                       LEFT JOIN students s ON m.UserId = s.UserId
                       LEFT JOIN departmentHead dh ON m.UserId = dh.UserId";
                        break;
                }

                if (!isAdmin && recipientType != "Admin")
                {
                    parameters.Add("@univId", universityId);
                    parameters.Add("@majorId", majorId);
                }

                DataTable dt = myCrud.getDTPassSqlDic(sql, parameters);

                ddlRecipients.DataSource = dt;
                ddlRecipients.DataTextField = "DisplayName";
                ddlRecipients.DataValueField = "Email";
                ddlRecipients.DataBind();

                if (ddlRecipients.Items.Count == 0)
                {
                    ddlRecipients.Items.Add(new ListItem("No recipients found", ""));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recipients: {ex.Message}");
                ddlRecipients.Items.Add(new ListItem("Error loading recipients", ""));
            }
        }

        protected void btnSendEmail_Click(object sender, EventArgs e)
        {
            try
            {
                string recipientEmail = ddlRecipients.SelectedValue;
                string title = txtEmailTitle.Text.Trim();
                string body = txtEmailBody.Text.Trim();

                MembershipUser currentUser = Membership.GetUser();
                if (currentUser == null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('You must be logged in to send messages.');", true);
                    return;
                }

                string userEmail = currentUser.Email;
                Guid userId = (Guid)currentUser.ProviderUserKey;
                string fullName = GetUserFullName(userId);

                string formattedBody = $@"
Message sent from {fullName} ({userEmail})

------------------
{body}
";

                mailMgr emailSender = new mailMgr
                {
                    myTo = recipientEmail,
                    mySubject = title,
                    myBody = formattedBody,
                    myIsBodyHtml = false
                };

                string result = emailSender.sendEmailViaGmail();

                ScriptManager.RegisterStartupScript(this, GetType(), "EmailSuccess",
                    $"alert('Email sent successfully to {recipientEmail}');", true);

                txtEmailTitle.Text = "";
                txtEmailBody.Text = "";
                pnlEmailForm.Visible = false;
                updHelp.Update();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "EmailError",
                    $"alert('Error sending email: {ex.Message}');", true);
            }
        }
        private string GetUserFullName(Guid userId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"
        SELECT COALESCE(
            s.studentEnglishFirstName + ' ' + s.studentEnglishLastName,
            f.facultyEnglishFirstName + ' ' + f.facultyEnglishLastName,
            dh.departmentHeadEnglishFirstName + ' ' + dh.departmentHeadEnglishLastName,
            'User'
        ) AS FullName
        FROM aspnet_Users u
        LEFT JOIN students s ON u.UserId = s.UserId
        LEFT JOIN faculty f ON u.UserId = f.UserId
        LEFT JOIN departmentHead dh ON u.UserId = dh.UserId
        WHERE u.UserId = @userId";

            DataTable dt = myCrud.getDTPassSqlDic(sql, new Dictionary<string, object> { { "@userId", userId } });
            return dt.Rows.Count > 0 ? dt.Rows[0]["FullName"].ToString() : "User";
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}