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

        protected void Page_Load(object sender, EventArgs e)
        {
            projectID = 1;
            int start = Environment.TickCount;
            myQuery = new Query();
            myQuery.fetchSwimlane(projectID);
            createTable();
            System.Diagnostics.Debug.WriteLine("Total load time: " + (Environment.TickCount - start));
        }

        protected void createTable()
        {
            var dataset = myQuery.MyDataSet;
            var swimlane = dataset.Tables["Swimlane"];
            var backlog = dataset.Tables["Backlog"];
            var task = dataset.Tables["Task"];

            //Start creating the table
            TableRow header = new TableRow();
            panelKanban.Controls.Add(header);
            TableRow body = new TableRow();
            panelKanban.Controls.Add(body);

            //Create header row and add the cell in the same column
            for (int i = 0; i < swimlane.Rows.Count; i++)
            {
                var row = swimlane.Rows[i];
                header.Controls.Add(createHeader(row["Name"].ToString(), Convert.ToInt32(row["Type"])));
                body.Controls.Add(createCell(row["Swimlane_ID"].ToString(), row["Type"].ToString(), i.ToString()));
            }
        }

        protected TableHeaderCell createHeader(string name, int type)
        {
            TableHeaderCell th = new TableHeaderCell();
            //StringBuilder temp = new StringBuilder();
            if (type != 3)
            {
                Image img = new Image();
                img.ImageUrl = "images/sidebar/add_item.png";
                img.CssClass = "btnAddItem";
                img.Attributes.Add("onclick", (type == 1) ? "showWindow('backlogWindow')" : "showWindow('taskWindow')");
                th.Controls.Add(img);
                th.Controls.Add(new LiteralControl(name));
                //temp.Append("<img src='images/sidebar/add_item.png' class='btnAddItem' onclick=\"showWindow('");
                //temp.Append((type == 1) ? "backlogWindow" : "taskWindow");
                //temp.Append("')\" />");
                //temp.Append(name);
                //th.Text = temp.ToString();
            }
            else th.Text = name;
            return th;
        }

        protected TableCell createCell(string swimlane_id, string type, string position)
        {
            //Initialize
            TableCell td = new TableCell();
            td.CssClass = "connected";
            td.Attributes.Add("data-id", swimlane_id);
            td.Attributes.Add("data-lane-type", type);
            td.Attributes.Add("data-position", position);

            //Fetch data
            string tableName = type.Equals("1") ? "Backlog" : "Task";
            myQuery.fetchNote(tableName, projectID, Convert.ToInt32(swimlane_id));
            var tempTable = myQuery.MyDataSet.Tables[tableName];

            //Add notes to this cell
            for (int i = 0; i < tempTable.Rows.Count; i++)
            {
                td.Controls.Add(createNote(tempTable.Rows[i], type, tableName));
            }

            //Clear table for the next fetch
            myQuery.MyDataSet.Tables[tableName].Clear();
            return td;
        }

        protected HtmlGenericControl createNote(DataRow row, string type, string tableName)
        {
            HtmlGenericControl div = new HtmlGenericControl("div");
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
    }
}