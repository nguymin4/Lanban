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
            :base(callback, context, state)
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
            int projectID = Convert.ToInt32(_context.Session["projectID"]);
            int userID = Convert.ToInt32(_context.Session["userID"]);

            switch (action)
            {
                /***********************************************/
                // Submit a new comment of a task
                case "insertTaskComment":
                    if (userID == Convert.ToInt32(param["userID"]))
                        result = myAccess.insertTaskComment(param["taskID"], param["content"], userID);
                    else RedirectPage(errorPage);
                    break;

                // View all comments of a task
                case "viewTaskComment":
                    result = myAccess.viewTaskComment(param["itemID"], userID);
                    break;

                // Delete a comment of a task
                case "deleteTaskComment":
                    if (userID == Convert.ToInt32(param["userID"]))
                        result = myAccess.deleteTaskComment(param["itemID"], userID);
                    else RedirectPage(errorPage);
                    break;

                // Edit a comment of a task
                case "updateTaskComment":
                    if (userID == Convert.ToInt32(param["userID"]))
                        result = myAccess.updateTaskComment(param["itemID"], param["content"], userID);
                    else RedirectPage(errorPage);
                    break;
            }
            
            FinishWork();
            myAccess.Dipose();
        }
    }
}