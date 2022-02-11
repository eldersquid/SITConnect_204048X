using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_204048X
{
    public partial class TwoFactorAuthorization : System.Web.UI.Page
    {
        string secretCode = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
        string SITDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
            var expiredPassword = Session["PasswordExpired"];
            if (Session["Email"] != null )
            {
                var email = Session["Email"].ToString();
                Session["LoggedIn"] = email;
            }
            else
            {
                Response.Redirect("Login.aspx");
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

        protected void authorize_button_Click(object sender, EventArgs e)
        {

            var email = Session["Email"].ToString();
            Session["LoggedIn"] = email;

            
            if (email != null)
            {

                var twoFactorAuthenticator = new TwoFactorAuthenticator();
                bool result = twoFactorAuthenticator.ValidateTwoFactorPIN(secretCode, inputCode.Text, TimeSpan.FromDays(100));
                if (!result)
                {

                    error_twofac.Text = "Invalid authorization code. Try again.";
                    error_twofac.ForeColor = System.Drawing.Color.PaleVioletRed;

                }
                else
                {
                    if (Session["PasswordExpired"] == "Yes")
                    {
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;
                        Response.Cookies.Add(
                            new HttpCookie("AuthToken", guid)
                            {
                                Secure = true,
                                HttpOnly = true
                            }
                        );
                        Response.Cookies.Add(
                            new HttpCookie("AuthToken", guid)
                            {
                                Secure = true,
                                HttpOnly = true
                            }
                        );
                        Session["Email"] = email;
                        Response.Redirect("ChangePassword.aspx");
                    }
                    else if (Session["PasswordExpired"] == "No")
                    {
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;
                        Response.Cookies.Add(
                            new HttpCookie("AuthToken", guid)
                            {
                                Secure = true,
                                HttpOnly = true
                            }
                        );
                        Session["Email"] = email;
                        Response.Redirect("Profile.aspx");
                    }
                    
                    

                    
                }

            }

        }



    }
}