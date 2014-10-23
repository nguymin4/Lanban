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
        }

        public void fetchDataForBoard(int projectID)
        {
            myConnection.Open();
            myDataSet = new DataSet();
            myAdapter = new OleDbDataAdapter(myCommand);
            myCommand.CommandText = "SELECT * FROM Swimlane WHERE Project_ID=" + projectID;
            myAdapter.Fill(myDataSet,"Swimlane");
            myDataSet.Tables["Swimlane"].DefaultView.Sort = "Position";
           
            myCommand.CommandText = "SELECT * FROM Task WHERE Project_ID=" + projectID;
            myAdapter.Fill(myDataSet, "Task");
            myDataSet.Tables["Task"].DefaultView.Sort = "Swimlane, Position";
            myCommand.CommandText = "SELECT * FROM BacklogItem WHERE Project_ID=" + projectID;
            myAdapter.Fill(myDataSet, "BacklogItem");
            myDataSet.Tables["BacklogItem"].DefaultView.Sort = "Swimlane, Position";
        }

        public void updateTaskPosition(long id, int pos)
        {
            myCommand.CommandText = "";
        }
    }
}