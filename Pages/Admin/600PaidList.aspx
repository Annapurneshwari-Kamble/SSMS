<%@ Page Title="Additional Amount List" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master"
    AutoEventWireup="true" CodeBehind="600PaidList.aspx.cs"
    Inherits="SSMS.Pages.Admin._600PaidList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid mt-3">
        <div class="card shadow-sm">
            <div class="d-flex justify-content-center align-items-center mt-5">
                <div class="col-md-15 col-lg-12">
                    <div class="card shadow-lg border-0 rounded-3">
                        <div class="card-header bg-primary text-white text-center">
                            <h4 class="mb-0">Additional Amount List</h4>
                        </div>

                    </div>
                </div>
            </div>
            <div class="card-body">

                <!-- Summary -->
                <div class="row g-3 mb-3">
                    <div class="col-md-4">
                        <div class="card border-primary">
                            <div class="card-body text-center">
                                <h6 class="text-muted">Total Records</h6>
                                <h4 class="text-primary mb-0">
                                    <asp:Label ID="lblRecordCount" runat="server" Text="0" />
                                </h4>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <div class="card border-success">
                            <div class="card-body text-center">
                                <h6 class="text-muted">Additional Amount</h6>
                                <h4 class="text-success mb-0">₹<asp:Label ID="lblAdditionalTotal" runat="server" Text="0.00" />
                                </h4>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <div class="card border-info">
                            <div class="card-body text-center">
                                <h6 class="text-muted">Total Amount</h6>
                                <h4 class="text-info mb-0">₹<asp:Label ID="lblGrandTotal" runat="server" Text="0.00" />
                                </h4>
                            </div>
                        </div>
                    </div>

                </div>

                <!-- Search -->
                <div class="row g-2 mb-3">

                    <div class="col-md-3">
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Search name / mobile" />
                    </div>

                    <div class="col-md-2">
                        <asp:TextBox ID="txtSerialNo" runat="server"
                            CssClass="form-control"
                            placeholder="Serial No." />
                    </div>

                    <div class="col-md-2">
                        <asp:TextBox ID="txtRegisterNo" runat="server"
                            CssClass="form-control"
                            placeholder="Register No." />
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

                <!-- Table -->
                <div class="table-responsive">

                    <asp:GridView ID="gvPaidList" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-hover table-striped align-middle mb-0"
                        EmptyDataText="No records found with Additional Amount."
                        GridLines="None"
                        CellPadding="8">

                        <Columns>

                            <asp:BoundField DataField="SerialNo"
                                HeaderText="Serial No."
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="RegisterNo"
                                HeaderText="Register No."
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="DevoteeName"
                                HeaderText="Devotee Name"
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="ContactNumber"
                                HeaderText="Contact Number"
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="SevaType"
                                HeaderText="Seva Type"
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="SevaFor"
                                HeaderText="Seva For"
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="EntryDate"
                                HeaderText="Date"
                                DataFormatString="{0:dd-MM-yyyy}"
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="Price"
                                HeaderText="Price"
                                DataFormatString="₹ {0:N2}"
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="AdditionalAmount"
                                HeaderText="Additional Amount"
                                DataFormatString="₹ {0:N2}"
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="Total"
                                HeaderText="Total"
                                DataFormatString="₹ {0:N2}"
                                ItemStyle-CssClass="border border-2 border-secondary" />

                            <asp:BoundField DataField="PaymentMode"
                                HeaderText="Payment Mode"
                                ItemStyle-CssClass="border border-2 border-secondary" />

                        </Columns>
                        <HeaderStyle CssClass="table-primary text-center border border-2 border-secondary" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
