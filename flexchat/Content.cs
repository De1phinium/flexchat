using SFML.Graphics;

namespace flexchat
{
    class Content
    {
        public const string CONTENT_DIR = "..\\Content\\";

        public static Font font;
        public static Texture textbox0;

        public static Color color0;
        public static Color color1;
        public static Color color2;

        public static void Load()
        {
            textbox0 = new Texture(CONTENT_DIR + "textbar0.png");
            font = new Font(CONTENT_DIR + "arial.ttf");
            color0 = new Color(44, 47, 51, 255);
            color1 = new Color(64, 68, 75, 255);
            color2 = new Color(153, 170, 181, 255);
        }
    }
}
