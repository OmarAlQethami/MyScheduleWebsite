using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
//using iTextSharp.text;
//using iTextSharp.text.pdf;



namespace MyScheduleWebsite

{

    public partial class OrderSuccessfulPage : System.Web.UI.Page

    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Default.aspx");
            }

            if (!User.IsInRole("student"))
            {
                Response.Redirect("~/Default.aspx");
            }

            Guid userId = GetUserId();
            int studentId = GetStudentId(userId);

            if (!HasExistingOrder(studentId))
            {
                Response.Redirect("~/StudentPage.aspx");
            }

            if (!IsPostBack)
            {
                LoadStudentData(userId);
                LoadScheduleData(studentId);
                LoadWishlistData(studentId);
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

        private int GetStudentId(Guid userId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT studentId FROM students WHERE UserId = @userId";
            DataTable dt = myCrud.getDTPassSqlDic(sql, new Dictionary<string, object> { { "@userId", userId } });
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["studentId"]) : 0;
        }

        private bool HasExistingOrder(int studentId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT COUNT(*) FROM orders WHERE studentId = @studentId";
            DataTable dt = myCrud.getDTPassSqlDic(sql, new Dictionary<string, object> { { "@studentId", studentId } });
            return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        private void LoadStudentData(Guid userId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT s.studentEnglishFirstName + ' ' + s.studentEnglishLastName AS FullName, 
                          s.studentUniId, u.universityEnglishName, m.majorEnglishName
                          FROM students s
                          INNER JOIN universities u ON s.universityId = u.universityId
                          INNER JOIN majors m ON s.majorId = m.majorId
                          WHERE s.UserId = @userId";

            DataTable dt = myCrud.getDTPassSqlDic(sql, new Dictionary<string, object> { { "@userId", userId } });

            if (dt.Rows.Count > 0)
            {
                lblStudentName.Text = dt.Rows[0]["FullName"].ToString();
                lblUniID.Text = dt.Rows[0]["studentUniId"].ToString();
                lblMajor.Text = dt.Rows[0]["majorEnglishName"].ToString();
            }
        }

        private void LoadScheduleData(int studentId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT s.subjectCode AS SubjectCode, 
                  s.subjectEnglishName AS SubjectName, 
                  sec.sectionNumber AS SectionNumber,
                  s.creditHours AS Credits, 
                  d.day AS Day,
                  d.startTime AS StartTime,
                  d.endTime AS EndTime,
                  d.location AS Location,
                  sec.instuctorArabicName AS Instructor
                  FROM orders o
                  INNER JOIN orderDetails od ON o.orderId = od.orderId
                  INNER JOIN subjects s ON od.subjectId = s.subjectId
                  INNER JOIN sections sec ON od.sectionId = sec.sectionId
                  INNER JOIN sectionDetails d ON sec.sectionId = d.sectionId
                  WHERE o.studentId = @studentId
                  ORDER BY d.day, d.startTime";

            DataTable dt = myCrud.getDTPassSqlDic(sql, new Dictionary<string, object> { { "@studentId", studentId } });

            if (dt.Rows.Count > 0)
            {
                gvSchedule.DataSource = dt;
                gvSchedule.DataBind();

                int totalCredits = Convert.ToInt32(dt.Compute("SUM(Credits)", ""));
                lblTotalCredits.Text = totalCredits.ToString();
            }
        }
        private void LoadWishlistData(int studentId)
        {
            CRUD myCrud = new CRUD();
            string sql = @"SELECT s.subjectCode, s.subjectEnglishName AS SubjectName, 
                  w.status AS Status, 
                  w.statusChangedBy AS StatusChangedBy,
                  w.statusChangedDate AS StatusChangedDate,
                  w.requestDate AS RequestDate
                  FROM waitlist w
                  INNER JOIN subjects s ON w.subjectId = s.subjectId
                  WHERE w.studentId = @studentId
                  ORDER BY w.requestDate DESC";

            DataTable dt = myCrud.getDTPassSqlDic(sql, new Dictionary<string, object> { { "@studentId", studentId } });

            if (dt.Rows.Count > 0)
            {
                pnlWishlist.Visible = true;
                gvWishlist.DataSource = dt;
                gvWishlist.DataBind();
            }
        }

        protected void gvSchedule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblDay = (Label)e.Row.FindControl("lblDay");
                if (lblDay != null)
                {
                    int dayNumber = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Day"));
                    lblDay.Text = ConvertDayNumberToName(dayNumber);
                }

                Label lblStartTime = (Label)e.Row.FindControl("lblStartTime");
                Label lblEndTime = (Label)e.Row.FindControl("lblEndTime");

                if (lblStartTime != null && lblEndTime != null)
                {
                    TimeSpan start = (TimeSpan)DataBinder.Eval(e.Row.DataItem, "StartTime");
                    TimeSpan end = (TimeSpan)DataBinder.Eval(e.Row.DataItem, "EndTime");

                    lblStartTime.Text = ConvertToAmPm(start);
                    lblEndTime.Text = ConvertToAmPm(end);
                }
            }
        }

        private string ConvertDayNumberToName(int dayNumber)
        {
            switch (dayNumber)
            {
                case 1:
                    return "Sunday";
                case 2:
                    return "Monday";
                case 3:
                    return "Tuesday";
                case 4:
                    return "Wednesday";
                case 5:
                    return "Thursday";
                default:
                    return "Unknown Day";
            }
        }

        private string ConvertToAmPm(TimeSpan time)
        {
            DateTime dateTime = DateTime.Today.Add(time);
            return dateTime.ToString("hh:mm tt");
        }

        protected void gvSchedule_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = (DataTable)ViewState["ScheduleData"];
            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gvSchedule.DataSource = dt;
                gvSchedule.DataBind();
            }
        }

        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null && sortExpression == column)
            {
                string lastDirection = ViewState["SortDirection"] as string;
                if (lastDirection != null && lastDirection == "ASC")
                {
                    sortDirection = "DESC";
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }



        //protected void btnExport_Click(object sender, EventArgs e)

        //{

        //    try
        //    {

        //        Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 0f);
        //        MemoryStream memoryStream = new MemoryStream();


        //        PdfWriter
        //        writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
        //        writer.PdfVersion = PdfWriter.VERSION_1_7;


        //        pdfDoc.Open();


        //        Font titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD, new BaseColor(53, 149, 205));
        //        Paragraph title = new Paragraph("Your Schedule Details", titleFont);
        //        title.Alignment = Element.ALIGN_CENTER;
        //        pdfDoc.Add(title);
        //        pdfDoc.Add(Chunk.Newline);


        //        Font infoFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
        //        pdfDoc.Add(new Paragraph($"Student Name: {lblStudentName.Text}", infoFont));
        //        pdfDoc.Add(new Paragraph($"University ID: {lblUniID.Text}", infoFont));
        //        pdfDoc.Add(new Paragraph($"Major: {lblMajor.Text}", infoFont));
        //        pdfDoc.Add(new Paragraph($"Total Credits: {lblTotalCredits.Text}", infoFont));
        //        pdfDoc.Add(Chunk.Newline);


        //        PdfPTable pdfTable = new PdfPTable(gvSchedule.Columns.Count);
        //        pdfTable.WidthPercentage = 100;
        //        pdfTable.SpacingBefore = 10f;
        //        pdfTable.SpacingAfter = 10f;

        //        Font headerFont = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.White);
        //        foreach (DataControlField column in gvSchedule.Columns)
        //        {
        //            PdfPCell headerCell = new PdfPCell(new Phrase(column.HeaderText, headerFont));
        //            headerCell.BackgroundColor = new BaseColor(53, 149, 205);
        //            headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            pdfTable.AddCell(headerCell);
        //        }

        //        Font cellFont = FontFactory.GetFont("Arial", 9);
        //        foreach (GridViewRow row in gvSchedule.Rows)
        //        {
        //            if (row.RowType == DataControlRowType.DataRow)
        //            {
        //                foreach (TableCell cell in row.Cells)
        //                {
        //                    PdfPCell dataCell = new PdfPCell(new Phrase(cell.Text, cellFont));
        //                    dataCell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    pdfTable.AddCell(dataCell);
        //                }
        //            }
        //        }

        //        pdfDoc.Add(pdfTable);
        //        pdfDoc.Close();


        //        Response.Clear();
        //        Response.ContentType = "application/pdf";
        //        Response.AddHeader("content-disposition", "attachment;filename=StudentSchedule.pdf");
        //        Response.BinaryWrite(memoryStream.ToArray());
        //        Response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        ClientScript.RegisterStartupScript(this.GetType(), "alert",
        //            $"alert('Error exporting PDF: {ex.Message.Replace("'", "\\'")}');", true);
        //    }


        //}



        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Guid userId = GetUserId();
            int studentId = GetStudentId(userId);

            CRUD myCrud = new CRUD();
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@studentId", studentId }
                };

            string deleteDetailsSql = @"
                DELETE FROM orderDetails 
                WHERE orderId IN 
                (SELECT orderId FROM orders WHERE studentId = @studentId)";
            myCrud.InsertUpdateDelete(deleteDetailsSql, parameters);

            string deleteOrderSql = "DELETE FROM orders WHERE studentId = @studentId";
            myCrud.InsertUpdateDelete(deleteOrderSql, parameters);

            string deleteWaitlistSql = "DELETE FROM waitlist WHERE studentId = @studentId";
            myCrud.InsertUpdateDelete(deleteWaitlistSql, parameters);

            Response.Redirect("StudentPage.aspx");
        }
    }
}