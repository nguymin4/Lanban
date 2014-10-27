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
            
            if (!IsPostBack)
            {
                projectID = 1;
                int start = Environment.TickCount;
                myQuery = new Query();
                createTable();
                System.Diagnostics.Debug.WriteLine("Total load time: " + (Environment.TickCount - start));
                Session["myQuery"] = myQuery;
            }
            else
            {
                projectID = Convert.ToInt32(Session["projectID"]);
                myQuery = (Query) Session["myQuery"];
                cell = (TableCell []) Session["cell"];
            }
            
        }

        //1. Create the kanban board
        protected void createTable()
        {
            //Fetch data of all swimlanes in this project
            myQuery.fetchSwimlane(projectID);
            var swimlane = myQuery.MyDataSet.Tables["Swimlane"];

            //Start creating the table
            TableRow header = new TableRow();
            panelKanban.Controls.Add(header);
            TableRow body = new TableRow();
            panelKanban.Controls.Add(body);
            cell = new TableCell[swimlane.Rows.Count];
            //Create header row and add the cell in the same column
            for (int i = 0; i < swimlane.Rows.Count; i++)
            {
                var row = swimlane.Rows[i];
                header.Controls.Add(createHeader(row, i));
                createCell(row["Swimlane_ID"].ToString(), row["Type"].ToString(), i);
                body.Controls.Add(cell[i]);
            }

            Session["projectID"] = projectID;
            Session["cell"] = cell;
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
                string function = (type == 1) ? "showWindow('backlogWindow'" : "showWindow('taskWindow'";
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
            td.Attributes.Add("data-swimlane-id", swimlane_id);
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
            string divID;
            if (type.Equals("1"))
            {
                divID = row["Backlog_ID"].ToString();
            }
            else
            {
                div.Attributes.Add("data-id", row["Task_ID"].ToString());
                divID = row["Task_ID"].ToString();
            }
            div.Attributes.Add("data-id", divID);
            div.Attributes.Add("onclick", "viewDetailNote('" + tableName.ToLower() + "Window'," + divID + ")");
            div.InnerHtml = divID + " - " + row["Title"].ToString();
            return div;
        }

        //2. Clear the kanban board
        protected void clearTable()
        {
            panelKanban.Controls.Clear();
        }


        //3. Methods
        //3.1 Add new backlog item
        protected void btnAddBacklog_Click(object sender, EventArgs e)
        {
            string[] data = { 
                                projectID.ToString(), txtSwimlaneID.Text ,
                                txtBacklogTitle.Text, txtBacklogDescription.Text, 
                                ddlBacklogComplexity.SelectedValue, ddlBacklogColor.SelectedItem.Text, 
                                txtNoteIndex.Text
                            };
            myQuery.insertNewBacklog(data);
            addNotes(Convert.ToInt32(txtSwimlaneID.Text), "1", Convert.ToInt32(txtSwimlanePosition.Text));
        }
    }
}