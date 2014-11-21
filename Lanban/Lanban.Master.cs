using System;

namespace Lanban
{
    public partial class Lanban : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["uname"] = "Minh Son Nguyen";
            Session["avatar"] = "/Uploads/User/_79G2227.jpg";
            profile.ToolTip = Session["uname"].ToString();
            profile.ImageUrl = Session["avatar"].ToString();
        }

        protected void btnLogout_ServerClick(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}