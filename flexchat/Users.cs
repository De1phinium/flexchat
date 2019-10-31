using System;

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

        public string Authorize(Network Client, byte mode, string login, string pass)
        {
            Client.SendData(Convert.ToString((char)mode) + Convert.ToString((char)(login.Length)) + login + Convert.ToString((char)(login.Length)) + pass);
            return "";
        }

    }
}
