using System;
using SFML.Graphics;
using System.Windows.Forms;
using System.IO;

namespace flexchat
{
    class Users
    {
        private const int DATA_MIN_LENGTH = 4;
        private const int PHOTO_SIZE = 70;
        private const int AVA_SIZE = 200;
        private const int CH_SIZE = 28;
        private const int BCH_SIZE = 70;

        public uint photo_size 
        {
            get { return PHOTO_SIZE;  }
        }

        private string login;
        private int id;
        public int photo_id = -2;
        private StatusType prev_status;
        public StatusType status;
        public bool friend = false;
        public bool reqto = false;
        public int reqtoid = -1;
        public bool reqfrom = false;
        public int reqfromid = -1;
        public bool reqfromhid = false;

        public int pos_x;
        public int pos_y;

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
                photo.Radius = (PHOTO_SIZE - 6) / 2;
                photo.Position = new SFML.System.Vector2f(10, y + ((PHOTO_SIZE + 10) / 2) - (photo.Radius));
                Program.wnd.Draw(photo);
            }
            Text login = new Text();
            login.Font = Content.font;
            login.CharacterSize = CH_SIZE;
            if (status == StatusType.ACTIVE)
                login.Color = Content.color1;
            else login.Color = Content.color2;
            login.DisplayedString = Login;
            var lb = login.GetLocalBounds();
            login.Position = new SFML.System.Vector2f(80, y + ((PHOTO_SIZE + 10) / 2) - (lb.Height / 2) - 5);
            Program.wnd.Draw(login);

            if (status == StatusType.BLOCKED)
            {
                if (id != Program.Me.ID)
                {
                    Program.createconv.Draw();
                    if (Program.createconv.Clicked())
                    {
                        Program.Client.SendData(Convert.ToString(id), 300);
                        Program.createconv.Status = StatusType.BLOCKED;
                        Program.frreq.Status = StatusType.BLOCKED;
                        Program.accreq.Status = StatusType.BLOCKED;
                        Program.cancelreq.Status = StatusType.BLOCKED;
                        Program.remfr.Status = StatusType.BLOCKED;
                        status = StatusType.ACTIVE;
                        Program.Scroll = 0;
                        Program.mode = 0;
                        Program.UserSelected = -1;
                        Program.chgmode.LoadTextures(Content.chgmode[0], Content.chgmode[0], Content.chgmode[0]);
                    }

                    if (friend)
                    {
                        Program.frreq.Status = StatusType.BLOCKED;
                        Program.accreq.Status = StatusType.BLOCKED;
                        Program.cancelreq.Status = StatusType.BLOCKED;
                        if (Program.remfr.Status == StatusType.BLOCKED)
                            Program.remfr.Status = StatusType.ACTIVE;
                        Program.remfr.Draw();
                        if (Program.remfr.Clicked())
                        {
                            Program.Client.SendData(Convert.ToString(id), 103);
                        }
                    }
                    else
                    {
                        if (reqfrom)
                        {
                            Program.frreq.Status = StatusType.BLOCKED;
                            Program.cancelreq.Status = StatusType.BLOCKED;
                            Program.remfr.Status = StatusType.BLOCKED;
                            if (Program.accreq.Status == StatusType.BLOCKED)
                                Program.accreq.Status = StatusType.ACTIVE;
                            Program.accreq.Draw();
                            if (Program.accreq.Clicked())
                            {
                                Program.Client.SendData("0" + Convert.ToString((char)(0)) + Convert.ToString(reqfromid), 101);
                            }
                        }
                        else if (reqto)
                        {
                            Program.frreq.Status = StatusType.BLOCKED;
                            Program.accreq.Status = StatusType.BLOCKED;
                            Program.remfr.Status = StatusType.BLOCKED;
                            if (Program.cancelreq.Status == StatusType.BLOCKED)
                                Program.cancelreq.Status = StatusType.ACTIVE;
                            Program.cancelreq.Draw();
                            if (Program.cancelreq.Clicked() && reqtoid >= 0)
                            {
                                Program.Client.SendData("1" + Convert.ToString((char)(0)) + Convert.ToString(id), 100);
                            }
                        }
                        else
                        {
                            Program.cancelreq.Status = StatusType.BLOCKED;
                            Program.accreq.Status = StatusType.BLOCKED;
                            Program.remfr.Status = StatusType.BLOCKED;
                            if (Program.frreq.Status == StatusType.BLOCKED)
                                Program.frreq.Status = StatusType.ACTIVE;
                            Program.frreq.Draw();
                            if (Program.frreq.Clicked())
                            {
                                Program.Client.SendData("0" + Convert.ToString((char)(0)) + Convert.ToString(id), 100);
                            }
                        }
                    }
                }
                else
                {
                    if (Program.chgphoto.Status == StatusType.BLOCKED)
                        Program.chgphoto.Status = StatusType.ACTIVE;
                    Program.chgphoto.Draw();
                    if (Program.chgphoto.Clicked())
                    {

                    }
                }

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
            if (e.X <= Program.CHATS_WIDTH && e.Y >= pos_y && e.Y <= pos_y + PHOTO_SIZE + 10 && e.Y > Program.SEARCH_HEIGHT)
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
                if (id == Program.Me.id)
                    Program.chgphoto.Status = StatusType.ACTIVE;
                Program.createconv.Status = StatusType.ACTIVE;
                Program.Client.SendData(Convert.ToString(2) + Convert.ToString((char)(0)) + Convert.ToString(id), 100);
                if (friend)
                    Program.remfr.Status = StatusType.ACTIVE;
                else
                    Program.frreq.Status = StatusType.ACTIVE;
                Program.cancelreq.Status = StatusType.BLOCKED;
                Program.accreq.Status = StatusType.BLOCKED;
                if (!reqfrom)
                {
                    foreach (FriendRequest f in Program.frreqs)
                    {
                        if (f.from == id)
                        {
                            reqfrom = true;
                            reqfromhid = f.hidden;
                            reqfromid = f.id;
                        }
                    }
                }
                Program.UserSelected = id;
                Program.ConvSelected = -1;
                prev_status = StatusType.BLOCKED;
                status = prev_status;
            }
            else
            {
                if (id == Program.Me.id)
                    Program.chgphoto.Status = StatusType.BLOCKED;
                if (status == StatusType.BLOCKED)
                    Program.createconv.Status = StatusType.BLOCKED;
                if (Program.UserSelected == id)
                    Program.UserSelected = -1;
                prev_status = StatusType.ACTIVE;
                status = prev_status;
            }
        }
    }
}
