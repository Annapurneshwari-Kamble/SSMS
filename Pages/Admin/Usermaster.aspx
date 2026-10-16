<%@ Page Title="User Master" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="Usermaster.aspx.cs" Inherits="SSMS.Pages.Admin.Usermaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-4">
        <div class="card shadow-sm">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0">User Master Form</h5>
            </div>

            <div class="card-body">
                <div class="row g-3">
                   
                    <!-- User Name -->
                    <div class="col-md-6">
                        <label class="form-label">User Name</label>
                        <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvUserName" runat="server"
                            ControlToValidate="txtUserName"
                            ErrorMessage="User Name is required"
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>

                    <!-- Password -->
                    <div class="col-md-6">
                        <label class="form-label">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                            ControlToValidate="txtPassword"
                            ErrorMessage="Password is required"
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>

                    <!-- First Name -->
                    <div class="col-md-6">
                        <label class="form-label">First Name</label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                            ControlToValidate="txtFirstName"
                            ErrorMessage="First Name is required"
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>

                    <!-- Last Name -->
                    <div class="col-md-6">
                        <label class="form-label">Last Name</label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvLastName" runat="server"
                            ControlToValidate="txtLastName"
                            ErrorMessage="Last Name is required"
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>

                    <!-- Mobile -->
                    <div class="col-md-6">
                        <label class="form-label">Mobile No</label>
                        <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvMobile" runat="server"
                            ControlToValidate="txtMobile"
                            ErrorMessage="Mobile number is required"
                            CssClass="text-danger"
                            Display="Dynamic" />

                        <asp:RegularExpressionValidator ID="revMobile" runat="server"
                            ControlToValidate="txtMobile"
                            ValidationExpression="^[0-9]{10}$"
                            ErrorMessage="Enter valid 10-digit mobile number"
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>

                    <!-- Email -->
                    <div class="col-md-6">
                        <label class="form-label">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />

                    </div>

                    <!-- Institute Code -->
                    <div class="col-md-6">
                        <label class="form-label">Institute Code</label>
                        <asp:TextBox ID="txtInstituteCode" runat="server" CssClass="form-control" />
                    </div>

                    <!-- User Type -->
                    <div class="col-md-6">
                        <label class="form-label">User Type</label>
                        <asp:DropDownList ID="ddlUserType" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Select" Value="" />
                            <asp:ListItem Text="Admin" Value="Admin" />
                            <asp:ListItem Text="User" Value="User" />
                        </asp:DropDownList>

                        <asp:RequiredFieldValidator ID="rfvUserType" runat="server"
                            ControlToValidate="ddlUserType"
                            InitialValue=""
                            ErrorMessage="User Type is required"
                            CssClass="text-danger"
                            Display="Dynamic" />
                    </div>

                    <!-- User ID -->
                    <div class="col-md-6">
                        <label class="form-label">User ID</label>
                        <asp:TextBox ID="txtUserId" runat="server" CssClass="form-control" />
                    </div>

                    <!-- Is Active -->
                    <div class="col-md-6 d-flex align-items-end">
                        <div class="form-check">
                            <asp:CheckBox ID="chkIsActive" runat="server" CssClass="form-check-input" />
                            <label class="form-check-label">Is Active</label>
                        </div>
                    </div>

                    <!-- Profile Image -->
                    <div class="col-md-12">
                        <label class="form-label">Profile Image</label>
                        <asp:FileUpload ID="fuProfile" runat="server" CssClass="form-control" />
                    </div>

                    <!-- Buttons -->
                    <div class="col-12 mt-3">
                        <asp:Button ID="btnSave" runat="server"
                            Text="Save"
                            CssClass="btn btn-success me-2"
                            OnClick="btnSave_Click"
                            CausesValidation="true" />

                        <asp:Button ID="btnClear" runat="server"
                            Text="Clear"
                            CssClass="btn btn-secondary"
                            OnClick="btnClear_Click"
                            CausesValidation="false" />
                    </div>

                    <!-- Message -->
                    <div class="col-12 mt-3">
                        <asp:Label ID="lblMessage" runat="server" />
                    </div>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
