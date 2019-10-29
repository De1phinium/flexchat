using System;
using SFML.Graphics;

namespace flexchat
{
    class Program
    {
        static uint WND_WIDTH = 700;
        static uint WND_HEIGHT = 500;

        public static RenderWindow wnd = new RenderWindow(new SFML.Window.VideoMode(WND_WIDTH, WND_HEIGHT), "FLEXCHAT");

        static void Main()
        {
            wnd.SetVerticalSyncEnabled(true);

            wnd.Closed += Win_Closed;
            wnd.Resized += Win_Resized;

            Content.Load();

            TextBox loginInput = new TextBox("login", 500, 50, 100, 100);
            loginInput.LoadTextures(Content.textbox0, Content.textbox0, Content.textbox0);
            loginInput.SetTextColor(Content.color2);

            TextBox passInput = new TextBox("password", 500, 50, 100, 100);
            passInput.LoadTextures(Content.textbox0, Content.textbox0, Content.textbox0);
            passInput.SetTextColor(Content.color2);

            while (wnd.IsOpen)
            {
                wnd.DispatchEvents();

                wnd.Clear(Content.color0);

                loginInput.posX = (wnd.Size.X / 2) - (loginInput.sizeX / 2);
                loginInput.posY = (wnd.Size.Y / 2) - loginInput.sizeY - 50;
                loginInput.Draw();

                passInput.posX = (wnd.Size.X / 2) - (passInput.sizeX / 2);
                passInput.posY = (wnd.Size.Y / 2) - passInput.sizeY - 30 + passInput.sizeY;
                passInput.Draw();

                wnd.Display();
            }
        }
        
        private static void Win_Resized(object sender, SFML.Window.SizeEventArgs e)
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

        private static void Win_Closed(object sender, EventArgs e)
        {
            wnd.Close();
        }
    }
}
