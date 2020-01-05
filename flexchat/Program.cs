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
        public static uint CHATS_WIDTH = 260;
        public static int SEARCH_HEIGHT = 30;

        public static RenderWindow wnd = new RenderWindow(new VideoMode(WND_WIDTH, WND_HEIGHT), "FLEXCHAT");

        public static List<TextBox> textBoxes = new List<TextBox>();
        public static List<Button> buttons = new List<Button>();

        public static Int64 session_key = 0;
        public static Users Me = new Users(0);

        public static bool UpdAllowed = false;

        public static int UserSelected = -1;
        public static int ConvSelected = -1;

        public Int64 SessionKey
        {
            set { session_key = value; }
        }

        public static Error err;

        public static Network Client;

        public static Thread nw;

        private static DateTime UpdateTime;
        public static DateTime LastMessageTime;

        public static List<Network.tRequest> Resp;
        public static List<Conversations> convs;
        public static List<Users> users;
        public static List<int> friends;
        public static List<FriendRequest> frreqs;
        public static bool SmthSelected = false;

        public static Button frreq = new Button("", 384, 60, StatusType.BLOCKED);
        public static Button remfr = new Button("", 384, 60, StatusType.BLOCKED);
        public static Button accreq = new Button("", 384, 60, StatusType.BLOCKED);
        public static Button cancelreq = new Button("", 384, 60, StatusType.BLOCKED);
        public static Button createconv = new Button("", 384, 60, StatusType.BLOCKED);
        public static Button chgphoto = new Button("", 60, 60, StatusType.BLOCKED);
        public static Button chgmode;

        public static Button convmenu = new Button("", 75, 75, StatusType.BLOCKED);
        public static Button chgtitle = new Button("", 250, 50, StatusType.BLOCKED);

        private static SortedSet<int> searchResults;
        private static int nResults = 0;
        private static bool search = false;

        public static bool VoiceRecording = false;
        public static bool VoiceRecordingAvailable = false;
        public static bool StopRecording = false;
        public static bool SendingVoice = false;

        public static uint mode;

        public static int Scroll = 0;

        static void Main()
        {

            VoiceRecordingAvailable = true;

            Audio recorder = new Audio("temp.wav");

            LastMessageTime = DateTime.ParseExact("2000-01-01 01:01:01", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            Resp = new List<Network.tRequest>();
            convs = new List<Conversations>();
            users = new List<Users>();
            friends = new List<int>();
            frreqs = new List<FriendRequest>();
            searchResults = new SortedSet<int>();

            buttons.Add(frreq);
            buttons.Add(remfr);
            buttons.Add(accreq);
            buttons.Add(cancelreq);
            buttons.Add(createconv);
            buttons.Add(chgphoto);

            buttons.Add(convmenu);
            buttons.Add(chgtitle);

            wnd.SetVerticalSyncEnabled(true);

            wnd.Closed += Win_Closed;
            wnd.Resized += Win_Resized;
            wnd.KeyReleased += Win_KeyReleased;
            wnd.TextEntered += Win_TextEntered;
            wnd.MouseButtonReleased += Win_MouseButtonReleased;
            wnd.MouseMoved += Win_MouseMoved;
            wnd.MouseWheelScrolled += Win_MouseScrolled;

            Content.Load();

            err = new Error(20, Color.Red);
            err.Clear();

            Client = new Network("localhost", 8081); 
            nw = new Thread(Client.WaitForResponse);
            nw.Start();

            TextBox loginInput = new TextBox(441, 66, 40, Content.LoginTextbox, Content.LoginTextbox, 65, Content.color1);
            loginInput.textLengthBound = 18;
            loginInput.SpacebarAllowed = false;
            loginInput.symbolsAllowed = false;
            loginInput.defaultString = "login";
            textBoxes.Add(loginInput);

            Button mic = new Button("", 80, 80, StatusType.BLOCKED);
            buttons.Add(mic);
            mic.LoadTextures(Content.mic, Content.micS, Content.mic);
            mic.posX = CHATS_WIDTH + 10;
            Button stop = new Button("", 80, 80, StatusType.BLOCKED);
            buttons.Add(stop);
            stop.LoadTextures(Content.stop, Content.stopS, Content.stop);
            stop.posX = CHATS_WIDTH + 10;

            TextBox msgTextBox = new TextBox(90, 80, 46, Content.MessageTextbox, Content.MessageTextbox, 15, Content.color1);
            msgTextBox.workMode = 1;

            TextBox passInput = new TextBox(441, 66, 40, Content.PassTextbox, Content.PassTextbox, 65, Content.color1);
            passInput.textLengthBound = 18;
            passInput.SpacebarAllowed = false;
            passInput.symbolsAllowed = false;
            passInput.defaultString = "password";
            passInput.sub = "*";
            textBoxes.Add(passInput);

            TextBox SearchBox = new TextBox(CHATS_WIDTH, Convert.ToUInt32(SEARCH_HEIGHT), 22, Content.searchbox, Content.searchbox, 28, Content.color1);
            SearchBox.textLengthBound = 18;
            SearchBox.SpacebarAllowed = false;
            SearchBox.symbolsAllowed = false;
            SearchBox.defaultString = "";

            TextBox typeMsg = new TextBox(441, 66, 17, Content.LoginTextbox, Content.LoginTextbox, 20, new Color(80, 72, 153, 255));

            Button submitButton = new Button("", 207, 50, StatusType.ACTIVE);
            submitButton.SetTextColor(Color.White, new Color(54, 38, 84, 255), Color.White);
            submitButton.LoadTextures(Content.submitbutton[0,0], Content.submitbutton[0,1], Content.submitbutton[0,0]);
            submitButton.textSize = 30;
            buttons.Add(submitButton);

            frreq.posX = CHATS_WIDTH + 70;
            frreq.posY = 300;
            frreq.LoadTextures(Content.frreq, Content.frreqS, Content.frreq);

            remfr.posX = CHATS_WIDTH + 70;
            remfr.posY = 300;
            remfr.LoadTextures(Content.remfr, Content.remfrS, Content.remfr);

            cancelreq.posX = CHATS_WIDTH + 70;
            cancelreq.posY = 300;
            cancelreq.LoadTextures(Content.cancelreq, Content.cancelreqS, Content.cancelreq);

            chgphoto.posX = CHATS_WIDTH + 295;
            chgphoto.posY = 210;
            chgphoto.LoadTextures(Content.chgphoto, Content.chgphotoS, Content.chgphoto);

            createconv.posX = CHATS_WIDTH + 70;
            createconv.posY = 400;
            createconv.LoadTextures(Content.createconv, Content.createconvS, Content.createconv);

            accreq.posX = CHATS_WIDTH + 70;
            accreq.posY = 300;
            accreq.LoadTextures(Content.accreq, Content.accreqS, Content.accreq);

            convmenu.posY = 0;
            convmenu.LoadTextures(Content.convmenu, Content.convmenuS, Content.convmenuS);
            chgtitle.posY = 0;
            chgtitle.LoadTextures(Content.chgtitle, Content.chgtitleS, Content.chgtitleS);

            chgmode = new Button("", CHATS_WIDTH, 45, StatusType.ACTIVE);
            chgmode.LoadTextures(Content.chgmode[0], Content.chgmode[0], Content.chgmode[0]);

            Button SendButton = new Button("", 80, 80, StatusType.ACTIVE);
            SendButton.workMode = 1;

            Button chgButton = new Button("", 60, 21, StatusType.ACTIVE);
            chgButton.textSize = 16;
            chgButton.LoadTextures(Content.chg[0], Content.chg[0], Content.chg[0]);
            buttons.Add(chgButton);

            Button chatsButton = new Button("", 112, 30, StatusType.ACTIVE);
            Button friendsButton = new Button("", 113, 30, StatusType.ACTIVE);

            UpdateTime = DateTime.Now;

            mode = 0;

            Me.pos_x = 0;
            Me.pos_y = 0;

            while (wnd.IsOpen)
            {
                wnd.DispatchEvents();

                wnd.Clear(Content.color1);

                if (session_key == 0)
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
                        Scroll = 0;
                        mode = 1 - mode;
                        chgButton.LoadTextures(Content.chg[mode], Content.chg[mode], Content.chg[mode]);
                        submitButton.LoadTextures(Content.submitbutton[mode,0], Content.submitbutton[mode,1], Content.submitbutton[mode,0]);
                    }

                }
                else
                {
                    // IF AUTH
                    // Draw Background
                    RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(CHATS_WIDTH, wnd.Size.Y));
                    rect.Position = new SFML.System.Vector2f(0, 0);
                    rect.FillColor = Content.color2;
                    wnd.Draw(rect);
                    // Draw Chats

                    int scr = 0;
                    if (!search)
                    {
                        if (mode == 0)
                        {
                            foreach (Conversations c in convs)
                            {
                                int t = c.Draw(scr + Scroll + SEARCH_HEIGHT);
                                if (t == 0) break;
                                else scr += t;
                            }
                        }
                        else
                        {
                            foreach (FriendRequest f in frreqs)
                            {
                                if (!f.hidden)
                                {
                                    f.changeStatus(StatusType.ACTIVE);
                                    scr += f.Draw(scr + Scroll + SEARCH_HEIGHT);
                                    if (f.Deny.Clicked())
                                    {
                                        f.hidden = true;
                                        Client.SendData("1" + Convert.ToString((char)0) + Convert.ToString(f.id), 101);
                                    } else if (f.Accept.Clicked())
                                    {
                                        f.hidden = true;
                                        Client.SendData("0" + Convert.ToString((char)0) + Convert.ToString(f.id), 101);
                                    }
                                }
                            }
                            foreach (Users u in users)
                            {
                                if (!u.friend)
                                    continue;
                                if (u.status != StatusType.ACTIVE)
                                {
                                    RectangleShape popa = new RectangleShape(new SFML.System.Vector2f(CHATS_WIDTH, u.photo_size + 10));
                                    popa.Position = new SFML.System.Vector2f(0, scr + Scroll + SEARCH_HEIGHT);
                                    popa.FillColor = Content.color1;
                                    wnd.Draw(popa);
                                }
                                u.pos_x = 0;
                                u.pos_y = scr + Scroll + SEARCH_HEIGHT;
                                int t = u.Draw(scr + Scroll + SEARCH_HEIGHT);
                                if (t == 0) break;
                                else scr += t;
                            }
                        }
                    }
                    else
                    {
                        foreach (Users u in users)
                        {
                            if (!searchResults.Contains(u.ID))
                                continue;
                            if (u.status != StatusType.ACTIVE)
                            {
                                RectangleShape popa = new RectangleShape(new SFML.System.Vector2f(CHATS_WIDTH, u.photo_size + 10));
                                popa.Position = new SFML.System.Vector2f(0, scr + Scroll + SEARCH_HEIGHT);
                                popa.FillColor = Content.color1;
                                wnd.Draw(popa);
                            }
                            u.pos_x = 0;
                            u.pos_y = scr + Scroll + SEARCH_HEIGHT;
                            int t = u.Draw(scr + Scroll + SEARCH_HEIGHT);
                            if (t == 0) break;
                            else scr += t;
                        }
                    }


                    if (chgmode.Clicked())
                    {
                        mode = 1 - mode;
                        if (mode == 0)
                        {
                            foreach (FriendRequest f in frreqs)
                            {
                                f.changeStatus(StatusType.BLOCKED);
                            }
                        } 
                        chgmode.textures[0] = Content.chgmode[mode];
                        chgmode.textures[1] = Content.chgmode[mode];
                        chgmode.textures[2] = Content.chgmode[mode];
                    }
                    chgmode.posX = 0;
                    chgmode.posY = wnd.Size.Y - Me.photo_size - 10 - chgmode.sizeY;
                    chgmode.Draw();

                    if (StopRecording)
                    {
                        StopRecording = false;
                        if (VoiceRecording)
                        {
                            recorder.StopRecording();
                            VoiceRecording = false;
                            stop.Status = StatusType.BLOCKED;
                        }
                    }

                    if (ConvSelected >= 0)
                    {
                        msgTextBox.posX = CHATS_WIDTH + 90;
                        msgTextBox.posY = wnd.Size.Y - 81;
                        msgTextBox.sizeY = 81;
                        msgTextBox.sizeX = wnd.Size.X - CHATS_WIDTH - 10;
                        msgTextBox.Draw();

                        if (VoiceRecording)
                        {
                            if (stop.Status == StatusType.BLOCKED)
                                stop.Status = StatusType.ACTIVE;
                            stop.posY = wnd.Size.Y - 80;
                            stop.Draw();
                            if (stop.Clicked())
                            {
                                recorder.StopRecording();
                                Client.SendVoice();
                                VoiceRecording = false;
                                stop.Status = StatusType.BLOCKED;
                            }
                        }
                        else
                        {
                            if (mic.Status == StatusType.BLOCKED)
                                mic.Status = StatusType.ACTIVE;
                            mic.posY = wnd.Size.Y - 80;
                            mic.Draw();
                            if (mic.Clicked() && VoiceRecordingAvailable)
                            {
                                recorder.StartRecording();
                                VoiceRecording = true;
                                mic.Status = StatusType.BLOCKED;
                            }
                        }

                        RectangleShape liniya = new RectangleShape(new SFML.System.Vector2f(msgTextBox.sizeX, 1));
                        liniya.Position = new SFML.System.Vector2f(msgTextBox.posX, msgTextBox.posY);
                        liniya.FillColor = Content.color1;
                        wnd.Draw(liniya);

                        SendButton.posX = wnd.Size.X - 80;
                        SendButton.posY = wnd.Size.Y - 80;
                        SendButton.Draw();
                    }

                    // Draw User(s)

                    RectangleShape mebg = new RectangleShape(new SFML.System.Vector2f(CHATS_WIDTH, Me.photo_size + 10));
                    mebg.Position = new SFML.System.Vector2f(0, wnd.Size.Y - Me.photo_size - 10);
                    if (Me.status == StatusType.SELECTED || Me.status == StatusType.BLOCKED)
                        mebg.FillColor = Content.color1;
                    else mebg.FillColor = Content.color2;
                    wnd.Draw(mebg);
                    Me.Draw(Convert.ToInt32(wnd.Size.Y - Me.photo_size - 10));

                    SearchBox.Draw();
                    RectangleShape poloska = new RectangleShape(new SFML.System.Vector2f(236, 2));
                    poloska.FillColor = Content.color1;
                    poloska.Position = new SFML.System.Vector2f(12, SearchBox.sizeY);
                    wnd.Draw(poloska);

                    //UpdateData

                    if (SearchBox.Changed())
                    {
                        if (SearchBox.typed == "")
                        {
                            search = false;
                            searchResults.Clear();
                        }
                        else
                        {
                            search = true;
                            Scroll = 0;
                            Client.SendData(SearchBox.typed + Convert.ToString((char)(0)) + Convert.ToString(0), 200);
                        }
                    }

                    if (SendButton.Clicked())
                    {
                        foreach (Conversations c in convs)
                        {
                            if (c.status == StatusType.BLOCKED)
                            {
                                Client.SendMessage(c, msgTextBox.typed, "0" + Convert.ToString((char)(0)));
                                break;
                            }
                        }
                        msgTextBox.typed = "";
                    }
                    UpdateData(false);
                }

                err.Draw();

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
                                        Me.ID = int.Parse(s);
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
                                        Me.photo_id = int.Parse(s);
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
                                            Users u = new Users(int.Parse(s));
                                            users.Add(u);
                                            u.friend = true;
                                            friends.Add(int.Parse(s));
                                            u.RequestData(Client);
                                        }
                                        mode = 0;
                                        WND_WIDTH = 1200;
                                        WND_HEIGHT = 700;
                                        SendButton.LoadTextures(Content.SendButton, Content.SendButton_Clicked, Content.SendButton);
                                        buttons.Add(SendButton);
                                        msgTextBox.textLengthBound = 512;
                                        msgTextBox.SpacebarAllowed = true;
                                        msgTextBox.symbolsAllowed = true;
                                        msgTextBox.defaultString = "Type your message";
                                        msgTextBox.evAllowed = true;
                                        msgTextBox.workMode = 1;
                                        textBoxes.Add(msgTextBox);
                                        buttons.Add(chgmode);
                                        textBoxes.Add(SearchBox);
                                        mode = 0;
                                        if (wnd.Size.X < WND_WIDTH)
                                            wnd.Size = new SFML.System.Vector2u(WND_WIDTH, wnd.Size.Y);
                                        if (wnd.Size.Y < WND_HEIGHT)
                                            wnd.Size = new SFML.System.Vector2u(wnd.Size.X, WND_HEIGHT);
                                        Client.SendData("", 102);
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
                                    int photo_id = int.Parse(s);
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
                                    p++;
                                    string ctitle = s;
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    int second_person = int.Parse(s);
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    DateTime lread = DateTime.ParseExact(s, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                    
                                    for (int c = 0; c < convs.Count; c++)
                                    {
                                        if (convs[c].id == conv_id)
                                        {
                                            convs[c].creator_id = cr_id;
                                            convs[c].photo_id = ph_id;
                                            convs[c].title = ctitle;
                                            convs[c].last_read = lread;
                                            convs[c].AskForMessages(Client);
                                            convs[c].loaded = true;
                                            convs[c].second_person = second_person;
                                            break;
                                        }
                                    }
                                    break;
                                case 4:
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    int nm = int.Parse(s);
                                    for (int ct = 0; ct < nm; ct++)
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
                                        int last = convs.Count;
                                        int curc = 0;
                                        string att = "";
                                        while (respond[p] != 0)
                                        {
                                            att += respond[p];
                                            p++;
                                        }
                                        p++;
                                        int natts = int.Parse(att);
                                        att += respond[p - 1];
                                        for (int i = 0; i < natts; i++)
                                        {
                                            while (respond[p] != 0)
                                            {
                                                att += respond[p];
                                                p++;
                                            }
                                            att += respond[p];
                                            p++;
                                        }
                                        foreach (Conversations c in convs)
                                        {
                                            curc++;
                                            if (c.id == convID)
                                            {
                                                c.AddMessage(m_id, sender_id, convID, msgtext, msgSent, att);
                                                if (curc == last)
                                                    UpdAllowed = true;
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                case 6:
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    int nupd = int.Parse(s);
                                    for (int i = 0; i < nupd; i++)
                                    {
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        p++;
                                        ParseUpdate(s);
                                    }
                                    break;
                                case 7:
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    int msgid = int.Parse(s);
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    int convidmsg = int.Parse(s);
                                    foreach (Conversations c in convs)
                                    {
                                        if (c.id == convidmsg)
                                        {
                                            for (int i = 0; i < c.messages.Count; i++)
                                            {
                                                if (c.messages[i].id == -Resp[it].id)
                                                {
                                                    c.messages[i].id = msgid;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 8:
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    int newmsgid = int.Parse(s);
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    int newmsgsenderid = int.Parse(s);
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    int newmsgconvid = int.Parse(s);
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    string newmsgtxt = s;
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    DateTime newmsgsent = DateTime.ParseExact(s, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                    string att2 = "";
                                    while (respond[p] != 0)
                                    {
                                        att2 += respond[p];
                                        p++;
                                    }
                                    p++;
                                    int natts2 = int.Parse(att2);
                                    att2 += respond[p - 1];
                                    for (int i = 0; i < natts2; i++)
                                    {
                                        while (respond[p] != 0)
                                        {
                                            att2 += respond[p];
                                            p++;
                                        }
                                        att2 += respond[p];
                                        p++;
                                    }
                                    foreach (Conversations c in convs)
                                    {
                                        if (c.id == newmsgconvid)
                                        {
                                            c.AddMessage(newmsgid, newmsgsenderid, newmsgconvid, newmsgtxt, newmsgsent, att2);
                                            break;
                                        }
                                    }
                                    break;
                                case 100:
                                    s = "";
                                    while (respond[p] != 0)
                                    {
                                        s += respond[p];
                                        p++;
                                    }
                                    p++;
                                    if (s == "0")
                                    {
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        p++;
                                        int toid = int.Parse(s);
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        p++;

                                        foreach (Users u in users)
                                        {
                                            if (u.ID == toid)
                                            {
                                                u.reqto = true;
                                                u.reqtoid = int.Parse(s);
                                            }
                                        }
                                    }
                                    else if (s == "1")
                                    {
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        p++;

                                        int delreqid = int.Parse(s);

                                        foreach (Users u in users)
                                        {
                                            if (u.ID == delreqid)
                                            {
                                                u.reqto = false;
                                                u.reqtoid = -1;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        p++;
                                        int recpid = int.Parse(s);
                                        s = "";
                                        while (respond[p] != 0)
                                        {
                                            s += respond[p];
                                            p++;
                                        }
                                        p++;
                                        if (s == "0")
                                        {
                                            foreach (Users u in users)
                                            {
                                                if (u.ID == recpid)
                                                {
                                                    u.reqto = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            s = "";
                                            while (respond[p] != 0)
                                            {
                                                s += respond[p];
                                                p++;
                                            }
                                            p++;
                                            int reqid = int.Parse(s);
                                            foreach (Users u in users)
                                            {
                                                if (u.ID == recpid)
                                                {
                                                    u.reqto = true;
                                                    u.reqtoid = reqid;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 101:
                                    s = "";
                                    while (respond[p] != 0)
                                        s += respond[p++];
                                    p++;
                                    if (s == "0")
                                    {
                                        s = "";
                                        while (respond[p] != 0)
                                            s += respond[p++];
                                        p++;
                                        int senderID = int.Parse(s);
                                        for (int i = 0; i < frreqs.Count; i++)
                                        {
                                            if (frreqs[i].from == senderID)
                                            {
                                                frreqs.Remove(frreqs[i]);
                                                break;
                                            }
                                        }
                                        foreach (Users u in users)
                                        {
                                            if (u.ID == senderID)
                                            {
                                                u.reqfrom = false;
                                                u.reqfromhid = false;
                                                u.reqfromid = -1;
                                                u.friend = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        s = "";
                                        while (respond[p] != 0)
                                            s += respond[p++];
                                        p++;
                                        int chgreqid = int.Parse(s);
                                        for (int i = 0; i < frreqs.Count; i++)
                                        {
                                            if (frreqs[i].id == chgreqid)
                                            {
                                                FriendRequest f = frreqs[i];
                                                f.hidden = true;
                                                frreqs.Remove(frreqs[i]);
                                                frreqs.Add(f);
                                                break;
                                            }
                                        }
                                        foreach (Users u in users)
                                        {
                                            if (u.reqfromid == chgreqid)
                                            {
                                                u.reqfromhid = true;
                                            }
                                        }
                                    }

                                    break;
                                case 102:
                                    frreqs.Clear();
                                    s = "";
                                    while (respond[p] != 0)
                                        s += respond[p++];
                                    p++;
                                    int nreqs = int.Parse(s);
                                    for (int i = 0; i < nreqs; i++)
                                    {
                                        FriendRequest newreq = new FriendRequest();
                                        s = "";
                                        while (respond[p] != 0)
                                            s += respond[p++];
                                        p++;
                                        newreq.id = int.Parse(s);

                                        s = "";
                                        while (respond[p] != 0)
                                            s += respond[p++];
                                        p++;
                                        newreq.from = int.Parse(s);

                                        s = "";
                                        while (respond[p] != 0)
                                            s += respond[p++];
                                        p++;
                                        newreq.hidden = false;
                                        if (s == "t") newreq.hidden = true;
                                        frreqs.Add(newreq);
                                        frreqs[frreqs.Count - 1].Make();
                                    }
                                    break;
                                case 103:
                                    s = "";
                                    while (respond[p] != 0)
                                        s += respond[p++];
                                    int frdelid = int.Parse(s);
                                    for (int i = 0; i < users.Count; i++)
                                    {
                                        if (users[i].ID == frdelid)
                                        {
                                            users[i].friend = false;
                                        }
                                    }
                                    break;
                                case 200:
                                    s = "";
                                    while (respond[p] != 0)
                                        s += respond[p++];
                                    p++;
                                    searchResults.Clear();
                                    nResults = int.Parse(s);
                                    for (int i = 0; i < nResults; i++)
                                    {
                                        s = "";
                                        while (respond[p] != 0)
                                            s += respond[p++];
                                        p++;
                                        int res_id = int.Parse(s);
                                        searchResults.Add(res_id);
                                        bool f = false;
                                        for (int j = 0; j < users.Count; j++)
                                        {
                                            if (users[j].ID == res_id)
                                            {
                                                f = true;
                                                break;
                                            }
                                        }
                                        if (!f)
                                        {
                                            Users jopa = new Users(res_id);
                                            jopa.RequestData(Client);
                                            users.Add(jopa);
                                        }
                                    }
                                    break;
                                case 300:
                                    s = "";
                                    while (respond[p] != 0)
                                        s += respond[p++];
                                    p++;
                                    int newconvid = int.Parse(s);
                                    foreach (Conversations c in convs)
                                    {
                                        c.status = StatusType.ACTIVE;
                                    }
                                    Conversations newc = new Conversations(newconvid);
                                    convs.Add(newc);
                                    newc.RequestData(Client);
                                    newc.status = StatusType.ACTIVE;
                                    ConvSelected = newc.id;
                                    break;
                                case 999:
                                    string filename = "";
                                    while (respond[p] != 0)
                                        filename += respond[p++];
                                    p++;
                                    string _fileid = "";
                                    while (respond[p] != 0)
                                        _fileid += respond[p++];
                                    int fileid = int.Parse(_fileid);
                                    if (filename == "tosend.wav")
                                    {
                                        string text = msgTextBox.typed;
                                        if (text == "")
                                            text = " ";
                                        foreach (Conversations c in convs)
                                        {
                                            if (c.status == StatusType.BLOCKED)
                                            {
                                                Client.SendMessage(c, text, "1" + Convert.ToString((char)(0)) + _fileid);
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
        
        public static void UpdateData(bool urgent)
        {
            if (!urgent && !UpdAllowed)
                return;
            const double TimePassedInSeconds = 1.75;
            DateTime T = DateTime.Now;
            DateTime U = UpdateTime;
            U = U.AddSeconds(TimePassedInSeconds);
            if (urgent || T >= U)
            {
                UpdateTime = DateTime.Now;
                Client.SendData("", 6);
                UpdateTime = DateTime.Now;
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
            foreach (FriendRequest f in frreqs)
            {
                f.Accept.Update(args);
                f.Deny.Update(args);
            }
            if (search)
            {
                foreach (Users u in users)
                {
                    if (searchResults.Contains(u.ID))
                    {
                        u.Update(args);
                    }
                }
            }
            else
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
                    foreach (Users u in users)
                    {
                        u.Update(args);
                    }
                }
            }
            Me.pos_y = Convert.ToInt32(wnd.Size.Y - Me.photo_size - 10);
            Me.pos_x = 0;
            Me.Update(args);
        }

        private static void Win_MouseButtonReleased(object sender, MouseButtonEventArgs args)
        {
            SmthSelected = false;
            err.Clear();
            foreach(TextBox t in textBoxes)
            {
                if ((t.workMode == 1 && ConvSelected == -1) || (t.workMode == 2 && UserSelected == -1))
                    continue;
                t.Update(args);
            }
            foreach (FriendRequest f in frreqs)
            {
                f.Accept.Update(args);
                f.Deny.Update(args);
            }
            foreach (Button b in buttons)
            {
                if ((b.workMode == 1 && ConvSelected == -1) || (b.workMode == 2 && UserSelected == -1))
                    continue;
                b.Update(args);
            }
            if (args.X <= CHATS_WIDTH && args.Y > SEARCH_HEIGHT)
            {
                frreq.Status = StatusType.BLOCKED;
                remfr.Status = StatusType.BLOCKED;

                Me.Update(args);
                if (search)
                {
                    foreach (Users u in users)
                    {
                        if (searchResults.Contains(u.ID))
                            u.Update(args);
                    }
                }
                else
                {
                    if (mode == 0)
                    {
                        chgphoto.Status = StatusType.BLOCKED;
                        frreq.Status = StatusType.BLOCKED;
                        remfr.Status = StatusType.BLOCKED;
                        cancelreq.Status = StatusType.BLOCKED;
                        accreq.Status = StatusType.BLOCKED;
                        createconv.Status = StatusType.BLOCKED;
                        foreach (Users u in users)
                        {
                            u.status = StatusType.ACTIVE;
                        }
                        foreach (Conversations c in convs)
                        {
                            c.Update(args);
                        }
                    }
                    else
                    {
                        foreach (Conversations c in convs)
                        {
                            c.status = StatusType.ACTIVE;
                        }
                        convmenu.Status = StatusType.BLOCKED;
                        chgtitle.Status = StatusType.BLOCKED;
                        foreach (Users u in users)
                        {
                            u.Update(args);
                        }
                    }
                }
            }
        }

        private static void ParseUpdate(string upd)
        {
            string s = "";
            int p = 0;
            while (upd[p] != ' ')
                s += upd[p++];
            p++;
            int updcode = int.Parse(s);
            s = "";
            switch (updcode)
            {
                case 1: // New message
                    while (upd[p] != ' ')
                        s += upd[p++];
                    int newmsgid = int.Parse(s);
                    Client.SendData(Convert.ToString(newmsgid), 8);
                    break;
                case 2: //Conversation created
                    while (upd[p] != ' ')
                        s += upd[p++];
                    int newconvid = int.Parse(s);
                    Conversations c = new Conversations(newconvid);
                    c.RequestData(Client);
                    convs.Add(c);
                    break;
                case 3: //Removed as a friend
                    while (upd[p] != ' ')
                        s += upd[p++];
                    int nolongerfriend = int.Parse(s);
                    for (int i = 0; i < friends.Count; i++)
                    {
                        if (friends[i] == nolongerfriend)
                        {
                            friends.Remove(friends[i]);
                            break;
                        }
                    }
                    foreach (Users u in users)
                    {
                        if (u.ID == nolongerfriend)
                        {
                            u.friend = false;
                            break;
                        }
                    }
                    break;
                case 4: // Accept the request
                    while (upd[p] != ' ')
                        s += upd[p++];
                    int newfriendid = int.Parse(s);
                    friends.Add(newfriendid);
                    foreach (Users u in users)
                    {
                        if (u.ID == newfriendid)
                        {
                            u.reqto = false;
                            u.reqtoid = -1;
                            u.friend = true;
                            break;
                        }
                    }
                    break;
                case 5: // New request
                    while (upd[p] != ' ')
                        s += upd[p++];
                    p++;
                    int nreqid = int.Parse(s);
                    s = "";
                    while (upd[p] != ' ')
                        s += upd[p++];
                    int from = int.Parse(s);
                    FriendRequest newreq = new FriendRequest();
                    newreq.id = nreqid;
                    newreq.from = from;
                    newreq.hidden = false;
                    newreq.Make();
                    frreqs.Add(newreq);
                    break;
                case 6: // Cancel request
                    while (upd[p] != ' ')
                        s += upd[p++];
                    int cancelreqid = int.Parse(s);
                    for (int i = 0; i < frreqs.Count; i++)
                    {
                        if (frreqs[i].id == cancelreqid)
                        {
                            frreqs.Remove(frreqs[i]);
                            break;
                        }
                    }
                    break;
            }
        }

        private static void Win_Closed(object sender, EventArgs e)
        {
            nw.Abort();
            Client.Disconnect();
            Content.DeleteCache();
            wnd.Close();
            Environment.Exit(0);
        }

        private static void Win_MouseScrolled (object sender, MouseWheelScrollEventArgs e)
        {
            if (e.X <= CHATS_WIDTH)
            {
                if (mode == 0)
                {
                    if (e.Delta > 0)
                    {
                        Scroll += 30;
                        if (Scroll > 0)
                            Scroll = 0;
                    }
                    else
                    {
                        if (convs.Count >= 7)
                        {
                            int sum = SEARCH_HEIGHT + convs.Count * 80;
                            Scroll -= 30;
                            if (sum + Scroll < wnd.Size.Y - 125)
                            {
                                Scroll = Convert.ToInt32(wnd.Size.Y - 125 - sum);
                            }
                        }
                    }
                }
                else
                {
                    if (e.Delta < 0)
                    {
                        if (users.Count >= 7)
                        {
                            int sum = SEARCH_HEIGHT + users.Count * 80;
                            Scroll += 30;
                            if (sum - Scroll < wnd.Size.Y - 125)
                            {
                                Scroll = Convert.ToInt32(sum - wnd.Size.Y + 125);
                            }
                        }
                    }
                    else
                    {
                        Scroll -= 30;
                        if (Scroll < 0)
                            Scroll = 0;
                    }
                }
            }
            else
            {
                if (mode == 0)
                {
                    foreach (Conversations c in convs)
                    {
                        c.Update(e);
                    }
                }
            }
        }
    }
}
