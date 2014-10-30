using System;
using System.Threading;
using System.Collections.Generic;
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
            string projectID, swimlaneID, itemID, type, pos;
            string result = "";
            
            switch (action)
            {
                // Insert new data
                case "insertBacklog":
                    Backlog backlog = JsonConvert.DeserializeObject<Backlog>(param["backlog"]);
                    result = myQuery.insertNewBacklog(backlog.getStringArray());
                    break;
                
                // Get all data of an item
                case "viewItem":
                    itemID = param["itemID"];
                    type = param["type"];
                    result = myQuery.viewItem(itemID, type);
                    _context.Response.ContentType = "application/json";
                    break;
                
                // Update position of an item in a swimlane
                case "updatePosition":
                    type = param["type"];
                    itemID = param["itemID"];
                    pos = param["pos"];
                    myQuery.updatePosition(itemID, pos, type);
                    break;
                
                // Change swimlane of an item
                case "changeSwimlane":
                    type = param["type"];
                    itemID = param["itemID"];
                    pos = param["pos"];
                    swimlaneID = param["swimlane"];
                    myQuery.changeSwimlane(itemID, pos, type, swimlaneID);
                    break;
                
                // Search name of members in a project
                case "searchAssignee":
                    projectID = param["projectID"];
                    result = myQuery.searchAssignee(projectID, param["keyword"], param["type"]);
                    break;
                
                // View all assignee of an item
                case "viewAssignee":
                    type = param["type"];
                    itemID = param["itemID"];
                    result = myQuery.viewAssignee(itemID, type);
                    break;
                
                // Save assignee of an item
                case "saveAssignee":
                    itemID = param["itemID"];
                    type = param["type"];
                    string aID = param["assigneeID"];
                    string aName = param["assigneeName"];
                    myQuery.saveAssignee(itemID, type, aID, aName);
                    break;
                
                // Delete all assignees of an item
                case "deleteAssignee":
                    itemID = param["itemID"];
                    type = param["type"];
                    myQuery.deleteAssignee(itemID, type);
                    break;
            }
            _context.Response.Write(result);
            _completed = true;
            _callback(this);
        }
    }
}

class Backlog
{
    public string Project_ID { get; set; }
    public string Swimlane_ID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Complexity { get; set; }
    public string Color { get; set; }
    public string Position { get; set; }

    public string[] getStringArray()
    {
        string[] result = {this.Project_ID, this.Swimlane_ID, this.Title, this.Description, 
                             this.Complexity, this.Color, this.Position};
        return result;
    }
}

class Task
{
    public string Project_ID { get; set; }
    public string Swimlane_ID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Backlog_ID { get; set; }
    public string Work_estimation { get; set; }
    public string Color { get; set; }
    public string Position { get; set; }

}