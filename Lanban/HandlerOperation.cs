using System;
using System.Threading;
using System.Collections;
using System.Web;
using System.Text;
using Newtonsoft.Json;


namespace Lanban
{
    public class HandlerOperation : IAsyncResult
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
            string result = "";
            
            switch (action)
            {
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
                    result = myQuery.searchAssignee(param["projectID"], param["keyword"], param["type"]);
                    break;
                
                // View all assignee of an item
                case "viewAssignee":
                    result = myQuery.viewAssignee(param["itemID"], param["type"]);
                    break;
                
                // Save assignee of an item
                case "saveAssignee":
                    string aID = param["assigneeID"];
                    string aName = param["assigneeName"];
                    myQuery.saveAssignee(param["itemID"], param["type"], aID, aName);
                    break;
                
                // Delete all assignees of an item
                case "deleteAssignee":
                    myQuery.deleteAssignee(param["itemID"], param["type"]);
                    break;
            }
            _context.Response.Write(result);
            _completed = true;
            _callback(this);
        }
    }


    public class Backlog
    {
        public int Project_ID { get; set; }
        public int Swimlane_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Complexity { get; set; }
        public string Color { get; set; }

    }

    public class Task
    {
        public int Project_ID { get; set; }
        public int Swimlane_ID { get; set; }
        public int Backlog_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Work_estimation { get; set; }
        public string Color { get; set; }
        public string Due_date { get; set; }

        public Task()
        {
            this.Work_estimation = 0;
        }
    }
}