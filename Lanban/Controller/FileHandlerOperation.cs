using Lanban.AccessLayer;
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
            int projectID = Convert.ToInt32(_context.Session["projectID"]);
            int userID = Convert.ToInt32(_context.Session["userID"]);

            switch (action)
            {
                /***********************************************/
                // Upload files
                case "uploadFile":
                    result = new FileManager().uploadFile(_context, myAccess, projectID);
                    break;

                // Get list of all files belong to a task
                case "viewTaskFile":
                    result = myAccess.viewTaskFile(Convert.ToInt32(param["taskID"]));
                    break;

                // Delete a file belongs to a task
                case "deleteTaskFile":
                    new FileManager().deleteFile(_context, myAccess);
                    break;

                // Upload screenshot of a project
                case "uploadScreenshot":
                    new FileManager().uploadScreenshot(_context, projectID);
                    break;
            }

            FinishWork();
            myAccess.Dipose();
        }
    }
}