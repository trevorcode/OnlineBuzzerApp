using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineBuzzerApp.Shared
{
    public class ChatMessage
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public MessageType MessageType { get; set; } = MessageType.ChatMessage;

        public ChatMessage()
        {
            DateTime = DateTime.UtcNow;
        }
    }

    public enum MessageType {
        ChatMessage,
        Notification
    }
}
