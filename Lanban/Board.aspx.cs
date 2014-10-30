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
                int start = Environment.TickCount;
                createKanban();
                System.Diagnostics.Debug.WriteLine("Total load time: " + (Environment.TickCount - start));
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
            Session["projectID"] = projectID;
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
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Style.Add("background-color", row["Color"].ToString());
            div.Attributes.Add("class", "note");
            div.Attributes.Add("data-type", type);
            div.Attributes.Add("data-swimlane-id", row["Swimlane_ID"].ToString());
            
            // Although ID value is repeatable but it's needed for updatePosition and changeSwimlane
            string divID = (type.Equals("1")) ? row["Backlog_ID"].ToString() : row["Task_ID"].ToString();
            string id = tableName + "." + divID;
            div.Attributes.Add("id", id);
            div.Attributes.Add("data-id", divID); 
            div.Attributes.Add("ondblclick", "viewDetailNote('" + tableName.ToLower() + "'," + divID + ")");
            div.InnerHtml = divID + " - " + row["Title"].ToString();
            return div;
        }
    }
}