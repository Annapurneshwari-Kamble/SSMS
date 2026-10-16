using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using SSMS.Masters.Helper;

using SevaBL = SSMS.Masters.BL.BL;
using SevaGC = SSMS.Masters.GC.GC;

namespace SSMS.Pages.Admin
{
    public partial class Add_Seva_Member_details : Page
    {
        private readonly SevaBL bl = new SevaBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            this.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                BindStates();
                BindSevaTypes();
                LoadTodaysSevas();
                BindSevaFor("");

                string serialNo = Request.QueryString["SerialNo"];
                if (!string.IsNullOrEmpty(serialNo))
                {
                    LoadForEdit(serialNo);
                    btnGenerateReport.Text = "Update & Generate Report";
                }
                else
                {
                    txtSerialNo.Text = bl.GenerateSerialNumber();
                    txtRegisterNo.Text = "";
                    txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    btnGenerateReport.Text = "Save & Generate Report";
                }
            }
        }

        // ------------------------------------------------------------
        //  BINDING
        // ------------------------------------------------------------
        private void BindSevaTypes()
        {
            ddlMainSeva.DataSource = bl.GetSevaTypes();
            ddlMainSeva.DataTextField = "name";
            ddlMainSeva.DataValueField = "id";
            ddlMainSeva.DataBind();
            ddlMainSeva.Items.Insert(0, new ListItem("-- Select Seva --", ""));
        }

        private void BindStates()
        {
            ddlState.Items.Clear();
            ddlState.Items.Add(new ListItem("-- Select State --", ""));
            foreach (string state in bl.GetStates())
                ddlState.Items.Add(new ListItem(state, state));
        }

        private void BindSevaFor(string sevaType)
        {
            ddlSevaFor.DataSource = bl.GetSevaFor(sevaType);
            ddlSevaFor.DataTextField = "SevaFor";
            ddlSevaFor.DataValueField = "SevaCode";
            ddlSevaFor.DataBind();
            ddlSevaFor.Items.Insert(0, new ListItem("-- Select Seva For --", ""));
        }

        private void LoadTodaysSevas()
        {
            rptAbhishek.DataSource = bl.GetTodaysSevas(1);
            rptAbhishek.DataBind();

            rptAnnaprasad.DataSource = bl.GetTodaysSevas(2);
            rptAnnaprasad.DataBind();

            rptOtherSevas.DataSource = bl.GetTodaysSevas(3);
            rptOtherSevas.DataBind();
        }

        // ------------------------------------------------------------
        //  EDIT MODE
        // ------------------------------------------------------------
        private void LoadForEdit(string serialNo)
        {
            SevaGC m = bl.GetBySerialNo(serialNo);
            if (m == null) return;

            hdnSevaId.Value = m.SerialNo;
            txtSerialNo.Text = m.SerialNo;
            txtDate.Text = m.EntryDate.ToString("yyyy-MM-dd");
            rblLanguage.SelectedValue = m.Language;
            ddlMainSeva.SelectedValue = m.SevaType;
            BindSevaFor(m.SevaType);
            ddlSevaFor.SelectedValue = m.SevaFor;
            txtEvent.Text = m.EventName;
            txtRegisterNo.Text = m.RegisterNo;
            txtName.Text = m.DevoteeName;
            ddlState.SelectedValue = m.State;
            txtDistrict.Text = m.District;
            txtPincode.Text = m.Pincode;
            txtCity.Text = m.City;
            txtAddress.Text = m.Address;
            txtContact.Text = m.ContactNumber;
            txtEmail.Text = m.Email;
            txtPrice.Text = m.Price.ToString();
            txtAdditionalAmount.Text = m.AdditionalAmount.ToString("0.00");
            txtTotal.Text = m.Total.ToString("0.00");
            rblSevaStart.SelectedValue = m.AdditionalAmount > 0 ? "ThisYear" : "NextYear";
            txtRemarks.Text = m.Remarks;
            ddlPaymentMode.SelectedValue = m.PaymentMode;
            txtUTRNumber.Text = m.UTRNumber;
            txtBankName.Text = m.BankName;
            txtBranch.Text = m.Branch;
            txtIFSC.Text = m.IFSCCode;
            txtAccountNo.Text = m.AccountNumber;
        }

        // ------------------------------------------------------------
        //  SAVE
        // ------------------------------------------------------------
        private SevaGC BuildModel()
        {
            return new SevaGC
            {
                SerialNo = txtSerialNo.Text.Trim(),
                Language = rblLanguage.SelectedValue,
                SevaType = ddlMainSeva.SelectedValue,
                SevaFor = ddlSevaFor.SelectedValue,
                EventName = txtEvent.Text,
                RegisterNo = txtRegisterNo.Text,
                DevoteeName = txtName.Text,
                State = ddlState.SelectedValue,
                District = txtDistrict.Text,
                Pincode = txtPincode.Text,
                City = txtCity.Text,
                Address = txtAddress.Text,
                ContactNumber = txtContact.Text,
                Email = txtEmail.Text,
                Remarks = txtRemarks.Text,
                PaymentMode = ddlPaymentMode.SelectedValue,
                UTRNumber = txtUTRNumber.Text,
                BankName = txtBankName.Text,
                Branch = txtBranch.Text,
                IFSCCode = txtIFSC.Text,
                AccountNumber = txtAccountNo.Text,
                EntryDateText = txtDate.Text,
                PriceText = txtPrice.Text,
                SevaStart = rblSevaStart.SelectedValue,
                AdditionalAmountText = txtAdditionalAmount.Text
            };
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            SevaGC m = BuildModel();
            bool isEdit = !string.IsNullOrEmpty(Request.QueryString["SerialNo"]);

            string error;
            bool valid = bl.Validate(m, out error);

            // Reflect values computed by BL (total, bank defaults)
            txtTotal.Text = m.Total.ToString("0.00");
            if (m.PaymentMode == "Bank")
            {
                txtBankName.Text = m.BankName;
                txtBranch.Text = m.Branch;
                txtIFSC.Text = m.IFSCCode;
                txtAccountNo.Text = m.AccountNumber;
            }

            if (!valid)
            {
                ShowMessage(error, "alert alert-danger");
                return;
            }

            try
            {
                bl.Save(m, isEdit);

                UserLogHelper.Log(
                    isEdit ? "UPDATE" : "INSERT",
                    "SevaMemberDetails.aspx",
                    null,
                    m.SerialNo,
                    isEdit ? "Seva Member details updated" : "New Seva Member inserted");

                Response.Redirect("Receipt.aspx?SerialNo=" + m.SerialNo, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                ShowMessage("❌ Error: " + ex.Message, "text-danger fw-bold");
            }
        }

        // ------------------------------------------------------------
        //  OTHER EVENTS
        // ------------------------------------------------------------
        protected void btnClearForm_Click(object sender, EventArgs e)
        {
            txtSerialNo.Text = bl.GenerateSerialNumber();
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtEvent.Text = "";
            txtRegisterNo.Text = "";
            txtName.Text = "";
            ddlState.SelectedIndex = 0;
            txtDistrict.Text = "";
            txtPincode.Text = "";
            txtCity.Text = "";
            txtAddress.Text = "";
            txtContact.Text = "";
            txtEmail.Text = "";
            txtPrice.Text = "";
            rblSevaStart.ClearSelection();
            txtAdditionalAmount.Text = "0";
            txtTotal.Text = "0.00";
            txtRemarks.Text = "";
            ddlPaymentMode.SelectedIndex = 0;
            ddlMainSeva.SelectedIndex = 0;
            ddlSevaFor.SelectedIndex = 0;
            rblLanguage.SelectedIndex = 0;
            txtUTRNumber.Text = "";
            txtBankName.Text = "";
            txtBranch.Text = "";
            txtIFSC.Text = "";
            txtAccountNo.Text = "";

            ShowMessage("✅ Form cleared successfully!", "text-info fw-bold");
        }

        protected void ddlMainSeva_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sevaType = ddlMainSeva.SelectedValue;
            ddlSevaFor.Items.Clear();

            if (string.IsNullOrEmpty(sevaType))
            {
                ddlSevaFor.Items.Insert(0, new ListItem("-- Select Seva For --", ""));
                txtRegisterNo.Text = "";
                return;
            }

            BindSevaFor(sevaType);
            txtRegisterNo.Text = "";   // cleared until Seva For is selected
        }

        protected void ddlSevaFor_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSerialNo.Text = bl.GenerateSerialNumber();
            txtRegisterNo.Text = bl.GenerateRegisterNumber(ddlSevaFor.SelectedValue);
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowMessage($"✅ Selected State: {ddlState.SelectedValue}. Now enter District and City.",
                        "text-info fw-bold");
        }

        protected void txtPincode_TextChanged(object sender, EventArgs e)
        {
            ShowMessage($"📍 Pincode entered: {txtPincode.Text}", "text-primary fw-bold");
        }

        private void ShowMessage(string text, string cssClass)
        {
            lblMessage.Text = text;
            lblMessage.CssClass = cssClass;
            lblMessage.Visible = true;
        }
    }
}