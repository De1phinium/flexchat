using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace flexchat
{
    class Conversations
    {
        private const int PHOTO_SIZE = 70;
        private const int TITLE_SIZE = 24;
        private const int TEXT_SIZE = 18;
        private const int offs = 5;
        private const int Scroll = 0;

        public StatusType status;
        private StatusType prev_status;
        private uint msgs;
        public DateTime last_read;

        public int id;
        public int creator_id;
        public int photo_id;
        public string title;
        public bool loaded;

        public int pos_x;
        public int pos_y;

        public List<Message> messages;

        public int DSize()
        {
            return PHOTO_SIZE + 2 * offs;
        }

        public uint photo_size
        {
            get { return PHOTO_SIZE; }
        }

        public Conversations(int id)
        {
            pos_x = 0;
            loaded = false;
            this.id = id;
            messages = new List<Message>();
            msgs = 0;
            status = StatusType.ACTIVE;
            prev_status = status;
        }

        public void AskForMessages(Network Client)
        {
            string data = id.ToString() + Convert.ToString((char)0) + msgs.ToString();
            Client.SendData(data, 4);
        }

        public void AddMessage(int id, int sender_id, int conv_id, string text, DateTime sent)
        {
            Message msg = new Message(id, sender_id, conv_id, text, sent);
            if (msg.sent > Program.LastMessageTime)
            {
                Program.LastMessageTime = msg.sent;
            }
            if (messages.Count == 0)
                messages.Add(msg);
            else
            {
                if (msg.sent < messages[messages.Count - 1].sent)
                {
                    messages.Add(msg);
                }
                else
                {
                    messages.Insert(0, msg);
                }
            }
        }

        string GetSenderName(int msgid)
        {
            string s = "";
            if (messages[msgid].sender_id == Program.Me.ID)
                s = "You";
            else foreach (Users u in Program.users)
            {
                    if (u.ID == messages[msgid].sender_id)
                    {
                        s = u.Login;
                        break;
                    }
            }
            if (s == "")
            {
                Users user = new Users("", messages[msgid].sender_id);
                Program.users.Add(user);
                user.RequestData(Program.Client);
            }
            return s;
        }

        public int Draw(int yc)
        {
            int scr = DSize();
            pos_y = yc;
            if (loaded && (yc < Program.wnd.Size.Y && yc >= -scr))
            {
                if (status != StatusType.ACTIVE)
                {
                    RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(Program.CHATS_WIDTH, scr));
                    rect.Position = new SFML.System.Vector2f(0, yc);
                    rect.FillColor = Content.colorLightGray;
                    Program.wnd.Draw(rect);
                }
                scr = PHOTO_SIZE + offs*2;
                CircleShape photo = new CircleShape();
                photo.Position = new SFML.System.Vector2f(offs, yc + offs);
                photo.Radius = (PHOTO_SIZE / 2);
                int TextureId = Content.CachedTextureId(photo_id);
                if (TextureId >= 0)
                    photo.Texture = Content.cache[TextureId].texture;
                Program.wnd.Draw(photo);
                Text text = new Text();
                text.Font = Content.font;
                text.Position = new SFML.System.Vector2f(offs * 2 + 2 + PHOTO_SIZE, yc + offs + 3);
                text.CharacterSize = TITLE_SIZE;
                text.Color = Color.White;
                text.DisplayedString = title;
                bool f = true, changed = false;
                do
                {
                    f = true;
                    FloatRect tsize = text.GetLocalBounds();
                    int sizex = Convert.ToInt32(tsize.Width);
                    if (sizex >= Program.CHATS_WIDTH - text.Position.X - offs)
                    {
                        f = false;
                        if (!changed) text.DisplayedString = "..." + text.DisplayedString.Substring(0, text.DisplayedString.Length - 1);
                        else text.DisplayedString = text.DisplayedString.Substring(0, text.DisplayedString.Length - 1);
                        changed = true;
                    }
                } while (!f);
                if (changed)
                {
                    text.DisplayedString = text.DisplayedString.Substring(3, text.DisplayedString.Length - 3) + "...";
                }
                Program.wnd.Draw(text);
                if (messages.Count > 0)
                {
                    text.Position = new SFML.System.Vector2f(text.Position.X + 1, text.Position.Y + TITLE_SIZE + offs * 2);
                    text.CharacterSize = TEXT_SIZE;
                    text.DisplayedString = GetSenderName(0) + ": " + messages[0].text;
                    text.Color = Color.White;
                    f = true;
                    changed = false;
                    do
                    {
                        f = true;
                        FloatRect tsize = text.GetLocalBounds();
                        int sizex = Convert.ToInt32(tsize.Width);
                        if (sizex >= Program.CHATS_WIDTH - text.Position.X - offs)
                        {
                            f = false;
                            if (!changed) text.DisplayedString = "..." + text.DisplayedString.Substring(0, text.DisplayedString.Length - 1);
                            else text.DisplayedString = text.DisplayedString.Substring(0, text.DisplayedString.Length - 1);
                            changed = true;
                        }
                    } while (!f);
                    if (changed)
                    {
                        text.DisplayedString = text.DisplayedString.Substring(3, text.DisplayedString.Length - 3) + "...";
                    }
                    Program.wnd.Draw(text);
                }
                if (status == StatusType.BLOCKED)
                {
                    int t = 0;
                    for (int i = 0; i < messages.Count; i++)
                    {
                        t += messages[i].Draw(Convert.ToInt32(Program.wnd.Size.Y) - 80 - t - Scroll);
                    }
                }
            }
            return scr;
        }

        public void RequestData(Network Client)
        {
            Client.SendData(Convert.ToString(id), 3);
        }

        public void Update(SFML.Window.MouseMoveEventArgs e)
        {
            if (status == StatusType.BLOCKED)
                return;
            if (e.X >= pos_x && e.X <= pos_x + Program.CHATS_WIDTH && e.Y >= pos_y && e.Y <= pos_y + DSize() && e.X > Program.SEARCH_HEIGHT)
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
                Program.SmthSelected = true;
                prev_status = StatusType.BLOCKED;
                status = prev_status;
                Program.ConvSelected = id;
                Program.UserSelected = -1;
            }
            else
            {
                if (Program.ConvSelected == id)
                    Program.ConvSelected = -1;
                prev_status = StatusType.ACTIVE;
                status = prev_status;
            }
        }
    }
}
