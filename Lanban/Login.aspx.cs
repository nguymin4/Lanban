using System;
using System.Web.Security;

namespace Lanban
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtRUsername.Attributes["onkeyup"] = "checkName()";
            if (IsPostBack)
            {
                if (Request.Params["__EVENTTARGET"].Equals("Register")) {
                    btnRRegister_Click();
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var query = new Query();
            var user = query.login(txtUsername.Text, txtPassword.Text);
            query.Dipose();

            if (user != null)
            {
                if ((user.Avatar == null) || (user.Avatar.Equals("")))
                    user.Avatar = "images/sidebar/profile.png";
                Session["user"] = user;
                Session["userID"] = user.User_ID;
                
                // Create authentication ticket
                var auth = new Controller.LanbanAuthentication();
                auth.Authenticate(Response, user);
                FormsAuthentication.RedirectFromLoginPage(txtUsername.Text, false);
            }
            else
            {
                btnLogin.Style["margin-top"] = "10px";
                btnLRegister.Style["margin-top"] = "10px";
                lblMsg.Style["display"] = "block";
                lblMsg.Text = "Wrong Username and/or Password";
            }
        }

        protected void btnRRegister_Click()
        {
        }
    }
}