<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect_204048X.Registration" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SITConnect - Register</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous">
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p" crossorigin="anonymous"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script type="text/javascript">


        function validate() {
            var pw = document.getElementById('<%=pw_box.ClientID %>').value;
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
        $(function () {
            var today = new Date();
            var month = ('0' + (today.getMonth() + 1)).slice(-2);
            var day = ('0' + today.getDate()).slice(-2);
            var year = today.getFullYear();
            var date = year + '-' + month + '-' + day;
            $('[id*=dob_box]').attr('max', date);
        });



    </script>
    <style>
        .gradient-custom-3 {
  /* fallback for old browsers */
  background: #84fab0;

  /* Chrome 10-25, Safari 5.1-6 */
  background: -webkit-linear-gradient(to right, rgba(132, 250, 176, 0.5), rgba(143, 211, 244, 0.5));

  /* W3C, IE 10+/ Edge, Firefox 16+, Chrome 26+, Opera 12+, Safari 7+ */
  background: linear-gradient(to right, rgba(132, 250, 176, 0.5), rgba(143, 211, 244, 0.5))
}
.gradient-custom-4 {
  /* fallback for old browsers */
  background: #84fab0;

  /* Chrome 10-25, Safari 5.1-6 */
  background: -webkit-linear-gradient(to right, rgba(132, 250, 176, 1), rgba(143, 211, 244, 1));

  /* W3C, IE 10+/ Edge, Firefox 16+, Chrome 26+, Opera 12+, Safari 7+ */
  background: linear-gradient(to right, rgba(132, 250, 176, 1), rgba(143, 211, 244, 1))
}

html {
        font-size: 0.8vw;
}
    </style>
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
                
            </div>
            <div class="navbar-nav ms-auto">
                
                <a href="Registration.aspx" class="nav-item nav-link">Register</a>
                <a href="Login.aspx" class="nav-item nav-link">Login</a>
            </div>
        </div>
    </div>
</nav>
    <div>
        <section class="bg-image" style="background-image: url('https://mdbcdn.b-cdn.net/img/Photos/new-templates/search-box/img4.webp');">
            <div class="mask d-flex align-items-center h-100 gradient-custom-3">
    <div class="container h-100 mt-5">
      <div class="row d-flex justify-content-center align-items-center h-100">
        <div class="col-12 col-md-9 col-lg-7 col-xl-6">
          <div class="card" style="border-radius: 15px;">
            <div class="card-body p-5">
              <h2 class="text-uppercase text-center mb-5">Create an account</h2>

              <form id="form1" runat="server">

            <div class="form-outline ">

                            <asp:Label ID="fname_label" class="form-label" runat="server" Text="First Name"></asp:Label>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidatorfname" Display="Dynamic" runat="server" ForeColor="Red" ErrorMessage="First Name is required." ControlToValidate="fname_box" SetFocusOnError="True" ValidationGroup="Register">*</asp:RequiredFieldValidator>

            <asp:TextBox ID="fname_box" class="form-control form-control-lg" runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatorfname" ControlToValidate="fname_box" ForeColor="Red" runat="server" ErrorMessage="Invalid First Name" Visible="True" ValidationExpression="^[a-z -']+$" ValidationGroup="Register"></asp:RegularExpressionValidator>

                </div>

                  <div class="form-outline">
                                  <asp:Label ID="lname_label" class="form-label" runat="server" Text="Last Name"></asp:Label>
                                      <asp:RequiredFieldValidator ID="RequiredFieldValidatorlname" Display="Dynamic" runat="server" ForeColor="Red" ErrorMessage="Last Name is required." ControlToValidate="lname_box" SetFocusOnError="True" ValidationGroup="Register">*</asp:RequiredFieldValidator>

            <asp:TextBox ID="lname_box"  class="form-control form-control-lg"  runat="server"></asp:TextBox>
            <asp:RegularExpressionValidator ID="RegularExpressionValidatorlname" ControlToValidate="lname_box" ForeColor="Red" runat="server" ErrorMessage="Invalid Last Name" Visible="True" ValidationExpression="^[a-z -']+$" ValidationGroup="Register"></asp:RegularExpressionValidator>

                </div>

            <div class="form-outline ">
                  
            <asp:Label ID="email_label" class="form-label" runat="server" Text="Email address"></asp:Label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidatoremail" Display="Dynamic" runat="server" ForeColor="Red" ErrorMessage="Email is required." ControlToValidate="email_box" SetFocusOnError="True" ValidationGroup="Register">*</asp:RequiredFieldValidator>

            <asp:TextBox ID="email_box" class="form-control form-control-lg" runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatoremail" ControlToValidate="email_box" ForeColor="Red" runat="server" ErrorMessage="Invalid Email address" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Register"></asp:RegularExpressionValidator>

                </div>

                  <div class="form-outline ">
                              <asp:Label ID="pw_label" class="form-label"  runat="server" Text="Password"></asp:Label>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidatorpw" Display="Dynamic" runat="server" ForeColor="Red" ErrorMessage="Password is required." ControlToValidate="pw_box" SetFocusOnError="True" ValidationGroup="Register">*</asp:RequiredFieldValidator>
            <asp:TextBox ID="pw_box" onkeyup="javascript:validate()" class="form-control form-control-lg" runat="server" TextMode="Password"></asp:TextBox>
            <asp:Label ID="pwchecker_label" class="form-label"  runat="server" Text=""></asp:Label>
                
                </div>

                  <div class="form-outline mt-2 "> 
                                  <asp:Label ID="cfmpw_label" class="form-label"  runat="server" Text="Confirm Password"></asp:Label>

                <asp:TextBox ID="cfmpw_box" runat="server" class="form-control form-control-lg" TextMode="Password"></asp:TextBox><asp:CompareValidator ID="CompareValidatorpw" runat="server" Display="Dynamic" ForeColor="Red" ErrorMessage="Password must match!" ControlToValidate="cfmpw_box" ControlToCompare="pw_box" Operator="Equal" Type="String" SetFocusOnError="True" ValidationGroup="Register"></asp:CompareValidator>
   
                </div>

                  <div class="form-outline mt-2 ">

                              <asp:Label ID="dob_label" class="form-label" runat="server" Text="Date of Birth"></asp:Label>
                                      <asp:RequiredFieldValidator ID="RequiredFieldValidatordob" Display="Dynamic" runat="server" ForeColor="Red" ErrorMessage="Date of Birth is required." ControlToValidate="dob_box" SetFocusOnError="True" ValidationGroup="Register">*</asp:RequiredFieldValidator><asp:CompareValidator ID="CompareValidatordob" runat="server" Display="Dynamic" ForeColor="Red" ControlToValidate="dob_box" Operator="LessThanEqual" Type="Date" SetFocusOnError="True" ValidationGroup="Register">*</asp:CompareValidator>

            <asp:TextBox ID="dob_box" class="form-control form-control-lg" runat="server" TextMode="Date"></asp:TextBox>
                </div>

                  <div class="form-outline mt-2 ">
                       <asp:Label ID="photo_label" class="form-label" runat="server" Text="Photo"></asp:Label>
                                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="Dynamic" runat="server" ForeColor="Red" ErrorMessage="Photo is required." ControlToValidate="photo_box" SetFocusOnError="True" ValidationGroup="Register">*</asp:RequiredFieldValidator>

  
            <asp:FileUpload ID="photo_box" class="form-control form-control-lg" runat="server" />
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server"
                   ControlToValidate="photo_box"
                   ErrorMessage="Only .jpg .png or .jpeg files are allowed."
                   ForeColor="Red"
                    ValidationGroup="Register"
                   ValidationExpression="(.*?)\.(jpg|jpeg|png|JPG|JPEG|PNG)$"></asp:RegularExpressionValidator>
                </div>

                  <div class="form-outline ">
            <asp:Label ID="ccard_label" class="form-label" runat="server" Text="Credit Card Info"></asp:Label>
                                      <asp:RequiredFieldValidator ID="RequiredFieldValidatorccard" Display="Dynamic" runat="server" ForeColor="Red" ErrorMessage="Credit card info is required." ControlToValidate="ccard_box" SetFocusOnError="True" ValidationGroup="Register">*</asp:RequiredFieldValidator>

            <asp:TextBox ID="ccard_box" class="form-control form-control-lg" runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="ccard_box" ForeColor="Red" runat="server" ErrorMessage="Invalid Credit card number." ValidationExpression="^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$" ValidationGroup="Register"></asp:RegularExpressionValidator>

                </div>

                  <div class="form-outline ">
                                  <asp:Label ID="verify_label" class="form-label" runat="server" Text="Verification Code"></asp:Label>
            <asp:Image ID="Image1" runat="server" class="form-control form-control-lg"  Height="55px" ImageUrl="~/Captcha.aspx" Width="186px" />

                </div>

                  <div class="form-outline ">
                         <asp:Label ID="enter_verify_label" class="form-label" runat="server" Text="Enter Verification Code "></asp:Label>
            <asp:TextBox ID="verificationCode_box" class="form-control form-control-lg" runat="server"></asp:TextBox>
                </div>

                  <div class="form-outline ">
                                  <asp:Label ID="error_captcha" class="form-label" runat="server" Text=""></asp:Label>
                </div>

                  <div class="form-outline mb-4">
                      <asp:ValidationSummary ID="ValidationSummary1"  class="form-label" HeaderText="Errors : " ForeColor="Red" runat="server" ValidationGroup="Register" />
                </div>






                                  <div class="d-flex justify-content-center mb-4">
            <asp:Button ID="signup_button" runat="server" Text="Sign Up" class="btn btn-success btn-block btn-lg gradient-custom-4 text-body" OnClick="signup_button_Click" ValidationGroup="Register" />

                </div>

                <p class="text-center text-muted mt-5 mb-0">Have already an account? <a href="Login.aspx" class="fw-bold text-body"><u>Login here</u></a></p>



        
          
        
    </form>
                
                

                
            

            </div>
          </div>
        </div>
      </div>
    </div>
  </div>




           
        </section>

        


    </div>
    
</body>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.10.2/dist/umd/popper.min.js" integrity="sha384-7+zCNj/IqJ95wo16oMtfsKbZ9ccEh31eOz1HGyDuCQ6wgnyJNSYdrPa03rtR1zdB" crossorigin="anonymous"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.min.js" integrity="sha384-QJHtvGhmr9XOIpI6YVutG+2QOK9T+ZnN4kzFN1RtK3zEFEIsxhlmWl5/YESvpZ13" crossorigin="anonymous"></script>

</html>
