using System;
using System.Web;

// Aliases avoid clashes between namespace and class names (BL.BL, GC.*, System.GC)
using SevaBL = SSMS.Masters.BL.BL;
using ReceiptGC = SSMS.Masters.GC.ReceiptGC;

namespace SSMS.Pages.Admin
{
    public partial class Receipt : System.Web.UI.Page
    {
        private readonly SevaBL bl = new SevaBL();

        // Used by the .aspx, e.g. <%= Model.DevoteeName %>
        protected ReceiptGC Model { get; private set; } = new ReceiptGC();

        protected void Page_Load(object sender, EventArgs e)
        {
            string serialNo = Request.QueryString["SerialNo"];

            if (string.IsNullOrWhiteSpace(serialNo))
            {
                Response.Redirect("Add_Seva_Member_details.aspx");
                return;
            }

            ReceiptGC receipt = null;

            try
            {
                receipt = bl.GetReceipt(serialNo);
            }
            catch (Exception ex)
            {
                Response.Write(
                    "<div style='color:red;font-weight:bold;padding:20px;'>" +
                    "Error loading receipt: " + Server.HtmlEncode(ex.Message) +
                    "</div>");
                return;
            }

            if (receipt == null)
            {
                Response.Redirect("RecordNotFound.aspx");
                return;
            }

            Model = receipt;
        }

        protected void btnSaveReport_Click(object sender, EventArgs e)
        {
            try
            {
                bl.SaveReceipt(Model);

                // Model is not reset, so the page still shows the receipt for printing
                ClientScript.RegisterStartupScript(
                    this.GetType(), "printAfterSave",
                    "alert('Report saved successfully!'); printReceipt();", true);
            }
            catch (Exception ex)
            {
                string msg = HttpUtility.JavaScriptStringEncode(ex.Message);
                ClientScript.RegisterStartupScript(
                    this.GetType(), "alert", "alert('Error: " + msg + "');", true);
            }
        }
    }
}