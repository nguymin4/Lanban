using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Web.Security;
using Lanban.AccessLayer;
using System;

namespace Lanban.Hubs
{
    // This Hub is for all client who are sharing the same task view
    public class TaskHub : Hub
    {
        UserAccess myUA;
        CommentAccess myCA;
        Model.UserModel sender;
        Model.CommentModel comment;
        int projectID;

        // Send new submitted comment to other clients.
        public async Task SendSubmittedComment(int commentID)
        {
            await FetchData(commentID);
            string channelID = GetChannelID();
            if ((sender.User_ID == comment.User_ID) && (projectID == Convert.ToInt32(channelID)))
            {
                string obj = myCA.getTaskComment(commentID, comment.User_ID);
                DiposeConnection();
                Clients.OthersInGroup(channelID).receiveSubmittedComment(comment.Task_ID, sender.User_ID, obj);
            }
        }

        // Delete comment
        public void DeleteComment(int commentID)
        {
            string channelID = GetChannelID();
            Clients.OthersInGroup(channelID).deleteComment(commentID);
        }

        // Update comment
        public async Task UpdateComment(int commentID, string content)
        {
            await FetchData(commentID);
            DiposeConnection();

            string channelID = GetChannelID();
            if ((sender.User_ID == comment.User_ID) && (projectID == Convert.ToInt32(channelID)))
                Clients.OthersInGroup(channelID).updateComment(comment.Task_ID, commentID, content);
        }

        // Send visual of a file
        public void SendUploadedFile(int taskID, string objtext)
        {
            string channelID = GetChannelID();
            Clients.OthersInGroup(channelID).receiveUploadedFile(taskID, objtext);
        }

        // Delete file
        public void DeleteFile(int taskID, int fileID)
        {
            string channelID = GetChannelID();
            Clients.OthersInGroup(channelID).deleteFile(taskID, fileID);
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

        // Get Sender
        public async Task FetchData(int commentID)
        {
            myUA = new UserAccess();
            myCA = new CommentAccess();
            string username = Context.User.Identity.Name;
            Task<Model.UserModel> task1 = Task.Run(() => myUA.getUserData(username));
            Task<Model.CommentModel> task2 = Task.Run(() => myCA.getComment(commentID));
            sender = await task1;
            comment = await task2;
            projectID = comment.Project_ID;
        }

        // Dispose all access
        public void DiposeConnection()
        {
            myCA.Dipose();
            myUA.Dipose();
        }
    }
}