using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace Minesweeper
{
    class Menu
    {
        public string title;
        public Texture2D top;
        public int selectedItem = 0;
        public MenuItem[] items = new MenuItem[6];

        public Menu(string title)
        {
            this.title = title;           
        }

        public Menu(Texture2D top)
        {
            this.top = top;
        }

        public void Add(int position, ref MenuItem item)
        {
            items[position] = item;
        }

        public void DownClick()
        {
            if (selectedItem == 5) selectedItem = 0;
            else selectedItem++;
            while (items[selectedItem] == null || !items[selectedItem].selectable)
            {
                if (selectedItem == 5) selectedItem = 0;
                else selectedItem++;
            }
        }

        public void UpClick()
        {
            if (selectedItem == 0) selectedItem = 5;
            else selectedItem--;
            while (items[selectedItem] == null || !items[selectedItem].selectable)
            {
                if (selectedItem == 0) selectedItem = 5;
                else selectedItem--;
            }
        }

        public void ClickItem()
        {
            items[selectedItem].OnClick();
        }
    }
}