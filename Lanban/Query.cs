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

        public void Dipose()
        {
            myConnection.Close();
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Add parameter for myCommand
        protected void addParameter<T>(string parameter, OleDbType type, T value)
        {
            myCommand.Parameters.Add(parameter, type);
            myCommand.Parameters[parameter].Value = value;
        }

        //2. Methods manipulate and query data
        //2.1 Query data
        public void fetchSwimlane(int projectID)
        {
            //Retrieve Swimlane data
            myCommand.CommandText = "SELECT * FROM Swimlane WHERE Project_ID=@projectID ORDER BY [Position]";
            addParameter<int>("@projectID", OleDbType.Integer, projectID);
            myAdapter.Fill(myDataSet, "Swimlane");
            myCommand.Parameters.Clear();
        }

        //Fill dataset with data from either Task table or Backlog table based on parameter
        public void fetchNote(string tableName, int projectID, int swimlaneID)
        {
            myCommand.CommandText = "SELECT * FROM " + tableName + " WHERE Project_ID=@projectID AND Swimlane_ID=@swimlaneID ORDER BY [Position]";
            addParameter<int>("@projectID", OleDbType.Integer, projectID);
            addParameter<int>("@swimlaneID", OleDbType.Integer, swimlaneID);
            myAdapter.Fill(myDataSet, tableName);
            myCommand.Parameters.Clear();
        }

        //Get all information of an item based on it
        public string viewItem(string id, string type)
        {
            StringBuilder result = new StringBuilder();
            myCommand.CommandText = "SELECT * FROM " + type + " WHERE " + type + "_ID=@id";
            addParameter<int>("@id", OleDbType.Integer, Convert.ToInt32(id));
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
            myCommand.Parameters.Clear();
            return result.ToString();
        }

        //2.2.1 Insert new backlog
        public string insertNewBacklog(Backlog backlog)
        {
            StringBuilder command = new StringBuilder();
            command.Append("INSERT INTO Backlog (Project_ID, Swimlane_ID, Title, Description, Complexity, Color, [Position], Status) ");
            command.Append("VALUES (@Project_ID, @Swimlane_ID, @Title, @Description, @Complexity, @Color, @Position, @Status)");
            addParameter<int>("@Project_ID", OleDbType.Integer, backlog.Project_ID);
            addParameter<int>("@Swimlane_ID", OleDbType.Integer, backlog.Swimlane_ID);
            addParameter<string>("@Title", OleDbType.LongVarChar, backlog.Title);
            addParameter<string>("@Description", OleDbType.LongVarChar, backlog.Description);
            addParameter<int>("@Complexity", OleDbType.Integer, backlog.Complexity);
            addParameter<string>("@Color", OleDbType.VarChar, backlog.Color);
            addParameter<int>("@Position", OleDbType.VarChar, countItem(backlog.Swimlane_ID, "Task"));
            string status = getDataStatus(backlog.Swimlane_ID.ToString());
            addParameter<string>("@Status", OleDbType.VarChar, status);
            myCommand.CommandText = command.ToString();
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            // Get the ID just inserted -- Change later with @@SCOPE_IDENTITY in SQL SERVER
            myCommand.CommandText = "SELECT MAX(Backlog_ID) FROM Backlog";
            string id = myCommand.ExecuteScalar().ToString();
            updateDate(id, "Backlog", status);
            return id;
        }

        //2.2.2 Insert new task
        public string insertNewTask(Task task)
        {
            StringBuilder command = new StringBuilder();
            command.Append("INSERT INTO Task (Project_ID, Swimlane_ID, Backlog_ID, Title, ");
            command.Append("Description, Work_estimation, Color, [Position], Status, Due_date) ");
            command.Append("VALUES (@Project_ID, @Swimlane_ID, @Backlog_ID, @Title, ");
            command.Append("@Description, @Work_estimation, @Color, @Position, @Status, @Due_date)");

            addParameter<int>("@Project_ID", OleDbType.Integer, task.Project_ID);
            addParameter<int>("@Swimlane_ID", OleDbType.Integer, task.Swimlane_ID);
            addParameter<int>("@Backlog_ID", OleDbType.Integer, task.Backlog_ID);
            addParameter<string>("@Title", OleDbType.LongVarChar, task.Title);
            addParameter<string>("@Description", OleDbType.LongVarChar, task.Description);

            // Work estimation can be null
            if (task.Work_estimation == null) addParameter<DBNull>("@Work_estimation", OleDbType.Integer, DBNull.Value);
            else addParameter("@Work_estimation", OleDbType.Integer, task.Work_estimation);

            addParameter<string>("@Color", OleDbType.VarChar, task.Color);
            addParameter<int>("@Position", OleDbType.VarChar, countItem(task.Swimlane_ID, "Task"));

            string status = getDataStatus(task.Swimlane_ID.ToString());
            addParameter<string>("@Status", OleDbType.VarChar, status);

            // Due date is nullable field - the user can skip input data of that field
            if (task.Due_date.Equals("")) addParameter<DBNull>("@Due_date", OleDbType.DBDate, DBNull.Value);
            else addParameter<DateTime>("@Due_date", OleDbType.DBDate, DateTime.ParseExact(task.Due_date, "dd.MM.yyyy", null));

            myCommand.CommandText = command.ToString();
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            // Get the ID just inserted -- Change later with @@SCOPE_IDENTITY in SQL SERVER
            myCommand.CommandText = "SELECT MAX(Task_ID) FROM Task";
            string id = myCommand.ExecuteScalar().ToString();
            updateDate(id, "Task", status);
            return id;
        }

        //2.3.1 Save editted data of a backlog
        public void updateBacklog(string id, Backlog backlog)
        {
            string command = "UPDATE Backlog SET Title=@Title, Description=@Description, " +
                "Complexity=@Complexity, Color=@Color WHERE Backlog_ID=@Backlog_ID";

            addParameter<string>("@Title", OleDbType.LongVarChar, backlog.Title);
            addParameter<string>("@Description", OleDbType.LongVarChar, backlog.Description);
            addParameter<int>("@Complexity", OleDbType.Integer, backlog.Complexity);
            addParameter<string>("@Color", OleDbType.VarChar, backlog.Color);
            addParameter("@Backlog_ID", OleDbType.Integer, Convert.ToInt32(id));

            myCommand.CommandText = command;
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        //2.3.2 save editted data of a task
        public void updateTask(string id, Task task)
        {
            string command = "UPDATE Task SET Backlog_ID=@Backlog_ID, Title=@Title, Description=@Description, " +
                "Work_estimation=@Work_estimation, Color=@Color, Due_date=@Due_date WHERE Task_ID=@Task_ID";

            addParameter<int>("@Backlog_ID", OleDbType.Integer, task.Backlog_ID);
            addParameter<string>("@Title", OleDbType.LongVarChar, task.Title);
            addParameter<string>("@Description", OleDbType.LongVarChar, task.Description);

            // Work estimation can be null
            if (task.Work_estimation == null) addParameter("@Work_estimation", OleDbType.Integer, DBNull.Value);
            else addParameter("@Work_estimation", OleDbType.Integer, task.Work_estimation);

            addParameter<string>("@Color", OleDbType.VarChar, task.Color);

            // Due date is nullable field - the user can skip input data of that field
            if (task.Due_date.Equals("")) addParameter<DBNull>("@Due_date", OleDbType.DBDate, DBNull.Value);
            else addParameter<DateTime>("@Due_date", OleDbType.DBDate, DateTime.ParseExact(task.Due_date, "dd.MM.yyyy", null));

            addParameter<int>("@Task_ID", OleDbType.Integer, Convert.ToInt32(id));

            myCommand.CommandText = command;
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        //2.4 Delete an item
        public void deleteItem(string id, string type)
        {
            myCommand.CommandText = "DELETE FROM " + type + " WHERE " + type + "_ID=@id";
            addParameter<int>("@id", OleDbType.Integer, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            //Cascade deleting
            deleteAssignee(id, type);
        }

        //2.5.1 Update position of sticky note
        public void updatePosition(string id, string pos, string table)
        {
            myCommand.CommandText = "UPDATE " + table + " SET [Position]=@Position  WHERE " + table + "_ID=@id";
            addParameter<int>("@Position", OleDbType.Integer, Convert.ToInt32(pos));
            addParameter<int>("@id", OleDbType.Integer, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        //2.5.2 Change swimlane of sticky note
        public void changeSwimlane(string id, string pos, string table, string swimlane_id)
        {
            string status = getDataStatus(swimlane_id);
            myCommand.CommandText = "UPDATE " + table + " SET Swimlane_ID= @Swimlane_ID, [Position]=@Position, Status=@Status WHERE " + table + "_ID=@id";
            addParameter<int>("@Swimlane_ID", OleDbType.Integer, Convert.ToInt32(swimlane_id));
            addParameter<int>("@Position", OleDbType.Integer, Convert.ToInt32(pos));
            addParameter<string>("@Status", OleDbType.VarChar, status);
            addParameter<int>("@id", OleDbType.Integer, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            updateDate(id, table, status);
        }

        //2.5.3 Update date fields in Task and Backlog table based on the status of the item
        protected void updateDate(string id, string table, string status)
        {
            switch (status)
            {
                case "Standby":
                    if (table.Equals("Backlog"))
                    {
                        myCommand.CommandText = "UPDATE Backlog SET Start_date= @Start_date, Completion_date=@Completion_date WHERE Backlog_ID=@id";
                        addParameter<DBNull>("@Start_date", OleDbType.DBDate, DBNull.Value);
                    }
                    else
                    {
                        myCommand.CommandText = "UPDATE Task SET Completion_date=@Completion_date WHERE Task_ID=@id";
                    }
                    addParameter<DBNull>("@Completion_date", OleDbType.DBDate, DBNull.Value);
                    break;
                case "Ongoing":
                    myCommand.CommandText = "UPDATE Backlog SET Start_date= @Start_date WHERE Backlog_ID=@id";
                    addParameter<DateTime>("@Start_date", OleDbType.DBDate, DateTime.Now.Date);
                    break;
                case "Done":
                    myCommand.CommandText = "UPDATE "+table+" SET Completion_date= @Completion_date WHERE "+table+"_ID=@id";
                    addParameter<DateTime>("@Completion_date", OleDbType.DBDate, DateTime.Now.Date);
                    break;
            }
            addParameter<int>("@id", OleDbType.Integer, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        //2.6.1 Save assignee of a task or backlog
        public void saveAssignee(string id, string type, string uid, string name)
        {
            myCommand.CommandText = "INSERT INTO " + type + "_User (" + type + "_ID, User_ID, [Name])" +
                " VALUES (@id, @uid, @name)";
            addParameter<int>("@id", OleDbType.Integer, Convert.ToInt32(id));
            addParameter<int>("@uid", OleDbType.Integer, Convert.ToInt32(uid));
            addParameter<string>("@name", OleDbType.VarChar, name);
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        //2.6.2 Delete all assignee of a task or backlog
        public void deleteAssignee(string id, string type)
        {
            myCommand.CommandText = "DELETE FROM " + type + "_User WHERE " + type + "_ID=@id";
            addParameter<int>("@id", OleDbType.Integer, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        //2.7 View assignee of a task or backlog
        public string viewAssignee(string id, string type)
        {
            myCommand.CommandText = "SELECT * FROM " + type + "_User WHERE " + type + "_ID=@id";
            addParameter<int>("@id", OleDbType.Integer, Convert.ToInt32(id));

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
            myCommand.Parameters.Clear();
            return result.ToString();
        }

        //2.7.1 Helper 2.7
        protected string getAssigneeDisplay(string id, string name)
        {
            string display = "<div class='assignee-name-active' data-id='" + id + "' onclick='removeAssignee(this)'>" + name + "</div>";
            return display;
        }

        //a.1 Search member name in a project
        public string searchAssignee(int projectID, string keyword, string type)
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
        protected int countItem(int swimlane_id, string type)
        {
            myCommand.CommandText = "SELECT COUNT(*) FROM " + type + " WHERE Swimlane_ID=" + swimlane_id;
            return Convert.ToInt32(myCommand.ExecuteScalar());
        }


        //3. Query data for building chart
        // Fetch all necessary data to dataset for fast
        string[,] colorHex = {
                                {"#ff9898", "#ffc864", "#ffff95", "#98ff98", "#caffff", "#adadff", "#d598ff" },
                                { "#ff4b4b", "#ffa500", "#ffff4b", "#4bff4b", "#80ffff", "#6464ff", "#b64bff" }
                             };

        public string getPieChart(int projectID)
        {
            StringBuilder result = new StringBuilder("");
            myCommand.CommandText = "SELECT Count(*), [Name] FROM Task_User INNER JOIN " +
                "(SELECT Task_ID FROM Task INNER JOIN (SELECT Backlog_ID FROM Backlog WHERE Project_ID=@projectID AND Status='Ongoing') AS A ON Task.Backlog_ID = A.Backlog_ID) "+
                "AS B ON Task_User.Task_ID = B.Task_ID GROUP BY User_ID, [Name]";
            addParameter<int>("@projectID", OleDbType.Integer, projectID);
            myReader = myCommand.ExecuteReader();
            bool available = myReader.Read();
            int index = 0;
            StringWriter sw = new StringWriter(result);
            JsonTextWriter myWriter = new JsonTextWriter(sw);
            myWriter.WriteStartArray();
            while(available)
            {
                myWriter.WriteStartObject();
                // Number of tasks of a person in the sprint
                myWriter.WritePropertyName("value");
                myWriter.WriteValue(myReader.GetValue(0));

                // Color represent for the corresponding part in pie chart
                myWriter.WritePropertyName("color");
                myWriter.WriteValue(colorHex[1, index]);

                // Color when hovering on that part in pie chart
                myWriter.WritePropertyName("highlight");
                myWriter.WriteValue(colorHex[0, index]);

                // Name of the person
                myWriter.WritePropertyName("label");
                myWriter.WriteValue(myReader.GetValue(1));
                myWriter.WriteEndObject();
                
                available = myReader.Read();
                index++;
            }
            myWriter.WriteEndArray();
            myCommand.Parameters.Clear();
            return result.ToString();
        }
    }
}