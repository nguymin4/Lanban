using Lanban.AccessLayer;
using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lanban.Hubs
{
    public class UserHub : Hub
    {
        UserAccess myUA = new UserAccess();
        ProjectAccess myPA = new ProjectAccess();
        Model.UserModel sender;
        List<string> ids;

        protected async Task FetchUserData(int projectID)
        {
            string username = Context.User.Identity.Name;
            Task<Model.UserModel> task1 = Task.Run(() => myUA.getUserData(username));
            Task<List<int>> task2 = Task.Run(()
                => myPA.getProjectMemberID(projectID));
            sender = await task1;
            ids = (await task2).ConvertAll(x => x.ToString());
        }
        
        public void SendMessage(string userID, string message)
        {
            Clients.User(userID).receiveMessage(message);
        }

        public async Task UpdateProject(int projectID)
        {
            await FetchUserData(projectID);
        }

        public void DeleteProject(List<string> userList, string projectID)
        {
            string userID = "";
            Clients.User(userID).deleteProject(projectID);
        }

        public async Task AddUser(int projectID, int addedMember)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await FetchUserData(projectID);
            Model.UserModel target = myUA.getUserData(addedMember);
            Task<string> task1 = Task.Run(() => 
                myUA.getPersonContainer<Model.UserModel>(target, true, projectID));

            Task<Dictionary<string, object>> task2 = Task.Run(() => myPA.getProjectData(projectID));
            string personContainer = await task1;
            Dictionary<string, object> data = await task2;

            // Message for notification center
            Task task3 = Task.Run(() 
                => Clients.Users(ids).msgAddUser(sender, target, data.ElementAt(1).Value));
            
            // Person Object for display in other member display
            Task task4 = Task.Run(() => Clients.Users(ids).addUser(projectID, personContainer));

            // Project object for the added person
            Clients.User(addedMember.ToString()).addProject(JsonConvert.SerializeObject(data));

            await Task.WhenAll(task3, task4);
            myPA.Dipose();
            myUA.Dipose();
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("Send msg: " + watch.ElapsedMilliseconds);
        }
    }

    public class UserHubIdProvider: IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            string username = request.User.Identity.Name;
            var myAccess = new UserAccess();
            int uid = myAccess.getUserID(username);
            myAccess.Dipose();
            return uid.ToString();
        }
    }
}