using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;

// Aliases avoid clashes between namespace and class names (BL.BL, GC.*, System.GC)
using SevaBL = SSMS.Masters.BL.BL;
using AllListGC = SSMS.Masters.GC.AllListGC;

namespace SSMS.Pages.Admin
{
    public partial class AllList : System.Web.UI.Page
    {
        private readonly SevaBL bl = new SevaBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateDropdowns();
            }
        }

        private void PopulateDropdowns()
        {
            ddlSevaType.Items.Clear();
            ddlSevaType.Items.Add(new System.Web.UI.WebControls.ListItem("-- Select Seva Type --", ""));

            foreach (System.Data.DataRow r in bl.GetSevaTypeNames().Rows)
            {
                string name = r["name"].ToString();
                ddlSevaType.Items.Add(new System.Web.UI.WebControls.ListItem(name, name));
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            var filter = new AllListGC
            {
                DevoteeName = txtDevoteeName.Text,
                SevaType = ddlSevaType.SelectedValue
            };

            GridView1.DataSource = bl.SearchDevotees(filter);
            GridView1.DataBind();
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            // Collect the selected rows from the grid (UI work)
            var selected = new List<AllListGC>();

            foreach (GridViewRow row in GridView1.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelect");
                if (chk != null && chk.Checked)
                {
                    selected.Add(new AllListGC
                    {
                        DevoteeName = row.Cells[1].Text,
                        Address = row.Cells[3].Text
                    });
                }
            }

            // BL builds the label HTML
            litReport.Text = bl.BuildLabelsHtml(selected);
            btnPrint.Visible = true;

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "ScrollToReport",
                "scrollToReport();",
                true);
        }

        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=DevoteesReport.pdf");
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);

            string htmlContent = bl.BuildPdfHtml(litReport.Text);

            using (StringReader sr = new StringReader(htmlContent))
            using (Document pdfDoc = new Document(PageSize.A4, 0, 0, 0, 0))
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
            }

            Response.End();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtDevoteeName.Text = string.Empty;
            ddlSevaType.SelectedIndex = 0;
            GridView1.DataSource = null;
            GridView1.DataBind();
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Print", "window.print();", true);
        }
    }
}