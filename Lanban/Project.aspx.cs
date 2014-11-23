using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Lanban
{
    public partial class Project : System.Web.UI.Page
    {
        StringBuilder lists;
        protected async void Page_Load(object sender, EventArgs e) 
        {
            if (!IsPostBack)
            {
                //int userID = Convert.ToInt32(Session["userID"]);
                //int role = Convert.ToInt32(Session["userRole"]);
                int userID = 1;
                int role = 1;
                Session["userID"] = userID;
                lists = new StringBuilder("var userID=" + userID + ";");

                var timer = System.Diagnostics.Stopwatch.StartNew();
                await loadProject(userID, role);
                await Task.Run(() => loadUser(userID));
                timer.Stop();
                System.Diagnostics.Debug.WriteLine(timer.ElapsedMilliseconds.ToString());
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "projectScript", lists.ToString(), true);
            }
            else
            {
                if (Request.Params["__EVENTTARGET"].Equals("RedirectBoard"))
                    await Task.Run(() => gotoProject(Request.Params["__EVENTARGUMENT"]));
            }
        }

        //1. Load all projects
        protected async Task loadProject(int userID, int role)
        {
            // Fetch all projects belong to or under supervised of this user
            Query myQuery = new Query();
            myQuery.fetchProject(userID, role);
            var project = myQuery.MyDataSet.Tables["Project"];
            await Task.Run(() =>
            {
                for (int i = 0; i < project.Rows.Count; i++)
                {
                    var row = project.Rows[i];
                    projectbrowser.Controls.Add(ProjectBox(row));
                }
            });
            
            // Send project list in JSON to client for further processing
            await Task.Run(() =>
            {
                StringBuilder projectList = new StringBuilder("projectList = ");
                projectList.Append(JsonConvert.SerializeObject(project));
                projectList.Append(";");
                lists.Append(projectList);
            });
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

        // 2. Load users who share the same projects
        private void loadUser(int userID)
        {
            // Fetch data from database
            Query myQuery = new Query();
            myQuery.fetchSharedProjectUser(userID);
            var user = myQuery.MyDataSet.Tables["User"];

            // Send user list in JSON to client for further processing
            StringBuilder userList = new StringBuilder("userList = ");
            userList.Append(JsonConvert.SerializeObject(user));
            userList.Append(";");
            lists.Append(userList);
            myQuery.Dipose();
        }
    }
}