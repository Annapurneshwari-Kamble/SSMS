using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Kannada.AsciiUnicode.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace SSMS.Pages.Admin
{
    public partial class ExcelDataPreview : System.Web.UI.Page
    {
        private const string SessionKey = "ExcelPreviewData";

        private static readonly KannadaConverter KannadaTextConverter =
            KannadaConverter.Instance;

        // Header photos (loaded once from the hidden fields)
        private byte[] _leftImageBytes;
        private string _leftImageExt;
        private byte[] _rightImageBytes;
        private string _rightImageExt;

        // Seva info for the PPT
        private string _sevaType;
        private string _sevaDate;

        // =====================================================
        // IMPORT EXCEL
        // =====================================================
        protected void btnImport_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            pnlPreview.Visible = false;
            Session[SessionKey] = null;

            if (!fuExcel.HasFile)
            {
                ShowMessage("Please choose an Excel file.", "warning");
                return;
            }

            string extension =
                Path.GetExtension(fuExcel.FileName).ToLowerInvariant();

            if (extension != ".xlsx")
            {
                ShowMessage("Please upload an .xlsx Excel file.", "warning");
                return;
            }

            if (fuExcel.PostedFile.ContentLength > 10 * 1024 * 1024)
            {
                ShowMessage("The file must be smaller than 10 MB.", "warning");
                return;
            }

            try
            {
                using (Stream stream = fuExcel.PostedFile.InputStream)
                using (XLWorkbook workbook = new XLWorkbook(stream))
                {
                    if (!workbook.Worksheets.Any())
                    {
                        ShowMessage("The workbook contains no worksheets.", "warning");
                        return;
                    }

                    IXLWorksheet worksheet = workbook.Worksheet(1);
                    IXLRange usedRange = worksheet.RangeUsed();

                    if (usedRange == null)
                    {
                        ShowMessage("The selected worksheet is empty.", "warning");
                        return;
                    }

                    int firstRow = usedRange.FirstRow().RowNumber();
                    int lastRow = usedRange.LastRow().RowNumber();
                    int firstCol = usedRange.FirstColumn().ColumnNumber();
                    int lastCol = usedRange.LastColumn().ColumnNumber();

                    DataTable dt = new DataTable();
                    dt.TableName = "ExcelPreview";

                    // Read and convert column headings.
                    for (int col = firstCol; col <= lastCol; col++)
                    {
                        string header = worksheet
                            .Cell(firstRow, col)
                            .GetFormattedString()
                            .Trim();

                        header = ConvertLegacyKannadaText(header);

                        if (string.IsNullOrWhiteSpace(header))
                            header = "Column" + col;

                        string originalHeader = header;
                        int suffix = 2;

                        while (dt.Columns.Contains(header))
                        {
                            header = originalHeader + "_" + suffix;
                            suffix++;
                        }

                        dt.Columns.Add(header, typeof(string));
                    }

                    // Read and convert data rows.
                    for (int row = firstRow + 1; row <= lastRow; row++)
                    {
                        DataRow dataRow = dt.NewRow();
                        bool hasData = false;

                        for (int col = firstCol; col <= lastCol; col++)
                        {
                            string value = worksheet
                                .Cell(row, col)
                                .GetFormattedString()
                                .Trim();

                            value = ConvertLegacyKannadaText(value);

                            dataRow[col - firstCol] = value;

                            if (!string.IsNullOrWhiteSpace(value))
                                hasData = true;
                        }

                        if (hasData)
                            dt.Rows.Add(dataRow);
                    }

                    // Keep the list for the "Generate PPT" postback
                    Session[SessionKey] = dt;

                    gvExcelData.DataSource = dt;
                    gvExcelData.DataBind();

                    if (string.IsNullOrWhiteSpace(txtToDate.Text))
                        txtToDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

                    pnlPreview.Visible = true;
                    lblRecordCount.Text = dt.Rows.Count + " records";

                    ShowMessage(
                        "Excel file loaded successfully. " +
                        "No data was saved to the database.",
                        "success");
                }
            }
            catch (Exception)
            {
                ShowMessage(
                    "Unable to read or convert this Excel file. " +
                    "Please verify that it is a valid .xlsx workbook " +
                    "and uses a supported Nudi/Baraha encoding.",
                    "danger");
            }
        }

        private string ConvertLegacyKannadaText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            // Do not convert text that is already Kannada Unicode.
            if (value.Any(ch => ch >= '\u0C80' && ch <= '\u0CFF'))
                return value;

            // Detect common characters found in legacy Nudi text.
            const string legacyMarkers =
                "ÀÁÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßªº£";

            bool looksLikeLegacyKannada =
                value.Any(ch => legacyMarkers.IndexOf(ch) >= 0);

            if (!looksLikeLegacyKannada)
                return value;

            // true keeps digits such as phone numbers in English format.
            return KannadaTextConverter.ConvertAsciiToUnicode(
                value, convertToEnglishDigit: true);
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.CssClass =
                "alert alert-" + type + " d-block mt-3 mb-0";

            lblMessage.Text = HttpUtility.HtmlEncode(message);
        }

        // =====================================================
        // FIND NAME / PLACE COLUMNS AUTOMATICALLY
        // =====================================================
        private int FindColumnIndex(DataTable dt, string[] keywords)
        {
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string columnName = dt.Columns[i].ColumnName;

                foreach (string key in keywords)
                {
                    if (columnName.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                        return i;
                }
            }

            return -1;
        }

        private void DetectColumns(DataTable source, out int nameIndex, out int placeIndex)
        {
            string[] nameKeys = { "name", "devotee", "ಹೆಸರು", "ಭಕ್ತ" };
            string[] placeKeys = { "place", "address", "city", "village", "ಊರು", "ವಿಳಾಸ", "ಸ್ಥಳ" };

            nameIndex = FindColumnIndex(source, nameKeys);
            placeIndex = FindColumnIndex(source, placeKeys);

            int count = source.Columns.Count;

            // Fallback: first column = Excel serial (ignored),
            // second = name, third = place
            if (nameIndex < 0)
                nameIndex = count >= 3 ? 1 : 0;

            if (placeIndex < 0 || placeIndex == nameIndex)
            {
                if (count >= 3)
                    placeIndex = 2;
                else if (count == 2)
                    placeIndex = nameIndex == 0 ? 1 : 0;
                else
                    placeIndex = nameIndex;
            }
        }

        // Build table: SlNo (1,2,3...), Name, Place from the imported list
        private DataTable BuildReportTable(DataTable source)
        {
            int nameIndex, placeIndex;
            DetectColumns(source, out nameIndex, out placeIndex);

            DataTable report = new DataTable();
            report.Columns.Add("SlNo", typeof(int));
            report.Columns.Add("DevoteeName", typeof(string));
            report.Columns.Add("Address", typeof(string));

            int serial = 1;

            foreach (DataRow row in source.Rows)
            {
                string name = Convert.ToString(row[nameIndex]).Trim();
                string place = placeIndex == nameIndex
                    ? ""
                    : Convert.ToString(row[placeIndex]).Trim();

                if (string.IsNullOrWhiteSpace(name) &&
                    string.IsNullOrWhiteSpace(place))
                    continue;

                report.Rows.Add(serial++, name, place);
            }

            return report;
        }

        // =====================================================
        // LOAD HEADER PHOTOS FROM THE HIDDEN FIELDS
        // =====================================================
        private void LoadHeaderImages()
        {
            ReadImageFromPath(hfLeftImage.Value,
                out _leftImageBytes, out _leftImageExt);

            ReadImageFromPath(hfRightImage.Value,
                out _rightImageBytes, out _rightImageExt);
        }

        private void ReadImageFromPath(
            string virtualPath,
            out byte[] bytes,
            out string extension)
        {
            bytes = null;
            extension = null;

            if (string.IsNullOrWhiteSpace(virtualPath))
                return;

            string physicalPath = Server.MapPath(virtualPath);

            System.Diagnostics.Debug.WriteLine(physicalPath);

            if (!File.Exists(physicalPath))
                return;

            extension = Path.GetExtension(physicalPath).ToLowerInvariant();
            bytes = File.ReadAllBytes(physicalPath);
        }

        // =====================================================
        // GENERATE PPT FROM IMPORTED LIST
        // =====================================================
        protected void btnExportPPT_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable source = Session[SessionKey] as DataTable;

                if (source == null || source.Rows.Count == 0)
                {
                    pnlPreview.Visible = false;
                    ShowMessage("Please import an Excel file first.", "warning");
                    return;
                }

                DateTime parsedDate;
                if (!DateTime.TryParse(txtToDate.Text, out parsedDate))
                {
                    ShowMessage("Please select a valid Seva date.", "warning");
                    return;
                }

                // Rebuild the grid so the preview stays after postback
                gvExcelData.DataSource = source;
                gvExcelData.DataBind();

                _sevaType = ddlSevaType.SelectedItem != null
                    ? ddlSevaType.SelectedItem.Text
                    : "";
                _sevaDate = parsedDate.ToString("dd-MM-yyyy");

                DataTable dt = BuildReportTable(source);

                if (dt.Rows.Count == 0)
                {
                    ShowMessage("No records available to generate the PPT.", "warning");
                    return;
                }

                LoadHeaderImages();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (PresentationDocument presentationDocument =
                        PresentationDocument.Create(
                            memoryStream,
                            PresentationDocumentType.Presentation))
                    {
                        PresentationPart presentationPart =
                            presentationDocument.AddPresentationPart();
                        presentationPart.Presentation = new P.Presentation();

                        // 16:9 WIDESCREEN
                        presentationPart.Presentation.SlideSize = new P.SlideSize
                        {
                            Cx = 12192000,
                            Cy = 6858000
                        };

                        // SLIDE MASTER
                        SlideMasterPart slideMasterPart =
                            presentationPart.AddNewPart<SlideMasterPart>();
                        slideMasterPart.SlideMaster = CreateSlideMaster();

                        // SLIDE LAYOUT
                        SlideLayoutPart slideLayoutPart =
                            slideMasterPart.AddNewPart<SlideLayoutPart>();
                        slideLayoutPart.SlideLayout = CreateSlideLayout();

                        // CONNECT MASTER TO PRESENTATION
                        presentationPart.Presentation.SlideMasterIdList =
                            new P.SlideMasterIdList(
                                new P.SlideMasterId
                                {
                                    Id = 2147483648U,
                                    RelationshipId =
                                        presentationPart.GetIdOfPart(slideMasterPart)
                                });

                        // CONNECT LAYOUT TO MASTER
                        slideMasterPart.SlideMaster.SlideLayoutIdList =
                            new P.SlideLayoutIdList(
                                new P.SlideLayoutId
                                {
                                    Id = 2147483649U,
                                    RelationshipId =
                                        slideMasterPart.GetIdOfPart(slideLayoutPart)
                                });

                        // 6 DEVOTEES PER SLIDE
                        int recordsPerSlide = 6;
                        int totalSlides = (int)Math.Ceiling(
                            (double)dt.Rows.Count / recordsPerSlide);

                        List<SlidePart> slideParts = new List<SlidePart>();

                        for (int slideNumber = 0; slideNumber < totalSlides; slideNumber++)
                        {
                            SlidePart slidePart =
                                presentationPart.AddNewPart<SlidePart>();

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

                            CreateSevaSlide(slidePart, dt, slideNumber, recordsPerSlide);
                            SetSlideTransition(slidePart);
                            slideParts.Add(slidePart);
                        }

                        // SLIDE ID LIST
                        P.SlideIdList slideIdList = new P.SlideIdList();
                        uint slideId = 256U;

                        foreach (SlidePart slidePart in slideParts)
                        {
                            slideIdList.Append(
                                new P.SlideId
                                {
                                    Id = slideId++,
                                    RelationshipId =
                                        presentationPart.GetIdOfPart(slidePart)
                                });
                        }

                        presentationPart.Presentation.SlideIdList = slideIdList;
                        presentationPart.Presentation.Save();
                    }

                    // DOWNLOAD PPT
                    byte[] pptBytes = memoryStream.ToArray();

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.Buffer = true;

                    Response.ContentType =
                        "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    Response.AddHeader(
                        "Content-Disposition",
                        "attachment; filename=Todays_Seva_" +
                        DateTime.Now.ToString("yyyyMMddHHmmss") + ".pptx");
                    Response.AddHeader("Content-Length", pptBytes.Length.ToString());

                    Response.OutputStream.Write(pptBytes, 0, pptBytes.Length);
                    Response.Flush();

                    // Stop the page from appending its own HTML to the file
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Unable to generate the PPT: " + ex.Message, "danger");
            }
        }

        // =====================================================
        // SLIDE CREATION
        // =====================================================
        private void CreateSevaSlide(
            SlidePart slidePart,
            DataTable dt,
            int slideNumber,
            int recordsPerSlide)
        {
            P.ShapeTree shapeTree = slidePart.Slide.CommonSlideData.ShapeTree;

            long bannerX = 0;
            long bannerY = 0;
            long bannerWidth = 12192000;
            long bannerHeight = 1600000;

            // 1. Banner background
            AddBannerBackground(shapeTree, bannerX, bannerY, bannerWidth, bannerHeight);

            // TABLE POSITION AND SIZE
            long tableX = 322000;
            long tableY = 1800000;
            long rowHeight = 650000;

            // COLUMN WIDTHS
            long slNoWidth = 900000;
            long nameWidth = 7000000;
            long addressWidth = 3500000;

            long totalTableWidth = slNoWidth + nameWidth + addressWidth;

            // TRUST COMMITTEE TITLE - WHITE TEXT
            AddTextBox(shapeTree, "SHRI SIDDHAROODHA SWAMIJI MATH TRUST COMMITTEE",
                tableX, 70000, totalTableWidth, 350000, 24, true, "FFFFFF");

            AddTextBox(shapeTree, "ಶ್ರೀ ಸಿದ್ಧಾರೂಢ ಸ್ವಾಮೀಜಿ ಮಠ ಟ್ರಸ್ಟ್ ಕಮಿಟಿ",
                tableX, 490000, totalTableWidth, 350000, 26, true, "FFFFFF", "kn-IN");

            // SELECTED SEVA TYPE - ENGLISH (GOLD)
            string selectedSevaType = _sevaType ?? "";

            AddTextBox(shapeTree, $"{selectedSevaType} ({_sevaDate})",
                tableX, 800000, totalTableWidth, 350000, 28, true, "C9A84C");

            // SELECTED SEVA TYPE - KANNADA
            string selectedSevaTypeKannada;

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

            AddTextBox(shapeTree, selectedSevaTypeKannada,
                tableX, 1220000, totalTableWidth, 300000, 26, true, "C9A84C", "kn-IN");

            // TABLE HEADER
            AddTableCell(shapeTree, "SL NO", tableX, tableY, slNoWidth, rowHeight, true);

            AddTableCell(shapeTree, "DEVOTEE NAME",
                tableX + slNoWidth, tableY, nameWidth, rowHeight, true);

            AddTableCell(shapeTree, "ADDRESS",
                tableX + slNoWidth + nameWidth, tableY, addressWidth, rowHeight, true);

            // GET RECORD RANGE
            int startIndex = slideNumber * recordsPerSlide;
            int endIndex = Math.Min(startIndex + recordsPerSlide, dt.Rows.Count);

            // DATA ROWS
            int currentRow = 1;

            for (int i = startIndex; i < endIndex; i++)
            {
                DataRow row = dt.Rows[i];
                long currentY = tableY + (rowHeight * currentRow);

                // Serial number continues across slides: 1, 2, 3 ...
                AddTableCell(shapeTree, (i + 1).ToString(),
                    tableX, currentY, slNoWidth, rowHeight, false);

                AddTableCell(shapeTree, Convert.ToString(row["DevoteeName"]),
                    tableX + slNoWidth, currentY, nameWidth, rowHeight, false);

                AddTableCell(shapeTree, Convert.ToString(row["Address"]),
                    tableX + slNoWidth + nameWidth, currentY, addressWidth, rowHeight, false);

                currentRow++;
            }

            // HEADER PHOTOS (from hfLeftImage / hfRightImage)
            if (_leftImageBytes != null)
            {
                AddImageToSlide(slidePart, _leftImageBytes, _leftImageExt,
                    120000, 100000, 1150000, 1150000);
            }

            if (_rightImageBytes != null)
            {
                AddImageToSlide(slidePart, _rightImageBytes, _rightImageExt,
                    10800000, 100000, 1150000, 1150000);
            }
        }

        private void AddImageToSlide(
            SlidePart slidePart,
            byte[] imageBytes,
            string extension,
            long x,
            long y,
            long width,
            long height)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return;

            // OpenXML 3.x uses PartTypeInfo
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
                    return; // unsupported image type - skip
            }

            ImagePart imagePart = slidePart.AddImagePart(imagePartType);

            using (MemoryStream imageStream = new MemoryStream(imageBytes))
            {
                imagePart.FeedData(imageStream);
            }

            string relationshipId = slidePart.GetIdOfPart(imagePart);

            P.ShapeTree shapeTree = slidePart.Slide.CommonSlideData.ShapeTree;

            uint shapeId = GetNextShapeId(shapeTree);

            P.Picture picture = new P.Picture();

            P.NonVisualPictureProperties nonVisualPictureProperties =
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

            A.Blip blip = new A.Blip { Embed = relationshipId };

            A.Stretch stretch = new A.Stretch(new A.FillRectangle());

            P.BlipFill blipFill = new P.BlipFill();
            blipFill.Append(blip);
            blipFill.Append(stretch);

            A.Transform2D transform =
                new A.Transform2D(
                    new A.Offset { X = x, Y = y },
                    new A.Extents { Cx = width, Cy = height });

            A.PresetGeometry presetGeometry =
                new A.PresetGeometry(new A.AdjustValueList())
                {
                    Preset = A.ShapeTypeValues.Rectangle
                };

            P.ShapeProperties shapeProperties = new P.ShapeProperties();
            shapeProperties.Append(transform);
            shapeProperties.Append(presetGeometry);

            picture.Append(nonVisualPictureProperties);
            picture.Append(blipFill);
            picture.Append(shapeProperties);

            shapeTree.Append(picture);

            slidePart.Slide.Save();
        }

        private void AddTableCell(
            P.ShapeTree shapeTree,
            string text,
            long x,
            long y,
            long width,
            long height,
            bool isHeader)
        {
            uint shapeId = GetNextShapeId(shapeTree);

            P.Shape shape = new P.Shape();

            P.NonVisualShapeProperties nonVisualShapeProperties =
                new P.NonVisualShapeProperties(
                    new P.NonVisualDrawingProperties
                    {
                        Id = shapeId,
                        Name = "Table Cell " + shapeId
                    },
                    new P.NonVisualShapeDrawingProperties(
                        new A.ShapeLocks { NoGrouping = true }),
                    new P.ApplicationNonVisualDrawingProperties()
                );

            P.ShapeProperties shapeProperties = new P.ShapeProperties();

            shapeProperties.Transform2D =
                new A.Transform2D(
                    new A.Offset { X = x, Y = y },
                    new A.Extents { Cx = width, Cy = height });

            shapeProperties.Append(
                new A.PresetGeometry(new A.AdjustValueList())
                {
                    Preset = A.ShapeTypeValues.Rectangle
                });

            shapeProperties.Append(
                new A.SolidFill(
                    new A.RgbColorModelHex
                    {
                        Val = isHeader ? "2C5AA0" : "FFFFFF"
                    }));

            shapeProperties.Append(
                new A.Outline(
                    new A.SolidFill(
                        new A.RgbColorModelHex { Val = "808080" }))
                {
                    Width = 12700
                });

            P.TextBody textBody = new P.TextBody();

            textBody.Append(new A.BodyProperties
            {
                Anchor = A.TextAnchoringTypeValues.Center
            });

            textBody.Append(new A.ListStyle());

            A.Paragraph paragraph = new A.Paragraph();

            paragraph.ParagraphProperties =
                new A.ParagraphProperties
                {
                    Alignment = A.TextAlignmentTypeValues.Center
                };

            A.Run run = new A.Run();

            run.RunProperties =
                new A.RunProperties
                {
                    // All table text = 20 pt and bold
                    FontSize = 2000,
                    Bold = true,
                    Language = isHeader ? "en-US" : "kn-IN"
                };

            // Text colour: white for header, black for data
            run.RunProperties.Append(
                new A.SolidFill(
                    new A.RgbColorModelHex
                    {
                        Val = isHeader ? "FFFFFF" : "000000"
                    }));

            // Kannada-capable font
            run.RunProperties.Append(new A.LatinFont { Typeface = "Nirmala UI" });
            run.RunProperties.Append(new A.ComplexScriptFont { Typeface = "Nirmala UI" });

            run.Text = new A.Text(text ?? string.Empty);

            paragraph.Append(run);
            textBody.Append(paragraph);

            shape.Append(nonVisualShapeProperties);
            shape.Append(shapeProperties);
            shape.Append(textBody);

            shapeTree.Append(shape);
        }

        private uint GetNextShapeId(P.ShapeTree shapeTree)
        {
            uint maxId = 1;

            foreach (P.NonVisualDrawingProperties properties
                in shapeTree.Descendants<P.NonVisualDrawingProperties>())
            {
                if (properties.Id != null && properties.Id.Value > maxId)
                    maxId = properties.Id.Value;
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

            P.NonVisualShapeProperties nonVisualShapeProperties =
                new P.NonVisualShapeProperties(
                    new P.NonVisualDrawingProperties { Id = shapeId, Name = "Text Box " + shapeId },
                    new P.NonVisualShapeDrawingProperties(new A.ShapeLocks { NoGrouping = true }),
                    new P.ApplicationNonVisualDrawingProperties()
                );

            P.ShapeProperties shapeProperties = new P.ShapeProperties();
            shapeProperties.Transform2D = new A.Transform2D(
                new A.Offset { X = x, Y = y },
                new A.Extents { Cx = width, Cy = height });

            shapeProperties.Append(new A.PresetGeometry(new A.AdjustValueList())
            {
                Preset = A.ShapeTypeValues.Rectangle
            });
            shapeProperties.Append(new A.NoFill());
            shapeProperties.Append(new A.Outline(new A.NoFill()));

            P.TextBody textBody = new P.TextBody();
            textBody.Append(new A.BodyProperties { Anchor = A.TextAnchoringTypeValues.Center });
            textBody.Append(new A.ListStyle());

            A.Paragraph paragraph = new A.Paragraph();
            paragraph.ParagraphProperties = new A.ParagraphProperties
            {
                Alignment = A.TextAlignmentTypeValues.Center
            };

            A.Run run = new A.Run();

            run.RunProperties = new A.RunProperties
            {
                FontSize = fontSize * 100,
                Bold = bold,
                Language = language
            };

            run.RunProperties.Append(
                new A.SolidFill(new A.RgbColorModelHex { Val = textColorHex }));

            // Kannada-capable font
            run.RunProperties.Append(new A.LatinFont { Typeface = "Nirmala UI" });
            run.RunProperties.Append(new A.ComplexScriptFont { Typeface = "Nirmala UI" });

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
                            new P.NonVisualDrawingProperties { Id = 1U, Name = "" },
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
                            new P.NonVisualDrawingProperties { Id = 1U, Name = "" },
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

            slidePart.Slide.Append(transition);
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

            properties.Append(new A.PresetGeometry(new A.AdjustValueList())
            {
                Preset = A.ShapeTypeValues.Rectangle
            });

            A.GradientFill gradientFill = new A.GradientFill();
            var stopList = new A.GradientStopList();

            stopList.Append(new A.GradientStop(
                new A.RgbColorModelHex { Val = "0A1E3D" })
            { Position = 0 });

            stopList.Append(new A.GradientStop(
                new A.RgbColorModelHex { Val = "1A3A5C" })
            { Position = 30000 });

            stopList.Append(new A.GradientStop(
                new A.RgbColorModelHex { Val = "2C5F8A" })
            { Position = 50000 });

            stopList.Append(new A.GradientStop(
                new A.RgbColorModelHex { Val = "1A3A5C" })
            { Position = 70000 });

            stopList.Append(new A.GradientStop(
                new A.RgbColorModelHex { Val = "0A1E3D" })
            { Position = 100000 });

            gradientFill.Append(stopList);
            gradientFill.Append(new A.LinearGradientFill { Angle = 0 });

            properties.Append(gradientFill);

            A.Outline outline = new A.Outline(
                new A.SolidFill(new A.RgbColorModelHex { Val = "0A1E3D" }))
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

            properties.Append(new A.PresetGeometry(new A.AdjustValueList())
            {
                Preset = A.ShapeTypeValues.Rectangle
            });

            A.GradientFill goldGradient = new A.GradientFill();
            var goldStops = new A.GradientStopList();

            goldStops.Append(new A.GradientStop(
                new A.RgbColorModelHex { Val = "B8860B" })
            { Position = 0 });

            goldStops.Append(new A.GradientStop(
                new A.RgbColorModelHex { Val = "FFD700" })
            { Position = 50000 });

            goldStops.Append(new A.GradientStop(
                new A.RgbColorModelHex { Val = "B8860B" })
            { Position = 100000 });

            goldGradient.Append(goldStops);
            goldGradient.Append(new A.LinearGradientFill { Angle = 0 });

            properties.Append(goldGradient);
            properties.Append(new A.Outline(new A.NoFill()));

            shape.Append(properties);
            shape.Append(new P.TextBody(new A.BodyProperties(), new A.ListStyle(), new A.Paragraph()));
            shapeTree.Append(shape);
        }
    }
}