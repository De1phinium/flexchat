using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace flexchat
{
    class Conversations
    {
        private const int PHOTO_SIZE = 70;
        private const int TITLE_SIZE = 22;
        private const int TEXT_SIZE = 17;
        private const int offs = 5;
        private int Scroll = 0;
        private bool blockAsking = false;

        public StatusType status;
        private StatusType prev_status;
        private uint msgs;
        public DateTime last_read;

        public int id;
        public int creator_id;
        public int photo_id;
        public string title;
        public bool loaded;
        public int second_person;

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
            second_person = 0;
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

        public void AddMessage(int id, int sender_id, int conv_id, string text, DateTime sent, string att)
        {
            blockAsking = false;
            Message msg = new Message(id, sender_id, conv_id, text, sent);
            if (msg.sent > Program.LastMessageTime)
            {
                Program.LastMessageTime = msg.sent;
            }
            string s = "";
            int p = 0;
            while (att[p] != 0)
                s += att[p++];
            p++;
            int natt = int.Parse(s);
            for (int i = 0; i < natt; i++)
            {
                s = "";
                while (att[p] != 0)
                    s += att[p++];
                msg.AddAtt(int.Parse(s));
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
            string s = "\0";
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
            if (s == "\0")
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
                    rect.FillColor = Content.color1;
                    Program.wnd.Draw(rect);
                }
                scr = PHOTO_SIZE + offs*2;
                CircleShape photo = new CircleShape();
                photo.Radius = (PHOTO_SIZE - 6) / 2;
                photo.Position = new SFML.System.Vector2f(10, yc + ((PHOTO_SIZE + 10) / 2) - (photo.Radius));
                int TextureId = 0;
                if (second_person == creator_id)
                    TextureId = Content.CachedTextureId(photo_id);
                else
                {
                    foreach (Users u in Program.users)
                    {
                        if ((u.ID == second_person || u.ID == creator_id) && u.ID != Program.Me.ID)
                        {
                            TextureId = Content.CachedTextureId(u.photo_id);
                            break;
                        }
                    }
                }
                if (TextureId >= 0)
                    photo.Texture = Content.GetTexture(TextureId);
                Program.wnd.Draw(photo);
                Text text = new Text();
                text.Font = Content.font;
                text.Position = new SFML.System.Vector2f(80, yc + offs + 3);
                text.CharacterSize = TITLE_SIZE;
                if (status == StatusType.ACTIVE)
                    text.Color = Content.color1;
                else text.Color = Content.color2;
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
                    if (status == StatusType.ACTIVE)
                        text.Color = Content.color1;
                    else
                        text.Color = Content.color2;
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
                    if (Program.convmenu.Status == StatusType.BLOCKED)
                        Program.convmenu.Status = StatusType.ACTIVE;
                    RectangleShape titlebox = new RectangleShape(new SFML.System.Vector2f(Program.wnd.Size.X - Program.CHATS_WIDTH - 10, 75));
                    titlebox.Position = new SFML.System.Vector2f(Program.CHATS_WIDTH + 10, 0);
                    titlebox.Texture = Content.titlebox;
                    int t = 0;
                    for (int i = 0; i < messages.Count; i++)
                    {
                        int lastt = t;
                        t += messages[i].Draw(Convert.ToInt32(Program.wnd.Size.Y) - 100 - t - Scroll);
                        if (lastt == t)
                            break;
                    }
                    Program.wnd.Draw(titlebox);

                    titlebox.Texture = null;
                    titlebox.FillColor = Content.color1;
                    titlebox.Position = new SFML.System.Vector2f(Program.CHATS_WIDTH + 10, titlebox.Size.Y);
                    titlebox.Size = new SFML.System.Vector2f(titlebox.Size.X, 1);
                    Program.wnd.Draw(titlebox);

                    if (Program.convmenu.Status == StatusType.SELECTED)
                    {
                        if (Program.chgtitle.Status != StatusType.SELECTED)
                            Program.chgtitle.Status = StatusType.ACTIVE;
                    }
                    else
                    {
                        if (Program.chgtitle.Status == StatusType.SELECTED)
                            Program.convmenu.Status = StatusType.SELECTED;
                        else
                            Program.chgtitle.Status = StatusType.BLOCKED;
                    }

                    if (second_person == creator_id)
                    {
                        Program.convmenu.posX = Convert.ToInt32(Program.wnd.Size.X - Program.convmenu.sizeX);
                        Program.convmenu.Draw();
                        if (Program.chgtitle.Status != StatusType.BLOCKED)
                        {
                            Program.chgtitle.posY = 75;
                            Program.chgtitle.posX = Convert.ToInt32(Program.wnd.Size.X - Program.chgtitle.sizeX);
                            Program.chgtitle.Draw();
                        }
                    }

                    Text Ttext = new Text();
                    Ttext.CharacterSize = 50;
                    Ttext.DisplayedString = title;
                    if (second_person != creator_id)
                    {
                        title = "Private Conversation";
                        int userTitleSearch = -1;
                        if (second_person == Program.Me.ID)
                            userTitleSearch = creator_id;
                        else
                            userTitleSearch = second_person;

                        for (int i = 0; i < Program.users.Count; i++)
                        {
                            if (Program.users[i].ID == userTitleSearch)
                            {
                                title = Program.users[i].Login;
                                break;
                            }
                        }
                    }
                    Ttext.Position = new SFML.System.Vector2f(Program.CHATS_WIDTH + 22, 6);
                    Ttext.Font = Content.font;
                    Ttext.Color = Content.color1;
                    Program.wnd.Draw(Ttext);
                }
            }
            return scr;
        }

        public void RequestData(Network Client)
        {
            Client.SendData(Convert.ToString(id), 3);
        }

        public void UpdateMessages(SFML.Window.MouseButtonEventArgs args)
        {
            for (int i = 0; i < messages.Count; i++)
                messages[i].Update(args);
        }
        public void UpdateMessages(SFML.Window.MouseMoveEventArgs args)
        {
            for (int i = 0; i < messages.Count; i++)
                messages[i].Update(args);
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
                if (status != StatusType.BLOCKED)
                {
                    Program.StopRecording = true;
                }
                Program.SmthSelected = true;
                prev_status = StatusType.BLOCKED;
                status = prev_status;
                Program.ConvSelected = id;
                Program.UserSelected = -1;
                Program.convmenu.Status = StatusType.ACTIVE;
            }
            else
            {
                if (status == StatusType.BLOCKED)
                    Program.convmenu.Status = StatusType.BLOCKED;
                Program.chgtitle.Status = StatusType.BLOCKED;
                if (Program.ConvSelected == id)
                    Program.ConvSelected = -1;
                prev_status = StatusType.ACTIVE;
                status = prev_status;
            }
        }

        public void Update(SFML.Window.MouseWheelScrollEventArgs e)
        {
            if (e.Delta < 0)
            {
                if (Scroll + 40 > 0)
                    Scroll = 0;
                else Scroll += 40;
            }
            else
            {
                int sum = 0;
                for (int i = 0; i < messages.Count; i++)
                {
                    sum -= messages[i].drawsizey;
                }
                if (-sum >= Program.wnd.Size.Y - 170)
                {
                    Scroll -= 40;
                    if (Scroll < sum + (Program.wnd.Size.Y - 170))
                    {
                        Scroll = sum + Convert.ToInt32(Program.wnd.Size.Y - 170);
                        if (!blockAsking)
                        {
                            blockAsking = true;
                        }
                    }
                }
            }
        }
    }
}
