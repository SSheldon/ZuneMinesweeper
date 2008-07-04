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
        public bool selectable = true;
        public bool colored = true; //true = black, false = gray
        public bool smallFont = false;

        public MenuItem(string text)
        {
            this.text = text;
        }

        public MenuItem(string text, bool selectable) : this(text, selectable, selectable)
        {
            
        }

        public MenuItem(string text, bool selectable, bool colored) : this(text)
        {
            this.selectable = selectable;
            this.colored = colored;
        }

        public MenuItem(string text, bool selectable, bool colored, bool smallFont) : this(text, selectable, colored)
        {
            this.smallFont = smallFont;
        }

        public event ItemClick itemClicked;
        
        public void OnClick()
        {
            itemClicked();
        }
    }
}