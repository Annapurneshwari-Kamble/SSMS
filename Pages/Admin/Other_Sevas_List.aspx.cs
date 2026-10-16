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
    public partial class Other_Sevas_List : System.Web.UI.Page
    {
        private readonly SevaBL bl = new SevaBL();
        private const string SevaType = "3";   // Other Sevas

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
                gvOtherSevas.DataSource = bl.GetSevaList(
                    SevaType,
                    ddlFilterType.SelectedValue,
                    txtFilterValue.Text);
                gvOtherSevas.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Error loading data: " + ex.Message;
            }
        }

        protected void ddlFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.TextMode = ddlFilterType.SelectedValue == "EntryDate"
                ? TextBoxMode.Date
                : TextBoxMode.SingleLine;

            txtFilterValue.Text = "";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlFilterType.SelectedIndex = 0;
            txtFilterValue.Text = "";
            txtFilterValue.TextMode = TextBoxMode.SingleLine;

            BindGrid();
        }

        protected void gvOtherSevas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOtherSevas.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        // ------------------------------------------------------------
        //  ROW COMMANDS
        // ------------------------------------------------------------
        protected void gvOtherSevas_RowCommand(object sender, GridViewCommandEventArgs e)
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
        }

        protected void gvOtherSevas_RowDataBound(object sender, GridViewRowEventArgs e)
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
                        "Other_Sevas_List.aspx",
                        serialNo,
                        serialNo,
                        "Other Seva Member record deleted");

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
            try
            {
                // Read filter rows from the UI into GC objects
                var filters = new List<SevaFilterGC>();

                foreach (GridViewRow row in gvAdvancedFilters.Rows)
                {
                    DropDownList ddlCondition = (DropDownList)row.FindControl("ddlCondition");
                    DropDownList ddlColumn = (DropDownList)row.FindControl("ddlColumn");
                    TextBox txtValue = (TextBox)row.FindControl("txtValue");

                    filters.Add(new SevaFilterGC
                    {
                        Condition = ddlCondition.SelectedValue,
                        Column = ddlColumn.SelectedValue,
                        Value = txtValue.Text
                    });
                }

                gvOtherSevas.DataSource = bl.GetSevaListAdvanced(SevaType, filters);
                gvOtherSevas.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Error: " + ex.Message;
            }
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

        protected void btnClearAdvancedSearch_Click(object sender, EventArgs e)
        {
            ViewState["AdvancedFilters"] = null;

            gvAdvancedFilters.DataSource = null;
            gvAdvancedFilters.DataBind();

            pnlAdvancedSearch.Visible = false;

            BindGrid();
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

            foreach (GridViewRow row in gvOtherSevas.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                if (chk != null && chk.Checked)
                {
                    serialNos.Add(gvOtherSevas.DataKeys[row.RowIndex].Value.ToString());
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