using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public class NewGameMenuScreen : MenuScreen
    {
        MenuItem beginner, intermediate, expert, zune, custom, back;

        public NewGameMenuScreen(MinesweeperGame game)
            : base(game, "New Game:")
        {
            beginner = new MenuItem("Beginner");
            beginner.Clicked += () => NewGame(9, 9, 10);
            Add(0, beginner);
            intermediate = new MenuItem("Intermediate");
            intermediate.Clicked += () => NewGame(16, 16, 40);
            Add(1, intermediate);
            expert = new MenuItem("Expert");
            expert.Clicked += () => NewGame(24, 30, 99);
            Add(2, expert);
            zune = new MenuItem("Zune Fit");
            zune.Clicked += () => NewGame(15, 14, 30);
            Add(3, zune);
            custom = new MenuItem("Custom");
            custom.Clicked += () => ScreenManager.AddScreen(new CustomGameMenuScreen(Game));
            Add(4, custom);
            back = new MenuItem("Back");
            back.Clicked += new ItemClick(Back);
            Add(5, back);
        }
    }
}