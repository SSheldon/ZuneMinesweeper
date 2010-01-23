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
            resume.Clicked += new ItemClick(Back);
            Add(0, resume);
            newGame = new MenuItem("New Game");
            newGame.Clicked += () => ScreenManager.AddScreen(new NewGameMenuScreen(Game));
            Add(1, newGame);
            music = new MenuItem("Music");
            music.Clicked += new ItemClick(Guide.Show);
            Add(2, music);
            bestTimes = new MenuItem("Best Times");
            bestTimes.Clicked += () => ScreenManager.AddScreen(new BestTimesMenuScreen(Game));
            Add(3, bestTimes);
            options = new MenuItem("Options");
            options.Clicked += () => ScreenManager.AddScreen(new OptionsMenuScreen(Game));
            Add(4, options);
            exit = new MenuItem("Exit");
            exit.Clicked += new ItemClick(Game.Exit);
            Add(5, exit);
        }

        protected override void DrawHeader(SpriteBatch batch)
        {
            batch.Draw(Game.Skin.mTop, new Vector2(0, 0), Color.White);
        }

        protected override void Back()
        {
            if (resume.selectable) base.Back();
        }
    }
}