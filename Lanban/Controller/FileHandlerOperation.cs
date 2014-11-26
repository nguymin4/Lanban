using Lanban.AccessLayer;
using Lanban.Model;
using System;
using System.Threading;
using System.Web;

namespace Lanban
{
    public class FileHandlerOperation : HandlerOperation
    {
        FileAccess myAccess;

        public FileHandlerOperation(AsyncCallback callback, HttpContext context, Object state)
            :base(callback, context, state)
        {
            myAccess = new FileAccess();
        }

        public void QueueWork()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartTask), null);
        }

        private void StartTask(Object workItemState)
        {
            var param = _context.Request.Params;
            int projectID = Convert.ToInt32(param["projectID"]);
            int taskID;
            var user = (UserModel)_context.Session["user"];
            if (!myAccess.IsProjectMember(projectID, user.User_ID, user.Role))
            {
                RedirectPage(errorPage);
            }
            else
            {
                switch (action)
                {
                    /***********************************************/
                    // Upload files
                    case "uploadFile":
                        taskID = Convert.ToInt32(param["taskID"]);
                        if (myAccess.IsInProject(projectID, taskID, "Task"))
                            result = new FileManager().uploadFile(_context, myAccess, projectID);
                        else RedirectPage(errorPage);
                        break;

                    // Get list of all files belong to a task
                    case "viewTaskFile":
                        taskID = Convert.ToInt32(param["taskID"]);
                        if (myAccess.IsInProject(projectID, taskID, "Task"))
                            result = myAccess.viewTaskFile(taskID);
                        else RedirectPage(errorPage);
                        break;

                    // Delete a file belongs to a task
                    case "deleteTaskFile":
                        taskID = Convert.ToInt32(param["taskID"]);
                        if (myAccess.IsInProject(projectID, taskID, "Task") && myAccess.IsInTask(param["fileID"], taskID))
                            new FileManager().deleteFile(_context, myAccess);
                        else RedirectPage(errorPage);
                        break;

                    // Upload screenshot of a project
                    case "uploadScreenshot":
                        new FileManager().uploadScreenshot(_context, projectID);
                        break;
                }
            }

            FinishWork();
            myAccess.Dipose();
        }
    }
}