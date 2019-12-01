using System;
using SFML.Graphics;

namespace flexchat
{
    class Users
    {
        private const uint DATA_MIN_LENGTH = 4;
        private const uint PHOTO_SIZE = 60;
        private const uint CH_SIZE = 24;

        public uint photo_size 
        {
            get { return PHOTO_SIZE;  }
        }

        private string login;
        private int id;
        public uint photo_id;
        private StatusType prev_status;
        public StatusType status;

        public uint pos_x;
        public uint pos_y;

        public void Draw()
        {
            if (pos_y + PHOTO_SIZE <= 5 || pos_y >= Program.wnd.Size.Y - 5)
                return;
            RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(Program.WND_WIDTH / 4, PHOTO_SIZE + 10));
            rect.Position = new SFML.System.Vector2f(pos_x, pos_y);
            rect.FillColor = Color.Red;
            if (status == StatusType.ACTIVE)
            {
            }
            else if (status == StatusType.SELECTED)
            {
            }
            else
            {
            }
            Program.wnd.Draw(rect);
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
                Position = new SFML.System.Vector2f(pos_x + PHOTO_SIZE + 13, pos_y + 3 + PHOTO_SIZE / 2 - CH_SIZE / 2)
            };
            text.Color = Color.White;
            Program.wnd.Draw(text);
        }

        public string Login
        {
            get { return login;  }
            set { login = value; }
        }

        public uint photoID
        {
            get { return photo_id;  }
            set { photo_id = value; }
        }

        public int ID
        {
            get { return id;  }
            set { id = value; }
        }

        public Users(int id)
        {
            this.id = id;
            status = StatusType.ACTIVE;
        }

        public void RequestData(Network Client)
        {
            Client.SendData(Convert.ToString(id), 2);
        }

        public Users(string login, int id)
        {
            this.login = login;
            this.id = id;
            status = StatusType.ACTIVE;
        }

        public uint Authorize(Network Client, uint mode, string login, string pass)
        {
            if (login.Length < DATA_MIN_LENGTH || pass.Length < DATA_MIN_LENGTH)
            {
                Program.err.code = Error.ERROR_DATA_LENGTH;
                Program.err.text = "Minimum length of login and password is " + Convert.ToString(DATA_MIN_LENGTH);
                return 0;
            }
            string data = login + Convert.ToString((char)0) + pass;
            return Client.SendData(data, mode);
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
