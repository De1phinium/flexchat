using System.Collections.Generic;

namespace flexchat
{
    class Conversations
    {
        private int id;
        private int creator_id;
        private int photo_id;
        private string title;

        List<Message> message;

        public Conversations(int id)
        {
            this.id = id;
            message = new List<Message>();
        }

        public void Conv(int creator_id, int photo_id, string title)
        {
            this.creator_id = creator_id;
            this.photo_id = photo_id;
            this.title = title;
        }
    }
}
