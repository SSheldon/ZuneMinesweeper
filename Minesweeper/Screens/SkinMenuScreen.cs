using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public class SkinMenuScreen : MenuScreen
    {
        MenuItem skinMI, change, back;
        int tempSelectedSkin;
        Texture2D blank;

        public SkinMenuScreen(MinesweeperGame game)
            : base(game, "Skin:")
        {
            tempSelectedSkin = Game.options.SelectedSkin;
            
            skinMI = new MenuItem("Classic");
            Add(0, skinMI);
            change = new MenuItem("Change");
            change.itemClicked += new ItemClick(ChangeClick);
            Add(4, change);
            back = new MenuItem("Back");
            back.itemClicked += new ItemClick(Back);
            Add(5, back);
        }

        public override void LoadContent()
        {
            blank = new Texture2D(ScreenManager.GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            blank.SetData<Color>(new Color[] { Color.White });
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            skinMI.text = Game.skins[tempSelectedSkin].name;

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        protected override void DrawSelect(SpriteBatch batch)
        {
            batch.Draw(Game.Skin.leftArrow, new Vector2(1, 85), Color.White);
            batch.Draw(Game.Skin.rightArrow, new Vector2(14 + 212 + 2, 85), Color.White);
            batch.DrawString(Game.Skin.normal, "By " + Game.skins[tempSelectedSkin].creator, new Vector2(14, 118), Game.Skin.coloredText);
            DrawSkinSample(batch);
            batch.Draw(Game.Skin.mSelect, new Vector2(14, 78 + selectedItem * 40), Color.White);
        }

        private void DrawSkinSample(SpriteBatch spriteBatch)
        {
            Skin s = Game.skins[tempSelectedSkin];
            spriteBatch.Draw(blank, new Rectangle(22, 166, 196, 64), s.background);
            spriteBatch.Draw(s.numBox, new Vector2(30, 191), Color.White);
            spriteBatch.Draw(s.numbers[10], new Vector2(38, 199), Color.White);
            spriteBatch.Draw(s.numbers[4], new Vector2(51, 199), Color.White);
            spriteBatch.Draw(s.numbers[9], new Vector2(64, 199), Color.White);
            spriteBatch.Draw(s.tHidden, new Vector2(30, 174), Color.White);
            spriteBatch.Draw(s.tFlag, new Vector2(46, 174), Color.White);
            spriteBatch.Draw(s.tMine, new Vector2(62, 174), Color.White);
            spriteBatch.Draw(s.tClickedMine, new Vector2(78, 174), Color.White);
            spriteBatch.Draw(s.tNotMine, new Vector2(94, 174), Color.White);
            for (int counter = 0; counter < 7; counter++)
            {
                spriteBatch.Draw(s.t[counter], new Vector2(94 + 16 * counter, 174), Color.White);
            }
            spriteBatch.Draw(s.t[7], new Vector2(190, 190), Color.White);
            spriteBatch.Draw(s.t[8], new Vector2(190, 206), Color.White);
            spriteBatch.Draw(s.fHappy, new Vector2(110 - 24, 194), Color.White);
            spriteBatch.Draw(s.fScared, new Vector2(134 - 24, 194), Color.White);
            spriteBatch.Draw(s.fDead, new Vector2(158 - 24, 194), Color.White);
            spriteBatch.Draw(s.fWin, new Vector2(182 - 24, 194), Color.White);
        }

        protected override void RightClick()
        {            
            if (selectedItem == 0)
            {
                if (tempSelectedSkin == Game.skins.Count - 1) tempSelectedSkin = 0;
                else tempSelectedSkin++;
            }
            int a = ScreenManager.GetScreens().Length;
        }

        protected override void LeftClick()
        {
            
            if (selectedItem == 0)
            {
                if (tempSelectedSkin == 0) tempSelectedSkin = Game.skins.Count - 1;
                else tempSelectedSkin--;
            }
        }


        void ChangeClick()
        {
            Game.options.SelectedSkin = tempSelectedSkin;
        }
    }
}