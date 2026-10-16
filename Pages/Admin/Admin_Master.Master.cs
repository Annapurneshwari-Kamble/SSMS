using SSMS.Masters.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSMS.Pages.Admin
{
    public partial class Admin_Master : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Never let the browser cache any admin page,
            //    so Back always asks the server again
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetValidUntilExpires(false);

            // 2. Session check: if logged out, go to login
            if (Session["UserName"] == null)
            {
                Response.Redirect("~/Default.aspx");   // keep default (true): it stops the page from running
            }

            if (!IsPostBack)
            {
                string userType = Session["UserType"]?.ToString();

                // Show User Management only for Admin
                UsermasterMenuItem.Visible =
                    !string.IsNullOrEmpty(userType) &&
                    userType.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Update the existing LOGIN row with LogoutDate
            UserLogHelper.Log(
                "LOGOUT",
                "Default.aspx",
                null,
                null,
                "User logged out"
            );

            Session.Clear();
            Session.Abandon();

            if (Request.Cookies["SSMS_Login"] != null)
            {
                HttpCookie cookie = new HttpCookie("SSMS_Login");
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.Path = "/";          // must match the path used when the cookie was created
                Response.Cookies.Add(cookie);
            }

            Response.Redirect("~/Default.aspx", true);
        }
    }
}