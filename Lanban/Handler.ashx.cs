using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Newtonsoft.Json;
namespace Lanban
{
    /// <summary>
    /// Summary description for Handler
    /// </summary>
    public class Handler : IHttpHandler
    {
        Query myQuery;

        public void ProcessRequest(HttpContext context)
        {
            myQuery = new Query();
            var param = context.Request.Params;
            string action = param["action"];
            string projectID;
            string swimlaneID;
            string table;
            string id;
            string pos;
            string result = "";
            switch (action)
            {
                case "searchAssignee":
                    projectID = param["projectID"];
                    result = myQuery.searchAssignee(projectID, param["keyword"], param["type"]);
                    break;
                case "insertBacklog":
                    Backlog backlog = JsonConvert.DeserializeObject<Backlog>(param["backlog"]);
                    result = myQuery.insertNewBacklog(backlog.getStringArray());
                    break;
                case "updatePosition":
                    table = param["table"];
                    id = param["id"];
                    pos = param["pos"];
                    result = myQuery.updatePosition(id, pos, table);
                    break;
                case "changeSwimlane":
                    table = param["table"];
                    id = param["id"];
                    pos = param["pos"];
                    swimlaneID = param["swimlane"];
                    result = myQuery.changeSwimlane(id, pos, table, swimlaneID);
                    break;
                default:
                    break;
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(result);
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
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