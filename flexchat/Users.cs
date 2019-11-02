using System;
using SFML.Graphics;

namespace flexchat
{
    class Users
    {
        private const uint DATA_MIN_LENGTH = 4;
        private const uint PHOTO_SIZE = 60;
        private const uint CH_SIZE = 24;

        private string login;
        private uint id;
        public uint photo_id;

        public uint pos_x;
        public uint pos_y;

        public void Draw()
        {
            CircleShape ph = new CircleShape();
            ph.Radius = PHOTO_SIZE / 2;
            ph.FillColor = Color.White;
            ph.Position = new SFML.System.Vector2f(pos_x + 5, pos_y + (Program.WND_HEIGHT / 8) / 2 - (PHOTO_SIZE / 2));
            Program.wnd.Draw(ph);
            Text text = new Text()
            {
                Font = Content.font,
                DisplayedString = login,
                CharacterSize = CH_SIZE,
                Color = Color.White,
                Position = new SFML.System.Vector2f(pos_x + PHOTO_SIZE + 13, pos_y + 8)
            };
            Program.wnd.Draw(text);
        }

        public string Login
        {
            get { return login;  }
            set { login = value; }
        }

        public uint ID
        {
            get { return id;  }
            set { id = value; }
        }

        public Users()
        {

        }

        public Users(string login, uint id)
        {
            this.login = login;
            this.id = id;
        }

        public uint Authorize(Network Client, uint mode, string login, string pass)
        {
            if (login.Length < DATA_MIN_LENGTH || pass.Length < DATA_MIN_LENGTH)
            {
                Program.err.code = Error.ERROR_DATA_LENGTH;
                Program.err.text = "Minimum length of login and password is " + Convert.ToString(DATA_MIN_LENGTH);
                return 0;
            }
            string data = Convert.ToString(mode) + Convert.ToString((char)0) + login + Convert.ToString((char)0) + pass;
            return Client.SendData(data, mode);
        }

    }
}
