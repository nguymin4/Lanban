using Lanban.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace Lanban.AccessLayer
{
    public class ProjectAccess : Query
    {
        // 5.1 Fetch project list that a user has been joining 
        public void fetchProject(int userID, int role)
        {
            string table = (role == 1) ? "Project_User" : "Project_Supervisor";
            myCommand.CommandText = "SELECT Project.* FROM Project INNER JOIN " +
                                    "(SELECT Project_ID FROM " + table + " WHERE User_ID=@userID) AS A " +
                                    "ON A.Project_ID = Project.Project_ID ORDER BY Project.Start_Date DESC;";
            addParameter<int>("@userID", SqlDbType.Int, userID);
            myAdapter.Fill(myDataSet, "Project");
            myCommand.Parameters.Clear();
        }

        // 5.2 Fetch user list of those who share the same projects with user has [userID]
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
                result.Append(getPersonContainer(myReader, false));
            }
            myCommand.Parameters.Clear();
            return result.ToString();
        }

        // Fetch all user of a project
        public string fetchUser(int projectID, int userID)
        {
            bool owner = IsOwner(projectID, userID);

            StringBuilder result = new StringBuilder();
            myCommand.CommandText = "SELECT Users.User_ID, [Name], Avatar FROM Users INNER JOIN " +
                "(SELECT User_ID FROM Project_User WHERE Project_ID = @projectID) AS A ON A.User_ID = Users.User_ID";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
            {
                result.Append(getPersonContainer(myReader, owner, projectID));
                string temp = myReader["User_ID"].ToString();
            }
            myReader.Close();
            myCommand.Parameters.Clear();
            return result.ToString();
        }

        // Check whether a user is owner of a project
        public bool IsOwner(int projectID, int userID)
        {
            myCommand.CommandText = "IF EXISTS (SELECT Project_ID FROM Project WHERE Project_ID=@projectID AND Owner=@userID) SELECT 1 ELSE SELECT 0";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            addParameter<int>("@userID", SqlDbType.Int, userID);
            bool result = (Convert.ToInt32(myCommand.ExecuteScalar()) == 1);
            myCommand.Parameters.Clear();
            return result;
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
        public bool deleteProject(int projectID, int userID)
        {
            myCommand.CommandText = "DELETE FROM Project WHERE Project_ID=@projectID AND Owner=@userID;";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            addParameter<int>("@userID", SqlDbType.Int, userID);
            bool result = (myCommand.ExecuteNonQuery() == 1);
            myCommand.Parameters.Clear();
            return result;
        }

        // 5.6 Update project
        public bool updateProject(ProjectModel project, int userID)
        {
            myCommand.CommandText = "UPDATE Project SET Name = @name, Description = @description, " +
                "Start_Date = @startDate WHERE Project_ID = @projectID AND Owner=@userID";
            addParameter<string>("@name", SqlDbType.NVarChar, project.Name);
            addParameter<string>("@description", SqlDbType.NVarChar, project.Description);
            try { addParameter<DateTime>("@startDate", SqlDbType.DateTime2, DateTime.ParseExact(project.Start_Date, "dd.MM.yyyy", null)); }
            catch { addParameter<DBNull>("@startDate", SqlDbType.DateTime2, DBNull.Value); }
            addParameter("@projectID", SqlDbType.Int, project.Project_ID);
            addParameter("@userID", SqlDbType.Int, userID);
            bool result = (myCommand.ExecuteNonQuery() == 1);
            myCommand.Parameters.Clear();
            return result;
        }

        // 5.7 Kick a user
        public void kickUser(int projectID, int uid)
        {
            myCommand.CommandText = "DELETE FROM Project_User WHERE Project_ID=@projectID AND User_ID=@uid;";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            addParameter<int>("@uid", SqlDbType.Int, uid);
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        // Get Supervisor and User ID list 
        public List<int> getProjectMemberID(int projectID)
        {
            List<int> IDs = new List<int>();
            myCommand.CommandText = "SELECT User_ID FROM Project_User WHERE Project_ID = @projectID UNION " +
                "SELECT User_ID FROM Project_Supervisor WHERE Project_ID = @projectID ";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
                IDs.Add(Convert.ToInt32(myReader[0]));
            myReader.Close();
            myCommand.Parameters.Clear();
            return IDs;
        }

        // Get Project Data
        public Model.ProjectModel getProjectData(int projectID)
        {
            myCommand.CommandText = "SELECT * FROM Project WHERE Project_ID=@projectID";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myReader = myCommand.ExecuteReader();
            myReader.Read();
            var projectData = SerializeTo<ProjectModel>(myReader);
            myReader.Close();
            return projectData;
        }
    }
} 