<%@ Page Title="Convert Kannada Data" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="ConvertKannadaData.aspx.cs" Inherits="SSMS.Pages.Admin.ConvertKannadaData" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .conversion-container {
            max-width: 900px;
            margin: 30px auto;
        }
        .preview-box {
            max-height: 400px;
            overflow-y: auto;
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 15px;
            font-family: 'Courier New', monospace;
            font-size: 14px;
        }
        .conversion-table {
            width: 100%;
        }
        .conversion-table td {
            padding: 5px 10px;
            border-bottom: 1px solid #e9ecef;
        }
        .old-text {
            color: #dc3545;
        }
        .new-text {
            color: #198754;
        }
        .status-badge {
            font-size: 12px;
            padding: 3px 10px;
            border-radius: 12px;
        }
        .status-success {
            background: #d1e7dd;
            color: #0f5132;
        }
        .status-warning {
            background: #fff3cd;
            color: #856404;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid pt-4 conversion-container">
        <div class="card shadow-lg border-0 rounded-4">
            <div class="card-header bg-gradient bg-primary text-white text-center py-4">
                <h3 class="mb-0 fw-bold">
                    <i class="bi bi-translate me-2"></i>Convert Imported Kannada Data
                </h3>
                <p class="mb-0 mt-2 opacity-75">Convert Nudi/Baraha ASCII Kannada to Unicode Kannada</p>
            </div>
            <div class="card-body p-4">
                <!-- Statistics -->
                <div class="row g-3 mb-4">
                    <div class="col-md-4">
                        <div class="card bg-light">
                            <div class="card-body text-center">
                                <h5 class="card-title text-primary">Total Records</h5>
                                <asp:Label ID="lblTotalRecords" runat="server" CssClass="display-6 fw-bold" Text="0" />
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="card bg-light">
                            <div class="card-body text-center">
                                <h5 class="card-title text-warning">Records to Convert</h5>
                                <asp:Label ID="lblRecordsToConvert" runat="server" CssClass="display-6 fw-bold text-warning" Text="0" />
                            </div>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="card bg-light">
                            <div class="card-body text-center">
                                <h5 class="card-title text-success">Converted</h5>
                                <asp:Label ID="lblConverted" runat="server" CssClass="display-6 fw-bold text-success" Text="0" />
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Preview Section -->
                <div class="mb-4">
                    <h5 class="fw-bold">
                        <i class="bi bi-eye me-2"></i>Preview (First 10 Records to Convert)
                    </h5>
                    <div class="preview-box">
                        <asp:Repeater ID="rptPreview" runat="server">
                            <HeaderTemplate>
                                <table class="conversion-table">
                                    <thead>
                                        <tr style="font-weight: bold; border-bottom: 2px solid #dee2e6;">
                                            <td>#</td>
                                            <td>SerialNo</td>
                                            <td>Current (Old Nudi)</td>
                                            <td>Will Become (Unicode)</td>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Container.ItemIndex + 1 %></td>
                                    <td><%# Eval("SerialNo") %></td>
                                    <td class="old-text"><%# Eval("OldDevoteeName") %></td>
                                    <td class="new-text"><%# Eval("NewDevoteeName") %></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                    </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lblNoRecords" runat="server" CssClass="text-muted" Text="No records found to convert." Visible="false" />
                    </div>
                </div>

                <!-- Action Buttons -->
                <div class="d-flex flex-wrap gap-3 justify-content-center mt-4">
                    <asp:Button ID="btnPreview" runat="server" 
                        CssClass="btn btn-info btn-lg px-5 py-3 fw-semibold rounded-3 shadow"
                        Text="Preview Conversion" OnClick="btnPreview_Click" />

                    <asp:Button ID="btnConvert" runat="server" 
                        CssClass="btn btn-success btn-lg px-5 py-3 fw-semibold rounded-3 shadow"
                        Text="Convert All Records" OnClick="btnConvert_Click" 
                        OnClientClick="return confirm('Are you sure you want to convert all imported records? This cannot be undone!');" />

                    <asp:Button ID="btnBackup" runat="server" 
                        CssClass="btn btn-secondary btn-lg px-5 py-3 fw-semibold rounded-3 shadow"
                        Text="Backup First" OnClick="btnBackup_Click" />
                </div>

                <!-- Message Display -->
                <div class="mt-4">
                    <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold fs-5" />
                </div>

                <!-- Progress (for large conversions) -->
                <div class="mt-3" id="progressContainer" style="display: none;">
                    <div class="progress" style="height: 25px;">
                        <div id="progressBar" class="progress-bar progress-bar-striped progress-bar-animated" 
                             role="progressbar" style="width: 0%;">
                            0%
                        </div>
                    </div>
                    <p class="text-muted mt-2 small">
                        <i class="bi bi-hourglass-split me-1"></i>
                        Converting <span id="convertedCount">0</span> of <span id="totalCount">0</span> records...
                    </p>
                </div>

            </div>
        </div>
    </div>

    <script>
        function updateProgress(current, total) {
            const container = document.getElementById('progressContainer');
            const progressBar = document.getElementById('progressBar');
            const convertedSpan = document.getElementById('convertedCount');
            const totalSpan = document.getElementById('totalCount');

            container.style.display = 'block';
            const percentage = Math.round((current / total) * 100);
            progressBar.style.width = percentage + '%';
            progressBar.textContent = percentage + '%';
            convertedSpan.textContent = current;
            totalSpan.textContent = total;
        }
    </script>
</asp:Content>