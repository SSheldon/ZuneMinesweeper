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
            beginner.itemClicked += new ItemClick(BeginnerClick);
            Add(0, beginner);
            intermediate = new MenuItem("Intermediate");
            intermediate.itemClicked += new ItemClick(IntermediateClick);
            Add(1, intermediate);
            expert = new MenuItem("Expert");
            expert.itemClicked += new ItemClick(ExpertClick);
            Add(2, expert);
            zune = new MenuItem("Zune Fit");
            zune.itemClicked += new ItemClick(ZuneClick);
            Add(3, zune);
            custom = new MenuItem("Custom");
            custom.itemClicked += new ItemClick(CustomClick);
            Add(4, custom);
            back = new MenuItem("Back");
            back.itemClicked += new ItemClick(Back);
            Add(5, back);
        }

        void BeginnerClick()
        {
            NewGame(9, 9, 10);
        }

        void IntermediateClick()
        {
            NewGame(16, 16, 40);
        }

        void ExpertClick()
        {
            NewGame(24, 30, 99);
        }

        void ZuneClick()
        {
            NewGame(15, 14, 30);
        }

        void CustomClick()
        {
            GameScreen[] screens = ScreenManager.GetScreens();
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i] is GameplayScreen)
                {
                    ScreenManager.AddScreen(new CustomGameMenuScreen(Game, 
                        (screens[i] as GameplayScreen).Height,
                        (screens[i] as GameplayScreen).Width,
                        (screens[i] as GameplayScreen).Mines));
                    return;
                }
            }
            ScreenManager.AddScreen(new CustomGameMenuScreen(Game, 9, 9, 10));
        }
    }
}