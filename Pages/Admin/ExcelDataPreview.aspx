<%@ Page Title="Excel Data Preview" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="ExcelDataPreview.aspx.cs" Inherits="SSMS.Pages.Admin.ExcelDataPreview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .bi-cloud-upload {
            color: #212529 !important;
        }
    </style>
    <link rel="stylesheet"
      href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid px-4 py-4">
        <!-- Page Heading -->
        <div class="d-flex flex-wrap justify-content-between align-items-center mb-4">
            <div>
                <h3 class="fw-bold mb-1 text-primary">
                    <i class="bi bi-file-earmark-excel me-2"></i>Excel Data Preview
                </h3>
                <p class="text-muted mb-0">
                    Upload and preview old Seva Excel records.
                </p>
            </div>
        </div>

        <!-- Upload Section Card -->
        <div class="card border-0 shadow-sm rounded-4 mb-4">
            <div class="card-body p-4 p-xl-5">

                <!-- Card Heading -->

                <h3 class="fw-semibold mb-4 d-flex align-items-center">
                    <span class="bg-primary bg-opacity-10 p-3 rounded-3 me-3">
                        <i class="bi bi-cloud-upload text-dark fs-4"></i>
                    </span>
                    Import Excel File
                </h3>

                <!-- Upload Section -->
                <div class="row g-3 align-items-start">

                    <!-- Excel File Upload -->
                    <div class="col-lg-8 col-md-7">
                        <label class="form-label fw-semibold small text-uppercase text-secondary mb-2">
                            Choose Excel File
                        </label>

                        <div class="input-group">
                            <span class="input-group-text bg-light border-end-0">
                                <i class="bi bi-file-earmark-excel text-success"></i>
                            </span>

                            <asp:FileUpload
                                ID="fuExcel"
                                runat="server"
                                CssClass="form-control border-start-0"
                                accept=".xlsx" />
                        </div>

                        <div class="form-text mt-2">
                            <i class="bi bi-info-circle me-1"></i>
                            Select an Excel workbook (.xlsx). Data will be displayed
                    without saving to the database.
                        </div>
                    </div>

                    <!-- Import Button -->
                    <div class="col-lg-4 col-md-5">
                        <!-- Spacer to align with the file input -->
                        <label class="form-label mb-2 invisible">
                            Import Excel
                        </label>

                        <asp:Button
                            ID="btnImport"
                            runat="server"
                            Text="&#xf1c3; Import &amp; Preview"
                            CssClass="btn btn-primary w-100 py-2 fw-semibold shadow-sm"
                            OnClick="btnImport_Click" />
                    </div>

                </div>

                <!-- Status Message -->
                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    EnableViewState="false"
                    CssClass="d-block mt-4 alert" />

            </div>
        </div>

        <!-- Header photos hidden fields (kept as is) -->
        <asp:HiddenField ID="hfLeftImage" runat="server" Value="../../assets/images/sri-siddharrodha(1).png" />
        <asp:HiddenField ID="hfRightImage" runat="server" Value="../../assets/images/gurunathswamij(1).png" />

        <!-- Preview Section -->
        <asp:Panel ID="pnlPreview" runat="server" Visible="false">
            <div class="card border-0 shadow-sm rounded-4 overflow-hidden">
                <!-- Card Header with Filters and Actions -->
                <div class="card-header bg-white border-bottom p-4">
                    <div class="d-flex flex-wrap justify-content-between align-items-center gap-3">
                        <div class="d-flex align-items-center gap-3">
                            <h5 class="fw-semibold mb-0 d-flex align-items-center">
                                <i class="bi bi-table me-2 text-primary"></i>
                                Imported Records
                            </h5>
                            <asp:Label ID="lblRecordCount" runat="server"
                                CssClass="badge bg-primary bg-opacity-10 text-primary fs-6 fw-normal px-3 py-2 rounded-pill" />
                        </div>

                        <div class="d-flex flex-wrap align-items-center gap-2">
                            <div class="input-group input-group-sm">
                                <span class="input-group-text bg-light border-end-0">
                                    <i class="bi bi-funnel"></i>
                                </span>
                                <asp:DropDownList ID="ddlSevaType" runat="server"
                                    CssClass="form-select border-start-0" Style="min-width: 180px;">
                                    <asp:ListItem Text="Nirantara Abhishek" Value="Nirantara Abhishek" />
                                    <asp:ListItem Text="Nirantara Annaprasad" Value="Nirantara Annaprasad" />
                                    <asp:ListItem Text="Other Sevas" Value="Other Sevas" />
                                </asp:DropDownList>
                            </div>

                            <div class="input-group input-group-sm">
                                <span class="input-group-text bg-light border-end-0">
                                    <i class="bi bi-calendar-event"></i>
                                </span>
                                <asp:TextBox ID="txtToDate" runat="server" TextMode="Date"
                                    CssClass="form-control border-start-0" Style="min-width: 150px;" />
                            </div>

                            <asp:Button ID="btnExportPPT" runat="server" Text="Generate PPT"
                                CssClass="btn btn-success btn-sm px-3 py-2 fw-semibold shadow-sm"
                                OnClick="btnExportPPT_Click" />
                        </div>
                    </div>
                </div>

                <!-- Card Body with Scrollable Grid -->
                <div class="card-body p-0">
                    <div class="table-responsive" style="max-height: 600px; overflow-y: auto;">
                        <asp:GridView ID="gvExcelData" runat="server"
                            AutoGenerateColumns="true"
                            CssClass="table table-bordered table-hover table-striped align-middle mb-0"
                            GridLines="None"
                            EmptyDataText="No records found."
                            UseAccessibleHeader="true">

                            <HeaderStyle CssClass="table-primary text-nowrap" />

                            <RowStyle CssClass="text-nowrap" />

                            <EmptyDataRowStyle CssClass="text-center text-muted p-5" />

                            <PagerStyle CssClass="pagination justify-content-center" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>

    <!-- Optional: Add a small script to auto-dismiss alert messages after a few seconds -->
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            // Find the message label and if it has content, make it an alert that fades
            var lblMsg = document.getElementById('<%= lblMessage.ClientID %>');
            if (lblMsg && lblMsg.innerText.trim() !== '') {
                lblMsg.classList.add('alert-info', 'alert-dismissible', 'fade', 'show');
                // Optionally auto-dismiss after 5 seconds (uncomment if desired)
                // setTimeout(function () {
                //     lblMsg.classList.remove('show');
                //     setTimeout(function () { lblMsg.style.display = 'none'; }, 150);
                // }, 5000);
            }
        });
    </script>
</asp:Content>
