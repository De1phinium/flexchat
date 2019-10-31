namespace flexchat
{
    class Users
    {
        private string login;
        private uint id;
        
        public string Login
        {
            get { return login;  }
        }

        public uint ID
        {
            get { return id;  }
        }

        public Users()
        {

        }

        public Users(string login, uint id)
        {
            this.login = login;
            this.id = id;
        }

        public string Authorize(byte mode, string login, string pass)
        {
            string err = "";
            return err;
        }

    }
}
