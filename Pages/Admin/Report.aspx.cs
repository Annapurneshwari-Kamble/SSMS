using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
using System.Linq;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;
using System.Globalization;



namespace SSMS.Pages.Admin
{
    public partial class Report : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSevaTypes();

                // Set default From Date and To Date to today
                DateTime today = DateTime.Today;

                txtFromDate.Text = today.ToString("yyyy-MM-dd");
                txtToDate.Text = today.ToString("yyyy-MM-dd");

                // Update previous year's matching date only once per session
                if (Session["PreviousYearRecordsUpdated"] == null)
                {
                    UpdatePreviousYearRecords(today);

                    Session["PreviousYearRecordsUpdated"] = true;
                }
            }
        }

        private void UpdatePreviousYearRecords(DateTime today)
        {
            string connStr =
                ConfigurationManager.ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            DateTime previousYearDate = today.AddYears(-1);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
            UPDATE SMSF.dbo.SevaMemberDetails
            SET EntryDate = DATEADD(YEAR, 1, EntryDate)
            WHERE EntryDate >= @PreviousYearDate
              AND EntryDate < @NextDayPreviousYear;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@PreviousYearDate", SqlDbType.DateTime).Value = previousYearDate.Date;
                    cmd.Parameters.Add("@NextDayPreviousYear", SqlDbType.DateTime).Value = previousYearDate.Date.AddDays(1);
                    con.Open();
                    int rowsUpdated = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("Previous year records updated: " + rowsUpdated);
                }
            }
        }

        private void LoadSevaTypes()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT id, name FROM Seva_Type", con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    ddlSevaType.DataSource = dr;
                    ddlSevaType.DataTextField = "name";
                    ddlSevaType.DataValueField = "id";
                    ddlSevaType.DataBind();
                }
            }

            // Fully qualified to avoid ambiguity with iTextSharp.ListItem
            ddlSevaType.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select --", "0"));
        }
        private void LoadEventNames()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            ddlEventName.Items.Clear();

            ddlEventName.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- All Events --", ""));

            if (ddlSevaType.SelectedValue == "0")
                return;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
            SELECT DISTINCT EventName
            FROM SevaMemberDetails
            WHERE SevaType = @SevaType
              AND EventName IS NOT NULL
              AND LTRIM(RTRIM(EventName)) <> ''
            ORDER BY EventName";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SevaType", ddlSevaType.SelectedValue);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ddlEventName.Items.Add(
                                new System.Web.UI.WebControls.ListItem(
                                    dr["EventName"].ToString(),
                                    dr["EventName"].ToString()
                                )
                            );
                        }
                    }
                }
            }
        }

        protected void ddlSevaType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEventNames();
        }
        private DataTable GetReportData()
        {
            DataTable dt = new DataTable();
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT
                        smd.MemberID,smd.DevoteeName,st.name AS SevaType,
                        ISNULL(smd.City,'') AS Address,
                        smd.EntryDate

                    FROM SevaMemberDetails smd
                    INNER JOIN Seva_Type st
                    ON smd.SevaType = st.id

                    WHERE (@SevaType = 0 OR smd.SevaType=@SevaType)
                    AND smd.EntryDate BETWEEN @FromDate AND @ToDate

                    ORDER BY smd.EntryDate";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SevaType", ddlSevaType.SelectedValue);
                    DateTime fromDate = DateTime.Parse(txtFromDate.Text);
                    DateTime toDate = DateTime.Parse(txtToDate.Text);

                    cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate;
                    cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = toDate;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        // Generate report in GridView
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            DataTable dt = GetReportData();
            gvReport.DataSource = dt;
            gvReport.DataBind();
        }

        // Export to Excel
        //protected void btnExportExcel_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = GetReportData();
        //    if (dt == null || dt.Rows.Count == 0)
        //        return;

        //    // Set EPPlus license (for EPPlus 8+)
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


        //    using (ExcelPackage package = new ExcelPackage())
        //    {
        //        // Add worksheet
        //        ExcelWorksheet ws = package.Workbook.Worksheets.Add("SevaReport");

        //        // Load DataTable
        //        ws.Cells["A1"].LoadFromDataTable(dt, true);

        //        // Auto-fit columns
        //        ws.Cells[ws.Dimension.Address].AutoFitColumns();

        //        // Optional: make header bold
        //        using (var range = ws.Cells[1, 1, 1, dt.Columns.Count])
        //        {
        //            range.Style.Font.Bold = true;
        //            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //        }

        //        // Prepare response
        //        Response.Clear();
        //        Response.Buffer = true;
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", "attachment; filename=SevaReport.xlsx");

        //        // Save package to Response
        //        byte[] excelBytes = package.GetAsByteArray();
        //        Response.OutputStream.Write(excelBytes, 0, excelBytes.Length);

        //        Response.Flush();
        //        HttpContext.Current.ApplicationInstance.CompleteRequest();
        //    }
        //}


        // Export to PDF

        //protected void btnExportExcel_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = GetReportData();
        //    if (dt == null || dt.Rows.Count == 0)
        //        return;

        //    using (var wb = new XLWorkbook())
        //    {
        //        var ws = wb.Worksheets.Add(dt, "SevaReport");

        //        // Optional: format header row
        //        var headerRow = ws.Row(1);
        //        headerRow.Style.Font.Bold = true;
        //        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        //        // Auto-fit columns
        //        ws.Columns().AdjustToContents();

        //        // Prepare the response
        //        Response.Clear();
        //        Response.Buffer = true;
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", "attachment; filename=SevaReport.xlsx");

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            wb.SaveAs(memoryStream);
        //            memoryStream.WriteTo(Response.OutputStream);
        //        }

        //        Response.Flush();
        //        HttpContext.Current.ApplicationInstance.CompleteRequest();
        //    }
        //}
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = GetReportData(); // Your method to get filtered data

                if (dt == null || dt.Rows.Count == 0)
                {
                    //lblMsg.Text = "No data to export!";
                    return;
                }

                // Add SL No column at the beginning
                dt.Columns.Add("SL No").SetOrdinal(0);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["SL No"] = (i + 1).ToString();
                }

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=SevaReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                // REPORT HEADER 
                Response.Write("<table border='0' cellspacing='0' cellpadding='4' " +
                               "style='border-collapse:collapse; font-family:Calibri, Arial, sans-serif; font-size:10pt;'>");

                // Title
                Response.Write("<tr>");
                Response.Write("<td colspan='" + dt.Columns.Count + "' " +
                               "style='font-size:16px; font-weight:bold; text-align:center; background-color:#2c5aa0; color:white; padding:10px;'>");
                Response.Write("SEVA REPORT");
                Response.Write("</td>");
                Response.Write("</tr>");

                // Generated Date
                Response.Write("<tr>");
                Response.Write("<td colspan='" + dt.Columns.Count + "' " +
                               "style='font-weight:bold; text-align:center; background-color:#e8effb; padding:6px;'>");
                Response.Write("Generated on: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt"));
                Response.Write("</td>");
                Response.Write("</tr>");

                // TABLE HEADER 
                Response.Write("<tr>");
                foreach (DataColumn column in dt.Columns)
                {
                    Response.Write("<td style='text-align:center; padding:6px; border:1px solid #2c5aa0; " +
                                   "background-color:#3a66b0; color:white; font-weight:bold; height:28px;'>");
                    Response.Write(column.ColumnName.ToUpper());
                    Response.Write("</td>");
                }
                Response.Write("</tr>");

                //DATA ROWS 
                bool alternate = false;
                foreach (DataRow row in dt.Rows)
                {
                    Response.Write("<tr>");
                    foreach (DataColumn column in dt.Columns)
                    {
                        string bg = alternate ? "#f8f9fc" : "#ffffff"; // apply bg per cell only
                        Response.Write("<td style='padding:5px; border:1px solid #d0d9e9; font-weight:bold;font-size:20px; text-align:center; background-color:" + bg + ";'>");
                        Response.Write(row[column].ToString());
                        Response.Write("</td>");
                    }
                    Response.Write("</tr>");
                    alternate = !alternate;
                }

                // SUMMARY 
                Response.Write("<tr>");
                Response.Write("<td colspan='" + (dt.Columns.Count - 1) + "' " +
                               "style='text-align:right; padding:6px; border:1px solid #d0d9e9; background-color:#e8effb; font-weight:bold;'>TOTAL RECORDS:</td>");
                Response.Write("<td style='text-align:center; border:1px solid #d0d9e9; background-color:#e8effb; font-weight:bold;'>" + dt.Rows.Count + "</td>");
                Response.Write("</tr>");
                Response.Write("</table>");
                Response.Flush();
                Response.End();
            }
            catch (ThreadAbortException)
            {
                // Expected from Response.End()
            }
            catch (Exception)
            {
                //lblMsg.Text = ex.Message;
            }
        }

        //imp

        //protected void btnExportPDF_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = GetReportData();
        //    if (dt.Rows.Count == 0) return;

        //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        PdfWriter.GetInstance(pdfDoc, memoryStream);
        //        pdfDoc.Open();

        //        // Add title
        //        iTextSharp.text.Paragraph title =
        //        new iTextSharp.text.Paragraph("Seva Report")
        //        {
        //            Alignment = iTextSharp.text.Element.ALIGN_CENTER,
        //            SpacingAfter = 20f
        //        };
        //        pdfDoc.Add(title);

        //        // Add table
        //        PdfPTable pdfTable = new PdfPTable(dt.Columns.Count) { WidthPercentage = 100 };
        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName))
        //            {
        //                //BackgroundColor = BaseColor.LightGray
        //            };
        //            pdfTable.AddCell(cell);
        //        }

        //        foreach (DataRow row in dt.Rows)
        //        {
        //            foreach (var item in row.ItemArray)
        //            {
        //                pdfTable.AddCell(item.ToString());
        //            }
        //        }

        //        pdfDoc.Add(pdfTable);
        //        pdfDoc.Close();

        //        Response.Clear();
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        Response.Buffer = true;
        //        Response.ContentType = "application/pdf";
        //        Response.AddHeader("content-disposition", "attachment;filename=SevaReport.pdf");
        //        Response.OutputStream.Write(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
        //        Response.OutputStream.Flush();
        //        HttpContext.Current.ApplicationInstance.CompleteRequest();
        //    }
        //}



        //kannada language not correct 

        //protected void btnExportPDF_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = GetReportData();
        //    if (dt.Rows.Count == 0) return;

        //    Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 10f); // landscape - you have many columns
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        PdfWriter.GetInstance(pdfDoc, memoryStream);
        //        pdfDoc.Open();

        //        // ---- Load Unicode font that supports Kannada ----
        //        // Place NotoSansKannada-Regular.ttf (or Nirmala.ttf) inside a "Fonts" folder in your project
        //        string fontPath = Server.MapPath("~/Fonts/NotoSansKannada-Regular.ttf");
        //        BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

        //        iTextSharp.text.Font titleFont = new iTextSharp.text.Font(baseFont, 16f, iTextSharp.text.Font.BOLD);
        //        iTextSharp.text.Font headerFont = new iTextSharp.text.Font(baseFont, 9f, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
        //        iTextSharp.text.Font cellFont = new iTextSharp.text.Font(baseFont, 8f, iTextSharp.text.Font.NORMAL);

        //        // ---- Title ----
        //        iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Seva Report", titleFont)
        //        {
        //            Alignment = iTextSharp.text.Element.ALIGN_CENTER,
        //            SpacingAfter = 20f
        //        };
        //        pdfDoc.Add(title);

        //        // ---- Table ----
        //        PdfPTable pdfTable = new PdfPTable(dt.Columns.Count)
        //        {
        //            WidthPercentage = 100
        //        };
        //        pdfTable.SetWidths(GetColumnWidths(dt.Columns.Count)); // optional, see helper below
        //        pdfTable.HeaderRows = 1; // repeats header row on every page

        //        // Header row
        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, headerFont))
        //            {
        //                BackgroundColor = new BaseColor(52, 73, 94), // dark blue-gray
        //                HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER,
        //                VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,
        //                Padding = 5f
        //            };
        //            pdfTable.AddCell(cell);
        //        }

        //        // Data rows
        //        bool alternate = false;
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            BaseColor rowColor = alternate ? new BaseColor(245, 245, 245) : BaseColor.WHITE;

        //            foreach (var item in row.ItemArray)
        //            {
        //                string text = (item == null || item == DBNull.Value) ? "" : item.ToString();
        //                PdfPCell cell = new PdfPCell(new Phrase(text, cellFont))
        //                {
        //                    BackgroundColor = rowColor,
        //                    Padding = 4f,
        //                    VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE
        //                };
        //                pdfTable.AddCell(cell);
        //            }
        //            alternate = !alternate;
        //        }

        //        pdfDoc.Add(pdfTable);
        //        pdfDoc.Close();

        //        Response.Clear();
        //        Response.ClearContent();
        //        Response.ClearHeaders();
        //        Response.Buffer = true;
        //        Response.ContentType = "application/pdf";
        //        Response.AddHeader("content-disposition", "attachment;filename=SevaReport.pdf");
        //        Response.BinaryWrite(memoryStream.ToArray()); // safer than GetBuffer() - avoids trailing empty bytes
        //        Response.Flush();
        //        HttpContext.Current.ApplicationInstance.CompleteRequest();
        //    }
        //}

        // Optional: proportion column widths so long Kannada name/address columns get more room
        private float[] GetColumnWidths(int columnCount)
        {
            float[] widths = new float[columnCount];
            for (int i = 0; i < columnCount; i++)
                widths[i] = 1f; // equal by default - tweak specific indexes if needed
            return widths;
        }

        protected void btnExportPPT_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = GetReportData();

                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (PresentationDocument presentationDocument = PresentationDocument.Create(memoryStream, PresentationDocumentType.Presentation))
                    {
                        PresentationPart presentationPart = presentationDocument.AddPresentationPart();
                        presentationPart.Presentation = new P.Presentation();

                        // SET PPT TO 16:9 WIDESCREEN
                        presentationPart.Presentation.SlideSize = new P.SlideSize
                        {
                            Cx = 12192000, // 13.333 inches
                            Cy = 6858000   // 7.5 inches
                        };

                        // CREATE SLIDE MASTER
                        SlideMasterPart slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>();
                        slideMasterPart.SlideMaster = CreateSlideMaster();

                        // CREATE SLIDE LAYOUT
                        SlideLayoutPart slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>();
                        slideLayoutPart.SlideLayout = CreateSlideLayout();

                        // CONNECT SLIDE MASTER TO PRESENTATION
                        presentationPart.Presentation.SlideMasterIdList =
                            new P.SlideMasterIdList(
                                new P.SlideMasterId
                                {
                                    Id = 1U,
                                    RelationshipId = presentationPart.GetIdOfPart(slideMasterPart)
                                });

                        // CONNECT SLIDE LAYOUT TO MASTER
                        slideMasterPart.SlideMaster.SlideLayoutIdList =
                            new P.SlideLayoutIdList(
                                new P.SlideLayoutId
                                {
                                    Id = 1U,
                                    RelationshipId = slideMasterPart.GetIdOfPart(slideLayoutPart)
                                });

                        // 7 DEVOTEES PER SLIDE

                        int recordsPerSlide = 6;
                        int totalSlides = (int)Math.Ceiling((double)dt.Rows.Count / recordsPerSlide);
                        List<SlidePart> slideParts = new List<SlidePart>();
                        for (int slideNumber = 0;
                             slideNumber < totalSlides;
                             slideNumber++)
                        {
                            SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();

                            slidePart.Slide =
                                new P.Slide(
                                    new P.CommonSlideData(
                                        new P.ShapeTree(
                                            new P.NonVisualGroupShapeProperties(
                                                new P.NonVisualDrawingProperties
                                                {
                                                    Id = 1U,
                                                    Name = ""
                                                },
                                                new P.NonVisualGroupShapeDrawingProperties(),
                                                new P.ApplicationNonVisualDrawingProperties()
                                            ),
                                            new P.GroupShapeProperties()
                                        )
                                    ),
                                    new P.ColorMapOverride(
                                        new A.MasterColorMapping()
                                    )
                                );

                            // Connect slide to layout
                            slidePart.AddPart(slideLayoutPart);

                            // Create title and table
                            CreateSevaSlide(slidePart, dt, slideNumber, recordsPerSlide);
                            SetSlideTransition(slidePart);
                            slideParts.Add(slidePart);
                        }

                        // CREATE SLIDE ID LIST
                        P.SlideIdList slideIdList = new P.SlideIdList();
                        uint slideId = 256U;
                        foreach (SlidePart slidePart in slideParts)
                        {
                            slideIdList.Append(
                                new P.SlideId
                                {
                                    Id = slideId++,
                                    RelationshipId = presentationPart.GetIdOfPart(slidePart)
                                });
                        }

                        presentationPart.Presentation.SlideIdList = slideIdList;

                        // ADD BACKGROUND MUSIC
                        //AddBackgroundMusic(presentationDocument,presentationPart,slideParts);
                        presentationPart.Presentation.Save();
                    }

                    // DOWNLOAD PPT

                    byte[] pptBytes = memoryStream.ToArray();

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();

                    Response.Buffer = true;

                    Response.ContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    Response.AddHeader("Content-Disposition", "attachment; filename=Todays_Seva_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pptx");
                    Response.OutputStream.Write(pptBytes, 0, pptBytes.Length);
                    Response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                // Add your logging here if required
                // lblMsg.Text = ex.Message;
            }
        }

        //private void AddBackgroundMusic( PresentationDocument presentationDocument,PresentationPart presentationPart, List<SlidePart> slideParts)
        //{
        //    if (slideParts == null || slideParts.Count == 0)
        //        return;

        //    string musicPath = Server.MapPath("~/assets/music/om_Dhun.mp3");

        //    if (!File.Exists(musicPath))
        //        return;


        //    // EMBED MP3
        //    // =========================================================

        //    MediaDataPart mediaDataPart =
        //        presentationDocument.CreateMediaDataPart(
        //            "audio/mp3",
        //            ".mp3");

        //    using (FileStream fs = new FileStream(
        //        musicPath,
        //        FileMode.Open,
        //        FileAccess.Read,
        //        FileShare.Read))
        //    {
        //        mediaDataPart.FeedData(fs);
        //    }

        //    // =========================================================
        //    // FIRST SLIDE
        //    // =========================================================

        //    SlidePart firstSlide = slideParts[0];

        //    string audioRelationshipId = "rIdBackgroundMusic";

        //    firstSlide.AddAudioReferenceRelationship(
        //        mediaDataPart,
        //        audioRelationshipId);

        //    // =========================================================
        //    // CREATE INVISIBLE AUDIO SHAPE
        //    // =========================================================

        //    P.ShapeTree shapeTree =
        //        firstSlide.Slide.CommonSlideData.ShapeTree;

        //    uint shapeId = GetNextShapeId(shapeTree);

        //    P.Shape audioShape = new P.Shape();

        //    P.NonVisualDrawingProperties drawingProperties =
        //        new P.NonVisualDrawingProperties
        //        {
        //            Id = shapeId,
        //            Name = "Background Music"
        //        };

        //    A.AudioFromFile audioFromFile =
        //        new A.AudioFromFile
        //        {
        //            Link = audioRelationshipId
        //        };

        //    P.ApplicationNonVisualDrawingProperties appProperties =
        //        new P.ApplicationNonVisualDrawingProperties();

        //    appProperties.Append(audioFromFile);

        //    P.NonVisualShapeProperties nonVisualProperties =
        //        new P.NonVisualShapeProperties(
        //            drawingProperties,
        //            new P.NonVisualShapeDrawingProperties(),
        //            appProperties);

        //    P.ShapeProperties shapeProperties =
        //        new P.ShapeProperties();

        //    shapeProperties.Transform2D =
        //        new A.Transform2D(
        //            new A.Offset
        //            {
        //                X = 0,
        //                Y = 0
        //            },
        //            new A.Extents
        //            {
        //                Cx = 1,
        //                Cy = 1
        //            });

        //    shapeProperties.Append(new A.NoFill());

        //    shapeProperties.Append(
        //        new A.Outline(new A.NoFill()));

        //    P.TextBody textBody =
        //        new P.TextBody(
        //            new A.BodyProperties(),
        //            new A.ListStyle(),
        //            new A.Paragraph());

        //    audioShape.Append(nonVisualProperties);
        //    audioShape.Append(shapeProperties);
        //    audioShape.Append(textBody);

        //    shapeTree.Append(audioShape);

        //    // =========================================================
        //    // CREATE TIMING
        //    // =========================================================

        //    P.Timing timing = new P.Timing();

        //    P.TimeNodeList timeNodeList =
        //        new P.TimeNodeList();

        //    P.ParallelTimeNode mainParallel =
        //        new P.ParallelTimeNode();

        //    P.CommonTimeNode mainTimeNode =
        //        new P.CommonTimeNode
        //        {
        //            Id = 1U,
        //            Duration = "indefinite",
        //            Fill = P.TimeNodeFillValues.Hold
        //        };

        //    // Start immediately
        //    P.StartConditionList mainStartConditions =
        //        new P.StartConditionList();

        //    mainStartConditions.Append(
        //        new P.Condition
        //        {
        //            Delay = "0"
        //        });

        //    mainTimeNode.Append(mainStartConditions);

        //    // =========================================================
        //    // AUDIO NODE
        //    // =========================================================

        //    P.Audio audio = new P.Audio();

        //    P.CommonMediaNode mediaNode =
        //        new P.CommonMediaNode
        //        {
        //            SlideCount = (uint)slideParts.Count,
        //            ShowWhenStopped = false
        //        };

        //    // Media timing node
        //    P.CommonTimeNode mediaTimeNode =
        //        new P.CommonTimeNode
        //        {
        //            Id = 2U,
        //            Duration = "indefinite",
        //            RepeatCount = "indefinite",
        //            Fill = P.TimeNodeFillValues.Hold
        //        };

        //    // Start audio immediately
        //    P.StartConditionList mediaStartConditions =
        //        new P.StartConditionList();

        //    mediaStartConditions.Append(
        //        new P.Condition
        //        {
        //            Delay = "0"
        //        });

        //    mediaTimeNode.Append(mediaStartConditions);

        //    // =========================================================
        //    // TARGET AUDIO SHAPE
        //    // =========================================================

        //    P.TargetElement targetElement =
        //        new P.TargetElement();

        //    P.ShapeTarget shapeTarget =
        //        new P.ShapeTarget
        //        {
        //            // IMPORTANT:
        //            // ShapeId is StringValue, not uint
        //            ShapeId = shapeId.ToString()
        //        };

        //    targetElement.Append(shapeTarget);

        //    // =========================================================
        //    // BUILD AUDIO NODE
        //    // =========================================================

        //    mediaNode.Append(mediaTimeNode);
        //    mediaNode.Append(targetElement);

        //    audio.Append(mediaNode);

        //    mainTimeNode.Append(audio);

        //    mainParallel.Append(mainTimeNode);

        //    timeNodeList.Append(mainParallel);

        //    timing.Append(timeNodeList);

        //    // =========================================================
        //    // REMOVE EXISTING TIMING
        //    // =========================================================

        //    P.Timing existingTiming =
        //        firstSlide.Slide.GetFirstChild<P.Timing>();

        //    if (existingTiming != null)
        //    {
        //        existingTiming.Remove();
        //    }

        //    // =========================================================
        //    // ADD TIMING
        //    // =========================================================

        //    firstSlide.Slide.Append(timing);

        //    // =========================================================
        //    // SAVE
        //    // =========================================================

        //    firstSlide.Slide.Save();
        //}

        private void CreateSevaSlide(SlidePart slidePart, DataTable dt, int slideNumber, int recordsPerSlide)
        {
            P.ShapeTree shapeTree = slidePart.Slide.CommonSlideData.ShapeTree;

            long bannerX = 0;
            long bannerY = 0;
            long bannerWidth = 12192000;
            long bannerHeight = 1600000; // Slightly increased height

            // 1. Draw banner background
            AddBannerBackground(shapeTree, bannerX, bannerY, bannerWidth, bannerHeight);


            // TABLE POSITION AND SIZE
            long tableX = 322000;
            long tableY = 1800000; // Moved down to accommodate larger banner
            long rowHeight = 650000;

            // COLUMN WIDTHS
            long slNoWidth = 900000;
            long nameWidth = 7000000;
            long addressWidth = 3500000;

            long totalTableWidth = slNoWidth + nameWidth + addressWidth;

            // TRUST COMMITTEE TITLE - WHITE TEXT

            AddTextBox(shapeTree, "SHRI SIDDHAROODHA SWAMIJI MATH TRUST COMMITTEE", tableX, 70000, totalTableWidth, 350000, 24, true, "FFFFFF");

            AddTextBox(shapeTree, "ಶ್ರೀ ಸಿದ್ಧಾರೂಢ ಸ್ವಾಮೀಜಿ ಮಠ ಟ್ರಸ್ಟ್ ಕಮಿಟಿ", tableX, 490000, totalTableWidth, 350000, 26, true, "FFFFFF", "kn-IN");

            // SELECTED SEVA TYPE TITLE - GOLD TEXT
            // SELECTED SEVA TYPE - ENGLISH

            string selectedSevaType = ddlSevaType.SelectedItem != null
                ? ddlSevaType.SelectedItem.Text
                : "";

            string sevaDate = DateTime.Parse(txtToDate.Text).ToString("dd-MM-yyyy");

            AddTextBox(shapeTree, $"{selectedSevaType} ({sevaDate})", tableX, 800000, totalTableWidth, 350000, 28, true, "C9A84C");

            // SELECTED SEVA TYPE - KANNADA

            string selectedSevaTypeKannada = "";

            switch (selectedSevaType)
            {
                case "Nirantara Abhishek":
                    selectedSevaTypeKannada = "ನಿರಂತರ ಅಭಿಷೇಕ";
                    break;

                case "Nirantara Annaprasad":
                    selectedSevaTypeKannada = "ನಿರಂತರ ಅನ್ನಪ್ರಸಾದ";
                    break;

                case "Other Sevas":
                    selectedSevaTypeKannada = "ಇತರೆ ಸೇವೆಗಳು";
                    break;

                default:
                    selectedSevaTypeKannada = selectedSevaType;
                    break;
            }

            AddTextBox(shapeTree, selectedSevaTypeKannada, tableX, 1220000, totalTableWidth, 300000, 26, true, "C9A84C", "kn-IN");

            // TABLE HEADER
            AddTableCell(
                        shapeTree,
                        "SL NO",
                        tableX,
                        tableY,
                        slNoWidth,
                        rowHeight,
                        true
                    );

            AddTableCell(
                shapeTree,
                "DEVOTEE NAME",
                tableX + slNoWidth,
                tableY,
                nameWidth,
                rowHeight,
                true
            );

            AddTableCell(
                shapeTree,
                "ADDRESS",
                tableX + slNoWidth + nameWidth,
                tableY,
                addressWidth,
                rowHeight,
                true
            );

            // GET RECORD RANGE

            int startIndex = slideNumber * recordsPerSlide;
            int endIndex = Math.Min(startIndex + recordsPerSlide, dt.Rows.Count);

            // DATA ROWS


            int currentRow = 1;

            for (int i = startIndex; i < endIndex; i++)
            {
                DataRow row = dt.Rows[i];
                long currentY = tableY + (rowHeight * currentRow);

                AddTableCell(
                    shapeTree,
                    (i + 1).ToString(),
                    tableX,
                    currentY,
                    slNoWidth,
                    rowHeight,
                    false
                );

                AddTableCell(
                    shapeTree,
                    Convert.ToString(row["DevoteeName"]),
                    tableX + slNoWidth,
                    currentY,
                    nameWidth,
                    rowHeight,
                    false
                );

                AddTableCell(
                    shapeTree,
                    Convert.ToString(row["Address"]),
                    tableX + slNoWidth + nameWidth,
                    currentY,
                    addressWidth,
                    rowHeight,
                    false
                );

                currentRow++;
            }

            // HEADER PHOTOS

            string leftPhotoPath = Server.MapPath(hfLeftImage.Value);
            string rightPhotoPath = Server.MapPath(hfRightImage.Value);

            System.Diagnostics.Debug.WriteLine(leftPhotoPath);
            System.Diagnostics.Debug.WriteLine(rightPhotoPath);

            if (File.Exists(leftPhotoPath))
            {
                AddImageToSlide(
                    slidePart,
                    leftPhotoPath,
                    120000,
                    100000,
                    1150000,
                    1150000
                );
            }

            if (File.Exists(rightPhotoPath))
            {
                AddImageToSlide(
                    slidePart,
                    rightPhotoPath,
                    10800000,
                    100000,
                    1150000,
                    1150000
                );
            }
        }

        private void AddImageToSlide(SlidePart slidePart, string imagePath, long x, long y, long width, long height)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            if (!File.Exists(imagePath))
                return;

            string extension = Path.GetExtension(imagePath).ToLowerInvariant();

            // OpenXML 3.x uses PartTypeInfo instead of the old ImagePartType enum
            PartTypeInfo imagePartType;

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    imagePartType = ImagePartType.Jpeg;
                    break;

                case ".png":
                    imagePartType = ImagePartType.Png;
                    break;

                case ".gif":
                    imagePartType = ImagePartType.Gif;
                    break;

                case ".bmp":
                    imagePartType = ImagePartType.Bmp;
                    break;

                default:
                    throw new Exception(
                        "Unsupported image type: " + extension);
            }

            ImagePart imagePart = slidePart.AddImagePart(imagePartType);

            using (FileStream fileStream = new FileStream(
                imagePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                imagePart.FeedData(fileStream);
            }

            string relationshipId =
                slidePart.GetIdOfPart(imagePart);

            P.ShapeTree shapeTree =
                slidePart.Slide.CommonSlideData.ShapeTree;

            // CREATE UNIQUE SHAPE ID

            uint shapeId =
                GetNextShapeId(shapeTree);

            // CREATE PICTURE

            P.Picture picture = new P.Picture();

            // NON-VISUAL PROPERTIES

            P.NonVisualPictureProperties
                nonVisualPictureProperties =
                new P.NonVisualPictureProperties(
                    new P.NonVisualDrawingProperties
                    {
                        Id = shapeId,
                        Name = "Picture " + shapeId
                    },

                    new P.NonVisualPictureDrawingProperties(
                        new A.PictureLocks
                        {
                            NoChangeAspect = false,
                            NoChangeArrowheads = true
                        }),

                    new P.ApplicationNonVisualDrawingProperties()
                );

            // IMAGE BLIP

            A.Blip blip =
               new A.Blip
               {
                   Embed = relationshipId
               };

            // STRETCH IMAGE

            A.Stretch stretch =
                new A.Stretch(
                    new A.FillRectangle());

            P.BlipFill blipFill =
                new P.BlipFill();

            blipFill.Append(blip);
            blipFill.Append(stretch);

            // IMAGE POSITION / SIZE

            A.Transform2D transform =
                new A.Transform2D(
                    new A.Offset
                    {
                        X = x,
                        Y = y
                    },
                    new A.Extents
                    {
                        Cx = width,
                        Cy = height
                    });

            A.PresetGeometry presetGeometry =
                new A.PresetGeometry(
                    new A.AdjustValueList())
                {
                    Preset =
                        A.ShapeTypeValues.Rectangle
                };

            P.ShapeProperties shapeProperties =
                new P.ShapeProperties();

            shapeProperties.Append(transform);
            shapeProperties.Append(presetGeometry);

            // ADD TO SLIDE

            picture.Append(nonVisualPictureProperties);

            picture.Append(blipFill);

            picture.Append(shapeProperties);

            shapeTree.Append(picture);

            slidePart.Slide.Save();
        }

        private void AddTableCell(P.ShapeTree shapeTree, string text, long x, long y, long width, long height, bool isHeader)
        {

            // CREATE UNIQUE SHAPE ID

            uint shapeId = GetNextShapeId(shapeTree);

            // CREATE SHAPE

            P.Shape shape = new P.Shape();

            // NON-VISUAL PROPERTIES

            P.NonVisualShapeProperties nonVisualShapeProperties =
                new P.NonVisualShapeProperties(
                    new P.NonVisualDrawingProperties
                    {
                        Id = shapeId,
                        Name = "Table Cell " + shapeId
                    },
                    new P.NonVisualShapeDrawingProperties(
                        new A.ShapeLocks
                        {
                            NoGrouping = true
                        }),
                    new P.ApplicationNonVisualDrawingProperties()
                );

            // SHAPE PROPERTIES

            P.ShapeProperties shapeProperties =
                new P.ShapeProperties();

            shapeProperties.Transform2D =
                new A.Transform2D(
                    new A.Offset
                    {
                        X = x,
                        Y = y
                    },
                    new A.Extents
                    {
                        Cx = width,
                        Cy = height
                    });

            // Rectangle
            shapeProperties.Append(
                new A.PresetGeometry(
                    new A.AdjustValueList())
                {
                    Preset = A.ShapeTypeValues.Rectangle
                });

            // BACKGROUND

            shapeProperties.Append(
                new A.SolidFill(
                    new A.RgbColorModelHex
                    {
                        Val = isHeader ? "2C5AA0" : "FFFFFF"
                    }));

            // BORDER

            shapeProperties.Append(
                new A.Outline(
                    new A.SolidFill(
                        new A.RgbColorModelHex
                        {
                            Val = "808080"
                        }))
                {
                    Width = 12700
                });

            // PRESENTATION TEXT BODY
            // IMPORTANT:
            // P.TextBody is required here

            P.TextBody textBody =
                new P.TextBody();

            // Body properties
            A.BodyProperties bodyProperties =
                new A.BodyProperties
                {
                    Anchor = A.TextAnchoringTypeValues.Center
                };

            // Add body properties
            textBody.Append(bodyProperties);

            // List style
            textBody.Append(
                new A.ListStyle());

            // PARAGRAPH

            A.Paragraph paragraph =
                new A.Paragraph();

            paragraph.ParagraphProperties =
                new A.ParagraphProperties
                {
                    Alignment =
                        A.TextAlignmentTypeValues.Center
                };

            // RUN
            A.Run run = new A.Run();

            run.RunProperties =
                new A.RunProperties
                {
                    // All table text = 20 pt and bold
                    FontSize = 2000,
                    Bold = true,
                    Language = "en-US"
                };

            // Make header text WHITE
            if (isHeader)
            {
                run.RunProperties.Append(
                    new A.SolidFill(
                        new A.RgbColorModelHex
                        {
                            Val = "FFFFFF"
                        }));
            }

            // TEXT
            run.Text =
                new A.Text(
                    text ?? string.Empty);

            // Add Run to Paragraph
            paragraph.Append(run);

            // Add Paragraph to TextBody
            textBody.Append(paragraph);

            // ADD EVERYTHING TO SHAPE

            shape.Append(
                nonVisualShapeProperties);

            shape.Append(
                shapeProperties);

            shape.Append(
                textBody);

            // ADD SHAPE TO SLIDE

            shapeTree.Append(shape);
        }

        private uint GetNextShapeId(P.ShapeTree shapeTree)
        {
            uint maxId = 1;

            foreach (P.NonVisualDrawingProperties properties
                in shapeTree
                    .Descendants<P.NonVisualDrawingProperties>())
            {
                if (properties.Id != null &&
                    properties.Id.Value > maxId)
                {
                    maxId = properties.Id.Value;
                }
            }

            return maxId + 1;
        }

        private void AddTextBox(
    P.ShapeTree shapeTree,
    string text,
    long x,
    long y,
    long width,
    long height,
    int fontSize,
    bool bold,
    string textColorHex = "0A1426",
             string language = "en-US")
        {
            uint shapeId = GetNextShapeId(shapeTree);
            P.Shape shape = new P.Shape();

            P.NonVisualShapeProperties nonVisualShapeProperties = new P.NonVisualShapeProperties(
                new P.NonVisualDrawingProperties { Id = shapeId, Name = "Text Box " + shapeId },
                new P.NonVisualShapeDrawingProperties(new A.ShapeLocks { NoGrouping = true }),
                new P.ApplicationNonVisualDrawingProperties()
            );

            P.ShapeProperties shapeProperties = new P.ShapeProperties();
            shapeProperties.Transform2D = new A.Transform2D(
                new A.Offset { X = x, Y = y },
                new A.Extents { Cx = width, Cy = height });

            shapeProperties.Append(new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle });
            shapeProperties.Append(new A.NoFill());
            shapeProperties.Append(new A.Outline(new A.NoFill()));

            P.TextBody textBody = new P.TextBody();
            textBody.Append(new A.BodyProperties { Anchor = A.TextAnchoringTypeValues.Center });
            textBody.Append(new A.ListStyle());

            A.Paragraph paragraph = new A.Paragraph();
            paragraph.ParagraphProperties = new A.ParagraphProperties { Alignment = A.TextAlignmentTypeValues.Center };

            A.Run run = new A.Run();

            run.RunProperties = new A.RunProperties
            {
                FontSize = fontSize * 100,
                Bold = bold,
                Language = language
            };

            run.RunProperties.Append(
                new A.SolidFill(
                    new A.RgbColorModelHex { Val = textColorHex }
                ));

            run.Text = new A.Text(text ?? string.Empty);
            paragraph.Append(run);
            textBody.Append(paragraph);

            shape.Append(nonVisualShapeProperties);
            shape.Append(shapeProperties);
            shape.Append(textBody);
            shapeTree.Append(shape);
        }

        private P.SlideMaster CreateSlideMaster()
        {
            return new P.SlideMaster(
                new P.CommonSlideData(
                    new P.ShapeTree(
                        new P.NonVisualGroupShapeProperties(
                            new P.NonVisualDrawingProperties
                            {
                                Id = 1U,
                                Name = ""
                            },
                            new P.NonVisualGroupShapeDrawingProperties(),
                            new P.ApplicationNonVisualDrawingProperties()
                        ),
                        new P.GroupShapeProperties()
                    )
                ),
                new P.ColorMap
                {
                    Background1 = A.ColorSchemeIndexValues.Light1,
                    Text1 = A.ColorSchemeIndexValues.Dark1,
                    Background2 = A.ColorSchemeIndexValues.Light2,
                    Text2 = A.ColorSchemeIndexValues.Dark2,
                    Accent1 = A.ColorSchemeIndexValues.Accent1,
                    Accent2 = A.ColorSchemeIndexValues.Accent2,
                    Accent3 = A.ColorSchemeIndexValues.Accent3,
                    Accent4 = A.ColorSchemeIndexValues.Accent4,
                    Accent5 = A.ColorSchemeIndexValues.Accent5,
                    Accent6 = A.ColorSchemeIndexValues.Accent6,
                    Hyperlink = A.ColorSchemeIndexValues.Hyperlink,
                    FollowedHyperlink = A.ColorSchemeIndexValues.FollowedHyperlink
                }
            );
        }

        private P.SlideLayout CreateSlideLayout()
        {
            return new P.SlideLayout(
                new P.CommonSlideData(
                    new P.ShapeTree(
                        new P.NonVisualGroupShapeProperties(
                            new P.NonVisualDrawingProperties
                            {
                                Id = 1U,
                                Name = ""
                            },
                            new P.NonVisualGroupShapeDrawingProperties(),
                            new P.ApplicationNonVisualDrawingProperties()
                        ),
                        new P.GroupShapeProperties()
                    )
                )
                {
                    Name = "Blank Layout"
                }
            );
        }

        private void SetSlideTransition(SlidePart slidePart)
        {
            // Automatically advance to the next slide after 5 seconds
            P.Transition transition = new P.Transition
            {
                AdvanceAfterTime = "5000",
                AdvanceOnClick = false
            };

            // Add transition to the Slide itself
            slidePart.Slide.Append(transition);

            // Save the slide
            slidePart.Slide.Save();
        }
        private void AddBannerBackground(
            P.ShapeTree shapeTree,
            long x,
            long y,
            long width,
            long height)
                {
                    uint shapeId = GetNextShapeId(shapeTree);
                    P.Shape shape = new P.Shape();

                    shape.Append(
                        new P.NonVisualShapeProperties(
                            new P.NonVisualDrawingProperties { Id = shapeId, Name = "Banner Header" },
                            new P.NonVisualShapeDrawingProperties(),
                            new P.ApplicationNonVisualDrawingProperties()));

                    P.ShapeProperties properties = new P.ShapeProperties();

                    properties.Transform2D = new A.Transform2D(
                        new A.Offset { X = x, Y = y },
                        new A.Extents { Cx = width, Cy = height });

                    properties.Append(new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle });

                    // FIXED GRADIENT - Using A.SchemeColor for better compatibility
                    A.GradientFill gradientFill = new A.GradientFill();

                    // Add the gradient stops with proper positioning
                    var stopList = new A.GradientStopList();

                    // Left edge - Dark Blue
                    stopList.Append(
                        new A.GradientStop(
                            new A.RgbColorModelHex { Val = "0A1E3D" }
                        )
                        { Position = 0 });

                    // Left-mid - Navy
                    stopList.Append(
                        new A.GradientStop(
                            new A.RgbColorModelHex { Val = "1A3A5C" }
                        )
                        { Position = 30000 });

                    // Center - Medium Blue (highlight)
                    stopList.Append(
                        new A.GradientStop(
                            new A.RgbColorModelHex { Val = "2C5F8A" }
                        )
                        { Position = 50000 });

                    // Right-mid - Navy
                    stopList.Append(
                        new A.GradientStop(
                            new A.RgbColorModelHex { Val = "1A3A5C" }
                        )
                        { Position = 70000 });

                    // Right edge - Dark Blue
                    stopList.Append(
                        new A.GradientStop(
                            new A.RgbColorModelHex { Val = "0A1E3D" }
                        )
                        { Position = 100000 });

                    gradientFill.Append(stopList);

                    // Set the gradient path (Linear)
                    gradientFill.Append(new A.LinearGradientFill { Angle = 0 });

                    properties.Append(gradientFill);

                    // ADD A SUBTLE DARK BORDER

                    A.Outline outline = new A.Outline(
                        new A.SolidFill(
                            new A.RgbColorModelHex { Val = "0A1E3D" }
                        ))
                    {
                        Width = 12700
                    };
                    properties.Append(outline);

                    shape.Append(properties);
                    shape.Append(new P.TextBody(new A.BodyProperties(), new A.ListStyle(), new A.Paragraph()));
                    shapeTree.Append(shape);

                    // GOLD ACCENT LINE AT BOTTOM

                    AddGoldAccentLine(shapeTree, x + 50000, y + height - 40000, width - 100000, 30000);
                }

        private void AddGoldAccentLine(
            P.ShapeTree shapeTree,
            long x,
            long y,
            long width,
            long height)
        {
            uint shapeId = GetNextShapeId(shapeTree);
            P.Shape shape = new P.Shape();

            shape.Append(
                new P.NonVisualShapeProperties(
                    new P.NonVisualDrawingProperties { Id = shapeId, Name = "Gold Line" },
                    new P.NonVisualShapeDrawingProperties(),
                    new P.ApplicationNonVisualDrawingProperties()));

            P.ShapeProperties properties = new P.ShapeProperties();

            properties.Transform2D = new A.Transform2D(
                new A.Offset { X = x, Y = y },
                new A.Extents { Cx = width, Cy = height });

            properties.Append(new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle });

            // Gold gradient
            A.GradientFill goldGradient = new A.GradientFill();
            var goldStops = new A.GradientStopList();

            goldStops.Append(
                new A.GradientStop(
                    new A.RgbColorModelHex { Val = "B8860B" } // Dark Goldenrod
                )
                { Position = 0 });

            goldStops.Append(
                new A.GradientStop(
                    new A.RgbColorModelHex { Val = "FFD700" } // Bright Gold
                )
                { Position = 50000 });

            goldStops.Append(
                new A.GradientStop(
                    new A.RgbColorModelHex { Val = "B8860B" } // Dark Goldenrod
                )
                { Position = 100000 });

            goldGradient.Append(goldStops);
            goldGradient.Append(new A.LinearGradientFill { Angle = 0 });

            properties.Append(goldGradient);
            properties.Append(new A.Outline(new A.NoFill()));

            shape.Append(properties);
            shape.Append(new P.TextBody(new A.BodyProperties(), new A.ListStyle(), new A.Paragraph()));
            shapeTree.Append(shape);
        }
        protected void btnImportExcel_Click(object sender, EventArgs e)
        {
            lblImportMessage.Text = "";

            if (!fuExcel.HasFile)
            {
                lblImportMessage.Text = "Please select an Excel file.";
                lblImportMessage.CssClass = "text-danger mt-3 d-inline-block";
                return;
            }

            string extension = Path.GetExtension(fuExcel.FileName).ToLowerInvariant();

            if (extension != ".xlsx" && extension != ".xls")
            {
                lblImportMessage.Text = "Please upload an Excel file (.xlsx or .xls).";
                lblImportMessage.CssClass = "text-danger mt-3 d-inline-block";
                return;
            }

            if (extension == ".xls")
            {
                lblImportMessage.Text =
                    "Please save your Excel file as .xlsx and upload it again. ClosedXML supports .xlsx files.";
                lblImportMessage.CssClass = "text-danger mt-3 d-inline-block";
                return;
            }

            int importedCount = 0;
            List<string> errors = new List<string>();

            DataTable importedRecords = new DataTable();

            importedRecords.Columns.Add("SerialNo");
            importedRecords.Columns.Add("RegisterNo");
            importedRecords.Columns.Add("DevoteeName");
            importedRecords.Columns.Add("City");
            importedRecords.Columns.Add("ContactNumber");
            importedRecords.Columns.Add("SevaType");
            importedRecords.Columns.Add("EntryDate", typeof(DateTime));
            importedRecords.Columns.Add("SevaFor");
            importedRecords.Columns.Add("EventName");

            string connStr =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (XLWorkbook workbook = new XLWorkbook(fuExcel.PostedFile.InputStream))
                {
                    IXLWorksheet worksheet = workbook.Worksheet(1);

                    int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

                    if (lastRow < 2)
                    {
                        lblImportMessage.Text = "The Excel file has no data rows.";
                        lblImportMessage.CssClass = "text-danger mt-3 d-inline-block";
                        return;
                    }

                    // Read and validate Excel column headings.
                    Dictionary<string, int> columns =
                        new Dictionary<string, int>(
                            StringComparer.OrdinalIgnoreCase);

                    int lastColumn =
                        worksheet.Row(1).LastCellUsed()?.Address.ColumnNumber ?? 0;

                    for (int col = 1; col <= lastColumn; col++)
                    {
                        string heading = worksheet.Cell(1, col)
                            .GetFormattedString()
                            .Trim();

                        // Remove spaces, dots, underscores and make uppercase
                        string normalized = heading
                            .Replace(".", "")
                            .Replace(" ", "")
                            .Replace("_", "")
                            .ToUpperInvariant();

                        switch (normalized)
                        {
                            case "SLNO":
                            case "SERIALNO":
                            case "SNO":
                                columns["SerialNo"] = col;
                                break;

                            case "DEVOTEENAME":
                                columns["Devotee Name"] = col;
                                break;

                            case "CITY":
                                columns["City"] = col;
                                break;

                            case "PHONENUMBER":
                            case "CONTACTNUMBER":
                            case "MOBILE":
                                columns["Contact Number"] = col;
                                break;

                            case "SEVATYPE":
                                columns["Seva Type"] = col;
                                break;

                            case "ENTRYDATE":
                            case "DATE":
                                columns["Entry Date"] = col;
                                break;

                            case "SEVAFOR":
                                columns["SevaFor"] = col;
                                break;

                            case "EVENTNAME":
                            case "EVENT":
                                columns["EventName"] = col;
                                break;
                        }
                    }

                    // Required Excel columns
                    string[] requiredHeaders =
                    {
                        "Devotee Name",
                        "City",
                        "Contact Number",
                        "Seva Type",
                        "Entry Date",
                        "SevaFor",
                        "EventName"
                    };

                    foreach (string header in requiredHeaders)
                    {
                        if (!columns.ContainsKey(header))
                        {
                            lblImportMessage.Text =
                                "Missing Excel column: " + header;

                            lblImportMessage.CssClass =
                                "text-danger mt-3 d-inline-block";

                            return;
                        }
                    }

                    foreach (string header in requiredHeaders)
                    {
                        if (!columns.ContainsKey(header))
                        {
                            lblImportMessage.Text =
                                "Missing Excel column: " + header;

                            lblImportMessage.CssClass =
                                "text-danger mt-3 d-inline-block";

                            return;
                        }
                    }

                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();

                        using (SqlTransaction transaction = con.BeginTransaction())
                        {
                            int nextSerialNo = 1;

                            // Get the next RegisterNo for each SevaFor separately
                            int nextS = GetNextRegisterNumber("S", con, transaction);
                            int nextG = GetNextRegisterNumber("G", con, transaction);
                            int nextD = GetNextRegisterNumber("D", con, transaction);


                            string serialQuery = @"
                                SELECT ISNULL(
                                    MAX(TRY_CONVERT(INT, NULLIF(LTRIM(RTRIM(SerialNo)), ''))),0) + 1
                                FROM SMSF.dbo.SevaMemberDetails";

                            using (SqlCommand serialCmd =
                                new SqlCommand(serialQuery, con, transaction))
                            {
                                nextSerialNo = Convert.ToInt32(serialCmd.ExecuteScalar());
                            }
                            try
                            {
                                // Load Seva Type IDs and names
                                Dictionary<string, string> sevaTypes =
                                    new Dictionary<string, string>(
                                        StringComparer.OrdinalIgnoreCase);

                                string sevaQuery =
                                    "SELECT id, name FROM SMSF.dbo.Seva_Type";

                                using (SqlCommand sevaCmd =
                                    new SqlCommand(sevaQuery, con, transaction))
                                {
                                    using (SqlDataReader reader = sevaCmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string sevaId =
                                                reader["id"].ToString().Trim();

                                            string sevaName =
                                                reader["name"].ToString().Trim();

                                            // Map the ID, for example "1" -> "1"
                                            if (!sevaTypes.ContainsKey(sevaId))
                                            {
                                                sevaTypes.Add(sevaId, sevaId);
                                            }

                                            // Map the name, for example "Nirantara Abhishek" -> "1"
                                            if (!string.IsNullOrWhiteSpace(sevaName) &&
                                                !sevaTypes.ContainsKey(sevaName))
                                            {
                                                sevaTypes.Add(sevaName, sevaId);
                                            }
                                        }
                                    }
                                }

                                // Prepare the insert command.
                                string insertQuery = @"
                                    INSERT INTO SMSF.dbo.SevaMemberDetails
                                    (
                                        SerialNo, RegisterNo, DevoteeName, City,
                                        ContactNumber, SevaType, EntryDate, SevaFor, EventName
                                    )
                                    VALUES
                                    (
                                        @SerialNo, @RegisterNo, @DevoteeName, @City,
                                        @ContactNumber, @SevaType,
                                        CONVERT(datetime, @EntryDate, 105),
                                        @SevaFor, @EventName
                                    )";

                                using (SqlCommand cmd = new SqlCommand(insertQuery, con, transaction))
                                {
                                    cmd.Parameters.Add("@SerialNo", SqlDbType.NVarChar, 50);
                                    cmd.Parameters.Add("@RegisterNo", SqlDbType.NVarChar, 50);
                                    cmd.Parameters.Add("@DevoteeName", SqlDbType.NVarChar, 200);
                                    cmd.Parameters.Add("@City", SqlDbType.NVarChar, 100);
                                    cmd.Parameters.Add("@ContactNumber", SqlDbType.NVarChar, 50);
                                    cmd.Parameters.Add("@SevaType", SqlDbType.NVarChar, 50);
                                    cmd.Parameters.Add("@EntryDate", SqlDbType.DateTime);
                                    cmd.Parameters.Add("@SevaFor", SqlDbType.NVarChar, 100);
                                    cmd.Parameters.Add("@EventName", SqlDbType.NVarChar, 200);

                                    // Start from row 2 because row 1 contains headers.
                                    for (int row = 2; row <= lastRow; row++)
                                    {
                                        string devoteeName = worksheet.Cell(row, columns["Devotee Name"]).GetFormattedString().Trim();
                                        string city = worksheet.Cell(row, columns["City"]).GetFormattedString().Trim();
                                        string phone = worksheet.Cell(row, columns["Contact Number"]).GetFormattedString().Trim();
                                        string sevaName = worksheet.Cell(row, columns["Seva Type"]).GetFormattedString().Trim();
                                        string dateText = worksheet.Cell(row, columns["Entry Date"]).GetFormattedString().Trim();
                                        string sevaFor = worksheet.Cell(row, columns["SevaFor"]).GetFormattedString().Trim();
                                        string eventName = worksheet.Cell(row, columns["EventName"]).GetFormattedString().Trim();
                                        // Convert Excel SevaFor value to code
                                        string sevaCode = "";

                                        if (sevaFor.Equals("Siddharood Swami", StringComparison.OrdinalIgnoreCase))
                                        {
                                            sevaCode = "S";
                                        }
                                        else if (sevaFor.Equals("Gurunathrood Swami", StringComparison.OrdinalIgnoreCase))
                                        {
                                            sevaCode = "G";
                                        }
                                        else if (sevaFor.Equals("Dasoha", StringComparison.OrdinalIgnoreCase))
                                        {
                                            sevaCode = "D";
                                        }
                                        else
                                        {
                                            // If Excel already contains S / G / D
                                            sevaCode = sevaFor.ToUpperInvariant();
                                        }

                                        string registerNo = "";

                                        if (sevaCode == "S")
                                        {
                                            registerNo = "S-" + nextS.ToString("D3");
                                        }
                                        else if (sevaCode == "G")
                                        {
                                            registerNo = "G-" + nextG.ToString("D3");
                                        }
                                        else if (sevaCode == "D")
                                        {
                                            registerNo = "D-" + nextD.ToString("D3");
                                        }
                                        else
                                        {
                                            errors.Add("Row " + row + ": Invalid SevaFor '" + sevaFor + "'.");
                                            continue;
                                        }
                                        // Ignore completely empty rows.
                                        if (string.IsNullOrWhiteSpace(devoteeName) &&
                                            string.IsNullOrWhiteSpace(city) &&
                                            string.IsNullOrWhiteSpace(phone) &&
                                            string.IsNullOrWhiteSpace(sevaName) &&
                                            string.IsNullOrWhiteSpace(dateText))
                                        {
                                            continue;
                                        }

                                        // Validate mandatory fields.
                                        if (string.IsNullOrWhiteSpace(devoteeName))
                                        {
                                            errors.Add("Row " + row + ": Devotee Name is empty.");
                                            continue;
                                        }

                                        if (string.IsNullOrWhiteSpace(sevaName))
                                        {
                                            errors.Add("Row " + row + ": Seva Type is empty.");
                                            continue;
                                        }

                                        // Match the Excel Seva Type name to its database ID.
                                        string sevaId;

                                        if (!sevaTypes.TryGetValue(sevaName, out sevaId))
                                        {
                                            errors.Add("Row " + row + ": Seva Type '" + sevaName + "' was not found in Seva_Type.");

                                            continue;
                                        }

                                        // Parse Entry Date.
                                        DateTime entryDate;

                                        if (!DateTime.TryParse(
                                            dateText,
                                            CultureInfo.CurrentCulture,
                                            DateTimeStyles.None,
                                            out entryDate) &&
                                            !DateTime.TryParse(
                                            dateText,
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None,
                                            out entryDate))
                                        {
                                            errors.Add("Row " + row + ": Invalid Entry Date '" + dateText + "'.");
                                            continue;
                                        }

                                        cmd.Parameters["@SerialNo"].Value = nextSerialNo.ToString();
                                        cmd.Parameters["@RegisterNo"].Value = registerNo;
                                        cmd.Parameters["@DevoteeName"].Value = devoteeName;
                                        cmd.Parameters["@City"].Value = string.IsNullOrWhiteSpace(city) ? (object)DBNull.Value : city;
                                        cmd.Parameters["@ContactNumber"].Value = string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone;

                                        // Store the Seva Type database ID as text
                                        cmd.Parameters["@SevaType"].Value = sevaId.ToString();
                                        cmd.Parameters["@EntryDate"].Value = entryDate;
                                        cmd.Parameters["@SevaFor"].Value = sevaCode;
                                        cmd.Parameters["@EventName"].Value = string.IsNullOrWhiteSpace(eventName) ? (object)DBNull.Value : eventName;

                                        try
                                        {
                                            // Execute the database insert
                                            cmd.ExecuteNonQuery();

                                            // Insert succeeded
                                            importedCount++;
                                            if (sevaCode == "S")
                                            {
                                                nextS++;
                                            }
                                            else if (sevaCode == "G")
                                            {
                                                nextG++;
                                            }
                                            else if (sevaCode == "D")
                                            {
                                                nextD++;
                                            }

                                            // Store the successfully imported row
                                            importedRecords.Rows.Add(
                                                nextSerialNo.ToString(),
                                                registerNo,
                                                devoteeName,
                                                city,
                                                phone,
                                                sevaId,
                                                entryDate,
                                                sevaFor,
                                                eventName
                                            );

                                            // Increment only after a successful insert
                                            nextSerialNo++;
                                        }
                                        catch (SqlException ex)
                                        {
                                            errors.Add("Row " + row + ": Database error - " + ex.Message);
                                        }
                                    }
                                }
                                // Commit valid rows. Invalid rows are reported below.
                                transaction.Commit();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }

                if (errors.Count == 0)
                {
                    lblImportMessage.Text = "Successfully imported " + importedCount + " records into the database.";
                    lblImportMessage.CssClass = "text-success mt-3 d-inline-block fw-bold";
                }
                else
                {
                    lblImportMessage.Text =
                        "Imported " + importedCount +
                        " records. " + errors.Count +
                        " row(s) could not be imported:<br/>" +
                        string.Join("<br/>", errors);

                    lblImportMessage.CssClass = "text-danger mt-3 d-inline-block";
                }
                // Display only the records imported from this Excel file
                BindReportGrid(importedRecords);
            }
            catch (Exception ex)
            {
                lblImportMessage.Text = "Import failed: " + Server.HtmlEncode(ex.Message);

                lblImportMessage.CssClass = "text-danger mt-3 d-inline-block";
            }
        }


        protected void btnUpdateDates_Click(object sender, EventArgs e)
        {
            lblUpdateMessage.Text = "";
            lblImportMessage.Text = "";

            DateTime fromDate;
            DateTime toDate;

            // 1. Validate selected dates
            if (!DateTime.TryParseExact(
                    txtFromDate.Text,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fromDate) ||
                !DateTime.TryParseExact(
                    txtToDate.Text,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out toDate))
            {
                lblUpdateMessage.Text = "Please select valid dates.";
                lblUpdateMessage.CssClass = "text-danger fw-bold";
                return;
            }

            fromDate = fromDate.Date;
            toDate = toDate.Date;

            if (fromDate > toDate)
            {
                lblUpdateMessage.Text = "From Date cannot be greater than To Date.";
                lblUpdateMessage.CssClass = "text-danger fw-bold";
                return;
            }

            int currentYear = DateTime.Today.Year;

            // Validate conversion to current year
            DateTime newFromDate;
            DateTime newToDate;

            try
            {
                newFromDate = new DateTime(currentYear, fromDate.Month, fromDate.Day);

                newToDate = new DateTime(currentYear, toDate.Month, toDate.Day);
            }
            catch (ArgumentOutOfRangeException)
            {
                lblUpdateMessage.Text = "The selected dates cannot be converted to the current year.";
                lblUpdateMessage.CssClass = "text-danger fw-bold";
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                // This DataTable holds the actual updated records
                DataTable updatedRecords = new DataTable();

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"

                DECLARE @UpdatedRecords TABLE
                (
                    SerialNo NVARCHAR(50),
                    MemberID NVARCHAR(50),
                    DevoteeName NVARCHAR(200),
                    SevaType NVARCHAR(100),
                    City NVARCHAR(100),
                    OldEntryDate DATETIME,
                    NewEntryDate DATETIME
                );

                UPDATE SMSF.dbo.SevaMemberDetails

                SET EntryDate = DATEADD(
                    YEAR,
                    @CurrentYear - YEAR(EntryDate),
                    EntryDate
                )

                OUTPUT
                    INSERTED.SerialNo,
                    CONVERT(NVARCHAR(50), INSERTED.MemberID),
                    INSERTED.DevoteeName,
                    INSERTED.SevaType,
                    INSERTED.City,
                    DELETED.EntryDate,
                    INSERTED.EntryDate

                INTO @UpdatedRecords
                (
                    SerialNo, MemberID,DevoteeName,SevaType,City,OldEntryDate,NewEntryDate
                )

                WHERE EntryDate >= @FromDate
                  AND EntryDate < @NextDayToDate
                  AND YEAR(EntryDate) <> @CurrentYear;

                SELECT
                    SerialNo, MemberID,DevoteeName,SevaType,City,OldEntryDate,NewEntryDate

                FROM @UpdatedRecords
                ORDER BY NewEntryDate, SerialNo;
            ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@CurrentYear", SqlDbType.Int).Value = currentYear;
                        cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate;
                        cmd.Parameters.Add("@NextDayToDate", SqlDbType.DateTime).Value = toDate.AddDays(1);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(updatedRecords);
                        }
                    }
                }

                // Number of records actually updated
                int rowsUpdated = updatedRecords.Rows.Count;

                // Update date fields on the page
                txtFromDate.Text = newFromDate.ToString("yyyy-MM-dd");
                txtToDate.Text = newToDate.ToString("yyyy-MM-dd");

                // 2. Bind only the updated records
                BindReportGrid(updatedRecords);

                // 3. Display update message
                lblUpdateMessage.Text = rowsUpdated + " record(s) updated to " + currentYear + ". Showing updated records.";
                lblUpdateMessage.CssClass = "text-success mt-3 d-inline-block fw-bold";
            }
            catch (Exception ex)
            {
                lblUpdateMessage.Text = "Update failed: " + Server.HtmlEncode(ex.Message);
                lblUpdateMessage.CssClass = "text-danger mt-3 d-inline-block fw-bold";
            }
        }

        private void BindReportGrid(DataTable dt)
        {
            gvReport.DataSource = dt;
            gvReport.DataBind();
        }
        private int GetNextRegisterNumber(string sevaCode, SqlConnection con, SqlTransaction transaction)
        {
            string query = @"
        SELECT ISNULL(
            MAX(
                CASE
                    -- Old format: 1, 2, 3...
                    WHEN RegisterNo NOT LIKE '%-%'
                    THEN TRY_CAST(RegisterNo AS INT)

                    -- New format: S-001, S-002...
                    WHEN RegisterNo LIKE @SevaCode + '-%'
                    THEN TRY_CAST(
                        SUBSTRING(
                            RegisterNo,
                            LEN(@SevaCode) + 2,
                            50
                        ) AS INT
                    )

                    ELSE NULL
                END
            ),
            0
        ) + 1
        FROM SMSF.dbo.SevaMemberDetails
        WHERE SevaFor = @SevaCode;";

            using (SqlCommand cmd = new SqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@SevaCode", sevaCode);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

    }
}