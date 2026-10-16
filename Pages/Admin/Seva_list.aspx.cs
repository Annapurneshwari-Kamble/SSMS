using System;
using System.Drawing;
using System.Web.UI.WebControls;

// Alias avoids clashes between namespace and class names (BL.BL, GC.*, System.GC)
using SevaBL = SSMS.Masters.BL.BL;

namespace SSMS.Pages.Admin
{
    public partial class Seva_list : System.Web.UI.Page
    {
        private readonly SevaBL bl = new SevaBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSevaList();
            }
        }

        private void BindSevaList()
        {
            try
            {
                gvSevaList.DataSource = bl.GetSevaMasterList();
                gvSevaList.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("❌ Error loading data: " + ex.Message, Color.Red);
            }
        }

        protected void gvSevaList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSevaList.PageIndex = e.NewPageIndex;
            BindSevaList();
        }

        protected void gvSevaList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int sevaId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out sevaId))
                return;

            if (e.CommandName == "EditSeva")
            {
                Response.Redirect("Seva_Master.aspx?SevaID=" + sevaId);
            }
            else if (e.CommandName == "DeleteSeva")
            {
                try
                {
                    int rows = bl.DeleteSevaMaster(sevaId);

                    if (rows > 0)
                        ShowMessage("✅ Seva deleted successfully.", Color.Green);
                    else
                        ShowMessage("⚠️ No record found to delete.", Color.OrangeRed);

                    BindSevaList();
                }
                catch (Exception ex)
                {
                    ShowMessage("❌ Error deleting Seva: " + ex.Message, Color.Red);
                }
            }
        }

        private void ShowMessage(string text, Color color)
        {
            lblMessage.Text = text;
            lblMessage.ForeColor = color;
        }
    }
}