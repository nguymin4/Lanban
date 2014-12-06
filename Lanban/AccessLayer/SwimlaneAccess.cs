using Lanban.Model;
using System;
using System.Data;
using System.Text;


namespace Lanban.AccessLayer
{
    /* Working with task comments */
    public class SwimlaneAccess: Query
    {
        // Fetch swimlane list
        public void fetchSwimlane(int projectID)
        {
            //Retrieve Swimlane data
            myCommand.CommandText = "SELECT * FROM Swimlane WHERE Project_ID = @projectID ORDER BY [Position]";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myAdapter.Fill(myDataSet, "Swimlane");
            myCommand.Parameters.Clear();
        }

        // Fill dataset with data from either Task table or Backlog table based on parameter
        public void fetchNote(string tableName, int projectID, int swimlaneID)
        {
            myCommand.CommandText = "SELECT * FROM " + tableName + " WHERE Project_ID=@projectID AND Swimlane_ID=@swimlaneID ORDER BY [Position]";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            addParameter<int>("@swimlaneID", SqlDbType.Int, swimlaneID);
            myAdapter.Fill(myDataSet, "init_temp");
            myCommand.Parameters.Clear();
        }

        // Fill dataset with data status = done from both Task table and Backlog table
        public void fetchDoneNote(int swimlaneID)
        {
            myCommand.CommandText = "SELECT Task_ID AS Item_ID, Task.Backlog_ID, Title, [Position], Color, Status, Relative_ID, 2 AS [Type] From Task " +
                "WHERE Swimlane_ID = @swimlaneID UNION ALL SELECT Backlog_ID, Null, Title, [Position], Color, Status, Relative_ID, 1 From Backlog " +
                "WHERE Swimlane_ID = @swimlaneID ORDER BY [Position]";
            addParameter<int>("@swimlaneID", SqlDbType.Int, swimlaneID);
            myAdapter.Fill(myDataSet, "init_temp");
            myCommand.Parameters.Clear();
        }

        // Add new swimlane to a project
        public string addSwimlane(Swimlane sw)
        {
            // Get position
            int position = countSwimlane(sw.Project_ID) - 1;
            
            StringBuilder command = new StringBuilder();
            command.Append("INSERT INTO Swimlane (Project_ID, Name, Type, Data_status, Position) ");
            command.Append("VALUES (@Project_ID, @Name, @Type, @Data_status, @Position);");
            addParameter<int>("@Project_ID", SqlDbType.Int, sw.Project_ID);
            addParameter<string>("@Name", SqlDbType.NVarChar, sw.Name);
            addParameter<int>("@Type", SqlDbType.Int, sw.Type);
            addParameter<string>("@Data_status", SqlDbType.VarChar, sw.Data_status);
            addParameter<int>("@Position", SqlDbType.Int, position);

            // Get the ID just inserted
            command.Append("SELECT SCOPE_IDENTITY();");
            myCommand.CommandText = command.ToString();
            string id = myCommand.ExecuteScalar().ToString();
            myCommand.Parameters.Clear();

            return id;
        }

        // Update swimlane information
        public int updateSwimlane(Swimlane sw, int projectID)
        {
            myCommand.CommandText = "UPDATE Swimlane SET Name=@name, Type=@type, Data_status=@data_status "+
                "WHERE Swimlane_ID = @swimlaneID AND Project_ID = @projectID";
            addParameter<string>("@name", SqlDbType.NVarChar, sw.Name);
            addParameter<int>("@type", SqlDbType.Int, sw.Type);
            addParameter<string>("@data_status", SqlDbType.VarChar, sw.Data_status);
            addParameter("@swimlaneID", SqlDbType.Int, sw.Swimlane_ID);
            addParameter<int>("@projectID", SqlDbType.Int, sw.Project_ID);
            int result = myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            return result;
        }

        // Delete swimlane
        public int deleteSwimlane(string swimlaneID, int projectID)
        {
            myCommand.CommandText = "DELETE FROM Swimlane WHERE Swimlane_ID=@swimlaneID AND Project_ID=@projectID";
            addParameter<int>("@swimlaneID", SqlDbType.Int, int.Parse(swimlaneID));
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            int result = myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            return result;
        }

        // Update position
        public void updatePositon(string swimlaneID, int projectID, string position)
        {
            myCommand.CommandText = "UPDATE Swimlane SET [Position]=@position WHERE Swimlane_ID=@swimlaneID AND Project_ID=@projectID";
            addParameter<int>("@position", SqlDbType.Int, int.Parse(position));
            addParameter<int>("@swimlaneID", SqlDbType.Int, int.Parse(swimlaneID));
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        // Count total number of swimlane in a project
        public int countSwimlane(int projectID)
        {
            myCommand.CommandText = "SELECT COUNT(*) FROM Swimlane WHERE Project_ID = @projectID";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            int result = Convert.ToInt32(myCommand.ExecuteScalar());
            myCommand.Parameters.Clear();
            return result;
        }
    }
}