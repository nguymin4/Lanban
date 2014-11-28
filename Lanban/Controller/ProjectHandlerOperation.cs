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
            var user = (UserModel)_context.Session["user"];
            int userID = user.User_ID;
            int projectID = Convert.ToInt32(param["projectID"]);
            ProjectModel project;
            switch (action)
            {
                /***********************************************/
                // Create new project
                case "createProject":
                    project = JsonConvert.DeserializeObject<ProjectModel>(param["project"]);
                    if (userID == project.Owner)
                    {
                        result = myAccess.createProject(project);
                        new FileManager().createProjectFolder(_context, result);
                    }
                    else RedirectPage(errorPage);
                    break;

                // Update project
                case "updateProject":
                    project = JsonConvert.DeserializeObject<ProjectModel>(param["project"]);
                    if (!myAccess.updateProject(project , userID)) RedirectPage(errorPage);
                    break;

                // Delete a project
                case "deleteProject":
                    if (myAccess.deleteProject(projectID, userID))
                        new FileManager().deleteProjectFolder(_context, projectID);
                    else RedirectPage(errorPage);
                    break;

                case "quitProject":
                    if (!myAccess.quitProject(projectID, userID, user.Role)) RedirectPage(errorPage);
                    break;

                // Get data of all supervisors in a project
                case "fetchSupervisor":
                    if (myAccess.IsProjectMember(projectID, userID, user.Role))
                        result = myAccess.fetchSupervisor(projectID);
                    else RedirectPage(errorPage);
                    break;

                // Get data of all user in a project
                case "fetchUser":
                    if (myAccess.IsProjectMember(projectID, userID, user.Role))
                        result = myAccess.fetchUser(projectID, userID);
                    else RedirectPage(errorPage);
                    break;
            }
            myAccess.Dipose();
            FinishWork();
        }
    }
}