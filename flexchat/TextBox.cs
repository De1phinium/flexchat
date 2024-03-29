﻿using SFML.Graphics;
using SFML.Window;
using System;

namespace flexchat
{
    class TextBox : Interface
    {
        public string typed = "";
        public string before = "";
        public string defaultString = "";
        private int textOffset;
        private Color textColor;
        private int BlinkingRatio = 30;
        private int blink = 1;
        public int textLengthBound;
        public string sub = "";
        public bool symbolsAllowed;
        public bool SpacebarAllowed;
        public bool evAllowed;
        public uint textSize = 22;

        public short workMode = 0;

        public bool Changed()
        {
            if (before == typed)
                return false;
            else
            {
                before = typed;
                return true;
            }
        }

        public TextBox(uint sizex, uint sizey, uint textSize, Texture active, Texture selected, int textOffset, Color textColor)
        {
            Status = StatusType.ACTIVE;
            this.textSize = textSize;
            sizeX = sizex;
            sizeY = sizey;
            textures = new Texture[3];
            textures[0] = active;
            textures[1] = selected;
            textures[2] = active;
            evAllowed = false;
            this.textOffset = textOffset;
            this.textColor = textColor;
        }

        public void Update(TextEventArgs e)
        {
            if (Status == StatusType.SELECTED)
            {
                if (e.Unicode[0] == '\b') // If it is a backspace
                {
                    if (typed.Length > 0) typed = typed.Substring(0, typed.Length - 1);
                    return;
                }
                if (evAllowed || (Content.IsSymbol(e.Unicode[0]) && symbolsAllowed) || (e.Unicode[0] == ' ' && SpacebarAllowed) || Content.TextChar(e.Unicode[0]))
                {
                    if (typed.Length < textLengthBound)
                    {
                        /*byte[] symbol = System.Text.Encoding.GetEncoding(1251).GetBytes(e.Unicode);
                        string binary = Convert.ToString(symbol[0], 2);
                        typed += (char)symbol[0];
                        symbol[0] = Convert.ToByte(binary, 2);*/
                        typed += e.Unicode;
                    }
                }
            }
        }


        public void Update(MouseButtonEventArgs mouse)
        {
            if (Status == StatusType.BLOCKED)
                return;
            if (mouse.Button == Mouse.Button.Left)
            {
                if (mouse.X >= pos_x && mouse.X <= pos_x + size_x && mouse.Y >= pos_y && mouse.Y <= pos_y + size_y)
                {
                    Status = StatusType.SELECTED;
                }
                else
                {
                    Status = StatusType.ACTIVE;
                }
            }
        }

        public void Draw()
        {
            RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(sizeX, sizeY));
            rect.Position = new SFML.System.Vector2f(pos_x, pos_y);
            rect.Texture = textures[(uint)status];
            rect.Size = new SFML.System.Vector2f(size_x, size_y);
            if (rect.Texture == null) rect.FillColor = Color.Red;
            Program.wnd.Draw(rect);
            Text text = new Text();
            text.Color = textColor;
            text.DisplayedString = defaultString;
            if (typed != "" || status == StatusType.SELECTED)
            {
                if (sub != "")
                {
                    string s = "";
                    for (var i = 0; i < typed.Length; i++)
                    {
                        s += sub;
                    }
                    text.DisplayedString = s;
                }
                else text.DisplayedString = typed;
            }
            text.Font = Content.font;
            text.CharacterSize = textSize;
            text.DisplayedString = "Ig" + text.DisplayedString;
            var bounds = text.GetLocalBounds();
            text.DisplayedString = text.DisplayedString.Substring(2, text.DisplayedString.Length - 2);
            text.Position = new SFML.System.Vector2f(pos_x + textOffset, pos_y + (size_y / 2) - (bounds.Height / 2) - 7);
            Program.wnd.Draw(text);
            if (status == StatusType.SELECTED)
            {
                blink--;
                if (blink <= BlinkingRatio / 2)
                {
                    if (blink == 0) blink = BlinkingRatio;
                    FloatRect textRect = text.GetLocalBounds();
                    rect.Texture = null;
                    rect.Size = new SFML.System.Vector2f(2, sizeY * 3 / 5);
                    rect.FillColor = textColor;
                    rect.Position = new SFML.System.Vector2f(pos_x + textOffset + Convert.ToInt32(textRect.Width) + 5, pos_y + (size_y / 2) - (rect.Size.Y / 2) - 2);
                    Program.wnd.Draw(rect);
                }
            }
        }
    }
}
