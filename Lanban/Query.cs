using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Text;
using System.Web.UI;
using Newtonsoft.Json;

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

        public void Dispose()
        {
            myConnection.Close();
            myDataSet.Dispose();
            myAdapter.Dispose();
            myCommand.Dispose();
            myConnection.Dispose();
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
            myCommand.CommandText = "SELECT * FROM " + tableName + " WHERE Project_ID=" + projectID + " AND Swimlane_ID=" + swimlaneID + " ORDER BY [Position]";
            myAdapter.Fill(myDataSet, tableName);
        }

        //Get all information of an item based on it
        public string viewItem(string id, string type)
        {
            StringBuilder result = new StringBuilder();
            myCommand.CommandText = "SELECT * FROM " + type + " WHERE " + type + "_ID=" + id;
            myReader = myCommand.ExecuteReader();
            bool available = myReader.Read();
            if (available == false) result.Append("");
            else
            {
                StringWriter sw = new StringWriter(result);
                JsonTextWriter myWriter = new JsonTextWriter(sw);
                myWriter.WriteStartObject();
                for (int i = 0; i < myReader.FieldCount; i++)
                {
                    myWriter.WritePropertyName(myReader.GetName(i));
                    myWriter.WriteValue(myReader.GetValue(i));
                }
                myWriter.WriteEndObject();
            }
            return result.ToString();
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
            myCommand.CommandText = "SELECT MAX(Backlog_ID) FROM Backlog";
            result = myCommand.ExecuteScalar().ToString();
            return result;
        }

        //2.3 Update position of sticky note
        public void updatePosition(string id, string pos, string table)
        {
            myCommand.CommandText = "UPDATE " + table + " SET [Position]=" + pos + "  WHERE " + table + "_ID=" + id;
            myCommand.ExecuteNonQuery();
        }

        //2.4 Change swimlane of sticky note
        public void changeSwimlane(string id, string pos, string table, string swimlane_id)
        {
            myCommand.CommandText = "UPDATE " + table + " SET Swimlane_ID=" + swimlane_id + ", [Position]=" + pos + "  WHERE " + table + "_ID=" + id;
            myCommand.ExecuteNonQuery();
        }

        //2.5 Save assignee of a task or backlog
        public void saveAssignee(string id, string type, string uid, string name)
        {
            myCommand.CommandText = "INSERT INTO " + type + "_User (" + type + "_ID, User_ID, [Name])" +
                " VALUES (" + id + "," + uid + ",'" + name + "');";
            myCommand.ExecuteNonQuery();
        }

        //2.6 Delete all assignee of a task or backlog
        public void deleteAssignee(string id, string type)
        {
            myCommand.CommandText = "DELETE FROM " + type + "_User WHERE " + type + "_ID=" + id;
            myCommand.ExecuteNonQuery();
        }

        //2.7 View assignee of a task or backlog
        public string viewAssignee(string id, string type)
        {
            myCommand.CommandText = "SELECT * FROM " + type + "_User WHERE " + type + "_ID=" + id;
            StringBuilder result = new StringBuilder();

            myReader = myCommand.ExecuteReader();
            bool available = myReader.Read();
            if (available == false) result.Append("");
            else
            {
                while (available)
                {
                    result.Append(getAssigneeDisplay(myReader[1].ToString(), myReader[2].ToString()));
                    available = myReader.Read();
                }
            }
            myReader.Close();
            return result.ToString();
        }

        //2.7.1 Helper 2.7
        protected string getAssigneeDisplay(string id, string name)
        {
            string display = "<div class='assignee-name-active' data-id='" + id + "' onclick='removeAssignee(this)'>" + name + "</div>";
            return display;
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
                    result.Append(getAssigneeResultDisplay(myReader[0].ToString(), myReader[1].ToString(), type));
                    available = myReader.Read();
                }
            }
            myReader.Close();
            return result.ToString();
        }

        //a.1.1 Helper a.1
        protected string getAssigneeResultDisplay(string ID, string name, string type)
        {
            string display = "<div class='resultline' data-id='" + ID + "' onclick=\"addAssignee(this,'" + type + "')\">" + name + "</div>";
            return display;
        }
    }
}