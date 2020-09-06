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

        public int id;
        public int sender_id;
        public int conv_id;
        public DateTime sent;
        public string text;
        public List<int> Attachments;

        public int min(int a, int b)
        {
            if (a < b) return a;
            else return b;
        }

        public int Draw(int yc)
        {
            if (drawsizey != 0 && yc <= -drawsizey)
                return 0;
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
                    photo.Texture = Content.cache[textureID].texture;
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

            RectangleShape box = new RectangleShape(new SFML.System.Vector2f(boxwidth + 20, sy + 6));
            box.Position = new SFML.System.Vector2f(Program.CHATS_WIDTH + 10, yc - sy);
            box.Texture = Content.msgbox;
            Program.wnd.Draw(box);
            photo.Position = new SFML.System.Vector2f(photo.Position.X + offsetX + Program.CHATS_WIDTH, yc + photo.Position.Y - sy);
            Program.wnd.Draw(photo);
            login.Position = new SFML.System.Vector2f(login.Position.X + offsetX + Program.CHATS_WIDTH, yc + login.Position.Y - sy);
            Program.wnd.Draw(login);
            TimeSent.Position = new SFML.System.Vector2f(TimeSent.Position.X + offsetX + Program.CHATS_WIDTH, yc + TimeSent.Position.Y - sy);
            Program.wnd.Draw(TimeSent);
            OText.Position = new SFML.System.Vector2f(OText.Position.X + offsetX + Program.CHATS_WIDTH, yc + OText.Position.Y - sy);
            OText.DisplayedString = outtext[0];
            while (OText.DisplayedString[0] == ' ')
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

            drawsizey = sy + offs * 2 + 5;
            return sy + offs * 2 + 5;
        }
        public Message(int id, int sender_id, int conv_id, string text, DateTime sent)
        {
            Attachments = new List<int>();
            this.id = id;
            this.sender_id = sender_id;
            this.conv_id = conv_id;
            this.text = text;
            this.sent = sent;
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
