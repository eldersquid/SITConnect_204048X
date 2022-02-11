<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect_204048X.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6Legu90dAAAAAOI3ds4zqw0kpy4yT0-mI2dsz4Zn"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous">
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p" crossorigin="anonymous"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script>
    grecaptcha.ready(function () {
        grecaptcha.execute('6Legu90dAAAAAOI3ds4zqw0kpy4yT0-mI2dsz4Zn', { action: 'Login' }).then(function (token) {
        document.getElementById("g-recaptcha-response").value = token;
        });
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

body {
  margin: 0;
  padding: 0;
  background-color: #17a2b8;
  height: 100vh;
}
#login .container #login-row #login-column #login-box {
  margin-top: 120px;
  max-width: 600px;
  height: 320px;
  border: 1px solid #9C9C9C;
  background-color: #EAEAEA;
}
#login .container #login-row #login-column #login-box #login-form {
  padding: 20px;
}
#login .container #login-row #login-column #login-box #login-form #register-link {
  margin-top: -85px;
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

        




    <div id="login">
        <div class="container">
            <div id="login-row" class="row justify-content-center align-items-center">
                <div id="login-column" class="col-md-6 text-center">
                    <div id="login-box" class="col-md-12">
                        <form id="form1" class="form" runat="server">
                            <h3 class="text-center text-info">Login</h3>
                            <div class="form-group">
                                 <asp:Label ID="email_label" class="text-info" runat="server" Text="Email : "></asp:Label>
                <asp:TextBox ID="email_box" runat="server" class="form-control "></asp:TextBox>
                                
                               
                            </div>
                            <div class="form-group">
                                  <asp:Label ID="password_label" class="text-info" runat="server" Text="Password : "></asp:Label>
                                <asp:TextBox ID="password_box" runat="server" class="form-control" TextMode="Password"></asp:TextBox>
                            </div>
                                            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>

                            <div class="form-group">


                                
                
                                <asp:Button ID="login_button" class="btn btn-info btn-md" runat="server" OnClick="loginMethod" Text="Login" />
                            </div>
                            <div class="form-group">
                                <asp:Label ID="error_label" runat="server" Text="" ForeColor="Red"></asp:Label>
                            </div>
                            
                            <div id="register-link" class="text-right">
                                <a href="Registration.aspx" class="text-info">Register here</a>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>

</body>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.10.2/dist/umd/popper.min.js" integrity="sha384-7+zCNj/IqJ95wo16oMtfsKbZ9ccEh31eOz1HGyDuCQ6wgnyJNSYdrPa03rtR1zdB" crossorigin="anonymous"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.min.js" integrity="sha384-QJHtvGhmr9XOIpI6YVutG+2QOK9T+ZnN4kzFN1RtK3zEFEIsxhlmWl5/YESvpZ13" crossorigin="anonymous"></script>

</html>
