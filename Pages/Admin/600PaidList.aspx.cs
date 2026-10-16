using System;
using System.Data;

namespace SSMS.Pages.Admin
{
    public partial class _600PaidList : System.Web.UI.Page
    {
        private readonly Masters.BL.BL bl = new Masters.BL.BL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPaidList();
            }
        }

        private void LoadPaidList()
        {
            DataTable dt = bl.GetPaidList(
                txtSearch.Text,
                txtSerialNo.Text,
                txtRegisterNo.Text);

            gvPaidList.DataSource = dt;
            gvPaidList.DataBind();

            int recordCount;
            decimal additionalTotal, grandTotal;
            bl.CalculateTotals(dt, out recordCount, out additionalTotal, out grandTotal);

            lblRecordCount.Text = recordCount.ToString();
            lblAdditionalTotal.Text = additionalTotal.ToString("N2");
            lblGrandTotal.Text = grandTotal.ToString("N2");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadPaidList();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSerialNo.Text = "";
            txtRegisterNo.Text = "";

            LoadPaidList();
        }
    }
}