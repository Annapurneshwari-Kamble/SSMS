using System;

// Aliases avoid clashes between namespace and class names (BL.BL, GC.*, System.GC)
using SevaBL = SSMS.Masters.BL.BL;
using DashboardGC = SSMS.Masters.GC.DashboardGC;

namespace SSMS.Pages.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private readonly SevaBL bl = new SevaBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check the session first, before loading any data
            if (Session["UserName"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboardCounts();
                LoadTodaysSevas();
            }
        }

        // Summary counts
        private void LoadDashboardCounts()
        {
            DashboardGC counts = bl.GetDashboardCounts();

            lblTotalAbhishek.Text = counts.TotalAbhishek.ToString();
            lblTotalAnnaprasad.Text = counts.TotalAnnaprasad.ToString();
            lblTotalOtherSevas.Text = counts.TotalOtherSevas.ToString();
            lblTotalSeva.Text = counts.TotalSeva.ToString();
        }

        // Today's devotees (right-side notifications)
        private void LoadTodaysSevas()
        {
            rptAbhishek.DataSource = bl.GetTodaysSevas(1);
            rptAbhishek.DataBind();

            rptAnnaprasad.DataSource = bl.GetTodaysSevas(2);
            rptAnnaprasad.DataBind();

            rptOtherSevas.DataSource = bl.GetTodaysSevas(3);
            rptOtherSevas.DataBind();
        }
    }
}