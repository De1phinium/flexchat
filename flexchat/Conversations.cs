using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace flexchat
{
    class Conversations
    {
        private const uint PHOTO_SIZE = 60;
        private const uint TITLE_SIZE = 24;
        private const uint TEXT_SIZE = 18;
        public StatusType status;
        private StatusType prev_status;
        private uint msgs;

        public int id;
        public int creator_id;
        public int photo_id;
        public string title;

        public uint pos_x;
        public uint pos_y;

        List<Message> messages;

        public void DrawMessages()
        {
            int posx = Convert.ToInt32(Program.WND_WIDTH / 4);
            int posy = Convert.ToInt32(Program.wnd.Size.Y - 68);
            foreach (Message m in messages)
            {
                int pp = posy;
                posy -= m.Draw(posx, posy) + 3;
                RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(100, 2));
                rect.FillColor = Color.Red;
                rect.Position = new SFML.System.Vector2f(posx, pp);
                Program.wnd.Draw(rect);
                rect.Position = new SFML.System.Vector2f(posx, posy);
                Program.wnd.Draw(rect);
            }
        }

        public uint photo_size
        {
            get { return PHOTO_SIZE; }
        }

        public Conversations(int id)
        {
            this.id = id;
            messages = new List<Message>();
            msgs = 0;
            status = StatusType.ACTIVE;
            prev_status = status;
        }

        public void AskForMessages(Network Client)
        {
            string data = id.ToString() + System.Convert.ToString((char)0) + msgs.ToString();
            Client.SendData(data, 4);
        }

        public void AddMessage(int id, int sender_id, int conv_id, string text, DateTime sent)
        {
            Message msg = new Message(id, sender_id, conv_id, text, sent);
            messages.Add(msg);
        }

        public void Draw()
        {
            RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(Program.WND_WIDTH / 4, PHOTO_SIZE + 10));
            rect.Position = new SFML.System.Vector2f(pos_x, pos_y);
            if (status == StatusType.ACTIVE)
            {
                rect.FillColor = Content.color0_2;
            }
            else if (status == StatusType.SELECTED)
            {
                rect.FillColor = Content.color3;
            }
            else
            {
                rect.FillColor = Content.color0;
            }
            Program.wnd.Draw(rect);
            if (pos_y + PHOTO_SIZE <= 5 || pos_y >= Program.wnd.Size.Y - 5)
                return;
            CircleShape ph = new CircleShape();
            ph.Radius = PHOTO_SIZE / 2;
            ph.FillColor = Color.White;
            ph.Position = new SFML.System.Vector2f(pos_x + 5, pos_y + (Program.WND_HEIGHT / 8) / 2 - (PHOTO_SIZE / 2));
            Program.wnd.Draw(ph);
            Text text = new Text()
            {
                Font = Content.font,
                DisplayedString = title,
                CharacterSize = TITLE_SIZE,
                Position = new SFML.System.Vector2f(pos_x + PHOTO_SIZE + 13, pos_y + 7)
            };
            if (status == StatusType.ACTIVE)
            {
                text.Color = Color.White;
            }
            else if (status == StatusType.SELECTED)
            {
                text.Color = Content.color0_2;
            }
            else
            {
                text.Color = Content.color2;
            }
            Program.wnd.Draw(text);
            if (messages.Count > 0)
            {
                text.DisplayedString = messages[0].text;
                if (text.DisplayedString.Length * (TEXT_SIZE / 2) >= Program.WND_WIDTH / 4 - PHOTO_SIZE - 15)
                {
                    int k = Convert.ToInt32((Program.WND_WIDTH / 4 - PHOTO_SIZE - 15) / (TEXT_SIZE / 2));
                    if (text.DisplayedString.Length < k) k = text.DisplayedString.Length;
                    text.DisplayedString = text.DisplayedString.Substring(0, k) + "...";
                }
                if (status == StatusType.ACTIVE)
                {
                    text.Color = Content.color2;
                }
                else if (status == StatusType.SELECTED)
                {
                    text.Color = Content.color0_1;
                }
                else
                {
                    text.Color = Content.color2;
                }
                text.CharacterSize = TEXT_SIZE;
                text.Position = new SFML.System.Vector2f(pos_x + PHOTO_SIZE + 13, pos_y + 13 + TITLE_SIZE);
            }
            Program.wnd.Draw(text);
        }

        public void RequestData(Network Client)
        {
            Client.SendData(Convert.ToString(id), 3);
        }

        public void Update(SFML.Window.MouseMoveEventArgs e)
        {
            if (status == StatusType.BLOCKED)
                return;
            if (e.X >= pos_x && e.X <= pos_x + Program.WND_WIDTH / 4 && e.Y >= pos_y && e.Y <= pos_y + PHOTO_SIZE + 10)
            {
                if (status != StatusType.SELECTED)
                    prev_status = status;
                status = StatusType.SELECTED;
            }
            else
            {
                status = prev_status;
            }
        }

        public void Update(SFML.Window.MouseButtonEventArgs e)
        {
            if (e.Button == SFML.Window.Mouse.Button.Left && status == StatusType.SELECTED)
            {
                prev_status = StatusType.BLOCKED;
                status = prev_status;
            }
            else
            {
                prev_status = StatusType.ACTIVE;
                status = prev_status;
            }
        }
    }
}
