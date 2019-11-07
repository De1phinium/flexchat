using SFML.Graphics;
using System.Collections.Generic;
using System;

namespace flexchat
{
    class Content
    {
        public const uint UINT_MAX = 4294967295;
        public const string CONTENT_DIR = "..\\Content\\";

        public static Font font;

        public static Texture Background;
        public static Texture LoginTextbox;
        public static Texture PassTextbox;
        public static Texture[,] submitbutton = new Texture[2,2];
        public static Texture[] chg = new Texture[2];
        public static Texture panel;
        public static Texture exitButton;
        public static Texture exitButtonSelected;
        public static Texture settings;
        public static Texture settingssel;

        private static SortedSet<char> symbols = new SortedSet<char>();

        public static void Load()
        {
            try
            {
                Background = new Texture(CONTENT_DIR + "background.png");
                LoginTextbox = new Texture(CONTENT_DIR + "logininp.png");
                PassTextbox = new Texture(CONTENT_DIR + "passinp.png");
                submitbutton[0,0] = new Texture(CONTENT_DIR + "signinbutton.png");
                submitbutton[1,0] = new Texture(CONTENT_DIR + "signupbutton.png");
                submitbutton[0,1] = new Texture(CONTENT_DIR + "signinbuttonsel.png");
                submitbutton[1,1] = new Texture(CONTENT_DIR + "signupbuttonsel.png");
                chg[0] = new Texture(CONTENT_DIR + "chgsignup.png");
                chg[1] = new Texture(CONTENT_DIR + "chgsignin.png");
                panel = new Texture(CONTENT_DIR + "panel.png");
                exitButton = new Texture(CONTENT_DIR + "exitButton.png");
                exitButtonSelected = new Texture(CONTENT_DIR + "exitButtonSelected.png");
                settings = new Texture(CONTENT_DIR + "settings.png");
                settingssel = new Texture(CONTENT_DIR + "settingssel.png");

                Background.Smooth = true;
                LoginTextbox.Smooth = true;
                PassTextbox.Smooth = true;
                submitbutton[0,0].Smooth = true;
                submitbutton[1,0].Smooth = true;
                submitbutton[0,1].Smooth = true;
                submitbutton[1,1].Smooth = true;
                chg[0].Smooth = true;
                chg[1].Smooth = true;
                panel.Smooth = true;
                exitButton.Smooth = true;
                exitButtonSelected.Smooth = true;
                settings.Smooth = true;
                settingssel.Smooth = true;

                font = new Font(CONTENT_DIR + "tahoma.ttf");
            }
            catch (Exception)
            {
                Environment.Exit(1337);
            }
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
