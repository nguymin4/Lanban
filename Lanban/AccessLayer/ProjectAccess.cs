using Lanban.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace Lanban.AccessLayer
{
    /* Working with task comments */
    public class ProjectAccess: Query
    {
        /*5. Project page */
        // 5.1 Fetch project list that a user has been joining 
        public void fetchProject(int userID, int role)
        {
            string table = (role == 1) ? "Project_User" : "Project_Supervisor";
            myCommand.CommandText = "SELECT Project.* FROM Project INNER JOIN " +
                                    "(SELECT Project_ID FROM " + table + " WHERE User_ID=@userID) AS A " +
                                    "ON A.Project_ID = Project.Project_ID;";
            addParameter<int>("@userID", SqlDbType.Int, userID);
            myAdapter.Fill(myDataSet, "Project");
            myCommand.Parameters.Clear();
        }

        // 5.2 Fetch user list of those who share the same project with user has [userID]
        public void fetchSharedProjectUser(int userID)
        {
            myCommand.CommandText = "SELECT B.User_ID, [Name], Avatar FROM Users INNER JOIN " +
                                    "(SELECT DISTINCT User_ID FROM Project_User " +
                                    "INNER JOIN (SELECT Project_ID FROM Project_User WHERE User_ID = @userID) AS A " +
                                    "ON A.Project_ID = Project_User.Project_ID) AS B ON B.User_ID = Users.User_ID";
            addParameter<int>("@userID", SqlDbType.Int, userID);
            myAdapter.Fill(myDataSet, "User");
            myCommand.Parameters.Clear();
        }

        // 5.3 Fetch all supervisor of a project
        public string fetchSupervisor(int projectID)
        {
            StringBuilder result = new StringBuilder();
            myCommand.CommandText = "SELECT Users.User_ID, [Name], Avatar FROM Users INNER JOIN " +
                "(SELECT User_ID FROM Project_Supervisor WHERE Project_ID = @projectID) AS A ON A.User_ID = Users.User_ID";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
            {
                result.Append("<div class='person' data-id='" + myReader["User_ID"] + "' title='" + myReader["Name"] + "'>");
                result.Append("<img class='person-avatar' src='" + myReader["Avatar"] + "' />");
                result.Append("<div class='person-name'>" + myReader["Name"] + "</div></div>");
            }
            return result.ToString();
        }

        // 5.4 Create new project
        public string createProject(ProjectModel project)
        {
            myCommand.CommandText = "INSERT INTO Project (Name, Description, Owner, Start_Date) VALUES (@Name, @Description, @Owner, @Start_Date);";
            addParameter<string>("@Name", SqlDbType.NVarChar, project.Name);
            addParameter<string>("@Description", SqlDbType.NVarChar, project.Description);
            addParameter<int>("@Owner", SqlDbType.Int, project.Owner);
            try { addParameter<DateTime>("@Start_Date", SqlDbType.DateTime2, DateTime.ParseExact(project.Start_Date, "dd.MM.yyyy", null)); }
            catch { addParameter<DBNull>("@Start_Date", SqlDbType.DateTime2, DBNull.Value); }
            myCommand.CommandText += "SELECT SCOPE_IDENTITY();";

            // In database trigger:
            // 1. Create default 5 swimlanes for created project
            // 2. Add owner as one of the user of the created project

            return myCommand.ExecuteScalar().ToString();
        }

        // 5.5 Delete project
        public void deleteProject(int projectID)
        {
            myCommand.CommandText = "DELETE FROM Project WHERE Project_ID=@projectID;";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myCommand.ExecuteNonQuery();
        }

        // 5.6 Update project
        public void updateProject(ProjectModel project)
        {
            myCommand.CommandText = "UPDATE Project SET Name = @name, Description = @description, " +
                "Start_Date = @startDate WHERE Project_ID = @projectID";
            addParameter<string>("@name", SqlDbType.NVarChar, project.Name);
            addParameter<string>("@description", SqlDbType.NVarChar, project.Description);
            try { addParameter<DateTime>("@startDate", SqlDbType.DateTime2, DateTime.ParseExact(project.Start_Date, "dd.MM.yyyy", null)); }
            catch { addParameter<DBNull>("@startDate", SqlDbType.DateTime2, DBNull.Value); }
            addParameter("@projectID", SqlDbType.Int, project.Project_ID);
            myCommand.ExecuteNonQuery();
        }

        // Get Supervisor id list 
        public List<int> getProjectMemberID(int projectID)
        {
            List<int> IDs = new List<int>();
            myCommand.CommandText = "SELECT User_ID FROM Project_User WHERE Project_ID = @projectID UNION " +
                "SELECT User_ID FROM Project_Supervisor WHERE Project_ID = @projectID ";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
                IDs.Add(Convert.ToInt32(myReader[0]));
            return IDs;
        }
    }
}