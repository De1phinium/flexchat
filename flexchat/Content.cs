using SFML.Graphics;
using System.Collections.Generic;
using System;

namespace flexchat
{
    class Content
    {
        public struct tFile
        {
            public int id;
            public string filename;
            public DateTime last_checked;
        }
        public struct CachedTexture
        {
            public int FileId;
            public Texture texture;
        }

        public const uint UINT_MAX = 4294967295;
        public const string CONTENT_DIR = "..\\Content\\";
        public const string CACHE_DIR = "..\\Cache\\";
        public const int CacheSizeT = 64;
        public const int CacheSize = 128;

        private static tFile[] files = new tFile[128];
        public static CachedTexture[] cache = new CachedTexture[64];

        public static Font font;

        public static Texture Background;
        public static Texture LoginTextbox;
        public static Texture PassTextbox;
        public static Texture[,] submitbutton = new Texture[2,2];
        public static Texture[] chg = new Texture[2];
        public static Texture exitButton;
        public static Texture exitButtonSelected;
        public static Texture settings;
        public static Texture settingssel;
        public static Texture SendButton;
        public static Texture SendButton_Clicked;
        public static Texture MessageTextbox;
        public static Texture[] chgmode = new Texture[2];
        public static Texture[] chgmodeS = new Texture[2];
        public static Texture searchbox;
        public static Texture searchbutton;
        public static Texture searchbuttonS;

        public static Color colorLightGray;
        public static Color colorAlmostBlack;
        public static Color colorDarkGray;
        public static Color colorGray;
        public static Color colorAlmostWhite;
        public static Color MainColor;

        private static SortedSet<char> symbols = new SortedSet<char>();

        public static int CachedTextureId(int id)
        {
            int res = -1;
            if (id <= -1) return res;
            
            for (int i = 0; i < CacheSizeT; i++)
            {
                if (cache[i].FileId >= 0 && files[cache[i].FileId].id == id)
                {
                    res = i;
                    files[cache[i].FileId].last_checked = DateTime.Now;
                    break;
                }
            }
            if (res == -1)
            {
                Program.err.code = Error.ERROR_DATA_LENGTH;
                DateTime n = DateTime.Now;
                Program.err.text = n.ToString("yyyy-MM-dd HH:mm:ss");
                res = LoadTexture(id);
            }
            return res;
        }

        public static int LoadTexture(int id)
        {
            int res = -1;
            bool f = true;

            for (int i = 0; i < CacheSize; i++)
            {
                if (files[i].id == id)
                {
                    int toReplace = -1;
                    DateTime toReplaceTime = DateTime.Now;
                    for (int j = 0; j < CacheSizeT; j++)
                    {
                        if (cache[j].FileId == -1)
                        {
                            toReplace = j;
                            break;
                        } else if (files[cache[j].FileId].last_checked < toReplaceTime)
                        {
                            toReplace = j;
                            toReplaceTime = files[cache[j].FileId].last_checked;
                        }
                    }
                    files[i].last_checked = DateTime.Now;
                    if (cache[toReplace].texture != null)
                        cache[toReplace].texture.Dispose();
                    try
                    {
                        cache[toReplace].texture = new Texture(CACHE_DIR + files[i].filename);
                        cache[toReplace].FileId = i;
                        res = toReplace;
                    }
                    catch (Exception)
                    {
                        f = false;
                    }
                }
            }
            if (res == -1 && f)
            {
                Program.Client.DownloadFile(id);
            }
            return res;
        }

        public static void DeleteCache()
        {
            for (int i = 0; i < CacheSizeT; i++)
            {
                if (cache[i].texture != null)
                    cache[i].texture.Dispose();
                cache[i].FileId = -1;
            }
            for (int i = 0; i < CacheSize; i++)
            {
                files[i].id = -1;
                try
                {
                    System.IO.File.Delete(CACHE_DIR + files[i].filename);
                }
                catch (Exception)
                {
                    // ((
                }
                files[i].filename = "";
                files[i].last_checked = DateTime.Now;
            }
        }

        public static void AddFile(string filename)
        {
            string _id = "";
            int p = 0;
            while (filename[p] != '.')
            {
                _id += filename[p];
                p++;
            }
            int id = int.Parse(_id);
            int toReplace = 0;
            DateTime toReplaceTime = DateTime.Now;
            for (int i = 0; i < CacheSize; i++)
            {
                if (files[i].id < 0)
                {
                    toReplace = i;
                    break;
                }
                else if (files[i].last_checked < toReplaceTime)
                {
                    toReplace = i;
                    toReplaceTime = files[i].last_checked;
                }
            }
            if (files[toReplace].id >= 0)
                System.IO.File.Delete(CACHE_DIR + files[toReplace].filename);
            files[toReplace].last_checked = DateTime.Now;
            files[toReplace].id = id;
            files[toReplace].filename = filename;
            Program.Client.FilesAsked.Remove(id);
        }

        public static void Load()
        {
            for (int i = 0; i < CacheSize; i++)
            {
                files[i].id = -1;
            }
            for (int i = 0; i < CacheSizeT; i++)
            {
                cache[i].FileId = -1;
            }
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
                exitButton = new Texture(CONTENT_DIR + "exitButton.png");
                exitButtonSelected = new Texture(CONTENT_DIR + "exitButtonSelected.png");
                settings = new Texture(CONTENT_DIR + "settings.png");
                settingssel = new Texture(CONTENT_DIR + "settingssel.png");
                SendButton = new Texture(CONTENT_DIR + "sendbutton.png");
                SendButton_Clicked = new Texture(CONTENT_DIR + "sendbuttonclicked.png");
                MessageTextbox = new Texture(CONTENT_DIR + "MessageTextbox.png");
                chgmode[0] = new Texture(CONTENT_DIR + "chgmode0.png");
                chgmode[1] = new Texture(CONTENT_DIR + "chgmode1.png");
                chgmodeS[0] = new Texture(CONTENT_DIR + "chgmode0S.png");
                chgmodeS[1] = new Texture(CONTENT_DIR + "chgmode1S.png");
                searchbox = new Texture(CONTENT_DIR + "searchbox.png");
                searchbutton = new Texture(CONTENT_DIR + "searchbutton.png");
                searchbuttonS = new Texture(CONTENT_DIR + "searchbuttonS.png");

                colorAlmostBlack = new Color(35, 35, 35);
                colorDarkGray = new Color(85, 85, 85);
                colorGray = new Color(130, 130, 130);
                colorLightGray = new Color(195, 195, 195);
                colorAlmostWhite = new Color(225, 225, 225);
                MainColor = new Color(80, 72, 153);

                searchbox.Smooth = true;
                searchbutton.Smooth = true;
                searchbuttonS.Smooth = true;
                chgmode[0].Smooth = true;
                chgmode[1].Smooth = true;
                chgmodeS[0].Smooth = true;
                chgmodeS[1].Smooth = true;
                Background.Smooth = true;
                LoginTextbox.Smooth = true;
                PassTextbox.Smooth = true;
                submitbutton[0,0].Smooth = true;
                submitbutton[1,0].Smooth = true;
                submitbutton[0,1].Smooth = true;
                submitbutton[1,1].Smooth = true;
                chg[0].Smooth = true;
                chg[1].Smooth = true;
                exitButton.Smooth = true;
                exitButtonSelected.Smooth = true;
                settings.Smooth = true;
                settingssel.Smooth = true;
                SendButton.Smooth = true;
                //MessageTextbox.Smooth = true;

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
