using Lanban.AccessLayer;
using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Lanban
{
    public class UserHandlerOperation : HandlerOperation
    {
        UserAccess myAccess;

        public UserHandlerOperation(AsyncCallback callback, HttpContext context, Object state)
            :base(callback, context, state)
        {
            myAccess = new UserAccess();
        }

        public void QueueWork()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartTask), null);
        }

        private void StartTask(Object workItemState)
        {
            var param = _context.Request.Params;
            int projectID = Convert.ToInt32(param["projectID"]);
            int userID = Convert.ToInt32(_context.Session["userID"]);

            switch (action)
            {
                 /***********************************************/
                // Search name of members in a project
                case "searchAssignee":
                    result = myAccess.searchAssignee(projectID, param["keyword"], param["type"]);
                    break;
                
                // View all assignees/members of an item
                case "viewAssignee":
                    result = myAccess.viewAssignee(param["itemID"], param["type"]);
                    break;

                // Save assignee/member of an object
                case "saveAssignee":
                    string aID = param["assigneeID"];
                    myAccess.saveAssignee(param["itemID"], param["type"], aID);
                    break;

                // Delete all assignees/member of an object
                case "deleteAssignee":
                    myAccess.deleteAssignee(param["itemID"], param["type"]);
                    break;

                // Get the user data based on name and role
                case "searchUser":
                    result = myAccess.searchUser(param["name"], Convert.ToInt32(param["role"]));
                    break;

                /***********************************************/
                // Working with supervisor
                // Delete all supervisor of a project
                case "deleteSupervisor":
                    myAccess.deleteSupervisor(projectID);
                    break;

                // Save supervisor of a project
                case "saveSupervisor":
                    myAccess.saveSupervisor(projectID, Convert.ToInt32(param["supervisorID"]));
                    break;
            }
            myAccess.Dipose();
            FinishWork();
        }
    }
}