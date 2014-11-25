using System;

namespace Lanban
{
    public partial class Lanban : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Model.UserModel user = (Model.UserModel) Session["user"];
            Model.UserModel user = new Query().login("nguymin4", "Lanban2014");
            profile.ToolTip = user.Name;
            profile.ImageUrl = user.Avatar;
        }

        protected void btnLogout_ServerClick(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}