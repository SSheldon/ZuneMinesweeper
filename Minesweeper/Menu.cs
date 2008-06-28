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
        string title;
        SpriteFont headerFont;
        Texture2D top;
        Texture2D select;
        public int selectedItem = 0;
        MenuItem[] items = new MenuItem[6];

        public Menu(string title, SpriteFont font, Texture2D select)
        {
            this.title = title;
            headerFont = font;
            this.select = select;            
        }

        public Menu(Texture2D top, Texture2D select)
        {
            this.top = top;
            this.select = select;
        }

        public void Add(int position, ref MenuItem item)
        {
            items[position] = item;
        }

        public void Draw(SpriteBatch batch)
        {
            if (top != null) batch.Draw(top, new Rectangle(0, 0, 240, 72), Color.White);
            else batch.DrawString(headerFont, title, new Vector2(4, 16), Color.Black);
            for (int counter = 0; counter < 6; counter++)
            {
                if (items[counter] != null)
                {
                    Color fontColor;
                    fontColor = items[counter].color ? Color.Black : Color.Gray;
                    int y;
                    if (items[counter].smallFont) y = 12;
                    else y = 0;
                    batch.DrawString(items[counter].font, items[counter].text, new Vector2(16, 72 + counter * 40 + y), fontColor);
                }
            }

            while (items[selectedItem] == null || !items[selectedItem].selectable)
            {
                if (selectedItem == 5) selectedItem = 0;
                else selectedItem++;
            }
            batch.Draw(select, new Vector2(14, 78 + selectedItem * 40), Color.White);
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