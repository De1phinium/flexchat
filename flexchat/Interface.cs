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
        private uint pos_x;
        private uint pos_y;
        private uint size_x;
        private uint size_y;
        private StatusType status;

        public StatusType Status
        {
            get { return status; }
            set { status = value; }
        }

        public Interface()
        {
            status = StatusType.ACTIVE;
        }

        public void SetPos(uint pos_x, uint pos_y)
        {
            this.pos_x = pos_x;
            this.pos_y = pos_y;
        }

        public void Clicked()
        {

        }

        public void GetTexture()
        {

        }
    }
}
