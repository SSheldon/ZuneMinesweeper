using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public class CustomGameMenuScreen : MenuScreen
    {
        MenuItem heightMI, widthMI, minesMI, ok, back;
        int tempHeight, tempWidth, tempMines;

        public CustomGameMenuScreen(MinesweeperGame game)
            : base(game, "Custom Game:")
        {
            heightMI = new MenuItem("Height");
            heightMI.backed = false;
            Add(0, heightMI);
            widthMI = new MenuItem("Width");
            widthMI.backed = false;
            Add(1, widthMI);
            minesMI = new MenuItem("Mines");
            minesMI.backed = false;
            Add(2, minesMI);
            ok = new MenuItem("OK");
            ok.itemClicked += new ItemClick(OKClick);
            Add(4, ok);
            back = new MenuItem("Back");
            back.itemClicked += new ItemClick(Back);
            Add(5, back);

            tempHeight = Game.gameplayScreen.height;
            tempWidth = Game.gameplayScreen.width;
            tempMines = Game.gameplayScreen.mines;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (tempMines > (tempHeight - 1) * (tempWidth - 1)) 
                tempMines = (tempHeight - 1) * (tempWidth - 1);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        protected override void DrawSelect(SpriteBatch batch)
        {
            batch.Draw(Game.Skin.numBox, new Vector2(162, 76), Color.White);
            MinesweeperGame.DrawNumbers(batch, Game.Skin.numbers, tempHeight, 170, 84);
            batch.Draw(Game.Skin.numBox, new Vector2(162, 116), Color.White);
            MinesweeperGame.DrawNumbers(batch, Game.Skin.numbers, tempWidth, 170, 124);
            batch.Draw(Game.Skin.numBox, new Vector2(162, 156), Color.White);
            MinesweeperGame.DrawNumbers(batch, Game.Skin.numbers, tempMines, 170, 164);
            if (selectedItem == 0 || selectedItem == 1 || selectedItem == 2)
            {
                switch (selectedItem)
                {
                    case 0:
                        if (tempHeight > 9) batch.Draw(Game.Skin.leftArrow, new Vector2(156, 85), Color.White);
                        if (tempHeight < 24) batch.Draw(Game.Skin.rightArrow, new Vector2(212, 85), Color.White);
                        break;
                    case 1:
                        if (tempWidth > 9) batch.Draw(Game.Skin.leftArrow, new Vector2(156, 125), Color.White);
                        if (tempWidth < 30) batch.Draw(Game.Skin.rightArrow, new Vector2(212, 125), Color.White);
                        break;
                    case 2:
                        if (tempMines > 10) batch.Draw(Game.Skin.leftArrow, new Vector2(156, 165), Color.White);
                        if (tempMines < (tempHeight - 1) * (tempWidth - 1)) batch.Draw(Game.Skin.rightArrow, new Vector2(212, 165), Color.White);
                        break;
                }
            }
            else batch.Draw(Game.Skin.mSelect, new Vector2(14, 78 + selectedItem * 40), Color.White);
        }

        protected override void RightClick()
        {
            
            switch (selectedItem)
            {
                case 0:
                    tempHeight++;
                    if (tempHeight > 24) tempHeight = 9;
                    break;
                case 1:
                    tempWidth++;
                    if (tempWidth > 30) tempWidth = 9;
                    break;
                case 2:
                    tempMines++;
                    if (tempMines > (tempHeight - 1) * (tempWidth - 1)) tempMines = 10;
                    break;
            }
        }

        protected override void LeftClick()
        {            
            switch (selectedItem)
            {
                case 0:
                    tempHeight--;
                    if (tempHeight < 9) tempHeight = 24;
                    break;
                case 1:
                    tempWidth--;
                    if (tempWidth < 9) tempWidth = 30;
                    break;
                case 2:
                    tempMines--;
                    if (tempMines < 10) tempMines = (tempHeight - 1) * (tempWidth - 1);
                    break;
            }
        }


        void OKClick()
        {
            Game.gameplayScreen.SetGame(tempHeight, tempWidth, tempMines);
            Game.gameplayScreen.faceSelected = true;
            GameScreen[] screens = ScreenManager.GetScreens();
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i] is MainMenuScreen)
                {
                    screens[i].ExitScreen();
                    break;
                }
            }
            screens = ScreenManager.GetScreens();
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i] is NewGameMenuScreen)
                {
                    screens[i].ExitScreen();
                    break;
                }
            }
            ExitScreen();
        }
    }
}