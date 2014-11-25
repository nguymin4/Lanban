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
            Clients.User(userID).receiveMessage(message);
        }
        
        public void DeleteProject(List<string> userList, string projectID)
        {
            string userID = "";
            Clients.User(userID).deleteProject(projectID);
        }
    }

    public class UserHubIdProvider: IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            string username = request.User.Identity.Name;
            int uid = new AccessLayer.UserAccess().getUserID(username);
            return uid.ToString();
        }
    }
}