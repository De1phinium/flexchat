using System;
using SFML.Graphics;

namespace flexchat
{
    class Message
    {
        private const int PH_SIZE = 50;

        public int id;
        public int sender_id;
        public int conv_id;
        public DateTime sent;
        public string text;

        public int Draw(int posx, int posy)
        {
            const int CharSize = 16;
            const int TitleSize = 18;
            string text = this.text;
            int offset = 0;
            int maxslen = Convert.ToInt32(Program.wnd.Size.X - (Program.WND_WIDTH / 4) - 10) / ((3*CharSize) / 5);
            Text Dtext = new Text()
            {
                Font = Content.font,
                Color = Color.White,
                CharacterSize = CharSize
            };
            string[] disp = new string[24];
            int p = 0, nDisp = 0;
            while (p + maxslen < text.Length)
            {
                disp[nDisp] = text.Substring(p, maxslen);
                nDisp++;
                p += maxslen;
            }
            if (p < text.Length - 1)
            {
                disp[nDisp] = text.Substring(p, text.Length - p);
                nDisp++;
            }
            for (int i = nDisp; i > 0; i--)
            {
                offset += CharSize;
                Dtext.Position = new SFML.System.Vector2f(posx + 18, posy - 5 - offset);
                Dtext.DisplayedString = disp[i - 1];
                Program.wnd.Draw(Dtext);
            }
            offset += PH_SIZE;
            CircleShape ph = new CircleShape();
            ph.FillColor = Color.White;
            ph.Radius = (PH_SIZE - 10) / 2;
            ph.Position = new SFML.System.Vector2f(posx + 10, posy - offset);
            Program.wnd.Draw(ph);
            if (Program.Me.ID == sender_id)
            {
                Dtext.DisplayedString = Program.Me.Login;
            }
            else
            {
                foreach (Users u in Program.users)
                {
                    if (u.ID == sender_id)
                    {
                        Dtext.DisplayedString = u.Login;
                        break;
                    }
                }
            }
            Dtext.CharacterSize = TitleSize;
            Dtext.Position = new SFML.System.Vector2f(posx + 7 + PH_SIZE, posy - offset);
            Program.wnd.Draw(Dtext);
            Dtext.CharacterSize = CharSize - 2;
            Dtext.Color = Color.Red; ;
            Dtext.DisplayedString = sent.ToString();
            Dtext.Position = new SFML.System.Vector2f(posx + 10 + PH_SIZE, posy - offset + TitleSize + 2);
            Program.wnd.Draw(Dtext);
            return offset;
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
