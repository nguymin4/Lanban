using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Lanban
{
    public class Query
    {
        string myConnectionString = Environment.GetEnvironmentVariable("SQLCONNSTR_Lanban");
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
            myDataSet.Dispose();
            myAdapter.Dispose();
            myCommand.Dispose();
            myConnection.Close();
            myConnection.Dispose();
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Add parameter for myCommand
        protected void addParameter<T>(string parameter, SqlDbType type, T value)
        {
            myCommand.Parameters.Add(parameter, type);
            myCommand.Parameters[parameter].Value = value;
        }

        // Get the visual display of an User object
        public string getPersonContainer<T>(T obj, bool removeable, int projectID = 0)
        {
            StringBuilder result = new StringBuilder();
            string id = "", name = "", avatar = "";
            dynamic reader = obj;
            string type = obj.GetType().ToString();
            if (obj.GetType().ToString().Contains("SqlDataReader"))
            {
                id = reader["User_ID"].ToString();
                name = reader["Name"].ToString();
                avatar = reader["avatar"].ToString();
            }
            else
            {
                id = reader.User_ID.ToString();
                name = reader.Name;
                avatar = reader.Avatar;
            }
            result.Append("<div class='person' data-id='" + id + "' title='" + name + "'>");
            result.Append("<img class='person-avatar' src='" + avatar + "' />");
            result.Append("<div class='person-name'>" + name + "</div>");
            if (removeable) result.Append("<div class='person-remove' title='Remove' onclick=\"removeMember(this.parentElement," + projectID + ")\"></div>");
            result.Append("</div>");
            return result.ToString();
        }

        // Get Data_status field of a swimlane in Swimlane table
        protected string getDataStatus(string swimlane_id)
        {
            myCommand.CommandText = "SELECT Data_status FROM Swimlane WHERE Swimlane_ID=" + swimlane_id;
            return myCommand.ExecuteScalar().ToString();
        }

        // Get number of item in a swimlane
        protected int countItem(int swimlane_id, string type)
        {
            myCommand.CommandText = "SELECT COUNT(*) FROM " + type + " WHERE Swimlane_ID=" + swimlane_id;
            return Convert.ToInt32(myCommand.ExecuteScalar());
        }

        // Get relative id of a type
        protected int getRelativeID(int project_id, string type)
        {
            myCommand.CommandText = "SELECT COUNT(*) FROM " + type + " WHERE Project_ID=" + project_id;
            return Convert.ToInt32(myCommand.ExecuteScalar());
        }

        // Get projectID of an item with itemID
        public int getProjectID(int itemID, string type)
        {
            myCommand.CommandText = "SELECT Project_ID FROM " + type + " WHERE " + type + "_ID = @itemID";
            addParameter<int>("@itemID", SqlDbType.Int, itemID);
            return Convert.ToInt32(myCommand.ExecuteScalar());
        }

        // Check whether the item with itemID belongs to the project
        public bool IsInProject(int projectID, int itemID, string type)
        {
            myCommand.CommandText = "IF EXISTS (SELECT 1 FROM " + type + " WHERE Project_ID=@projectID " +
                "AND " + type + "_ID=@itemID) SELECT 1 ELSE SELECT 0";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            addParameter<int>("@itemID", SqlDbType.Int, itemID);
            bool result = (Convert.ToInt32(myCommand.ExecuteScalar()) == 1);
            myCommand.Parameters.Clear();
            return result;
        }

        // Check whether the user is authorized to watch and work on that project
        public bool IsProjectMember(int projectID, int userID, int role)
        {
            string table = (role == 1) ? "Project_User" : "Project_Supervisor";
            myCommand.CommandText = "IF EXISTS (SELECT 1 FROM " + table + " WHERE Project_ID=@projectID AND "+
                "User_ID=@userID) SELECT 1 ELSE SELECT 0";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            addParameter<int>("@userID", SqlDbType.Int, userID);
            bool result = (Convert.ToInt32(myCommand.ExecuteScalar()) == 1);
            myCommand.Parameters.Clear();
            return result;
        }

        // Convert a reader to Data model
        public T SerializeTo<T>(SqlDataReader reader) {
            var result = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
                result.Add(reader.GetName(i), reader.GetValue(i));
            
            var temp = JsonConvert.SerializeObject(result);
            return JsonConvert.DeserializeObject<T>(temp);
        }

        /*4. Login page */
        public UserModel login(string username, string password)
        {
            myCommand.CommandText = "SELECT * FROM Users WHERE Username = @username";
            addParameter<string>("@username", SqlDbType.VarChar, username);
            myReader = myCommand.ExecuteReader();
            UserModel user = null;
            if (myReader.Read())
            {
                if (password.Equals(myReader["Password"]))
                    user = SerializeTo<UserModel>(myReader);
                myReader.Close();
            }
            return user;
        }
    }
}
