using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Data;

namespace Lanban
{
    public class Query
    {
        string myConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Programming\Source code\ASP.NET\Lanban\Lanban\Lanban.accdb";
        public OleDbConnection myConnection;
        public OleDbCommand myCommand;
        public OleDbDataAdapter myAdapter;
        public DataSet myDataSet;

        public Query()
        {
            myConnection = new OleDbConnection(myConnectionString);
            myConnection.BeginTransaction(IsolationLevel.Serializable);
            myCommand = new OleDbCommand();
            myCommand.Connection = myConnection;
            myConnection.Open();
        }

        public void fetchDataForBoard(int projectID)
        {
            myDataSet = new DataSet();
            myAdapter = new OleDbDataAdapter(myCommand);
            
            //Retrieve Swimlane data
            myCommand.CommandText = "SELECT * FROM Swimlane WHERE Project_ID=" + projectID;
            myAdapter.Fill(myDataSet,"Swimlane");
            myDataSet.Tables["Swimlane"].DefaultView.Sort = "Position";
           
            //Retreive Task data
            myCommand.CommandText = "SELECT * FROM Task WHERE Project_ID=" + projectID;
            myAdapter.Fill(myDataSet, "Task");
            myDataSet.Tables["Task"].DefaultView.Sort = "Swimlane, Position";

            //Retrieve Backlog item
            myCommand.CommandText = "SELECT * FROM BacklogItem WHERE Project_ID=" + projectID;
            myAdapter.Fill(myDataSet, "BacklogItem");
            myDataSet.Tables["BacklogItem"].DefaultView.Sort = "Swimlane, Position";
        }

        public void insertNewBacklog(string[] data)
        {
            string command = @"INSERT INTO Backlog (Project_ID, Swimlane_ID," +
                "Title, Description, Complexity, Color, Position)" +
                " VALUES (";
            for (int i = 0; i < data.Length; i++)
            {
                command += "'" + data[i] + "',";
            }
            myCommand.CommandText = command + ")";
            myCommand.ExecuteNonQuery();
        }

        public void updateTaskPosition(long id, int pos)
        {
            myCommand.CommandText = "";
        }
    }
}