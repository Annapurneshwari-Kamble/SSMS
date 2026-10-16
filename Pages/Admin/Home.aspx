<%@ Page Title="HOME" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="SSMS.Pages.Admin.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="d-flex justify-content-center align-items-center min-vh-80 bg-light py-5">
        <div class="text-center p-4 p-md-5">
            <!-- Decorative elements -->
            <div class="position-relative mb-4">
                <div class="position-absolute top-0 start-50 translate-middle">
                    <div class="bg-warning rounded-circle p-1 opacity-25"></div>
                </div>
                <img src="../../assets/images/smsf5.jpg" alt="Shri Siddharood Math Seva" class="img-fluid rounded-circle shadow-lg border border-5 border-white mb-3" style="max-width: 280px;">
                <div class="position-absolute bottom-0 start-50 translate-middle">
                    <div class="bg-warning rounded-circle p-1 opacity-25"></div>
                </div>
            </div>
            
            <h1 class="display-4 fw-bold text-dark mb-3">Shri Siddharood Math Seva</h1>
            
            <p class="lead text-muted mb-4">Service • Devotion • Community</p>
            
            <!-- Decorative divider -->
            <div class="d-flex align-items-center justify-content-center mb-4">
                <div class="bg-primary opacity-75 rounded-pill" style="height: 4px; width: 40px;"></div>
                <div class="bg-warning rounded-circle mx-2" style="height: 10px; width: 10px;"></div>
                <div class="bg-primary opacity-75 rounded-pill" style="height: 4px; width: 40px;"></div>
            </div>
            
            <!-- Welcome message for admin -->
            <div class="row justify-content-center mt-5">
                <div class="col-lg-8">
                    <div class="p-4 bg-white rounded-3 shadow-sm">
                        <h3 class="fw-semibold text-primary mb-3">Administrative Portal</h3>
                        <p class="text-muted">Welcome to the Shri Siddharood Math Seva management system. Use the navigation menu to access various administrative functions.</p>
                        
                        <!-- Add Seva Member Details Button -->
                        <asp:Button ID="btnAddSevaMember" runat="server" 
                            Text="Add Seva Member Details" 
                            CssClass="btn btn-primary mt-3"
                            OnClick="btnAddSevaMember_Click" />
                    </div>
                </div>
            </div>
            
            <!-- Footer note -->
            <div class="mt-5 pt-4 border-top">
                <p class="text-muted small">Seva with devotion and compassion</p>
            </div>
        </div>
    </div>

    <!-- Font Awesome Icons -->
    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
</asp:Content>
