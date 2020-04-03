using Microsoft.AspNetCore.SignalR;
using OnlineBuzzerApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBuzzerApp.Server
{
    public class ChatHub : Hub
    {
        public static string BuzzedUser { get; set; }

        public static Dictionary<string, string> ConnectedUsersReal = new Dictionary<string, string>();
        public static List<string> ConnectedUsers { get; set; } = new List<string>();
        public static List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

        public async override Task OnConnectedAsync()
        {
            if (!string.IsNullOrEmpty(BuzzedUser))
            {
                await Clients.Caller.SendAsync("ReceiveBuzz", BuzzedUser);
                await Clients.Caller.SendAsync("ReceiveCurrentChatMessages", ChatMessages);
            }
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUsers.Remove(ConnectedUsersReal[Context.ConnectionId]);
            ConnectedUsersReal.Remove(Context.ConnectionId);
            
            await Clients.All.SendAsync("ReceiveAllUsers", ConnectedUsers);
        }

        public async Task ConnectUser(string user)
        {
            ConnectedUsers.Add(user);
            ConnectedUsersReal.Add(Context.ConnectionId, user);
            await Clients.All.SendAsync("ReceiveAllUsers", ConnectedUsers);
        }

        public async Task SendMessage(string user, string message)
        {
            if (message == "/reset")
            {
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.MessageType = MessageType.Notification;
                chatMessage.Name = user;
                chatMessage.Message = $"{user} has reset the buzzer";
                AddChatMessage(chatMessage);

                BuzzedUser = string.Empty;

                await Clients.All.SendAsync("ReceiveBuzzClear", chatMessage);
            }
            else if (!message.StartsWith('/'))
            {
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.Name = user;
                chatMessage.Message = message;
                AddChatMessage(chatMessage);

                await Clients.All.SendAsync("ReceiveMessage", chatMessage);
            }
            
        }

        public async Task Buzz(string user)
        {
            if (string.IsNullOrEmpty(BuzzedUser))
            {
                BuzzedUser = user;
                await Clients.All.SendAsync("ReceiveBuzz", BuzzedUser);
            }
            ChatMessage chatMessage = new ChatMessage();
            chatMessage.MessageType = MessageType.Notification;
            chatMessage.Name = user;
            chatMessage.Message = $"{user} has buzzed in!";
            AddChatMessage(chatMessage);
            await Clients.All.SendAsync("ReceiveMessage", chatMessage);

        }

        private void AddChatMessage(ChatMessage chatMessage)
        {
            ChatMessages.Add(chatMessage);
            if (ChatMessages.Count > 40)
            {
                ChatMessages.RemoveAt(0);
            }
        }
    }
}
