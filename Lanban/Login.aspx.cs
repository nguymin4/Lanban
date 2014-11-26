using System;
using System.Web;
using System.Web.Security;

namespace Lanban
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Model.UserModel user = new Query().login(txtUsername.Text, txtPassword.Text);
            if (user != null)
            {
                if ((user.Avatar == null) || (user.Avatar.Equals("")))
                    user.Avatar = "images/sidebar/profile.png";
                Session["user"] = user;
                Session["userID"] = user.User_ID;
                
                // Create authentication ticket
                var auth = new Controller.LanbanAuthentication();
                auth.Authenticate(Response, user);
                FormsAuthentication.RedirectFromLoginPage(user.Username, false);
            }
            else
            {
                btnLogin.Style["margin-top"] = "15px";
                lblMsg.Style["display"] = "block";
                lblMsg.Text = "Wrong Username and/or Password";
            }
        }

    }
}