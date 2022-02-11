using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_204048X
{
    public partial class TwoFactorAuthentication : System.Web.UI.Page
    {
        string secretCode = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
        string SITDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
            if (Session["Email"] != null)
            {
                var email = Session["Email"].ToString();
                TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
                var accountSecretKey = $"{secretCode}-{email}";
                var setupCode = twoFactor.GenerateSetupCode("2FA", email, secretCode, false, 3);

                barcodeURL.ImageUrl = setupCode.QrCodeSetupImageUrl;
                setupCodeText.Text = setupCode.ManualEntryKey;


            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void enable_button_Click(object sender, EventArgs e)
        {
            
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            var email = Session["Email"].ToString();
            var accountSecretKey = $"{secretCode}-{email}";
            bool result = twoFactorAuthenticator.ValidateTwoFactorPIN(secretCode, inputCode.Text, TimeSpan.FromDays(100));
           
            System.Diagnostics.Debug.Write(result.ToString());
            if (!result)
            {
                System.Diagnostics.Debug.Write(result);
                
                error_twofac.Text = "Security code has expired. Please enter a new one.";
                error_twofac.ForeColor = System.Drawing.Color.Red;



            }
            else
            {
                changeTwoFactor(email);
                Session["LoggedIn"] = email;
                Session["Email"] = email;
                string guid = Guid.NewGuid().ToString();
                Session["AuthToken"] = guid;
                Response.Cookies.Add(
                    new HttpCookie("AuthToken", guid)
                    {
                        Secure = true,
                        HttpOnly = true
                    }
                );
                Response.Redirect("Login.aspx", false);
            }

        }

        protected void logoutMethod(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();



            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
            SessionIDManager newSession = new SessionIDManager();
            var newSessionID = newSession.CreateSessionID(HttpContext.Current);
            var redirect = false;
            var added = false;
            newSession.SaveSessionID(HttpContext.Current, newSessionID, out redirect, out added);
            Response.Redirect("Login.aspx", false);
        }

        protected void changeTwoFactor(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [Users] SET TwoFactorEnabled=@TwoFactorEnabled WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@TwoFactorEnabled", true);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteScalar();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
        }
    }
}