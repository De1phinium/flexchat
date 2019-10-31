using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace flexchat
{
    class Program
    {
        static uint WND_WIDTH = 700;
        static uint WND_HEIGHT = 500;

        public static RenderWindow wnd = new RenderWindow(new VideoMode(WND_WIDTH, WND_HEIGHT), "FLEXCHAT");

        public static List<TextBox> textBoxes = new List<TextBox>();
        public static List<Button> buttons = new List<Button>();

        private static uint session_key = 0;
        public static Users Me = new Users();

        public uint SessionKey
        {
            set { session_key = value; }
        }

        private static Network Client;

        static void Main()
        {

            wnd.SetVerticalSyncEnabled(true);

            wnd.Closed += Win_Closed;
            wnd.Resized += Win_Resized;
            wnd.KeyReleased += Win_KeyReleased;
            wnd.TextEntered += Win_TextEntered;
            wnd.MouseButtonReleased += Win_MouseButtonReleased;
            wnd.MouseMoved += Win_MouseMoved;

            Content.Load();

            Client = new Network("192.168.1.10", 8081);

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

            byte ARmode = 0;

            while (wnd.IsOpen)
            {
                wnd.DispatchEvents();

                wnd.Clear(Content.color0);

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
                        var err = Me.Authorize(Client, ARmode, loginInput.Text(), passInput.Text());
                        submitButton.Status = StatusType.BLOCKED;
                        chgButton.Status = StatusType.BLOCKED;
                        loginInput.Status = StatusType.BLOCKED;
                        passInput.Status = StatusType.BLOCKED;
                    }

                    if (chgButton.Clicked())
                    {
                        ARmode = (byte)((int)1 - (int)ARmode);
                        string s = chgButton.Text;
                        chgButton.Text = submitButton.Text;
                        submitButton.Text = s;
                        loginInput.Clear();
                        passInput.Clear();
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
            Client.Disconnect();
            wnd.Close();
        }
    }
}
