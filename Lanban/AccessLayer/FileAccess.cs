using Lanban.Model;
using System;
using System.Data;
using System.Text;


namespace Lanban.AccessLayer
{
    /* Working with task comments */
    public class FileAccess: Query
    {
        // Check whether a file belongs to a task
        public bool IsInTask(string fileID, int taskID)
        {
            myCommand.CommandText = "SELECT COUNT(*) FROM Task_File WHERE File_ID = @fileID AND Task_ID = @taskID";
            addParameter<int>("@fileID", SqlDbType.Int, Convert.ToInt32(fileID));
            addParameter<int>("@taskID", SqlDbType.Int, taskID);
            bool result = (Convert.ToInt32(myCommand.ExecuteScalar()) == 1);
            myCommand.Parameters.Clear();
            return result;
        }

        // 2.9.1 Link the task to uploaded file
        public string linkTaskFile(FileModel file)
        {
            myCommand.CommandText = "INSERT INTO Task_File (Task_ID, User_ID, Name, Type, Path) " +
                                    "VALUES (@taskID, @userID, @name, @type, @path); SELECT SCOPE_IDENTITY();";
            addParameter<int>("@taskID", SqlDbType.Int, file.Task_ID);
            addParameter<int>("@userID", SqlDbType.Int, file.User_ID);
            addParameter<string>("@name", SqlDbType.NVarChar, file.Name);
            addParameter<string>("@type", SqlDbType.VarChar, file.Type);
            addParameter<string>("@path", SqlDbType.NVarChar, file.Path);
            file.File_ID = Convert.ToInt32(myCommand.ExecuteScalar());
            myCommand.Parameters.Clear();
            return getFileDisplay(file);
        }

        // 2.9.2 Delete a file of a task
        public void deleteTaskFile(int fileID, int userID)
        {
            myCommand.CommandText = "DELETE FROM Task_File WHERE File_ID=@fileID AND User_ID=@userID ";
            addParameter<int>("@fileID", SqlDbType.Int, fileID);
            addParameter<int>("@userID", SqlDbType.Int, userID);
            myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
        }

        // 2.9.3 View all files of a task
        public string viewTaskFile(int taskID)
        {
            myCommand.CommandText = "SELECT * FROM Task_File WHERE Task_ID=@id";
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(taskID));

            StringBuilder result = new StringBuilder();
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
            {
                FileModel file = SerializeTo<FileModel>(myReader);
                result.Append(getFileDisplay(file));
            }
            myReader.Close();
            myCommand.Parameters.Clear();
            return result.ToString();
        }

        // 2.9.a Get Visual display of a file
        public string getFileDisplay(FileModel file)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<div class='file-container' data-id=" + file.File_ID + " title='" + file.Name + "'>");
            result.Append("<a href='" + file.Path + "'>");
            result.Append("<img class='file-icon' src='images/files/" + file.Type + ".png' />");
            result.Append("<div class='file-name'>" + file.Name + "</div></a>");
            result.Append("<div class='file-remove' title='Delete' onclick=\"deleteFile(" + file.File_ID + ")\"></div></div>");
            return result.ToString();
        }

        // 2.9.b Get path of a file
        public string getFilePath(int fileID)
        {
            myCommand.CommandText = "SELECT Path FROM Task_File WHERE File_ID=@fileID";
            addParameter<int>("@fileID", SqlDbType.Int, fileID);
            string path = myCommand.ExecuteScalar().ToString();
            myCommand.Parameters.Clear();
            return path;
        }

        // Get Task ID of the File
        public int getTaskID(int fileID)
        {
            myCommand.CommandText = "SELECT Task_ID FROM Task_File WHERE File_ID = @fileID;";
            addParameter<int>("@fileID", SqlDbType.Int, Convert.ToInt32(fileID));
            int result = Convert.ToInt32(myCommand.ExecuteScalar());
            myCommand.Parameters.Clear();
            return result;
        }
    }
}