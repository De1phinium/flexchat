using System;

namespace flexchat
{
    class Message
    {
        private int id;
        private int sender_id;
        private int conv_id;
        private DateTime sent;
        private string text;

        public Message(int id, int conv_id)
        {
            this.id = id;
            this.conv_id = conv_id;
        }

        public void Msg(int sender_id, DateTime sent)
        {
            this.sender_id = sender_id;
            this.sent = sent;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public string GetText()
        {
            return text;
        }
    }
}
