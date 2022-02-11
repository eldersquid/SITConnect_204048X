using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_204048X
{
    public partial class EmailVerification : System.Web.UI.Page
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
                verificationCodeText.Text = checkCode();


            }
            else
            {if (checkVerificationTime() != null)
                {
                    int verificationTime;
                    if (Int32.TryParse(checkVerificationTime(), out verificationTime) == true)
                    {
                        if (Int32.Parse(checkVerificationTime()) <= 0)
                        {
                            changeVerification(Session["Email"].ToString());
                            Response.Redirect("Login.aspx", false);
                        }
                        else
                        {
                            error_twofac.Text = "Verification code expired. Sending a new one.";
                            
                            sendEmail(Session["Email"].ToString(),createVerificationCode());
                        }
                    }
                    else
                    {
                        error_twofac.Text = checkVerificationTime();
                    }
                }
            else
                {
                    
                }
                Response.Redirect("Login.aspx", false);
            }
        }


        //protected void sendEmail(string email, string code)
        //{
        //    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        //    mail.To.Add(email);
        //    mail.From = new MailAddress("miyawakipan@gmail.com", "Verification", System.Text.Encoding.UTF8);
        //    mail.Subject = "Verification Code for SITConnect";
        //    mail.SubjectEncoding = System.Text.Encoding.UTF8;

        //    mail.Body = "Verification code is : " + code;
        //    mail.BodyEncoding = System.Text.Encoding.UTF8;
        //    mail.IsBodyHtml = true;
        //    mail.Priority = MailPriority.High;
        //    SmtpClient client = new SmtpClient();
        //    client.Credentials = new System.Net.NetworkCredential("miyawakipan@gmail.com", "uekaramariko48");
        //    client.Port = 587;
        //    client.Host = "smtp.gmail.com";
        //    client.EnableSsl = true;
        //    try
        //    {
        //        client.Send(mail);

        //    }
        //    catch (Exception ex)
        //    {
        //        Exception ex2 = ex;
        //        string errorMessage = string.Empty;
        //        while (ex2 != null)
        //        {
        //            errorMessage += ex2.ToString();
        //            ex2 = ex2.InnerException;
        //        }

        //    }
        //}


        protected string checkCode()
        {
            try
            {
                string h = null;
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT VerificationCode FROM [Users] WHERE Email = @Email AND Active = @Active ORDER BY DateTimeCreated DESC"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", Session["Email"].ToString());
                            cmd.Parameters.AddWithValue("@Active", true);
                            cmd.Connection = con;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["VerificationCode"] != null)
                                    {
                                        if (reader["VerificationCode"] != DBNull.Value)
                                        {
                                            h = reader["VerificationCode"].ToString();
                                        }
                                    }
                                }
                            }
                            
                            con.Close();
                            return h;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
        }

        protected string createVerificationCode()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [Verifications] VALUES(@Email,@VerificationCode,@DateTimeCreated,@Active)"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            Random rand = new Random();
                            string code = Convert.ToString(rand.Next(10000, 99999));
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", Session["Email"].ToString());
                            cmd.Parameters.AddWithValue("@VerificationCode", code);
                            cmd.Parameters.AddWithValue("@DateTimeCreated", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Active", 1);


                            cmd.Connection = con;

                            con.Open();
                            cmd.ExecuteNonQuery();

                            con.Close();
                            return code;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
        }

        protected string checkVerificationTime()
        {
            try
            {
                string h = null;
                string secondsLeft = null;
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT DateTimeCreated FROM [Users] WHERE Email = @Email AND Active = @Active ORDER BY DateTimeCreated DESC"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", Session["Email"].ToString());
                            cmd.Parameters.AddWithValue("@Active", true);
                            cmd.Connection = con;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["DateTimeCreated"] != null)
                                    {
                                        if (reader["DateTimeCreated"] != DBNull.Value)
                                        {
                                            h = reader["DateTimeCreated"].ToString();
                                            DateTime dateTimeLocked = DateTime.Parse(h, System.Globalization.CultureInfo.InvariantCulture);
                                            TimeSpan ts = dateTimeLocked.Subtract(DateTime.Now);
                                            var secondsLocked = System.Math.Abs(Convert.ToInt32(ts.TotalSeconds));
                                            secondsLeft = (120 - secondsLocked).ToString();
                                            
                                        }
                                    }
                                }
                            }

                            con.Close();
                            return secondsLeft;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());

            }
        }

        protected void enable_button_Click(object sender, EventArgs e)
        {
            var email = Session["Email"].ToString();
            var enteredCode = HttpUtility.HtmlEncode(inputCode.Text);
            var verificationCode = checkCode();
            if (verificationCode == enteredCode)
            {
                changeVerification(email);

            }
            

        }

        protected void changeVerification(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [Users] SET Verified=@Verified WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Verified", true);
                            SqlCommand cmd2 = new SqlCommand("UPDATE [Verifications] SET Active=@Active WHERE Email = @Email ORDER BY DateTimeCreated DESC");
                            cmd.Parameters.AddWithValue("@Active", 0);
                            cmd.Connection = con;
                            cmd2.Connection = con;

                            con.Open();
                            cmd.ExecuteScalar();
                            cmd2.ExecuteScalar();
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


        protected void expireVerification(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [Verifications] SET Active=@Active WHERE Email = @Email "))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@Verified", true);
                            SqlCommand cmd2 = new SqlCommand("UPDATE [Verifications] SET Active=@Active WHERE Email = @Email ORDER BY DateTimeCreated DESC");
                            cmd.Parameters.AddWithValue("@Active", 0);
                            cmd.Connection = con;
                            cmd2.Connection = con;
                            con.Open();
                            cmd.ExecuteScalar();
                            cmd2.ExecuteScalar();
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