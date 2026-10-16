<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SSMS.Default" %>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Sidharood Math - Admin Login</title>

    <meta name="description" content="Sidharood Math Administration Dashboard">
    <meta name="author" content="Sidharood Math">
    <link rel="shortcut icon" href="assets/images/favicon.svg">
    
    <link rel="stylesheet" href="assets/fonts/remix/remixicon.css">
    <link rel="stylesheet" href="assets/css/main.min.css">
    
    <style>
        .login-wrapper {
            min-height: 100vh;
            background-image: url('/assets/images/ssmabg2.jpg');
            background-repeat: no-repeat;
            background-position: center center;
            background-size: cover;
            position: relative;
        }
        
        .login-wrapper::before {
            content: "";
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(0, 0, 0, 0.4);
            backdrop-filter: blur(3px);
            -webkit-backdrop-filter: blur(3px);
        }

        .login-card {
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255, 255, 255, 0.2);
            box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
            border-radius: 16px;
            width: 440px;
            max-width: 440px;
            transform: scale(0.75);
            transform-origin: center center;
            padding: 30px !important;

        }

        .form-control:focus {
            border-color: #4d78ff;
            box-shadow: 0 0 0 0.25rem rgba(77, 120, 255, 0.25);
        }
        
        .password-toggle:hover {
            background-color: rgba(77, 120, 255, 0.1);
            border-color: #4d78ff;
        }
        
        .login-btn {
            padding: 12px;
            font-weight: 600;
            letter-spacing: 0.5px;
            transition: all 0.3s ease;
        }
        
        .login-btn:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(77, 120, 255, 0.3);
        }
    </style>
</head>

<body class="login-wrapper">
    <form id="form1" runat="server">
        <div class="container d-flex align-items-center justify-content-center min-vh-100">
            <div class="login-card w-100">
                <!-- Logo/Header Section -->
                <div class="text-center mb-5">
                    <div class="mb-4">
                        <i class="ri-admin-line text-primary" style="font-size: 3.5rem;"></i>
                    </div>
                    <h2 class="fw-bold mb-2">SidharoodMath</h2>
                    <p class="text-muted">Administration Dashboard</p>
                </div>
                
                <!-- Login Form -->
                <div>
                    <h4 class="fw-semibold mb-4 text-center">Sign In to Continue</h4>
                    
                    <!-- Username Field -->
                    <div class="mb-4">
                        <label for="txtUsername" class="form-label fw-medium text-secondary mb-2">
                            <i class="ri-user-line me-2"></i>Username
                        </label>
                        <asp:TextBox ID="txtUsername" runat="server"
                            CssClass="form-control form-control-lg py-3"
                            Placeholder="Enter your Username"
                            aria-describedby="emailHelp">
                        </asp:TextBox>
      
                        <div id="emailHelp" class="form-text mt-2">
                            Enter your registered Username
                        </div>
                    </div>
                    
                    <!-- Password Field -->
                    <div class="mb-4">
                        <label for="txtPassword" class="form-label fw-medium text-secondary mb-2">
                            <i class="ri-lock-line me-2"></i>Password
                        </label>
                        <div class="input-group input-group-lg">
                            <asp:TextBox ID="txtPassword" runat="server"
                                TextMode="Password"
                                CssClass="form-control py-3"
                                Placeholder="Enter your password">
                            </asp:TextBox>
                          
                            <button class="btn btn-outline-secondary password-toggle" type="button" id="togglePassword">
                                <i class="ri-eye-line"></i>
                            </button>
                        </div>
                    </div>
               
                    <asp:Label
                        ID="lblError"
                        runat="server"
                        CssClass="text-danger fw-medium text-center d-block mb-3"
                        Visible="false">
                    </asp:Label>
                    <!-- Login Button -->
                    <div class="d-grid gap-3 mb-4">
                        <asp:Button ID="btnLogin" runat="server" 
                            Text="Sign In" 
                            CssClass="btn btn-primary login-btn btn-lg" 
                            OnClick="btnLogin_Click" />
                    </div>
                </div>
            </div>
        </div>
    </form>
    
    <!-- JavaScript for Password Toggle -->
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            const togglePassword = document.getElementById('togglePassword');
            const passwordInput = document.getElementById('<%= txtPassword.ClientID %>');
            
            if (togglePassword && passwordInput) {
                togglePassword.addEventListener('click', function() {
                    const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
                    passwordInput.setAttribute('type', type);
                    
                    // Toggle eye icon
                    const eyeIcon = this.querySelector('i');
                    if (type === 'text') {
                        eyeIcon.classList.remove('ri-eye-line');
                        eyeIcon.classList.add('ri-eye-off-line');
                    } else {
                        eyeIcon.classList.remove('ri-eye-off-line');
                        eyeIcon.classList.add('ri-eye-line');
                    }
                });
            }
        });
    </script>
    <script type="text/javascript">
        (function () {
            function trap() {
                history.pushState(null, document.title, location.href);
            }

            history.replaceState(null, document.title, location.href);
            trap();

            // Every Back press pushes the login page again
            window.addEventListener('popstate', trap);

            // Chrome ignores entries added without user interaction.
            // On the first click / key press / touch, add a buffer of fresh entries.
            var armed = false;
            function arm() {
                if (armed) return;
                armed = true;
                for (var i = 0; i < 30; i++) { trap(); }
            }
            ['mousedown', 'pointerup', 'touchend', 'keydown'].forEach(function (evt) {
                window.addEventListener(evt, arm, { capture: true, once: true });
            });

            // If restored from back/forward cache, reload
            window.addEventListener('pageshow', function (e) {
                if (e.persisted) { window.location.reload(); }
            });
        })();
    </script>
</body>
</html>