<%@ Page Title="User Master List" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master"
    AutoEventWireup="true" CodeBehind="UsermasterList.aspx.cs"
    Inherits="SSMS.Pages.Admin.UsermasterList" %>

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
                    <h4 class="mb-0">User Master List</h4>
                </div>
                <div class="card-body">
                    
                   
                    <asp:GridView ID="gvUsers"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-bordered table-striped text-center align-middle"
                        DataKeyNames="id"
                        OnRowCommand="gvUsers_RowCommand">
                        
                        <HeaderStyle CssClass="table-primary text-center" />
                        
                        <Columns>

                            <asp:BoundField DataField="UserName" HeaderText="User Name" />
                            <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                            <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                            <asp:BoundField DataField="MobileNo" HeaderText="Mobile" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:BoundField DataField="user_type" HeaderText="User Type" />

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:HyperLink ID="lnkEdit"
                                        runat="server"
                                        CssClass="btn btn-sm btn-warning me-2"
                                        NavigateUrl='<%# "Usermaster.aspx?id=" + Eval("id") %>'
                                        ToolTip="Edit">
                                        <i class="bi bi-pencil"></i>
                                    </asp:HyperLink>
                                    
                                    <asp:LinkButton ID="lnkDelete"
                                        runat="server"
                                        CommandName="DeleteUser"
                                        CommandArgument='<%# Eval("id") %>'
                                        CssClass="btn btn-sm btn-danger"
                                        ToolTip="Delete"
                                        OnClientClick="return confirm('Are you sure you want to delete this user?');">
                                        <i class="bi bi-trash"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold text-danger"></asp:Label>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function toggleAll(source) {
            var grid = document.getElementById('<%= gvUsers.ClientID %>');
            var checkboxes = grid.querySelectorAll('input[type="checkbox"]');

            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i] != source) {
                    checkboxes[i].checked = source.checked;
                }
            }
        }
    </script>
</asp:Content>