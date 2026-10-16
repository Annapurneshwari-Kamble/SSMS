<%@ Page Title="Import Excel"
    Language="C#"
    MasterPageFile="~/Pages/Admin/Admin_Master.Master"
    AutoEventWireup="true"
    CodeBehind="ImportExcel.aspx.cs"
    Inherits="SSMS.Pages.Admin.ImportExcel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .import-card {
            max-width: 900px;
            margin: 30px auto;
        }

        .upload-box {
            border: 2px dashed #ccc;
            border-radius: 10px;
            padding: 35px;
            text-align: center;
            background: #fafafa;
        }

        .result-box {
            margin-top: 20px;
            padding: 15px;
            border-radius: 8px;
        }

        .success {
            background: #e8f5e9;
            color: #2e7d32;
        }

        .warning {
            background: #fff8e1;
            color: #f57c00;
        }

        .error {
            background: #ffebee;
            color: #c62828;
        }

        .info {
            background: #e3f2fd;
            color: #1565c0;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container">

        <div class="card shadow-sm import-card">

            <div class="card-header">
                <h4 class="mb-0">
                    <i class="fa-solid fa-file-import"></i>
                    Import Excel
                </h4>
            </div>

            <div class="card-body">

                <div class="upload-box">

                    <i class="fa-solid fa-file-excel fa-3x mb-3"></i>

                    <h5>Import Seva Member Data</h5>

                    <p class="text-muted">
                        Upload Excel file containing Seva Member Details.
                    </p>

                    <asp:FileUpload
                        ID="fuExcel"
                        runat="server"
                        CssClass="form-control mb-3"
                        accept=".xlsx,.xls" />

                    <asp:Button
                        ID="btnImport"
                        runat="server"
                        Text="Import Excel"
                        CssClass="btn btn-primary"
                        OnClick="btnImport_Click" />

                </div>


                <asp:Panel
                    ID="pnlResult"
                    runat="server"
                    Visible="false"
                    CssClass="result-box info">

                    <asp:Label
                        ID="lblResult"
                        runat="server">
                    </asp:Label>

                </asp:Panel>


                <asp:Panel
                    ID="pnlErrors"
                    runat="server"
                    Visible="false"
                    CssClass="result-box error">

                    <h6>
                        <i class="fa-solid fa-circle-exclamation"></i>
                        Import Errors
                    </h6>

                    <asp:Literal
                        ID="litErrors"
                        runat="server">
                    </asp:Literal>

                </asp:Panel>


                <div class="mt-4">

                    <h6>Excel Requirements</h6>

                    <ul>
                        <li>Column order does not matter.</li>
                        <li>Extra Excel columns are ignored.</li>
                        <li>Existing records are identified using <b>RegisterNo</b>.</li>
                        <li>If RegisterNo is empty for a new record, it will be generated automatically.</li>
                        <li>Seva For values such as Siddharood Swami, Gurunathrood Swami and Dasoha are supported.</li>
                        <li>Both old and new records can exist in the same Excel file.</li>
                    </ul>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
