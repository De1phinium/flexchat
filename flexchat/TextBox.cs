using SFML.Graphics;
using SFML.Window;

namespace flexchat
{
    class TextBox : Interface
    {
        private string default_String;
        private string typed;
        private short cursorBlinkingRatio;
        private short blink;
        private bool CursorBlinking;
        private string cursor;
        private string sub;
        public bool symbolsAllowed;
        public bool spaceBarAllowed;

        public uint textLengthBound = 255;

        private RectangleShape rect;

        private Color []textColor;

        public TextBox(string s, uint size_x, uint size_y, StatusType status)
        {
            symbolsAllowed = false;
            spaceBarAllowed = true;
            sizeX = size_x;
            sizeY = size_y;
            posX = pos_x;
            posY = pos_y;
            default_String = s;
            typed = "";
            this.status = status;
            textures = new Texture[3];
            rect = new RectangleShape(new SFML.System.Vector2f(size_x, size_y));
            textColor = new Color[3];
            sub = "";
        }

        public void SetSub(string sub)
        {
            this.sub = sub;
        }

        public void SetCursor(short ratio, bool blinking, string cursor)
        {
            cursorBlinkingRatio = ratio;
            blink = cursorBlinkingRatio;
            CursorBlinking = blinking;
            this.cursor = cursor;
        }

        public string Text()
        {
            return typed;
        }

        public void Clear()
        {
            typed = "";
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
                if ((Content.IsSymbol(e.Unicode[0]) && symbolsAllowed) || (e.Unicode[0] == ' ' && spaceBarAllowed) || Content.TextChar(e.Unicode[0]))
                {
                    if (typed.Length < textLengthBound)
                    {
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
        public void SetTextColor(Color colorActive, Color colorSelected, Color colorBlocked)
        {
            textColor[0] = colorActive;
            textColor[1] = colorSelected;
            textColor[2] = colorBlocked;
        }

        public string defaultString
        {
            set { default_String = value; }
        }

        public void Draw()
        {
            rect.Position = new SFML.System.Vector2f(pos_x, pos_y);
            rect.Texture = textures[(uint)status];
            rect.Size = new SFML.System.Vector2f(size_x, size_y);
            if (rect.Texture == null) rect.FillColor = Content.color0;
            Program.wnd.Draw(rect);
            Text text = new Text();
            text.Color = textColor[(byte)status];
            bool hidden = false;
            if (typed == "") text.DisplayedString = default_String;
            else
            {
                text.DisplayedString = typed;
                if (sub != "")
                {
                    string s = "";
                    for (int i = 0; i < text.DisplayedString.Length; i++)
                        s += sub;
                    text.DisplayedString = s;
                    hidden = true;
                }
            }
            if (Status == StatusType.SELECTED)
            {
                if (!hidden)
                {
                    text.DisplayedString = typed;
                    if (sub != "")
                    {
                        string s = "";
                        for (int i = 0; i < text.DisplayedString.Length; i++)
                            s += sub;
                        text.DisplayedString = s;
                    }
                }
                text.Color = textColor[(byte)StatusType.SELECTED];
                if (CursorBlinking)
                {
                    blink--;
                    if (blink <= cursorBlinkingRatio / 2)
                    {
                        if (blink == 0) blink = cursorBlinkingRatio;
                        text.DisplayedString += cursor;
                    }
                }
            }
            text.Font = Content.font;
            text.CharacterSize = 4 * sizeY / 7;
            text.Position = new SFML.System.Vector2f(posX + text.CharacterSize / 3, posY + sizeY / 2 - text.CharacterSize / 2 - (text.CharacterSize / 6));
            Program.wnd.Draw(text);
        }
    }
}
