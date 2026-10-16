using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSMS.Pages.Admin
{
    public partial class Nirantara_Abhishek : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ Basic validation before inserting
                if (string.IsNullOrWhiteSpace(txtDevoteeName.Text) ||
                    string.IsNullOrWhiteSpace(txtSevaType.Text) ||
                    string.IsNullOrWhiteSpace(txtSevaCategory.Text) ||
                    string.IsNullOrWhiteSpace(txtSevaAmount.Text) ||
                    !decimal.TryParse(txtSevaAmount.Text, out decimal sevaAmount) || sevaAmount <= 0)
                {
                    lblMessage.Text = "⚠ Please enter valid details in all required fields.";
                    lblMessage.CssClass = "text-danger fw-bold";
                    return; // Stop execution here
                }

                string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"INSERT INTO Nirantara_Abhishek 
                            (DevoteeName, SevaType, SevaCategory, SevaAmount, AvailabilityDays, PrasadaDetails, IsActive, CreatedDate, CreatedBy) 
                            VALUES (@DevoteeName, @SevaType, @SevaCategory, @SevaAmount, @AvailabilityDays, @PrasadaDetails, @IsActive, GETDATE(), @CreatedBy)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DevoteeName", txtDevoteeName.Text.Trim());
                        cmd.Parameters.AddWithValue("@SevaType", txtSevaType.Text.Trim());
                        cmd.Parameters.AddWithValue("@SevaCategory", txtSevaCategory.Text.Trim());
                        cmd.Parameters.AddWithValue("@SevaAmount", sevaAmount);
                        cmd.Parameters.AddWithValue("@AvailabilityDays", txtAvailabilityDays.Text.Trim());
                        cmd.Parameters.AddWithValue("@PrasadaDetails", txtPrasadaDetails.Text.Trim());
                        cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                        cmd.Parameters.AddWithValue("@CreatedBy", "Admin"); // Or Session["User"]

                        con.Open();
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            lblMessage.Text = "✅ Seva details saved successfully!";
                            lblMessage.CssClass = "text-success fw-bold";
                        }
                        else
                        {
                            lblMessage.Text = "❌ Failed to save data!";
                            lblMessage.CssClass = "text-danger fw-bold";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.CssClass = "text-danger fw-bold";
            }
        }

    }
}
    