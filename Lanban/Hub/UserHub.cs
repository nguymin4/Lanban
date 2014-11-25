using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Web;

namespace Lanban.Hubs
{
    public class UserHub : Hub
    {
        public void SendMessage(string userID, string message)
        {
            Clients.Group(userID).receiveMessage(message);
        }
        
        public void DeleteProject(List<string> userList, string projectID)
        {
            string userID = "";
            Clients.User(userID).deleteProject(projectID);
        }

        public Task Connect(string userID)
        {
            return Groups.Add(Context.ConnectionId, userID);
        }
    }
}