using Lanban.AccessLayer;
using System;
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
        int projectID;

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null) Response.Redirect("Login.aspx");
            if (!IsPostBack)
            {
                projectID = Convert.ToInt32(Session["projectID"]);
                string name = Session["projectName"].ToString();
                Page.Title = "Lanban " + name;
                lblProjectName.Text = name;
                var timer = System.Diagnostics.Stopwatch.StartNew();
                await createKanban();
                await Task.Run(() => initDropdownList());
                timer.Stop();
                System.Diagnostics.Debug.WriteLine(timer.ElapsedMilliseconds);
                string script = "var projectID="+projectID+"; var userID="+Session["userID"]+";";
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "boardScript", script, true);
            }
            else
            {
                if (Request.Params["__EVENTTARGET"].Equals("RedirectProject"))
                {
                    await Task.Run(() => Response.Redirect("Project.aspx"));
                }
            }
        }

        //1. Create the kanban board
        protected async Task createKanban()
        {
            SwimlaneAccess myAccess = new SwimlaneAccess();
            //Fetch data of all swimlanes in this project
            myAccess.fetchSwimlane(projectID);
            var swimlane = myAccess.MyDataSet.Tables["Swimlane"];
            cell = new TableCell[swimlane.Rows.Count];
            //Create header row and add the cell in the same column
            for (int i = 0; i < swimlane.Rows.Count; i++)
            {
                var row = swimlane.Rows[i];
                await Task.Run(() => panelKanbanHeader.Controls.Add(createHeader(row, i)));
                await createCell(row, i);
                panelKanbanBody.Controls.Add(cell[i]);
            }
        }

        //1.1 Add header cell to kanban board
        protected TableHeaderCell createHeader(DataRow row, int i)
        {
            TableHeaderCell th = new TableHeaderCell();
            int type = Convert.ToInt32(row["Type"]);
            int swimlane_id = Convert.ToInt32(row["Swimlane_ID"]);
            string name = row["Name"].ToString();
            if (type != 3)
            {
                Image img = new Image();
                img.ImageUrl = "images/sidebar/add_item.png";
                img.CssClass = "btnAddItem";
                string function = (type == 1) ? "showInsertWindow('backlogWindow'" : "showInsertWindow('taskWindow'";
                function += "," + i + "," + swimlane_id + ")";
                img.Attributes.Add("onclick", function);
                th.Controls.Add(img);
                th.Controls.Add(new LiteralControl(name));
            }
            else th.Text = name;
            return th;
        }

        //1.2 Add table cell to kanban board with sticky note
        protected async Task createCell(DataRow row, int position)
        {
            int swimlane_id = Convert.ToInt32(row["Swimlane_ID"]);
            string type = row["Type"].ToString();
            //Initialize
            cell[position] = new TableCell();
            var td = cell[position];
            td.CssClass = "connected";
            td.Attributes.Add("data-id", swimlane_id.ToString());
            td.Attributes.Add("data-lane-type", type);
            td.Attributes.Add("data-status", row["Data_status"].ToString());
            await addNotes(swimlane_id, type, position);
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
                var tempTable = tempAccess.MyDataSet.Tables["init_temp"];
                //Add notes to this cell
                for (int i = 0; i < tempTable.Rows.Count; i++)
                    cell[position].Controls.Add(await Task.Run(
                        () => createNote(tempTable.Rows[i], type)));
            }
            else
            {
                tempAccess.fetchDoneNote(swimlane_id);
                //Add notes to this cell
                foreach (DataRow row in tempAccess.MyDataSet.Tables["init_temp"].Rows)
                    cell[position].Controls.Add(await Task.Run(
                        () => createNote(row, row["Type"].ToString())));
            }

            //Clear table for the next fetch
            tempAccess.Dipose();
        }

        //1.4 Create sticky notes based on retrieved data from database
        protected HtmlGenericControl createNote(DataRow row, string type)
        {
            string tableName = (type.Equals("1")) ? "backlog" : "task";

            HtmlGenericControl div = new HtmlGenericControl("div");
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