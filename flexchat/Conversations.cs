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
        public StatusType status;
        private StatusType prev_status;
        private uint msgs;

        public int id;
        public int creator_id;
        public int photo_id;
        public string title;
        public bool loaded;

        public uint pos_x;
        public uint pos_y;

        List<Message> messages;

        public void DrawMessages()
        {
            int posx = Convert.ToInt32(Program.WND_WIDTH / 4);
            int posy = Convert.ToInt32(Program.wnd.Size.Y - 68);
            int i = 0;
            foreach (Message m in messages)
            {
                if (posy < 3)
                    break;
                i++;
                posy -= m.Draw(posx, posy) + 3 + 5 * i;
            }
        }

        public uint photo_size
        {
            get { return PHOTO_SIZE; }
        }

        public Conversations(int id)
        {
            loaded = false;
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

        public void AddMessage(int id, int sender_id, int conv_id, string text, DateTime sent, bool read)
        {
            Message msg = new Message(id, sender_id, conv_id, text, sent, read);
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
            const int offs = 5;
            int scr = 0;
            if (loaded && (yc < Program.wnd.Size.Y && yc >= 0))
            {
                scr = PHOTO_SIZE + offs*2;
                CircleShape photo = new CircleShape();
                photo.Position = new SFML.System.Vector2f(offs, yc + offs + 2);
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
