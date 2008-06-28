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
    class MenuItem
    {
        public string text;
        public SpriteFont font;
        public bool selectable = true;
        public bool color = true; //true = black, false = gray
        public bool smallFont = false;

        public MenuItem(string text, SpriteFont font)
        {
            this.text = text;
            this.font = font;
        }

        public MenuItem(string text, bool selectable, SpriteFont font) : this(text, selectable, selectable, font)
        {
            
        }

        public MenuItem(string text, bool selectable, bool color, SpriteFont font) : this(text, font)
        {
            this.selectable = selectable;
            this.color = color;
        }

        public event ItemClick itemClicked;
        
        public void OnClick()
        {
            itemClicked();
        }
    }
}