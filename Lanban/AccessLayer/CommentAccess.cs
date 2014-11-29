using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Lanban.Model;


namespace Lanban.AccessLayer
{
    /* Working with task comments */
    public class CommentAccess : Query
    {
        /* Working with task comments */
        // View all comments of a task
        public string viewTaskComment(string taskID, int userID)
        {
            myCommand.CommandText = "SELECT Comment_ID, A.User_ID, Content, Name, Avatar FROM " +
                "(SELECT * FROM Task_Comment WHERE Task_ID=@id) AS A INNER JOIN Users ON A.User_ID = Users.User_ID";
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(taskID));

            StringBuilder result = new StringBuilder();
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
            {
                var comment = SerializeTo<Comment>(myReader);
                result.Append(getCommentDisplay(userID, comment));
            }
            myReader.Close();
            myCommand.Parameters.Clear();
            return result.ToString();
        }

        // Delete a comment
        public string deleteTaskComment(string commentID, int userID)
        {
            myCommand.CommandText = "DELETE FROM Task_Comment WHERE Comment_ID=@id AND User_ID = @userID";
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(commentID));
            addParameter<int>("@userID", SqlDbType.Int, userID);
            int result = myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            if (result == 1) return "Success";
            else return "";
        }

        // Edit a comment
        public string updateTaskComment(string commentID, string content, int userID)
        {
            myCommand.CommandText = "UPDATE Task_Comment SET Content=@content WHERE Comment_ID=@id AND User_ID = @userID";
            addParameter<string>("@content", SqlDbType.Text, content);
            addParameter<int>("@id", SqlDbType.Int, Convert.ToInt32(commentID));
            addParameter<int>("@userID", SqlDbType.Int, userID);
            int result = myCommand.ExecuteNonQuery();
            myCommand.Parameters.Clear();
            if (result == 1) return "Success";
            else return "";
        }

        // Insert new comment
        public string insertTaskComment(string taskID, string content, int userID)
        {
            myCommand.CommandText = "INSERT INTO Task_Comment (Task_ID, Content, User_ID) VALUES(@taskID, @content, @userID);" +
                                    "SELECT SCOPE_IDENTITY();";
            addParameter<int>("@taskID", SqlDbType.Int, Convert.ToInt32(taskID));
            addParameter<string>("@content", SqlDbType.Text, content);
            addParameter<int>("@userID", SqlDbType.Int, userID);
            string id = myCommand.ExecuteScalar().ToString();
            myCommand.Parameters.Clear();
            return id;
        }

        // Get comment visual object
        protected string getCommentDisplay(int userID, Comment comment)
        {
            int id = comment.Comment_ID;
            int owner = comment.User_ID;
            string content = comment.Content;
            content = Regex.Replace(content, @"\r\n?|\n", "<br />");
            StringBuilder result = new StringBuilder();

            if (owner == userID) result.Append("<div class='comment-box' id='comment." + id + "'><div class='comment-panel'>");
            else result.Append("<div class='comment-box'><div class='comment-panel'>");
            result.Append("<img class='comment-profile' src='" + comment.Avatar + "' title='" + comment.Name + "' /></div>");
            result.Append("<div class='comment-container'><div class='comment-content'>" + content + "</div>");
            if (owner == userID)
            {
                result.Append("<div class='comment-footer'>");
                result.Append("<div class='comment-button' title='Edit comment' onclick='fetchTaskComment(" + id + ")'></div>");
                result.Append("<div class='comment-button' title='Delete comment' onclick='deleteTaskComment(" + id + ")'></div>");
                result.Append("</div>");
            }
            result.Append("</div></div>");
            return result.ToString();
        }
    }
}