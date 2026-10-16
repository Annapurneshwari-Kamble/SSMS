<%@ Page Title="DASHBOARD" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SSMS.Pages.Admin.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Admin Dashboard</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid px-4 py-4 bg-light min-vh-100">
        
        <!-- Page Header -->
        <div class="d-flex align-items-center justify-content-between mb-4">
            <div>
                <h2 class="fw-bold text-dark mb-1">Admin Dashboard</h2>
                <p class="text-muted mb-0 small">Overview of activities and devotees</p>
            </div>
        </div>

        <div class="row g-4">
            
            <!-- Left Column: Summary Cards + Extra Content -->
            <div class="col-lg-8">
                
                <!-- Welcome Banner (Fills space creatively) -->
                <div class="card border-0 shadow-sm rounded-4 mb-4 bg-primary text-white overflow-hidden position-relative">
                    <div class="card-body p-4 position-relative z-1">
                        <h4 class="fw-bold mb-2">Welcome back!</h4>
                        <p class="mb-0 opacity-75">Here is what's happening with your Seva activities today. You have new entries in the right panel.</p>
                    </div>
                    <!-- Decorative background element (pure Bootstrap) -->
                    <div class="position-absolute top-0 end-0 translate-middle-y me-4 mt-5 opacity-25 d-none d-md-block">
                        <i class="fas fa-om fa-6x"></i>
                    </div>
                </div>

                <!-- Summary Cards -->
                <div class="row g-3 mb-4">
                    
                    <!-- Total Nirantara Abhishek Card -->
                    <div class="col-xl-6 col-md-6">
                        <div class="card border-0 shadow-sm rounded-4 h-100 border-start border-4 border-primary">
                            <div class="card-body p-4">
                                <div class="d-flex justify-content-between align-items-center">
                                    <div>
                                        <div class="text-primary text-uppercase fw-bold small mb-2" style="letter-spacing: 0.5px;">
                                            Total Nirantara Abhishek
                                        </div>
                                        <asp:Label ID="lblTotalAbhishek" runat="server" CssClass="h2 fw-bold text-dark mb-0 d-block"></asp:Label>
                                    </div>
                                    <div class="bg-primary bg-opacity-10 rounded-circle d-flex align-items-center justify-content-center" style="width: 60px; height: 60px;">
                                        <i class="fas fa-calendar-alt fa-2x text-primary"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Total Nirantara Annaprasad Card -->
                    <div class="col-xl-6 col-md-6">
                        <div class="card border-0 shadow-sm rounded-4 h-100 border-start border-4 border-success">
                            <div class="card-body p-4">
                                <div class="d-flex justify-content-between align-items-center">
                                    <div>
                                        <div class="text-success text-uppercase fw-bold small mb-2" style="letter-spacing: 0.5px;">
                                            Total Nirantara Annaprasad
                                        </div>
                                        <asp:Label ID="lblTotalAnnaprasad" runat="server" CssClass="h2 fw-bold text-dark mb-0 d-block"></asp:Label>
                                    </div>
                                    <div class="bg-success bg-opacity-10 rounded-circle d-flex align-items-center justify-content-center" style="width: 60px; height: 60px;">
                                        <i class="fas fa-bowl-food fa-2x text-success"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Total Other Sevas Card -->
                    <div class="col-xl-6 col-md-6">
                        <div class="card border-0 shadow-sm rounded-4 h-100 border-start border-4 border-info">
                            <div class="card-body p-4">
                                <div class="d-flex justify-content-between align-items-center">
                                    <div>
                                        <div class="text-info text-uppercase fw-bold small mb-2" style="letter-spacing: 0.5px;">
                                            Total Other Sevas
                                        </div>
                                        <asp:Label ID="lblTotalOtherSevas" runat="server" CssClass="h2 fw-bold text-dark mb-0 d-block"></asp:Label>
                                    </div>
                                    <div class="bg-info bg-opacity-10 rounded-circle d-flex align-items-center justify-content-center" style="width: 60px; height: 60px;">
                                        <i class="fas fa-praying-hands fa-2x text-info"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Total Seva Card -->
                    <div class="col-xl-6 col-md-6">
                        <div class="card border-0 shadow-sm rounded-4 h-100 border-start border-4 border-warning">
                            <div class="card-body p-4">
                                <div class="d-flex justify-content-between align-items-center">
                                    <div>
                                        <div class="text-warning text-uppercase fw-bold small mb-2" style="letter-spacing: 0.5px;">
                                            Total Seva
                                        </div>
                                        <asp:Label ID="lblTotalSeva" runat="server" CssClass="h2 fw-bold text-dark mb-0 d-block"></asp:Label>
                                    </div>
                                    <div class="bg-warning bg-opacity-10 rounded-circle d-flex align-items-center justify-content-center" style="width: 60px; height: 60px;">
                                        <i class="fas fa-hands-helping fa-2x text-warning"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <!-- Quick Actions Section (Fills the empty bottom space) -->
                

            </div>

            <!-- Right Column: Today's Notifications (Unchanged as requested) -->
            <div class="col-lg-4">
                <div class="sticky-top" style="top: 20px; z-index: 1;">
                    <div class="card shadow-lg border-0 rounded-4 h-100">
                        <div class="card-header bg-success text-white text-center py-3 rounded-top-4 border-0">
                            <h5 class="mb-0 fw-bold"><i class="bi bi-bell-fill me-2"></i>Today's Nirantara Sevas</h5>
                        </div>
                        <div class="card-body" style="max-height: 70vh; overflow-y: auto;">

                            <!-- Nirantara Abhishek -->
                            <h6 class="text-primary fw-bold"><i class="fas fa-tint me-1"></i>Nirantara Abhishek</h6>
                            <asp:Repeater ID="rptAbhishek" runat="server">
                                <ItemTemplate>
                                    <div class="alert alert-info p-2 mb-2 rounded">
                                        <strong><%# Eval("DevoteeName") %></strong><br />
                                        <small><%# Eval("EntryDate","{0:dd-MMM-yyyy HH:mm}") %></small>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <hr />

                            <!-- Nirantara Annaprasad -->
                            <h6 class="text-danger fw-bold"><i class="fas fa-bowl-rice me-1"></i>Nirantara Annaprasad</h6>
                            <asp:Repeater ID="rptAnnaprasad" runat="server">
                                <ItemTemplate>
                                    <div class="alert alert-warning p-2 mb-2 rounded">
                                        <strong><%# Eval("DevoteeName") %></strong><br />
                                        <small><%# Eval("EntryDate","{0:dd-MMM-yyyy HH:mm}") %></small>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <hr />

                            <!-- Other Sevas -->
                            <h6 class="text-danger fw-bold"><i class="fas fa-tree me-1"></i>Other Sevas</h6>
                            <asp:Repeater ID="rptOtherSevas" runat="server">
                                <ItemTemplate>
                                    <div class="alert alert-warning p-2 mb-2 rounded">
                                        <strong><%# Eval("DevoteeName") %></strong><br />
                                        <small><%# Eval("EntryDate","{0:dd-MMM-yyyy HH:mm}") %></small>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

</asp:Content>