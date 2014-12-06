using Lanban.AccessLayer;
using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Web;

namespace Lanban
{
    public class SwimlaneHandlerOperation : HandlerOperation
    {
        SwimlaneAccess myAccess;

        public SwimlaneHandlerOperation(AsyncCallback callback, HttpContext context, Object state)
            : base(callback, context, state)
        {
            myAccess = new SwimlaneAccess();
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

            var user = (UserModel)_context.Session["user"];
            try
            {
                if (myAccess.IsProjectMember(projectID, user.User_ID, user.Role))
                {
                    bool error = false;
                    Swimlane sw;
                    switch (action)
                    {
                        // Add new swimlane
                        case "addSwimlane":
                            sw = JsonConvert.DeserializeObject<Swimlane>(param["swimlane"]);
                                if (sw.Project_ID == projectID) result = myAccess.addSwimlane(sw);
                                else error = true;
                            break;
                        
                        // Edit swimlane
                        case "updateSwimlane":
                            sw = JsonConvert.DeserializeObject<Swimlane>(param["swimlane"]);
                            if (myAccess.updateSwimlane(sw, projectID) != 1) error = true;
                            break;
                        
                        // Delete swimlane
                        case "deleteSwimlane":
                            if (myAccess.deleteSwimlane(param["swimlaneID"], projectID) != 1)
                                error = true;
                            break;

                        // Save position
                        case "updatePosition":
                            myAccess.updatePositon(param["swimlaneID"], projectID, param["pos"]);
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
            finally {
                myAccess.Dipose();
                FinishWork();
            }
        }
    }
}