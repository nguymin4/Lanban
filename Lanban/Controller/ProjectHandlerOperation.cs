using Lanban.AccessLayer;
using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Lanban
{
    public class ProjectHandlerOperation : HandlerOperation
    {
        ProjectAccess myAccess;

        public ProjectHandlerOperation(AsyncCallback callback, HttpContext context, Object state)
            :base(callback, context, state)
        {
            myAccess = new ProjectAccess();
        }

        public void QueueWork()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartTask), null);
        }

        private void StartTask(Object workItemState)
        {
            var param = _context.Request.Params;
            int userID = Convert.ToInt32(_context.Session["userID"]);

            switch (action)
            {
                /***********************************************/
                // Create new project
                case "createProject":
                    result = myAccess.createProject(JsonConvert.DeserializeObject<ProjectModel>(param["project"]));
                    new FileManager().createProjectFolder(_context, result);
                    break;

                // Update project
                case "updateProject":
                    myAccess.updateProject(JsonConvert.DeserializeObject<ProjectModel>(param["project"]));
                    break;

                // Delete a project
                case "deleteProject":
                    int projectID = Convert.ToInt32(param["projectID"]);
                    myAccess.deleteProject(projectID);
                    new FileManager().deleteProjectFolder(_context, projectID);
                    break;

                // Get data of all supervisors in a project
                case "fetchSupervisor":
                    result = myAccess.fetchSupervisor(Convert.ToInt32(param["projectID"]));
                    break;
            }

            FinishWork();
            myAccess.Dipose();
        }
    }
}