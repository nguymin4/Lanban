using Lanban.AccessLayer;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Web.Security;

namespace Lanban.Hubs
{
    // This Hub is for all client who are in the same kanban board
    public class NoteHub : Hub
    {
        UserAccess myUA = new UserAccess();
        Model.UserModel sender;
        string channelID;

        /********************************************/
        /* Sticky note */
        // Send new submitted note to other clients.
        public void SendInsertedNote(string swimlanePosition, string objtext)
        {
            GetChannelID();
            Clients.OthersInGroup(channelID).receiveInsertedNote(swimlanePosition, objtext);
        }

        // Delete note
        public void DeleteNote(string noteID)
        {
            GetChannelID();
            Clients.OthersInGroup(channelID).deleteNote(noteID);
        }

        // Update note content
        public void UpdateNote(string noteID, string title, string color)
        {
            GetChannelID();
            Clients.OthersInGroup(channelID).updateNote(noteID, title, color);
        }

        // Change position of a note
        public void ChangePosition(string noteID, string swimlanePosition, string position)
        {
            GetChannelID();
            Clients.OthersInGroup(channelID).changePosition(noteID, swimlanePosition, position);
        }

        // Change lane of a note
        public void ChangeLane(string noteID, string swimlanePosition, string position)
        {
            GetChannelID();
            Clients.OthersInGroup(channelID).changeLane(noteID, swimlanePosition, position);
        }

        /********************************************/
        /* Swimlane */
        // Insert a swimlane
        public async Task InsertSwimlane(dynamic swimlane)
        {
            await FetchData();
            Clients.OthersInGroup(channelID).insertSwimlane(sender, swimlane);
        }

        // Update a swimlane
        public async Task UpdateSwimlane(int swimlaneID, string name)
        {
            await FetchData();
            Clients.OthersInGroup(channelID).updateSwimlane(sender, swimlaneID, name);
        }

        // Delete a swimlane
        public async Task DeleteSwimlane(int swimlaneID)
        {
            await FetchData();
            Clients.OthersInGroup(channelID).deleteSwimlane(sender, swimlaneID);
        }

        // Change position of a swimlane
        public async Task ChangeSWPosition(int org, int target)
        {
            await FetchData();
            Clients.OthersInGroup(channelID).changeSWPosition(sender, org, target);
        }

        // Change position of a swimlane

        /********************************************/
        // Fetch data
        protected async Task FetchData()
        {
            string username = Context.User.Identity.Name;
            Task<Model.UserModel> task1 = Task.Run(() => myUA.getUserData(username));
            Task task2 = Task.Run(() => GetChannelID());
            sender = await task1;
            await task2;
        }

        // Join a channel
        public Task JoinChannel()
        {
            GetChannelID();
            return Groups.Add(Context.ConnectionId, channelID);
        }

        // Leave the channel
        public Task LeaveChannel()
        {
            GetChannelID();
            return Groups.Remove(Context.ConnectionId, channelID);
        }

        // Get ProjectID aka ChannelID
        public void GetChannelID()
        {
            var cookie = Context.RequestCookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            channelID = ticket.UserData;
        }

    }
}