using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace SSMS.Masters.Helper
{
    public class UserLogHelper
    {
        public static void Log(string actionType,string pageName = null,string recordID = null,string serialNo = null,string description = null)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                int? userMasterID = null;
                string userID = null;
                string userName = null;

                if (HttpContext.Current.Session["UserMasterID"] != null)
                {
                    int tempID;

                    if (int.TryParse(HttpContext.Current.Session["UserMasterID"].ToString(),out tempID))
                    {
                        userMasterID = tempID;
                    }
                }

                if (HttpContext.Current.Session["UserID"] != null)
                {
                    userID =HttpContext.Current.Session["UserID"].ToString().Trim();
                }
                if (HttpContext.Current.Session["UserName"] != null)
                {
                    userName =HttpContext.Current.Session["UserName"].ToString().Trim();
                }

                // PAGE NAME

                if (string.IsNullOrEmpty(pageName))
                {
                    pageName =
                        HttpContext.Current.Request.Url.AbsolutePath;
                }

                // INSERT LOG
                // LOGIN / LOGOUT / INSERT / UPDATE / DELETE / PRINT
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        INSERT INTO SMSF.dbo.UserLog
                        (UserMasterID,UserID,UserName,ActionType,PageName,RecordID,SerialNo,Description,LogDate,LogoutDate)
                        VALUES
                        (@UserMasterID, @UserID,@UserName,@ActionType,@PageName,@RecordID,@SerialNo,@Description,GETDATE(),NULL)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@UserMasterID",System.Data.SqlDbType.Int).Value = (object)userMasterID ?? DBNull.Value;
                        cmd.Parameters.Add("@UserID",System.Data.SqlDbType.NVarChar, 200).Value =(object)userID ?? DBNull.Value;
                        cmd.Parameters.Add("@UserName",System.Data.SqlDbType.NVarChar, 200).Value = (object)userName ?? DBNull.Value;
                        cmd.Parameters.Add("@ActionType", System.Data.SqlDbType.NVarChar, 50).Value =(object)actionType ?? DBNull.Value;
                        cmd.Parameters.Add("@PageName",System.Data.SqlDbType.NVarChar, 500).Value =(object)pageName ?? DBNull.Value;
                        cmd.Parameters.Add("@RecordID",System.Data.SqlDbType.NVarChar, 100).Value = (object)recordID ?? DBNull.Value;
                        cmd.Parameters.Add("@SerialNo",System.Data.SqlDbType.NVarChar, 100).Value =(object)serialNo ?? DBNull.Value;
                        cmd.Parameters.Add("@Description",System.Data.SqlDbType.NVarChar, -1).Value =(object)description ?? DBNull.Value;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UserLogHelper Error: " + ex.ToString());
            }
        }
    }
}