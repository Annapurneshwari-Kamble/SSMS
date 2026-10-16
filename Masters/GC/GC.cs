using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSMS.Masters.GC
{
    public class GC
    {
        public string SerialNo { get; set; }
        public string Language { get; set; }
        public string SevaType { get; set; }
        public string SevaFor { get; set; }
        public string EventName { get; set; }
        public string RegisterNo { get; set; }
        public string DevoteeName { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Remarks { get; set; }
        public string PaymentMode { get; set; }
        public string UTRNumber { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string IFSCCode { get; set; }
        public string AccountNumber { get; set; }

        // Raw inputs (parsed and validated in BL)
        public string EntryDateText { get; set; }
        public string PriceText { get; set; }
        public string SevaStart { get; set; }          // "ThisYear" / "NextYear"
        public string AdditionalAmountText { get; set; }

        // Parsed by BL.Validate()
        public DateTime EntryDate { get; set; }
        public decimal Price { get; set; }
        public decimal AdditionalAmount { get; set; }
        public decimal Total { get; set; }
    }
    public class AllListGC
    {
        public string DevoteeName { get; set; }
        public string SevaType { get; set; }
        public string Address { get; set; }
    }
    public class DashboardGC
    {
        public int TotalAbhishek { get; set; }
        public int TotalAnnaprasad { get; set; }
        public int TotalOtherSevas { get; set; }

        public int TotalSeva
        {
            get { return TotalAbhishek + TotalAnnaprasad + TotalOtherSevas; }
        }
    }
    public class SevaFilterGC
    {
        public string Condition { get; set; }   // "AND" / "OR" (advanced search only)
        public string Column { get; set; }      // e.g. "DevoteeName"
        public string Value { get; set; }
    }
    public class PaymentFilterGC
    {
        // "RegisterNo", "Date", "SevaType", "PaymentMode",
        // "DevoteeName", "UTRNumber", "BankName" (or empty for no filter)
        public string FilterType { get; set; }

        // Text value (RegisterNo, Date, DevoteeName, UTRNumber, BankName)
        public string Value { get; set; }

        // Dropdown values
        public string SevaType { get; set; }
        public string PaymentMode { get; set; }
    }

    public class ReceiptGC
    {
        public string SerialNo { get; set; }
        public string RegisterNo { get; set; }
        public string DevoteeName { get; set; }
        public string Address { get; set; }
        public string Particular { get; set; }
        public string Amount { get; set; }
        public string AmountInWords { get; set; }
        public string PaymentMode { get; set; }
        public string Total { get; set; }
        public string AdditionalAmount { get; set; }
    }
    public class SevaMasterGC
    {
        public int SevaID { get; set; }
        public string SevaName { get; set; }
        public string SevaDescription { get; set; }
        public string SevaType { get; set; }
        public string SevaCategory { get; set; }
        public decimal? SevaAmount { get; set; }
        public string AvailabilityDays { get; set; }
        public string PrasadaDetails { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
    public class UserMasterGC
    {
        public int Id { get; set; }              // 0 = new user, > 0 = edit
        public string UserName { get; set; }
        public string Password { get; set; }     // plain text from the form, hashed in BL
        public string ProfileImage { get; set; } // saved path, empty = no new image
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string InstituteCode { get; set; }
        public string UserType { get; set; }
        public string UserId { get; set; }
        public bool IsActive { get; set; }
    }
}