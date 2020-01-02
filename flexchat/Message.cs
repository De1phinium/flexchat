using System;
using SFML.Graphics;

namespace flexchat
{
    class Message
    {
        private const int PH_SIZE = 60;
        private const int TEXT_SIZE = 22;
        private const int TIME_SIZE = 12;
        private const int offs = 4;
        private const int offsetX = 8;

        public int id;
        public int sender_id;
        public int conv_id;
        public DateTime sent;
        public string text;

        public int min(int a, int b)
        {
            if (a < b) return a;
            else return b;
        }

        public int Draw(int yc)
        {
            int sy = 0;
            CircleShape photo = new CircleShape();
            photo.Radius = PH_SIZE / 2;
            int photoid = -1;
            Text login = new Text();
            login.DisplayedString = "";
            login.Font = Content.font;
            login.CharacterSize = TEXT_SIZE;
            login.Position = new SFML.System.Vector2f(offs*3 + PH_SIZE, offs);
            login.Color = Content.color2;
            Text TimeSent = new Text();
            TimeSent.DisplayedString = sent.ToString("U");
            TimeSent.CharacterSize = TIME_SIZE;
            TimeSent.Font = Content.font;
            TimeSent.Color = Content.color2;
            TimeSent.Position = new SFML.System.Vector2f(login.Position.X + 1, login.Position.Y + TEXT_SIZE + offs + 1);
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
                Users user = new Users("", sender_id);
                Program.users.Add(user);
                user.RequestData(Program.Client);
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
            OText.Color = Content.color2;
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
                while (xsize > Program.wnd.Size.X - OText.Position.X - offs * 2 - Program.CHATS_WIDTH)
                {
                    f = false;
                    left = OText.DisplayedString[OText.DisplayedString.Length - 1] + left;
                    OText.DisplayedString = OText.DisplayedString.Substring(0, OText.DisplayedString.Length - 1);
                    tsize = OText.GetLocalBounds();
                    xsize = Convert.ToInt32(tsize.Width);
                }
                outtext[ind] = OText.DisplayedString;
                outtext[ind + 1] = left;
                ind++;
                textlen++;
            } while (!f);
            sy += textlen * (TEXT_SIZE + offs * 2);

            photo.Position = new SFML.System.Vector2f(photo.Position.X + offsetX + Program.CHATS_WIDTH, yc + photo.Position.Y - sy);
            Program.wnd.Draw(photo);
            login.Position = new SFML.System.Vector2f(login.Position.X + offsetX + Program.CHATS_WIDTH, yc + login.Position.Y - sy);
            Program.wnd.Draw(login);
            TimeSent.Position = new SFML.System.Vector2f(TimeSent.Position.X + offsetX + Program.CHATS_WIDTH, yc + TimeSent.Position.Y - sy);
            Program.wnd.Draw(TimeSent);
            OText.Position = new SFML.System.Vector2f(OText.Position.X + offsetX + Program.CHATS_WIDTH, yc + OText.Position.Y - sy);
            OText.DisplayedString = outtext[0];
            Program.wnd.Draw(OText);
            for (int i = 1; i < textlen; i++)
            {
                OText.DisplayedString = outtext[i];
                OText.Position = new SFML.System.Vector2f(OText.Position.X, OText.Position.Y + TEXT_SIZE + offs * 2);
                Program.wnd.Draw(OText);
            }

            return sy + offs * 2;
        }
        public Message(int id, int sender_id, int conv_id, string text, DateTime sent)
        {
            this.id = id;
            this.sender_id = sender_id;
            this.conv_id = conv_id;
            this.text = text;
            this.sent = sent;
        }

        public string GetText()
        {
            return text;
        }
    }
}
