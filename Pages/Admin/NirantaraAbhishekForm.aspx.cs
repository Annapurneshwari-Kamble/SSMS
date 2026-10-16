using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Web.UI;

namespace SSMS.Pages.Admin
{
    public partial class NirantaraAbhishekForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["SerialNo"] != null)
            {
                string serialNo = Request.QueryString["SerialNo"];
                LoadAbhishekData(serialNo);
            }
        }

        private void LoadAbhishekData(string serialNo)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "SELECT EntryDate FROM SevaMemberDetails WHERE SerialNo=@SerialNo";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SerialNo", serialNo);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    DateTime abhishekDate = reader["EntryDate"] != DBNull.Value
                        ? Convert.ToDateTime(reader["EntryDate"])
                        : DateTime.Now;

                    string formattedDate = abhishekDate.ToString("dd-MM-yyyy");
                    kannadaDate.InnerText = formattedDate;
                    englishDate.InnerText = formattedDate;
                }
            }
        }

        // Generate PDF
        protected void btnPdf_Click(object sender, EventArgs e)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=NirantaraAbhishekForm.pdf");
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            pnlCertificate.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());

            // Force exact A4 size (210mm × 297mm)
            using (iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4))
            {
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
            }

            Response.End();
        }



        // Send Email
        protected void btnEmail_Click(object sender, EventArgs e)
        {
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            pnlCertificate.RenderControl(hw);
            string body = sw.ToString();

            MailMessage mm = new MailMessage("your-email@example.com", "recipient@example.com");
            mm.Subject = "Nirantara Abhishek Form";
            mm.Body = body;
            mm.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.yourmailserver.com"; // change as per your server
            smtp.Port = 587; // usually 587 for TLS
            smtp.Credentials = new System.Net.NetworkCredential("your-email@example.com", "your-password");
            smtp.EnableSsl = true;
            smtp.Send(mm);
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required for rendering server controls in export
        }
    }
}
