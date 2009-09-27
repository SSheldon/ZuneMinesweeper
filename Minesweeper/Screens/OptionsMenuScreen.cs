using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public class OptionsMenuScreen : MenuScreen
    {
        MenuItem flagButton, changeSkin, useTouchMI, back;
        Options oldOptions;

        public OptionsMenuScreen(MinesweeperGame game)
            : base(game, "Options:")
        {
            oldOptions = new Options(Game.options.FlagWithPlay, Game.options.UseTouch, Game.options.SelectedSkin);

            flagButton = new MenuItem("Flag tiles with Play button", true, true, true);
            flagButton.Clicked += new ItemClick(FlagButtonClick);
            Add(0, flagButton);
            useTouchMI = new MenuItem("Touch control off", false, false, true);
            useTouchMI.Clicked += new ItemClick(UseTouchClick);
            Add(1, useTouchMI);
            changeSkin = new MenuItem("Change Skin");
            changeSkin.Clicked += new ItemClick(ChangeSkinClick);
            Add(2, changeSkin);
            back = new MenuItem("Back");
            back.Clicked += new ItemClick(Back);
            Add(5, back);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            flagButton.text = Game.options.FlagWithPlay ? "Flag tiles with Play button" : "Flag tiles with Center button";
            useTouchMI.text = Game.options.UseTouch ? "Touch control on" : "Touch control off";

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        void FlagButtonClick()
        {
            Game.options.FlagWithPlay = !Game.options.FlagWithPlay;
        }

        void UseTouchClick()
        {
            Game.options.UseTouch = !Game.options.UseTouch;
        }

        void ChangeSkinClick()
        {
            ScreenManager.AddScreen(new SkinMenuScreen(Game));
        }

        protected override void Back()
        {
            if (!OptionsEqual(oldOptions, Game.options)) Game.UpdateOptions();
            base.Back();
        }

        private bool OptionsEqual(Options a, Options b)
        {
            bool equal = true;
            if (a.FlagWithPlay != b.FlagWithPlay) equal = false;
            if (a.UseTouch != b.UseTouch) equal = false;
            if (a.SelectedSkin != b.SelectedSkin) equal = false;
            return equal;
        }
    }
}