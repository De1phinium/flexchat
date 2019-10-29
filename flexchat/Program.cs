using System;
using SFML.Graphics;

namespace flexchat
{
    class Program
    {
        static uint WND_WIDTH = 700;
        static uint WND_HEIGHT = 750;
        static uint WND_MIN_WIDTH = 400;
        static uint WND_MIN_HEIGHT = 300;

        static RenderWindow wnd = new RenderWindow(new SFML.Window.VideoMode(WND_WIDTH, WND_HEIGHT), "FLEXCHAT");

        static void Main()
        {
            wnd.SetVerticalSyncEnabled(true);

            wnd.Closed += Win_Closed;
            wnd.Resized += Win_Resized;

            RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(100, 100));
            rect.FillColor = Color.White;

            int vectx = 2, vecty = 2;

            var rand = new Random();

            while (wnd.IsOpen)
            {
                wnd.DispatchEvents();

                wnd.Clear(new Color(44, 47, 51, 1));

                SFML.System.Vector2f pos = rect.Position;
                if (pos.X + vectx <= 0 || pos.X + vectx >= WND_WIDTH - 100)
                {
                    vectx *= -1;
                    rect.FillColor = new Color((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));
                }
                if (pos.Y + vecty <= 0 || pos.Y + vecty >= WND_HEIGHT - 100)
                {
                    vecty *= -1;
                    rect.FillColor = new Color((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));
                }
                pos.X += vectx;
                pos.Y += vecty;

                rect.Position = pos;

                wnd.Draw(rect);

                wnd.Display();
            }
        }
        
        private static void Win_Resized(object sender, SFML.Window.SizeEventArgs e)
        {
            if (e.Width < WND_MIN_WIDTH)
            {
                wnd.Size = new SFML.System.Vector2u(WND_MIN_WIDTH, e.Height);
                e.Width = WND_MIN_WIDTH;
            }
            if (e.Height < WND_MIN_HEIGHT)
            {
                wnd.Size = new SFML.System.Vector2u(e.Width, WND_MIN_HEIGHT);
                e.Height = WND_MIN_HEIGHT;
            }
            wnd.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
            WND_WIDTH = e.Width; WND_HEIGHT = e.Height;
        }

        private static void Win_Closed(object sender, EventArgs e)
        {
            wnd.Close();
        }
    }
}
