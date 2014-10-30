using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Text;
using System.Web.UI;

namespace Lanban
{
    public class Query
    {
        string myConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Programming\Source code\ASP.NET\Lanban\Lanban\Lanban.accdb";
        private OleDbConnection myConnection;
        private OleDbCommand myCommand;
        private OleDbDataAdapter myAdapter;
        private OleDbDataReader myReader;
        private DataSet myDataSet;

        public DataSet MyDataSet
        {
            get { return myDataSet; }
        }

        //1. Constructor
        public Query()
        {
            myConnection = new OleDbConnection(myConnectionString);
            myCommand = new OleDbCommand();
            myCommand.Connection = myConnection;
            myDataSet = new DataSet();
            myAdapter = new OleDbDataAdapter(myCommand);
            myConnection.Open();
        }


        //2. Methods
        //2.1 Query data
        public void fetchSwimlane(int projectID)
        {
            //Retrieve Swimlane data
            myCommand.CommandText = "SELECT * FROM Swimlane WHERE Project_ID=" + projectID;
            myAdapter.Fill(myDataSet, "Swimlane");
            myDataSet.Tables["Swimlane"].DefaultView.Sort = "Position";
        }

        //Fill dataset with data from either Task table or Backlog table based on parameter
        public void fetchNote(string tableName, int projectID, int swimlaneID)
        {
            myCommand.CommandText = "SELECT * FROM " + tableName + " WHERE Project_ID=" + projectID + " AND Swimlane_ID=" + swimlaneID;
            myAdapter.Fill(myDataSet, tableName);
            myDataSet.Tables[tableName].DefaultView.Sort = "Position";
        }


        //2.2 Insert new data
        public string insertNewBacklog(string[] data)
        {
            string result;
            StringBuilder command = new StringBuilder("INSERT INTO Backlog (Project_ID, Swimlane_ID," +
               "Title, Description, Complexity, Color, [Position]) VALUES (");
            for (int i = 0; i < data.Length - 1; i++)
                command.Append("'" + data[i] + "',");

            command.Append("'" + data[data.Length - 1] + "')");
            myCommand.CommandText = command.ToString();
            myCommand.ExecuteNonQuery();
            myCommand.CommandText = "SELECT LAST(Backlog_ID) FROM Backlog";
            result = myCommand.ExecuteScalar().ToString();
            return result;
        }

        //2.3 Update position of sticky note
        public string updatePosition(string id, string pos, string table)
        {
            myCommand.CommandText = "UPDATE " + table + " SET [Position]=" + pos + "  WHERE " + table + "_ID=" + id;
            myCommand.ExecuteNonQuery();
            return "Updated " + id + " in " + table + " at " + pos;
        }

        //2.4 Change swimlane of sticky note
        public void changeSwimlane(string id, string pos, string table, string swimlane_id)
        {
            myCommand.CommandText = "UPDATE " + table + " SET Swimlane_ID=" + swimlane_id + ", [Position]=" + pos + "  WHERE " + table + "_ID=" + id;
            myCommand.ExecuteNonQuery();
        }

        //2.5 Save assignee of a task or backlog
        public void saveAssignee(string type, string id, string uid, string name)
        {
            myCommand.CommandText = "INSERT INTO " + type + "_User (" + type + "_ID, User_ID, [Name])" +
                "VALUES (" + id + "," + uid + ",'" + name + "');";
            myCommand.ExecuteNonQuery();
        }

        //2.6 Delete all assignee of a task or backlog
        public void deleteAssignee(string type, string id)
        {
            myCommand.CommandText = "DELETE FROM " + type + "_User WHERE " + type + "_ID=" + id;
            myCommand.ExecuteNonQuery();
        }

        //a.1 Search member name in a project
        public string searchAssignee(string projectID, string keyword, string type)
        {
            myCommand.CommandText = "SELECT TOP 3 User_ID, Name FROM Project_User " +
                                    "WHERE Project_ID = " + projectID +
                                    " AND Name LIKE '%" + keyword + "%'";
            StringBuilder result = new StringBuilder();

            myReader = myCommand.ExecuteReader();
            bool available = myReader.Read();
            if (available == false) result.Append("No record found.");
            else
            {
                while (available)
                {
                    result.Append(getAssigneeDisplay(myReader[0].ToString(), myReader[1].ToString(), type));
                    available = myReader.Read();
                }
            }
            return result.ToString();
        }

        protected string getAssigneeDisplay(string ID, string name, string type)
        {
            string display = "<div class='resultline' data-id='" + ID + "' onclick=\"addAssignee(this,'" + type + "')\">" + name + "</div>";
            return display;
        }
    }
}