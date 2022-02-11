using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Text;
using WebMatrix.WebData;
using System.Data;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SITConnect_204048X
{

 
    public class MyObject
    {
        public string success { get; set; }
        public List<string> ErrorMessage { get; set; }


    }
    public partial class Login : System.Web.UI.Page
    {
        string SITDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6Legu90dAAAAAIetX0vtYuMiV0NCL3jWlv2dOEH2 &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        error_label.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }

            catch (WebException ex)
            {
                throw ex;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
         

        }

        private bool checkEmail(string email)
        {

            if (Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        protected void loginMethod(object sender, EventArgs e)
        {
            string pwd = password_box.Text.Trim();
            string email = email_box.Text.Trim();
            if (checkEmail(email))
            {
                if (emailExist() > 0)
                {
                    SHA512Managed hashing = new SHA512Managed();
                    string dbHash = getDBHash(email);
                    string dbSalt = getDBSalt(email);
                    if (ValidateCaptcha())
                    {

                        try
                        {
                            if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                            {
                                string pwdWithSalt = pwd + dbSalt;
                                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                string userHash = Convert.ToBase64String(hashWithSalt);
                                if (userHash.Equals(dbHash))
                                {
                                    if (checkLock() == 1)
                                    {
                                        changeLockStatus();
                                        Session["Email"] = email_box.Text.Trim();
                                        Session["LoggedIn"] = email_box.Text.Trim();
                                       
                                        string guid = Guid.NewGuid().ToString();
                                        Session["AuthToken"] = guid;
                                        Response.Cookies.Add(
                                            new HttpCookie("AuthToken", guid)
                                            {
                                                Secure = true,
                                                HttpOnly = true
                                            }
                                        );
                                        if (Int32.Parse(checkPasswordTime().ToString()) <= 0)
                                        {
                                            Session["PasswordExpired"] = "Yes";
                                            Response.Redirect("TwoFactorAuthorization.aspx", false);
                                        }
                                        else
                                        {
                                            Session["PasswordExpired"] = "No";
                                            Response.Redirect("TwoFactorAuthorization.aspx", false);
                                        }

                                    }
                                    else
                                    {
                                        if (Int32.Parse(checkTime().ToString()) <= 0)
                                        {
                                            changeLockStatus();
                                            Session["LoggedIn"] = email_box.Text.Trim();
                                            Session["Email"] = email_box.Text.Trim();
                                            string guid = Guid.NewGuid().ToString();
                                            Session["AuthToken"] = guid;
                                            Response.Cookies.Add(
                                                new HttpCookie("AuthToken", guid)
                                                {
                                                    Secure = true,
                                                    HttpOnly = true
                                                }
                                            );
                                            if (Int32.Parse(checkPasswordTime().ToString()) <= 0)
                                            {
                                                Session["PasswordExpired"] = "Yes";
                                                Response.Redirect("TwoFactorAuthorization.aspx", false);
                                            }
                                            else
                                            {
                                                Session["PasswordExpired"] = "No";
                                                Response.Redirect("TwoFactorAuthorization.aspx", false);
                                            }

                                        }
                                        else
                                        {
                                            error_label.Text = "Email is currently locked out. It will unlock in " + checkTime().ToString() + " minutes.";
                                        }

                                    }

                                }
                                else
                                {
                                    if (checkLock() == 0)
                                    {
                                        if (Int32.Parse(checkTime().ToString()) <= 0)
                                        {
                                            changeLockStatus();
                                            error_label.Text = "Account is unlocked after 30 minutes but wrong password entered.";
                                        }
                                        else
                                        {
                                            error_label.Text = "Account is already locked. Please contact admin or wait " + checkTime().ToString() + " minutes.";
                                        }



                                    }
                                    else
                                    {
                                        var attempts = loginAttempt();
                                        if (attempts < 3)
                                        {

                                            error_label.Text = "Wrong email or password. " + (2 - attempts).ToString() + " attempts left.";
                                        }
                                        else
                                        {


                                            error_label.Text = "Maximum attempts of 3 reached. Account is locked for 30 minutes. Please contact admin(Danish).";

                                        }
                                    }




                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }
                        finally { }




                    }
                    else
                    {
                        error_label.Text = "CAPTCHA Failed.";
                    }

                }
                else
                {
                    error_label.Text = "Email not found in system.";
                }
                
            }
            else
            {
                error_label.Text = "Invalid email entered.";
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

        protected void sessionMethod(object sender, EventArgs e)
        {
            SessionIDManager newSession = new SessionIDManager();
            var newSessionID = newSession.CreateSessionID(HttpContext.Current);
            var redirect = false;
            var added = false;
            newSession.SaveSessionID(HttpContext.Current, newSessionID, out redirect, out added);
            Response.Redirect("Login.aspx", false);




        }

        protected int emailExist()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(@Email) FROM [Users] WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_box.Text));

                            cmd.Connection = con;
                            con.Open();
                            var count = Convert.ToInt32(cmd.ExecuteScalar());
                            con.Close();
                            return count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
        }


        protected string getDBHash(string email)
        {
            string SITDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITDBConnection"].ConnectionString;
            string h = null;
            SqlConnection connection = new SqlConnection(SITDBConnectionString);
            string sql = "select CurrentPasswordHash FROM [Users] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["CurrentPasswordHash"] != null)
                        {
                            if (reader["CurrentPasswordHash"] != DBNull.Value)
                            {
                                h = reader["CurrentPasswordHash"].ToString();
                            }
                        }
                    }
                }
            }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.ToString());
                                }
                                finally { connection.Close(); }
                                return h;
                            }



        protected string getDBSalt(string email)
        {
            string SITDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITDBConnection"].ConnectionString;
            string s = null;
            SqlConnection connection = new SqlConnection(SITDBConnectionString);
            string sql = "select CurrentPasswordSalt FROM [Users] WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["CurrentPasswordSalt"] != null)
                        {
                            if (reader["CurrentPasswordSalt"] != DBNull.Value)
                            {
                                s = reader["CurrentPasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
                plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }



        protected int checkLock()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT LockedOut FROM [Users] WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email_box.Text.Trim());

                            cmd.Connection = con;
                            con.Open();
                            var locked = cmd.ExecuteScalar().ToString();
                            if (locked == "False")
                            {
                                return 1;
                            }
                            else
                            {
                                return 0;
                            }
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

        protected int checkTime()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DateTimeLocked FROM [Users] WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email_box.Text.Trim());

                            cmd.Connection = con;
                            con.Open();
                            var dateTimeLockedString = cmd.ExecuteScalar().ToString();
                            DateTime dateTimeLocked = DateTime.Parse(dateTimeLockedString, System.Globalization.CultureInfo.InvariantCulture);
                            TimeSpan ts = dateTimeLocked.Subtract(DateTime.Now);
                            var minutesLocked = System.Math.Abs(Convert.ToInt32(ts.TotalMinutes));
                            System.Diagnostics.Debug.WriteLine("Date time locked : " + dateTimeLocked);
                            System.Diagnostics.Debug.WriteLine("Time span : " + ts);
                            System.Diagnostics.Debug.WriteLine("Minutes Locked : " + minutesLocked);
                            var minutesLeft = 30 - minutesLocked;
                            con.Close();
                            return minutesLeft;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
        }

        protected void changeLockStatus()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [Users] SET DateTimeLocked = null,LogInAttempts = @LogInAttempts, LockedOut = @LockedOut WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email_box.Text.Trim());
                            cmd.Parameters.AddWithValue("@LockedOut", false);
                            cmd.Parameters.AddWithValue("@LogInAttempts", 0);
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

        protected int loginAttempt()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd2 = new SqlCommand("SELECT LogInAttempts FROM [Users] WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd2.CommandType = CommandType.Text;
                            cmd2.Parameters.AddWithValue("@Email", email_box.Text.Trim());
                            
                            cmd2.Connection = con;
                            con.Open();
                            var attempts = Convert.ToInt32(cmd2.ExecuteScalar());
                            
                            if (attempts <2)
                            {
                                
                                using (SqlCommand cmd = new SqlCommand("UPDATE [Users] SET LogInAttempts = (LogInAttempts + 1) WHERE Email = @Email"))
                                {

                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Email", email_box.Text.Trim());
                                        
                                    cmd.Connection = con;
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                    return attempts;

                                    
                                }
                            }
                            else
                            {
                                using (SqlCommand cmd = new SqlCommand("UPDATE [Users] SET LogInAttempts = LogInAttempts, LockedOut = @LockedOut, DateTimeLocked = @DateTimeLocked WHERE Email = @Email"))
                                {

                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Email", email_box.Text.Trim());
                                    cmd.Parameters.AddWithValue("@LockedOut", true);
                                    cmd.Parameters.AddWithValue("@DateTimeLocked", DateTime.Now);
                                    cmd.Connection = con;
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                    return attempts;


                                }
                            }
                            
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
        }

        protected int checkPasswordTime()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT CurrentPasswordDateTime FROM [Users] WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email_box.Text.Trim());
                            
                            
                            cmd.Connection = con;
                            con.Open();
                            var dateTimeLockedString = cmd.ExecuteScalar().ToString();
                            CultureInfo.CurrentCulture = new CultureInfo("en-SG");
                            DateTime dateTimeLocked = DateTime.Parse(dateTimeLockedString, CultureInfo.CurrentCulture);
                            System.Diagnostics.Debug.WriteLine("Date time Now : " + DateTime.Now);
                            TimeSpan ts = dateTimeLocked.Subtract(DateTime.Now);
                            var minutesLocked = System.Math.Abs(Convert.ToInt32(ts.TotalMinutes));
                            System.Diagnostics.Debug.WriteLine("Date time locked : " + dateTimeLocked);
                            System.Diagnostics.Debug.WriteLine("Time span : " + ts);
                            System.Diagnostics.Debug.WriteLine("Minutes Locked : " + minutesLocked);
                            var minutesLeft = 20 - minutesLocked;
                            System.Diagnostics.Debug.WriteLine("Minutes Left till need to reset : " + minutesLeft);
                            con.Close();
                            return minutesLeft;
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