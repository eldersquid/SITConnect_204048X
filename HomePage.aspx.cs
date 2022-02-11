using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_204048X
{
    public partial class HomePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["LoggedIn"] != null && Session["AuthToken"] !=null && Request.Cookies["AuthToken"].Value != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    email_label.Text = Session["LoggedIn"].ToString();
                    SessionIDManager newSession = new SessionIDManager();
                    var newSessionID = newSession.CreateSessionID(HttpContext.Current);
                    var redirect = false;
                    var added = false;
                    newSession.SaveSessionID(HttpContext.Current, newSessionID, out redirect, out added);
                    success_label.Text = "You are logged in with " + email_label.Text + ".";
                    success_label.ForeColor = System.Drawing.Color.AliceBlue;
                    logout_button.Visible = true;
                    //error_label.Text = "Session ID from session is " + Session.SessionID.ToString() + " and Session ID from cookie is " + Request.Cookies["ASP.NET_SessionId"].Value.ToString();

                    //if (Session.SessionID != Session["RegistrationID"])
                    //{
                    //    session_label.Text = "Not the same. Session ID is " + Session.SessionID.ToString() + " and registration ID is " + Session["RegistrationID"];
                    //}
                    //else
                    //{
                    //    session_label.Text = "Session ID from session is " + Session.SessionID.ToString() + " and registration ID is " + Session["RegistrationID"];
                    //}

                }
                
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void logoutMethod(object sender,EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-1);
            }    
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-1);
            }
            
        }

        protected void changePW(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(email_label.Text);
            Session["Email"] = email_label.Text;
            Session["LoggedIn"] = email_label.Text;
            Session["AuthToken"] = Request.Cookies["AuthToken"].Value;
            System.Diagnostics.Debug.WriteLine("This is auth token cookie : ");
            System.Diagnostics.Debug.WriteLine(Request.Cookies["AuthToken"].Value);
            Response.Redirect("ChangePassword.aspx", false);

        }
    }
}