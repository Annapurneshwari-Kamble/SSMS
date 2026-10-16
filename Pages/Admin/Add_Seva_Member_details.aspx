<%@ Page Title="SEVA MEMBER DETAILS" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="Add_Seva_Member_details.aspx.cs" Inherits="SSMS.Pages.Admin.Add_Seva_Member_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap Icons -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.0/font/bootstrap-icons.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid pt-4">
        <div class="row g-4">
            <!-- Left Side: Form -->
            <div class="col-xl-8 col-lg-7">
                <div class="card shadow-lg border-0 rounded-4 overflow-hidden animate__animated animate__fadeInLeft">
                    <div class="card-header bg-gradient bg-primary text-white text-center py-4 position-relative">
                        <div class="position-absolute top-0 start-0 w-100 h-100 bg-gradient bg-opacity-10"></div>
                        <h3 class="mb-0 fw-bold">
                            <i class="bi bi-person-plus-fill me-2"></i>Seva Member Registration
                        </h3>
                        <p class="mb-0 mt-2 opacity-75">Complete all required details for seva registration</p>
                    </div>
                    <div class="card-body p-4 p-md-5 bg-light bg-gradient">

                        <!-- Hidden field -->
                        <asp:HiddenField ID="hdnSevaId" runat="server"/>

                        <!-- Language Selection -->
                        <div class="mb-4">
                            <label class="form-label fw-semibold fs-6 text-primary mb-3">
                                <i class="bi bi-translate me-2"></i>Select Preferred Language
                            </label>
                            <div class="d-flex flex-wrap gap-3">
                                <asp:RadioButtonList ID="rblLanguage" runat="server" RepeatDirection="Horizontal" CssClass="d-flex flex-wrap gap-3" AutoPostBack="false">
                                    <asp:ListItem Text="English" Value="English" Selected="True" CssClass="btn btn-outline-primary rounded-pill px-4 py-2" />
                                    <asp:ListItem Text="Kannada" Value="Kannada" CssClass="btn btn-outline-primary rounded-pill px-4 py-2" />
                                    <asp:ListItem Text="Hindi" Value="Hindi" CssClass="btn btn-outline-primary rounded-pill px-4 py-2" />
                                </asp:RadioButtonList>
                            </div>
                        </div>

                        <div class="bg-primary bg-opacity-10 p-3 rounded-3 mb-4">
                            <div class="row g-3">
                                <!-- Seva Type -->
                                <div class="col-md-6">
                                    <label for="ddlMainSeva" class="form-label fw-semibold">
                                        <i class="bi bi-list-ul me-2"></i>Select Seva Type <span class="text-danger">*</span>
                                    </label>
                                    <asp:DropDownList ID="ddlMainSeva"
                                        runat="server"
                                        CssClass="form-select form-select-lg border-primary border-2 shadow-sm"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlMainSeva_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <div class="mt-1">
                                        <asp:RequiredFieldValidator ID="rfvMainSeva" runat="server"
                                            ControlToValidate="ddlMainSeva" InitialValue=""
                                            ErrorMessage="Please select Seva Type." CssClass="text-danger small fw-medium" Display="Dynamic" />
                                    </div>
                                </div>
                                <!-- Seva For -->
                                <div class="col-md-6">
                                    <label for="ddlSevaFor" class="form-label fw-semibold">
                                        <i class="bi bi-person-heart me-2"></i>Seva For <span class="text-danger">*</span>
                                    </label>

                                    <asp:DropDownList ID="ddlSevaFor" runat="server"
                                        CssClass="form-select form-select-lg border-primary border-2 shadow-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlSevaFor_SelectedIndexChanged">

                                        <asp:ListItem Text="-- Select Seva For --" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Siddharood Swami" Value="Siddharood Swami"></asp:ListItem>
                                        <asp:ListItem Text="Gurunathrood Swami" Value="Gurunathrood Swami"></asp:ListItem>
                                        <asp:ListItem Text="Dasoha" Value="Dasoha"></asp:ListItem>

                                    </asp:DropDownList>

                                    <div class="mt-1">
                                        <asp:RequiredFieldValidator ID="rfvSevaFor" runat="server"
                                            ControlToValidate="ddlSevaFor"
                                            InitialValue=""
                                            ErrorMessage="Please select Seva For."
                                            CssClass="text-danger small fw-medium"
                                            Display="Dynamic" />
                                    </div>
                                </div>
                                <!-- Event -->
                                <div class="col-md-6">
                                    <label class="form-label fw-semibold">
                                        <i class="bi bi-calendar-event me-2"></i>Event
                                    </label>

                                    <asp:TextBox ID="txtEvent" runat="server"
                                        CssClass="form-control border-primary border-2 shadow-sm"
                                        placeholder="Enter Event Name (e.g., Mahashivratri, Special Pooja)">
                                    </asp:TextBox>
                                </div>

                                <!-- Serial + Date -->
                                <div class="col-md-3">
                                    <label class="form-label fw-semibold"><i class="bi bi-123 me-2"></i>Serial Number</label>
                                    <asp:TextBox ID="txtSerialNo" runat="server" CssClass="form-control border-primary border-2 shadow-sm bg-light fw-bold text-primary" ReadOnly="true" />
                                </div>
                                <div class="col-md-3">
                                    <label class="form-label fw-semibold"><i class="bi bi-calendar-date me-2"></i>Seva Date <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="txtDate" runat="server" CssClass="form-control border-primary border-2 shadow-sm" TextMode="Date" />
                                    <div class="mt-1">
                                        <asp:RequiredFieldValidator ID="rfvDate" runat="server"
                                            ControlToValidate="txtDate" ErrorMessage="Please select a date."
                                            CssClass="text-danger small fw-medium" Display="Dynamic" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Personal Information Section -->
                        <div class="bg-white rounded-3 p-4 shadow-sm mb-4">
                            <h5 class="text-primary mb-4 border-bottom pb-2">
                                <i class="bi bi-person-vcard me-2"></i>Personal Information
                            </h5>
                            <!-- Register Number -->
                            <div class="mb-4">
                                <label class="form-label fw-semibold">
                                    <i class="bi bi-card-heading me-2"></i>Register Number
                                    <span class="text-danger">*</span>
                                </label>

                                <asp:TextBox ID="txtRegisterNo"
                                    runat="server"
                                    ReadOnly="true"
                                    CssClass="form-control border-2 shadow-sm"
                                    placeholder="Enter Register Number" />

                                <div class="mt-1">
                                    <asp:RequiredFieldValidator
                                        ID="rfvRegisterNo"
                                        runat="server"
                                        ControlToValidate="txtRegisterNo"
                                        ErrorMessage="Register Number is required."
                                        CssClass="text-danger small fw-medium"
                                        Display="Dynamic" />
                                </div>
                            </div>

                            <!-- Devotee Name -->
                            <div class="mb-4">
                                <label class="form-label fw-semibold"><i class="bi bi-person-badge me-2"></i>Devotee Name <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control border-2 shadow-sm" placeholder="Enter full name" />
                                <div class="mt-1">
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server"
                                        ControlToValidate="txtName" ErrorMessage="Devotee Name is required."
                                        CssClass="text-danger small fw-medium" Display="Dynamic" />
                                    <%--<asp:RegularExpressionValidator ID="revName" runat="server"
                                        ControlToValidate="txtName"  ValidationExpression="^[\p{L}\s]+$"
                                        ErrorMessage="Only alphabets allowed." CssClass="text-danger small fw-medium" Display="Dynamic" />--%>
                                </div>
                            </div>

                            <!-- Contact & Email -->
                            <div class="row g-3 mb-4">
                                <div class="col-md-6">
                                    <label class="form-label fw-semibold"><i class="bi bi-telephone me-2"></i>Contact Number <span class="text-danger">*</span> </label>
                                    <asp:TextBox ID="txtContact" runat="server" CssClass="form-control border-2 shadow-sm" TextMode="Number" placeholder="10-digit mobile number" />
                                    <div class="mt-1">
                                        <%--<asp:RequiredFieldValidator ID="rfvContact" runat="server"
                                            ControlToValidate="txtContact" ErrorMessage="Contact number is required."
                                            CssClass="text-danger small fw-medium" Display="Dynamic" />
                                        <asp:RegularExpressionValidator ID="revContact" runat="server"
                                            ControlToValidate="txtContact" ValidationExpression="^[0-9]{10}$"
                                            ErrorMessage="Enter a valid 10-digit number." CssClass="text-danger small fw-medium" Display="Dynamic" />--%>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <label for="txtEmail" class="form-label fw-semibold"><i class="bi bi-envelope me-2"></i>Email </label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control border-2 shadow-sm" TextMode="Email" placeholder="example@domain.com" />
                                    <div class="mt-1">
                                        <%--<asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                            ControlToValidate="txtEmail" ErrorMessage="Email is required."
                                            CssClass="text-danger small fw-medium" Display="Dynamic" />--%>
                                        <asp:RegularExpressionValidator ID="revEmail" runat="server"
                                            ControlToValidate="txtEmail"
                                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                            ErrorMessage="Enter a valid email format."
                                            CssClass="text-danger small fw-medium" Display="Dynamic" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Address Information Section -->
                        <div class="bg-white rounded-3 p-4 shadow-sm mb-4">
                            <h5 class="text-primary mb-4 border-bottom pb-2">
                                <i class="bi bi-geo-alt me-2"></i>Address Information
                            </h5>

                            <!-- State/District/City -->
                            <div class="row g-3 mb-4">
                                <div class="col-md-4">
                                    <label class="form-label fw-semibold">State <span class="text-danger">*</span></label>
                                    <asp:DropDownList ID="ddlState" runat="server" CssClass="form-select border-2 shadow-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlState_SelectedIndexChanged"></asp:DropDownList>
                                    <div class="mt-1">
                                        <%--<asp:RequiredFieldValidator ID="rfvState" runat="server"
                                            ControlToValidate="ddlState" InitialValue=""
                                            ErrorMessage="Please select a state." CssClass="text-danger small fw-medium" Display="Dynamic" />--%>
                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <label class="form-label fw-semibold">District <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="txtDistrict" runat="server" CssClass="form-control border-2 shadow-sm" />
                                    <div class="mt-1">
                                        <%--<asp:RequiredFieldValidator ID="rfvDistrict" runat="server"
                                            ControlToValidate="txtDistrict" ErrorMessage="District is required."
                                            CssClass="text-danger small fw-medium" Display="Dynamic" />--%>
                                        <%--<asp:RegularExpressionValidator ID="revDistrict" runat="server"
                                            ControlToValidate="txtDistrict"  ValidationExpression="^[\p{L}\s]+$"
                                            ErrorMessage="Only alphabets allowed." CssClass="text-danger small fw-medium" Display="Dynamic" />--%>
                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <label class="form-label fw-semibold">City <span class="text-danger">*</span></label>
                                    <asp:TextBox ID="txtCity" runat="server" CssClass="form-control border-2 shadow-sm" />
                                    <div class="mt-1">
                                        <asp:RequiredFieldValidator ID="rfvCity" runat="server"
                                            ControlToValidate="txtCity" ErrorMessage="City is required."
                                            CssClass="text-danger small fw-medium" Display="Dynamic" />
                                        <%--    <asp:RegularExpressionValidator ID="revCity" runat="server"
                                            ControlToValidate="txtCity"  ValidationExpression="^[\p{L}\s]+$"
                                            ErrorMessage="Only alphabets allowed." CssClass="text-danger small fw-medium" Display="Dynamic" />--%>
                                    </div>
                                </div>
                            </div>

                            <!-- Pincode -->
                            <div class="mb-4">
                                <label class="form-label fw-semibold"><i class="bi bi-mailbox me-2"></i>Pincode <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtPincode" runat="server" CssClass="form-control border-2 shadow-sm" AutoPostBack="true" OnTextChanged="txtPincode_TextChanged" />
                                <div class="mt-1">
                                    <%--<asp:RequiredFieldValidator ID="rfvPincode" runat="server"
                                        ControlToValidate="txtPincode" ErrorMessage="Pincode is required."
                                        CssClass="text-danger small fw-medium" Display="Dynamic" />
                                    <asp:RegularExpressionValidator ID="revPincode" runat="server"
                                        ControlToValidate="txtPincode" ValidationExpression="^[0-9]{6}$"
                                        ErrorMessage="Enter a valid 6-digit pincode." CssClass="text-danger small fw-medium" Display="Dynamic" />--%>
                                </div>
                            </div>

                            <!-- Address -->
                            <div class="mb-4">
                                <label for="txtAddress" class="form-label fw-semibold"><i class="bi bi-house-door me-2"></i>Address <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control border-2 shadow-sm" TextMode="MultiLine" Rows="3" placeholder="Enter complete address" />
                                <div class="mt-1">
                                    <%--<asp:RequiredFieldValidator ID="rfvAddress" runat="server"
                                        ControlToValidate="txtAddress" ErrorMessage="Address is required."
                                        CssClass="text-danger small fw-medium" Display="Dynamic" />--%>
                                </div>
                            </div>
                        </div>

                        <!-- Payment Information Section -->
                        <div class="bg-white rounded-3 p-4 shadow-sm mb-4">
                            <h5 class="text-primary mb-4 border-bottom pb-2">
                                <i class="bi bi-credit-card me-2"></i>Payment Details
                            </h5>

                            <div class="row g-3">
                                <!-- Price -->

                                <!-- Payment Mode -->
                                <div class="row g-3 align-items-start">

                                    <!-- LEFT SIDE -->
                                    <div class="col-md-8">

                                        <div class="row g-3">


                                            <!-- Seva Start Selection -->
                                            <div class="col-md-12">
                                                <label class="form-label fw-semibold">
                                                    <i class="bi bi-calendar-check me-2"></i>
                                                    Seva Start From <span class="text-danger">*</span>
                                                </label>

                                                <asp:RadioButtonList ID="rblSevaStart" runat="server"
                                                    RepeatDirection="Horizontal"
                                                    CssClass="d-flex flex-wrap gap-4"
                                                    onclick="toggleAdditionalAmount()">

                                                    <asp:ListItem Text="Seva Start From This Year"
                                                        Value="ThisYear" />
                                       
                                                    <asp:ListItem Text="Seva Start From Next Year"
                                                        Value="NextYear" />
                                                </asp:RadioButtonList>

                                                <asp:RequiredFieldValidator ID="rfvSevaStart"
                                                    runat="server"
                                                    ControlToValidate="rblSevaStart"
                                                    InitialValue=""
                                                    ErrorMessage="Please select Seva Start From."
                                                    CssClass="text-danger small fw-medium"
                                                    Display="Dynamic" />
                                            </div>

                                            <!-- Additional Amount -->
                                            <div class="col-md-6" id="additionalAmountSection"
                                                style="display: none;">

                                                <label class="form-label fw-semibold">
                                                    <i class="bi bi-currency-rupee me-2"></i>
                                                    Additional Amount
                                                </label>

                                                <asp:TextBox ID="txtAdditionalAmount" runat="server"
                                                    CssClass="form-control border-2 shadow-sm"
                                                    Text="0"
                                                    placeholder="Enter additional amount"
                                                    oninput="calculateTotal()" />

                                                <div class="mt-1">
                                                    <asp:RegularExpressionValidator
                                                        ID="revAdditionalAmount"
                                                        runat="server"
                                                        ControlToValidate="txtAdditionalAmount"
                                                        ValidationExpression="^\d+(\.\d{1,2})?$"
                                                        ErrorMessage="Enter a valid additional amount."
                                                        CssClass="text-danger small fw-medium"
                                                        Display="Dynamic" />
                                                </div>
                                            </div>

                                            <!-- Amount -->
                                            <div class="col-md-6">
                                                <label for="txtPrice" class="form-label fw-semibold">
                                                    <i class="bi bi-currency-rupee me-2"></i>
                                                    Amount <span class="text-danger">*</span>
                                                </label>

                                                <asp:TextBox ID="txtPrice" runat="server"
                                                    CssClass="form-control border-2 shadow-sm"
                                                    placeholder="Enter amount"
                                                    oninput="calculateTotal()" />

                                                <div class="mt-1">
                                                    <asp:RequiredFieldValidator ID="rfvPrice"
                                                        runat="server"
                                                        ControlToValidate="txtPrice"
                                                        ErrorMessage="Price is required."
                                                        CssClass="text-danger small fw-medium"
                                                        Display="Dynamic" />

                                                    <asp:RegularExpressionValidator ID="revPrice"
                                                        runat="server"
                                                        ControlToValidate="txtPrice"
                                                        ValidationExpression="^\d+(\.\d{1,2})?$"
                                                        ErrorMessage="Enter a valid amount (e.g., 100 or 100.50)."
                                                        CssClass="text-danger small fw-medium"
                                                        Display="Dynamic" />

                                                    <asp:CustomValidator ID="cvMinimumAmount"
                                                        runat="server"
                                                        ControlToValidate="txtPrice"
                                                        ErrorMessage="Invalid amount."
                                                        CssClass="text-danger small fw-medium"
                                                        Display="Dynamic"
                                                        ValidateEmptyText="true"
                                                        ClientValidationFunction="validateMinimumAmount" />
                                                </div>
                                            </div>

                                            <!-- Total Amount -->
                                            <div class="col-md-6">
                                                <label class="form-label fw-semibold">
                                                    <i class="bi bi-calculator me-2"></i>
                                                    Total Amount (₹)
                                                </label>

                                                <asp:TextBox ID="txtTotal" runat="server"
                                                    CssClass="form-control border-2 shadow-sm fw-bold text-success"
                                                    ReadOnly="true"
                                                    Text="0.00" />
                                            </div>

                                            <!-- Payment Mode -->
                                            <div class="col-md-6">
                                                <label class="form-label fw-semibold">
                                                    <i class="bi bi-credit-card me-2"></i>Payment Mode
                                                </label>
                                                <asp:DropDownList ID="ddlPaymentMode" runat="server"
                                                    CssClass="form-select border-2 shadow-sm">
                                                    <asp:ListItem Text="Cash" Value="Cash"></asp:ListItem>
                                                    <asp:ListItem Text="UPI" Value="UPI"></asp:ListItem>
                                                    <asp:ListItem Text="PayU" Value="PayU"></asp:ListItem>
                                                    <asp:ListItem Text="Bank" Value="Bank"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <!-- UTR -->
                                            <div class="col-md-6">
                                                <label class="form-label fw-semibold">
                                                    <i class="bi bi-hash me-2"></i>UTR Number
                                                </label>
                                                <asp:TextBox ID="txtUTRNumber" runat="server"
                                                    CssClass="form-control border-2 shadow-sm"
                                                    placeholder="Enter UTR / Transaction ID" />
                                            </div>

                                            <!-- Bank -->
                                            <div class="col-md-6">
                                                <label class="form-label fw-semibold">
                                                    <i class="bi bi-bank me-2"></i>Bank Name
                                                </label>
                                                <asp:TextBox ID="txtBankName" runat="server"
                                                    CssClass="form-control border-2 shadow-sm"
                                                    placeholder="Enter Bank Name" />
                                            </div>
                                            <asp:CustomValidator ID="cvPaymentMode" runat="server"
                                                ControlToValidate="ddlPaymentMode"
                                                ErrorMessage="UTR and Bank Name are not valid for Cash payment. Select UPI/Bank."
                                                CssClass="text-danger small fw-medium"
                                                Display="Dynamic"
                                                ClientValidationFunction="validatePaymentMode">
                                            </asp:CustomValidator>

                                            <!-- Bank Details Container -->
                                            <div id="bankDetailsSection" style="display: none;" class="row g-3">

                                                <!-- Branch -->
                                                <div class="col-md-6">
                                                    <label class="form-label fw-semibold">
                                                        <i class="bi bi-geo-alt me-2"></i>Branch
                                                    </label>
                                                    <asp:TextBox ID="txtBranch" runat="server"
                                                        CssClass="form-control border-2 shadow-sm" />
                                                </div>

                                                <!-- IFSC Code -->
                                                <div class="col-md-6">
                                                    <label class="form-label fw-semibold">
                                                        <i class="bi bi-credit-card-2-front me-2"></i>IFSC Code
                                                    </label>
                                                    <asp:TextBox ID="txtIFSC" runat="server"
                                                        CssClass="form-control border-2 shadow-sm" />
                                                </div>

                                                <!-- Account Number -->
                                                <div class="col-md-6">
                                                    <label class="form-label fw-semibold">
                                                        <i class="bi bi-bank2 me-2"></i>Account Number
                                                    </label>
                                                    <asp:TextBox ID="txtAccountNo" runat="server"
                                                        CssClass="form-control border-2 shadow-sm" />
                                                </div>

                                            </div>
                                            <!-- Remarks -->
                                            <div class="col-md-12">
                                                <label class="form-label fw-semibold">
                                                    <i class="bi bi-chat-left-text me-2"></i>Remarks
                                                </label>
                                                <asp:TextBox ID="txtRemarks" runat="server"
                                                    CssClass="form-control border-2 shadow-sm"
                                                    TextMode="MultiLine"
                                                    Rows="3"
                                                    placeholder="Enter any remarks or additional information" />
                                            </div>
                                        </div>
                                    </div>


                                    <!-- RIGHT SIDE QR CODE -->
                                    <div class="col-md-4 text-center">

                                        <div class="border rounded-3 p-3 shadow-sm bg-light">

                                            <label class="fw-semibold text-primary mb-2 d-block">
                                                <i class="bi bi-qr-code-scan me-1"></i>Scan & Pay
                                            </label>

                                            <asp:Image ID="imgQRCode"
                                                runat="server"
                                                ImageUrl="~/assets/images/SSMSscanner2.png"
                                                CssClass="img-fluid rounded"
                                                Style="max-height: 180px;" />

                                            <div class="small text-muted mt-2">
                                                Scan QR for UPI Payment
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Action Buttons -->
                        <div class="d-flex flex-column flex-md-row justify-content-between align-items-center gap-3 mt-4 pt-4 border-top">
                            <asp:Button ID="btnGenerateReport" runat="server"
                                CssClass="btn btn-success btn-lg fw-semibold px-5 py-3 rounded-3 shadow-lg animate__animated animate__pulse animate__infinite"
                                Text="Save & Generate Report" OnClick="btnGenerateReport_Click" />

                            <asp:Button ID="btnClearForm" runat="server"
                                CssClass="btn btn-outline-danger btn-lg fw-semibold px-5 py-3 rounded-3"
                                Text="Clear Form" OnClick="btnClearForm_Click" CausesValidation="false" />
                        </div>

                        <!-- Message Display -->
                        <div class="mt-4 text-center">
                            <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold fs-5 alert alert-dismissible fade show d-none" role="alert"></asp:Label>
                        </div>
                    </div>
                </div>
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

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <!-- Enhanced Script -->
    <script type="text/javascript">
        function showLanguageAlert(value) {
            if (value === 'Kannada') {
                alert("Press ALT + 5");
            } else if (value === 'Hindi') {
                alert("Press ALT + 4");
            } else if (value === 'English') {
                alert("Press ALT + 12");
            }
        }

        window.onload = function () {
            var radios = document.querySelectorAll('[id$="rblLanguage"] input[type=radio]');
            radios.forEach(function (radio) {
                radio.onclick = function () {
                    showLanguageAlert(this.value);
                };
            });
        };
        function validatePaymentMode(sender, args) {

            var paymentMode = document.getElementById('<%= ddlPaymentMode.ClientID %>').value;
            var utr = document.getElementById('<%= txtUTRNumber.ClientID %>').value.trim();
            var bank = document.getElementById('<%= txtBankName.ClientID %>').value.trim();

            if (paymentMode === "Cash" && (utr !== "" || bank !== "")) {
                args.IsValid = false;
            } else {
                args.IsValid = true;
            }
        }

        document.addEventListener("DOMContentLoaded", function () {

            var paymentMode = document.getElementById('<%= ddlPaymentMode.ClientID %>');
            var bankName = document.getElementById('<%= txtBankName.ClientID %>');
            var branch = document.getElementById('<%= txtBranch.ClientID %>');
            var ifsc = document.getElementById('<%= txtIFSC.ClientID %>');
            var account = document.getElementById('<%= txtAccountNo.ClientID %>');
            var bankSection = document.getElementById("bankDetailsSection");

            function setBankDetails() {

                if (paymentMode.value === "Bank") {

                    bankSection.style.display = "flex";

                    bankName.value = "State Bank of India";
                    branch.value = "Main Branch";
                    ifsc.value = "SBIN0001234";
                    account.value = "123456789012";

                } else {

                    bankSection.style.display = "none";

                    bankName.value = "";
                    branch.value = "";
                    ifsc.value = "";
                    account.value = "";

                }
            }

            paymentMode.addEventListener("change", setBankDetails);

            // run on page load (important for edit mode)
            setBankDetails();

        });

        // Enhanced form interactions
        document.addEventListener('DOMContentLoaded', function () {
            // Add focus effects to form controls
            const formControls = document.querySelectorAll('.form-control, .form-select');
            formControls.forEach(control => {
                control.addEventListener('focus', function () {
                    this.classList.add('border-primary', 'shadow');
                });
                control.addEventListener('blur', function () {
                    this.classList.remove('border-primary', 'shadow');
                });
            });

            // Add animation to success message
            const successMessage = document.querySelector('.alert-success');
            if (successMessage) {
                successMessage.classList.add('animate__animated', 'animate__bounceIn');
            }
        });

        function validateMinimumAmount(sender, args) {

            var sevaTypeElement = document.getElementById('<%= ddlMainSeva.ClientID %>');
            var amountElement = document.getElementById('<%= txtPrice.ClientID %>');

            var sevaType = sevaTypeElement ? sevaTypeElement.value : "";
            var amountText = amountElement ? amountElement.value.trim() : "";

            // Let RequiredFieldValidator handle empty amount
            if (amountText === "") {
                args.IsValid = true;
                return;
            }

            var amount = parseFloat(amountText);

            if (isNaN(amount)) {
                sender.innerHTML = "Please enter a valid amount.";
                args.IsValid = false;
                return;
            }

            // NIRANTARA ABHISHEK Minimum ₹2,500

            if (sevaType === "1") {

                if (amount < 2500) {

                    sender.innerHTML =
                        "For Nirantara Abhishek, amount must be ₹2,500 or more.";

                    args.IsValid = false;
                    return;
                }
            }


            // NIRANTARA ANNAPRASAD Minimum ₹6,000

            if (sevaType === "2") {

                if (amount < 6000) {

                    sender.innerHTML =
                        "For Nirantara Annaprasad, amount must be ₹6,000 or more.";

                    args.IsValid = false;
                    return;
                }
            }

            args.IsValid = true;
        }

        function toggleAdditionalAmount() {

            var radios = document.querySelectorAll(
                '[id$="rblSevaStart"] input[type="radio"]'
            );

            var selectedValue = "";

            radios.forEach(function (radio) {
                if (radio.checked) {
                    selectedValue = radio.value;
                }
            });

            var section = document.getElementById(
                "additionalAmountSection"
            );

            var additionalAmount = document.getElementById(
        '<%= txtAdditionalAmount.ClientID %>'
    );

            if (selectedValue === "ThisYear") {

                section.style.display = "block";

                if (additionalAmount.value.trim() === "") {
                    additionalAmount.value = "0";
                }

            } else {

                section.style.display = "none";
                additionalAmount.value = "0";
            }

            calculateTotal();
        }

        function calculateTotal() {

            var priceElement = document.getElementById(
        '<%= txtPrice.ClientID %>'
    );

    var additionalElement = document.getElementById(
        '<%= txtAdditionalAmount.ClientID %>'
    );

    var totalElement = document.getElementById(
        '<%= txtTotal.ClientID %>'
    );

            var selectedValue = "";

            document.querySelectorAll(
                '[id$="rblSevaStart"] input[type="radio"]'
            ).forEach(function (radio) {

                if (radio.checked) {
                    selectedValue = radio.value;
                }
            });

            var price = parseFloat(priceElement.value) || 0;

            var additional = 0;

            if (selectedValue === "ThisYear") {
                additional = parseFloat(additionalElement.value) || 0;
            }

            var total = price + additional;

            totalElement.value = total.toFixed(2);
        }

        // Run on initial page load and when editing
        document.addEventListener("DOMContentLoaded", function () {

            var radios = document.querySelectorAll(
                '[id$="rblSevaStart"] input[type="radio"]'
            );

            radios.forEach(function (radio) {
                radio.addEventListener("change", toggleAdditionalAmount);
            });

            toggleAdditionalAmount();
        });
    </script>
</asp:Content>
