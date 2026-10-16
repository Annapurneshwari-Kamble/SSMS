using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SSMS.Pages.Admin
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //protected void btnNirantaraAbhishek_Click(object sender, EventArgs e)
        //{
        //    // Redirect to Seva Master page
        //    Response.Redirect("~/Pages/Admin/Nirantara_Abhishek.aspx");
        //}

        //protected void btnNirantaraAnnaprasad_Click(object sender, EventArgs e)
        //{
        //    // Redirect to Annaprasad page
        //    Response.Redirect("~/Pages/Admin/Nirantara_Annaprasad.aspx");
        //}

        protected void btnAddSevaMember_Click(object sender, EventArgs e)
        {
            // Redirect to Add Seva Member Details page
            Response.Redirect("~/Pages/Admin/Add_Seva_Member_Details.aspx");
        }
    }
}
