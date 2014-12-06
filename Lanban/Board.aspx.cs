using Lanban.AccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Lanban
{
    public partial class Board : System.Web.UI.Page
    {
        TableCell[] cell;
        TableHeaderCell[] header;
        int projectID;
        int userID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((Session["projectID"] == null) || (!User.Identity.IsAuthenticated))
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    try { InitBoard(); }
                    catch (Exception) { }
                }
                else
                {
                    if (Request.Params["__EVENTTARGET"].Equals("RedirectProject"))
                        Response.Redirect("Project.aspx");
                }
            }
        }

        protected async void InitBoard()
        {
            Model.UserModel user = (Model.UserModel)Session["user"];
            userID = user.User_ID;
            projectID = Convert.ToInt32(Session["projectID"]);

            // Start initialize
            var timer = System.Diagnostics.Stopwatch.StartNew();
            Task task1 = createKanban();
            Task task2 = Task.Run(() => initDropdownList());

            // Profile general info
            string name = Session["projectName"].ToString();
            Page.Title = "Lanban - " + name;
            lblProjectName.Text = name;

            //Profile area
            var profile = (Image)Master.FindControl("profile");
            profile.ToolTip = user.Name;
            profile.ImageUrl = user.Avatar;

            // Authentication
            var authen = new Controller.LanbanAuthentication();
            var ticket = authen.GetAuthenTicket(user.Username, projectID.ToString(), 30);
            authen.SetAuthenCookie(Response, ticket);
            string script = "const userID = " + userID + "; const projectID = " + projectID + ";";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "boardScript", script, true);
            // Wait all task to complete
            await Task.WhenAll(task1, task2);
            timer.Stop();
            System.Diagnostics.Debug.WriteLine(timer.ElapsedMilliseconds);
        }

        //1. Create the kanban board
        protected async Task createKanban()
        {
            SwimlaneAccess myAccess = new SwimlaneAccess();
            //Fetch data of all swimlanes in this project
            myAccess.fetchSwimlane(projectID);
            var swimlane = myAccess.MyDataSet.Tables["Swimlane"];
            int count = swimlane.Rows.Count;

            // Init
            cell = new TableCell[count];
            header = new TableHeaderCell[count];
            for (int i = 0; i < count; i++)
            {
                header[i] = new TableHeaderCell();
                panelKanbanHeader.Controls.Add(header[i]);

                cell[i] = new TableCell();
                panelKanbanBody.Controls.Add(cell[i]);
            }

            List<Task> tasks = new List<Task>();
            Task[] headerTask = new Task[count];
            //Create header row and add the cell in the same column
            for (int i = 0; i < count; i++)
            {
                // Temp value for this iteration - Parallel computing
                var row = swimlane.Rows[i];
                var tempi = i;

                tasks.Add(Task.Run(() => createHeader(row, tempi)));
                tasks.Add(Task.Run(() => createCell(row, tempi)));
            }

            await Task.WhenAll(tasks.ToArray());
            myAccess.Dipose();
        }

        //1.1 Add header cell to kanban board
        protected void createHeader(DataRow row, int i)
        {
            string swimlane_id = row["Swimlane_ID"].ToString();
            int type = Convert.ToInt32(row["Type"]);
            string name = row["Name"].ToString();
            
            // Attributes for the header
            var th = header[i];
            th.Attributes["data-id"] = swimlane_id;
            th.Attributes["data-type"] = type.ToString();
            th.Attributes["data-status"] = row["Data_status"].ToString();

            // Swimlane name
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Attributes["class"] = "swName";
            div.Attributes["title"] = name;
            div.InnerText = name;
            th.Controls.Add(div);
            
            if (type != 3)
            {
                Image img = new Image();
                img.ImageUrl = "images/sidebar/add_item.png";
                img.CssClass = "btnAddItem";
                string function = (type == 1) ? "showInsertWindow('backlogWindow'" : "showInsertWindow('taskWindow'";
                function += "," + i + "," + swimlane_id + ")";
                img.Attributes.Add("onclick", function);
                th.Controls.Add(img);
            }
        }

        //1.2 Add table cell to kanban board with sticky note
        protected async Task createCell(DataRow row, int position)
        {
            int swimlane_id = Convert.ToInt32(row["Swimlane_ID"]);
            string type = row["Type"].ToString();
            //Initialize
            Task task = addNotes(swimlane_id, type, position);
            var td = cell[position];
            td.CssClass = "connected";
            td.Attributes.Add("data-id", swimlane_id.ToString());
            td.Attributes.Add("data-lane-type", type);
            td.Attributes.Add("data-status", row["Data_status"].ToString());

            await task;
        }

        //1.3 Add content to the cell at position [position]
        protected async Task addNotes(int swimlane_id, string type, int position)
        {
            SwimlaneAccess tempAccess = new SwimlaneAccess();

            //Fetch data
            if (!type.Equals("3"))
            {
                string tableName = type.Equals("1") ? "Backlog" : "Task";
                tempAccess.fetchNote(tableName, projectID, swimlane_id);
            }
            else
                tempAccess.fetchDoneNote(swimlane_id);

            var tempTable = tempAccess.MyDataSet.Tables["init_temp"];
            int count = tempTable.Rows.Count;

            // Reserve the slot for note
            HtmlGenericControl[] notes = new HtmlGenericControl[count];
            for (int i = 0; i < count; i++)
            {
                notes[i] = new HtmlGenericControl("div");
                cell[position].Controls.Add(notes[i]);
            }

            List<Task> task = new List<Task>();
            //Add notes to this cell
            if (!type.Equals("3"))
            {
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    var tempi = i;
                    task.Add(Task.Run(() =>
                        createNote(ref notes[tempi], tempTable.Rows[tempi], type)));
                }
            }
            else
            {
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    var tempi = i;
                    var row = tempTable.Rows[i];
                    task.Add(Task.Run(() =>
                        createNote(ref notes[tempi], row, row["Type"].ToString())));
                }
            }

            //Clear table for the next fetch
            await Task.WhenAll(task.ToArray());
            tempAccess.Dipose();
        }

        //1.4 Create sticky notes based on retrieved data from database
        protected HtmlGenericControl createNote(ref HtmlGenericControl div, DataRow row, string type)
        {
            string tableName = (type.Equals("1")) ? "backlog" : "task";

            div.Attributes.Add("class", "note");
            div.Attributes.Add("data-type", type);
            div.Attributes.Add("data-status", row["Status"].ToString());
            if (type.Equals("2")) div.Attributes.Add("data-backlog-id", row["Backlog_ID"].ToString());

            // Although ID value is repeatable 
            // but it's needed for updatePosition, changeSwimlane and delete a note

            // In case the swimlane has mixture of both Task and Backlog items
            string divID;
            try { divID = row["Item_ID"].ToString(); }
            catch { divID = (type.Equals("1")) ? row["Backlog_ID"].ToString() : row["Task_ID"].ToString(); }

            string id = tableName + "." + divID;
            div.Attributes.Add("id", id);
            div.Attributes.Add("data-id", divID);
            string color = row["Color"].ToString();

            // note-header
            HtmlGenericControl header = new HtmlGenericControl("div");
            header.Attributes.Add("class", "note-header");
            header.Style.Add("background-color", color.Substring(0, 7));
            header.InnerHtml = "<span class='item-id'>" + row["Relative_ID"].ToString() + "</span>";

            // Add edit button
            Image edit = new Image();
            edit.ImageUrl = "images/sidebar/edit_note.png";
            edit.CssClass = "note-button";
            edit.Attributes.Add("onclick", "viewDetailNote(" + divID + ",'" + tableName + "')");
            header.Controls.Add(edit);

            // Add delete button
            Image delete = new Image();
            delete.ImageUrl = "images/sidebar/delete_note.png";
            delete.CssClass = "note-button";
            delete.Attributes.Add("onclick", "deleteItem(" + divID + ",'" + tableName + "')");
            header.Controls.Add(delete);
            div.Controls.Add(header);

            // note-content
            HtmlGenericControl content = new HtmlGenericControl("div");
            content.Attributes.Add("class", "note-content");
            content.Style.Add("background-color", color.Substring(8));
            content.InnerHtml = row["Title"].ToString();
            div.Controls.Add(content);

            // note-footer
            HtmlGenericControl footer = new HtmlGenericControl("div");
            footer.Attributes.Add("class", "note-footer");
            footer.Style.Add("background-color", color.Substring(8));
            div.Controls.Add(footer);

            if (type.Equals("1"))
            {
                // Add statistics button
                HtmlGenericControl stat = new HtmlGenericControl("div");
                stat.Attributes.Add("class", "note-stat-button");
                stat.Attributes.Add("onmouseover", "viewBacklogStat(this, " + divID + ")");
                stat.Attributes.Add("onmouseout", "hideBacklogStat()");
                footer.Controls.Add(stat);
            }
            else
            {
                footer.Controls.Add(new LiteralControl("&nbsp;"));
            }

            return div;
        }

        //2. Init dropdown list value
        string[] colorText = { "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple" };
        string[,] colorHex = {
                                {"#ff9898", "#ffc864", "#ffff95", "#98ff98", "#caffff", "#adadff", "#d598ff" },
                                { "#ff4b4b", "#ffa500", "#ffff4b", "#4bff4b", "#80ffff", "#6464ff", "#b64bff" }
                             };

        protected void initDropdownList()
        {
            //Dropdown list Complexity - Backlog
            for (int i = 1; i <= 10; i++)
                ddlBacklogComplexity.Items.Add(new ListItem(i.ToString(), i.ToString()));
            ddlBacklogComplexity.SelectedIndex = 0;

            //Dropdown list Color - Backlog and Task
            for (int i = 0; i < colorText.Length; i++)
            {
                ddlBacklogColor.Items.Add(new ListItem(colorText[i], colorHex[0, i] + "." + colorHex[1, i]));
                ddlTaskColor.Items.Add(new ListItem(colorText[i], colorHex[0, i] + "." + colorHex[1, i]));
            }
            ddlBacklogColor.SelectedIndex = 0;
            ddlTaskColor.SelectedIndex = 0;
        }

    }
}