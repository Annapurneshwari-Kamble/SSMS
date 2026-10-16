using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Web.UI;

namespace SSMS.Pages.Admin
{
    public partial class NirantaraAnnaprasadForm : System.Web.UI.Page
    {
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack && Request.QueryString["SerialNo"] != null)
        //    {
        //        string serialNo = Request.QueryString["SerialNo"];
        //        LoadAnnaprasadData(serialNo);
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string serialNo = Request.QueryString["SerialNo"];
                if (string.IsNullOrEmpty(serialNo))
                {
                    kannadaDate.InnerText = "⚠ SerialNo missing";
                    englishDate.InnerText = "⚠ SerialNo missing";
                }
                else
                {
                    LoadAnnaprasadData(serialNo);
                }
            }
        }


        //private void LoadAnnaprasadData(string serialNo)
        //{
        //    string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        string query = "SELECT EntryDate FROM SevaMemberDetails WHERE SerialNo=@SerialNo";
        //        SqlCommand cmd = new SqlCommand(query, con);
        //        cmd.Parameters.AddWithValue("@SerialNo", serialNo);

        //        con.Open();
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        if (reader.Read())
        //        {
        //            DateTime annaprasadDate = reader["EntryDate"] != DBNull.Value
        //                ? Convert.ToDateTime(reader["EntryDate"])
        //                : DateTime.Now;

        //            string formattedDate = annaprasadDate.ToString("dd-MM-yyyy");

        //            // Assign to HTML spans in your ASPX
        //            kannadaDate.InnerText = formattedDate;
        //            englishDate.InnerText = formattedDate;
        //        }
        //    }
        //}


        //new
        private void LoadAnnaprasadData(string serialNo)
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
                    object entryDateObj = reader["EntryDate"];
                    DateTime annaprasadDate = entryDateObj != DBNull.Value
                        ? Convert.ToDateTime(entryDateObj)
                        : DateTime.Now;

                    string formattedDate = annaprasadDate.ToString("dd-MM-yyyy");

                    kannadaDate.InnerHtml = " " + formattedDate;  // debugging
                    englishDate.InnerHtml = " " + formattedDate;
                }
                else
                {
                    kannadaDate.InnerHtml = "⚠ No data for " + serialNo;
                    englishDate.InnerHtml = "⚠ No data for " + serialNo;
                }
            }
        }



        // PDF + Email code stays same...
        public override void VerifyRenderingInServerForm(Control control) { }
    }
}
