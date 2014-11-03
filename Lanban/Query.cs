using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Text;
using System.Web.UI;
using Newtonsoft.Json;
using System.Globalization;

namespace Lanban
{
    public class Query
    {
        string myConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\Lanban.accdb";
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

        //2.2.1 Insert new backlog
        public string insertNewBacklog(string[] data)
        {
            StringBuilder command = new StringBuilder("INSERT INTO Backlog (Project_ID, Swimlane_ID," +
               "Title, Description, Complexity, Color, [Position], Status) VALUES (");
            for (int i = 0; i < data.Length; i++)
                command.Append("'" + data[i] + "',");

            // The new position is the current number of item
            command.Append(countItem(data[1], "Backlog") + ",");

            // The status is based on the data status in swimlane
            command.Append("'" + getDataStatus(data[1]) + "')");
            myCommand.CommandText = command.ToString();
            myCommand.ExecuteNonQuery();

            // Get the ID just inserted -- Change later with @@SCOPE_IDENTITY in SQL SERVER
            myCommand.CommandText = "SELECT MAX(Backlog_ID) FROM Backlog";
            string id = myCommand.ExecuteScalar().ToString();
            return id;
        }

        //2.2.2 Insert new task
        public string insertNewTask(string[] data)
        {
            StringBuilder command = new StringBuilder("INSERT INTO Task (Project_ID, Swimlane_ID, Backlog_ID, " +
               "Title, Description, Work_estimation, Color, [Position], Status) VALUES (");
            for (int i = 0; i < data.Length - 1; i++)
                command.Append("'" + data[i] + "',");

            // The new position is the current number of item
            command.Append(countItem(data[1], "Task") + ",");

            // The status is based on the data status in swimlane
            command.Append("'" + getDataStatus(data[1]) + "')");
            myCommand.CommandText = command.ToString();
            myCommand.ExecuteNonQuery();

            // Get the ID just inserted -- Change later with @@SCOPE_IDENTITY in SQL SERVER
            myCommand.CommandText = "SELECT MAX(Task_ID) FROM Task";
            string id = myCommand.ExecuteScalar().ToString();
            return id;
        }

        //2.3.1 Save editted data of a backlog
        public void updateBacklog(string id, string[] data)
        {
            StringBuilder command = new StringBuilder("UPDATE Backlog SET ");
            command.Append("Title='").Append(data[2]).Append("', ");
            command.Append("Description='").Append(data[3]).Append("', ");
            command.Append("Complexity='").Append(data[4]).Append("', ");
            command.Append("Color='").Append(data[5]).Append("' ");
            command.Append("WHERE Backlog_ID=").Append(id);
            myCommand.CommandText = command.ToString();
            myCommand.ExecuteNonQuery();
        }

        //2.3.2 save editted data of a task
        public void updateTask(string id, string[] data)
        {
            StringBuilder command = new StringBuilder("UPDATE Task SET ");
            command.Append("Backlog_ID='").Append(data[2]).Append("', ");
            command.Append("Title='").Append(data[3]).Append("', ");
            command.Append("Description='").Append(data[4]).Append("', ");
            command.Append("Work_estimation='").Append(data[5]).Append("', ");
            command.Append("Color='").Append(data[6]).Append("', ");
            //command.Append("Due_date= CONVERT(DATETIME, '").Append(data[7]).Append("', 104) ");
            command.Append("Due_date='#").Append(data[7]).Append("#' ");
            command.Append("WHERE Task_ID=").Append(id);
            myCommand.CommandText = command.ToString();
            myCommand.ExecuteNonQuery();
        }

        //2.4 Delete an item
        public void deleteItem(string id, string type)
        {
            myCommand.CommandText = "DELETE FROM " + type + " WHERE " + type + "_ID=" + id;
            myCommand.ExecuteNonQuery();

            //Cascade deleting
            deleteAssignee(id, type);
        }

        //2.5.1 Update position of sticky note
        public void updatePosition(string id, string pos, string table)
        {
            myCommand.CommandText = "UPDATE " + table + " SET [Position]=" + pos + "  WHERE " + table + "_ID=" + id;
            myCommand.ExecuteNonQuery();
        }

        //2.5.2 Change swimlane of sticky note
        public void changeSwimlane(string id, string pos, string table, string swimlane_id)
        {
            string status = getDataStatus(swimlane_id);
            myCommand.CommandText = "UPDATE " + table + " SET " +
                                    "Swimlane_ID=" + swimlane_id + ", [Position]=" + pos + ", Status='" + status +
                                    "' WHERE " + table + "_ID=" + id;
            myCommand.ExecuteNonQuery();
        }

        //2.6.1 Save assignee of a task or backlog
        public void saveAssignee(string id, string type, string uid, string name)
        {
            myCommand.CommandText = "INSERT INTO " + type + "_User (" + type + "_ID, User_ID, [Name])" +
                " VALUES (" + id + "," + uid + ",'" + name + "');";
            myCommand.ExecuteNonQuery();
        }

        //2.6.2 Delete all assignee of a task or backlog
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

        //a.2 Get Data_status field of a swimlane in Swimlane table
        protected string getDataStatus(string swimlane_id)
        {
            myCommand.CommandText = "SELECT Data_status FROM Swimlane WHERE Swimlane_ID=" + swimlane_id;
            return myCommand.ExecuteScalar().ToString();
        }

        //a.3 Get number of item in a swimlane
        protected string countItem(string swimlane_id, string type)
        {
            myCommand.CommandText = "SELECT COUNT(*) FROM " + type + " WHERE Swimlane_ID=" + swimlane_id;
            return myCommand.ExecuteScalar().ToString();
        }
    }
}