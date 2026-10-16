using System;
using System.Web.UI.WebControls;

// Alias avoids clashes between namespace and class names (BL.BL, GC.*, System.GC)
using SevaBL = SSMS.Masters.BL.BL;

namespace SSMS.Pages.Admin
{
    public partial class UsermasterList : System.Web.UI.Page
    {
        private readonly SevaBL bl = new SevaBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            try
            {
                gvUsers.DataSource = bl.GetUserList();
                gvUsers.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("❌ Error loading users: " + ex.Message, "fw-bold text-danger");
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteUser")
                return;

            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id))
                return;

            try
            {
                int rows = bl.DeleteUser(id);

                if (rows > 0)
                    ShowMessage("User deleted successfully.", "fw-bold text-success");
                else
                    ShowMessage("⚠️ User not found.", "fw-bold text-danger");

                LoadUsers();
            }
            catch (Exception ex)
            {
                ShowMessage("❌ Error deleting user: " + ex.Message, "fw-bold text-danger");
            }
        }

        private void ShowMessage(string text, string cssClass)
        {
            lblMessage.Text = text;
            lblMessage.CssClass = cssClass;
        }
    }
}