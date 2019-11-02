using System;
using System.Threading;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace flexchat
{
    class Program
    {
        public static uint WND_WIDTH = 700;
        public static uint WND_HEIGHT = 500;

        public static RenderWindow wnd = new RenderWindow(new VideoMode(WND_WIDTH, WND_HEIGHT), "FLEXCHAT");

        public static List<TextBox> textBoxes = new List<TextBox>();
        public static List<Button> buttons = new List<Button>();

        private static Int64 session_key = 0;
        public static Users Me = new Users();

        public Int64 SessionKey
        {
            set { session_key = value; }
        }

        public static Error err;

        private static Network Client;

        private static Thread nw;

        public static bool Closed = false;

        public static List<Network.tRequest> Resp;
        private static List<Conversations> convs;
        private static List<Users> users;
        private static List<int> friends;

        static void Main()
        {
            Resp = new List<Network.tRequest>();
            convs = new List<Conversations>();
            users = new List<Users>();
            friends = new List<int>();

            wnd.SetVerticalSyncEnabled(true);

            wnd.Closed += Win_Closed;
            wnd.Resized += Win_Resized;
            wnd.KeyReleased += Win_KeyReleased;
            wnd.TextEntered += Win_TextEntered;
            wnd.MouseButtonReleased += Win_MouseButtonReleased;
            wnd.MouseMoved += Win_MouseMoved;

            Content.Load();

            err = new Error(20, Color.Red);
            err.Clear();

            Client = new Network("192.168.1.10", 8081);
            nw = new Thread(Client.WaitForResponse);
            nw.Start();

            TextBox loginInput = new TextBox("login", 500, 50, StatusType.ACTIVE);
            loginInput.LoadTextures(Content.textbox0, Content.textbox0, Content.textbox0);
            loginInput.SetCursor(40, true, "|");
            loginInput.SetTextColor(Content.color2, Color.White, Content.color0);
            loginInput.textLengthBound = 24;
            textBoxes.Add(loginInput);

            TextBox passInput = new TextBox("password", 500, 50, StatusType.ACTIVE);
            passInput.LoadTextures(Content.textbox0, Content.textbox0, Content.textbox0);
            passInput.SetCursor(40, true, "|");
            passInput.SetTextColor(Content.color2, Color.White, Content.color0);
            passInput.textLengthBound = 24;
            passInput.SetSub("*");
            textBoxes.Add(passInput);

            Button submitButton = new Button("Sign in", 140, 60, StatusType.ACTIVE);
            submitButton.LoadTextures(Content.button0, Content.button1, Content.button2);
            submitButton.SetTextColor(Color.White, Content.color3, Content.color1);
            submitButton.textSize = 30;
            buttons.Add(submitButton);

            Button chgButton = new Button("Sign up", 80, 12, StatusType.ACTIVE);
            chgButton.LoadTextures(null, null, null);
            chgButton.SetTextColor(Content.color3, Color.White, Content.color1);
            chgButton.textSize = 16;
            buttons.Add(chgButton);

            uint mode = 0;

            Me.pos_x = 0;
            Me.pos_y = 0;

            while (wnd.IsOpen)
            {
                wnd.DispatchEvents();

                wnd.Clear(Content.color0);

                err.posX = (wnd.Size.X / 2) - (loginInput.sizeX / 2) + 5;
                err.posY = (wnd.Size.Y / 2) - loginInput.sizeY - 60 - err.textSize;
                err.Draw();

                if (session_key == 0)
                {
                    loginInput.posX = (wnd.Size.X / 2) - (loginInput.sizeX / 2);
                    loginInput.posY = (wnd.Size.Y / 2) - loginInput.sizeY - 50;
                    loginInput.Draw();

                    passInput.posX = (wnd.Size.X / 2) - (passInput.sizeX / 2);
                    passInput.posY = (wnd.Size.Y / 2) - 30;
                    passInput.Draw();

                    submitButton.posX = (wnd.Size.X / 2) - (submitButton.sizeX / 2);
                    submitButton.posY = (wnd.Size.Y / 2) + 50;
                    submitButton.Draw();

                    chgButton.posX = (wnd.Size.X / 2) + (passInput.sizeX / 2) - (chgButton.sizeX);
                    chgButton.posY = (wnd.Size.Y / 2) + passInput.sizeY / 2 + 5;
                    chgButton.Draw();

                    if (submitButton.Clicked())
                    {
                        uint r_id = Me.Authorize(Client, mode, loginInput.Text(), passInput.Text());
                        if (err.code != Error.ERROR_DATA_LENGTH)
                        {
                            submitButton.Status = StatusType.BLOCKED;
                            chgButton.Status = StatusType.BLOCKED;
                            loginInput.Status = StatusType.BLOCKED;
                            passInput.Status = StatusType.BLOCKED;
                        }
                    }

                    if (chgButton.Clicked())
                    {
                        mode = 1 - mode;
                        string s = chgButton.Text;
                        chgButton.Text = submitButton.Text;
                        submitButton.Text = s;
                        loginInput.Clear();
                        passInput.Clear();
                    }
                }
                else
                {
                    // IF AUTHENTHICATED
                    RectangleShape bg = new RectangleShape(new SFML.System.Vector2f(WND_WIDTH / 4, WND_HEIGHT/ 8));
                    bg.Position = new SFML.System.Vector2f(0, 0);
                    bg.FillColor = Content.color0_1;
                    wnd.Draw(bg);
                    bg.Position = new SFML.System.Vector2f(0, WND_HEIGHT / 8);
                    bg.Size = new SFML.System.Vector2f(bg.Size.X, wnd.Size.Y - bg.Size.Y);
                    bg.FillColor = Content.color0_2;
                    wnd.Draw(bg);
                    Me.Draw();
                }
                if (Resp.Count > 0)
                {
                    List<Network.tRequest> toDelete = new List<Network.tRequest>();
                    foreach (Network.tRequest r in Resp)
                    {
                        uint rMode = r.mode;
                        string respond = r.respond;
                        if (respond != null)
                        {
                            int p = 0;
                            while (respond[p] != 0)
                            {
                                p++;
                            }
                            p++;
                            string s = "";
                            switch (rMode)
                            {
                                case 0:
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    if (s == "")
                                    {
                                        err.code = Error.ERROR_WRONG_DATA;
                                        err.text = "WRONG LOGIN OR PASSWORD";
                                        submitButton.Status = StatusType.ACTIVE;
                                        chgButton.Status = StatusType.ACTIVE;
                                        loginInput.Status = StatusType.ACTIVE;
                                        passInput.Status = StatusType.ACTIVE;
                                    }
                                    else
                                    {
                                        Me.ID = uint.Parse(s);
                                        Me.Login = loginInput.Text();
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        session_key = Int64.Parse(s);
                                        s = "";
                                        p++;
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        Me.photo_id = uint.Parse(s);
                                        mode = 0;
                                        WND_WIDTH = 900;
                                        WND_HEIGHT = 600;
                                        wnd.Size = new SFML.System.Vector2u(WND_WIDTH, WND_HEIGHT);
                                    }
                                    break;
                                case 1:
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    if (s == "")
                                    {
                                        err.code = Error.ERROR_WRONG_DATA;
                                        err.text = "LOGIN IS ALREADY USED";
                                        submitButton.Status = StatusType.ACTIVE;
                                        chgButton.Status = StatusType.ACTIVE;
                                        loginInput.Status = StatusType.ACTIVE;
                                        passInput.Status = StatusType.ACTIVE;
                                    }
                                    else
                                    {
                                        Me.ID = uint.Parse(s);
                                        Me.Login = loginInput.Text();
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        session_key = Int64.Parse(s);
                                        s = "";
                                        p++;
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        Me.photo_id = uint.Parse(s);
                                        mode = 0;
                                        WND_WIDTH = 900;
                                        WND_HEIGHT = 600;
                                        wnd.Size = new SFML.System.Vector2u(WND_WIDTH, WND_HEIGHT);

                                    }
                                    break;
                            }
                        }
                        toDelete.Add(r);
                    }
                    foreach (Network.tRequest q in toDelete)
                    {
                        Resp.Remove(q);
                    }
                }
                wnd.Display();
            }
        }
        
        private static void Win_Resized(object sender, SizeEventArgs e)
        {
            if (e.Width < WND_WIDTH)
            {
                wnd.Size = new SFML.System.Vector2u(WND_WIDTH, e.Height);
                e.Width = WND_WIDTH;
            }
            if (e.Height < WND_HEIGHT)
            {
                wnd.Size = new SFML.System.Vector2u(e.Width, WND_HEIGHT);
                e.Height = WND_HEIGHT;
            }
            wnd.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private static void Win_KeyReleased(object sender, KeyEventArgs args)
        {
        }
        private static void Win_TextEntered(object sender, TextEventArgs args)
        {
            foreach (TextBox t in textBoxes)
            {
                t.Update(args);
            }
        }

        private static void Win_MouseMoved(object sender, MouseMoveEventArgs args)
        {
            foreach (Button b in buttons)
            {
                b.Update(args);
            }
        }

        private static void Win_MouseButtonReleased(object sender, MouseButtonEventArgs args)
        {
            err.Clear();
            foreach(TextBox t in textBoxes)
            {
                t.Update(args);
            }
            foreach(Button b in buttons)
            {
                b.Update(args);
            }
        }

        private static void Win_Closed(object sender, EventArgs e)
        {
            Closed = true;
            nw.Abort();
            Client.Disconnect();
            wnd.Close();
        }
    }
}
