using System;
using SFML.Graphics;

namespace flexchat
{
    class Users
    {
        private const int DATA_MIN_LENGTH = 4;
        private const int PHOTO_SIZE = 70;
        private const int AVA_SIZE = 200;
        private const int CH_SIZE = 24;
        private const int BCH_SIZE = 70;

        public uint photo_size 
        {
            get { return PHOTO_SIZE;  }
        }

        private string login;
        private int id;
        public int photo_id;
        private StatusType prev_status;
        public StatusType status;

        public uint pos_x;
        public uint pos_y;

        public string Login
        {
            get { return login;  }
            set { login = value; }
        }

        public int photoID
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

        public int Draw(int y)
        {
            int size = PHOTO_SIZE + 10;
            if (y < -PHOTO_SIZE - 10 || (y >= Program.wnd.Size.Y - PHOTO_SIZE - 40 && id != Program.Me.id))
                return 0;
            CircleShape photo = new CircleShape();
            int text_id = Content.CachedTextureId(photo_id);
            if (text_id != -1)
            {
                photo.Texture = Content.cache[text_id].texture;
                photo.Radius = PHOTO_SIZE / 2;
                photo.Position = new SFML.System.Vector2f(5, y + 5);
                Program.wnd.Draw(photo);
            }
            Text login = new Text();
            login.Font = Content.font;
            login.CharacterSize = CH_SIZE;
            login.Color = Content.colorLightGray;
            login.DisplayedString = Login;
            login.Position = new SFML.System.Vector2f(PHOTO_SIZE + 15, y + PHOTO_SIZE / 2 - 9);
            Program.wnd.Draw(login);

            if (status == StatusType.BLOCKED)
            {
                RectangleShape bg = new RectangleShape(new SFML.System.Vector2f(Program.wnd.Size.X - Program.CHATS_WIDTH, Program.wnd.Size.Y));
                bg.Position = new SFML.System.Vector2f(Program.CHATS_WIDTH, 0);
                bg.FillColor = Content.colorAlmostBlack;
                Program.wnd.Draw(bg);

                if (text_id != -1)
                {
                    RectangleShape ava = new RectangleShape(new SFML.System.Vector2f(AVA_SIZE, AVA_SIZE));
                    ava.Texture = photo.Texture;
                    ava.Position = new SFML.System.Vector2f(Program.CHATS_WIDTH + 70, 70);
                    Program.wnd.Draw(ava);
                }

                login.CharacterSize = BCH_SIZE;
                login.Position = new SFML.System.Vector2f(Program.CHATS_WIDTH + 100 + AVA_SIZE, 100);
                Program.wnd.Draw(login);
            }
            return size;
        }

        public void Update(SFML.Window.MouseMoveEventArgs e)
        {
            if (status == StatusType.BLOCKED)
                return;
            if (e.X <= Program.CHATS_WIDTH && e.Y >= pos_y && e.Y <= pos_y + PHOTO_SIZE + 10)
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
                Program.UserSelected = id;
                Program.ConvSelected = -1;
                prev_status = StatusType.BLOCKED;
                status = prev_status;
            }
            else
            {
                if (Program.UserSelected == id)
                    Program.UserSelected = -1;
                prev_status = StatusType.ACTIVE;
                status = prev_status;
            }
        }
    }
}
