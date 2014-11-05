using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Data;


namespace Lanban
{
    public partial class Board : System.Web.UI.Page
    {
        Query myQuery;
        int projectID;
        TableCell[] cell;

        protected void Page_Load(object sender, EventArgs e)
        {
            myQuery = new Query();
            if (!IsPostBack)
            {
                //projectID = Convert.ToInt32(Session["projectID"]);
                projectID = 1;
                txtProjectID.Text = projectID.ToString();
                createKanban();
                initDropdownList();
                Session["projectID"] = projectID;
            }
        }

        //1. Create the kanban board
        protected void createKanban()
        {
            //Fetch data of all swimlanes in this project
            myQuery.fetchSwimlane(projectID);
            var swimlane = myQuery.MyDataSet.Tables["Swimlane"];
            cell = new TableCell[swimlane.Rows.Count];
            //Create header row and add the cell in the same column
            for (int i = 0; i < swimlane.Rows.Count; i++)
            {
                var row = swimlane.Rows[i];
                panelKanbanHeader.Controls.Add(createHeader(row, i));
                createCell(row["Swimlane_ID"].ToString(), row["Type"].ToString(), i);
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
        protected void createCell(string swimlane_id, string type, int position)
        {
            //Initialize
            cell[position] = new TableCell();
            var td = cell[position];
            td.CssClass = "connected";
            td.Attributes.Add("data-id", swimlane_id);
            td.Attributes.Add("data-lane-type", type);
            //td.Attributes.Add("data-position", position.ToString());
            addNotes(Convert.ToInt32(swimlane_id), type, position);
        }

        //1.3 Add content to the cell at position [position]
        protected void addNotes(int swimlane_id, string type, int position)
        {
            if (cell[position].HasControls()) cell[position].Controls.Clear();

            //Fetch data
            string tableName = type.Equals("1") ? "Backlog" : "Task";
            myQuery.fetchNote(tableName, projectID, Convert.ToInt32(swimlane_id));
            var tempTable = myQuery.MyDataSet.Tables[tableName];

            //Add notes to this cell
            for (int i = 0; i < tempTable.Rows.Count; i++)
                cell[position].Controls.Add(createNote(tempTable.Rows[i], type, tableName));

            //Clear table for the next fetch
            myQuery.MyDataSet.Tables[tableName].Clear();
        }

        //1.4 Create sticky notes based on retrieved data from database
        protected HtmlGenericControl createNote(DataRow row, string type, string tableName)
        {
            tableName = tableName.ToLower();
            
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Attributes.Add("class", "note");
            div.Attributes.Add("data-type", type);

            // Although ID value is repeatable 
            // but it's needed for updatePosition, changeSwimlane and delete a note
            string divID = (type.Equals("1")) ? row["Backlog_ID"].ToString() : row["Task_ID"].ToString();
            string id = tableName + "." + divID;
            div.Attributes.Add("id", id);
            div.Attributes.Add("data-id", divID);
            string editFunction = "viewDetailNote(" + divID + ",'" + tableName + "')";
            div.Attributes.Add("ondblclick", editFunction);

            string color = row["Color"].ToString();
            // note-header
            HtmlGenericControl header = new HtmlGenericControl("div");
            header.Attributes.Add("class", "note-header");
            header.Style.Add("background-color", color.Substring(0, 7));
            HtmlGenericControl item_id = new HtmlGenericControl("span");
            item_id.Attributes.Add("class", "item-id");
            item_id.InnerHtml = row["Relative_ID"].ToString();
            header.Controls.Add(item_id);
            // Add edit button
            Image edit = new Image();
            edit.ImageUrl = "images/sidebar/edit_note.png";
            edit.CssClass = "note-button";
            edit.Attributes.Add("onclick", editFunction);
            header.Controls.Add(edit);
            // Add delete button
            Image delete = new Image();
            delete.ImageUrl = "images/sidebar/delete_note.png";
            delete.CssClass = "note-button";
            string deleteFunction = "deleteItem(" + divID + ",'" + tableName + "')";
            delete.Attributes.Add("onclick", deleteFunction);
            header.Controls.Add(delete);
            div.Controls.Add(header);


            // note-content
            HtmlGenericControl content = new HtmlGenericControl("div");
            content.Attributes.Add("class", "note-content");
            content.Style.Add("background-color", color.Substring(8));
            content.InnerHtml = row["Title"].ToString();
            div.Controls.Add(content);
            
            return div;
        }

        //2. Init dropdown list value
        string[] colorText = { "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple"};
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