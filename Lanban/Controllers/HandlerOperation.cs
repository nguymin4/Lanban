using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Lanban
{
    public class HandlerOperation : IAsyncResult, IReadOnlySessionState
    {
        private bool _completed;
        private Object _state;
        private AsyncCallback _callback;
        private HttpContext _context;


        bool IAsyncResult.IsCompleted { get { return _completed; } }
        WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
        Object IAsyncResult.AsyncState { get { return _state; } }
        bool IAsyncResult.CompletedSynchronously { get { return false; } }

        Query myQuery;

        public HandlerOperation(AsyncCallback callback, HttpContext context, Object state)
        {
            _callback = callback;
            _context = context;
            _state = state;
            _completed = false;
            myQuery = new Query();
        }

        public void QueueWork()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartTask), null);
        }

        private void StartTask(Object workItemState)
        {
            _context.Response.ContentType = "text/plain";
            var param = _context.Request.Params;
            string action = param["action"];
            int projectID = Convert.ToInt32(_context.Session["projectID"]);
            int userID = Convert.ToInt32(_context.Session["userID"]);
            string result = "";

            switch (action)
            {
                /* Operations in Board.aspx */

                // Insert new data
                case "insertItem":
                    if (param["type"].Equals("backlog"))
                        result = myQuery.insertNewBacklog(JsonConvert.DeserializeObject<Backlog>(param["item"]));
                    else
                        result = myQuery.insertNewTask(JsonConvert.DeserializeObject<Task>(param["item"]));
                    break;

                // Get all data of an item
                case "viewItem":
                    result = myQuery.viewItem(param["itemID"], param["type"]);
                    _context.Response.ContentType = "application/json";
                    break;

                // Update an item
                case "updateItem":
                    if (param["type"].Equals("backlog"))
                        myQuery.updateBacklog(JsonConvert.DeserializeObject<Backlog>(param["item"]));
                    else
                        myQuery.updateTask(JsonConvert.DeserializeObject<Task>(param["item"]));
                    break;

                // Delete an item
                case "deleteItem":
                    myQuery.deleteItem(param["itemID"], param["type"]);
                    break;

                // Update position of an item in a swimlane
                case "updatePosition":
                    myQuery.updatePosition(param["itemID"], param["pos"], param["type"]);
                    break;

                // Change swimlane of an item
                case "changeSwimlane":
                    myQuery.changeSwimlane(param["itemID"], param["pos"], param["type"], param["swimlane"]);
                    break;

                // Search name of members in a project
                case "searchAssignee":
                    result = myQuery.searchAssignee(projectID, param["keyword"], param["type"]);
                    break;

                // View all assignee of an item
                case "viewAssignee":
                    result = myQuery.viewAssignee(param["itemID"], param["type"]);
                    break;

                // Save assignee of an item
                case "saveAssignee":
                    string aID = param["assigneeID"];
                    myQuery.saveAssignee(param["itemID"], param["type"], aID);
                    break;

                // Delete all assignees of an item
                case "deleteAssignee":
                    myQuery.deleteAssignee(param["itemID"], param["type"]);
                    break;

                // Submit a new comment of a task
                case "insertTaskComment":
                    if (userID == Convert.ToInt32(param["userID"]))
                        result = myQuery.insertTaskComment(param["taskID"], param["content"], userID);
                    break;

                // View all comments of a task
                case "viewTaskComment":
                    result = myQuery.viewTaskComment(param["itemID"], userID);
                    break;

                // Delete a comment of a task
                case "deleteTaskComment":
                    if (userID == Convert.ToInt32(param["userID"]))
                    {
                        result = myQuery.deleteTaskComment(param["itemID"], userID);
                    }
                    break;

                // Edit a comment of a task
                case "updateTaskComment":
                    if (userID == Convert.ToInt32(param["userID"]))
                    {
                        result = myQuery.updateTaskComment(param["itemID"], param["content"], userID);
                    }
                    break;

                // Working with chart in Board.aspx
                // Get Pie Chart data
                case "getPieChart":
                    result = myQuery.getPieChart(projectID);
                    _context.Response.ContentType = "application/json";
                    break;

                // Get Bar Chart data
                case "getBarChart":
                    result = myQuery.getBarChart(projectID);
                    _context.Response.ContentType = "application/json";
                    break;

                // Get Line Graph data
                case "getLineGraph":
                    result = myQuery.getLineGraph(projectID);
                    _context.Response.ContentType = "application/json";
                    break;

                // Upload files
                case "uploadFile":
                    result = new FileManager().uploadFile(_context, myQuery, projectID);
                    break;

                // Get list of all files belong to a task
                case "viewTaskFile":
                    result = myQuery.viewTaskFile(Convert.ToInt32(param["taskID"]));
                    break;

                // Delete a file belongs to a task
                case "deleteTaskFile":
                    new FileManager().deleteFile(_context, myQuery);
                    break;

                // Upload screenshot of a project
                case "uploadScreenshot":
                    new FileManager().uploadScreenshot(_context, projectID);
                    break;

                /* Operation in Project.aspx */
                /*                           */
                // Get data of all supervisors in a project
                case "fetchSupervisor":
                    result = myQuery.fetchSupervisor(Convert.ToInt32(param["projectID"]));
                    break;

                // Get the user data based on name and role
                case "searchUser":
                    result = myQuery.searchUser(param["name"], Convert.ToInt32(param["role"]));
                    break;

                // Working with Project
                // Create new project
                case "createProject":
                    result = myQuery.createProject(JsonConvert.DeserializeObject<ProjectModel>(param["project"]));
                    new FileManager().createProjectFolder(_context, result);
                    break;

                // Update project
                case "updateProject":
                    myQuery.updateProject(JsonConvert.DeserializeObject<ProjectModel>(param["project"]));
                    break;

                // Delete a project
                case "deleteProject":
                    projectID = Convert.ToInt32(param["projectID"]);
                    myQuery.deleteProject(projectID);
                    new FileManager().deleteProjectFolder(_context, projectID);
                    break;

                // Working with supervisor
                // Delete all supervisor of a project
                case "deleteSupervisor":
                    projectID = Convert.ToInt32(param["projectID"]);
                    myQuery.deleteSupervisor(projectID);
                    break;
                // Save supervisor of a project
                case "saveSupervisor":
                    projectID = Convert.ToInt32(param["projectID"]);
                    myQuery.saveSupervisor(projectID, Convert.ToInt32(param["supervisorID"]));
                    break;
            }
            _context.Response.Write(result);
            _completed = true;
            _callback(this);
            myQuery.Dipose();
        }
    }
}