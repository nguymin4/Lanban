using Lanban.AccessLayer;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lanban.Hubs
{
    public class UserHub : Hub
    {
        public void SendMessage(string userID, string message)
        {

            Clients.User(userID).receiveMessage(message);
        }

        public async Task UpdateProject(string projectID)
        {
            string username = Context.User.Identity.Name;
            Task<int> task1 = Task.Run(() => new UserAccess().getUserID(username));
            Task<List<int>> task2 = Task.Run(() 
                => new ProjectAccess().getProjectMemberID(Convert.ToInt32(projectID)));
            int uid = await task1;
            List<int> ids = await task2;
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