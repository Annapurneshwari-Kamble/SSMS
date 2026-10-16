using System;
using System.Web.UI.WebControls;

// Aliases avoid clashes between namespace and class names (BL.BL, GC.*, System.GC)
using SevaBL = SSMS.Masters.BL.BL;
using PaymentFilterGC = SSMS.Masters.GC.PaymentFilterGC;

namespace SSMS.Pages.Admin
{
    public partial class PaymentList : System.Web.UI.Page
    {
        private readonly SevaBL bl = new SevaBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSevaTypes();
                LoadPayments();
            }
        }

        // ------------------------------------------------------------
        //  BINDING
        // ------------------------------------------------------------
        private void LoadSevaTypes()
        {
            ddlSevaTypeFilter.DataSource = bl.GetSevaTypesOrdered();
            ddlSevaTypeFilter.DataTextField = "name";
            ddlSevaTypeFilter.DataValueField = "id";
            ddlSevaTypeFilter.DataBind();

            ddlSevaTypeFilter.Items.Insert(0, new ListItem("Select Seva Type", ""));
        }

        private void LoadPayments()
        {
            var filter = new PaymentFilterGC
            {
                FilterType = ddlFilterType.SelectedValue,
                Value = txtFilterValue.Text,
                SevaType = ddlSevaTypeFilter.SelectedValue,
                PaymentMode = ddlPaymentModeFilter.SelectedValue
            };

            try
            {
                gvPayment.DataSource = bl.GetPayments(filter);
                gvPayment.DataBind();
            }
            catch (ArgumentException ex)
            {
                // Invalid date typed by the user
                Response.Write("<script>alert('" + ex.Message.Replace("'", "") + "');</script>");
            }
        }

        // ------------------------------------------------------------
        //  EVENTS
        // ------------------------------------------------------------
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadPayments();
        }

        protected void gvPayment_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPayment.PageIndex = e.NewPageIndex;
            LoadPayments();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlFilterType.SelectedIndex = 0;
            txtFilterValue.Text = "";
            ddlPaymentModeFilter.SelectedIndex = 0;
            ddlSevaTypeFilter.SelectedIndex = 0;

            txtFilterValue.Visible = true;
            ddlPaymentModeFilter.Visible = false;
            ddlSevaTypeFilter.Visible = false;

            LoadPayments();
        }

        protected void ddlFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = true;
            ddlPaymentModeFilter.Visible = false;
            ddlSevaTypeFilter.Visible = false;

            if (ddlFilterType.SelectedValue == "PaymentMode")
            {
                txtFilterValue.Visible = false;
                ddlPaymentModeFilter.Visible = true;
            }
            else if (ddlFilterType.SelectedValue == "SevaType")
            {
                txtFilterValue.Visible = false;
                ddlSevaTypeFilter.Visible = true;
            }
            else if (ddlFilterType.SelectedValue == "Date")
            {
                txtFilterValue.TextMode = TextBoxMode.Date;
            }
            else
            {
                txtFilterValue.TextMode = TextBoxMode.SingleLine;
            }
        }
    }
}