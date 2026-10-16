using SSMS.Masters.Helper;
using SSMS.Pages.Admin;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSMS
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Stop the browser from caching the login page (always, not conditionally)
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetValidUntilExpires(false);

            if (!IsPostBack)
            {
                if (Session["UserName"] == null && Request.Cookies["SSMS_Login"] != null)
                {
                    // Do NOT automatically restore the session from cookie.
                    HttpCookie cookie = new HttpCookie("SSMS_Login");
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    cookie.Path = "/";
                    Response.Cookies.Add(cookie);

                    Session.Clear();
                    Session.Abandon();
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate empty fields BEFORE connecting to database
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Please enter username and password.";
                lblError.Visible = true;
                return;
            }

            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    string hashedPassword = HashPassword(password);

                    string query = @"
                SELECT id, user_id, UserName, user_type
                FROM [SMSF].[dbo].[Usermaster]
                WHERE UserName = @UserName
                AND PasswordHash = @Password
                AND isActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userMasterID = Convert.ToInt32(reader["id"]);

                                string userID = reader["user_id"] == DBNull.Value
                                    ? ""
                                    : reader["user_id"].ToString();

                                string userName = reader["UserName"].ToString();

                                string userType = reader["user_type"] == DBNull.Value
                                    ? ""
                                    : reader["user_type"].ToString();

                                // Store user information in Session
                                Session["UserMasterID"] = userMasterID;
                                Session["UserID"] = userID;
                                Session["UserName"] = userName;
                                Session["UserType"] = userType;

                                // Login log
                                UserLogHelper.Log( "LOGIN","Default.aspx",null,null,"User logged in successfully");

                                // Login cookie
                                HttpCookie loginCookie = new HttpCookie("SSMS_Login");
                                loginCookie["UserName"] = userName;
                                loginCookie.HttpOnly = true;
                                loginCookie.Path = "/";

                                Response.Cookies.Add(loginCookie);

                                Response.Redirect(
                                    "~/Pages/Admin/Dashboard.aspx",
                                    false
                                );

                                Context.ApplicationInstance.CompleteRequest();
                            }
                            else
                            {
                                lblError.Text = "Invalid username or password.";
                                lblError.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (SqlException)
            {
                lblError.Text = "Unable to connect to the database. Please contact the administrator.";
                lblError.Visible = true;
            }
            catch (Exception)
            {
                lblError.Text = "An unexpected error occurred. Please try again.";
                lblError.Visible = true;
            }
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Log logout only if a user session exists
            if (Session["UserMasterID"] != null)
            {
                UserLogHelper.Log("LOGOUT",Request.Url.AbsolutePath,null,null,"User logged out");
            }

            // Clear session completely
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            // Remove login cookie
            if (Request.Cookies["SSMS_Login"] != null)
            {
                HttpCookie cookie = new HttpCookie("SSMS_Login");
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.HttpOnly = true;
                cookie.Path = "/";
                Response.Cookies.Add(cookie);
            }

            // Always return to login page
            Response.Redirect("~/Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

    }
} 