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
    public delegate void ItemClick();

    public class MenuItem
    {
        public string text;
        public bool selectable = true;
        public bool colored = true; //true = black, false = gray
        public bool smallFont = false;
        public bool backed = true;

        public MenuItem(string text)
            : this(text, true, true, false) { }

        public MenuItem(string text, bool selectable) 
            : this(text, selectable, selectable, false) { }

        public MenuItem(string text, bool selectable, bool colored) 
            : this(text, selectable, colored, false) { }

        public MenuItem(string text, bool selectable, bool colored, bool smallFont)
        {
            this.text = text;
            this.selectable = selectable;
            this.colored = colored;
            this.smallFont = smallFont;
        }

        //public MenuItem(string text, bool selectable = true, bool colored = true, bool smallFont = false)
        //{
        //    this.smallFont = smallFont;
        //}
        
        public event ItemClick itemClicked;
        //public event EventHandler<EventArgs> Clicked;
        
        public void OnClick()
        {
            itemClicked();
            //if (Clicked != null) Clicked(this, EventArs.Empty);
        }
    }
}