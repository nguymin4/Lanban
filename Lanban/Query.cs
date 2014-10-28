using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Text;

namespace Lanban
{
    public class Query
    {
        string myConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Programming\Source code\ASP.NET\Lanban\Lanban\Lanban.accdb";
        private OleDbConnection myConnection;
        private OleDbCommand myCommand;
        private OleDbDataAdapter myAdapter;
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
        public void insertNewBacklog(string[] data)
        {
            StringBuilder command = new StringBuilder("INSERT INTO Backlog (Project_ID, Swimlane_ID," +
               "Title, Description, Complexity, Color, [Position]) VALUES (");
            for (int i = 0; i < data.Length - 1; i++)
                command.Append("'" + data[i] + "',");

            command.Append("'" + data[data.Length - 1] + "');");
            myCommand.CommandText = command.ToString();
            myCommand.ExecuteNonQuery();
        }

        //2.3 Update data
        public void updateTaskPosition(long id, int pos)
        {
        }
    }
}