<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Admin/Admin_Master.Master" AutoEventWireup="true" CodeBehind="NirantaraAnnaprasadForm.aspx.cs" Inherits="SSMS.Pages.Admin.NirantaraAnnaprasadForm" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: 'Times New Roman', Times, serif;
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            background-color: #f0f0f0;
            color: orangered; /* Sets all text to blue */
        }

        .page-container {
            width: 210mm;
            min-height: 297mm;
            margin: auto;
            background: #fff;
            padding: 20mm;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

        .header {
            text-align: center;
            border-bottom: 2px solid #000;
            padding-bottom: 5px;
            margin-bottom: 20px;
        }

        .header-top {
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 14px;
            margin-bottom: 5px;
        }

        .header-main {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 20px;
            margin-top: 5px;
        }

            .header-main img {
                height: 80px;
                width: auto;
            }

            .header-main .title-section {
                text-align: center;
                line-height: 1.2;
            }

            .header-main .kannada-title {
                font-size: 24px;
                font-weight: bold;
                margin: 0;
            }

            .header-main .english-title {
                font-size: 18px;
                font-weight: bold;
                margin: 0;
            }

        .header-contact-info {
            font-size: 12px;
            margin-top: 5px;
        }

            .header-contact-info a {
                color: orangered; /* Ensures links are also blue */
                text-decoration: none;
            }

        .content {
            margin-top: 20px;
        }

        .kannada-section {
            font-family: Arial, sans-serif;
            margin-bottom: 0; /* Reduced gap */
            border-bottom: 1px dashed #000;
            padding-bottom: 5px; /* Reduced gap */
        }

            .kannada-section p {
                font-size: 16px;
                line-height: 1.5;
                margin: 0;
                text-indent: 40px;
            }

                .kannada-section p.no-indent {
                    text-indent: 0;
                }

        .kannada-signature {
            text-align: right;
            margin-top: 20px;
        }

            .kannada-signature p {
                margin: 0;
                font-weight: bold;
                line-height: 1.2;
            }

        .english-section {
            font-family: 'Times New Roman', Times, serif;
            margin-top: 20px;
            line-height: 1.6;
        }

            .english-section .salutation {
                margin-bottom: 10px;
            }

            .english-section .main-text {
                margin: 0 0 10px 40px;
            }

            .english-section .blessing {
                font-style: italic;
                margin: 0 0 0 40px;
            }

        .english-signature {
            text-align: right;
            margin-top: 30px;
        }

            .english-signature p {
                margin: 0;
                font-weight: bold;
                line-height: 1.2;
            }

        .footer {
            margin-top: 5px; /* Reduced gap */
            text-align: center;
            font-size: 12px;
            border-top: 1px dashed #000;
            padding-top: 5px; /* Reduced gap */
        }

        .footer-note {
            margin-bottom: 10px;
            border: 1px solid orangered;
            padding: 5px;
            line-height: 1.5;
        }

            .footer-note p {
                margin: 0;
            }

        .kannada-section {
            font-size: 14px;
            line-height: 1.6;
        }

            .kannada-section p {
                font-size: 14px !important;
                line-height: 1.6 !important;
            }

        .main-text {
            text-align: justify; /* makes it proper paragraph alignment */
            line-height: 1.6; /* improves readability */
        }

        .kannada-section p {
            text-align: justify; /* proper paragraph alignment */
            line-height: 1.8; /* improves readability */
        }

        .kannada-signature {
            text-align: right !important; /* force parent alignment */
            margin-top: 20px;
        }

            .kannada-signature p {
                text-align: right !important; /* force children alignment */
                margin: 0;
                font-weight: bold;
            }

        .om-text {
            text-align: center;
            margin-bottom: 5px;
        }

            .om-text h6 {
                margin: 0;
                font-size: 16px;
                font-weight: bold;
            }

        .om-image {
            text-align: center;
            margin-bottom: 5px;
        }

            .om-image img {
                height: 40px; /* adjust size as needed */
                width: auto;
            }

        .center-logo {
            flex: 1; /* take remaining space */
            text-align: center; /* center content */
        }

        .right-info {
            text-align: right;
            margin: 0; /* remove default spacing if needed */
        }

        .kannada-title {
            white-space: nowrap; /* Prevents line break */
            overflow: hidden; /* Optional: hides overflow if too long */
            text-align: center; /* Center align if needed */
            font-size: 24px; /* Adjust size */
            margin-bottom: 5px; /* Reduce the space below the h1 */
        }

        .kannada-subtitle {
            margin-top: 0; /* Remove top margin of the paragraph */
        }

        .header-images-container {
            display: flex; /* This aligns the images side-by-side */
            justify-content: space-between; /* This pushes them to the far ends of the container */
            align-items: center; /* This centers them vertically */
            width: 100%; /* The container takes up the full width of its parent */
            max-width: 100px; /* Adjust this value as needed to control overall width */
            margin-left: -40px;
            margin-right: -40px;
        }

            .header-images-container img {
                max-width: 100px; /* Adjust this value to control the width of each logo */
                height: auto; /* This maintains the image's aspect ratio */
            }
        /* Force page size A4 */
        /* Force exact A4 size */
        @page {
            size: A4 portrait; /* or landscape */
            margin: 0;
        }

        html, body {
            margin: 0;
            padding: 0;
            background: #f0f0f0;
        }

        .page-container {
            width: 794px; /* 210mm ≈ 794px */
            height: 1123px; /* 297mm ≈ 1123px */
            margin: auto;
            background: #fff;
            padding: 40px; /* ~20mm padding equivalent */
            box-sizing: border-box;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }

        @media print {
            body {
                background: none;
            }

            .page-container {
                width: 794px !important;
                height: 1123px !important;
                margin: 0;
                padding: 40px;
                box-shadow: none;
                page-break-after: always;
            }

            .no-print {
                display: none !important;
            }

            .button-container {
                display: none !important;
            }
        }

        .kannada-section .main-text {
            margin: 0 0 10px 40px;
        }

        /* New styles for buttons */
        .button-container {
            text-align: center;
            margin: 20px auto;
            width: 794px;
        }

        .action-button {
            background-color: #0044cc;
            color: white;
            border: none;
            padding: 10px 20px;
            margin: 0 10px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 16px;
            transition: background-color 0.3s;
        }

            .action-button:hover {
                background-color: #0033aa;
            }
    </style>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Button container outside the A4 content -->
    <div class="button-container">
        <%-- <button id="btnPrint" class="action-button" onclick="printPage()">Print Form</button>--%>
        <button id="btnDownloadPDF" class="action-button" onclick="downloadPDF()">Download PDF</button>
    </div>

    <div class="page-container">
        <div class="header">
            <div class="om-text">
                <h6 class="right-info">Math Office: 0836-2203485</h6>

                <h6>ಓಂ ನಮಃ ಶಿವಾಯ</h6>


            </div>
            <div class="header-top">

                <div class="center-logo">
                    <asp:Literal ID="Literal2" runat="server"
                        Text="<img src='../../assets/images/om2.jpg' alt='Om symbol' style='height: 20px; width: auto;' />" />
                </div>


            </div>
            <div class="header-main">
                <div class="header-images-container">
                    <img src="../../assets/images/siddharrodha.png" alt="Shri Siddharoodha Swamiji Logo Left " />
                </div>
                <div class="title-section">
                    <h1 class="kannada-title">ಶ್ರೀ ಸಿದ್ಧಾರೂಢ ಸ್ವಾಮಿಯವರ ಮಠ ಟ್ರಸ್ಟ ಕಮೀಟಿ</h1>
                    <p class="kannada-subtitle mb-0">ಹುಬ್ಬಳ್ಳಿ— 580 024.</p>
                    <p class="kannada-subtitle mb-1">Public Trust Reg. No. A-2609 (DWR)</p>
                    <p class="english-title">Shri Siddharoodha Swamiji Math Trust Committee,</p>
                    <p class="header-contact-info">
                        Hubli - 580 024. Dist : Dharwad (Karnataka State)<br />
                        Website: <a href="http://www.srisiddharoodhaswamiji.in">"srisiddharoodhaswamiji.in"</a> E-mail: ssmath1836@gmail.com<br />
                        Public Trust Reg. No. A- 2609 (DWR)<br />
                        ಆದಾಯ ತೆರಿಗೆ ವಿನಾಯ್ತಿ (80 G) ನಂ. F.No. 118 / 580 / AACTS 3690 N / CIT - HBL / 2010-11
                    </p>
                </div>
                <div class="header-images-container">
                    <img src="../../assets/images/gurunathswamij.png" alt="Shri Siddharoodha Swamiji Logo Right" />
                </div>
            </div>
        </div>

        <div class="content">
            <div class="kannada-section fs-6">
                <p class="main-text">
              <big style="margin-left:-40px;">ಮಾನ್ಯರೆ</big>,<br />
                    ತಾವು ಮಾಡಿಸುವ <b>ಶ್ರೀ ಸಿದ್ದಾರೂಢ ಸ್ವಾಮಿಗಳವರ / ಶ್ರೀ ಗುರುನಾಥರೂಢ ಸ್ವಾಮಿಗಳವರ ನಿರಂತರ ದಾಸೋಹದ</b> ತಾರೀಖು <b><span id="kannadaDate" runat="server"></span></b> ವಾರ ಇದ್ದು ಆ ದಿವಸ ತಾವುಗಳು ಮುಂಜಾನೆ <b>11-00</b> ಘಂಟೆಗೆ ಹಾಜರಿದ್ದು ಶ್ರೀಗಳವರಿಗೆ ನೈವೇದ್ಯವನ್ನು ಮಾಡಿ ಮಹಾಪ್ರಸಾದ ಸ್ವೀಕರಿಸಿ ಉಭಯ ಶ್ರೀಗಳವರ ಕೃಪಾಶೀರ್ವಾದಕ್ಕೆ ಪಾತ್ರರಾಗಬೇಕೆಂದು ಈ ಮೂಲಕ ಕೋರಲಾಗಿದೆ.<br />
                    ಈ ದಿವಸ ತಮಗೆ ಬರಲು ಸಾಧ್ಯವಾಗದೇ ಇದ್ದರೆ ಅದೇ ತಿಂಗಳಲ್ಲಿ ಬಂದು ಪೂಜೆಯನ್ನು ನೆರವೇರಿಸಿಕೊಂಡು ಹೋಗುವೆದು. ಇಲ್ಲವಾದಲ್ಲಿ ತಮ್ಮಗಳ ಹೆಸರಿನಲ್ಲಿ ಪೂಜೆಯನ್ನು ನೆರವೇರಿಸಿ ಪ್ರಸಾದ ಮುಟ್ಟಿಸುವ ವ್ಯವಸ್ಥೆ ಮಾಡಲಾಗುವದು.<br />
                    ಗೌರವಗಳೊಂದಿಗೆ
                </p>
                <div class="kannada-signature fs-5">
                    <p>ತಮ್ಮ ವಿಶ್ವಾಸಿ,</p>
                    <br />
                    <p <%--style="margin-top: 40px;"--%>>ಶ್ರೀ ಸಿದ್ಧಾರೂಢ ಸ್ವಾಮಿಯವರ ಮಠ ಟ್ರಸ್ಟ ಕಮಿಟಿ, ಹುಬ್ಬಳ್ಳಿ</p>
                </div>

            </div>

            <div class="english-section fs-6">
                <%--<p class="salutation">Sir,</p>--%>
                <p class="main-text">
                    Sir/Madam,<br />
                    We are pleased inform you that the Nirantara Dasoha booked by you is due on <b><span id="englishDate" runat="server"></span></b> Hence you are requested to be present on same day at 11-00am. to perform theb Nirantara Dasoha in Shri Siddharoodha Swamiji Math and get the blessing of Lord Shri Siddharoodha Swamiji and Shri Gurunatharoodha Swamiji.<br />
                    In the event of your inability to attend the Nirantara Dasoha on the day. The Nirantara Dasoha will be performed in your names.<br />
                    Thanking You.
                </p>


                <div class="english-signature">
                    <p>Your's in Faithfully,</p>
                    <br />
                    <p>Shri Siddharoodha Swamiji Math Trust Committee, Hubli</p>
                    <br />
                    <p class="text-center fs-6">
                        <p style="text-align: center; line-height: 1.6; font-weight: bold;">
                            ಸೂಚನೆ : ಈ ಪತ್ರವನ್ನು ಬರುವಾಗ ತಪ್ಪದೇ ತರಬೇಕು. ಪ್ರತಿ ದಾಸೋಹಕ್ಕೆ 25 ಜನರಿಗೆ ಮಾತ್ರ ಪ್ರಸಾದ ವ್ಯವಸ್ಥೆ ಇರುತ್ತದೆ.
                        </p>

                    </p>
                </div>
            </div>

            <div class="footer fs-6">

                <div class="footer-note">
                    <p>
                        <span style="font-weight: bold;">Note:</span> While Coming to attend the Nirantara Dasoha kindly bring this intimation. We will arrange prasadam for 25 Members only,which please note.
                    </p>

                </div>
                <p>
                    <span style="font-weight: bold;">ವಿಶೇಷ ಸೂಚನೆ:</span> <b>ನಿರಂತರ ಅಭಿಷೇಕ / ದಾಸೋಹದ ಸದಸ್ಯರು ಪ್ರಸ್ತುತ ಈಗಿರುವ ತಮ್ಮ ಮೊಬೈಲ ಸಂಖ್ಯೆಯನ್ನು ಶ್ರೀಮಠದ ಕಾರ್ಯಾಲಯಕ್ಕೆ ನೀಡಲು ವಿನಂತಿ.</b>
                </p>
            </div>
        </div>

        <!-- Certificate Panel -->
        <asp:Panel ID="pnlCertificate" runat="server">
            <!-- your certificate content goes here -->
        </asp:Panel>

    </div>

    <script>
        // Automatically fill date
        window.onload = function () {
            // Get today's date in DD-MM-YYYY format
            var today = new Date();
            var day = today.getDate().toString().padStart(2, '0');
            var month = (today.getMonth() + 1).toString().padStart(2, '0');
            var year = today.getFullYear();
            var formattedDate = day + '-' + month + '-' + year;

            // Fill all date placeholders
            var dateElements = document.querySelectorAll('.certificate-date');
            dateElements.forEach(function (elem) {
                elem.innerText = formattedDate;
            });
        }

        //function printPage() {
        //    window.print();
        //}

        function downloadPDF() {
            // Create a clone of the page container
            var originalContent = document.querySelector('.page-container');
            var clone = originalContent.cloneNode(true);

            // Create a new window for printing
            var printWindow = window.open('', '_blank');

            // Write the HTML content to the new window
            printWindow.document.write(`
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Nirantara Annaprasad Form</title>
                    <style>
                        body {
                            font-family: 'Times New Roman', Times, serif;
                            margin: 0;
                            padding: 0;
                            color: orangered;
                        }
                        .page-container {
                            width: 210mm;
                            min-height: 297mm;
                            margin: auto;
                            padding: 20mm;
                        }
                        .header {
                            text-align: center;
                            border-bottom: 2px solid #000;
                            padding-bottom: 5px;
                            margin-bottom: 20px;
                        }
                        .header-top {
                            display: flex;
                            justify-content: space-between;
                            align-items: center;
                            font-size: 14px;
                            margin-bottom: 5px;
                        }
                        .header-main {
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            gap: 20px;
                            margin-top: 5px;
                        }
                        .header-main img {
                            height: 80px;
                            width: auto;
                        }
                        .header-main .title-section {
                            text-align: center;
                            line-height: 1.2;
                        }
                        .header-main .kannada-title {
                            font-size: 24px;
                            font-weight: bold;
                            margin: 0;
                        }
                        .header-main .english-title {
                            font-size: 18px;
                            font-weight: bold;
                            margin: 0;
                        }
                        .header-contact-info {
                            font-size: 12px;
                            margin-top: 5px;
                        }
                        .header-contact-info a {
                            color: orangered;
                            text-decoration: none;
                        }
                        .content {
                            margin-top: 20px;
                        }
                        .kannada-section {
                            font-family: Arial, sans-serif;
                            margin-bottom: 0;
                            border-bottom: 1px dashed #000;
                            padding-bottom: 5px;
                        }
                        .kannada-section p {
                            font-size: 16px;
                            line-height: 1.5;
                            margin: 0;
                            text-indent: 40px;
                        }
                        .kannada-section p.no-indent {
                            text-indent: 0;
                        }
                        .kannada-signature {
                            text-align: right;
                            margin-top: 20px;
                        }
                        .kannada-signature p {
                            margin: 0;
                            font-weight: bold;
                            line-height: 1.2;
                        }
                        .english-section {
                            font-family: 'Times New Roman', Times, serif;
                            margin-top: 20px;
                            line-height: 1.6;
                        }
                        .english-section .salutation {
                            margin-bottom: 10px;
                        }
                        .english-section .main-text {
                            margin: 0 0 10px 40px;
                        }
                        .english-section .blessing {
                            font-style: italic;
                            margin: 0 0 0 40px;
                        }
                        .english-signature {
                            text-align: right;
                            margin-top: 30px;
                        }
                        .english-signature p {
                            margin: 0;
                            font-weight: bold;
                            line-height: 1.2;
                        }
                        .footer {
                            margin-top: 5px;
                            text-align: center;
                            font-size: 12px;
                            border-top: 1px dashed #000;
                            padding-top: 5px;
                        }
                        .footer-note {
                            margin-bottom: 10px;
                            border: 1px solid orangered;
                            padding: 5px;
                            line-height: 1.5;
                        }
                        .footer-note p {
                            margin: 0;
                        }
                        .kannada-section {
                            font-size: 14px;
                            line-height: 1.6;
                        }
                        .kannada-section p {
                            font-size: 14px !important;
                            line-height: 1.6 !important;
                        }
                        .main-text {
                            text-align: justify;
                            line-height: 1.6;
                        }
                        .kannada-section p {
                            text-align: justify;
                            line-height: 1.8;
                        }
                        .kannada-signature {
                            text-align: right !important;
                            margin-top: 20px;
                        }
                        .kannada-signature p {
                            text-align: right !important;
                            margin: 0;
                            font-weight: bold;
                        }
                        .om-text {
                            text-align: center;
                            margin-bottom: 5px;
                        }
                        .om-text h6 {
                            margin: 0;
                            font-size: 16px;
                            font-weight: bold;
                        }
                        .om-image {
                            text-align: center;
                            margin-bottom: 5px;
                        }
                        .om-image img {
                            height: 40px;
                            width: auto;
                        }
                        .center-logo {
                            flex: 1;
                            text-align: center;
                        }
                        .right-info {
                            text-align: right;
                            margin: 0;
                        }
                        .kannada-title {
                            white-space: nowrap;
                            overflow: hidden;
                            text-align: center;
                            font-size: 24px;
                            margin-bottom: 5px;
                        }
                        .kannada-subtitle {
                            margin-top: 0;
                        }
                        .header-images-container {
                            display: flex;
                            justify-content: space-between;
                            align-items: center;
                            width: 100%;
                            max-width: 100px;
                            margin-left: -40px;
                            margin-right: -40px;
                        }
                        .header-images-container img {
                            max-width: 100px;
                            height: auto;
                        }
                        @page {
                            size: A4 portrait;
                            margin: 0;
                        }
                        @media print {
                            body {
                                background: none;
                            }
                            .page-container {
                                width: 794px !important;
                                height: 1123px !important;
                                margin: 0;
                                padding: 40px;
                                box-shadow: none;
                                page-break-after: always;
                            }
                        }
                        .kannada-section .main-text {
                            margin: 0 0 10px 40px;
                        }
                    </style>
                </head>
                <body>
                    ${clone.outerHTML}
                </body>
                </html>
            `);

            printWindow.document.close();

            // Wait for content to load then trigger print
            printWindow.onload = function () {
                printWindow.print();
            };
        }
    </script>

</asp:Content>
