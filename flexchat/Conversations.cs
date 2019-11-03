using System.Collections.Generic;

namespace flexchat
{
    class Conversations
    {
        public uint id;
        public uint creator_id;
        public uint photo_id;
        public string title;

        List<Message> message;

        public Conversations(uint id)
        {
            this.id = id;
            message = new List<Message>();
        }

        public void RequestData(Network Client)
        {
            Client.SendData(System.Convert.ToString(id), 3);
        }
    }
}
