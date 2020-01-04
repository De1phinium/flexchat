using SFML.Graphics;

namespace flexchat
{
    class FriendRequest
    {
        public int id;
        public int from;
        public bool hidden;

        public Button Accept;
        public Button Deny;

        public void Make()
        {
            Accept = new Button("", 40, 40, StatusType.BLOCKED);
            Deny = new Button("", 40, 40, StatusType.BLOCKED);
            Accept.LoadTextures(Content.accbutton, Content.accbuttonS, Content.accbutton);
            Deny.LoadTextures(Content.denybutton, Content.denybuttonS, Content.denybutton);
        }

        public int Draw(int ycoor)
        {
            if (ycoor > -50 && ycoor < Program.wnd.Size.Y)
            {
                string login = "\0";
                foreach (Users u in Program.users)
                {
                    if (u.ID == from)
                    {
                        login = u.Login;
                        break;
                    }
                }
                if (login != "\0")
                {
                    changeStatus(StatusType.ACTIVE);
                    Text text = new Text();
                    text.Font = Content.font;
                    text.CharacterSize = 25;
                    text.Color = Content.color1;
                    text.DisplayedString = login;
                    text.Position = new SFML.System.Vector2f(10, ycoor + 10);
                    Program.wnd.Draw(text);
                    Accept.posX = Program.CHATS_WIDTH - 100;
                    Accept.posY = System.Convert.ToUInt32(ycoor) + 5;
                    Deny.posX = Program.CHATS_WIDTH - 50;
                    Deny.posY = System.Convert.ToUInt32(ycoor) + 5;
                    Accept.Draw();
                    Deny.Draw();
                    RectangleShape rect = new RectangleShape(new SFML.System.Vector2f(Program.CHATS_WIDTH - 20, 1));
                    rect.Position = new SFML.System.Vector2f(10, ycoor + 49);
                    rect.FillColor = Content.color1;
                    Program.wnd.Draw(rect);
                }
                else
                {
                    Users u = new Users(from);
                    Program.users.Add(u);
                    u.RequestData(Program.Client);
                }
            }
            return 50;
        }

        public void changeStatus(StatusType newstatus)
        {
            if (!(newstatus == StatusType.ACTIVE && Accept.Status == StatusType.SELECTED))
                Accept.Status = newstatus;
            if (!(newstatus == StatusType.ACTIVE && Deny.Status == StatusType.SELECTED))
                Deny.Status = newstatus;
        }
    }
}
