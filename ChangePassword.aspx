<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="SITConnect_204048X.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Change Password Page</title>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script type="text/javascript">
        
       
        function validate() {
            var pw = document.getElementById('<%=newPW_box.ClientID %>').value;
            if (pw.length < 12) {
                document.getElementById("pwchecker_label").innerHTML = "Client-Side Validation : Password must be at least 12 characters!";
                document.getElementById("pwchecker_label").style.color = "Red";
                return ("too_short");
            }
            else if (pw.search(/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/) == -1) {
                document.getElementById("pwchecker_label").innerHTML = "Client-Side Validation : Password is too weak!";
                document.getElementById("pwchecker_label").style.color = "Red";
                return ("too_weak");
            }

            document.getElementById("pwchecker_label").innerHTML = "Client-Side Validation : Password is strong!";
            document.getElementById("pwchecker_label").style.color = "Blue";

        }



    </script>
            <script src="https://www.google.com/recaptcha/api.js?render=6Legu90dAAAAAOI3ds4zqw0kpy4yT0-mI2dsz4Zn"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous">
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p" crossorigin="anonymous"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
</head>
<body>
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
    <div class="container-fluid">
        <a href="#" class="navbar-brand">SITConnect</a>
        <button type="button" class="navbar-toggler" data-bs-toggle="collapse" data-bs-target="#navbarCollapse">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarCollapse">
            <div class="navbar-nav">
                <a href="Profile.aspx" class="nav-item nav-link">Profile</a>
                <a href="Profile.aspx" class="nav-item nav-link">Change 2FA</a>
                <a href="ChangePassword.aspx" class="nav-item nav-link">Change Password</a>
            </div>
            <div class="navbar-nav ms-auto">
                <a href="Login.aspx" class="nav-item nav-link">Sign Out</a>
            </div>
        </div>
    </div>
</nav>

    <form id="form1" runat="server">
        
        
            <fieldset>
                <legend>Change Password</legend>
                <br />
            <br />
            <asp:Label ID="oldPW_label" runat="server" Text="Old Password"></asp:Label>
            <br />
            <br />
            <asp:TextBox ID="oldPW_box" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatoroldPW" Display="Dynamic" runat="server" ForeColor="Red" ErrorMessage="Previous password is wrongly entered." ControlToValidate="oldPW_box" SetFocusOnError="True">*</asp:RequiredFieldValidator>
            <br />
            <br />
            <asp:Label ID="newPW_label" runat="server" Text="New Password"></asp:Label>
            <br />
            <br />
            <asp:TextBox ID="newPW_box" onkeyup="javascript:validate()" runat="server" TextMode="Password"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidatorpw" Display="Dynamic" runat="server" ForeColor="Red" ErrorMessage="Password is required." ControlToValidate="newPW_box" SetFocusOnError="True">*</asp:RequiredFieldValidator>
            <asp:Label ID="pwchecker_label" runat="server" Text=""></asp:Label>
            <br />
            <br />
            <asp:Label ID="verify_label" runat="server" Text="Verification Code"></asp:Label>
            <asp:Image ID="Image1" runat="server" Height="55px" ImageUrl="~/Captcha.aspx" Width="186px" />
            <br />
            <br />

            <asp:Label ID="enter_verify_label" runat="server" Text="Enter Verification Code "></asp:Label>
            <asp:TextBox ID="verificationCode_box" runat="server"></asp:TextBox>
            
            <br />
            <br />
            <asp:Label ID="error_captcha" runat="server" Text=""></asp:Label>
            <br />
            <br />
            <asp:Label ID="error_label" runat="server" Text="" ForeColor="Red"></asp:Label>
                <br />
            <br />
            <asp:ValidationSummary ID="ValidationSummary1" HeaderText="Errors : " ForeColor="Red" runat="server" />
            <br />
            <br />
            <asp:Button ID="changePW_button" runat="server" Text="Change Password" OnClick="changePW_button_Click"  />




            </fieldset>
            
          
        
    </form>
</body>
            <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.10.2/dist/umd/popper.min.js" integrity="sha384-7+zCNj/IqJ95wo16oMtfsKbZ9ccEh31eOz1HGyDuCQ6wgnyJNSYdrPa03rtR1zdB" crossorigin="anonymous"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.min.js" integrity="sha384-QJHtvGhmr9XOIpI6YVutG+2QOK9T+ZnN4kzFN1RtK3zEFEIsxhlmWl5/YESvpZ13" crossorigin="anonymous"></script>

</html>
