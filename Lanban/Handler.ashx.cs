using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Text;

namespace Lanban
{
    /// <summary>
    /// Summary description for Handler
    /// </summary>
    public class Handler : IHttpHandler
    {
        string myConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Programming\Source code\ASP.NET\Lanban\Lanban\Lanban.accdb";
        OleDbConnection myConnection;
        OleDbCommand myCommand;
        OleDbDataReader myReader;
 
        public void ProcessRequest(HttpContext context)
        {
            myConnection = new OleDbConnection(myConnectionString);
            myCommand = new OleDbCommand();
            myCommand.Connection = myConnection;
            myCommand.CommandText = "SELECT TOP 3 User_ID, Name FROM Users WHERE Name LIKE '" +
                                    context.Request.Params[1] + "%'";

            StringBuilder result = new StringBuilder();
            myConnection.Open();
            myReader = myCommand.ExecuteReader();
            context.Response.ContentType = "text/plain";
            bool available = myReader.Read();
            if (available == false) result.Append("No record found.");
            else
            {
                while (available)
                {
                    result.Append(getDataDisplay(myReader[0].ToString(), myReader[1].ToString(), context.Request.Params[0]));
                    available = myReader.Read();
                }
            }
            context.Response.Write(result.ToString());
            myConnection.Close();
        }

        protected string getDataDisplay(string ID, string name, string type)
        {
            string display = "<div class='resultline' data-id='" + ID + "' onclick=\"addAssignee(this,'" + type + "')\">" + name + "</div>";
            return display;
        }
        

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}