using Lanban.AccessLayer;
using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Web;

namespace Lanban
{
    public class ItemHandlerOperation : HandlerOperation
    {
        ItemAccess myAccess;

        public ItemHandlerOperation(AsyncCallback callback, HttpContext context, Object state)
            : base(callback, context, state)
        {
            myAccess = new ItemAccess();
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
                    switch (action)
                    {
                        // Insert new data
                        case "insertItem":
                            if (param["type"].Equals("backlog"))
                            {
                                Backlog backlog = JsonConvert.DeserializeObject<Backlog>(param["item"]);
                                if (backlog.Project_ID == projectID) result = myAccess.insertNewBacklog(backlog);
                                else error = true;
                            }
                            else
                            {
                                Task task = JsonConvert.DeserializeObject<Task>(param["item"]);
                                if (task.Project_ID == projectID) result = myAccess.insertNewTask(task);
                                else error = true;
                            }
                            break;

                        // Update an item
                        case "updateItem":
                            if (param["type"].Equals("backlog"))
                            {
                                Backlog backlog = JsonConvert.DeserializeObject<Backlog>(param["item"]);
                                if (myAccess.updateBacklog(backlog, projectID) != 1) error = true;
                            }
                            else
                            {
                                Task task = JsonConvert.DeserializeObject<Task>(param["item"]);
                                if (myAccess.updateTask(task, projectID) != 1) error = true;
                            }
                            break;

                        // Get all data of an item
                        case "viewItem":
                            result = myAccess.viewItem(param["itemID"], param["type"], projectID);
                            if (result.Equals("")) error = true;
                            else _context.Response.ContentType = "application/json";
                            break;

                        // Delete an item
                        case "deleteItem":
                            if (myAccess.deleteItem(param["itemID"], param["type"], projectID) != 1)
                                error = true;
                            break;

                        // Update position of an item in a swimlane
                        case "updatePosition":
                            myAccess.updatePosition(param["itemID"], param["pos"], param["type"]);
                            break;

                        // Change swimlane of an item
                        case "changeSwimlane":
                            myAccess.changeSwimlane(param["itemID"], param["pos"], param["type"], param["swimlane"]);
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