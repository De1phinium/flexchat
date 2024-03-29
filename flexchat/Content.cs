﻿using SFML.Graphics;
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

        public struct CachedFile
        {
            public int FileId;
        }


        public const uint UINT_MAX = 4294967295;
        public const string CONTENT_DIR = "..\\Content\\";
        public const string CACHE_DIR = "..\\Cache\\";
        public const int CacheSizeT = 64;
        public const int CacheSize = 128;

        private static tFile[] files = new tFile[128];
        public static CachedTexture[] cacheIMG = new CachedTexture[64];
        public static CachedFile[] cacheF = new CachedFile[64];

        public static Font font;

        public static Texture LoginTextbox;
        public static Texture PassTextbox;
        public static Texture[,] submitbutton = new Texture[2,2];
        public static Texture[] chg = new Texture[2];
        public static Texture SendButton;
        public static Texture SendButton_Clicked;
        public static Texture MessageTextbox;
        public static Texture[] chgmode = new Texture[2];
        public static Texture searchbox;
        public static Texture frreq;
        public static Texture frreqS;
        public static Texture remfr;
        public static Texture remfrS;
        public static Texture cancelreq;
        public static Texture cancelreqS;
        public static Texture accreq;
        public static Texture accreqS;
        public static Texture accbutton;
        public static Texture accbuttonS;
        public static Texture denybutton;
        public static Texture denybuttonS;
        public static Texture titlebox;
        public static Texture convmenu;
        public static Texture convmenuS;
        public static Texture chgtitle;
        public static Texture chgtitleS;
        public static Texture createconv;
        public static Texture createconvS;
        public static Texture chgphoto;
        public static Texture chgphotoS;
        public static Texture mic;
        public static Texture micS;
        public static Texture stop;
        public static Texture stopS;
        public static Texture prMsg;
        public static Texture prMsgS;
        public static Texture friend_request_list;
        public static Texture friend_request_listB;
        public static Texture playvoicemsg;
        public static Texture pausevoicemsg;
        public static Texture msgBoxCornerSW;
        public static Texture msgBoxCornerSE;
        public static Texture msgBoxCornerNW;
        public static Texture msgBoxCornerNE;

        public static Color color1;
        public static Color color2;

        private static SortedSet<char> symbols = new SortedSet<char>();

        public static Texture GetTexture(int cachedTextureId)
        {
            return cacheIMG[cachedTextureId].texture;
        }

        public static int GetFileType(int fileId)
        {
            int dotIndex = 0;
            while (files[fileId].filename[dotIndex] != '.')
                dotIndex++;
            string format = files[fileId].filename.Substring(dotIndex + 1);
            switch (format)
            {
                case "wav":
                    return 1;
                default:
                    return -1;
            }
        }

        public static string GetFilePath(int fileId)
        {
            return CACHE_DIR + files[fileId].filename;
        }

        public static int SearchFileInCache(int id)
        {
            int res = -1;

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].id == id)
                {
                    res = i;
                    break;
                }
            }

            if (res == -1) Program.Client.DownloadFile(id);
            return res;
        }

        public static int CachedTextureId(int id)
        {
            int res = -1;
            if (id <= -1) return res;
            
            for (int i = 0; i < CacheSizeT; i++)
            {
                if (cacheIMG[i].FileId >= 0 && files[cacheIMG[i].FileId].id == id)
                {
                    res = i;
                    files[cacheIMG[i].FileId].last_checked = DateTime.Now;
                    break;
                }
            }
            if (res == -1)
            {
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
                        if (cacheIMG[j].FileId == -1)
                        {
                            toReplace = j;
                            break;
                        } else if (files[cacheIMG[j].FileId].last_checked < toReplaceTime)
                        {
                            toReplace = j;
                            toReplaceTime = files[cacheIMG[j].FileId].last_checked;
                        }
                    }
                    files[i].last_checked = DateTime.Now;
                    if (cacheIMG[toReplace].texture != null)
                        cacheIMG[toReplace].texture.Dispose();
                    try
                    {
                        cacheIMG[toReplace].texture = new Texture(CACHE_DIR + files[i].filename);
                        cacheIMG[toReplace].FileId = i;
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
                if (cacheIMG[i].texture != null)
                    cacheIMG[i].texture.Dispose();
                cacheIMG[i].FileId = -1;
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
                cacheIMG[i].FileId = -1;
            }
            try
            {
                LoginTextbox = new Texture(CONTENT_DIR + "logininp.png");
                PassTextbox = new Texture(CONTENT_DIR + "passinp.png");
                submitbutton[0,0] = new Texture(CONTENT_DIR + "signinbutton.png");
                submitbutton[1,0] = new Texture(CONTENT_DIR + "signupbutton.png");
                submitbutton[0,1] = new Texture(CONTENT_DIR + "signinbuttonsel.png");
                submitbutton[1,1] = new Texture(CONTENT_DIR + "signupbuttonsel.png");
                chg[0] = new Texture(CONTENT_DIR + "chgsignup.png");
                chg[1] = new Texture(CONTENT_DIR + "chgsignin.png");
                SendButton = new Texture(CONTENT_DIR + "sendbutton.png");
                SendButton_Clicked = new Texture(CONTENT_DIR + "sendbuttonclicked.png");
                MessageTextbox = new Texture(CONTENT_DIR + "MessageTextBox.png");
                chgmode[0] = new Texture(CONTENT_DIR + "chgmode0.png");
                chgmode[1] = new Texture(CONTENT_DIR + "chgmode1.png");
                searchbox = new Texture(CONTENT_DIR + "searchbox.png");
                frreq = new Texture(CONTENT_DIR + "frreq.png");
                frreqS = new Texture(CONTENT_DIR + "frreqS.png");
                remfr = new Texture(CONTENT_DIR + "remfr.png");
                remfrS = new Texture(CONTENT_DIR + "remfrS.png");
                cancelreq = new Texture(CONTENT_DIR + "cancelreq.png");
                cancelreqS = new Texture(CONTENT_DIR + "cancelreqS.png");
                accreq = new Texture(CONTENT_DIR + "accreq.png");
                accreqS = new Texture(CONTENT_DIR + "accreqS.png");
                accbutton = new Texture(CONTENT_DIR + "accbutton.png");
                accbuttonS = new Texture(CONTENT_DIR + "accbuttonS.png");
                denybutton = new Texture(CONTENT_DIR + "denybutton.png");
                denybuttonS = new Texture(CONTENT_DIR + "denybuttonS.png");
                titlebox = new Texture(CONTENT_DIR + "titlebox.png");
                convmenu = new Texture(CONTENT_DIR + "convmenu.png");
                convmenuS = new Texture(CONTENT_DIR + "convmenuS.png");
                chgtitle = new Texture(CONTENT_DIR + "chgtitle.png");
                chgtitleS = new Texture(CONTENT_DIR + "chgtitleS.png");
                createconv = new Texture(CONTENT_DIR + "createconv.png");
                createconvS = new Texture(CONTENT_DIR + "createconvS.png");
                chgphoto = new Texture(CONTENT_DIR + "chgphoto.png");
                chgphotoS = new Texture(CONTENT_DIR + "chgphotoS.png");
                mic = new Texture(CONTENT_DIR + "mic.png");
                micS = new Texture(CONTENT_DIR + "micS.png");
                stop = new Texture(CONTENT_DIR + "stop.png");
                stopS = new Texture(CONTENT_DIR + "stopS.png");
                prMsg = new Texture(CONTENT_DIR + "prmsg.png");
                prMsgS = new Texture(CONTENT_DIR + "prmsgS.png");
                friend_request_list = new Texture(CONTENT_DIR + "friend_requests_list.png");
                friend_request_listB = new Texture(CONTENT_DIR + "friend_requests_listB.png");
                playvoicemsg = new Texture(CONTENT_DIR + "playvoicemsg.png");
                pausevoicemsg = new Texture(CONTENT_DIR + "pausevoicemsg.png");
                msgBoxCornerSW = new Texture(CONTENT_DIR + "msgBoxCornerSW.png");
                msgBoxCornerSE = new Texture(CONTENT_DIR + "msgBoxCornerSE.png");
                msgBoxCornerNW = new Texture(CONTENT_DIR + "msgBoxCornerNW.png");
                msgBoxCornerNE = new Texture(CONTENT_DIR + "msgBoxCornerNE.png");

                /*colorAlmostBlack = new Color(35, 35, 35);
                colorDarkGray = new Color(85, 85, 85);
                colorGray = new Color(130, 130, 130);
                colorLightGray = new Color(195, 195, 195);
                colorAlmostWhite = new Color(225, 225, 225);
                MainColor = new Color(80, 72, 153);*/

                color1 = new Color(250, 250, 250);
                color2 = new Color(6, 110, 119);

                mic.Smooth = true;
                micS.Smooth = true;
                stop.Smooth = true;
                stopS.Smooth = true;
                chgphoto.Smooth = true;
                chgphotoS.Smooth = true;
                createconv.Smooth = true;
                createconvS.Smooth = true;
                convmenu.Smooth = true;
                convmenuS.Smooth = true;
                chgtitle.Smooth = true;
                chgtitleS.Smooth = true;
                accbutton.Smooth = true;
                accbuttonS.Smooth = true;
                denybutton.Smooth = true;
                denybuttonS.Smooth = true;
                cancelreq.Smooth = true;
                cancelreqS.Smooth = true;
                accreq.Smooth = true;
                accreqS.Smooth = true;
                remfr.Smooth = true;
                remfrS.Smooth = true;
                frreq.Smooth = true;
                frreqS.Smooth = true;
                searchbox.Smooth = true;
                chgmode[0].Smooth = true;
                chgmode[1].Smooth = true;
                LoginTextbox.Smooth = true;
                PassTextbox.Smooth = true;
                submitbutton[0,0].Smooth = true;
                submitbutton[1,0].Smooth = true;
                submitbutton[0,1].Smooth = true;
                submitbutton[1,1].Smooth = true;
                chg[0].Smooth = true;
                chg[1].Smooth = true;
                SendButton.Smooth = true;
                SendButton_Clicked.Smooth = true;
                MessageTextbox.Smooth = true;
                msgBoxCornerNE.Smooth = true;
                msgBoxCornerNW.Smooth = true;
                msgBoxCornerSE.Smooth = true;
                msgBoxCornerSW.Smooth = true;

                playvoicemsg.Smooth = true;
                pausevoicemsg.Smooth = true;

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
