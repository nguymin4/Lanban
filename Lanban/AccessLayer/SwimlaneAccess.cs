using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;


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
    }
}