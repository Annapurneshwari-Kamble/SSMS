<%@ Page Title="NIRANTARA ANNAPRASAD" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="Nirantara_Annaprasad.aspx.cs" Inherits="SSMS.Pages.Admin.Nirantara_Annaprasad" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-5">
        <div class="row justify-content-center">
            <div class="col-md-8 col-lg-6">
                <div class="card shadow-lg border-0 rounded-3">
                    <div class="card-header bg-success text-white text-center">
                        <h4 class="mb-0">Nirantara Annaprasad Details</h4>
                    </div>
                    <div class="card-body p-4">

                        <div class="mb-3">
                            <label for="txtDevoteeName" class="form-label">Devotee Name</label>
                            <asp:TextBox ID="txtDevoteeName" runat="server" CssClass="form-control" />
                        </div>

                        <div class="mb-3">
                            <label for="txtSevaType" class="form-label">Seva Type</label>
                            <asp:TextBox ID="txtSevaType" runat="server" CssClass="form-control" />
                        </div>

                        <div class="mb-3">
                            <label for="txtSevaCategory" class="form-label">Seva Category</label>
                            <asp:TextBox ID="txtSevaCategory" runat="server" CssClass="form-control" />
                        </div>

                        <div class="mb-3">
                            <label for="txtSevaAmount" class="form-label">Seva Amount</label>
                            <asp:TextBox ID="txtSevaAmount" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>

                        <div class="mb-3">
                            <label for="txtAvailabilityDays" class="form-label">Availability Days</label>
                            <asp:TextBox ID="txtAvailabilityDays" runat="server" CssClass="form-control" />
                        </div>

                        <div class="mb-3">
                            <label for="txtPrasadaDetails" class="form-label">Prasada Details</label>
                            <asp:TextBox ID="txtPrasadaDetails" runat="server" CssClass="form-control" />
                        </div>

                        <div class="form-check mb-3">
                            <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" />
                            <label class="form-check-label" for="chkIsActive">Is Active</label>
                        </div>

                        <div class="d-grid">
                            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary btn-lg" Text="Save Seva" OnClick="btnSave_Click" />
                        </div>

                        <asp:Label ID="lblMessage" runat="server" CssClass="mt-3 d-block text-center fw-bold"></asp:Label>

                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
