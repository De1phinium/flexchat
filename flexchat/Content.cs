using SFML.Graphics;
using System.Collections.Generic;
using SFML.Window;

namespace flexchat
{
    class Content
    {
        public const uint UINT_MAX = 4294967295;
        public const string CONTENT_DIR = "..\\Content\\";

        public static Font font;

        public static Texture textbox0;
        public static Texture button0;
        public static Texture button1;
        public static Texture button2;

        public static Color color0;
        public static Color color1;
        public static Color color2;
        public static Color color3;

        private static SortedSet<char> symbols = new SortedSet<char>();

        public static void Load()
        {
            textbox0 = new Texture(CONTENT_DIR + "textbar0.png");
            button0 = new Texture(CONTENT_DIR + "button0.png");
            button1 = new Texture(CONTENT_DIR + "button1.png");
            button2 = new Texture(CONTENT_DIR + "button2.png");

            font = new Font(CONTENT_DIR + "arial.ttf");

            color0 = new Color(44, 47, 51, 255);
            color1 = new Color(64, 68, 75, 255);
            color2 = new Color(153, 170, 181, 255);
            color3 = new Color(114, 137, 218, 255);

            symbols.Add('+');
            symbols.Add('=');
            symbols.Add('-');
            symbols.Add('_');
            symbols.Add(')');
            symbols.Add('(');
            symbols.Add('*');
            symbols.Add('&');
            symbols.Add('^');
            symbols.Add('%');
            symbols.Add('$');
            symbols.Add('#');
            symbols.Add('@');
            symbols.Add('!');
            symbols.Add('~');
            symbols.Add('"');
            symbols.Add('\'');
            symbols.Add('?');
            symbols.Add('/');
            symbols.Add('>');
            symbols.Add('<');
            symbols.Add(':');
            symbols.Add(';');
        }

        public static bool IsSymbol(char a)
        {
            return (symbols.Contains(a));
        }

        public static char LowerCase(char a)
        {
            if (a >= 'A' && a <= 'Z')
                return (char)((byte)'a' + ((byte)a - (byte)'A'));
            else return a;
        }

        public static bool TextChar(char c)
        {
            return ((LowerCase(c) >= 'a' && LowerCase(c) <= 'z') || (c >= '0' && c <= '9'));
        }
    }
}
