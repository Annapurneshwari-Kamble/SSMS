<%@ Page Title="RECEIPT" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="Receipt.aspx.cs" Inherits="SSMS.Pages.Admin.Receipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        @page {
            size: A4;
            margin: 0;
        }

        body {
            font-family: Arial, sans-serif;
            font-size: 13px;
        }

        .receipt {
            width: 21cm;
            height: 14.8cm;
            position: relative;
        }

        .receipt-title {
            position: absolute;
            top: 2.6cm; /* adjust as needed */
            left: 0;
            width: 100%;
            text-align: center;
            font-size: 18px;
            font-weight: bold;
            text-transform: uppercase;
        }

        .serial-no {
            position: absolute;
            top: 5.4cm;
            left: 1.8cm;
        }

        /* Positioning fields exactly as per measurements */

        .title-sr {
            position: absolute;
            top: 4.6cm;
            left: 1.9cm;
            font-weight: bold;
        }

        .vertical-line {
            position: absolute;
            top: 4.4cm;
            left: 2.8cm;
            width: 0.05cm; /* thin line */
            height: 5.6cm; /* vertical line height */
            border-left: 1px solid #000;
        }

        .donor-name {
            position: absolute;
            top: 3.3cm;
            left: 2cm;
        }

        .donor-address {
            position: absolute;
            top: 3.8cm;
            left: 2cm;
            width: 15cm; /* set a width so text can wrap */
            word-wrap: break-word; /* break long words if needed */
            white-space: normal; /* allow wrapping */
        }

        .voucher-no {
            position: absolute;
            top: 3.3cm;
            right: 2.5cm;
        }

        .voucher-date {
            position: absolute;
            top: 3.8cm;
            right: 2.5cm;
        }

        .address-line {
            position: absolute;
            top: 4.4cm;
            left: 1.8cm;
            right: 2.2cm;
            height: 0.05cm; /* thin line */
            border-top: 1px solid #000; /* black line */
        }

        .particulars-title {
            position: absolute;
            top: 4.6cm; /* adjust as per your design */
            left: 8.4cm;
            /*text-align: center;*/
            font-size: 14px;
            font-weight: bold;
        }

        .particulars {
            position: absolute;
            top: 5.4cm;
            left: 3cm;
            width: 10cm;
        }

        .particular-line {
            position: absolute;
            top: 5.2cm;
            left: 1.8cm;
            right: 2.2cm;
            height: 0.05cm; /* thin line */
            border-top: 1px solid #000; /* black line */
        }

        .title-amount {
            position: absolute;
            top: 4.6cm;
            right: 2.6cm;
            font-weight: bold;
            text-align: right;
            width: 3cm;
        }

        .amount {
            position: absolute;
            top: 5.4cm;
            right: 2.6cm;
            text-align: right;
            width: 3cm;
        }

        .total {
            position: absolute;
            top: 10.1cm;
            right: 2.6cm;
            font-weight: bold;
        }

        .amount-words {
            position: absolute;
            top: 10.1cm;
            left: 3cm;
            width: 15cm;
        }

        .payment-mode {
            position: absolute;
            top: 9.5cm;
            left: 3cm;
        }

        .payment-line {
            position: absolute;
            top: 10.0cm;
            left: 1.8cm;
            right: 2.2cm;
            height: 0.05cm; /* thin line */
            border-top: 1px solid #000; /* black line */
        }

        .vertical-line-amount {
            position: absolute;
            top: 4.4cm;
            right: 4.5cm;
            width: 0.05cm; /* thin line */
            height: 5.6cm; /* line height */
            border-left: 1px solid #000;
        }

        .total-title {
            position: absolute;
            top: 10.1cm;
            right: 4.7cm;
            font-weight: bold;
        }

        /*.total-line {
            position: absolute;
            top: 10.7cm;
            left: 0.9cm;
            right: 1.1cm;
            height: 0.05cm;*/ /* thin line */
        /*border-top: 1px solid #000;*/ /* black line */
        /*}*/
        .signature-payer {
            position: absolute;
            bottom: 3.2cm;
            left: 3cm;
        }

        .signature-auth {
            position: absolute;
            bottom: 3.2cm;
            right: 3cm;
        }

        .user-info {
            position: absolute;
            bottom: 3.2cm;
            left: 9cm;
            text-align: center;
        }

        @media print {
            .no-print {
                display: none;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="receipt">
        <div class="receipt-title">SEVA RECEIPT</div>
        <div class="serial-no"><%: Model.SerialNo %></div>
        <div class="title-sr">Sl.no</div>
        <div class="vertical-line"></div>
        <div class="donor-name">Devotee Name : <%: Model.DevoteeName %></div>
        <div class="donor-address">Devotee Address : <%: Model.Address %></div>
        <div class="address-line"></div>
        <div class="particulars-title">Particulars</div>
        <div class="particulars"><%: Model.Particular %></div>
        <div class="particular-line"></div>
        <div class="voucher-no">Reg.No: <%: Model.RegisterNo %></div>
        <div class="voucher-date">Date: <%= DateTime.Now.ToString("dd-MMM-yyyy") %></div>
        <div class="payment-mode">Received Through: <%: Model.PaymentMode %></div>
        <div class="payment-line"></div>
        <div class="vertical-line-amount"></div>
        <div class="title-amount">Amount</div>
        <div class="amount">₹ <%: Model.Amount %></div>
        <div class="amount-words">In Words: <%: Model.AmountInWords %> only</div>
        <div class="total-title">Total</div>
        <div class="total">₹ <%: Model.Total %></div>
        <%--<div class="total-line"></div>--%>
    </div>

    <!-- Print Button -->
    <div class="container my-3 no-print">
        <div class="d-flex justify-content-end gap-2">
            <%--<asp:Button ID="btnSaveReport" runat="server" CssClass="btn btn-success" Text="Save Report" OnClick="btnSaveReport_Click" />--%>
            <button type="button" class="btn btn-primary" onclick="printReceipt()">Print</button>
        </div>
    </div>

    <script type="text/javascript">
        function printReceipt() {
            var printContents = document.querySelector('.receipt').innerHTML;
            var originalContents = document.body.innerHTML;
            document.body.innerHTML = printContents;
            window.print();
            document.body.innerHTML = originalContents;
            //location.reload();
        }
    </script>
</asp:Content>
