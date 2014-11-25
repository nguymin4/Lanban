using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Lanban.Controller
{
    // This Hub is for all client who are sharing the same task view
    public class TaskHub : Hub
    {
        // Send new submitted comment to other clients.
        public void SendSubmittedComment(string channelID, string userID, string objtext)
        {
            Clients.OthersInGroup(channelID).receiveSubmittedComment(userID, objtext);
        }

        // Delete comment
        public void DeleteComment(string channelID, string commentID)
        {
            Clients.OthersInGroup(channelID).deleteComment(commentID);
        }

        // Update comment
        public void UpdateComment(string channelID, string commentID, string content)
        {
            Clients.OthersInGroup(channelID).updateComment(commentID, content);
        }

        // Send visual of a file
        public void SendUploadedFile(string channelID, string objtext)
        {
            Clients.OthersInGroup(channelID).receiveUploadedFile(objtext);
        }

        // Delete comment
        public void DeleteFile(string channelID, string fileID)
        {
            Clients.OthersInGroup(channelID).deleteFile(fileID);
        }


        // Join a channel
        public Task JoinChannel(string channelID)
        {
            return Groups.Add(Context.ConnectionId, channelID);
        }

        // Leave the channel
        public Task LeaveChannel(string channelID)
        {
            return Groups.Remove(Context.ConnectionId, channelID);
        }
    }
}