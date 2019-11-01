using SFML.Graphics;
namespace flexchat
{
    class Error : Interface
    {
        public const uint ERROR_NONE = 0;
        public const uint ERROR_DATA_LENGTH = 1;
        public const uint ERROR_WRONG_DATA = 2;

        private uint Code;
        private string ErrorText;

        private Color textColor;
        public uint textSize;

        public Error(uint textSize, SFML.Graphics.Color textColor)
        {
            this.textSize = textSize;
            this.textColor = textColor;
        }

        public uint code 
        {
            get { return Code;  }
            set { Code = value;  }
        }

        public string text
        {
            get { return ErrorText;  }
            set { ErrorText = value;  }
        }

        public void Clear()
        {
            Code = 0;
            ErrorText = null;
        }

        public void Draw()
        {
            if (Code != ERROR_NONE)
            {
                Text text = new Text
                {
                    Font = Content.font,
                    Color = textColor,
                    DisplayedString = ErrorText,
                    CharacterSize = textSize
                };
                text.Position = new SFML.System.Vector2f(pos_x, pos_y);
                Program.wnd.Draw(text);
            }
        }
    }
}
