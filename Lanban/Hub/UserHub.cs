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

        // Fetch list of user will receive the message and data
        protected async Task FetchUserData(int projectID)
        {
            string username = Context.User.Identity.Name;
            Task<Model.UserModel> task1 = Task.Run(() => myUA.getUserData(username));
            Task<List<int>> task2 = Task.Run(()
                => myPA.getProjectMemberID(projectID));
            sender = await task1;
            ids = (await task2).ConvertAll(x => x.ToString());
        }

        // Send updated info of a project to all member
        public async Task UpdateProject(int projectID)
        {
            await FetchUserData(projectID);

            if (myPA.IsOwner(projectID, sender.User_ID))
            {
                // Get project data
                Task<Model.ProjectModel> task1 = Task.Run(() => myPA.getProjectData(projectID));
                var project = await task1;
                DiposeConnection();

                // Message for notification center
                Task task2 = Task.Run(()
                    => Clients.Users(ids).msgProject(sender, "updated", projectID, project.Name));

                // Updated project object for other user
                Clients.Users(ids).updateProject(project);

                await task2;
            }
        }

        // Announce to all member about the deleted project
        public void DeleteProject(int projectID, string projectName, int userID)
        {
            string username = Context.User.Identity.Name;
            sender = myUA.getUserData(username);
            DiposeConnection();

            if (sender.User_ID == userID)
            {
                // Message for notification center
                Clients.Others.msgProject(sender, "deleted", projectID, projectName);

                // Deleted project object for other user
                Clients.Others.deleteProject(projectID);
            }
        }

        // When a member add new user to the project
        public async Task AddUser(int projectID, int addedMember)
        {
            await FetchUserData(projectID);
            Model.UserModel target = myUA.getUserData(addedMember);
            Task<string> task1 = Task.Run(() => 
                myUA.getPersonContainer<Model.UserModel>(target, true, projectID));

            // Get project data
            Task<Model.ProjectModel> task2 = Task.Run(() => myPA.getProjectData(projectID));
            // Get owner data if the sender is not owner
           
           
            var project = await task2;
            string personContainer = await task1;

            // Message for notification center
            Task task3 = Task.Run(() 
                => Clients.Users(ids).msgAddUser(sender, target, project.Name));
            
            // Person Object for display in other member display
            Task task4 = Task.Run(() => Clients.Users(ids).addUser(projectID, personContainer));

            // Project object for the added person
            Clients.User(addedMember.ToString()).addProject(project);

            // Owner object for the added person
            Model.UserModel owner;
            if (sender.User_ID != project.Owner)
                owner = myUA.getUserData(project.Owner);
            else owner = sender;
            Clients.User(addedMember.ToString()).addOwner(owner);
            
            DiposeConnection();
            await Task.WhenAll(task3, task4);
        }

        // The owner kick a member
        public async Task RemoveUser(int projectID, string name, int personID)
        {
            await FetchUserData(projectID);
            
            // Check in case of javascript injection
            if (myPA.IsOwner(projectID, sender.User_ID))
            {
                DiposeConnection();
                
                // Delete person in other view
                Clients.Users(ids).removeUser(projectID, personID);
                
                // Delete project in the kicked person view
                Clients.User(personID.ToString()).deleteProject(projectID);
                Clients.User(personID.ToString()).msgOwnerKick(sender, projectID, name);
            }
        }

        // When a member leave the project
        public async Task QuitProject(int projectID, int personID)
        {
            await FetchUserData(projectID);
            DiposeConnection();

            // Check in case of javascript injection
            if (sender.User_ID == personID)
                Clients.Users(ids).removeUser(projectID, personID);
        }

        // Dispose all access
        public void DiposeConnection()
        {
            myPA.Dipose();
            myUA.Dipose();
        }
    }

    /* ID Provider for Hubs */
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