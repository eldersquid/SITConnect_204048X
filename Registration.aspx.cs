using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;

namespace SITConnect_204048X
{
    public partial class Registration : System.Web.UI.Page
    {
        string SITDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {
            CompareValidatordob.ValueToCompare = DateTime.Today.ToShortDateString();
            CompareValidatordob.ErrorMessage = "Date of Birth cannot be today or after " + DateTime.Today.ToString("dd/MM/yyyy").ToString() + "!!";
            
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

        private bool checkCreditCard(string creditCard)
        {

            if (Regex.IsMatch(creditCard, @"^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool checkImage()
        {
            string folderPath = Server.MapPath("~/Photos/");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string imgName = photo_box.FileName;
            string Image = "~/Photos/" + imgName.ToString();
            int imgSize = photo_box.PostedFile.ContentLength;
            string ext = System.IO.Path.GetExtension(photo_box.PostedFile.FileName);
            if (photo_box.PostedFile!=null && photo_box.PostedFile.FileName != "")
            {
                if (imgSize > 5242880)
                {
                    error_captcha.Text = "Image is too big.";
                    error_captcha.ForeColor = System.Drawing.Color.Red;
                    return false;
                }
                if (ext.ToLower().Trim() != ".jpg" && ext.ToLower() != ".png" && ext.ToLower() != ".jpeg")
                {
                    error_captcha.Text = "Only .jpg, .jpeg or .png file allowed.";
                    error_captcha.ForeColor = System.Drawing.Color.Red;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                error_captcha.Text = "Image not uploaded.";
                error_captcha.ForeColor = System.Drawing.Color.Red;
                return false;
            }

        }




        protected bool checkPW()
        {
            //Extract data from textbox
            int scores = checkPassword(HttpUtility.HtmlEncode(pw_box.Text));
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



        protected void signup_button_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (verificationCode_box.Text.ToLower() == Session["CaptchaVerify"].ToString())
                {
                    string pwd = HttpUtility.HtmlEncode(pw_box.Text.ToString().Trim());
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
                    checkPW();
                    if (checkDupes() == 0)
                    {
                        if (checkPW() == true)
                        {
                            if (checkEmail(HttpUtility.HtmlEncode(email_box.Text)) == true)
                            {
                                if (checkCreditCard(HttpUtility.HtmlEncode(ccard_box.Text)) == true)
                                {
                                    if (checkImage() == true)
                                    {
                                        Random rand = new Random();
                                        string code = Convert.ToString(rand.Next(10000, 99999));
                                        createAccount(code);
                                        Session["Email"] = email_box.Text;
                                        Session["Password"] = pw_box.Text;
                                        Session["RegistrationID"] = Request.Cookies["ASP.NET_SessionId"].Value.ToString();
                                        //sendEmail(email_box.Text,code);
                                        Response.Redirect("TwoFactorAuthentication.aspx", false);
                                    }
                                    else
                                    {
                                        error_captcha.Text = "Image uploaded is not valid. Try again.";
                                        error_captcha.ForeColor = System.Drawing.Color.Red;
                                    }
                                    
                                    
                                }
                                else
                                {
                                    error_captcha.Text = "Credit card number is not valid. Try again.";
                                    error_captcha.ForeColor = System.Drawing.Color.Red;
                                }
                                
                            }
                            else
                            {
                                error_captcha.Text = "Email is not valid. Try again.";
                                error_captcha.ForeColor = System.Drawing.Color.Red;
                            }
                            
                        }
                        else
                        {
                            error_captcha.Text = "Password is not strong enough. Try again.";
                            error_captcha.ForeColor = System.Drawing.Color.Red;
                        }
                        
                    }
                    else
                    {
                        error_captcha.Text = "Duplicate email found. Please use a different email.";
                        error_captcha.ForeColor = System.Drawing.Color.Red;
                    }

                }
                else
                {
                    error_captcha.Text = "You have entered the wrong captcha. Please enter the right captcha.";
                    error_captcha.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                error_captcha.Text = "Error in fields! Check!!";
                error_captcha.ForeColor = System.Drawing.Color.Red;
            }
            

        }
        protected void createAccount(string code)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [Users] VALUES(@Email,@FirstName, @LastName, @CurrentPasswordHash,@DateOfBirth,@PhotoURL,@CreditInfo,@CurrentPasswordSalt,@CurrentPasswordDateTime,@LockedOut,@LogInAttempts,@DateTimeLocked,@TwoFactorEnabled,@IV,@Key,@Verified)"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            string folderPath = Server.MapPath("~/Photos/");

                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }
                            string imgName = photo_box.FileName;
                            photo_box.PostedFile.SaveAs(Server.MapPath("~/Photos/" + imgName));
                            string Image = "~/Photos/" + imgName.ToString();
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(email_box.Text.ToString()));
                            cmd.Parameters.AddWithValue("@FirstName", HttpUtility.HtmlEncode(fname_box.Text.Trim()));
                            cmd.Parameters.AddWithValue("@LastName", HttpUtility.HtmlEncode(lname_box.Text.Trim()));
                            cmd.Parameters.AddWithValue("@CurrentPasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@DateOfBirth", HttpUtility.HtmlEncode(dob_box.Text.ToString()));
                            cmd.Parameters.AddWithValue("@CurrentPasswordDateTime", DateTime.Now);
                            //cmd.Parameters.AddWithValue("@EmailVerified", DBNull.Value);
                            cmd.Parameters.AddWithValue("@PhotoURL", Image);
                            cmd.Parameters.AddWithValue("@CreditInfo", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(ccard_box.Text.Trim()))));
                            cmd.Parameters.AddWithValue("@CurrentPasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@LockedOut", 0);
                            cmd.Parameters.AddWithValue("@LogInAttempts", 0);
                            cmd.Parameters.AddWithValue("@DateTimeLocked", DBNull.Value);
                            cmd.Parameters.AddWithValue("@TwoFactorEnabled", 0);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@Verified", 0);
                            SqlCommand cmd2 = new SqlCommand("INSERT INTO [Passwords] VALUES(@Email,@Password1Hash,@Password2Hash,@Password3Hash,@Password1Salt,@Password2Salt,@Password3Salt,@Password1DateTime,@Password2DateTime,@Password3DateTime)");
                            cmd2.CommandType = CommandType.Text;
                            cmd2.Parameters.AddWithValue("@Email", email_box.Text.Trim());
                            cmd2.Parameters.AddWithValue("@Password1Hash", finalHash);
                            cmd2.Parameters.AddWithValue("@Password2Hash", DBNull.Value);
                            cmd2.Parameters.AddWithValue("@Password3Hash", DBNull.Value);
                            cmd2.Parameters.AddWithValue("@Password1Salt", salt);
                            cmd2.Parameters.AddWithValue("@Password2Salt", DBNull.Value);
                            cmd2.Parameters.AddWithValue("@Password3Salt", DBNull.Value);
                            cmd2.Parameters.AddWithValue("@Password1DateTime", DateTime.Now);
                            cmd2.Parameters.AddWithValue("@Password2DateTime", DBNull.Value);
                            cmd2.Parameters.AddWithValue("@Password3DateTime", DBNull.Value);

                            
                            

                            cmd.Connection = con;
                            cmd2.Connection = con;
                            
                            con.Open();
                            cmd.ExecuteNonQuery();
                            cmd2.ExecuteNonQuery();
                            
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

        protected void sendEmail(string email, string code)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress("miyawakipan@gmail.com", "Verification", System.Text.Encoding.UTF8);
            mail.Subject = "Verification Code for SITConnect";
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            
            mail.Body = "Verification code is : "+ code;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("miyawakipan@gmail.com", "uekaramariko48");
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            try
            {
                client.Send(mail);
                
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                string errorMessage = string.Empty;
                while (ex2 != null)
                {
                    errorMessage += ex2.ToString();
                    ex2 = ex2.InnerException;
                }
               
            }
        }

        protected int checkDupes()
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





        protected void createVerificationCode(string code)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO [Verifications] VALUES(@Email,@VerificationCode,@DateTimeCreated,@Active)"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email_box.Text.Trim());
                            cmd.Parameters.AddWithValue("@VerificationCode", code);
                            cmd.Parameters.AddWithValue("@DateTimeCreated", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Active", 1);


                            cmd.Connection = con;
                            
                            con.Open();
                            cmd.ExecuteNonQuery();
                            
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