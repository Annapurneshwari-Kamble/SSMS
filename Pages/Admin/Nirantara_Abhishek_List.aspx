<%@ Page Title="NIRANTARA ABHISHEK LIST" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="Nirantara_Abhishek_List.aspx.cs" Inherits="SSMS.Pages.Admin.Nirantara_Abhishek_List" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons/font/bootstrap-icons.css" rel="stylesheet" />
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
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="d-flex justify-content-center align-items-center mt-5">
        <div class="col-md-15 col-lg-12">
            <div class="card shadow-lg border-0 rounded-3">
                <div class="card-header bg-primary text-white text-center">
                    <h4 class="mb-0">Nirantara Abhishek List</h4>
                </div>
                <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold text-danger d-block mb-2"></asp:Label>
                <div class="card-body">
                    <!-- 🔍 Single-Line Search Bar (Wider Fields, Fits in One Row) -->
                    <div class="row mb-3 g-2 align-items-center">

                        <div class="col-md-3">
                            <asp:DropDownList ID="ddlFilterType" runat="server"
                                CssClass="form-control"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlFilterType_SelectedIndexChanged">

                                <asp:ListItem Value="">Select Filter</asp:ListItem>
                                <asp:ListItem Value="EntryDate">Date</asp:ListItem>
                                <asp:ListItem Value="RegisterNo">Register No</asp:ListItem>
                                <asp:ListItem Value="SerialNo">Serial No</asp:ListItem>
                                <asp:ListItem Value="DevoteeName">Devotee Name</asp:ListItem>
                                <asp:ListItem Value="EventName">Event</asp:ListItem>
                                <asp:ListItem Value="State">State</asp:ListItem>
                                <asp:ListItem Value="District">District</asp:ListItem>
                                <asp:ListItem Value="City">City</asp:ListItem>

                            </asp:DropDownList>
                        </div>

                        <div class="col-md-3">
                            <asp:TextBox ID="txtFilterValue"
                                runat="server"
                                CssClass="form-control"
                                Placeholder="Enter value"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <asp:Button ID="btnSearch" runat="server"
                                Text="Search"
                                CssClass="btn btn-primary w-100"
                                OnClick="btnSearch_Click" />
                        </div>

                        <div class="col-md-2">
                            <asp:Button ID="btnClear" runat="server"
                                Text="Clear"
                                CssClass="btn btn-secondary w-100"
                                OnClick="btnClear_Click" />
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnAdvancedSearch" runat="server"
                                Text="Advanced Search >>"
                                CssClass="btn btn-dark w-100"
                                OnClick="btnAdvancedSearch_Click"/>
                        </div>

                    </div>
                    <asp:Panel ID="pnlAdvancedSearch" runat="server" Visible="false" CssClass="card p-3 mb-3 border">

                        <asp:GridView ID="gvAdvancedFilters" runat="server" AutoGenerateColumns="false"
                            CssClass="table table-bordered"
                            OnRowDataBound="gvAdvancedFilters_RowDataBound">
                            <Columns>

                                
                                <asp:TemplateField HeaderText="Condition">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlCondition" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="AND">AND</asp:ListItem>
                                            <asp:ListItem Value="OR">OR</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                              
                                <asp:TemplateField HeaderText="Filter">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlColumn" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="SerialNo">Serial No</asp:ListItem>
                                            <asp:ListItem Value="RegisterNo">Register No</asp:ListItem>
                                            <asp:ListItem Value="DevoteeName">Devotee Name</asp:ListItem>
                                            <asp:ListItem Value="EventName">Event</asp:ListItem>
                                            <asp:ListItem Value="State">State</asp:ListItem>
                                            <asp:ListItem Value="District">District</asp:ListItem>
                                            <asp:ListItem Value="City">City</asp:ListItem>
                                            <asp:ListItem Value="EntryDate">Date</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Value">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtValue" runat="server" CssClass="form-control"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>

                        <div class="mt-2 text-end">
                            <asp:Button ID="btnApplyAdvancedSearch" runat="server"
                                Text="Apply Advanced Search"
                                CssClass="btn btn-success"
                                OnClick="btnApplyAdvancedSearch_Click"/>

                            <asp:Button ID="btnClearAdvancedSearch" runat="server"
                                Text="Clear Advanced Search"
                                CssClass="btn btn-secondary"
                                OnClick="btnClearAdvancedSearch_Click" />
                        </div>

                    </asp:Panel>

                    <div class="row mb-3">
                        <div class="col-md-3">
                            <asp:TextBox ID="txtBulkDate" runat="server"
                                CssClass="form-control"
                                TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="col-md-3">
                            <asp:Button ID="btnSetBulkDate" runat="server"
                                Text="Set Date for Selected"
                                CssClass="btn btn-success w-100"
                                OnClick="btnSetBulkDate_Click" />
                        </div>
                    </div>
                   <asp:GridView ID="gvAbhishek" runat="server"
                        DataKeyNames="SerialNo"
                       OnRowDataBound="gvAbhishek_RowDataBound"
                        CssClass="table table-bordered table-striped text-center align-middle"
                        AutoGenerateColumns="false" AllowPaging="true" PageSize="50"
                        OnPageIndexChanging="gvAbhishek_PageIndexChanging"
                        OnRowCommand="gvAbhishek_RowCommand">
                        <HeaderStyle CssClass="table-primary text-center" />
                        <Columns>
                            <asp:TemplateField HeaderText="Select">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkSelectAll" runat="server" onclick="toggleAll(this)" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="SerialNo" HeaderText="Serial No" />
                            <asp:BoundField DataField="RegisterNo" HeaderText="Register No" />
                            <asp:BoundField DataField="DevoteeName" HeaderText="Devotee Name" />
                            <asp:BoundField DataField="Language" HeaderText="Language" />
                            <asp:BoundField DataField="EventName" HeaderText="Event" />
                            <asp:BoundField DataField="State" HeaderText="State" />
                            <asp:BoundField DataField="District" HeaderText="District" />
                            <asp:BoundField DataField="City" HeaderText="City" />
                            <asp:BoundField DataField="ContactNumber" HeaderText="Contact" />
                            <asp:BoundField DataField="Price" HeaderText="Amount" DataFormatString="{0:N2}" />
                            <asp:TemplateField HeaderText="Date">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtEntryDate" runat="server"
                                        Text='<%# Bind("EntryDate", "{0:yyyy-MM-dd}") %>'
                                        TextMode="Date"
                                        CssClass="form-control">
                                    </asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnView" runat="server" CommandName="ViewRow" CommandArgument='<%# Eval("SerialNo") %>'
                                        CssClass="btn btn-sm btn-info me-2" ToolTip="View Letter">
                                        <i class="bi bi-eye"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditRow" CommandArgument='<%# Eval("SerialNo") %>'
                                        CssClass="btn btn-sm btn-warning me-2" ToolTip="Edit">
                                        <i class="bi bi-pencil"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("SerialNo") %>'
                                        CssClass="btn btn-sm btn-danger" ToolTip="Delete"
                                        OnClientClick="return confirm('Are you sure you want to delete this record?');">
                                        <i class="bi bi-trash"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
    function toggleAll(source) {
        var grid = document.getElementById('<%= gvAbhishek.ClientID %>');
        var checkboxes = grid.querySelectorAll('input[type="checkbox"]');

        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i] != source) {
                checkboxes[i].checked = source.checked;
            }
        }
    }
    </script>
</asp:Content>
