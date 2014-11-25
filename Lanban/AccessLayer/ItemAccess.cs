using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;


namespace Lanban.AccessLayer
{
    /* Working with task comments */
    public class ItemAccess: Query
    {
        //2.2.0 Get all information of an item Backlog/Task based on id
        public string viewItem(string id, string type, int projectID)
        {
            myCommand.CommandText = "SELECT * FROM " + type + " WHERE " + type + "_ID=@id AND Project_ID = @projectID";
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(id));
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myAdapter.Fill(myDataSet, "Item");
            myCommand.Parameters.Clear();
            return JsonConvert.SerializeObject(myDataSet.Tables["Item"]);
        }

        //2.2.1 Insert new backlog
        public string insertNewBacklog(Backlog backlog)
        {
            StringBuilder command = new StringBuilder();
            command.Append("INSERT INTO Backlog (Project_ID, Swimlane_ID, Title, Description, Complexity, Color) ");
            command.Append("VALUES (@Project_ID, @Swimlane_ID, @Title, @Description, @Complexity, @Color);");
            addParameter<int>("@Project_ID", SqlDbType.Int, backlog.Project_ID);
            addParameter<int>("@Swimlane_ID", SqlDbType.Int, backlog.Swimlane_ID);
            addParameter<string>("@Title", SqlDbType.Text, backlog.Title);
            addParameter<string>("@Description", SqlDbType.Text, backlog.Description);
            addParameter<int>("@Complexity", SqlDbType.Int, backlog.Complexity);
            addParameter<string>("@Color", SqlDbType.VarChar, backlog.Color);

            // Get the ID just inserted
            command.Append("SELECT SCOPE_IDENTITY();");
            myCommand.CommandText = command.ToString();
            string id = myCommand.ExecuteScalar().ToString();
            myCommand.Parameters.Clear();

            // Update data in some field of just inserted row
            string status = getDataStatus(backlog.Swimlane_ID.ToString());
            int position = countItem(backlog.Swimlane_ID, "Backlog") - 1;
            int relativeID = getRelativeID(backlog.Project_ID, "Backlog");
            myCommand.CommandText = "UPDATE Backlog SET [Status]=@Status, [Position]=@Position, Relative_ID=@Relative_ID WHERE Backlog_ID=@id";
            addParameter<string>("@Status", SqlDbType.VarChar, status);
            addParameter<int>("@Position", SqlDbType.Int, position);
            addParameter<int>("@Relative_ID", SqlDbType.Int, relativeID);
            addParameter("@id", SqlDbType.Int, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();

            // Upate the date fields based on item status
            updateDate(id, "Backlog", status);
            return id + "." + relativeID.ToString();
        }

        //2.2.2 Insert new task
        public string insertNewTask(Task task)
        {
            StringBuilder command = new StringBuilder();
            command.Append("INSERT INTO Task (Project_ID, Swimlane_ID, Backlog_ID, Title, ");
            command.Append("Description, Color, Work_estimation, Due_date) ");
            command.Append("VALUES (@Project_ID, @Swimlane_ID, @Backlog_ID, @Title, ");
            command.Append("@Description, @Color, @Work_estimation, @Due_date);");

            addParameter<int>("@Project_ID", SqlDbType.Int, task.Project_ID);
            addParameter<int>("@Swimlane_ID", SqlDbType.Int, task.Swimlane_ID);
            addParameter<int>("@Backlog_ID", SqlDbType.Int, task.Backlog_ID);
            addParameter<string>("@Title", SqlDbType.Text, task.Title);
            addParameter<string>("@Description", SqlDbType.Text, task.Description);
            addParameter<string>("@Color", SqlDbType.VarChar, task.Color);

            // Work estimation can be null
            if (task.Work_estimation == null) addParameter<DBNull>("@Work_estimation", SqlDbType.Int, DBNull.Value);
            else addParameter("@Work_estimation", SqlDbType.Int, task.Work_estimation);

            // Due date is nullable field - the user can skip input data of that field
            try { addParameter<DateTime>("@Due_date", SqlDbType.DateTime2, DateTime.ParseExact(task.Due_date, "dd.MM.yyyy", null)); }
            catch { addParameter<DBNull>("@Due_date", SqlDbType.DateTime2, DBNull.Value); }

            // Get the ID just inserted
            command.Append("SELECT SCOPE_IDENTITY();");
            myCommand.CommandText = command.ToString();
            string id = myCommand.ExecuteScalar().ToString();
            myCommand.Parameters.Clear();

            // Update data in some field of just inserted row
            string status = getDataStatus(task.Swimlane_ID.ToString());
            int position = countItem(task.Swimlane_ID, "Task") - 1;
            int relativeID = getRelativeID(task.Project_ID, "Task");
            myCommand.CommandText = "UPDATE Task SET Status=@Status, [Position]=@Position, Relative_ID=@Relative_ID WHERE Task_ID=@id";
            addParameter<string>("@Status", SqlDbType.VarChar, status);
            addParameter<int>("@Position", SqlDbType.Int, position);
            addParameter<int>("@Relative_ID", SqlDbType.Int, relativeID);
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();

            // Upate the date fields based on item status
            updateDate(id, "Task", status);
            return id + "." + relativeID.ToString();
        }

        //2.3.1 Save editted data of a backlog
        public int updateBacklog(Backlog backlog, int projectID)
        {
            string command = "UPDATE Backlog SET Title=@Title, Description=@Description, " +
                "Complexity=@Complexity, Color=@Color WHERE Backlog_ID=@Backlog_ID AND Project_ID = @projectID";

            addParameter<string>("@Title", SqlDbType.Text, backlog.Title);
            addParameter<string>("@Description", SqlDbType.Text, backlog.Description);
            addParameter<int>("@Complexity", SqlDbType.Int, backlog.Complexity);
            addParameter<string>("@Color", SqlDbType.VarChar, backlog.Color);
            addParameter("@Backlog_ID", SqlDbType.Int, backlog.Backlog_ID);
            addParameter<int>("@projectID", SqlDbType.Int, projectID);

            myCommand.CommandText = command;
            int result = myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            return result;
        }

        //2.3.2 save editted data of a task
        public int updateTask(Task task, int projectID)
        {
            string command = "UPDATE Task SET Backlog_ID=@Backlog_ID, Title=@Title, Description=@Description, " +
                "Work_estimation=@Work_estimation, Color=@Color, Due_date=@Due_date WHERE Task_ID=@Task_ID";

            addParameter<int>("@Backlog_ID", SqlDbType.Int, task.Backlog_ID);
            addParameter<string>("@Title", SqlDbType.Text, task.Title);
            addParameter<string>("@Description", SqlDbType.Text, task.Description);

            // Work estimation can be null
            if (task.Work_estimation == null) addParameter<DBNull>("@Work_estimation", SqlDbType.Int, DBNull.Value);
            else addParameter("@Work_estimation", SqlDbType.Int, task.Work_estimation);

            addParameter<string>("@Color", SqlDbType.VarChar, task.Color);

            // Due date is nullable field - the user can skip input data of that field
            try { addParameter<DateTime>("@Due_date", SqlDbType.DateTime2, DateTime.ParseExact(task.Due_date, "dd.MM.yyyy", null)); }
            catch { addParameter<DBNull>("@Due_date", SqlDbType.DateTime2, DBNull.Value); }

            addParameter("@Task_ID", SqlDbType.Int, task.Task_ID);
            addParameter<int>("@projectID", SqlDbType.Int, projectID);

            myCommand.CommandText = command;
            int result = myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            return result;
        }

        //2.4 Delete an item
        public int deleteItem(string id, string type, int projectID)
        {
            myCommand.CommandText = "DELETE FROM " + type + " WHERE " + type + "_ID=@id AND Project_ID=@projectID";
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(id));
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            int result = myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            return result;
        }

        //2.5.1 Update position of sticky note
        public void updatePosition(string id, string pos, string table)
        {
            myCommand.CommandText = "UPDATE " + table + " SET [Position]=@Position  WHERE " + table + "_ID=@id";
            addParameter<int>("@Position", SqlDbType.Int, Convert.ToInt32(pos));
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        //2.5.2 Change swimlane of sticky note
        public void changeSwimlane(string id, string pos, string table, string swimlane_id)
        {
            string status = getDataStatus(swimlane_id);
            myCommand.CommandText = "UPDATE " + table + " SET Swimlane_ID= @Swimlane_ID, [Position]=@Position, Status=@Status WHERE " + table + "_ID=@id";
            addParameter<int>("@Swimlane_ID", SqlDbType.Int, Convert.ToInt32(swimlane_id));
            addParameter<int>("@Position", SqlDbType.Int, Convert.ToInt32(pos));
            addParameter<string>("@Status", SqlDbType.VarChar, status);
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(id));
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
                        addParameter<DBNull>("@Start_date", SqlDbType.DateTime2, DBNull.Value);
                    }
                    else
                        myCommand.CommandText = "UPDATE Task SET Completion_date=@Completion_date WHERE Task_ID=@id";
                    addParameter<DBNull>("@Completion_date", SqlDbType.DateTime2, DBNull.Value);
                    break;
                case "Ongoing":
                    if (table.Equals("Backlog"))
                    {
                        myCommand.CommandText = "UPDATE Backlog SET Start_date=@Start_date, Completion_date=@Completion_date WHERE Backlog_ID=@id";
                        addParameter<DateTime>("@Start_date", SqlDbType.DateTime2, DateTime.Now.Date);
                    }
                    else
                        myCommand.CommandText = "UPDATE Task SET Completion_date=@Completion_date WHERE Task_ID=@id";
                    addParameter<DBNull>("@Completion_date", SqlDbType.DateTime2, DBNull.Value);
                    break;
                case "Done":
                    myCommand.CommandText = "UPDATE " + table + " SET Completion_date= @Completion_date WHERE " + table + "_ID=@id";
                    addParameter<DateTime>("@Completion_date", SqlDbType.DateTime2, DateTime.Now.Date);
                    break;
            }
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }
    }
}