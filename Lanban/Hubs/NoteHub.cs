using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Lanban.Controller
{
    // This Hub is for all client who are in the same kanban board
    public class NoteHub : Hub
    {
        // Send new submitted note to other clients.
        public void SendInsertedNote(string channelID, string swimlanePosition, string objtext)
        {
            Clients.OthersInGroup(channelID).receiveInsertedNote(swimlanePosition, objtext);
        }

        // Delete note
        public void DeleteNote(string channelID, string noteID)
        {
            Clients.OthersInGroup(channelID).deleteNote(noteID);
        }

        // Update note content
        public void UpdateNote(string channelID, string noteID, string title, string color)
        {
            Clients.OthersInGroup(channelID).updateNote(noteID, title, color);
        }

        // Change position of a note
        public void ChangePosition(string channelID, string noteID, string swimlanePosition, string position)
        {
            Clients.OthersInGroup(channelID).changePosition(noteID, swimlanePosition, position);
        }

        // Change lane of a note
        public void ChangeLane(string channelID, string noteID, string swimlanePosition, string position)
        {
            Clients.OthersInGroup(channelID).changeLane(noteID, swimlanePosition, position);
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