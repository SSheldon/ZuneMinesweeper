using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public abstract class MenuScreen : GameScreen
    {
        protected string title;
        protected int selectedItem = 0;
        protected MenuItem[] items = new MenuItem[6];
        protected MinesweeperGame Game;

        public MenuScreen(MinesweeperGame game, string title)
            : this(game)
        {
            this.title = title;           
        }

        public MenuScreen(MinesweeperGame game)
        {
            this.Game = game;
        }

        protected void Add(int position, MenuItem item)
        {
            items[position] = item;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            while (items[selectedItem] == null || !items[selectedItem].selectable)
            {
                if (selectedItem == 5) selectedItem = 0;
                else selectedItem++;
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(InputState input)
        {
            if (input.MenuUp) UpClick();
            if (input.MenuDown) DownClick();
            if (input.MenuSelect) ClickItem();
            if (input.MenuCancel) Back();
            if (input.IsNewButtonPress(Buttons.DPadLeft)) LeftClick();
            if (input.IsNewButtonPress(Buttons.DPadRight)) RightClick();
        }

        protected void DownClick()
        {
            if (selectedItem == 5) selectedItem = 0;
            else selectedItem++;
            while (items[selectedItem] == null || !items[selectedItem].selectable)
            {
                if (selectedItem == 5) selectedItem = 0;
                else selectedItem++;
            }
        }

        protected void UpClick()
        {
            if (selectedItem == 0) selectedItem = 5;
            else selectedItem--;
            while (items[selectedItem] == null || !items[selectedItem].selectable)
            {
                if (selectedItem == 0) selectedItem = 5;
                else selectedItem--;
            }
        }

        protected virtual void RightClick() { }

        protected virtual void LeftClick() { }

        protected virtual void Back()
        {
            ExitScreen();
        }

        protected void ClickItem()
        {
            items[selectedItem].OnClick();
        }

        protected void NewGame(int height, int width, int mines)
        {
            GameScreen[] screens = ScreenManager.GetScreens();
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i] is GameplayScreen)
                {
                    (screens[i] as GameplayScreen).SetGame(height, width, mines);
                    (screens[i] as GameplayScreen).SelectFace();
                    break;
                }
            }
            Game.ExitAllMenuScreens();
        }

        public override void Draw(GameTime gameTime)
        {
            DrawMenu(gameTime);
        }

        private void DrawMenu(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Game.Skin.background);
            ScreenManager.SpriteBatch.Begin();
            DrawHeader(ScreenManager.SpriteBatch);
            DrawItems(ScreenManager.SpriteBatch);
            DrawSelect(ScreenManager.SpriteBatch);
            ScreenManager.SpriteBatch.End();
        }

        protected virtual void DrawHeader(SpriteBatch batch)
        {
            if (Game.Skin.headersBacked) batch.Draw(Game.Skin.headerBack, new Vector2(0, 0), Color.White);
            batch.DrawString(Game.Skin.header, title, new Vector2(4, 16), Game.Skin.coloredText);
        }

        protected virtual void DrawItems(SpriteBatch batch)
        {
            for (int counter = 0; counter < 6; counter++)
            {
                if (items[counter] != null)
                {
                    if (Game.Skin.itemsBacked && items[counter].backed)
                        batch.Draw(Game.Skin.itemBack, new Vector2(14, 78 + counter * 40), Color.White);
                    Color fontColor = items[counter].colored ? Game.Skin.coloredText : Game.Skin.uncoloredText; ;
                    SpriteFont font = items[counter].smallFont ? Game.Skin.small : Game.Skin.normal;
                    int y = items[counter].smallFont ? 12 : 0;
                    batch.DrawString(font, items[counter].text, new Vector2(16, 72 + counter * 40 + y), fontColor);
                }
            }
        }

        protected virtual void DrawSelect(SpriteBatch batch)
        {
            batch.Draw(Game.Skin.mSelect, new Vector2(14, 78 + selectedItem * 40), Color.White);
        }
    }
}