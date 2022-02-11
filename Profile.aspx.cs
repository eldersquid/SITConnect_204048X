using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect_204048X
{
    public partial class Profile : System.Web.UI.Page
    {
        string SITDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            var email = Session["Email"];
            if (Session["Email"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"].Value != null && Session["PasswordExpired"] !=null)
            {
                if (Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    if (Session["PasswordExpired"] == "Yes")
                    {
                        Response.Redirect("ChangePassword.aspx", false);
                    }
                    else
                    {
                        retrieveUser(email.ToString());
                    }
                    
                }
                else
                {
                    Response.Redirect("Login.aspx", false);

                }

            }
            else
            {
                Response.Redirect("error403.aspx", false);
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
        protected void retrieveUser(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [Users] WHERE Email = @Email"))
                    {

                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", email);

                            cmd.Connection = con;
                            con.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    emailUser.InnerText = reader["Email"].ToString();
                                    dobUser.InnerText = reader["DateOfBirth"].ToString();
                                    nameUser.InnerText = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                                    IV = Convert.FromBase64String(reader["IV"].ToString());
                                    Key = Convert.FromBase64String(reader["Key"].ToString());
                                   
                                    creditcardUser.InnerText = decryptData(Convert.FromBase64String(reader["CreditInfo"].ToString()),IV,Key);
                                    photo.Src = reader["PhotoURL"].ToString();
                                    twofactorUser.InnerText = reader["TwoFactorEnabled"].ToString();
                                }
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


        protected string decryptData(byte[] cipherText, byte[] iv, byte[] key)
        {
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = iv;
                cipher.Key = key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally {
                System.Diagnostics.Debug.WriteLine("Plain text is " + plainText);
            }
            return plainText;
        }
    }
}