using Kannada.AsciiUnicode.Converters;
using Kannada; // Add this using statement
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace SSMS.Pages.Admin
{
    public partial class ConvertKannadaData : Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private KannadaConverter _converter;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
            _converter = KannadaConverter.Instance;

            if (!IsPostBack)
            {
                // Load initial stats
                LoadStatistics();
            }
        }

        private void LoadStatistics()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Total records
                string totalQuery = "SELECT COUNT(*) FROM SevaMemberDetails WHERE Language LIKE 'Kannada%'";
                SqlCommand cmdTotal = new SqlCommand(totalQuery, con);
                lblTotalRecords.Text = cmdTotal.ExecuteScalar().ToString();

                // Records to convert (not yet Unicode)
                string convertQuery = "SELECT COUNT(*) FROM SevaMemberDetails WHERE Language LIKE 'Kannada%' AND NOT (DevoteeName LIKE N'%[ಅ-ಔ]%')";
                SqlCommand cmdConvert = new SqlCommand(convertQuery, con);
                lblRecordsToConvert.Text = cmdConvert.ExecuteScalar().ToString();

                // Already converted (Unicode)
                string convertedQuery = "SELECT COUNT(*) FROM SevaMemberDetails WHERE Language LIKE 'Kannada%' AND DevoteeName LIKE N'%[ಅ-ಔ]%'";
                SqlCommand cmdConverted = new SqlCommand(convertedQuery, con);
                lblConverted.Text = cmdConverted.ExecuteScalar().ToString();

                lblMessage.Text = "";
            }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            DataTable dtPreview = GetPreviewData();
            rptPreview.DataSource = dtPreview;
            rptPreview.DataBind();

            if (dtPreview.Rows.Count == 0)
            {
                lblNoRecords.Visible = true;
                rptPreview.Visible = false;
            }
            else
            {
                lblNoRecords.Visible = false;
                rptPreview.Visible = true;
            }
        }

        private DataTable GetPreviewData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SerialNo", typeof(int));
            dt.Columns.Add("OldDevoteeName", typeof(string));
            dt.Columns.Add("NewDevoteeName", typeof(string));

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                // Get records that are not yet Unicode
                string query = @"
                    SELECT TOP 10 SerialNo, DevoteeName, EventName, District, City, Address
                    FROM SevaMemberDetails 
                    WHERE Language LIKE 'Kannada%' 
                    AND NOT (DevoteeName LIKE N'%[ಅ-ಔ]%')
                    ORDER BY SerialNo";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    DataRow row = dt.NewRow();
                    row["SerialNo"] = rdr["SerialNo"];

                    string oldName = rdr["DevoteeName"].ToString();
                    row["OldDevoteeName"] = oldName;

                    // Convert using KannadaAsciiUnicodeSDK
                    string convertedName = _converter.ConvertAsciiToUnicode(oldName);
                    row["NewDevoteeName"] = convertedName;

                    dt.Rows.Add(row);
                }
                rdr.Close();
            }

            return dt;
        }

        protected void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                int convertedCount = 0;
                int totalRecords = 0;

                // First, get all records that need conversion
                DataTable dtRecords = GetRecordsToConvert();

                if (dtRecords.Rows.Count == 0)
                {
                    lblMessage.Text = "ℹ️ No records found to convert.";
                    lblMessage.CssClass = "text-info fw-bold";
                    return;
                }

                totalRecords = dtRecords.Rows.Count;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        foreach (DataRow row in dtRecords.Rows)
                        {
                            int serialNo = Convert.ToInt32(row["SerialNo"]);
                            string oldDevoteeName = row["DevoteeName"].ToString();
                            string oldEventName = row["EventName"].ToString();
                            string oldDistrict = row["District"].ToString();
                            string oldCity = row["City"].ToString();
                            string oldAddress = row["Address"].ToString();

                            // Convert each field
                            string newDevoteeName = _converter.ConvertAsciiToUnicode(oldDevoteeName);
                            string newEventName = _converter.ConvertAsciiToUnicode(oldEventName);
                            string newDistrict = _converter.ConvertAsciiToUnicode(oldDistrict);
                            string newCity = _converter.ConvertAsciiToUnicode(oldCity);
                            string newAddress = _converter.ConvertAsciiToUnicode(oldAddress);

                            // Update the record
                            string updateQuery = @"
                                UPDATE SevaMemberDetails 
                                SET DevoteeName = @DevoteeName,
                                    EventName = @EventName,
                                    District = @District,
                                    City = @City,
                                    Address = @Address
                                WHERE SerialNo = @SerialNo";

                            SqlCommand cmd = new SqlCommand(updateQuery, con, transaction);
                            cmd.Parameters.AddWithValue("@DevoteeName", newDevoteeName);
                            cmd.Parameters.AddWithValue("@EventName", newEventName);
                            cmd.Parameters.AddWithValue("@District", newDistrict);
                            cmd.Parameters.AddWithValue("@City", newCity);
                            cmd.Parameters.AddWithValue("@Address", newAddress);
                            cmd.Parameters.AddWithValue("@SerialNo", serialNo);

                            cmd.ExecuteNonQuery();
                            convertedCount++;

                            // Update progress (for UI feedback)
                            if (convertedCount % 10 == 0 || convertedCount == totalRecords)
                            {
                                // You can add progress update logic here if needed
                            }
                        }

                        transaction.Commit();

                        lblMessage.Text = $"✅ Successfully converted {convertedCount} records to Unicode Kannada!";
                        lblMessage.CssClass = "text-success fw-bold";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        lblMessage.Text = $"❌ Error during conversion: {ex.Message}";
                        lblMessage.CssClass = "text-danger fw-bold";
                    }
                }

                // Refresh statistics
                LoadStatistics();
                btnPreview_Click(sender, e);
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"❌ Error: {ex.Message}";
                lblMessage.CssClass = "text-danger fw-bold";
            }
        }

        private DataTable GetRecordsToConvert()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                // Get all records that are not yet Unicode
                string query = @"
                    SELECT SerialNo, DevoteeName, EventName, District, City, Address
                    FROM SevaMemberDetails 
                    WHERE Language LIKE 'Kannada%' 
                    AND NOT (DevoteeName LIKE N'%[ಅ-ಔ]%')
                    ORDER BY SerialNo";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }

        protected void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    string backupQuery = @"
                        SELECT * 
                        INTO SevaMemberDetails_Backup_ConvertKannada
                        FROM SevaMemberDetails";

                    SqlCommand cmd = new SqlCommand(backupQuery, con);
                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "✅ Backup created successfully as 'SevaMemberDetails_Backup_ConvertKannada'";
                    lblMessage.CssClass = "text-success fw-bold";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"❌ Error creating backup: {ex.Message}";
                lblMessage.CssClass = "text-danger fw-bold";
            }
        }
    }
}