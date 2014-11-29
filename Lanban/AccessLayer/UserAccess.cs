using Lanban.Model;
using System;
using System.Data;
using System.Text;


namespace Lanban.AccessLayer
{
    /* Working with task comments */
    public class UserAccess : Query
    {
        // Get User ID based on username
        public int getUserID(string username)
        {
            myCommand.CommandText = "SELECT User_ID FROM Users WHERE Username = @username";
            addParameter<string>("@username", SqlDbType.NVarChar, username);
            int result = Convert.ToInt32(myCommand.ExecuteScalar());
            myCommand.Parameters.Clear();
            return result;
        }

        // Get user data  based on username/id
        public UserModel getUserData<T>(T id)
        {
            dynamic uid = id;
            string type = uid.GetType().ToString();
            if (type.Contains("String"))
            {
                myCommand.CommandText = "SELECT * FROM Users WHERE Username = @username";
                addParameter<string>("@username", SqlDbType.NVarChar, uid);
            }
            else
            {
                myCommand.CommandText = "SELECT * FROM Users WHERE User_ID = @uid";
                addParameter<int>("@uid", SqlDbType.Int, uid);
            }
            
            UserModel user = null;
            myReader = myCommand.ExecuteReader();
            if (myReader.Read())
                user = SerializeTo<UserModel>(myReader);

            myReader.Close();
            myCommand.Parameters.Clear();
            return user;
        }

        //2.6.1 Save assignee/member of a task/backlog/project
        public void saveAssignee(string id, string type, string uid)
        {
            myCommand.CommandText = "INSERT INTO " + type + "_User (" + type + "_ID, User_ID) VALUES (@id, @uid)";
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(id));
            addParameter<int>("@uid", SqlDbType.Int, Convert.ToInt32(uid));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        //2.6.2 Delete all assignee/member of a task/backlog/project
        public void deleteAssignee(string id, string type)
        {
            myCommand.CommandText = "DELETE FROM " + type + "_User WHERE " + type + "_ID=@id";
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(id));
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        //2.7 View assignee/member of a task/backlog
        public string viewAssignee(string id, string type)
        {
            myCommand.CommandText = "SELECT Users.User_ID, Users.[Name], Avatar FROM Users INNER JOIN " +
                "(SELECT User_ID FROM " + type + "_User WHERE " + type + "_ID=@id) AS A ON A.User_ID = Users.User_ID";
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(id));

            StringBuilder result = new StringBuilder();
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
                result.Append(getAssigneeDisplay(myReader[0].ToString(), myReader[1].ToString()));
            
            myReader.Close();
            myCommand.Parameters.Clear();
            return result.ToString();
        }

        //a.1 Search member name in a project
        public string searchAssignee(int projectID, string keyword, string type)
        {
            myCommand.CommandText = "SELECT Users.User_ID, Name FROM Users INNER JOIN " +
                                    "(SELECT User_ID FROM Project_User WHERE Project_ID = " + projectID + ") AS A " +
                                    "ON A.User_ID = Users.User_ID WHERE Name LIKE '%" + keyword + "%'";
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

        // 5.8 Search user based on name and role
        public string searchUser(string name, int role)
        {
            myCommand.CommandText = "SELECT TOP 3 User_ID, Name, Avatar FROM Users WHERE Role=@role AND Name LIKE '%" + name + "%'";
            addParameter<int>("@role", SqlDbType.Int, role);
            myReader = myCommand.ExecuteReader();
            StringBuilder result = new StringBuilder();
            while (myReader.Read())
            {
                result.Append("<div class='searchRecord' data-id='" + myReader["User_ID"] + "' ");
                result.Append("data-avatar='" + myReader["Avatar"] + "'>" + myReader["Name"] + "</div>");
            }
            if (result.ToString().Equals("")) return "No records found.";
            return result.ToString();
        }

        // 5.9.1 Save supervisor
        public void saveSupervisor(int projectID, int supervisorID)
        {
            myCommand.CommandText = "INSERT INTO Project_Supervisor (Project_ID, User_ID) VALUES (@projectID, @supervisorID)";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            addParameter<int>("@supervisorID", SqlDbType.Int, supervisorID);
            myCommand.ExecuteNonQuery();
        }

        // 5.9.2 Edit supervisor
        public void deleteSupervisor(int projectID)
        {
            myCommand.CommandText = "DELETE FROM Project_Supervisor WHERE Project_ID = @projectID";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myCommand.ExecuteNonQuery();
        }

        // 5.7 A member quit project
        public bool quitProject(int projectID, int userID, int role)
        {
            string table = (role == 1) ? "Project_User" : "Project_Supervisor";
            myCommand.CommandText = "DELETE FROM " + table + " WHERE Project_ID=@projectID AND User_ID=@userID";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            addParameter<int>("@userID", SqlDbType.Int, userID);
            bool result = (myCommand.ExecuteNonQuery() == 1);
            myCommand.Parameters.Clear();
            return result;
        }
    }
}