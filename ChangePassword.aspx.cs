using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_204048X
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string SITDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Email"] != null && Request.Cookies["AuthToken"].Value != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    

                }

            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        private int checkPassword(string password)
        {
            int score = 0;
            //Score 1 weak
            if (password.Length < 12)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            //Score 2 weak
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            //Score 3 medium
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            //Score 4 strong
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            //Score 5 excellent
            if (Regex.IsMatch(password, "[^0-9a-zA-Z]"))
            {
                score++;
            }
            return score;
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

        protected bool checkPW()
        {
            //Extract data from textbox
            int scores = checkPassword(newPW_box.Text);
            string status = "";
            switch (scores)
            {

                case 1:
                    status = "Very Weak";

                    break;

                case 2:
                    status = "Weak";
                    break;

                case 3:
                    status = "Medium";
                    break;

                case 4:
                    status = "Strong";
                    break;

                case 5:
                    status = "Very Strong";
                    break;

                default:
                    break;
            }

            if (scores < 4)
            {

                return false;
            }
            else
            {
                return true;
            }

        }

        protected string deHashSaltPassword(string OldPassword, string OldPasswordSalt)
        {
            SHA512Managed hashing = new SHA512Managed();
            string pwdWithSalt = OldPassword + OldPasswordSalt;
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
            string userHash = Convert.ToBase64String(hashWithSalt);
            return userHash;
        }
     

        protected void changePW_button_Click(object sender, EventArgs e)
        {
            
            if (Session["Email"] !=null)
            {
                string dbHash = getDBHash(Session["Email"].ToString());
                string dbSalt = getDBSalt(Session["Email"].ToString());
                if (Page.IsValid)
                {
                    if (verificationCode_box.Text.ToLower() == Session["CaptchaVerify"].ToString())
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            //Hash and salt Current Password
                            string pwd = HttpUtility.HtmlEncode(newPW_box.Text.ToString().Trim());
                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];
                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);
                            SHA512Managed hashing = new SHA512Managed();
                            string pwdWithSalt = pwd + salt;
                            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            finalHash = Convert.ToBase64String(hashWithSalt);
                            RijndaelManaged cipher = new RijndaelManaged();
                            cipher.GenerateKey();
                            Key = cipher.Key;
                            IV = cipher.IV;


                            SHA512Managed hashing2 = new SHA512Managed();
                            string currentPWDWithSalt = HttpUtility.HtmlEncode(oldPW_box.Text.ToString().Trim()) + dbSalt;
                            byte[] currentHashWithSalt = hashing2.ComputeHash(Encoding.UTF8.GetBytes(currentPWDWithSalt));
                            string userHash = Convert.ToBase64String(currentHashWithSalt);
                            System.Diagnostics.Debug.WriteLine(finalHash);
                            System.Diagnostics.Debug.WriteLine(dbHash);
                            if (userHash.Equals(dbHash))
                            {

                                checkPW();
                                if (checkPW() == true)
                                {
                                    if (finalHash.Equals(dbHash))
                                    {
                                        error_label.Text = "Same password set.";
                                    }
                                    else
                                    {

                                        changePW(Session["Email"].ToString(), finalHash, salt, pwd);



                                    }



                                }
                                else
                                {
                                    error_label.Text = "Password is not strrong enough. Try again.";
                                    error_label.ForeColor = System.Drawing.Color.Red;
                                }
                            }
                            else
                            {
                                error_label.Text = "Wrong current password.";
                            }

                        }






                    }
                    else
                    {
                        error_label.Text = "You have entered the wrong captcha. Please enter the right captcha.";
                        error_label.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    error_label.Text = "Error in fields! Check!!";
                    error_label.ForeColor = System.Drawing.Color.Red;
                }


            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
            


        }

        protected void changePW(string email, string newPWHash, string newPWSalt, string newPW)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd2 = new SqlCommand("SELECT * FROM [Passwords] WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd2.CommandType = CommandType.Text;
                            
                            cmd2.Parameters.AddWithValue("@Email", email);

                            cmd2.Connection = con;
                            con.Open();
                                SqlDataReader reader = cmd2.ExecuteReader();
                                reader.Read();
                                System.Diagnostics.Debug.WriteLine(reader["Password1Hash"].ToString());
                                System.Diagnostics.Debug.WriteLine(reader["Password1Salt"].ToString());
                                System.Diagnostics.Debug.WriteLine(reader["Password1DateTime"].ToString());
                                List<DateTime> dtList = new List<DateTime>();
                                if (!(reader["Password1DateTime"] is DBNull))
                                {
                                    dtList.Add(Convert.ToDateTime(reader["Password1DateTime"]));
                                }
                                if (!(reader["Password2DateTime"] is DBNull))
                                {
                                    dtList.Add(Convert.ToDateTime(reader["Password2DateTime"]));
                                }
                                if (!(reader["Password3DateTime"] is DBNull))
                                {
                                    dtList.Add(Convert.ToDateTime(reader["Password3DateTime"]));
                                }
                                System.Diagnostics.Debug.WriteLine("Biggest date is "+ dtList.Max(p => p));
                                
                            var minutesLeft = System.Math.Abs(Convert.ToInt32(dtList.Max(p => p).Subtract(DateTime.Now).TotalMinutes));
                            System.Diagnostics.Debug.WriteLine("Minutes left is " + minutesLeft);
                            if ( minutesLeft <10)
                                {
                                    error_label.Text = "Changing password too fast. Must wait for " + (10-minutesLeft) + " minutes before next change.";
                                }
                                else
                                {
                                    List<String> hashedPWList = new List<String>();
                                    List<String> saltPWList = new List<String>();
                                    if (!(reader["Password1Hash"] is DBNull))
                                    {
                                        hashedPWList.Add(reader["Password1Hash"].ToString());
                                        saltPWList.Add(reader["Password1Salt"].ToString());
                                    }
                                    if (!(reader["Password2Hash"] is DBNull))
                                    {
                                        hashedPWList.Add(reader["Password2Hash"].ToString());
                                        saltPWList.Add(reader["Password2Salt"].ToString());
                                    }
                                    if (!(reader["Password3Hash"] is DBNull))
                                    {
                                        hashedPWList.Add(reader["Password3Hash"].ToString());
                                        saltPWList.Add(reader["Password3Salt"].ToString());
                                    }
                                    string hashResult = hashedPWList.FirstOrDefault(p => p == newPWHash);
                                    string saltResult = saltPWList.FirstOrDefault(p => p == newPWSalt);
                                con.Close();
                                System.Diagnostics.Debug.WriteLine("This is hashed password list");
                                System.Diagnostics.Debug.WriteLine(hashedPWList.ToString());
                                
                                if (hashResult == null)
                                    {
                                    //Do hash
                                    var count = 0;
                                        if (saltResult == null)
                                        {
                                            foreach(var salt in saltPWList)
                                        {
                                            
                                            var userHash = deHashSaltPassword(newPW, salt);
                                            foreach(var hash in hashedPWList)
                                                {
                                                    if (userHash.Equals(hash))
                                                    {
                                                        count++;
                                                    }
                                                }
                                            
                                            
                                            System.Diagnostics.Debug.WriteLine(count);

                                        }
                                            if (count > 0)
                                        {
                                            error_label.Text = "Password already used before. Try a new one.";
                                            error_label.ForeColor = System.Drawing.Color.Red;
                                        }
                                            else
                                        {
                                            //Do salt
                                            saltPWList.Add(newPWSalt);
                                            dtList.Add(DateTime.Now);
                                            if (saltPWList.Count > 3)
                                            {
                                                saltPWList.RemoveAt(0);
                                                dtList.RemoveAt(0);
                                            }
                                            hashedPWList.Add(newPWHash);
                                            if (hashedPWList.Count > 3)
                                            {
                                                hashedPWList.RemoveAt(0);
                                            }

                                            SqlCommand cmd3 = new SqlCommand("UPDATE [Users] SET CurrentPasswordHash = @CurrentPasswordHash, CurrentPasswordSalt = @CurrentPasswordSalt, CurrentPasswordDateTime = @CurrentPasswordDateTime WHERE Email = @Email");
                                            cmd3.CommandType = CommandType.Text;
                                            cmd3.Parameters.AddWithValue("@Email", email);
                                            cmd3.Parameters.AddWithValue("@CurrentPasswordHash", newPWHash);
                                            cmd3.Parameters.AddWithValue("@CurrentPasswordSalt", newPWSalt);
                                            cmd3.Parameters.AddWithValue("@CurrentPasswordDateTime", DateTime.Now);


                                            SqlCommand cmd4 = new SqlCommand("UPDATE [Passwords] SET Password1Hash = @Password1Hash, Password2Hash = @Password2Hash, Password3Hash = @Password3Hash, Password1Salt = @Password1Salt, Password2Salt = @Password2Salt, Password3Salt = @Password3Salt, Password1DateTime = @Password1DateTime, Password2DateTime = @Password2DateTime, Password3DateTime = @Password3DateTime WHERE Email = @Email");

                                            cmd4.CommandType = CommandType.Text;
                                            cmd4.Parameters.AddWithValue("@Email", email);
                                            switch (hashedPWList.Count)
                                            {
                                                case 2:
                                                    cmd4.Parameters.AddWithValue("@Password1Hash", hashedPWList[0]);
                                                    cmd4.Parameters.AddWithValue("@Password2Hash", hashedPWList[1]);
                                                    cmd4.Parameters.AddWithValue("@Password3Hash", DBNull.Value);
                                                    cmd4.Parameters.AddWithValue("@Password1Salt", saltPWList[0]);
                                                    cmd4.Parameters.AddWithValue("@Password2Salt", saltPWList[1]);
                                                    cmd4.Parameters.AddWithValue("@Password3Salt", DBNull.Value);
                                                    cmd4.Parameters.AddWithValue("@Password1DateTime", dtList[0]);
                                                    cmd4.Parameters.AddWithValue("@Password2DateTime", dtList[1]);
                                                    cmd4.Parameters.AddWithValue("@Password3DateTime", DBNull.Value);
                                                    break;
                                                case 3:
                                                    cmd4.Parameters.AddWithValue("@Password1Hash", hashedPWList[0]);
                                                    cmd4.Parameters.AddWithValue("@Password2Hash", hashedPWList[1]);
                                                    cmd4.Parameters.AddWithValue("@Password3Hash", hashedPWList[2]);
                                                    cmd4.Parameters.AddWithValue("@Password1Salt", saltPWList[0]);
                                                    cmd4.Parameters.AddWithValue("@Password2Salt", saltPWList[1]);
                                                    cmd4.Parameters.AddWithValue("@Password3Salt", saltPWList[2]);
                                                    cmd4.Parameters.AddWithValue("@Password1DateTime", dtList[0]);
                                                    cmd4.Parameters.AddWithValue("@Password2DateTime", dtList[1]);
                                                    cmd4.Parameters.AddWithValue("@Password3DateTime", dtList[2]);
                                                    break;
                                            }
                                            con.Open();
                                            cmd3.Connection = con;
                                            cmd4.Connection = con;
                                            cmd3.ExecuteNonQuery();
                                            cmd4.ExecuteNonQuery();
                                            con.Close();
                                            Session["PasswordExpired"] = "No";
                                            Response.Redirect("Profile.aspx", false);
                                        }
                                            


                                    }

                                        


                                    }
                                    else
                                    {
                                        error_label.Text = "Similar password set.";
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

    }
}