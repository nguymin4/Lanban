using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Data;

namespace Lanban
{
    public partial class Project : System.Web.UI.Page
    {
        Query myQuery;
        int userID;
        int role;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                myQuery = new Query();
                //userID = Convert.ToInt32(Session["userID"]);
                //role = Convert.ToInt32(Session["userRole"]);
                userID = 1;
                role = 1;
                loadProject();
            }
            else
            {
                if (Request.Params["__EVENTTARGET"].Equals("RedirectBoard"))
                    gotoProject(Request.Params["__EVENTARGUMENT"]);
            }
        }

        //1. Load all projects
        protected void loadProject()
        {
            // Fetch all projects belong to or under supervised of this user
            myQuery.fetchProject(userID, role);
            var project = myQuery.MyDataSet.Tables["Project"];

            for (int i = 0; i < project.Rows.Count; i++)
            {
                var row = project.Rows[i];
                projectbrowser.Controls.Add(ProjectBox(row));
            }
        }

        //1.1 Display each project in box
        protected HtmlGenericControl ProjectBox(DataRow row)
        {
            string id = row["Project_ID"].ToString();
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = id;
            div.Attributes.Add("class", "projectContainer");
            div.Attributes.Add("onclick", "__doPostBack('RedirectBoard', " + id + ");");
            div.InnerHtml = "<strong>" + row["Name"].ToString() + "</strong>";
            return div;
        }

        // Event handler of clicking on a project box
        public void gotoProject(string projectID)
        {
            Session["project_ID"] = projectID;
            Response.Redirect("Board.aspx");
        }
    }
}