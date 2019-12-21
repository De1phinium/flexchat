using SFML.Graphics;
using SFML.Window;

namespace flexchat
{
    class Button : Interface
    {
        private string text;

        private bool WasClicked;

        private Color[] textColor;
        public uint textSize;
        public short workMode = 0;

        public string Text
        {
            get { return text;  }
            set { text = value;  }
        }

        public bool Clicked()
        {
            if (WasClicked)
            {
                WasClicked = false;
                return true;
            }
            return false;
        }

        public Button(string text, uint size_x, uint size_y, StatusType status)
        {
            this.text = text;
            this.size_x = size_x;
            this.size_y = size_y;
            this.status = status;
            textColor = new Color[3];
            WasClicked = false;
        }

        public void Update(MouseMoveEventArgs e)
        {
            if (status == StatusType.BLOCKED)
                return;
            if (e.X >= pos_x && e.X <= pos_x + size_x && e.Y >= pos_y && e.Y <= pos_y + size_y)
            {
                status = StatusType.SELECTED;
            }
            else
            {
                status = StatusType.ACTIVE;
            }
        }

        public void Update(MouseButtonEventArgs e)
        {
            if (status == StatusType.BLOCKED)
                return;
            if (e.Button == Mouse.Button.Left && status == StatusType.SELECTED)
            {
                WasClicked = true;
            }
        }

        public void SetTextColor(Color colorActive, Color colorSelected, Color colorBlocked)
        {
            textColor[0] = colorActive;
            textColor[1] = colorSelected;
            textColor[2] = colorBlocked;
        }
        public void Draw()
        {
            RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(size_x, size_y))
            {
                Texture = textures[(byte)status],
                Position = new SFML.System.Vector2f(pos_x, pos_y)
            };
            if (rect.Texture == null) rect.FillColor = Color.Red;
            Program.wnd.Draw(rect);
            Text text = new Text
            {
                Font = Content.font,
                Color = textColor[(byte)status],
                DisplayedString = this.text,
                CharacterSize = textSize
            };
            text.Position = new SFML.System.Vector2f(pos_x + (size_x / 2) - text.DisplayedString.Length / 2 * text.CharacterSize / 2 - 4, pos_y + size_y / 2 - text.CharacterSize / 2 - 4);
            Program.wnd.Draw(text);
        }
    }
}
