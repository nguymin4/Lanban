using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Web.Security;

namespace Lanban.Hubs
{
    // This Hub is for all client who are in the same kanban board
    public class NoteHub : Hub
    {
        // Send new submitted note to other clients.
        public void SendInsertedNote(string swimlanePosition, string objtext)
        {
            string channelID = GetChannelID();
            Clients.OthersInGroup(channelID).receiveInsertedNote(swimlanePosition, objtext);
        }

        // Delete note
        public void DeleteNote(string noteID)
        {
            string channelID = GetChannelID();
            Clients.OthersInGroup(channelID).deleteNote(noteID);
        }

        // Update note content
        public void UpdateNote(string noteID, string title, string color)
        {
            string channelID = GetChannelID();
            Clients.OthersInGroup(channelID).updateNote(noteID, title, color);
        }

        // Change position of a note
        public void ChangePosition(string noteID, string swimlanePosition, string position)
        {
            string channelID = GetChannelID();
            Clients.OthersInGroup(channelID).changePosition(noteID, swimlanePosition, position);
        }

        // Change lane of a note
        public void ChangeLane(string noteID, string swimlanePosition, string position)
        {
            string channelID = GetChannelID();
            Clients.OthersInGroup(channelID).changeLane(noteID, swimlanePosition, position);
        }

        // Join a channel
        public Task JoinChannel()
        {
            return Groups.Add(Context.ConnectionId, GetChannelID());
        }

        // Leave the channel
        public Task LeaveChannel()
        {
            return Groups.Remove(Context.ConnectionId, GetChannelID());
        }

        // Get ProjectID aka ChannelID
        public string GetChannelID()
        {
            var cookie = Context.RequestCookies[FormsAuthentication.FormsCookieName];
            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            return ticket.UserData;
        }
    }
}