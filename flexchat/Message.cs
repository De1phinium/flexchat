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
            int maxslen = Convert.ToInt32(Program.wnd.Size.X - (Program.WND_WIDTH / 4) - 10) / CharSize;
            Text Dtext = new Text()
            {
                Font = Content.font,
                Color = Color.White,
                CharacterSize = CharSize
            };
            string[] disp = new string[24];
            int p = 0, nDisp = 0;
            while (text.Length - p > maxslen)
            {
                if (p + maxslen > text.Length)
                    disp[nDisp] = text.Substring(p, text.Length - p);
                else
                    disp[nDisp] = text.Substring(p, maxslen);
                p += maxslen;
                nDisp++;
            }
            if (text.Substring(p, text.Length - p) != "")
            {
                disp[nDisp] = text.Substring(p, text.Length - p);
            }
            offset += nDisp * CharSize;
            for (int i = 1; i <= nDisp; i++)
            {
                RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(Program.wnd.Size.X, 2));
                rect.FillColor = Color.Blue;
                rect.Position = new SFML.System.Vector2f(0, posy - CharSize * i);
                Program.wnd.Draw(rect);
                Dtext.Position = new SFML.System.Vector2f(posx + 15, posy - CharSize * i);
                Dtext.DisplayedString = disp[i - 1];
                Program.wnd.Draw(Dtext);
            }
            Dtext.DisplayedString = "";
            CircleShape circ = new CircleShape();
            circ.Radius = (PH_SIZE - 5) / 2;
            circ.FillColor = Color.White;
            foreach (Users u in Program.users)
            {
                if (u.ID == sender_id)
                {
                    Dtext.DisplayedString = u.Login;
                    break;
                }
            }
            if (Program.Me.ID == sender_id)
            {
                Dtext.DisplayedString = Program.Me.Login;
            }
            offset += PH_SIZE;
            Dtext.CharacterSize = TitleSize;
            Dtext.Color = Color.White;
            Dtext.Position = new SFML.System.Vector2f(posx + PH_SIZE + 10, posy - offset + 5);
            Program.wnd.Draw(Dtext);
            RectangleShape rect2= new RectangleShape(new SFML.System.Vector2f(Program.wnd.Size.X, 2));
            rect2.FillColor = Color.Green;
            rect2.Position = new SFML.System.Vector2f(0, posy - offset);
            Program.wnd.Draw(rect2);
            Dtext.CharacterSize = CharSize;
            Dtext.DisplayedString = sent.ToString();
            Dtext.Position = new SFML.System.Vector2f(posx + PH_SIZE + 10, posy - offset + TitleSize + 4);
            Dtext.Color = Content.color2;
            Program.wnd.Draw(Dtext);
            circ.Position = new SFML.System.Vector2f(posx + 10, posy - offset + 4);
            Program.wnd.Draw(circ);
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
