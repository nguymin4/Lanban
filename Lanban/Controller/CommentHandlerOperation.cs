using Lanban.AccessLayer;
using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Lanban
{
    public class CommentHandlerOperation : HandlerOperation
    {
        CommentAccess myAccess;

        public CommentHandlerOperation(AsyncCallback callback, HttpContext context, Object state)
            : base(callback, context, state)
        {
            myAccess = new CommentAccess();
        }

        public void QueueWork()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartTask), null);
        }

        private void StartTask(Object workItemState)
        {
            var param = _context.Request.Params;
            int projectID = Convert.ToInt32(param["projectID"]);
            int taskID, userID;
            var user = (UserModel)_context.Session["user"];
            try
            {
                if (myAccess.IsProjectMember(projectID, user.User_ID, user.Role))
                {
                    bool error = false;
                    switch (action)
                    {
                        /***********************************************/
                        // Submit a new comment of a task
                        case "insertTaskComment":
                            CommentModel comment = JsonConvert.DeserializeObject<CommentModel>(param["comment"]);
                            userID = comment.User_ID;
                            projectID = comment.Project_ID;
                            taskID = comment.Task_ID;
                            if ((userID == user.User_ID) && (myAccess.IsInProject(projectID, taskID, "Task")))
                                result = myAccess.insertTaskComment(comment);
                            else error = true;
                            break;

                        // View all comments of a task
                        case "viewTaskComment":
                            taskID = Convert.ToInt32(param["taskID"]);
                            if (myAccess.IsInProject(projectID, taskID, "Task"))
                                result = myAccess.viewTaskComment(param["taskID"], user.User_ID);
                            else error = true;
                            break;

                        // Delete a comment of a task
                        case "deleteTaskComment":
                            if (user.User_ID == Convert.ToInt32(param["userID"]))
                                result = myAccess.deleteTaskComment(param["commentID"], user.User_ID);
                            else error = true;
                            break;

                        // Edit a comment of a task
                        case "updateTaskComment":
                            if (user.User_ID == Convert.ToInt32(param["userID"]))
                                result = myAccess.updateTaskComment(param["commentID"], param["content"], user.User_ID);
                            else error = true;
                            break;
                    }
                    if (error) Code = 500;
                }
                else Code = 401;
            }
            catch
            {
                Code = 403;
            }
            finally
            {
                myAccess.Dipose();
                FinishWork();
            }
        }
    }
}