using Lanban.AccessLayer;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Lanban
{
    public partial class Project : System.Web.UI.Page
    {
        StringBuilder lists;
        Model.UserModel user;

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try { await InitProject(); }
                catch (Exception) { }
            }
            else
            {
                if (Request.Params["__EVENTTARGET"].Equals("RedirectBoard"))
                    await Task.Run(() => gotoProject(Request.Params["__EVENTARGUMENT"]));
            }
        }

        protected async Task InitProject()
        {
            // Login - temporary for fast testing
            if (Session["user"] == null)
            {
                Query query = new Query();
                user = query.login("luosimin", "iloveyou");
                Session["user"] = user;
                Session["userID"] = user.User_ID;
                query.Dipose();
            }
            else
            {
                user = (Model.UserModel) Session["user"];
            }

            // Init variables
            int userID = user.User_ID;
            int role = user.Role;
            lists = new StringBuilder("const userID=" + userID + ";");

            // Start tasks
            var timer = System.Diagnostics.Stopwatch.StartNew();
            Task[] task = new Task[2];
            task[0] = Task.Run(() => loadProject(userID, role));
            task[1] = Task.Run(() => loadUser(userID));

            // Profile area
            var profile = (Image)Master.FindControl("profile");
            profile.ToolTip = user.Name;
            profile.ImageUrl = user.Avatar;
            new Controller.LanbanAuthentication().Authenticate(Response, user);

            // Wait all tasks to be completed
            await Task.WhenAll(task);
            timer.Stop();
            System.Diagnostics.Debug.WriteLine(timer.ElapsedMilliseconds.ToString());
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "projectScript", lists.ToString(), true);
        }

        //1. Load all projects
        protected async Task loadProject(int userID, int role)
        {
            // Fetch all projects belong to or under supervised of this user
            ProjectAccess myAccess = new ProjectAccess();
            myAccess.fetchProject(userID, role);
            var project = myAccess.MyDataSet.Tables["Project"];

            Task task1 = Task.Run(() =>
            {
                for (int i = 0; i < project.Rows.Count; i++)
                {
                    var row = project.Rows[i];
                    projectbrowser.Controls.Add(ProjectBox(row));
                }
            });

            // Send project list in JSON to client for further processing
            Task task2 = Task.Run(() =>
            {
                StringBuilder projectList = new StringBuilder("projectList = ");
                projectList.Append(JsonConvert.SerializeObject(project));
                projectList.Append(";");
                lists.Append(projectList);
            });

            await Task.WhenAll(task1, task2);
            myAccess.Dipose();
        }

        // 1.1 Display each project in box
        protected HtmlGenericControl ProjectBox(DataRow row)
        {
            string id = row["Project_ID"].ToString();
            string name = row["Name"].ToString();

            // Container
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = "project" + id;
            div.Attributes.Add("class", "project-container");
            div.Attributes.Add("onclick", "viewProjectDetail(this, " + id + ")");

            // Header
            HtmlGenericControl header = new HtmlGenericControl("div");
            header.Attributes.Add("class", "project-header");
            header.InnerHtml = id + ". " + name;
            div.Controls.Add(header);

            // Thumbnail
            HtmlGenericControl thumbnail = new HtmlGenericControl("div");
            thumbnail.Attributes.Add("class", "project-thumbnail");
            thumbnail.Style["background-image"] = "url('/Uploads/Project_" + id + "/screenshot.jpg')";
            div.Controls.Add(thumbnail);

            return div;
        }

        // 2. Load users who share the same projects
        private void loadUser(int userID)
        {
            // Fetch data from database
            ProjectAccess myAccess = new ProjectAccess();
            myAccess.fetchSharedProjectUser(userID);
            var user = myAccess.MyDataSet.Tables["User"];

            // Send user list in JSON to client for further processing
            StringBuilder userList = new StringBuilder("userList = ");
            userList.Append(JsonConvert.SerializeObject(user));
            userList.Append(";");
            lists.Append(userList);
            myAccess.Dipose();
        }

        // 3. Event handler of clicking on a project box
        public void gotoProject(string data)
        {
            string[] info = data.Split('$');
            Session["projectID"] = info[0];
            Session["projectName"] = info[1];

            user = (Model.UserModel)Session["user"];
            var myQuery = new Query();
            if (!myQuery.IsProjectMember(int.Parse(info[0]), user.User_ID, user.Role))
            {
                myQuery.Dipose();
                Response.Redirect("/Error/error.html", true);
            }
            else Response.Redirect("Board.aspx");
        }
    }
}