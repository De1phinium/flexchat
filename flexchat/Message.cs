using System;
using SFML.Graphics;
using System.Collections.Generic;

namespace flexchat
{
    class Message
    {
        private const int PH_SIZE = 60;
        private const int TEXT_SIZE = 22;
        private const int TIME_SIZE = 12;
        private const int offs = 4;
        private const int offsetX = 12;

        public int drawsizey = 0;
        public bool MsgActive;
        public bool PlaybackActive;
        public double VMProgress = 0;

        public int id;
        public int sender_id;
        public int conv_id;
        public DateTime sent;
        public string text;
        public List<int> Attachments;
        public List<Button> buttons;

        private int min(int a, int b)
        {
            if (a < b) return a;
            else return b;
        }
        private float min(float a, float b)
        {
            if (a < b) return a;
            else return b;
        }

        public int Draw(int yc)
        {
            MsgActive = false;
            if (drawsizey != 0 && yc <= -drawsizey)
                return 0;
            MsgActive = true;
            int sy = 0;
            CircleShape photo = new CircleShape();
            photo.Radius = PH_SIZE / 2;
            int photoid = -1;
            Text login = new Text();
            login.DisplayedString = "";
            login.Font = Content.font;
            login.CharacterSize = TEXT_SIZE;
            login.Position = new SFML.System.Vector2f(offs*3 + PH_SIZE, offs);
            login.Color = Content.color1;
            var loginbounds = login.GetLocalBounds();
            int boxwidth = PH_SIZE + Convert.ToInt32(loginbounds.Width);
            Text TimeSent = new Text();
            TimeSent.DisplayedString = sent.ToString("dd.MM.yyyy HH:mm:ss.fffzzz").Remove(16,13);
            TimeSent.CharacterSize = TIME_SIZE;
            TimeSent.Font = Content.font;
            TimeSent.Color = Content.color1;
            TimeSent.Position = new SFML.System.Vector2f(login.Position.X + 1, login.Position.Y + TEXT_SIZE + offs + 1);
            var tbounds = TimeSent.GetLocalBounds();
            if (boxwidth < PH_SIZE + Convert.ToInt32(tbounds.Width))
                boxwidth = PH_SIZE + Convert.ToInt32(tbounds.Width);
            boxwidth += 5;
            if (sender_id == Program.Me.ID)
            {
                photoid = Program.Me.photo_id;
                login.DisplayedString = Program.Me.Login;
            }
            else foreach (Users u in Program.users)
                {
                    if (u.ID == sender_id)
                    {
                        photoid = u.photo_id;
                        login.DisplayedString = u.Login;
                    }
                }
            if (photoid == -1)
            {
                if (photoid != -2)
                {
                    Users user = new Users("", sender_id);
                    user.photo_id = -2;
                    Program.users.Add(user);
                    user.RequestData(Program.Client);
                }
            }
            else
            {
                int textureID = Content.CachedTextureId(photoid);
                if (textureID >= 0)
                {
                    photo.Texture = Content.GetTexture(textureID);
                    photo.Texture.Smooth = true;
                    int x = min(Convert.ToInt32(photo.Texture.Size.X), Convert.ToInt32(photo.Texture.Size.Y));
                    photo.TextureRect = new IntRect(new SFML.System.Vector2i(0, 0), new SFML.System.Vector2i(x, x));
                }
                photo.Position = new SFML.System.Vector2f(offs, offs);
                sy += PH_SIZE + (offs * 2);
            }
            string[] outtext = new string[64];
            int textlen = 0;
            outtext[0] = text;
            Text OText = new Text();
            OText.Font = Content.font;
            OText.Color = Content.color1;
            OText.CharacterSize = TEXT_SIZE;
            OText.Position = new SFML.System.Vector2f(offs * 3, offs + 2 + PH_SIZE);
            int ind = 0;
            bool f;
            List<int> attachmentFileId = new List<int>();
            do
            {
                if (outtext[ind] == "") break;
                f = true;
                OText.DisplayedString = outtext[ind];
                FloatRect tsize = OText.GetLocalBounds();
                int xsize = Convert.ToInt32(tsize.Width);
                string left = "";
                while (xsize > 2 * (Program.wnd.Size.X - Program.CHATS_WIDTH - 20) / 3)
                {
                    f = false;
                    int lb = OText.DisplayedString.Length - 1;
                    while (lb > 0 && OText.DisplayedString[lb] != ' ')
                    {
                        lb--;
                    }
                    string backup = OText.DisplayedString;
                    OText.DisplayedString = OText.DisplayedString.Substring(0, lb);
                    var locbounds = OText.GetLocalBounds();
                    float dif1 = (2 * (Program.wnd.Size.X - Program.CHATS_WIDTH - 20) / 3) - locbounds.Width;
                    left = backup.Substring(lb, backup.Length - lb) + left;
                    if (dif1 > 50)
                    {
                        OText.DisplayedString = backup.Substring(0, backup.Length - ((backup.Length - lb) / 2));
                        locbounds = OText.GetLocalBounds();
                        float dif2 = (2 * (Program.wnd.Size.X - Program.CHATS_WIDTH - 20) / 3) - locbounds.Width;
                        if (Math.Abs(dif1) <= Math.Abs(dif2))
                        {
                            OText.DisplayedString = backup.Substring(0, lb);
                        }
                        else
                        {
                            left = backup.Substring(backup.Length - lb / 2, lb / 2) + left;
                        }
                    }
                    tsize = OText.GetLocalBounds();
                    xsize = Convert.ToInt32(tsize.Width);
                }
                var lbd = OText.GetLocalBounds();
                if (lbd.Width > boxwidth + 10)
                    boxwidth = Convert.ToInt32(lbd.Width) + 10;
                while (OText.DisplayedString[0] == ' ')
                    OText.DisplayedString = OText.DisplayedString.Substring(1, OText.DisplayedString.Length - 1);
                outtext[ind] = OText.DisplayedString;
                outtext[ind + 1] = left;
                ind++;
                textlen++;
            } while (!f);
            sy += textlen * (TEXT_SIZE + offs * 2);
            foreach (int att in Attachments)
            {
                int fileId = Content.SearchFileInCache(att);
                if (fileId != -1)
                {
                    attachmentFileId.Add(fileId);
                    int type = Content.GetFileType(fileId);
                    if (type == 1)
                    {
                        buttons[0].posY = sy + offs;
                        buttons[1].posY = sy + offs;
                        sy += Convert.ToInt32(buttons[1].sizeY) + offs * 3;
                    }
                }
            }

            const int CornerSize = 40;
            int BoxWidth = boxwidth + 20, BoxHeight = sy + 6, BoxX = Program.CHATS_WIDTH + 10, BoxY = yc - sy;
            if (sender_id == Program.Me.ID)
                BoxX = Convert.ToInt32(Program.WND_WIDTH) - 10 - BoxWidth;
            RectangleShape box = new RectangleShape(new SFML.System.Vector2f(BoxWidth - 2 * CornerSize, CornerSize));
            box.Position = new SFML.System.Vector2f(BoxX + CornerSize, BoxY);
            //box.Texture = Content.msgbox;
            box.FillColor = Content.color2;
            Program.wnd.Draw(box);
            box.Position = new SFML.System.Vector2f(BoxX + CornerSize, BoxY + BoxHeight - CornerSize);
            Program.wnd.Draw(box);
            box.Size = new SFML.System.Vector2f(BoxWidth, BoxHeight - 2 * CornerSize);
            box.Position = new SFML.System.Vector2f(BoxX, BoxY + CornerSize);
            Program.wnd.Draw(box);
            box.Size = new SFML.System.Vector2f(CornerSize, CornerSize);
            if (sender_id == Program.Me.ID)
                box.Position = new SFML.System.Vector2f(BoxX + BoxWidth - CornerSize, BoxY);
            else
                box.Position = new SFML.System.Vector2f(BoxX, BoxY);
            Program.wnd.Draw(box);
            box.FillColor = Color.White;
            if (sender_id == Program.Me.ID)
            {
                box.Texture = Content.msgBoxCornerNW;
                box.Position = new SFML.System.Vector2f(BoxX, BoxY);
            }
            else
            {
                box.Texture = Content.msgBoxCornerNE;
                box.Position = new SFML.System.Vector2f(BoxX + BoxWidth - CornerSize, BoxY);
            }
            Program.wnd.Draw(box);
            Program.wnd.Draw(box);
            box.Texture = Content.msgBoxCornerSW;
            box.Position = new SFML.System.Vector2f(BoxX, BoxY + BoxHeight - CornerSize);
            Program.wnd.Draw(box);
            box.Texture = Content.msgBoxCornerSE;
            box.Position = new SFML.System.Vector2f(BoxX + BoxWidth - CornerSize, BoxY + BoxHeight - CornerSize);
            Program.wnd.Draw(box);

            photo.Position = new SFML.System.Vector2f(photo.Position.X + BoxX, yc + photo.Position.Y - sy);
            Program.wnd.Draw(photo);
            login.Position = new SFML.System.Vector2f(login.Position.X + BoxX, yc + login.Position.Y - sy);
            Program.wnd.Draw(login);
            TimeSent.Position = new SFML.System.Vector2f(TimeSent.Position.X + BoxX, yc + TimeSent.Position.Y - sy);
            Program.wnd.Draw(TimeSent);
            OText.Position = new SFML.System.Vector2f(OText.Position.X + BoxX, yc + OText.Position.Y - sy);
            OText.DisplayedString = outtext[0];
            while (OText.DisplayedString.Length > 0 && OText.DisplayedString[0] == ' ')
                OText.DisplayedString = OText.DisplayedString.Substring(1, OText.DisplayedString.Length - 1);
            Program.wnd.Draw(OText);
            for (int i = 1; i < textlen; i++)
            {
                OText.DisplayedString = outtext[i];
                if (OText.DisplayedString.Length < 1)
                    break;
                while (OText.DisplayedString[0] == ' ')
                    OText.DisplayedString = OText.DisplayedString.Substring(1, OText.DisplayedString.Length - 1);
                OText.Position = new SFML.System.Vector2f(OText.Position.X, OText.Position.Y + TEXT_SIZE + offs * 2);
                Program.wnd.Draw(OText);
            }

            foreach (var id in attachmentFileId)
            {
                int type = Content.GetFileType(id);
                if (type == 1)
                {
                    buttons[0].posX = BoxX + 10;
                    buttons[0].posY = Convert.ToInt32(yc + buttons[0].posY - sy);
                    buttons[1].posX = buttons[0].posX;
                    buttons[1].posY = buttons[0].posY;
                    RectangleShape PlaybackStatus = new RectangleShape(new SFML.System.Vector2f(BoxWidth - buttons[0].sizeX - 10 - 4 * offs, 4));
                    PlaybackStatus.Position = new SFML.System.Vector2f(buttons[0].posX + buttons[0].sizeX + offs + 2, buttons[0].posY + buttons[0].sizeY / 2 - 2);
                    PlaybackStatus.FillColor = Content.color1;
                    Program.wnd.Draw(PlaybackStatus);
                    if (buttons[0].Status == buttons[1].Status & buttons[0].Status == StatusType.BLOCKED)
                    {
                        buttons[0].Status = StatusType.ACTIVE;
                        buttons[0].Draw();
                    }
                    else
                    {
                        if (buttons[0].Clicked())
                        {
                            buttons[0].Status = StatusType.BLOCKED;
                            buttons[1].Status = StatusType.ACTIVE;
                            if (PlaybackActive)
                                Program.Player.PauseVoiceMessage();
                            else
                            {
                                VMProgress = 0;
                                Program.Player.PlayVoiceMessage(conv_id, this.id, id);
                                PlaybackActive = true;
                            }
                            PlaybackStatus.Position = new SFML.System.Vector2f(min(PlaybackStatus.Position.X - 3, PlaybackStatus.Position.X + PlaybackStatus.Size.X - 6), PlaybackStatus.Position.Y - 4);
                            PlaybackStatus.Size = new SFML.System.Vector2f(6, 12);
                            Program.wnd.Draw(PlaybackStatus);
                            PlaybackStatus.Position = new SFML.System.Vector2f(PlaybackStatus.Position.X + 1, PlaybackStatus.Position.Y + 1);
                            PlaybackStatus.Size = new SFML.System.Vector2f(PlaybackStatus.Size.X - 2, PlaybackStatus.Size.Y - 2);
                            PlaybackStatus.FillColor = Content.color2;
                            Program.wnd.Draw(PlaybackStatus);
                        }
                        else if (buttons[1].Clicked())
                        {
                            buttons[1].Status = StatusType.BLOCKED;
                            buttons[0].Status = StatusType.ACTIVE;
                            Program.Player.PauseVoiceMessage();
                        }
                        if (buttons[0].Status != StatusType.BLOCKED)
                            buttons[0].Draw();
                        else if (buttons[1].Status != StatusType.BLOCKED)
                        {
                            buttons[1].Draw();
                            VMProgress = Program.Player.VoiceMessageProgress();
                            if (VMProgress >= 1.0)
                            {
                                VMProgress = 1.0;
                                Program.Player.StopVoiceMessage();
                            }
                            PlaybackStatus.Position = new SFML.System.Vector2f(min(PlaybackStatus.Position.X + Convert.ToInt32(Convert.ToDouble(PlaybackStatus.Size.X) * VMProgress - 3), PlaybackStatus.Position.X + PlaybackStatus.Size.X - 6), PlaybackStatus.Position.Y - 4);
                            PlaybackStatus.Size = new SFML.System.Vector2f(6, 12);
                            Program.wnd.Draw(PlaybackStatus);
                            PlaybackStatus.Position = new SFML.System.Vector2f(PlaybackStatus.Position.X + 1, PlaybackStatus.Position.Y + 1);
                            PlaybackStatus.Size = new SFML.System.Vector2f(PlaybackStatus.Size.X - 2, PlaybackStatus.Size.Y - 2);
                            PlaybackStatus.FillColor = Content.color2;
                            Program.wnd.Draw(PlaybackStatus);
                        }
                    }
                }
            }

            drawsizey = sy + offs * 2 + 5;
            return sy + offs * 2 + 5;
        }
        public Message(int id, int sender_id, int conv_id, string text, DateTime sent)
        {
            PlaybackActive = false;
            Attachments = new List<int>();
            this.id = id;
            this.sender_id = sender_id;
            this.conv_id = conv_id;
            this.text = text;
            this.sent = sent;
            buttons = new List<Button>();
            var  PlayVoiceMsgBut = new Button("", 40, 40, StatusType.BLOCKED);
            PlayVoiceMsgBut.LoadTextures(Content.playvoicemsg, Content.playvoicemsg, Content.playvoicemsg);
            buttons.Add(PlayVoiceMsgBut);
            var PauseVoiceMsgBut = new Button("", 40, 40, StatusType.BLOCKED);
            PauseVoiceMsgBut.LoadTextures(Content.pausevoicemsg, Content.pausevoicemsg, Content.pausevoicemsg);
            buttons.Add(PauseVoiceMsgBut);
        }

        public void Update(SFML.Window.MouseMoveEventArgs e)
        {
            foreach (Button b in buttons)
            {
                if (MsgActive)
                {
                    b.Update(e);
                    b.posY = Convert.ToInt32(Convert.ToUInt32(drawsizey));
                }
                else b.Status = StatusType.BLOCKED;
            }
        }

        public void Update(SFML.Window.MouseButtonEventArgs e)
        {
            foreach (Button b in buttons)
            {
                if (MsgActive)
                {
                    b.Update(e);
                    b.posY = Convert.ToInt32(Convert.ToUInt32(drawsizey));
                }
                else b.Status = StatusType.BLOCKED;
            }
        }

        public void AddAtt(int id)
        {
            Attachments.Add(id);
        }

        public string GetText()
        {
            return text;
        }
    }
}
