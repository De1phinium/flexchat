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
        protected int pos_x;
        protected int pos_y;
        protected uint size_x;
        protected uint size_y;
        protected StatusType status;

        public Texture[] textures;

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

        public int posX
        {
            get { return pos_x; }
            set { pos_x = value; }
        }
        public int posY
        {
            get { return pos_y; }
            set { pos_y = value; }
        }
    }
}
