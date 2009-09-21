using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public class MainMenuScreen : MenuScreen
    {
        MenuItem resume, newGame, music, bestTimes, options, exit;

        public MainMenuScreen(MinesweeperGame game, bool resumable)
            : base(game)
        {
            resume = new MenuItem("Resume", resumable);
            resume.itemClicked += new ItemClick(Back);
            Add(0, resume);
            newGame = new MenuItem("New Game");
            newGame.itemClicked += new ItemClick(NewGameClick);
            Add(1, newGame);
            music = new MenuItem("Music");
            music.itemClicked += new ItemClick(MusicClick);
            Add(2, music);
            bestTimes = new MenuItem("Best Times");
            bestTimes.itemClicked += new ItemClick(BestTimesClick);
            Add(3, bestTimes);
            options = new MenuItem("Options");
            options.itemClicked += new ItemClick(OptionsClick);
            Add(4, options);
            exit = new MenuItem("Exit");
            exit.itemClicked += new ItemClick(Game.Exit);
            Add(5, exit);
        }

        protected override void DrawHeader(SpriteBatch batch)
        {
            batch.Draw(Game.Skin.mTop, new Vector2(0, 0), Color.White);
        }

        void MusicClick()
        {
            Guide.Show();
        }

        void NewGameClick()
        {
            ScreenManager.AddScreen(new NewGameMenuScreen(Game));
        }

        void OptionsClick()
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(Game));
        }

        void BestTimesClick()
        {
            ScreenManager.AddScreen(new BestTimesMenuScreen(Game));
        }

        protected override void Back()
        {
            if (resume.selectable) base.Back();
        }
    }
}