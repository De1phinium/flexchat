using SFML.Graphics;

namespace flexchat
{
    enum StatusType
    {
        ACTIVE,
        SELECTED,
        BLOCKED
    }
    class Interface
    {
        protected uint pos_x;
        protected uint pos_y;
        protected uint size_x;
        protected uint size_y;
        protected StatusType status;

        protected Texture[] textures;

        public StatusType Status
        {
            get { return status; }
            set { status = value; }
        }

        public Interface()
        {
            status = StatusType.ACTIVE;
            textures = new Texture[3];
        }

        public void LoadTextures(Texture active, Texture selected, Texture blocked)
        {
            textures[0] = active;
            textures[1] = selected;
            textures[2] = blocked;
        }

        public uint sizeX
        {
            get { return size_x; }
            set { size_x = value; }
        }
        public uint sizeY
        {
            get { return size_y; }
            set { size_y = value; }
        }

        public uint posX
        {
            get { return pos_x; }
            set { pos_x = value; }
        }
        public uint posY
        {
            get { return pos_y; }
            set { pos_y = value; }
        }
    }

    class TextBox : Interface
    {
        private string default_String;
        private string typed;

        private RectangleShape rect;

        private Color textColor = Color.White;

        public TextBox(string s, uint size_x, uint size_y, uint pos_x, uint pos_y)
        {
            sizeX = size_x;
            sizeY = size_y;
            posX = pos_x;
            posY = pos_y;
            default_String = s;
            typed = "";
            status = StatusType.ACTIVE;
            textures = new Texture[3];
            rect = new RectangleShape(new SFML.System.Vector2f(size_x, size_y));
        }

        public void SetTextColor(Color color)
        {
            textColor = color;
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
            Program.wnd.Draw(rect);
            Text text = new Text();
            if (Status == StatusType.SELECTED) text.DisplayedString = typed;
            else text.DisplayedString = default_String;
            text.Font = Content.font;
            text.CharacterSize = 4 * sizeY / 7;
            text.Color = textColor;
            text.Position = new SFML.System.Vector2f(posX + text.CharacterSize / 3, posY + sizeY / 2 - text.CharacterSize / 2 - (text.CharacterSize / 6));
            Program.wnd.Draw(text);
        }
    }
}
