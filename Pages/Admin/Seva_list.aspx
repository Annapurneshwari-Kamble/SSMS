<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="Seva_list.aspx.cs" Inherits="SSMS.Pages.Admin.Seva_list" %>

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

    <!-- Full page centered container -->
    <div class="d-flex justify-content-center align-items-start" style="min-height: 100vh; padding-top: 50px;">
        <div class="col-md-10 col-lg-8"> <!-- Max width control, centers content -->

            <h2 class="text-center mb-4">Seva Master List</h2>

            <asp:GridView ID="gvSevaList" runat="server"
                AutoGenerateColumns="false" AllowPaging="true" PageSize="10"
                OnPageIndexChanging="gvSevaList_PageIndexChanging"
                OnRowCommand="gvSevaList_RowCommand"
                CssClass="table table-bordered table-striped table-hover text-center w-100">

                <Columns>
                    <asp:BoundField DataField="SevaID" HeaderText="Seva ID" />
                    <asp:BoundField DataField="SevaName" HeaderText="Seva Name" />
                    <asp:BoundField DataField="SevaDescription" HeaderText="Description" />
                    <asp:BoundField DataField="SevaType" HeaderText="Type" />
                    <asp:BoundField DataField="SevaCategory" HeaderText="Category" />
                    <asp:BoundField DataField="SevaAmount" HeaderText="Amount" DataFormatString="{0:C}" />
                    <asp:BoundField DataField="AvailabilityDays" HeaderText="Availability Days" />
                    <asp:BoundField DataField="PrasadaDetails" HeaderText="Prasada" />
                    <asp:CheckBoxField DataField="IsActive" HeaderText="Active" />
                    <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:dd-MM-yyyy}" />
                    <asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
                    <asp:BoundField DataField="UpdatedDate" HeaderText="Updated Date" DataFormatString="{0:dd-MM-yyyy}" />
                    <asp:BoundField DataField="UpdatedBy" HeaderText="Updated By" />

                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEdit" runat="server" 
                                CommandName="EditSeva" CommandArgument='<%# Eval("SevaID") %>'
                                CssClass="btn btn-sm btn-primary me-2">Edit</asp:LinkButton>

                            <asp:LinkButton ID="btnDelete" runat="server" 
                                CommandName="DeleteSeva" CommandArgument='<%# Eval("SevaID") %>'
                                OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                CssClass="btn btn-sm btn-danger">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold d-block mt-3"></asp:Label>

        </div>
    </div>
</asp:Content>
