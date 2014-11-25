using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;

namespace Lanban
{
    public class Query
    {
        string myConnectionString = "Data Source=jpk6v577q2.database.windows.net;Initial Catalog=Lanban;Persist Security Info=True;User ID=nguymin4;Password=Lanban2014;Connect Timeout=30;Encrypt=True";
        protected SqlConnection myConnection;
        protected SqlCommand myCommand;
        protected SqlDataAdapter myAdapter;
        protected SqlDataReader myReader;
        protected DataSet myDataSet;

        public DataSet MyDataSet
        {
            get { return myDataSet; }
        }

        //1. Constructor
        public Query()
        {
            myConnection = new SqlConnection(myConnectionString);
            myCommand = new SqlCommand();
            myCommand.Connection = myConnection;
            myAdapter = new SqlDataAdapter(myCommand);
            myDataSet = new DataSet();
            myConnection.Open();
        }

        public void Dipose()
        {
            myConnection.Close();
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Add parameter for myCommand
        protected void addParameter<T>(string parameter, SqlDbType type, T value)
        {
            myCommand.Parameters.Add(parameter, type);
            myCommand.Parameters[parameter].Value = value;
        }
        
        //2.7.1 Helper 2.7
        protected string getAssigneeDisplay(string id, string name)
        {
            string display = "<div class='assignee-name-active' data-id='" + id + "' onclick='removeAssignee(this)'>" + name + "</div>";
            return display;
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

        //a.4 Get relative id of a type
        protected int getRelativeID(int project_id, string type)
        {
            myCommand.CommandText = "SELECT COUNT(*) FROM " + type + " WHERE Project_ID=" + project_id;
            return Convert.ToInt32(myCommand.ExecuteScalar());
        }

        //a.5 Get projectID of an item with itemID
        public int getProjectID(int itemID, string type)
        {
            myCommand.CommandText = "SELECT Project_ID FROM " + type + " WHERE " + type + "_ID = @itemID";
            addParameter<int>("@itemID", SqlDbType.Int, itemID);
            return Convert.ToInt32(myCommand.ExecuteScalar());
        }

        /*4. Login page */
        public UserModel login(string username, string password)
        {
            myCommand.CommandText = "SELECT * FROM Users WHERE Username = @username";
            addParameter<string>("@username", SqlDbType.VarChar, username);
            myReader = myCommand.ExecuteReader();
            bool available = myReader.Read();
            if (available == true)
            {
                if (password.Equals(myReader["Password"]))
                {
                    UserModel user = new UserModel();
                    user.User_ID = Convert.ToInt32(myReader["User_ID"]);
                    user.Username = myReader["Username"].ToString();
                    user.Name = myReader["Name"].ToString();
                    user.Role = Convert.ToInt32(myReader["Role"]);
                    user.Avatar = myReader["Avatar"].ToString();
                    return user;
                }
            }
            return null;
        }
        
    }
}
