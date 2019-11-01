using System;

namespace flexchat
{
    class Users
    {
        private const uint DATA_MIN_LENGTH = 4;

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

        public int Authorize(Network Client, byte mode, string login, string pass)
        {
            if (login.Length < DATA_MIN_LENGTH || pass.Length < DATA_MIN_LENGTH)
            {
                Program.err.code = Error.ERROR_DATA_LENGTH;
                Program.err.text = "Minimum length of login and password is " + Convert.ToString(DATA_MIN_LENGTH);
                return 0;
            }
            string data = Convert.ToString(mode) + Convert.ToString((char)0) + login + Convert.ToString((char)0) + pass;
            return Client.SendData(data);
        }

    }
}
