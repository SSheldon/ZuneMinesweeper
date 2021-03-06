﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public class BestTimesMenuScreen : MenuScreen
    {
        MenuItem bestBeginnerMI, bestIntermediateMI, bestExpertMI, bestZuneMI, resetMI, back;

        public BestTimesMenuScreen(MinesweeperGame game)
            : base(game, "Best Times:")
        {
            bestBeginnerMI = new MenuItem("Beginner:", false, true);
            bestBeginnerMI.backed = false;
            Add(0, bestBeginnerMI);
            bestIntermediateMI = new MenuItem("Intermed.:", false, true);
            bestIntermediateMI.backed = false;
            Add(1, bestIntermediateMI);
            bestExpertMI = new MenuItem("Expert:", false, true);
            bestExpertMI.backed = false;
            Add(2, bestExpertMI);
            bestZuneMI = new MenuItem("Zune Fit:", false, true);
            bestZuneMI.backed = false;
            Add(3, bestZuneMI);
            resetMI = new MenuItem("Reset Times");
            resetMI.Clicked += new ItemClick(ResetClick);
            Add(4, resetMI);
            back = new MenuItem("Back");
            back.Clicked += new ItemClick(Back);
            Add(5, back);
        }

        protected override void DrawSelect(SpriteBatch batch)
        {
            MinesweeperGame.DrawNumbers(batch, Game.Skin, Game.bestTimes[Difficulty.Beginner], 178, 84);
            MinesweeperGame.DrawNumbers(batch, Game.Skin, Game.bestTimes[Difficulty.Intermediate], 178, 124);
            MinesweeperGame.DrawNumbers(batch, Game.Skin, Game.bestTimes[Difficulty.Expert], 178, 164);
            MinesweeperGame.DrawNumbers(batch, Game.Skin, Game.bestTimes[Difficulty.Zune], 178, 204);
            batch.Draw(Game.Skin.mSelect, new Vector2(14, 78 + selectedItem * 40), Color.White);
        }

        void ResetClick()
        {
            Game.bestTimes[Difficulty.Beginner] = 999;
            Game.UpdateBestTime(Difficulty.Beginner);
            Game.bestTimes[Difficulty.Intermediate] = 999;
            Game.UpdateBestTime(Difficulty.Intermediate);
            Game.bestTimes[Difficulty.Expert] = 999;
            Game.UpdateBestTime(Difficulty.Expert);
            Game.bestTimes[Difficulty.Zune] = 999;
            Game.UpdateBestTime(Difficulty.Zune);
        }
    }
}