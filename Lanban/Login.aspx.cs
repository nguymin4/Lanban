using System;

namespace Lanban
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            System.Data.SqlClient.SqlDataReader reader = new Query().login(txtUsername.Text, txtPassword.Text);
            if (reader != null)
            {
                Session["userID"] = reader["User_ID"];
                Session["userRole"] = reader["Role"];
                Session["uname"] = reader["Name"];
                if ((reader["avatar"] == null)||(reader["avatar"].ToString().Equals("")))
                    Session["avatar"] = "images/sidebar/profile.png";
                else Session["avatar"] = reader["Avatar"];
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