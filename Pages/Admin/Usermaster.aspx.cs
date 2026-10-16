using System;
using System.IO;

// Aliases avoid clashes between namespace and class names (BL.BL, GC.*, System.GC)
using SevaBL = SSMS.Masters.BL.BL;
using UserMasterGC = SSMS.Masters.GC.UserMasterGC;

namespace SSMS.Pages.Admin
{
    public partial class Usermaster : System.Web.UI.Page
    {
        private readonly SevaBL bl = new SevaBL();

        private static readonly string[] AllowedImageTypes = { ".jpg", ".jpeg", ".png", ".gif" };

        // 0 = new user, > 0 = editing
        private int EditId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["msg"] == "success")
                {
                    lblMessage.Text = "✅ Record saved successfully!";
                    lblMessage.CssClass = "text-success fw-bold";
                    Response.AddHeader("Refresh", "1;url=Usermaster.aspx");
                }

                if (EditId > 0)
                {
                    LoadUser(EditId);
                }
            }
        }

        // ------------------------------------------------------------
        //  LOAD (edit mode)
        // ------------------------------------------------------------
        private void LoadUser(int id)
        {
            UserMasterGC u = bl.GetUserById(id);
            if (u == null) return;

            txtUserName.Text = u.UserName;
            txtFirstName.Text = u.FirstName;
            txtLastName.Text = u.LastName;
            txtMobile.Text = u.MobileNo;
            txtEmail.Text = u.Email;
            txtInstituteCode.Text = u.InstituteCode;
            ddlUserType.SelectedValue = u.UserType;
            txtUserId.Text = u.UserId;
            chkIsActive.Checked = u.IsActive;

            btnSave.Text = "Update";
            rfvPassword.Enabled = false;
        }

        // ------------------------------------------------------------
        //  SAVE
        // ------------------------------------------------------------
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                // Image upload is a UI/file-system job, so it stays in the page
                string filePath = "";

                if (fuProfile.HasFile)
                {
                    string ext = Path.GetExtension(fuProfile.FileName).ToLowerInvariant();

                    if (Array.IndexOf(AllowedImageTypes, ext) < 0)
                    {
                        lblMessage.Text = "Only JPG, PNG or GIF images are allowed.";
                        lblMessage.CssClass = "text-danger";
                        return;
                    }

                    string folderPath = Server.MapPath("~/Uploads/Users/");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // Unique name so two users uploading "photo.jpg" don't overwrite each other
                    string fileName = Guid.NewGuid().ToString("N") + ext;
                    fuProfile.SaveAs(Path.Combine(folderPath, fileName));
                    filePath = "~/Uploads/Users/" + fileName;
                }

                var model = new UserMasterGC
                {
                    Id = EditId,
                    UserName = txtUserName.Text,
                    Password = txtPassword.Text,
                    ProfileImage = filePath,
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    MobileNo = txtMobile.Text,
                    Email = txtEmail.Text,
                    InstituteCode = txtInstituteCode.Text,
                    UserType = ddlUserType.SelectedValue,
                    UserId = txtUserId.Text,
                    IsActive = chkIsActive.Checked
                };

                bl.SaveUser(model);
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                lblMessage.CssClass = "text-danger";
                return;
            }

            // Redirect outside try/catch so it can't be caught as an error
            Response.Redirect("UsermasterList.aspx?msg=updated", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        // ------------------------------------------------------------
        //  CLEAR
        // ------------------------------------------------------------
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtUserName.Text = "";
            txtPassword.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtMobile.Text = "";
            txtEmail.Text = "";
            txtInstituteCode.Text = "";
            ddlUserType.SelectedIndex = 0;
            txtUserId.Text = "";
            chkIsActive.Checked = false;
            lblMessage.Text = "";
        }
    }
}