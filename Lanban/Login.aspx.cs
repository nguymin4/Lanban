using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lanban
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string result = new Query().login(txtUsername.Text, txtPassword.Text);
            if (result != "")
            {
                string[] data = result.Split('.');
                Session["userID"] = data[0];
                Session["userRole"] = data[1];
                Response.Redirect("Project.aspx");
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