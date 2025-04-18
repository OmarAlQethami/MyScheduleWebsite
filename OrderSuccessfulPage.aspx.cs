using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;



namespace MyScheduleWebsite

{

    public partial class OrderSuccessfulPage : System.Web.UI.Page

    {

        protected void Page_Load(object sender, EventArgs e)

        {

            if (!IsPostBack)

            {

                LoadStudentData();

                LoadScheduleData();

            }

        }



        private void LoadStudentData()

        {

            lblStudentName.Text = "Ahmed Al-Otaibi";

            lblUniID.Text = "123456789";

            lblMajor.Text = "Computer Science";

            lblTotalCredits.Text = "11";

        }



        private void LoadScheduleData()

        {

            DataTable dt = new DataTable();

            dt.Columns.Add("CourseCode");

            dt.Columns.Add("CourseName");

            dt.Columns.Add("SectionNumber");

            dt.Columns.Add("Credits");

            dt.Columns.Add("Schedule");

            dt.Columns.Add("Instructor");



            dt.Rows.Add("501472-3", "Computer Graphics", "244", "3", "Sun 8:00-11:00", "Dr.Al-Walid Al-Harbi");

            dt.Rows.Add("501427-3", "Programming Paradigms", "298", "3", "Mon 10:00-1:00", "Dr.Bader Al-Aoufi");

            dt.Rows.Add("501461-3", "Internet Technologies", "300", "3", "Tue 1:00-4:00", "Dr.Ibrahim Al-thomali");

            dt.Rows.Add("500321-2", "Professional Ethics", "286", "2", "Sun 11:00-1:00", "Dr.Sami Al-Suwat");



            gvSchedule.DataSource = dt;

            gvSchedule.DataBind();

        }



        protected void gvSchedule_RowDataBound(object sender, GridViewRowEventArgs e)

        {

            if (e.Row.RowType == DataControlRowType.DataRow)

            {

                TableCell sectionCell = e.Row.Cells[2];

                sectionCell.ToolTip = "Section Number: " + sectionCell.Text;

            }

        }



        protected void gvSchedule_Sorting(object sender, GridViewSortEventArgs e)

        {

            DataTable dt = GetScheduleData();

            dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);

            gvSchedule.DataSource = dt;

            gvSchedule.DataBind();

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



        private DataTable GetScheduleData()

        {

            DataTable dt = new DataTable();

            dt.Columns.Add("CourseCode");

            dt.Columns.Add("CourseName");

            dt.Columns.Add("SectionNumber");

            dt.Columns.Add("Credits");

            dt.Columns.Add("Schedule");

            dt.Columns.Add("Instructor");



            dt.Rows.Add("501472-3", "Computer Graphics", "244", "3", "Sun 8:00-11:00", "Dr.Al-Walid Al-Harbi");

            dt.Rows.Add("501427-3", "Programming Paradigms", "298", "3", "Mon 10:00-1:00", "Dr.Bader Al-Aoufi");

            dt.Rows.Add("501461-3", "Internet Technologies", "300", "3", "Tue 1:00-4:00", "Dr.Ibrahim Al-thomali");

            dt.Rows.Add("500321-2", "Professional Ethics", "286", "2", "Sun 11:00-1:00", "Dr.Sami Al-Suwat");



            return dt;

        }



        protected void btnExport_Click(object sender, EventArgs e)

        {
            try
            {
                
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 0f);
                MemoryStream memoryStream = new MemoryStream();

               
                PdfWriter
                writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                writer.PdfVersion = PdfWriter.VERSION_1_7;

 
                pdfDoc.Open();

               
                Font titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD, new BaseColor(53, 149, 205));
                Paragraph title = new Paragraph("Your Schedule Details", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(title);
                pdfDoc.Add(Chunk.Newline);

                
                Font infoFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                pdfDoc.Add(new Paragraph($"Student Name: {lblStudentName.Text}", infoFont));
                pdfDoc.Add(new Paragraph($"University ID: {lblUniID.Text}", infoFont));
                pdfDoc.Add(new Paragraph($"Major: {lblMajor.Text}", infoFont));
                pdfDoc.Add(new Paragraph($"Total Credits: {lblTotalCredits.Text}", infoFont));
                pdfDoc.Add(Chunk.Newline);

                
                PdfPTable pdfTable = new PdfPTable(gvSchedule.Columns.Count);
                pdfTable.WidthPercentage = 100;
                pdfTable.SpacingBefore = 10f;
                pdfTable.SpacingAfter = 10f;

                Font headerFont = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.White);
                foreach (DataControlField column in gvSchedule.Columns)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(column.HeaderText, headerFont));
                    headerCell.BackgroundColor = new BaseColor(53, 149, 205);
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfTable.AddCell(headerCell);
                }

                Font cellFont = FontFactory.GetFont("Arial", 9);
                foreach (GridViewRow row in gvSchedule.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        foreach (TableCell cell in row.Cells)
                        {
                            PdfPCell dataCell = new PdfPCell(new Phrase(cell.Text, cellFont));
                            dataCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(dataCell);
                        }
                    }
                }

                pdfDoc.Add(pdfTable);
                pdfDoc.Close();

            
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=StudentSchedule.pdf");
                Response.BinaryWrite(memoryStream.ToArray());
                Response.End();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Error exporting PDF: {ex.Message.Replace("'", "\\'")}');", true);
            }

        }


        protected void btnCancel_Click(object sender, EventArgs e)

        {

            try

            {

                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                   "alert('Schedule cancelled successfully!');", true);



                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect",

                    "setTimeout(function(){ window.location.href = 'StudentDashboard.aspx'; }, 2000);", true);

            }

            catch (Exception ex)

            {

                ClientScript.RegisterStartupScript(this.GetType(), "alert",

                    $"alert('Error cancelling schedule: {ex.Message}');", true);

            }

        }

    }

}