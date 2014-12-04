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
            int projectID = Convert.ToInt32(param["projectID"]);
            ProjectModel project;

            try
            {
                int userID = user.User_ID;
                bool error = false;
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
                        else error = true;
                        break;

                    // Update project
                    case "updateProject":
                        project = JsonConvert.DeserializeObject<ProjectModel>(param["project"]);
                        if (!myAccess.updateProject(project, userID)) error = true;
                        break;

                    // Delete a project
                    case "deleteProject":
                        if (myAccess.deleteProject(projectID, userID))
                            new FileManager().deleteProjectFolder(_context, projectID);
                        else error = true;
                        break;

                    // Get data of all supervisors in a project
                    case "fetchSupervisor":
                        if (myAccess.IsProjectMember(projectID, userID, user.Role))
                            result = myAccess.fetchSupervisor(projectID);
                        else error = true;
                        break;

                    // Get data of all user in a project
                    case "fetchUser":
                        if (myAccess.IsProjectMember(projectID, userID, user.Role))
                            result = myAccess.fetchUser(projectID, userID);
                        else error = true;
                        break;

                    // Kick a user
                    case "kickUser":
                        if (myAccess.IsOwner(projectID, userID))
                            myAccess.kickUser(projectID, Convert.ToInt32(param["uid"]));
                        else error = true;
                        break;

                    // Upload profile
                    case "uploadAvatar":
                        new FileManager().uploadAvatar(_context, userID);
                        break;
                }
                if (error) Code = 500;
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