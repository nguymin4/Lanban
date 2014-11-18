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
        int userID;
        int role;
        StringBuilder projectList;

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
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "", "var projectList = " + projectList.ToString(), true);
        }

        // 1.1 Display each project in box
        protected HtmlGenericControl ProjectBox(DataRow row)
        {
            string id = row["Project_ID"].ToString();
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = "project"+id;
            div.Attributes.Add("class", "project-container");
            div.Attributes.Add("onclick", "__doPostBack('RedirectBoard', " + id + ");");

            // note-header
            HtmlGenericControl header = new HtmlGenericControl("div");
            header.Attributes.Add("class", "project-header");

            // Add edit button
            Image edit = new Image();
            edit.ImageUrl = "images/sidebar/setting.png";
            edit.CssClass = "note-button";
            edit.Attributes.Add("onclick", "viewProject(" +id+ ")");
            header.Controls.Add(edit);

            // Add delete button
            Image delete = new Image();
            delete.ImageUrl = "images/sidebar/delete_note.png";
            delete.CssClass = "note-button";
            delete.Attributes.Add("onclick", "deleteProject("+id+")");
            header.Controls.Add(delete);
            div.Controls.Add(header);

            // Add content div
            HtmlGenericControl content = new HtmlGenericControl("div");
            content.Attributes.Add("class", "project-content");
            content.InnerHtml = row["Name"].ToString();
            div.Controls.Add(content);

            // Add footer div
            HtmlGenericControl footer = new HtmlGenericControl("div");
            footer.Attributes.Add("class", "project-footer");
            footer.InnerHtml = "&nbsp;";
            div.Controls.Add(footer);

            return div;
        }

        // 1.2 Create JSON Object for project

        // 1.3 Event handler of clicking on a project box
        public void gotoProject(string projectID)
        {
            Session["project_ID"] = projectID;
            Response.Redirect("Board.aspx");
        }
    }
}