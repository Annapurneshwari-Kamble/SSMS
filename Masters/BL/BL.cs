using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SSMS.Masters.BL
{
    public class BL
    {
        private readonly DL.DL dl = new DL.DL();


        // 600 Paid List
        // ---------------------------------------------------------
        public DataTable GetPaidList(string search, string serialNo, string registerNo)
        {
            string query = @"
                SELECT
                    MemberID, SerialNo, RegisterNo, DevoteeName, ContactNumber,
                    SevaType, City, EntryDate, Price, AdditionalAmount, Total,
                    PaymentMode, EventName, SevaFor
                FROM SevaMemberDetails
                WHERE AdditionalAmount IS NOT NULL
                  AND TRY_CONVERT(DECIMAL(18,2), AdditionalAmount) > 0";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query += @"
                  AND (DevoteeName LIKE @Search OR ContactNumber LIKE @Search)";
                parameters.Add(dl.Param("@Search", "%" + search.Trim() + "%"));
            }

            if (!string.IsNullOrWhiteSpace(serialNo))
            {
                query += @"
                  AND SerialNo LIKE @SerialNo";
                parameters.Add(dl.Param("@SerialNo", "%" + serialNo.Trim() + "%"));
            }

            if (!string.IsNullOrWhiteSpace(registerNo))
            {
                query += @"
                  AND RegisterNo LIKE @RegisterNo";
                parameters.Add(dl.Param("@RegisterNo", "%" + registerNo.Trim() + "%"));
            }

            query += @"
                ORDER BY EntryDate DESC, MemberID DESC";

            return dl.ExecuteDataTable(query, parameters.ToArray());
        }


        // Totals calculation (business logic)
        // ---------------------------------------------------------
        public void CalculateTotals(DataTable dt, out int recordCount, out decimal additionalTotal, out decimal grandTotal)
        {
            additionalTotal = 0;
            grandTotal = 0;
            recordCount = dt.Rows.Count;

            foreach (DataRow row in dt.Rows)
            {
                decimal additionalAmount;
                if (decimal.TryParse(Convert.ToString(row["AdditionalAmount"]),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out additionalAmount))
                {
                    additionalTotal += additionalAmount;
                }

                decimal totalAmount;
                if (decimal.TryParse(Convert.ToString(row["Total"]),
                    NumberStyles.Any, CultureInfo.InvariantCulture, out totalAmount))
                {
                    grandTotal += totalAmount;
                }
            }
        }

        //  LOOKUPS
        //--------------------------------------------------------
        public string GenerateSerialNumber()
        {
            string query = @"SELECT ISNULL(MAX(TRY_CAST(SerialNo AS INT)), 0) + 1
                             FROM SevaMemberDetails";
            return Convert.ToString(dl.ExecuteScalar(query));
        }

        public DataTable GetSevaTypes()
        {
            return dl.ExecuteDataTable("SELECT id, name FROM [Seva_Type]");
        }

        public string[] GetStates()
        {
            return new[]
            {
                "Andhra Pradesh","Arunachal Pradesh","Assam","Bihar","Chhattisgarh",
                "Goa","Gujarat","Haryana","Himachal Pradesh","Jharkhand","Karnataka","Kerala",
                "Madhya Pradesh","Maharashtra","Manipur","Meghalaya","Mizoram","Nagaland","Odisha",
                "Punjab","Rajasthan","Sikkim","Tamil Nadu","Telangana","Tripura","Uttar Pradesh",
                "Uttarakhand","West Bengal","Andaman and Nicobar Islands","Chandigarh",
                "Dadra and Nagar Haveli and Daman and Diu","Delhi","Jammu and Kashmir",
                "Ladakh","Lakshadweep","Puducherry"
            };
        }

        public DataTable GetSevaFor(string sevaType)
        {
            string query;

            if (sevaType == "1")        // Nirantara Abhishek
            {
                query = @"SELECT id, SevaFor, SevaCode
                          FROM SevaFor
                          WHERE SevaCode IN ('S', 'G')
                          ORDER BY SevaFor";
            }
            else if (sevaType == "2")   // Nirantara Annaprasad
            {
                query = @"SELECT id, SevaFor, SevaCode
                          FROM SevaFor
                          WHERE SevaCode = 'D'
                          ORDER BY SevaFor";
            }
            else                        // Other sevas: show all
            {
                query = @"SELECT id, SevaFor, SevaCode
                          FROM SevaFor
                          ORDER BY SevaFor";
            }

            return dl.ExecuteDataTable(query);
        }

        public string GenerateRegisterNumber(string sevaCode)
        {
            sevaCode = (sevaCode ?? "").Trim();
            if (sevaCode == "") return "";

            string query = @"
                SELECT ISNULL(
                    MAX(
                        CASE
                            WHEN RegisterNo NOT LIKE '%-%'
                                THEN TRY_CAST(RegisterNo AS INT)
                            WHEN RegisterNo LIKE @SevaCode + '-%'
                                THEN TRY_CAST(SUBSTRING(RegisterNo, LEN(@SevaCode) + 2, 50) AS INT)
                            ELSE NULL
                        END
                    ), 0) + 1
                FROM SevaMemberDetails
                WHERE SevaFor = @SevaCode;";

            int next = Convert.ToInt32(
                dl.ExecuteScalar(query, new[] { dl.Param("@SevaCode", sevaCode) }));

            return sevaCode + "-" + next.ToString("D3");
        }

        //  TODAY'S SEVAS (sevaType: 1 = Abhishek, 2 = Annaprasad, 3 = Other)
        //-----------------------------------------------------
        public DataTable GetTodaysSevas(int sevaType)
        {
            string query = @"
                SELECT DevoteeName, EntryDate
                FROM SevaMemberDetails
                WHERE SevaType = @SevaType
                  AND EntryDate >= @TodayStart
                  AND EntryDate <  @TomorrowStart
                ORDER BY EntryDate DESC";

            DateTime todayStart = DateTime.Today;

            return dl.ExecuteDataTable(query, new[]
            {
                dl.Param("@SevaType", sevaType),
                dl.Param("@TodayStart", todayStart),
                dl.Param("@TomorrowStart", todayStart.AddDays(1))
            });
        }

        //  LOAD FOR EDIT
        // -------------------------------------------
        public GC.GC GetBySerialNo(string serialNo)
        {
            DataTable dt = dl.ExecuteDataTable(
                "SELECT * FROM SevaMemberDetails WHERE SerialNo = @SerialNo",
                new[] { dl.Param("@SerialNo", serialNo) });

            if (dt.Rows.Count == 0) return null;

            DataRow r = dt.Rows[0];

            decimal additional = r["AdditionalAmount"] == DBNull.Value
                ? 0 : Convert.ToDecimal(r["AdditionalAmount"]);
            decimal total = r["Total"] == DBNull.Value
                ? 0 : Convert.ToDecimal(r["Total"]);

            return new GC.GC
            {
                SerialNo = Convert.ToString(r["SerialNo"]),
                EntryDate = Convert.ToDateTime(r["EntryDate"]),
                Language = Convert.ToString(r["Language"]),
                SevaType = Convert.ToString(r["SevaType"]),
                SevaFor = Convert.ToString(r["SevaFor"]),
                EventName = Convert.ToString(r["EventName"]),
                RegisterNo = Convert.ToString(r["RegisterNo"]),
                DevoteeName = Convert.ToString(r["DevoteeName"]),
                State = Convert.ToString(r["State"]),
                District = Convert.ToString(r["District"]),
                Pincode = Convert.ToString(r["Pincode"]),
                City = Convert.ToString(r["City"]),
                Address = Convert.ToString(r["Address"]),
                ContactNumber = Convert.ToString(r["ContactNumber"]),
                Email = Convert.ToString(r["Email"]),
                Price = r["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Price"]),
                AdditionalAmount = additional,
                Total = total,
                Remarks = Convert.ToString(r["Remarks"]),
                PaymentMode = Convert.ToString(r["PaymentMode"]),
                UTRNumber = Convert.ToString(r["UTRNumber"]),
                BankName = Convert.ToString(r["BankName"]),
                Branch = Convert.ToString(r["Branch"]),
                IFSCCode = Convert.ToString(r["IFSCCode"]),
                AccountNumber = Convert.ToString(r["AccountNumber"])
            };
        }

        //  VALIDATION (parses raw text and fills Price/Total/EntryDate)
        // ---------------------------------------------------
        public bool Validate(GC.GC m, out string error)
        {
            error = null;

            // Main amount
            decimal price;
            if (!decimal.TryParse((m.PriceText ?? "").Trim(), out price) || price < 0)
            {
                error = "Please enter a valid amount.";
                return false;
            }
            m.Price = price;

            // Seva start
            if (string.IsNullOrEmpty(m.SevaStart))
            {
                error = "Please select Seva Start From.";
                return false;
            }

            // Additional amount applies only to "This Year"
            decimal additional = 0;
            if (m.SevaStart == "ThisYear")
            {
                if (!decimal.TryParse((m.AdditionalAmountText ?? "").Trim(), out additional)
                    || additional < 0)
                {
                    error = "Please enter a valid Additional Amount.";
                    return false;
                }
            }
            m.AdditionalAmount = additional;
            m.Total = price + additional;

            // Minimum amounts
            if (m.SevaType == "1" && price < 2500)
            {
                error = "For Nirantara Abhishek, amount must be ₹2,500 or more.";
                return false;
            }
            if (m.SevaType == "2" && price < 6000)
            {
                error = "For Nirantara Annaprasad, amount must be ₹6,000 or more.";
                return false;
            }

            // Payment mode rules
            if (m.PaymentMode == "Bank")
            {
                m.BankName = "State Bank of India";
                m.Branch = "Main Branch";
                m.IFSCCode = "SBIN0001234";
                m.AccountNumber = "123456789012";
            }
            else if (m.PaymentMode == "Cash")
            {
                if (!string.IsNullOrEmpty(m.UTRNumber) || !string.IsNullOrEmpty(m.BankName))
                {
                    error = "UTR and Bank Name are not valid for Cash payment. Select UPI/Bank.";
                    return false;
                }
            }

            // Date
            DateTime entryDate;
            if (!DateTime.TryParse(m.EntryDateText, out entryDate))
            {
                error = "Please enter a valid date.";
                return false;
            }
            m.EntryDate = entryDate;

            return true;
        }

        //  SAVE (INSERT or UPDATE)
        // ------------------------------------------------------
        public int Save(GC.GC m, bool isEdit)
        {
            ApplyKannadaConversion(m);

            string query;
            var p = new List<SqlParameter>
            {
                dl.Param("@SerialNo",(m.SerialNo ?? "").Trim()),
                dl.Param("@EntryDate",m.EntryDate),
                dl.Param("@Language",m.Language),
                dl.Param("@SevaType",m.SevaType),
                dl.Param("@SevaFor", m.SevaFor),
                dl.Param("@EventName",m.EventName),
                dl.Param("@RegisterNo",(m.RegisterNo ?? "").Trim()),
                dl.Param("@DevoteeName",m.DevoteeName),
                dl.Param("@State",m.State),
                dl.Param("@District",m.District),
                dl.Param("@Pincode",(m.Pincode ?? "").Trim()),
                dl.Param("@City",m.City),
                dl.Param("@Address",m.Address),
                dl.Param("@ContactNumber",(m.ContactNumber ?? "").Trim()),
                dl.Param("@Email",(m.Email ?? "").Trim()),
                dl.Param("@Price",m.Price),
                dl.Param("@AdditionalAmount",m.AdditionalAmount),
                dl.Param("@Remarks", (m.Remarks ?? "").Trim()),
                dl.Param("@PaymentMode",m.PaymentMode),
                dl.Param("@UTRNumber",(m.UTRNumber ?? "").Trim()),
                dl.Param("@BankName",(m.BankName ?? "").Trim()),
                dl.Param("@Branch",(m.Branch ?? "").Trim()),
                dl.Param("@IFSCCode",(m.IFSCCode ?? "").Trim()),
                dl.Param("@AccountNumber",(m.AccountNumber ?? "").Trim())
            };

            if (isEdit)
            {
                query = @"
                    UPDATE SevaMemberDetails SET
                        EntryDate = @EntryDate, Language = @Language, SevaType = @SevaType, SevaFor = @SevaFor, EventName = @EventName,
                        RegisterNo = @RegisterNo, DevoteeName = @DevoteeName,
                        State = @State,
                        District = @District,
                        Pincode = @Pincode,
                        City = @City,
                        Address = @Address,
                        ContactNumber = @ContactNumber,
                        Email = @Email,
                        Price = @Price,
                        AdditionalAmount = @AdditionalAmount,
                        Remarks = @Remarks,
                        PaymentMode = @PaymentMode,
                        UTRNumber = @UTRNumber,
                        BankName = @BankName,
                        Branch = @Branch,
                        IFSCCode = @IFSCCode,
                        AccountNumber = @AccountNumber
                    WHERE SerialNo = @SerialNo";
            }
            else
            {
                query = @"
                    INSERT INTO SevaMemberDetails
                    (
                        SerialNo, EntryDate, CreatedOn, Language, SevaType, SevaFor, EventName, RegisterNo, DevoteeName, State,District, Pincode, City, Address, ContactNumber, Email, Price, AdditionalAmount, Remarks, PaymentMode, UTRNumber, BankName, Branch, IFSCCode, AccountNumber
                    )
                    VALUES
                    (
                        @SerialNo, @EntryDate, @CreatedOn, @Language, @SevaType, @SevaFor, @EventName, @RegisterNo, @DevoteeName, @State, @District, @Pincode, @City, @Address, @ContactNumber, @Email, @Price, @AdditionalAmount, @Remarks,
                        @PaymentMode, @UTRNumber, @BankName, @Branch, @IFSCCode, @AccountNumber
                    )";

                p.Add(dl.Param("@CreatedOn", DateTime.Now));
            }

            return dl.ExecuteNonQuery(query, p.ToArray());
        }

        // 
        //  PRIVATE HELPERS
        // ----------------------------------------
        private void ApplyKannadaConversion(GC.GC m)
        {
            m.DevoteeName = (m.DevoteeName ?? "").Trim();
            m.EventName = (m.EventName ?? "").Trim();
            m.District = (m.District ?? "").Trim();
            m.City = (m.City ?? "").Trim();
            m.Address = (m.Address ?? "").Trim();

            if (m.Language != "Kannada") return;

            try
            {
                var converter = Kannada.AsciiUnicode.Converters.KannadaConverter.Instance;
                if (converter == null) return;

                m.DevoteeName = converter.ConvertAsciiToUnicode(m.DevoteeName);
                m.EventName = converter.ConvertAsciiToUnicode(m.EventName);
                m.District = converter.ConvertAsciiToUnicode(m.District);
                m.City = converter.ConvertAsciiToUnicode(m.City);
                m.Address = converter.ConvertAsciiToUnicode(m.Address);
            }
            catch { /* ignore or log */ }
        }
        //  ALL LIST (Labels)
        // ---------------------------------------------------------
        // Seva type dropdown (value = name, as in the original page)
        public DataTable GetSevaTypeNames()
        {
            return dl.ExecuteDataTable(
                "SELECT id, name FROM Seva_Type ORDER BY name");
        }

        // Search devotees with optional filters
        public DataTable SearchDevotees(GC.AllListGC filter)
        {
            StringBuilder query = new StringBuilder(@"
        SELECT TOP 1000 smd.DevoteeName, st.name AS SevaType, smd.Address
        FROM SevaMemberDetails smd
        INNER JOIN Seva_Type st ON smd.SevaType = st.id
        WHERE 1=1 ");

            List<SqlParameter> p = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(filter.DevoteeName))
            {
                query.Append(" AND smd.DevoteeName LIKE '%' + @DevoteeName + '%' ");
                p.Add(dl.Param("@DevoteeName", filter.DevoteeName.Trim()));
            }

            if (!string.IsNullOrEmpty(filter.SevaType))
            {
                query.Append(" AND st.name = @SevaType ");
                p.Add(dl.Param("@SevaType", filter.SevaType));
            }

            return dl.ExecuteDataTable(query.ToString(), p.ToArray());
        }

        // Builds the A4 label sheet HTML (2 columns x 4 rows = 8 labels per page)
        public string BuildLabelsHtml(List<GC.AllListGC> labels)
        {
            const string tableStart =
                "<table style='width:210mm; height:297mm; border-collapse:collapse; table-layout:fixed; margin:0; padding:0;'>";

            StringBuilder report = new StringBuilder();
            report.Append(tableStart);

            int count = 0;
            int total = labels.Count;
            int processed = 0;

            foreach (var item in labels)
            {
                if (count % 2 == 0)
                    report.Append("<tr>");

                report.Append(@"
            <td style='width:101mm; height:71.25mm; padding:3mm; text-align:left; vertical-align:top;'>
                <div style='display:flex; flex-direction:column; justify-content:flex-start; align-items:flex-start; height:100%;'>
                    <h4 style='margin-top:5px; font-size:22px; text-align:left;'>" + item.DevoteeName + @"</h4>
                    <p style='margin:0; font-size:20px; text-align:left;'>" + item.Address + @"</p>
                </div>
            </td>");

                count++;
                processed++;

                if (count % 2 == 0)
                    report.Append("</tr>");

                // Page break after 8 labels, only if more remain
                if (count % 8 == 0 && processed < total)
                {
                    report.Append("</table><div style='page-break-after:always;'></div>");
                    report.Append(tableStart);
                }
            }

            if (count % 2 != 0) report.Append("</tr>");
            report.Append("</table>");

            return report.ToString();
        }

        // Wraps the label HTML with styles for PDF export
        public string BuildPdfHtml(string reportHtml)
        {
            return @"
        <html>
            <head>
                <style>
                    body { margin:0; padding:0; }
                    table { width:210mm; height:297mm; border-collapse:collapse; margin:0; padding:0; }
                    td { width:101mm; height:71.25mm; padding:3mm; text-align:left; vertical-align:top; }
                    h4 { margin:0; font-size:14px; text-align:left; }
                    p { margin:0; font-size:12px; text-align:left; }
                </style>
            </head>
            <body>" + reportHtml + @"</body>
        </html>";
        }
        public GC.DashboardGC GetDashboardCounts()
        {
            // One query instead of three separate COUNT(*) calls
            string query = @"
        SELECT SevaType, COUNT(*) AS Cnt
        FROM SevaMemberDetails
        WHERE SevaType IN (1, 2, 3)
        GROUP BY SevaType";

            DataTable dt = dl.ExecuteDataTable(query);

            GC.DashboardGC counts = new GC.DashboardGC();

            foreach (DataRow r in dt.Rows)
            {
                int count = Convert.ToInt32(r["Cnt"]);

                switch (Convert.ToString(r["SevaType"]).Trim())
                {
                    case "1": counts.TotalAbhishek = count; break;
                    case "2": counts.TotalAnnaprasad = count; break;
                    case "3": counts.TotalOtherSevas = count; break;
                }
            }

            return counts;
        }
        //  NIRANTARA ABHISHEK LIST
        // -------------------------------

        // Only these columns may be used in dynamic filters (blocks SQL injection via column name)
        private static readonly HashSet<string> AllowedFilterColumns =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
        "SerialNo", "RegisterNo", "DevoteeName", "Language", "EventName",
        "State", "District", "City", "ContactNumber", "Price", "EntryDate"
            };

        //  SEVA LISTS (Abhishek = "1", Annaprasad = "2", Other = "3")
        //---------------------------------------
        private const string SevaListSelect = @"
    SELECT SerialNo, RegisterNo, DevoteeName, Language, EventName, State, District, City,
           ContactNumber, Price, EntryDate
    FROM SevaMemberDetails
    WHERE SevaType = @SevaType";

        // Simple filter (one column + one value)
        public DataTable GetSevaList(string sevaType, string column, string value)
        {
            StringBuilder query = new StringBuilder(SevaListSelect);
            List<SqlParameter> p = new List<SqlParameter> { dl.Param("@SevaType", sevaType) };

            if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(value))
            {
                query.Append(" AND " + BuildFilterClause(column, value, "@Value", p));
            }

            query.Append(" ORDER BY EntryDate DESC");

            return dl.ExecuteDataTable(query.ToString(), p.ToArray());
        }

        // Advanced filter (multiple rows with AND / OR)
        public DataTable GetSevaListAdvanced(string sevaType, List<GC.SevaFilterGC> filters)
        {
            StringBuilder query = new StringBuilder(SevaListSelect);
            List<SqlParameter> p = new List<SqlParameter> { dl.Param("@SevaType", sevaType) };

            StringBuilder custom = new StringBuilder();
            int i = 0;

            foreach (var f in filters)
            {
                if (string.IsNullOrEmpty(f.Value)) continue;

                string clause = BuildFilterClause(f.Column, f.Value, "@Value" + i, p);

                if (custom.Length > 0)
                {
                    string op = string.Equals(f.Condition, "OR", StringComparison.OrdinalIgnoreCase)
                        ? "OR" : "AND";
                    custom.Append(" " + op + " ");
                }

                custom.Append(clause);
                i++;
            }

            // Parentheses keep the SevaType filter applied even when OR is used
            if (custom.Length > 0)
                query.Append(" AND (" + custom + ")");

            query.Append(" ORDER BY EntryDate DESC");

            return dl.ExecuteDataTable(query.ToString(), p.ToArray());
        }

        // Builds one filter clause and adds its parameter
        private string BuildFilterClause(string column, string value, string paramName,
                                         List<SqlParameter> parameters)
        {
            column = (column ?? "").Trim();
            if (!AllowedFilterColumns.Contains(column))
                throw new ArgumentException("Invalid filter column: " + column);

            if (column.Equals("EntryDate", StringComparison.OrdinalIgnoreCase))
            {
                DateTime d;
                if (!DateTime.TryParse(value, out d))
                    throw new ArgumentException("Please enter a valid date.");

                parameters.Add(dl.Param(paramName, d.Date));
                return "CONVERT(date, EntryDate) = " + paramName;
            }

            parameters.Add(dl.Param(paramName, value.Trim()));
            return column + " LIKE '%' + " + paramName + " + '%'";
        }

        // Bulk date update; returns number of rows updated
        public int UpdateEntryDate(List<string> serialNos, DateTime date)
        {
            int updated = 0;

            foreach (string serialNo in serialNos)
            {
                updated += dl.ExecuteNonQuery(
                    "UPDATE SevaMemberDetails SET EntryDate = @Date WHERE SerialNo = @SerialNo",
                    new[]
                    {
                dl.Param("@Date", date),
                dl.Param("@SerialNo", serialNo)
                    });
            }

            return updated;
        }

        // Returns rows deleted (0 = not found)
        public int DeleteSeva(string serialNo)
        {
            return dl.ExecuteNonQuery(
                "DELETE FROM SevaMemberDetails WHERE SerialNo = @SerialNo",
                new[] { dl.Param("@SerialNo", serialNo) });
        }

        //  PAYMENT LIST
        // --------------------------------------------------
        public DataTable GetSevaTypesOrdered()
        {
            return dl.ExecuteDataTable("SELECT id, name FROM Seva_Type ORDER BY name");
        }

        public DataTable GetPayments(GC.PaymentFilterGC f)
        {
            StringBuilder query = new StringBuilder(@"
        SELECT TOP 1000
            s.SerialNo, s.RegisterNo, s.EntryDate,
            st.name AS SevaType,
            s.DevoteeName, s.Price, s.PaymentMode, s.UTRNumber, s.BankName
        FROM SevaMemberDetails s
        LEFT JOIN Seva_Type st ON s.SevaType = st.id
        WHERE 1=1");

            List<SqlParameter> p = new List<SqlParameter>();
            string text = (f.Value ?? "").Trim();

            switch (f.FilterType)
            {
                case "RegisterNo":
                    if (text != "")
                    {
                        query.Append(" AND s.RegisterNo LIKE '%' + @RegisterNo + '%'");
                        p.Add(dl.Param("@RegisterNo", text));
                    }
                    break;

                case "Date":
                    if (text != "")
                    {
                        DateTime d;
                        if (!DateTime.TryParse(text, out d))
                            throw new ArgumentException("Please enter a valid date.");

                        query.Append(" AND s.EntryDate >= @DateStart AND s.EntryDate < @DateEnd");
                        p.Add(dl.Param("@DateStart", d.Date));
                        p.Add(dl.Param("@DateEnd", d.Date.AddDays(1)));
                    }
                    break;

                case "SevaType":
                    if (!string.IsNullOrEmpty(f.SevaType))
                    {
                        query.Append(" AND s.SevaType = @SevaType");
                        p.Add(dl.Param("@SevaType", f.SevaType));
                    }
                    break;

                case "PaymentMode":
                    if (!string.IsNullOrEmpty(f.PaymentMode))
                    {
                        query.Append(" AND s.PaymentMode = @PaymentMode");
                        p.Add(dl.Param("@PaymentMode", f.PaymentMode));
                    }
                    break;

                case "DevoteeName":
                    if (text != "")
                    {
                        query.Append(" AND s.DevoteeName LIKE '%' + @DevoteeName + '%'");
                        p.Add(dl.Param("@DevoteeName", text));
                    }
                    break;

                case "UTRNumber":
                    if (text != "")
                    {
                        query.Append(" AND s.UTRNumber LIKE '%' + @UTR + '%'");
                        p.Add(dl.Param("@UTR", text));
                    }
                    break;

                case "BankName":
                    if (text != "")
                    {
                        query.Append(" AND s.BankName LIKE '%' + @Bank + '%'");
                        p.Add(dl.Param("@Bank", text));
                    }
                    break;
            }

            query.Append(" ORDER BY s.EntryDate DESC");

            return dl.ExecuteDataTable(query.ToString(), p.ToArray());
        }

        //  RECEIPT
        //------------------------------------------------------------------------

        // Returns null when the serial number is invalid or the record doesn't exist
        public GC.ReceiptGC GetReceipt(string serialNo)
        {
            int serial;
            if (!int.TryParse((serialNo ?? "").Trim(), out serial))
                return null;

            string query = @"
        SELECT TOP 1
            SerialNo, RegisterNo, DevoteeName, Address, Remarks,
            TRY_CONVERT(decimal(18,2), Price) AS Price,
            TRY_CONVERT(decimal(18,2), AdditionalAmount) AS AdditionalAmount,
            PaymentMode,
            (ISNULL(TRY_CONVERT(decimal(18,2), Price), 0) +
             ISNULL(TRY_CONVERT(decimal(18,2), AdditionalAmount), 0)) AS Total
        FROM SevaMemberDetails
        WHERE SerialNo = @SerialNo";

            DataTable dt = dl.ExecuteDataTable(query, new[]
            {
        new SqlParameter("@SerialNo", SqlDbType.Int) { Value = serial }
    });

            if (dt.Rows.Count == 0) return null;

            DataRow r = dt.Rows[0];

            decimal price = r["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Price"]);
            decimal additional = r["AdditionalAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(r["AdditionalAmount"]);
            decimal total = r["Total"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Total"]);

            return new GC.ReceiptGC
            {
                SerialNo = Convert.ToString(r["SerialNo"]),
                RegisterNo = Convert.ToString(r["RegisterNo"]),
                DevoteeName = Convert.ToString(r["DevoteeName"]),
                Address = Convert.ToString(r["Address"]),
                Particular = Convert.ToString(r["Remarks"]),
                PaymentMode = Convert.ToString(r["PaymentMode"]),
                Amount = price.ToString("N2"),
                AdditionalAmount = additional.ToString("N2"),
                Total = total.ToString("N2"),
                AmountInWords = ConvertToWords(price + additional)   // words are based on the TOTAL
            };
        }

        // Insert or update; returns the SerialNo used
        public string SaveReceipt(GC.ReceiptGC m)
        {
            // Amount is stored as "1,500.00" text, so parse it with the same format
            decimal price;
            if (!decimal.TryParse(m.Amount, NumberStyles.Number, CultureInfo.InvariantCulture, out price))
                price = 0;

            // New entry: generate the next serial number
            if (string.IsNullOrEmpty(m.SerialNo))
            {
                m.SerialNo = Convert.ToString(dl.ExecuteScalar(
                    "SELECT ISNULL(MAX(TRY_CAST(SerialNo AS INT)), 0) + 1 FROM SevaMemberDetails"));
            }

            string query = @"
        IF EXISTS (SELECT 1 FROM SevaMemberDetails WHERE SerialNo = @SerialNo)
        BEGIN
            UPDATE SevaMemberDetails SET
                DevoteeName = @DevoteeName,
                Address = @Address,
                Remarks = @Remarks,
                Price = @Price,
                PaymentMode = @PaymentMode
            WHERE SerialNo = @SerialNo
        END
        ELSE
        BEGIN
            INSERT INTO SevaMemberDetails
                (SerialNo, DevoteeName, Address, Remarks, Price, PaymentMode)
            VALUES
                (@SerialNo, @DevoteeName, @Address, @Remarks, @Price, @PaymentMode)
        END";

            dl.ExecuteNonQuery(query, new[]
            {
        dl.Param("@SerialNo", m.SerialNo),
        dl.Param("@DevoteeName", m.DevoteeName),
        dl.Param("@Address", m.Address),
        dl.Param("@Remarks", m.Particular),
        dl.Param("@Price", price),
        dl.Param("@PaymentMode", m.PaymentMode)
    });

            return m.SerialNo;
        }

        // Amount in words (Indian system: Thousand, Lakh, Crore)
        private string ConvertToWords(decimal amount)
        {
            if (amount == 0) return "Zero Rupees";

            string[] units = { "Zero", "One", "Two", "Three", "Four", "Five", "Six",
                       "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
                       "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
                       "Eighteen", "Nineteen" };

            string[] tens = { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty",
                      "Sixty", "Seventy", "Eighty", "Ninety" };

            Func<int, string> convert = null;

            convert = num =>
            {
                if (num < 20) return units[num];
                if (num < 100)
                    return tens[num / 10] + (num % 10 > 0 ? " " + units[num % 10] : "");
                if (num < 1000)
                    return units[num / 100] + " Hundred" + (num % 100 > 0 ? " and " + convert(num % 100) : "");
                if (num < 100000)
                    return convert(num / 1000) + " Thousand" + (num % 1000 > 0 ? " " + convert(num % 1000) : "");
                if (num < 10000000)
                    return convert(num / 100000) + " Lakh" + (num % 100000 > 0 ? " " + convert(num % 100000) : "");
                return convert(num / 10000000) + " Crore" + (num % 10000000 > 0 ? " " + convert(num % 10000000) : "");
            };

            int rupees = (int)amount;
            int paise = (int)((amount - rupees) * 100);

            string words = convert(rupees) + " Rupees";
            if (paise > 0)
                words += " and " + convert(paise) + " Paise";

            return words;
        }

        //  SEVA MASTER LIST
        // =========================================================
        public List<GC.SevaMasterGC> GetSevaMasterList()
        {
            string query = @"
        SELECT TOP (1000)
            SevaID, SevaName, SevaDescription, SevaType, SevaCategory,
            SevaAmount, AvailabilityDays, PrasadaDetails, IsActive,
            CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
        FROM SevaMaster";

            DataTable dt = dl.ExecuteDataTable(query);
            List<GC.SevaMasterGC> list = new List<GC.SevaMasterGC>();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new GC.SevaMasterGC
                {
                    SevaID = Convert.ToInt32(r["SevaID"]),
                    SevaName = Convert.ToString(r["SevaName"]),
                    SevaDescription = Convert.ToString(r["SevaDescription"]),
                    SevaType = Convert.ToString(r["SevaType"]),
                    SevaCategory = Convert.ToString(r["SevaCategory"]),
                    SevaAmount = r["SevaAmount"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["SevaAmount"]),
                    AvailabilityDays = Convert.ToString(r["AvailabilityDays"]),
                    PrasadaDetails = Convert.ToString(r["PrasadaDetails"]),
                    IsActive = r["IsActive"] != DBNull.Value && Convert.ToBoolean(r["IsActive"]),
                    CreatedDate = r["CreatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["CreatedDate"]),
                    CreatedBy = Convert.ToString(r["CreatedBy"]),
                    UpdatedDate = r["UpdatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["UpdatedDate"]),
                    UpdatedBy = Convert.ToString(r["UpdatedBy"])
                });
            }

            return list;
        }

        // Returns rows deleted (0 = not found)
        public int DeleteSevaMaster(int sevaId)
        {
            return dl.ExecuteNonQuery(
                "DELETE FROM SevaMaster WHERE SevaID = @SevaID",
                new[] { dl.Param("@SevaID", sevaId) });
        }
        //  USER MASTER
        // =========================================================

        // Returns null when the user doesn't exist (password is never returned)
        public GC.UserMasterGC GetUserById(int id)
        {
            DataTable dt = dl.ExecuteDataTable(
                "SELECT * FROM Usermaster WHERE id = @id",
                new[] { dl.Param("@id", id) });

            if (dt.Rows.Count == 0) return null;

            DataRow r = dt.Rows[0];

            return new GC.UserMasterGC
            {
                Id = id,
                UserName = Convert.ToString(r["UserName"]),
                FirstName = Convert.ToString(r["FirstName"]),
                LastName = Convert.ToString(r["LastName"]),
                MobileNo = Convert.ToString(r["MobileNo"]),
                Email = Convert.ToString(r["Email"]),
                InstituteCode = Convert.ToString(r["InstituteCode"]),
                UserType = Convert.ToString(r["user_type"]),
                UserId = Convert.ToString(r["user_id"]),
                ProfileImage = Convert.ToString(r["ProfileImage"]),
                IsActive = r["isActive"] != DBNull.Value && Convert.ToBoolean(r["isActive"])
            };
        }

        // Insert (Id == 0) or update (Id > 0)
        public void SaveUser(GC.UserMasterGC m)
        {
            bool isEdit = m.Id > 0;
            bool hasPassword = !string.IsNullOrWhiteSpace(m.Password);
            bool hasImage = !string.IsNullOrWhiteSpace(m.ProfileImage);

            if (!isEdit && !hasPassword)
                throw new ArgumentException("Password is required for a new user.");

            var p = new List<SqlParameter>
            {
                dl.Param("@UserName",(m.UserName ?? "").Trim()),
                dl.Param("@FirstName",(m.FirstName ?? "").Trim()),
                dl.Param("@LastName", (m.LastName ?? "").Trim()),
                dl.Param("@MobileNo",(m.MobileNo ?? "").Trim()),
                dl.Param("@Email",(m.Email ?? "").Trim()),
                dl.Param("@InstituteCode",(m.InstituteCode ?? "").Trim()),
                dl.Param("@user_type", m.UserType),
                dl.Param("@user_id",(m.UserId ?? "").Trim()),
                dl.Param("@isActive",m.IsActive)
            };

            string query;

            if (isEdit)
            {
                query = @"
            UPDATE Usermaster SET
                UserName = @UserName,
                FirstName = @FirstName,
                LastName = @LastName,
                MobileNo = @MobileNo,
                Email = @Email,
                InstituteCode = @InstituteCode,
                user_type = @user_type,
                user_id = @user_id,
                isActive = @isActive";

                // Password only if entered
                if (hasPassword)
                {
                    query += ", PasswordHash = @PasswordHash";
                    p.Add(dl.Param("@PasswordHash", HashPassword(m.Password.Trim())));
                }

                // Image only if uploaded
                if (hasImage)
                {
                    query += ", ProfileImage = @ProfileImage";
                    p.Add(dl.Param("@ProfileImage", m.ProfileImage));
                }

                query += " WHERE id = @id";
                p.Add(dl.Param("@id", m.Id));
            }
            else
            {
                query = @"
            INSERT INTO Usermaster
                (UserName, PasswordHash, ProfileImage, FirstName, LastName,
                 MobileNo, Email, InstituteCode, user_type, user_id, isActive)
            VALUES
                (@UserName, @PasswordHash, @ProfileImage, @FirstName, @LastName,
                 @MobileNo, @Email, @InstituteCode, @user_type, @user_id, @isActive)";

                p.Add(dl.Param("@PasswordHash", HashPassword(m.Password.Trim())));
                p.Add(dl.Param("@ProfileImage", m.ProfileImage ?? ""));
            }

            dl.ExecuteNonQuery(query, p.ToArray());
        }

        // SHA-256 hex (same format as the original, so existing logins keep working)
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();

                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        //  USER MASTER LIST
        // =========================================================

        // Password hash is deliberately NOT selected, so it can never reach the page
        public DataTable GetUserList()
        {
            string query = @"
        SELECT id, UserName, FirstName, LastName, MobileNo, Email,
               InstituteCode, user_type, user_id, ProfileImage, isActive
        FROM Usermaster
        ORDER BY id DESC";

            return dl.ExecuteDataTable(query);
        }

        // Returns rows deleted (0 = not found)
        public int DeleteUser(int id)
        {
            return dl.ExecuteNonQuery(
                "DELETE FROM Usermaster WHERE id = @id",
                new[] { dl.Param("@id", id) });
        }
    }
}
