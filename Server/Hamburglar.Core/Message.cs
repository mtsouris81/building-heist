using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    [Serializable]
    public class Message
    {
        public int i { get; set; }
        public int t { get; set; }
        public MessageType MessageType()
        {
            return (MessageType)t;
        }
    }
    [Serializable]
    public class TaggedMessage : Message
    {
        public string tag { get; set; }
    }
    [Serializable]
    public class PlayerMessages
    {
        public PlayerMessages()
        {
            Messages = new List<Message>();
        }
        private List<Message> Messages { get; set; }
        public int HighestMessageId { get; set; }
        public void Create(int type)
        {
            Messages.Add(new Message()
            {
                i = ++HighestMessageId,
                t = type
            });
        }
        public void Create(int type, string tag)
        {
            Messages.Add(new TaggedMessage()
            {
                i = ++HighestMessageId,
                t = type,
                tag = tag
            });
        }
        public List<Message> GetMessages(int highestRecievedMessage)
        {
            var removeList = this.Messages.Where(x => x.i <= highestRecievedMessage).ToList();
            foreach(var r in removeList)
            {
                this.Messages.Remove(r);
            }
            return this.Messages;
        }
    }
    [Serializable]
    public class MessageManager
    {
        public Dictionary<string, PlayerMessages> PlayerMessages = new Dictionary<string, PlayerMessages>(StringComparer.OrdinalIgnoreCase);

        public void Create(string playerId, MessageType type)
        {
            if (!PlayerMessages.ContainsKey(playerId))
                PlayerMessages.Add(playerId, new PlayerMessages());

            PlayerMessages[playerId].Create((int)type);
        }
        public void Create(string playerId, MessageType type, string tag)
        {
            if (!PlayerMessages.ContainsKey(playerId))
                PlayerMessages.Add(playerId, new PlayerMessages());

            PlayerMessages[playerId].Create((int)type, tag);
        }
        public List<Message> GetMessages(string playerId, int highestRecievedMessage)
        {
            if (!PlayerMessages.ContainsKey(playerId))
                return new List<Message>();

            return PlayerMessages[playerId].GetMessages(highestRecievedMessage);
        }
    }
    public enum MessageType
    {
        GameUpdate,
        RoomEntered,
        RoomExited,
        OpponentTrapped,
        OpponentCaught
    }
}
