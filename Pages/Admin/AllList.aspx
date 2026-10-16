<%@ Page Title="ALL DEVOTEES LIST" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" 
    AutoEventWireup="true" CodeBehind="AllList.aspx.cs" Inherits="SSMS.Pages.Admin.AllList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function printReport() {
            var divContents = document.getElementById('reportDiv').innerHTML;
            var printWindow = window.open('', '', 'height=600,width=800');
            printWindow.document.write('<html><head><title>Devotees Report</title>');
            printWindow.document.write('<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css">');
            printWindow.document.write('</head><body class="p-3">');
            printWindow.document.write(divContents);
            printWindow.document.write('</body></html>');
            printWindow.document.close();
            printWindow.print();
        }
        function scrollToReport() {
            var reportDiv = document.getElementById('reportDiv');
            if (reportDiv) {
                reportDiv.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4">
        <!-- Page Header -->
        <div class="d-flex justify-content-between align-items-center mb-4">
            <div>
                <h1 class="h3 mb-1 text-primary">Devotee Management</h1>
                <p class="text-muted">View and manage all devotees and their seva information</p>
            </div>
            
        </div>

        <!-- Search Card -->
        <div class="card shadow-sm mb-4">
            <div class="card-header bg-light py-3">
                <h5 class="card-title mb-0">Filter Devotees</h5>
            </div>
            <div class="card-body">
                <div class="row g-3 align-items-end">
                    <div class="col-md-4">
                        <label for="<%= txtDevoteeName.ClientID %>" class="form-label fw-semibold">Devotee Name</label>
                        <asp:TextBox ID="txtDevoteeName" runat="server" CssClass="form-control" placeholder="Search by Devotee Name..."></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label for="<%= ddlSevaType.ClientID %>" class="form-label fw-semibold">Seva Type</label>
                        <asp:DropDownList ID="ddlSevaType" runat="server" CssClass="form-select">
                            <asp:ListItem Text="-- Select Seva Type --" Value="" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <div class="d-flex gap-2">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" 
                                CssClass="btn btn-primary flex-fill" OnClick="btnSearch_Click" />
                            <asp:Button ID="btnReset" runat="server" Text="Clear" 
                                CssClass="btn btn-outline-secondary" OnClick="btnReset_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Data Table Card -->
        <div class="card shadow-sm">
            <div class="card-header bg-light py-3 d-flex justify-content-between align-items-center">
                <h5 class="card-title mb-0">Devotee List</h5>
                
    <div class="d-flex align-items-center gap-3">

        <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report"
            CssClass="btn btn-primary"
            OnClick="btnGenerateReport_Click" />

        <div class="form-check form-switch mb-0">
            <input class="form-check-input" type="checkbox" id="selectAllCheckbox">
            <label class="form-check-label small" for="selectAllCheckbox">
                Select All
            </label>
        </div>

    </div>
            </div>
            <div class="card-body p-0">
                <div class="table-responsive">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                        CssClass="table table-hover align-middle mb-0">
                        <Columns>
                            <asp:TemplateField HeaderText="Select">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" CssClass="form-check-input row-select" />
                                </ItemTemplate>
                                <HeaderStyle Width="60px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="DevoteeName" HeaderText="Devotee Name" 
                                HeaderStyle-CssClass="fw-semibold" />
                            <asp:BoundField DataField="SevaType" HeaderText="Seva Type" 
                                HeaderStyle-CssClass="fw-semibold" />
                            <asp:BoundField DataField="Address" HeaderText="Address" 
                                HeaderStyle-CssClass="fw-semibold" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center py-5">
                                <i class="fas fa-users fa-3x text-muted mb-3"></i>
                                <h5 class="text-muted">No devotees found</h5>
                                <p class="text-muted">Try adjusting your search criteria</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- Report Section -->
        <div id="reportDiv" class="mt-5">
            <asp:Literal ID="litReport" runat="server" />
        </div>

        <!-- Action Buttons -->
        <div class="mt-4 d-flex justify-content-end gap-2" style="margin-bottom:80px"; >
            <asp:Button ID="btnPrint" runat="server" Text="Print Report" 
                CssClass="btn btn-outline-primary" OnClientClick="printReport(); return false;" 
                Visible="false" />
            <%--<asp:Button ID="btnExportPDF" runat="server" Text="Export to PDF" 
                CssClass="btn btn-danger" OnClick="btnExportPDF_Click" Visible="false" />--%>
        </div>
    </div>

    <script>
        document.addEventListener("DOMContentLoaded", function () {

            const selectAll = document.getElementById("selectAllCheckbox");

            selectAll.addEventListener("change", function () {

                // Select all checkboxes rendered by ASP.NET GridView
                document.querySelectorAll("input[id*='chkSelect']").forEach(cb => {
                    cb.checked = selectAll.checked;
                });

            });

        });
    </script>

</asp:Content>