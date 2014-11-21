using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Data;
using Newtonsoft.Json;

namespace Lanban
{
    public partial class Project : System.Web.UI.Page
    {
        Query myQuery;
        StringBuilder projectList;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                myQuery = new Query();
                int userID;
                int role;
                //userID = Convert.ToInt32(Session["userID"]);
                //role = Convert.ToInt32(Session["userRole"]);
                userID = 1;
                role = 1;
                Session["userID"] = userID;
                loadProject(userID, role);
            }
            else
            {
                if (Request.Params["__EVENTTARGET"].Equals("RedirectBoard"))
                    gotoProject(Request.Params["__EVENTARGUMENT"]);
            }
        }

        //1. Load all projects
        protected void loadProject(int userID, int role)
        {
            projectList = new StringBuilder();
            // Fetch all projects belong to or under supervised of this user
            myQuery.fetchProject(userID, role);
            var project = myQuery.MyDataSet.Tables["Project"];
            for (int i = 0; i < project.Rows.Count; i++)
            {
                var row = project.Rows[i];
                projectbrowser.Controls.Add(ProjectBox(row));
            }

            // Send project list in JSON to client for further processing
            projectList.Append(JsonConvert.SerializeObject(project));
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "projectList = " + projectList.ToString(), true);
        }

        // 1.1 Display each project in box
        protected HtmlGenericControl ProjectBox(DataRow row)
        {
            string id = row["Project_ID"].ToString();
            string name = row["Name"].ToString();
            
            // Container
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = "project"+id;
            div.Attributes.Add("class", "project-container");
            div.Attributes.Add("onclick", "viewProjectDetail(this, " + id + ")");

            // Header
            HtmlGenericControl header = new HtmlGenericControl("div");
            header.Attributes.Add("class", "project-header");
            header.InnerHtml = id+". "+name;
            div.Controls.Add(header);

            // Thumbnail
            HtmlGenericControl thumbnail = new HtmlGenericControl("div");
            thumbnail.Attributes.Add("class", "project-thumbnail");
            thumbnail.Style["background-image"] = "url('/Uploads/Project_" + id + "/screenshot.jpg')";
            div.Controls.Add(thumbnail);

            return div;
        }

        // 1.2 Event handler of clicking on a project box
        public void gotoProject(string data)
        {
            string[] info = data.Split('$');
            Session["projectID"] = info[0];
            Session["projectName"] = info[1];
            Response.Redirect("Board.aspx");
        }
    }
}