using System;
using SFML.Graphics;

namespace flexchat
{
    class Program
    {
        static uint WND_WIDTH = 300;
        static uint WND_HEIGHT = 300;

        static RenderWindow wnd = new RenderWindow(new SFML.Window.VideoMode(WND_WIDTH, WND_HEIGHT), "FLEXCHAT");

        static void Main()
        {
            wnd.SetVerticalSyncEnabled(true);

            wnd.Closed += Win_Closed;

            while (wnd.IsOpen)
            {
                wnd.DispatchEvents();

                wnd.Clear(new Color(44, 47, 51, 1));

                wnd.Display();
            }
        }
        
        private static void Win_Closed(object sender, EventArgs e)
        {
            wnd.Close();
        }
    }
}
