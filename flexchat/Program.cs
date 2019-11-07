using System;
using System.Threading;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace flexchat
{
    class Program
    {
        public static uint WND_WIDTH = 720;
        public static uint WND_HEIGHT = 500;

        public static RenderWindow wnd = new RenderWindow(new VideoMode(WND_WIDTH, WND_HEIGHT), null);

        public static List<TextBox> textBoxes = new List<TextBox>();
        public static List<Button> buttons = new List<Button>();

        public static Int64 session_key = 0;
        public static Users Me = new Users(0);

        public Int64 SessionKey
        {
            set { session_key = value; }
        }

        public static Error err;

        private static Network Client;

        private static Thread nw;

        public static List<Network.tRequest> Resp;
        public static List<Conversations> convs;
        public static List<Users> users;
        public static List<uint> friends;

        private static uint mode;

        private static Panel panel;

        static void Main()
        {
            Resp = new List<Network.tRequest>();
            convs = new List<Conversations>();
            users = new List<Users>();
            friends = new List<uint>();

            wnd.SetVerticalSyncEnabled(true);

            wnd.Closed += Win_Closed;
            wnd.Resized += Win_Resized;
            wnd.KeyReleased += Win_KeyReleased;
            wnd.TextEntered += Win_TextEntered;
            wnd.MouseButtonReleased += Win_MouseButtonReleased;
            wnd.MouseMoved += Win_MouseMoved;

            Content.Load();

            panel = new Panel(Content.panel, 19, Content.exitButton, Content.exitButtonSelected, 34);
            panel.exitButton.Function += Win_Closed;

            Panel.MenuElement menu;
            menu.Function = null;
            menu.selected = false;
            menu.sizeX = 49;
            menu.texture = Content.settings;
            menu.textureSelected = Content.settingssel;
            menu.submenu = null;
            Panel.menu.Add(menu);

            err = new Error(20, Color.Red);
            err.Clear();

            Client = new Network("192.168.1.10", 8081);
            nw = new Thread(Client.WaitForResponse);
            nw.Start();

            TextBox loginInput = new TextBox(441, 66, 40, Content.LoginTextbox, Content.LoginTextbox, 65, new Color(80, 72, 153, 255));
            loginInput.textLengthBound = 18;
            loginInput.SpacebarAllowed = false;
            loginInput.symbolsAllowed = false;
            loginInput.defaultString = "login";
            textBoxes.Add(loginInput);

            TextBox passInput = new TextBox(441, 66, 40, Content.PassTextbox, Content.PassTextbox, 65, new Color(80, 72, 153, 255));
            passInput.textLengthBound = 18;
            passInput.SpacebarAllowed = false;
            passInput.symbolsAllowed = false;
            passInput.defaultString = "password";
            passInput.sub = "*";
            textBoxes.Add(passInput);

            TextBox typeMsg = new TextBox(441, 66, 16, Content.LoginTextbox, Content.LoginTextbox, 20, new Color(80, 72, 153, 255));
            typeMsg.symbolsAllowed = true;

            Button submitButton = new Button("", 207, 50, StatusType.ACTIVE);
            submitButton.SetTextColor(Color.White, new Color(54, 38, 84, 255), Color.White);
            submitButton.LoadTextures(Content.submitbutton[0,0], Content.submitbutton[0,1], Content.submitbutton[0,0]);
            submitButton.textSize = 30;
            buttons.Add(submitButton);

            Button chgButton = new Button("", 60, 21, StatusType.ACTIVE);
            chgButton.textSize = 16;
            chgButton.LoadTextures(Content.chg[0], Content.chg[0], Content.chg[0]);
            buttons.Add(chgButton);

            Button chatsButton = new Button("", 112, 30, StatusType.ACTIVE);
            Button friendsButton = new Button("", 113, 30, StatusType.ACTIVE);

            RectangleShape Background = new RectangleShape(new SFML.System.Vector2f(wnd.Size.X, wnd.Size.Y));
            Background.Texture = Content.Background;
            Background.Position = new SFML.System.Vector2f(0, 0);

            mode = 0;

            Me.pos_x = 0;
            Me.pos_y = 0;

            while (wnd.IsOpen)
            {
                wnd.DispatchEvents();

                Background.Size = new SFML.System.Vector2f(wnd.Size.X, wnd.Size.Y);
                wnd.Draw(Background);

                if (true)
                {
                    err.posX = (wnd.Size.X / 2) - (loginInput.sizeX / 2) + 5;
                    err.posY = (wnd.Size.Y / 2) - loginInput.sizeY - 60 - err.textSize;

                    loginInput.posX = (wnd.Size.X / 2) - (loginInput.sizeX / 2);
                    loginInput.posY = (wnd.Size.Y / 2) - loginInput.sizeY - 50;
                    loginInput.Draw();

                    passInput.posX = (wnd.Size.X / 2) - (passInput.sizeX / 2);
                    passInput.posY = (wnd.Size.Y / 2) - 30;
                    passInput.Draw();

                    submitButton.posX = (wnd.Size.X / 2) - (submitButton.sizeX / 2);
                    submitButton.posY = (wnd.Size.Y / 2) + 100;
                    submitButton.Draw();

                    chgButton.posX = (wnd.Size.X / 2) + (passInput.sizeX / 2) - (chgButton.sizeX) - 32;
                    chgButton.posY = (wnd.Size.Y / 2) + passInput.sizeY / 2 + 7;
                    chgButton.Draw();

                    if (submitButton.Clicked())
                    {
                        uint r_id = Me.Authorize(Client, mode, loginInput.typed, passInput.typed);
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
                        chgButton.LoadTextures(Content.chg[mode], Content.chg[mode], Content.chg[mode]);
                        submitButton.LoadTextures(Content.submitbutton[mode,0], Content.submitbutton[mode,1], Content.submitbutton[mode,0]);
                    }
                }
                else
                {
                    // IF AUTHENTHICATED
                    RectangleShape bg = new RectangleShape(new SFML.System.Vector2f(WND_WIDTH / 4, wnd.Size.Y - WND_HEIGHT / 8));
                    bg.Position = new SFML.System.Vector2f(0, WND_HEIGHT / 8);
                    bg.FillColor = Color.Red;
                    wnd.Draw(bg);
                    uint posx = 0;
                    uint posy = WND_HEIGHT / 8 + chatsButton.sizeY;
                    if (mode == 0)
                    {
                        foreach (Conversations c in convs)
                        {
                            c.pos_x = posx;
                            c.pos_y = posy;
                            posy += c.photo_size + 10;
                            c.Draw();
                        }
                    }
                    else
                    {
                        foreach (Users u in users)
                        {
                            u.pos_x = posx;
                            u.pos_y = posy;
                            posy += u.photo_size + 10;
                            u.Draw();
                        }
                    }
                    bg.Size = new SFML.System.Vector2f(WND_WIDTH / 4, WND_HEIGHT / 8);
                    bg.Position = new SFML.System.Vector2f(0, 0);
                    bg.FillColor = Color.Red;
                    wnd.Draw(bg);
                    if (chatsButton.Clicked())
                    {
                        mode = 0;
                        chatsButton.Status = StatusType.BLOCKED;
                        friendsButton.Status = StatusType.ACTIVE;
                    }
                    if (friendsButton.Clicked())
                    {
                        mode = 1;
                        friendsButton.Status = StatusType.BLOCKED;
                        chatsButton.Status = StatusType.ACTIVE;
                    }
                    chatsButton.posX = 0;
                    chatsButton.posY = WND_HEIGHT / 8;
                    chatsButton.Draw();
                    friendsButton.posX = chatsButton.sizeX;
                    friendsButton.posY = WND_HEIGHT / 8;
                    friendsButton.Draw();
                    Me.Draw();

                    if (mode == 0)
                    {
                        foreach (Conversations c in convs)
                        {
                            if (c.status == StatusType.BLOCKED)
                            {
                                c.DrawMessages();

                                typeMsg.posX = WND_WIDTH / 4 + 10;
                                typeMsg.sizeX = wnd.Size.X - WND_WIDTH / 4 - 20;
                                typeMsg.posY = wnd.Size.Y - typeMsg.sizeY - 10;
                                typeMsg.Draw();

                                break;
                            }
                        }
                    }
                    else
                    {

                    }
                }

                err.Draw();

                panel.Draw();

                if (Resp.Count > 0)
                {
                    List<Network.tRequest> toDelete = new List<Network.tRequest>();
                    int loopBound = Resp.Count;
                    for (int it = 0; it < loopBound; it++)
                    {
                        uint rMode = Resp[it].mode;
                        string respond = Resp[it].respond;
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
                                case 1:
                                    textBoxes.Add(typeMsg);
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    if (s == "")
                                    {
                                        if (Resp[it].mode == 1)
                                        {
                                            err.code = Error.ERROR_WRONG_DATA;
                                            err.text = "LOGIN IS ALREADY USED";
                                        }
                                        else
                                        {
                                            err.code = Error.ERROR_WRONG_DATA;
                                            err.text = "WRONG LOGIN OR PASSWORD";
                                        }
                                        submitButton.Status = StatusType.ACTIVE;
                                        chgButton.Status = StatusType.ACTIVE;
                                        loginInput.Status = StatusType.ACTIVE;
                                        passInput.Status = StatusType.ACTIVE;
                                    }
                                    else
                                    {
                                        Me.ID = uint.Parse(s);
                                        Me.Login = loginInput.typed;
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
                                        p++;
                                        Me.photo_id = uint.Parse(s);
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        p++;
                                        int len = int.Parse(s);
                                        for (int i = 0; i < len; i++)
                                        {
                                            s = "";
                                            while (respond[p] != 0)
                                            {
                                                s += respond[p];
                                                p++;
                                            }
                                            p++;
                                            Conversations t = new Conversations(int.Parse(s));
                                            convs.Add(t);
                                            t.RequestData(Client);
                                        }
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        len = int.Parse(s);
                                        p++;
                                        for (int i = 0; i < len; i++)
                                        {
                                            s = "";
                                            while (respond[p] != 0)
                                            {
                                                s += respond[p];
                                                p++;
                                            }
                                            p++;
                                            Users u = new Users(uint.Parse(s));
                                            users.Add(u);
                                            friends.Add(uint.Parse(s));
                                            u.RequestData(Client);
                                        }
                                        mode = 0;
                                        WND_WIDTH = 900;
                                        WND_HEIGHT = 600;
                                        wnd.Size = new SFML.System.Vector2u(WND_WIDTH, WND_HEIGHT);
                                    }
                                    break;
                                case 2:
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    uint id = uint.Parse(s);
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    string login = s;
                                    p++;
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    uint photo_id = uint.Parse(s);
                                    for (int u = 0; u < users.Count; u++)
                                    {
                                        if (users[u].ID == id)
                                        {
                                            users[u].Login = login;
                                            users[u].photoID = photo_id;
                                            break;
                                        }
                                    }
                                    break;
                                case 3:
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    uint conv_id = uint.Parse(s);
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    int cr_id = int.Parse(s);
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    int ph_id = int.Parse(s);
                                    p++;
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    for (int c = 0; c < convs.Count; c++)
                                    {
                                        if (convs[c].id == conv_id)
                                        {
                                            convs[c].creator_id = cr_id;
                                            convs[c].photo_id = ph_id;
                                            convs[c].title = s;
                                            convs[c].AskForMessages(Client);
                                            break;
                                        }
                                    }
                                    break;
                                case 4:
                                    while (p < respond.Length)
                                    {
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        int m_id = int.Parse(s);
                                        s = "";
                                        p++;
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        int sender_id = int.Parse(s);
                                        s = "";
                                        p++;
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        int convID = int.Parse(s);
                                        s = "";
                                        p++;
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        p++;
                                        string msgtext = s;
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        p++;
                                        DateTime msgSent = DateTime.ParseExact(s, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                        foreach (Conversations c in convs)
                                        {
                                            if (c.id == convID)
                                            {
                                                c.AddMessage(m_id, sender_id, convID, msgtext, msgSent);
                                                break;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        toDelete.Add(Resp[it]);
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
            panel.Update(args);
            foreach (Button b in buttons)
            {
                b.Update(args);
            }
            foreach (Conversations c in convs)
            {
                c.Update(args);
            }
            foreach(Users u in users)
            {
                u.Update(args);
            }
        }

        private static void Win_MouseButtonReleased(object sender, MouseButtonEventArgs args)
        {
            panel.Update(args);
            err.Clear();
            foreach(TextBox t in textBoxes)
            {
                t.Update(args);
            }
            foreach(Button b in buttons)
            {
                b.Update(args);
            }
            if (args.X <= WND_WIDTH / 4)
            {
                if (mode == 0)
                {
                    foreach (Conversations c in convs)
                    {
                        c.Update(args);
                    }
                }
                else
                {
                    foreach(Users u in users)
                    {
                        u.Update(args);
                    }
                }
            }
        }

        private static void Win_Closed(object sender, EventArgs e)
        {
            nw.Abort();
            Client.Disconnect();
            wnd.Close();
            Environment.Exit(0);
        }
    }
}
