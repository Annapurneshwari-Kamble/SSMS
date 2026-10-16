<%@ Page Title="REPORT" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="SSMS.Pages.Admin.Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table > :not(caption) > * > * {
            border: 1px solid #dee2e6 !important;
        }

        .table {
            margin-bottom: 0;
        }

        .card-body {
            padding: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField
        ID="hfLeftImage"
        runat="server"
        Value="../../assets/images/sri-siddharrodha(1).png" />

    <asp:HiddenField
        ID="hfRightImage"
        runat="server"
        Value="../../assets/images/gurunathswamij(1).png" />


    <div class="container-fluid py-4">
        <!-- Header Section -->
        <div class="row mb-4">
            <div class="col">
                <div class="d-flex justify-content-between align-items-center">
                    <h2 class="h3 mb-0 text-primary">Seva Reports</h2>
                    <div class="bg-light rounded p-2">
                        <small class="text-muted">Generate and export seva reports</small>
                    </div>
                </div>
            </div>
        </div>


        <!-- Filter Card -->
        <div class="card shadow-sm mb-4">
            <div class="card-header bg-white py-3">
                <h5 class="card-title mb-0">Report Filters</h5>
            </div>

            <div class="card-body">
                <div class="row g-3">

                    <!-- Seva Type -->
                    <div class="col-md-3">
                        <label for="<%= ddlSevaType.ClientID %>"
                            class="form-label fw-semibold">
                            Seva Type
                        </label>

                        <asp:DropDownList
                            ID="ddlSevaType"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlSevaType_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <!-- Event Name -->
                    <div class="col-md-3">
                        <label for="<%= ddlEventName.ClientID %>"
                            class="form-label fw-semibold">
                            Event Name
                        </label>

                        <asp:DropDownList
                            ID="ddlEventName"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <!-- From Date -->
                    <div class="col-md-3">
                        <label for="<%= txtFromDate.ClientID %>"
                            class="form-label fw-semibold">
                            From Date
                        </label>

                        <asp:TextBox
                            ID="txtFromDate"
                            runat="server"
                            CssClass="form-control"
                            TextMode="Date">
                        </asp:TextBox>
                    </div>

                    <!-- To Date -->
                    <div class="col-md-3">
                        <label for="<%= txtToDate.ClientID %>"
                            class="form-label fw-semibold">
                            To Date
                        </label>

                        <asp:TextBox
                            ID="txtToDate"
                            runat="server"
                            CssClass="form-control"
                            TextMode="Date">
                        </asp:TextBox>
                    </div>

                </div>

           
                <!-- Action Buttons: First Line -->
                <div class="row mt-4 pt-3 border-top">
                    <div class="col text-center">

                        <asp:Button
                            ID="btnGenerate"
                            runat="server"
                            Text="Generate Report"
                            CssClass="btn btn-primary px-4 me-2"
                            OnClick="btnGenerate_Click"
                            CausesValidation="false" />

                        <asp:Button
                            ID="btnExportExcel"
                            runat="server"
                            Text="Export to Excel"
                            CssClass="btn btn-outline-success px-4 me-2"
                            OnClick="btnExportExcel_Click"
                            CausesValidation="false" />

                        <asp:Button
                            ID="btnExportPPT"
                            runat="server"
                            Text="Export to PPT"
                            CssClass="btn btn-outline-warning px-4 me-2"
                            OnClick="btnExportPPT_Click"
                            CausesValidation="false" />

                        <!-- NEW UPDATE BUTTON -->
                        <asp:Button
                            ID="btnUpdateDates"
                            runat="server"
                            Text="Update Dates"
                            CssClass="btn btn-outline-danger px-4 me-2"
                            OnClick="btnUpdateDates_Click"
                            CausesValidation="false"
                            OnClientClick="return confirm('Are you sure you want to update the dates in the selected range to the current year?');" />

                    </div>
                </div>

                <!-- Excel Import: Second Line -->
                <div class="row mt-3">
                    <div class="col text-center">

                        <asp:FileUpload
                            ID="fuExcel"
                            runat="server"
                            CssClass="form-control d-inline-block"
                            Style="width: 260px; display: inline-block !important;"
                            accept=".xlsx,.xls" />

                        <asp:Button
                            ID="btnImportExcel"
                            runat="server"
                            Text="Import Excel"
                            CssClass="btn btn-success px-4 ms-2"
                            OnClick="btnImportExcel_Click"
                            CausesValidation="false" />

                        <br />

                        <asp:Label
                            ID="lblImportMessage"
                            runat="server"
                            CssClass="mt-3 d-inline-block" />

                        <!-- NEW UPDATE MESSAGE -->
                        <br />

                        <asp:Label
                            ID="lblUpdateMessage"
                            runat="server"
                            CssClass="mt-3 d-inline-block fw-bold" />

                    </div>
                </div>
            </div>
        </div>

        <!-- Results Section -->
        <div class="card shadow-sm">
            <div class="card-header bg-white py-3">
                <h5 class="card-title mb-0">Report Results</h5>
            </div>
            <div class="card-body p-0">
                <div class="table-responsive">
                    <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="true"
                        CssClass="table table-hover table-bordered mb-0"
                        GridLines="None">
                        <HeaderStyle CssClass="table-light" />
                        <AlternatingRowStyle CssClass="table-light" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
