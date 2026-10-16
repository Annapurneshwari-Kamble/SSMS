using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using SSMS.Masters.Helper;

// Aliases avoid clashes between namespace and class names (BL.BL, GC.*, System.GC)
using SevaBL = SSMS.Masters.BL.BL;
using SevaFilterGC = SSMS.Masters.GC.SevaFilterGC;

namespace SSMS.Pages.Admin
{
    public partial class Nirantara_Abhishek_List : System.Web.UI.Page
    {
        private readonly SevaBL bl = new SevaBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        // ------------------------------------------------------------
        //  GRID BINDING
        // ------------------------------------------------------------
        private void BindGrid()
        {
            try
            {
                if (AdvancedActive && pnlAdvancedSearch.Visible)
                    gvAbhishek.DataSource = bl.GetSevaListAdvanced("1", ReadAdvancedFilters());
                else
                    gvAbhishek.DataSource = bl.GetSevaList("1", ddlFilterType.SelectedValue, txtFilterValue.Text);

                gvAbhishek.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Error loading data: " + ex.Message;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            AdvancedActive = false;      // simple search takes over
            gvAbhishek.PageIndex = 0;
            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            AdvancedActive = false;
            gvAbhishek.PageIndex = 0;

            ddlFilterType.SelectedIndex = 0;
            txtFilterValue.Text = "";
            txtFilterValue.TextMode = TextBoxMode.SingleLine;

            BindGrid();
        }

        protected void gvAbhishek_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAbhishek.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void ddlFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.TextMode = ddlFilterType.SelectedValue == "EntryDate"
                ? TextBoxMode.Date
                : TextBoxMode.SingleLine;

            txtFilterValue.Text = "";
        }

        // ------------------------------------------------------------
        //  ROW COMMANDS
        // ------------------------------------------------------------
        protected void gvAbhishek_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string serialNo = e.CommandArgument?.ToString();

            if (e.CommandName == "EditRow")
            {
                Response.Redirect("Add_Seva_Member_details.aspx?SerialNo=" + serialNo);
            }
            else if (e.CommandName == "DeleteRow")
            {
                if (Session["UserType"]?.ToString()?.ToLower() != "admin")
                {
                    lblMessage.Text = "❌ You are not authorized to delete.";
                    return;
                }

                DeleteRecord(serialNo);
                BindGrid();
            }
            else if (e.CommandName == "ViewRow")
            {
                Response.Redirect("NirantaraAbhishekForm.aspx?SerialNo=" + serialNo);
            }
        }

        protected void gvAbhishek_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string userType = Session["UserType"]?.ToString()?.ToLower();
                LinkButton btnDelete = (LinkButton)e.Row.FindControl("btnDelete");

                if (btnDelete != null && userType != "admin")
                {
                    btnDelete.Visible = false;
                }
            }
        }

        private void DeleteRecord(string serialNo)
        {
            try
            {
                int rowsAffected = bl.DeleteSeva(serialNo);

                // Log only if the record was actually deleted
                if (rowsAffected > 0)
                {
                    UserLogHelper.Log(
                        "DELETE",
                        "Nirantara_Abhishek_List.aspx",
                        serialNo,
                        serialNo,
                        "Nirantara Abhishek Seva Member record deleted");

                    lblMessage.CssClass = "fw-bold text-success";
                    lblMessage.Text = "✔ Record deleted successfully.";
                }
                else
                {
                    lblMessage.CssClass = "fw-bold text-danger";
                    lblMessage.Text = "❌ Record not found.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "fw-bold text-danger";
                lblMessage.Text = "❌ Error deleting record: " + ex.Message;
            }
        }

        // ------------------------------------------------------------
        //  ADVANCED SEARCH
        // ------------------------------------------------------------
        protected void btnAdvancedSearch_Click(object sender, EventArgs e)
        {
            pnlAdvancedSearch.Visible = true;

            DataTable dt;

            // Always get previous rows from ViewState
            if (ViewState["AdvancedFilters"] != null)
            {
                dt = (DataTable)ViewState["AdvancedFilters"];

                // update values typed by user
                for (int i = 0; i < gvAdvancedFilters.Rows.Count; i++)
                {
                    DropDownList ddlCondition = (DropDownList)gvAdvancedFilters.Rows[i].FindControl("ddlCondition");
                    DropDownList ddlColumn = (DropDownList)gvAdvancedFilters.Rows[i].FindControl("ddlColumn");
                    TextBox txtValue = (TextBox)gvAdvancedFilters.Rows[i].FindControl("txtValue");

                    dt.Rows[i]["Condition"] = ddlCondition.SelectedValue;
                    dt.Rows[i]["Column"] = ddlColumn.SelectedValue;
                    dt.Rows[i]["Value"] = txtValue.Text;
                }
            }
            else
            {
                dt = CreateFilterTable();
            }

            // Add new row
            DataRow dr = dt.NewRow();
            dr["Condition"] = "AND";
            dr["Column"] = "SerialNo";
            dr["Value"] = "";
            dt.Rows.Add(dr);

            ViewState["AdvancedFilters"] = dt;

            gvAdvancedFilters.DataSource = dt;
            gvAdvancedFilters.DataBind();
        }

        protected void btnApplyAdvancedSearch_Click(object sender, EventArgs e)
        {
            AdvancedActive = true;
            gvAbhishek.PageIndex = 0;
            BindGrid();
        }
        protected void btnClearAdvancedSearch_Click(object sender, EventArgs e)
        {
            AdvancedActive = false;
            ViewState["AdvancedFilters"] = null;

            gvAdvancedFilters.DataSource = null;
            gvAdvancedFilters.DataBind();

            pnlAdvancedSearch.Visible = false;
            gvAbhishek.PageIndex = 0;
            BindGrid();
        }

        private List<SevaFilterGC> ReadAdvancedFilters()
        {
            var filters = new List<SevaFilterGC>();

            foreach (GridViewRow row in gvAdvancedFilters.Rows)
            {
                filters.Add(new SevaFilterGC
                {
                    Condition = ((DropDownList)row.FindControl("ddlCondition")).SelectedValue,
                    Column = ((DropDownList)row.FindControl("ddlColumn")).SelectedValue,
                    Value = ((TextBox)row.FindControl("txtValue")).Text
                });
            }

            return filters;
        }

        private bool AdvancedActive
        {
            get { return ViewState["AdvancedActive"] as bool? ?? false; }
            set { ViewState["AdvancedActive"] = value; }
        }
        protected void gvAdvancedFilters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;

                DropDownList ddlCondition = (DropDownList)e.Row.FindControl("ddlCondition");
                DropDownList ddlColumn = (DropDownList)e.Row.FindControl("ddlColumn");
                TextBox txtValue = (TextBox)e.Row.FindControl("txtValue");

                if (drv != null)
                {
                    ddlCondition.SelectedValue = drv["Condition"].ToString();
                    ddlColumn.SelectedValue = drv["Column"].ToString();
                    txtValue.Text = drv["Value"].ToString();
                }
            }
        }

        private DataTable CreateFilterTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Condition");
            dt.Columns.Add("Column");
            dt.Columns.Add("Value");
            return dt;
        }

        // ------------------------------------------------------------
        //  BULK DATE UPDATE
        // ------------------------------------------------------------
        protected void btnSetBulkDate_Click(object sender, EventArgs e)
        {
            DateTime selectedDate;

            if (string.IsNullOrEmpty(txtBulkDate.Text) ||
                !DateTime.TryParse(txtBulkDate.Text, out selectedDate))
            {
                lblMessage.Text = "⚠ Please select a date.";
                return;
            }

            // Collect checked rows (UI work)
            var serialNos = new List<string>();

            foreach (GridViewRow row in gvAbhishek.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                if (chk != null && chk.Checked)
                {
                    serialNos.Add(gvAbhishek.DataKeys[row.RowIndex].Value.ToString());
                }
            }

            try
            {
                int updatedCount = bl.UpdateEntryDate(serialNos, selectedDate);
                lblMessage.Text = $"✔ {updatedCount} records updated.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Error: " + ex.Message;
            }

            BindGrid();
        }
    }
}