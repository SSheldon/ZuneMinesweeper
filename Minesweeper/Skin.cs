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
        public Texture2D itemBack, headerBack;
        public SpriteFont normal, header, small;
        public Color background, coloredText, uncoloredText;        

        public Skin(string name, string creator, bool itemsBacked, bool headersBacked, Vector3 background, Vector3 coloredText, Vector3 uncoloredText)
        {
            this.name = name;
            this.creator = creator;
            this.itemsBacked = itemsBacked;
            this.headersBacked = headersBacked;
            this.background = new Color(background);
            this.coloredText = new Color(coloredText);
            this.uncoloredText = new Color(uncoloredText);
        }

        public void InitializeTextures(string directory, ContentManager Content, SpriteFont normal, SpriteFont header, SpriteFont small)
        {
            top = Content.Load<Texture2D>(directory + "/top");
            numBox = Content.Load<Texture2D>(directory + "/number back");
            for (int counter = 0; counter < 10; counter++)
            {
                numbers[counter] = Content.Load<Texture2D>(directory + "/Numbers/" + counter);
            }
            numbers[10] = Content.Load<Texture2D>(directory + "/Numbers/-");
            numbers[11] = Content.Load<Texture2D>(directory + "/Numbers/_");
            fHappy = Content.Load<Texture2D>(directory + "/Faces/happy");
            fWin = Content.Load<Texture2D>(directory + "/Faces/win");
            fDead = Content.Load<Texture2D>(directory + "/Faces/dead");
            fScared = Content.Load<Texture2D>(directory + "/Faces/scared");
            faceSelect = Content.Load<Texture2D>(directory + "/faceselect");
            borderB = Content.Load<Texture2D>(directory + "/Borders/B");
            borderBL = Content.Load<Texture2D>(directory + "/Borders/BL");
            borderBR = Content.Load<Texture2D>(directory + "/Borders/BR");
            borderL = Content.Load<Texture2D>(directory + "/Borders/L");
            borderR = Content.Load<Texture2D>(directory + "/Borders/R");
            borderT = Content.Load<Texture2D>(directory + "/Borders/T");
            borderTL = Content.Load<Texture2D>(directory + "/Borders/TL");
            borderTR = Content.Load<Texture2D>(directory + "/Borders/TR");
            for (int counter = 0; counter < 9; counter++)
            {
                t[counter] = Content.Load<Texture2D>(directory + "/Tiles/" + counter);
            }
            tHidden = Content.Load<Texture2D>(directory + "/Tiles/hidden");
            tFlag = Content.Load<Texture2D>(directory + "/Tiles/flag");
            tMine = Content.Load<Texture2D>(directory + "/Tiles/bomb");
            tNotMine = Content.Load<Texture2D>(directory + "/Tiles/notbomb");
            tClickedMine = Content.Load<Texture2D>(directory + "/Tiles/clickedbomb");
            select = Content.Load<Texture2D>(directory + "/select");
            mTop = Content.Load<Texture2D>(directory + "/Menu/top");
            mSelect = Content.Load<Texture2D>(directory + "/Menu/select");
            rightArrow = Content.Load<Texture2D>(directory + "/Menu/right arrow");
            leftArrow = Content.Load<Texture2D>(directory + "/Menu/left arrow");
            this.normal = normal;
            this.header = header;
            this.small = small;

            if (this.itemsBacked) itemBack = Content.Load<Texture2D>(directory + "/Menu/item back");
            if (this.headersBacked) headerBack = Content.Load<Texture2D>(directory + "/Menu/header back");
        }
    }

    public class SkinContentReader : ContentTypeReader<Skin>
    {
        protected override Skin Read(ContentReader input, Skin existingInstance)
        {
            string name = input.ReadString();
            string creator = input.ReadString();
            bool itemsBacked = input.ReadBoolean();
            bool headersBacked = input.ReadBoolean();
            Vector3 background = input.ReadVector3();
            Vector3 coloredText = input.ReadVector3();
            Vector3 uncoloredText = input.ReadVector3();
            return new Skin(name, creator, itemsBacked, headersBacked, background, coloredText, uncoloredText);
        }
    }
}