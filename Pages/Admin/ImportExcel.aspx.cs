using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using OfficeOpenXml;

namespace SSMS.Pages.Admin
{
    public partial class ImportExcel : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            ExcelPackage.License.SetNonCommercialOrganization("Nirantara Seva");
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            pnlResult.Visible = false;
            pnlErrors.Visible = false;

            if (!fuExcel.HasFile)
            {
                ShowResult("Please select an Excel file.", "warning");
                return;
            }

            string extension =
                Path.GetExtension(fuExcel.FileName).ToLower();

            if (extension != ".xlsx" && extension != ".xls")
            {
                ShowResult(
                    "Only .xlsx and .xls Excel files are allowed.",
                    "warning");

                return;
            }

            try
            {
                ImportExcelFile();
            }
            catch (Exception ex)
            {
                ShowResult(
                    "Import failed: " + ex.Message,
                    "error");
            }
        }

        // IMPORT EXCEL FILE
 
        private void ImportExcelFile()
        {
            int totalRows = 0;
            int importedRows = 0;
            int skippedRows = 0;
            int errorRows = 0;

            List<string> errors =
                new List<string>();

            byte[] fileBytes =
                fuExcel.FileBytes;

            using (MemoryStream stream =
                   new MemoryStream(fileBytes))
            using (ExcelPackage package =
                   new ExcelPackage(stream))
            {
         
                // CHECK WORKSHEET

                if (package.Workbook.Worksheets.Count == 0)
                {
                    ShowResult(
                        "Excel file does not contain any worksheet.",
                        "error");

                    return;
                }

                ExcelWorksheet worksheet =
                    package.Workbook.Worksheets[0];

                if (worksheet.Dimension == null)
                {
                    ShowResult(
                        "Excel worksheet is empty.",
                        "error");

                    return;
                }

                int startRow =
                    worksheet.Dimension.Start.Row;

                int endRow =
                    worksheet.Dimension.End.Row;

                int startColumn =
                    worksheet.Dimension.Start.Column;

                int endColumn =
                    worksheet.Dimension.End.Column;

                // READ EXCEL HEADERS

                Dictionary<string, int> columns =
                    new Dictionary<string, int>(
                        StringComparer.OrdinalIgnoreCase);

                for (int col = startColumn;
                     col <= endColumn;
                     col++)
                {
                    string header =
                        worksheet.Cells[startRow, col]
                                 .Text
                                 .Trim();

                    if (!string.IsNullOrWhiteSpace(header))
                    {
                        string normalized =
                            NormalizeHeader(header);

                        if (!columns.ContainsKey(normalized))
                        {
                            columns.Add(
                                normalized,
                                col);
                        }
                    }
                }

                // ========================================================
                // DATABASE
                // ========================================================

                using (SqlConnection con =
                       new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlTransaction transaction =
                           con.BeginTransaction())
                    {
                        try
                        {
                            // ====================================================
                            // NEXT SERIAL NUMBER
                            // ====================================================

                            int nextSerialNo =
                                GetNextSerialNumber(
                                    con,
                                    transaction);

                            // ====================================================
                            // REGISTER NUMBER COUNTERS
                            //
                            // IMPORTANT:
                            // We DO NOT load S, G and D here.
                            //
                            // The counter is loaded only when that SevaFor
                            // is actually required.
                            //
                            // This means:
                            //
                            // S exists -> get next S
                            // G exists -> get next G
                            // D exists -> get next D
                            //
                            // If a prefix does not exist, that row will
                            // generate an error instead of S-1/G-1/D-1.
                            // ====================================================

                            Dictionary<string, int> nextRegisterNumbers =
                                new Dictionary<string, int>(
                                    StringComparer.OrdinalIgnoreCase);

                            // ====================================================
                            // REGISTER NUMBERS USED IN THIS EXCEL FILE
                            // ====================================================

                            HashSet<string> usedInFile =
                                new HashSet<string>(
                                    StringComparer.OrdinalIgnoreCase);

                            // ====================================================
                            // PROCESS EXCEL ROWS
                            // ====================================================

                            for (int row =
                                 startRow + 1;
                                 row <= endRow;
                                 row++)
                            {
                                try
                                {
                                    Dictionary<string, string> data =
                                        ReadExcelRow(
                                            worksheet,
                                            row,
                                            columns);

                                    // Completely empty row
                                    if (data.Count == 0)
                                    {
                                        continue;
                                    }

                                    totalRows++;

                                    // ====================================================
                                    // READ REGISTER NO
                                    // ====================================================

                                    string registerNo =
                                        GetValue(
                                            data,
                                            "RegisterNo")
                                        .Trim();

                                    // ====================================================
                                    // GET SEVA FOR CODE
                                    //
                                    // Siddharood Swami  -> S
                                    // Gurunathrood Swami -> G
                                    // Dasoha             -> D
                                    // ====================================================

                                    string sevaCode =
                                        GetSevaForCode(
                                            GetValue(
                                                data,
                                                "SevaFor"));

                                    bool generated = false;

                                    // ====================================================
                                    // IF REGISTER NO IS PROVIDED IN EXCEL
                                    // ====================================================

                                    if (!string.IsNullOrWhiteSpace(registerNo))
                                    {
                                        // --------------------------------------------
                                        // Check database
                                        // --------------------------------------------

                                        if (RegisterNoExists(
                                            registerNo,
                                            con,
                                            transaction))
                                        {
                                            skippedRows++;
                                            continue;
                                        }

                                        // --------------------------------------------
                                        // Check duplicate inside same Excel
                                        // --------------------------------------------

                                        if (usedInFile.Contains(
                                            registerNo))
                                        {
                                            skippedRows++;
                                            continue;
                                        }

                                        // --------------------------------------------
                                        // If SevaFor is blank,
                                        // determine S/G/D from RegisterNo
                                        //
                                        // Example:
                                        // S-168112 -> S
                                        // G-17650  -> G
                                        // D-15450  -> D
                                        // --------------------------------------------

                                        if (string.IsNullOrWhiteSpace(sevaCode))
                                        {
                                            string prefix =
                                                GetRegisterPrefix(
                                                    registerNo);

                                            if (prefix == "S" ||
                                                prefix == "G" ||
                                                prefix == "D")
                                            {
                                                sevaCode = prefix;
                                            }
                                        }

                                        // --------------------------------------------
                                        // If still no SevaFor, reject row
                                        // --------------------------------------------

                                        if (string.IsNullOrWhiteSpace(sevaCode))
                                        {
                                            errorRows++;

                                            errors.Add(
                                                "Row " +
                                                row +
                                                ": Unable to determine SevaFor / prefix for RegisterNo '" +
                                                registerNo +
                                                "'.");

                                            continue;
                                        }
                                    }

                                    // ====================================================
                                    // REGISTER NO IS BLANK
                                    //
                                    // GENERATE BASED ON SEVA FOR
                                    // ====================================================

                                    if (string.IsNullOrWhiteSpace(registerNo))
                                    {
                                        // --------------------------------------------
                                        // SevaFor must be S/G/D
                                        // --------------------------------------------

                                        if (sevaCode != "S" &&
                                            sevaCode != "G" &&
                                            sevaCode != "D")
                                        {
                                            errorRows++;

                                            errors.Add(
                                                "Row " +
                                                row +
                                                ": Invalid SevaFor '" +
                                                GetValue(
                                                    data,
                                                    "SevaFor") +
                                                "'. Use Siddharood Swami, Gurunathrood Swami or Dasoha.");

                                            continue;
                                        }

                                        // ====================================================
                                        // LOAD NEXT NUMBER FOR THIS PREFIX
                                        // ONLY WHEN REQUIRED
                                        // ====================================================

                                        if (!nextRegisterNumbers.ContainsKey(
                                            sevaCode))
                                        {
                                            int nextNumber;

                                            try
                                            {
                                                nextNumber =
                                                    GetNextRegisterNumber(
                                                        sevaCode,
                                                        con,
                                                        transaction);
                                            }
                                            catch (Exception prefixEx)
                                            {
                                                errorRows++;

                                                errors.Add(
                                                    "Row " +
                                                    row +
                                                    ": " +
                                                    prefixEx.Message);

                                                continue;
                                            }

                                            nextRegisterNumbers.Add(
                                                sevaCode,
                                                nextNumber);
                                        }

                                        // ====================================================
                                        // GENERATE REGISTER NUMBER
                                        // ====================================================

                                        registerNo =
                                            sevaCode +
                                            "-" +
                                            nextRegisterNumbers[sevaCode]
                                                .ToString();

                                        generated = true;
                                    }

                                    // ====================================================
                                    // SAFETY CHECK
                                    // ====================================================

                                    if (string.IsNullOrWhiteSpace(registerNo))
                                    {
                                        errorRows++;

                                        errors.Add(
                                            "Row " +
                                            row +
                                            ": RegisterNo could not be generated.");

                                        continue;
                                    }

                                    // ====================================================
                                    // SAFETY CHECK FOR GENERATED REGISTER NO
                                    // ====================================================

                                    if (RegisterNoExists(
                                        registerNo,
                                        con,
                                        transaction))
                                    {
                                        errorRows++;

                                        errors.Add(
                                            "Row " +
                                            row +
                                            ": RegisterNo '" +
                                            registerNo +
                                            "' already exists.");

                                        continue;
                                    }

                                    if (usedInFile.Contains(
                                        registerNo))
                                    {
                                        errorRows++;

                                        errors.Add(
                                            "Row " +
                                            row +
                                            ": RegisterNo '" +
                                            registerNo +
                                            "' is duplicated in the Excel file.");

                                        continue;
                                    }

                                    // ====================================================
                                    // INSERT MEMBER
                                    // ====================================================

                                    InsertMember(
                                        data,
                                        nextSerialNo.ToString(),
                                        registerNo,
                                        sevaCode,
                                        con,
                                        transaction);

                                    // ====================================================
                                    // INSERT SUCCESSFUL
                                    // ====================================================

                                    importedRows++;

                                    // Next SerialNo
                                    nextSerialNo++;

                                    // Remember RegisterNo in this Excel
                                    usedInFile.Add(registerNo);

                                    // ====================================================
                                    // IF REGISTER NO WAS GENERATED
                                    // MOVE COUNTER TO NEXT NUMBER
                                    // ====================================================

                                    if (generated)
                                    {
                                        nextRegisterNumbers[sevaCode]++;
                                    }
                                    else
                                    {
                                        // ====================================================
                                        // IF REGISTER NO WAS PROVIDED IN EXCEL
                                        //
                                        // Example:
                                        //
                                        // Existing DB:
                                        // S-168111
                                        //
                                        // Excel new row:
                                        // S-200000
                                        //
                                        // Then future generated S must start
                                        // after 200000, not 168112.
                                        // ====================================================

                                        UpdateCounterFromSuppliedRegisterNo(
                                            registerNo,
                                            sevaCode,
                                            nextRegisterNumbers);
                                    }
                                }
                                catch (Exception rowEx)
                                {
                                    errorRows++;

                                    errors.Add(
                                        "Row " +
                                        row +
                                        ": " +
                                        rowEx.Message);
                                }
                            }

                            // ========================================================
                            // COMMIT
                            // ========================================================

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

            // ================================================================
            // DISPLAY RESULT
            // ================================================================

            StringBuilder result =
                new StringBuilder();

            result.Append(
                "<b>Excel Import Completed</b><br/><br/>");

            result.Append(
                "Total rows: <b>" +
                totalRows +
                "</b><br/>");

            result.Append(
                "Imported: <b>" +
                importedRows +
                "</b><br/>");

            result.Append(
                "Skipped existing RegisterNo: <b>" +
                skippedRows +
                "</b><br/>");

            result.Append(
                "Errors: <b>" +
                errorRows +
                "</b>");

            ShowResult(
                result.ToString(),
                errorRows > 0
                    ? "warning"
                    : "success");

            // ================================================================
            // DISPLAY ERRORS
            // ================================================================

            if (errors.Count > 0)
            {
                StringBuilder errorHtml =
                    new StringBuilder();

                foreach (string error in errors)
                {
                    errorHtml.Append(
                        "<div>" +
                        Server.HtmlEncode(error) +
                        "</div>");
                }

                litErrors.Text =
                    errorHtml.ToString();

                pnlErrors.Visible = true;
            }
        }

        // ================================================================
        // READ EXCEL ROW
        // ================================================================
        private Dictionary<string, string> ReadExcelRow(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columns)
        {
            Dictionary<string, string> data =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, int> column
                     in columns)
            {
                data[column.Key] =
                    worksheet.Cells[
                        row,
                        column.Value]
                    .Text
                    .Trim();
            }

            if (!data.Values.Any(
                x => !string.IsNullOrWhiteSpace(x)))
            {
                data.Clear();
            }

            return data;
        }

        // ================================================================
        // HEADER NORMALIZATION
        // ================================================================
        private string NormalizeHeader(string header)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                return "";
            }

            return header
                .Trim()
                .Replace(" ", "")
                .Replace("_", "")
                .Replace("-", "")
                .Replace(".", "")
                .Replace("/", "")
                .Replace("\\", "")
                .ToLower();
        }

        // ================================================================
        // GET VALUE
        // ================================================================
        private string GetValue(
            Dictionary<string, string> data,
            string column)
        {
            string normalized =
                NormalizeHeader(column);

            if (data.ContainsKey(normalized))
            {
                return data[normalized];
            }

            return "";
        }

        // ================================================================
        // CHECK REGISTER NO EXISTS
        // ================================================================
        private bool RegisterNoExists(
            string registerNo,
            SqlConnection con,
            SqlTransaction transaction)
        {
            string query = @"
                SELECT COUNT(1)
                FROM [SMSF].[dbo].[SevaMemberDetails]
                WHERE LTRIM(RTRIM(RegisterNo)) = LTRIM(RTRIM(@RegisterNo))";

            using (SqlCommand cmd =
                   new SqlCommand(
                       query,
                       con,
                       transaction))
            {
                cmd.Parameters.Add(
                    "@RegisterNo",
                    SqlDbType.NVarChar,
                    100).Value =
                    registerNo.Trim();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()) > 0;
            }
        }

        // ================================================================
        // GET SEVA FOR CODE
        //
        // Siddharood Swami  -> S
        // Gurunathrood Swami -> G
        // Dasoha             -> D
        // ================================================================
        private string GetSevaForCode(string sevaFor)
        {
            if (string.IsNullOrWhiteSpace(sevaFor))
            {
                return "";
            }

            string value =
                sevaFor.Trim().ToLower();

            // Direct codes
            if (value == "s")
            {
                return "S";
            }

            if (value == "g")
            {
                return "G";
            }

            if (value == "d")
            {
                return "D";
            }

            // Siddharood / Siddharoodh
            if (value.Contains("siddharood"))
            {
                return "S";
            }

            // Gurunath / Gurunathrood
            if (value.Contains("gurunath"))
            {
                return "G";
            }

            // Dasoha
            if (value.Contains("dasoha"))
            {
                return "D";
            }

            return "";
        }

        // ================================================================
        // GET PREFIX FROM REGISTER NO
        //
        // S-168111 -> S
        // G-17649  -> G
        // D-15449  -> D
        // ================================================================
        private string GetRegisterPrefix(
            string registerNo)
        {
            if (string.IsNullOrWhiteSpace(registerNo))
            {
                return "";
            }

            string value =
                registerNo.Trim()
                          .ToUpperInvariant();

            int hyphenIndex =
                value.IndexOf('-');

            if (hyphenIndex <= 0)
            {
                return "";
            }

            string prefix =
                value.Substring(
                    0,
                    hyphenIndex);

            if (prefix == "S" ||
                prefix == "G" ||
                prefix == "D")
            {
                return prefix;
            }

            return "";
        }

        // ================================================================
        // GET NUMBER FROM REGISTER NO
        //
        // S-168111 -> 168111
        // G-17649  -> 17649
        // D-15449  -> 15449
        // ================================================================
        private int GetRegisterNumberValue(
            string registerNo)
        {
            if (string.IsNullOrWhiteSpace(registerNo))
            {
                return 0;
            }

            string value =
                registerNo.Trim();

            int hyphenIndex =
                value.IndexOf('-');

            if (hyphenIndex < 0 ||
                hyphenIndex >= value.Length - 1)
            {
                return 0;
            }

            string numberPart =
                value.Substring(
                    hyphenIndex + 1)
                    .Trim();

            int number;

            if (int.TryParse(
                numberPart,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out number))
            {
                return number;
            }

            return 0;
        }

        // ================================================================
        // NEXT SERIAL NUMBER
        // ================================================================
        private int GetNextSerialNumber(
            SqlConnection con,
            SqlTransaction transaction)
        {
            string query = @"
                SELECT ISNULL(
                    MAX(
                        TRY_CONVERT(
                            INT,
                            NULLIF(
                                LTRIM(RTRIM(SerialNo)),
                                ''
                            )
                        )
                    ),
                    0
                ) + 1
                FROM [SMSF].[dbo].[SevaMemberDetails]";

            using (SqlCommand cmd =
                   new SqlCommand(
                       query,
                       con,
                       transaction))
            {
                return Convert.ToInt32(
                    cmd.ExecuteScalar());
            }
        }


        private int GetNextRegisterNumber(
    string sevaCode,
    SqlConnection con,
    SqlTransaction transaction)
        {
            sevaCode = (sevaCode ?? "").Trim().ToUpperInvariant();

            if (sevaCode != "S" &&
                sevaCode != "G" &&
                sevaCode != "D")
            {
                throw new Exception(
                    "Invalid SevaFor code: " + sevaCode);
            }
            string query = @"
        SELECT ISNULL(
            MAX(
                CASE
                    WHEN RegisterNo NOT LIKE '%-%'
                    THEN TRY_CAST(
                        LTRIM(RTRIM(RegisterNo))
                        AS INT
                    )
             
                    WHEN UPPER(
                        LTRIM(RTRIM(RegisterNo))
                    ) LIKE @SevaCode + '-%'
                    THEN TRY_CAST(
                        SUBSTRING(
                            LTRIM(RTRIM(RegisterNo)),
                            LEN(@SevaCode) + 2,
                            50
                        ) AS INT
                    )

                    ELSE NULL

                END
            ),
            0
        ) + 1

        FROM [SMSF].[dbo].[SevaMemberDetails]

        WHERE UPPER(
            LTRIM(RTRIM(SevaFor))
        ) = @SevaCode;
    ";

            using (SqlCommand cmd =
                   new SqlCommand(
                       query,
                       con,
                       transaction))
            {
                cmd.Parameters.Add(
                    "@SevaCode",
                    SqlDbType.NVarChar,
                    10).Value = sevaCode;

                object result = cmd.ExecuteScalar();

                if (result == null ||
                    result == DBNull.Value)
                {
                    throw new Exception(
                        "Unable to generate RegisterNo for SevaFor '" +
                        sevaCode +
                        "'.");
                }

                int nextNumber = Convert.ToInt32(result);

                if (nextNumber <= 0)
                {
                    throw new Exception(
                        "Invalid next RegisterNo generated for SevaFor '" +
                        sevaCode +
                        "'.");
                }

                return nextNumber;
            }
        }


        private void UpdateCounterFromSuppliedRegisterNo(
            string registerNo,
            string sevaCode,
            Dictionary<string, int> nextRegisterNumbers)
        {
            if (string.IsNullOrWhiteSpace(registerNo))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(sevaCode))
            {
                return;
            }

            if (sevaCode != "S" &&
                sevaCode != "G" &&
                sevaCode != "D")
            {
                return;
            }

            int suppliedNumber =
                GetRegisterNumberValue(
                    registerNo);

            if (suppliedNumber <= 0)
            {
                return;
            }

            // If this prefix has not yet been initialized
            // during this Excel import, initialize it to
            // supplied number + 1.
            if (!nextRegisterNumbers.ContainsKey(
                sevaCode))
            {
                nextRegisterNumbers.Add(
                    sevaCode,
                    suppliedNumber + 1);

                return;
            }

            // Make sure future generated numbers do not
            // go below the supplied RegisterNo.
            if (suppliedNumber >=
                nextRegisterNumbers[sevaCode])
            {
                nextRegisterNumbers[sevaCode] =
                    suppliedNumber + 1;
            }
        }

        // ================================================================
        // INSERT MEMBER
        // ================================================================
        private void InsertMember(
            Dictionary<string, string> data,
            string serialNo,
            string registerNo,
            string sevaCode,
            SqlConnection con,
            SqlTransaction transaction)
        {
            string query = @"
                INSERT INTO [SMSF].[dbo].[SevaMemberDetails]
                (
                    SerialNo,
                    SevaType,
                    DevoteeName,
                    Language,
                    State,
                    District,
                    Pincode,
                    City,
                    ContactNumber,
                    Email,
                    EntryDate,
                    CreatedOn,
                    Address,
                    Price,
                    Remarks,
                    PaymentMode,
                    UTRNumber,
                    BankName,
                    Branch,
                    IFSCCode,
                    AccountNumber,
                    EventName,
                    RegisterNo,
                    SevaFor,
                    AdditionalAmount
                )
                VALUES
                (
                    @SerialNo,
                    @SevaType,
                    @DevoteeName,
                    @Language,
                    @State,
                    @District,
                    @Pincode,
                    @City,
                    @ContactNumber,
                    @Email,
                    @EntryDate,
                    GETDATE(),
                    @Address,
                    @Price,
                    @Remarks,
                    @PaymentMode,
                    @UTRNumber,
                    @BankName,
                    @Branch,
                    @IFSCCode,
                    @AccountNumber,
                    @EventName,
                    @RegisterNo,
                    @SevaFor,
                    @AdditionalAmount
                )";

            using (SqlCommand cmd =
                   new SqlCommand(
                       query,
                       con,
                       transaction))
            {
                cmd.Parameters.AddWithValue(
                    "@SerialNo",
                    serialNo);

                cmd.Parameters.AddWithValue(
                    "@SevaType",
                    DbValue(
                        GetValue(
                            data,
                            "SevaType")));

                cmd.Parameters.AddWithValue(
                    "@DevoteeName",
                    DbValue(
                        GetValue(
                            data,
                            "DevoteeName")));

                cmd.Parameters.AddWithValue(
                    "@Language",
                    DbValue(
                        GetValue(
                            data,
                            "Language")));

                cmd.Parameters.AddWithValue(
                    "@State",
                    DbValue(
                        GetValue(
                            data,
                            "State")));

                cmd.Parameters.AddWithValue(
                    "@District",
                    DbValue(
                        GetValue(
                            data,
                            "District")));

                cmd.Parameters.AddWithValue(
                    "@Pincode",
                    DbValue(
                        GetValue(
                            data,
                            "Pincode")));

                cmd.Parameters.AddWithValue(
                    "@City",
                    DbValue(
                        GetValue(
                            data,
                            "City")));

                cmd.Parameters.AddWithValue(
                    "@ContactNumber",
                    DbValue(
                        GetValue(
                            data,
                            "ContactNumber")));

                cmd.Parameters.AddWithValue(
                    "@Email",
                    DbValue(
                        GetValue(
                            data,
                            "Email")));

                cmd.Parameters.AddWithValue(
                    "@EntryDate",
                    GetDateValue(
                        GetValue(
                            data,
                            "EntryDate")));

                cmd.Parameters.AddWithValue(
                    "@Address",
                    DbValue(
                        GetValue(
                            data,
                            "Address")));

                cmd.Parameters.AddWithValue(
                    "@Price",
                    GetDecimalValue(
                        GetValue(
                            data,
                            "Price")));

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    DbValue(
                        GetValue(
                            data,
                            "Remarks")));

                cmd.Parameters.AddWithValue(
                    "@PaymentMode",
                    DbValue(
                        GetValue(
                            data,
                            "PaymentMode")));

                cmd.Parameters.AddWithValue(
                    "@UTRNumber",
                    DbValue(
                        GetValue(
                            data,
                            "UTRNumber")));

                cmd.Parameters.AddWithValue(
                    "@BankName",
                    DbValue(
                        GetValue(
                            data,
                            "BankName")));

                cmd.Parameters.AddWithValue(
                    "@Branch",
                    DbValue(
                        GetValue(
                            data,
                            "Branch")));

                cmd.Parameters.AddWithValue(
                    "@IFSCCode",
                    DbValue(
                        GetValue(
                            data,
                            "IFSCCode")));

                cmd.Parameters.AddWithValue(
                    "@AccountNumber",
                    DbValue(
                        GetValue(
                            data,
                            "AccountNumber")));

                cmd.Parameters.AddWithValue(
                    "@EventName",
                    DbValue(
                        GetValue(
                            data,
                            "EventName")));

                cmd.Parameters.AddWithValue(
                    "@RegisterNo",
                    registerNo);

                cmd.Parameters.AddWithValue(
                    "@SevaFor",
                    string.IsNullOrEmpty(sevaCode)
                        ? (object)DBNull.Value
                        : sevaCode);

                cmd.Parameters.AddWithValue(
                    "@AdditionalAmount",
                    GetDecimalValue(
                        GetValue(
                            data,
                            "AdditionalAmount")));

                // IMPORTANT:
                // Total is a computed column.
                // Therefore it is intentionally NOT included.

                cmd.ExecuteNonQuery();
            }
        }

        // ================================================================
        // DATABASE VALUE
        // ================================================================
        private object DbValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DBNull.Value;
            }

            return value.Trim();
        }

        // ================================================================
        // DECIMAL VALUE
        // ================================================================
        private object GetDecimalValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DBNull.Value;
            }

            decimal result;

            if (decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result))
            {
                return result;
            }

            if (decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out result))
            {
                return result;
            }

            return DBNull.Value;
        }

        // ================================================================
        // DATE VALUE
        // ================================================================
        private object GetDateValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DBNull.Value;
            }

            DateTime date;

            if (DateTime.TryParse(
                value,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out date))
            {
                return date;
            }

            if (DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date))
            {
                return date;
            }

            return DBNull.Value;
        }

        // ================================================================
        // RESULT MESSAGE
        // ================================================================
        private void ShowResult(
            string message,
            string type)
        {
            pnlResult.Visible = true;

            pnlResult.CssClass =
                "result-box " + type;

            lblResult.Text =
                message;
        }
    }
}