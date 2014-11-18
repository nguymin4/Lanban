using System;
using System.Threading;
using System.Collections;
using System.Web;
using System.Text;
using System.Web.SessionState;
using Newtonsoft.Json;
using Lanban.Model;

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
            string result = "";

            switch (action)
            {
                // Working with data query and manipulation from this part
                // Insert new data
                case "insertItem":
                    if (param["type"].Equals("backlog"))
                    {
                        Backlog backlog = JsonConvert.DeserializeObject<Backlog>(param["item"]);
                        result = myQuery.insertNewBacklog(backlog);
                    }
                    else
                    {
                        Task task = JsonConvert.DeserializeObject<Task>(param["item"]);
                        result = myQuery.insertNewTask(task);
                    }
                    break;

                // Get all data of an item
                case "viewItem":
                    result = myQuery.viewItem(param["itemID"], param["type"]);
                    _context.Response.ContentType = "application/json";
                    break;

                // Update an item
                case "updateItem":
                    if (param["type"].Equals("backlog"))
                    {
                        Backlog backlog = JsonConvert.DeserializeObject<Backlog>(param["item"]);
                        myQuery.updateBacklog(param["itemID"], backlog);
                    }
                    else
                    {
                        Task task = JsonConvert.DeserializeObject<Task>(param["item"]);
                        myQuery.updateTask(param["itemID"], task);
                    }
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
                    result = myQuery.insertTaskComment(param["taskID"], param["content"], Convert.ToInt32(_context.Session["userID"]));
                    break;

                // View all comments of a task
                case "viewTaskComment":
                    result = myQuery.viewTaskComment(param["itemID"], Convert.ToInt32(_context.Session["userID"]));
                    break;

                // Delete a comment of a task
                case "deleteTaskComment":
                    myQuery.deleteTaskComment(param["itemID"]);
                    break;

                // Edit a comment of a task
                case "updateTaskComment":
                    myQuery.updateTaskComment(param["itemID"], param["content"]);
                    break;

                // Working with chart from this part
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
                case "getLineGraph":
                    result = myQuery.getLineGraph(projectID);
                    _context.Response.ContentType = "application/json";
                    break;

                // Upload files
                case "uploadFile":
                    result = new FileUpload().uploadFile(_context, myQuery, projectID);
                    break;
                case "viewTaskFile":
                    result = myQuery.viewTaskFile(Convert.ToInt32(param["taskID"]));
                    break;
                case "deleteTaskFile":
                    new FileUpload().deleteFile(_context, myQuery);
                    break;
            }
            _context.Response.Write(result);
            _completed = true;
            _callback(this);
            myQuery.Dipose();
        }
    }
}