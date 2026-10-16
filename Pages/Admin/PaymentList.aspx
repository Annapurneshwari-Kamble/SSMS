<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="PaymentList.aspx.cs" Inherits="SSMS.Pages.Admin.PaymentList" %>

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
        <div class="col-md-12">

            <div class="card shadow-lg border-0 rounded-3">

                <div class="card-header bg-primary text-white text-center">
                    <h4 class="mb-0">Payment Details List</h4>
                </div>

                <div class="card-body">
                    <div class="row mb-3 g-2">

                        <div class="col-md-3">
                            <asp:DropDownList ID="ddlFilterType" runat="server" CssClass="form-control"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlFilterType_SelectedIndexChanged">
                                <asp:ListItem Value="">Select Filter</asp:ListItem>
                                <asp:ListItem Value="RegisterNo">Register No</asp:ListItem>
                                <asp:ListItem Value="Date">Date</asp:ListItem>
                                <asp:ListItem Value="SevaType">Seva Type</asp:ListItem>
                                <asp:ListItem Value="PaymentMode">Payment Mode</asp:ListItem>
                                <asp:ListItem Value="DevoteeName">Devotee Name</asp:ListItem>
                                <asp:ListItem Value="UTRNumber">UTR Number</asp:ListItem>
                                <asp:ListItem Value="BankName">Bank Name</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-3">
                            <asp:TextBox ID="txtFilterValue" runat="server"
                                CssClass="form-control"
                                Placeholder="Enter Value"></asp:TextBox>

                            <asp:DropDownList ID="ddlPaymentModeFilter" runat="server"
                                CssClass="form-control" Visible="false">
                                <asp:ListItem Value="">Select Payment Mode</asp:ListItem>
                                <asp:ListItem>Cash</asp:ListItem>
                                <asp:ListItem>UPI</asp:ListItem>
                                <asp:ListItem>PayU</asp:ListItem>
                                <asp:ListItem>Bank Transfer</asp:ListItem>
                            </asp:DropDownList>

                            <asp:DropDownList ID="ddlSevaTypeFilter" runat="server"
                                CssClass="form-control" Visible="false">
                            </asp:DropDownList>
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

                    </div>
                    <asp:GridView ID="gvPayment" runat="server"
                        CssClass="table table-bordered table-striped text-center align-middle"
                        AutoGenerateColumns="false" AllowPaging="true" PageSize="50"
                        OnPageIndexChanging="gvPayment_PageIndexChanging">

                        <HeaderStyle CssClass="table-primary text-center" />

                        <Columns>

                            <asp:BoundField DataField="SerialNo" HeaderText="Serial No" />

                            <asp:BoundField DataField="RegisterNo"
                                HeaderText="Register No" />

                            <asp:BoundField DataField="EntryDate"
                                HeaderText="Date"
                                DataFormatString="{0:yyyy-MM-dd}" />

                            <asp:BoundField DataField="SevaType"
                                HeaderText="Seva Type" />

                            <asp:BoundField DataField="DevoteeName"
                                HeaderText="Devotee Name" />

                            <asp:BoundField DataField="Price"
                                HeaderText="Price"
                                DataFormatString="{0:N2}" />

                            <asp:BoundField DataField="PaymentMode"
                                HeaderText="Payment Mode" />

                            <asp:BoundField DataField="UTRNumber"
                                HeaderText="UTR Number" />

                            <asp:BoundField DataField="BankName"
                                HeaderText="Bank Name" />

                        </Columns>

                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
