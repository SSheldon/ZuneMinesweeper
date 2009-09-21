using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public class OptionsMenuScreen : MenuScreen
    {
        MenuItem cantSelectRevealedMI, flagButton, changeSkin, useTouchMI, back;
        Options oldOptions;

        public OptionsMenuScreen(MinesweeperGame game)
            : base(game, "Options:")
        {
            oldOptions = new Options(Game.options.CantSelectRevealed, Game.options.FlagWithPlay, Game.options.UseTouch, Game.options.SelectedSkin);

            cantSelectRevealedMI = new MenuItem("Revealed tiles can't be selected", false, false, true);
            cantSelectRevealedMI.itemClicked += new ItemClick(CantSelectRevealedClick);
            Add(0, cantSelectRevealedMI);
            flagButton = new MenuItem("Flag tiles with Play button", true, true, true);
            flagButton.itemClicked += new ItemClick(FlagButtonClick);
            Add(1, flagButton);
            useTouchMI = new MenuItem("Touch control off", false, false, true);
            useTouchMI.itemClicked += new ItemClick(UseTouchClick);
            Add(2, useTouchMI);
            changeSkin = new MenuItem("Change Skin");
            changeSkin.itemClicked += new ItemClick(ChangeSkinClick);
            Add(3, changeSkin);
            back = new MenuItem("Back");
            back.itemClicked += new ItemClick(Back);
            Add(5, back);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            cantSelectRevealedMI.text = Game.options.CantSelectRevealed ? "Revealed tiles can't be selected" : "Revealed tiles can be selected";
            flagButton.text = Game.options.FlagWithPlay ? "Flag tiles with Play button" : "Flag tiles with Center button";
            useTouchMI.text = Game.options.UseTouch ? "Touch control on" : "Touch control off";

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        void CantSelectRevealedClick()
        {
            Game.options.CantSelectRevealed = !Game.options.CantSelectRevealed;
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
            if (a.CantSelectRevealed != b.CantSelectRevealed) equal = false;
            if (a.FlagWithPlay != b.FlagWithPlay) equal = false;
            if (a.UseTouch != b.UseTouch) equal = false;
            if (a.SelectedSkin != b.SelectedSkin) equal = false;
            return equal;
        }
    }
}