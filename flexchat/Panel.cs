using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;

namespace flexchat
{
    class Panel
    {
        public struct MenuElement
        {
            public bool selected;
            public int sizeX;
            public Texture texture;
            public Texture textureSelected;
            public EventHandler Function;
            public List<MenuElement> submenu;
        };
        const int posx = 0;
        const int posy = 0;

        private Texture texture;
        private int sizey = 0;

        public static List<MenuElement> menu;

        public MenuElement exitButton;

        public Panel(Texture texture, int sizey, Texture exitButtonTexture, Texture exitButtonTextureSelected, int exsizeX)
        {
            this.texture = texture;
            this.sizey = sizey;
            exitButton.texture = exitButtonTexture;
            exitButton.textureSelected = exitButtonTextureSelected;
            exitButton.sizeX = exsizeX;
            exitButton.selected = false;
            exitButton.submenu = null;
            menu = new List<MenuElement>();
        }

        public void Draw()
        {
            RectangleShape rect = new RectangleShape(new Vector2f(Program.wnd.Size.X, sizey));
            rect.Position = new Vector2f(posx, posy);
            rect.Texture = texture;
            Program.wnd.Draw(rect);
            RectangleShape exButton = new RectangleShape(new Vector2f(exitButton.sizeX, sizey));
            if (!exitButton.selected)
                exButton.Texture = exitButton.texture;
            else
                exButton.Texture = exitButton.textureSelected;
            exButton.Position = new Vector2f(Program.wnd.Size.X - exitButton.sizeX, 0);
            Program.wnd.Draw(exButton);
            RectangleShape line = new RectangleShape(new Vector2f(1, sizey - 2));
            line.Position = new Vector2f(Program.wnd.Size.X - exitButton.sizeX, 1);
            line.FillColor = Color.White;
            Program.wnd.Draw(line);
            int x = 0;
            for (int i = 0; i < menu.Count; i++)
            {
                RectangleShape m = new RectangleShape(new Vector2f(menu[i].sizeX, sizey));
                if (!menu[i].selected)
                    m.Texture = menu[i].texture;
                else
                    m.Texture = menu[i].textureSelected;
                m.Position = new Vector2f(x, 0);
                x += menu[i].sizeX;
                Program.wnd.Draw(m);
                line.Position = new Vector2f(x-1, 1);
                Program.wnd.Draw(line);
            }
        }

        public void Update(MouseMoveEventArgs e)
        {
            if (e.Y < 19 && e.X >= Program.wnd.Size.X - exitButton.sizeX)
            {
                exitButton.selected = true;
            }
            else
            {
                exitButton.selected = false;
            }
            int x = 0;
            for (int i = 0; i < menu.Count; i++)
            {
                if (e.Y < sizey && e.X >= x && e.X <= x + menu[i].sizeX)
                {
                    MenuElement m = menu[i];
                    m.selected = true;
                    menu[i] = m;
                }
                else
                {
                    MenuElement m = menu[i];
                    m.selected = false;
                    menu[i] = m;
                }
                x += menu[i].sizeX;
            }
        }

        public void Update(MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                if (exitButton.selected)
                {
                    exitButton.Function(null, null);
                }
                foreach (MenuElement m in menu)
                {
                    if (m.selected)
                    {
                        if (m.Function != null)
                            m.Function(null, null);
                    }
                }
            }
        }
    }
}
