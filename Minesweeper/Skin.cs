using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Minesweeper
{
    public class Skin
    {
        public string name, creator;
        public bool itemsBacked, headersBacked;

        public Texture2D top;
        public Texture2D[] numbers = new Texture2D[12];
        public Texture2D fHappy, fWin, fDead, fScared;
        public Texture2D faceSelect;
        public Texture2D borderTL, borderT, borderTR, borderR, borderBR, borderB, borderBL, borderL;
        public Texture2D[] t = new Texture2D[9];
        public Texture2D tHidden, tFlag, tMine, tNotMine, tClickedMine;
        public Texture2D select;
        public Texture2D mTop, mSelect;
        public Texture2D numBox, rightArrow, leftArrow;
        public SpriteFont normal, header, small;
        public Color background, coloredText, uncoloredText;
        public Texture2D itemBack, headerBack;        

        public Skin(string name, string creator, ContentManager Content, Color background, Color coloredText, Color uncoloredText, bool itemsBacked, bool headersBacked)
        {
            this.name = name;
            this.creator = creator;
            this.background = background;
            this.coloredText = coloredText;
            this.uncoloredText = uncoloredText;
            this.itemsBacked = itemsBacked;
            this.headersBacked = headersBacked;

            top = Content.Load<Texture2D>(name + "/top");
            for (int counter = 0; counter < 10; counter++)
            {
                numbers[counter] = Content.Load<Texture2D>(name + "/Numbers/" + counter);
            }
            numbers[10] = Content.Load<Texture2D>(name + "/Numbers/-");
            numbers[11] = Content.Load<Texture2D>(name + "/Numbers/_");
            fHappy = Content.Load<Texture2D>(name + "/Faces/happy");
            fWin = Content.Load<Texture2D>(name + "/Faces/win");
            fDead = Content.Load<Texture2D>(name + "/Faces/dead");
            fScared = Content.Load<Texture2D>(name + "/Faces/scared");
            faceSelect = Content.Load<Texture2D>(name + "/faceselect");
            borderB = Content.Load<Texture2D>(name + "/Borders/B");
            borderBL = Content.Load<Texture2D>(name + "/Borders/BL");
            borderBR = Content.Load<Texture2D>(name + "/Borders/BR");
            borderL = Content.Load<Texture2D>(name + "/Borders/L");
            borderR = Content.Load<Texture2D>(name + "/Borders/R");
            borderT = Content.Load<Texture2D>(name + "/Borders/T");
            borderTL = Content.Load<Texture2D>(name + "/Borders/TL");
            borderTR = Content.Load<Texture2D>(name + "/Borders/TR");
            for (int counter = 0; counter < 9; counter++)
            {
                t[counter] = Content.Load<Texture2D>(name + "/Tiles/" + counter);
            }
            tHidden = Content.Load<Texture2D>(name + "/Tiles/hidden");
            tFlag = Content.Load<Texture2D>(name + "/Tiles/flag");
            tMine = Content.Load<Texture2D>(name + "/Tiles/bomb");
            tNotMine = Content.Load<Texture2D>(name + "/Tiles/notbomb");
            tClickedMine = Content.Load<Texture2D>(name + "/Tiles/clickedbomb");
            select = Content.Load<Texture2D>(name + "/select");
            mTop = Content.Load<Texture2D>(name + "/Menu/top");
            mSelect = Content.Load<Texture2D>(name + "/Menu/select");
            numBox = Content.Load<Texture2D>(name + "/Menu/number back");
            rightArrow = Content.Load<Texture2D>(name + "/Menu/right arrow");
            leftArrow = Content.Load<Texture2D>(name + "/Menu/left arrow");
            normal = Content.Load<SpriteFont>(name + "/Fonts/normal");
            header = Content.Load<SpriteFont>(name + "/Fonts/header");
            small = Content.Load<SpriteFont>(name + "/Fonts/small");

            if (this.itemsBacked) itemBack = Content.Load<Texture2D>(name + "/Menu/item back");
            if (this.headersBacked) headerBack = Content.Load<Texture2D>(name + "/Menu/header back");
        }
    }
}