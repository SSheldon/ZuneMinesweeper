using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Minesweeper
{
    /// <summary>
    /// This component draws the entire background for the game.  It handles
    /// drawing the ground, clouds, sun, and moon.  also handles animating them
    /// and day/night transitions
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region FIELDS AND INITIALIZATION
        MinesweeperGame Game;
        Field field;
        public int height, width, mines;
        int flags, time;
        double totalTime;
        int[] selectedTile = new int[2]; //tile in column selectedTile[0] of row selectedTile[1]
        int[] selectedMine = new int[2];
        int[] corner = new int[2];
        enum Face { Happy, Win, Dead, Scared };
        Face faceValue;
        enum GameState { NotPlaying, Playing, Won, Lost };
        GameState gameState;
        public bool faceSelected;
        bool Movable
        {
            get { return gameState == GameState.Playing || gameState == GameState.NotPlaying; }
        }
        //int hZone, vZone;
        //TimeSpan lastUpdate, lastMove;

        public GameplayScreen(MinesweeperGame game)
        {
            this.Game = game;
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

            SetGame(9, 9, 10);
        }

        public void SetGame(int height, int width, int mines)
        {
            this.height = height;
            this.width = width;
            this.mines = mines;
            field = new Field(height, width, mines);
            flags = mines;
            gameState = GameState.NotPlaying;
            time = 0;
            totalTime = 0.0;
            faceValue = Face.Happy;
            selectedTile[0] = 0;
            selectedTile[1] = 0;
            corner[0] = 0;
            corner[1] = 0;
            faceSelected = false;
        }
        #endregion

        /// <summary>
        /// Runs one frame of update for the game.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (gameState == GameState.Playing)
            {
                totalTime += gameTime.ElapsedGameTime.TotalSeconds;
                time = Convert.ToInt32(totalTime);
            }
            if (time > 999) time = 999;

            if (height > 15)
            {
                if (selectedTile[1] - corner[1] > 10)
                {
                    corner[1] = selectedTile[1] - 10;
                    if (corner[1] + 14 > height - 1) corner[1] = height - 15;
                }
                else if (corner[1] + 4 > selectedTile[1])
                {
                    corner[1] = selectedTile[1] - 4;
                    if (corner[1] < 0) corner[1] = 0;
                }
            }
            if (width > 14)
            {
                if (selectedTile[0] - corner[0] > 9)
                {
                    corner[0] = selectedTile[0] - 9;
                    if (corner[0] + 13 > width - 1) corner[0] = width - 14;
                }
                else if (corner[0] + 4 > selectedTile[0])
                {
                    corner[0] = selectedTile[0] - 4;
                    if (corner[0] < 0) corner[0] = 0;
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Input helper method provided by GameScreen.  Packages up the various input
        /// values for ease of use.
        /// </summary>
        /// <param name="input">The state of the gamepads</param>
        public override void HandleInput(InputState input)
        {
            if (input.MenuCancel)
            {
                if (gameState == GameState.Playing) gameState = GameState.NotPlaying;
                ScreenManager.AddScreen(new MainMenuScreen(Game, true));
            }
            if (!Movable && input.MenuSelect)
            {
                SetGame(height, width, mines);
                faceSelected = true;
            }
            if (Movable)
            {
                if (!faceSelected)
                    if ((Game.options.FlagWithPlay && input.CurrentGamePadStates[0].Buttons.A == ButtonState.Pressed) ||
                        (!Game.options.FlagWithPlay && input.CurrentGamePadStates[0].Buttons.B == ButtonState.Pressed)) 
                        faceValue = Face.Scared;
                if (faceValue == Face.Scared)
                    if ((Game.options.FlagWithPlay && input.CurrentGamePadStates[0].Buttons.A != ButtonState.Pressed) ||
                        (!Game.options.FlagWithPlay && input.CurrentGamePadStates[0].Buttons.B != ButtonState.Pressed))
                        faceValue = Face.Happy;
                if (input.LastGamePadStates[0].Buttons.A == ButtonState.Pressed &&
                    input.CurrentGamePadStates[0].Buttons.A == ButtonState.Released)
                {
                    if (faceSelected) SetGame(height, width, mines);
                    else if (Game.options.FlagWithPlay)
                    {
                        if (field.tiles[selectedTile[1], selectedTile[0]].Hidden) TileClick();
                        else SurroundClick();
                    }
                    else TileFlag();
                }
                if (input.LastGamePadStates[0].Buttons.B == ButtonState.Pressed &&
                    input.CurrentGamePadStates[0].Buttons.B == ButtonState.Released && !faceSelected)
                {
                    if (Game.options.FlagWithPlay) TileFlag();
                    else
                    {
                        if (field.tiles[selectedTile[1], selectedTile[0]].Hidden) TileClick();
                        else SurroundClick();
                    }
                }

            }
            //DPAD Controls
            if (Movable)
            {
                if (input.MenuUp)
                {
                    if (faceSelected)
                    {
                        selectedTile[1] = height - 1;
                        faceSelected = false;
                    }
                    else if (selectedTile[1] == 0) faceSelected = true;
                    else selectedTile[1]--;
                }
                if (input.MenuDown)
                {
                    if (faceSelected)
                    {
                        selectedTile[1] = 0;
                        faceSelected = false;
                    }
                    else if (selectedTile[1] < height - 1) selectedTile[1]++;
                    else faceSelected = true;
                }
                if (input.IsNewButtonPress(Buttons.DPadLeft) && !faceSelected)
                {
                    if (selectedTile[0] == 0) selectedTile[0] = width - 1;
                    else selectedTile[0]--;
                }
                if (input.IsNewButtonPress(Buttons.DPadRight) && !faceSelected)
                {
                    if (selectedTile[0] < width - 1) selectedTile[0]++;
                    else selectedTile[0] = 0;
                }
            }
        }

        void TileClick()
        {
            if (field.AllHidden) field.MoveMine(selectedTile[1], selectedTile[0]);
            if (gameState != GameState.Playing) gameState = GameState.Playing;
            if (field.Click(selectedTile[1], selectedTile[0])) //Game over
            {
                gameState = GameState.Lost;
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (field.tiles[row, col].Mined == true) field.tiles[row, col].Reveal();
                    }
                }
                selectedMine = selectedTile;
                faceValue = Face.Dead;
                faceSelected = true;
            }
            else
            {
                if (field.AllUnminedRevealed) //Game won
                {
                    gameState = GameState.Won;
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            if (field.tiles[row, col].Mined == true) field.tiles[row, col].Flag();
                        }
                    }
                    flags = 0;
                    faceValue = Face.Win;
                    faceSelected = true;
                    if (height == 9 & width == 9 & mines == 10 & time < Game.bestTimes[Difficulty.Beginner])
                    {
                        Game.bestTimes[Difficulty.Beginner] = time;
                        Game.UpdateBestTime(Difficulty.Beginner);
                    }
                    if (height == 16 & width == 16 & mines == 40 & time < Game.bestTimes[Difficulty.Intermediate])
                    {
                        Game.bestTimes[Difficulty.Intermediate] = time;
                        Game.UpdateBestTime(Difficulty.Intermediate);
                    }
                    if (height == 24 & width == 30 & mines == 99 & time < Game.bestTimes[Difficulty.Expert])
                    {
                        Game.bestTimes[Difficulty.Expert] = time;
                        Game.UpdateBestTime(Difficulty.Expert);
                    }
                    if (height == 15 & width == 14 & mines == 30 & time < Game.bestTimes[Difficulty.Zune])
                    {
                        Game.bestTimes[Difficulty.Zune] = time;
                        Game.UpdateBestTime(Difficulty.Zune);
                    }
                }
                else //Game continues
                {
                    gameState = GameState.Playing;
                    faceValue = Face.Happy;
                }
            }
        }

        void TileFlag()
        {
            if (field.tiles[selectedTile[1], selectedTile[0]].Hidden)
            {
                if (!(field.tiles[selectedTile[1], selectedTile[0]].Flagged))
                {
                    field.tiles[selectedTile[1], selectedTile[0]].Flag();
                    flags--;
                }
                else
                {
                    field.tiles[selectedTile[1], selectedTile[0]].Unflag();
                    flags++;
                }
            }
        }

        int FlagsSurroundingSelected
        {
            get
            {
                int surroundingFlags = 0;

                if (!(selectedTile[1] == 0))
                    if (field.tiles[(selectedTile[1] - 1), selectedTile[0]].Flagged) surroundingFlags++;
                if (!(selectedTile[0] == 0))
                    if (field.tiles[selectedTile[1], (selectedTile[0] - 1)].Flagged) surroundingFlags++;
                if (!(selectedTile[1] == 0) & !(selectedTile[0] == 0))
                    if (field.tiles[(selectedTile[1] - 1), (selectedTile[0] - 1)].Flagged) surroundingFlags++;
                if (!(selectedTile[0] == width - 1))
                    if (field.tiles[selectedTile[1], (selectedTile[0] + 1)].Flagged) surroundingFlags++;
                if (!(selectedTile[1] == 0) & !(selectedTile[0] == width - 1))
                    if (field.tiles[(selectedTile[1] - 1), (selectedTile[0] + 1)].Flagged) surroundingFlags++;
                if (!(selectedTile[1] == height - 1))
                    if (field.tiles[(selectedTile[1] + 1), selectedTile[0]].Flagged) surroundingFlags++;
                if (!(selectedTile[1] == height - 1) & !(selectedTile[0] == 0))
                    if (field.tiles[(selectedTile[1] + 1), (selectedTile[0] - 1)].Flagged) surroundingFlags++;
                if (!(selectedTile[1] == height - 1) & !(selectedTile[0] == width - 1))
                    if (field.tiles[(selectedTile[1] + 1), (selectedTile[0] + 1)].Flagged) surroundingFlags++;

                return surroundingFlags;
            }
        }

        void SurroundClick()
        {
            if (FlagsSurroundingSelected == field.tiles[selectedTile[1], selectedTile[0]].Number)
            {
                int[] originalSelectedTile = new int[2];
                originalSelectedTile[1] = selectedTile[1];
                originalSelectedTile[0] = selectedTile[0];

                if (!(originalSelectedTile[1] == 0))
                {
                    selectedTile[1] = originalSelectedTile[1] - 1;
                    selectedTile[0] = originalSelectedTile[0];
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelectedTile[0] == 0))
                {
                    selectedTile[1] = originalSelectedTile[1];
                    selectedTile[0] = originalSelectedTile[0] - 1;
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelectedTile[1] == 0) & !(originalSelectedTile[0] == 0))
                {
                    selectedTile[1] = originalSelectedTile[1] - 1;
                    selectedTile[0] = originalSelectedTile[0] - 1;
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelectedTile[0] == width - 1))
                {
                    selectedTile[1] = originalSelectedTile[1];
                    selectedTile[0] = originalSelectedTile[0] + 1;
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelectedTile[1] == 0) & !(originalSelectedTile[0] == width - 1))
                {
                    selectedTile[1] = originalSelectedTile[1] - 1;
                    selectedTile[0] = originalSelectedTile[0] + 1;
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelectedTile[1] == height - 1))
                {
                    selectedTile[1] = originalSelectedTile[1] + 1;
                    selectedTile[0] = originalSelectedTile[0];
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelectedTile[1] == height - 1) & !(originalSelectedTile[0] == 0))
                {
                    selectedTile[1] = originalSelectedTile[1] + 1;
                    selectedTile[0] = originalSelectedTile[0] - 1;
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelectedTile[1] == height - 1) & !(originalSelectedTile[0] == width - 1))
                {
                    selectedTile[1] = originalSelectedTile[1] + 1;
                    selectedTile[0] = originalSelectedTile[0] + 1;
                    TileClick();
                }
                if (gameState != GameState.Playing) return;

                selectedTile[1] = originalSelectedTile[1];
                selectedTile[0] = originalSelectedTile[0];
            }
            else
            {
                faceValue = Face.Happy;
            }
        }

        #region DRAWING
        /// <summary>
        /// Draw the game world, effects, and hud
        /// </summary>
        /// <param name="gameTime">The elapsed time since last Draw</param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Game.Skin.background);

            ScreenManager.SpriteBatch.Begin();
            DrawFieldBorder(ScreenManager.SpriteBatch);
            DrawField(ScreenManager.SpriteBatch);
            DrawBackground(ScreenManager.SpriteBatch);
            DrawNumbers(ScreenManager.SpriteBatch, flags, 16, 16);
            DrawNumbers(ScreenManager.SpriteBatch, time, 185, 16);
            DrawFace(ScreenManager.SpriteBatch);
            DrawTileSelect(ScreenManager.SpriteBatch);
            ScreenManager.SpriteBatch.End();
        }

        void DrawBackground(SpriteBatch batch)
        {
            batch.Draw(Game.Skin.top, new Rectangle(0, 0, 240, 56), Color.White);
        }

        public void DrawNumbers(SpriteBatch batch, int amount, int x, int y)
        {
            int[] amountNums = new int[3];
            if (amount >= 0)
            {
                if (amount > 999) amount = 999;
                amountNums[0] = (amount - (amount % 100)) / 100;
                amountNums[1] = ((amount - amountNums[0] * 100) - ((amount - amountNums[0] * 100) % 10)) / 10;
                amountNums[2] = amount - (amountNums[0] * 100) - (amountNums[1] * 10);
                batch.Draw(Game.Skin.numbers[amountNums[0]], new Rectangle(x, y, 13, 23), Color.White);
                batch.Draw(Game.Skin.numbers[amountNums[1]], new Rectangle(x + 13, y, 13, 23), Color.White);
                batch.Draw(Game.Skin.numbers[amountNums[2]], new Rectangle(x + 26, y, 13, 23), Color.White);
            }
            else
            {
                char[] amountParts = new char[10];
                amountParts = amount.ToString().ToCharArray();
                if (amount < 0 & amount > -10) amountNums[1] = 0;
                else amountNums[1] = int.Parse(amountParts[amountParts.GetUpperBound(0) - 1].ToString());
                amountNums[2] = int.Parse(amountParts[amountParts.GetUpperBound(0)].ToString());
                if (amount < 0 & amount > -10) amountNums[1] = 0;
                batch.Draw(Game.Skin.numbers[10], new Rectangle(x, y, 13, 23), Color.White);
                batch.Draw(Game.Skin.numbers[amountNums[1]], new Rectangle(x + 13, y, 13, 23), Color.White);
                batch.Draw(Game.Skin.numbers[amountNums[2]], new Rectangle(x + 26, y, 13, 23), Color.White);
            }
        }

        void DrawFace(SpriteBatch batch)
        {
            if (faceValue == Face.Happy) batch.Draw(Game.Skin.fHappy, new Rectangle(108, 16, 24, 24), Color.White);
            else if (faceValue == Face.Win) batch.Draw(Game.Skin.fWin, new Rectangle(108, 16, 24, 24), Color.White);
            else if (faceValue == Face.Dead) batch.Draw(Game.Skin.fDead, new Rectangle(108, 16, 24, 24), Color.White);
            else batch.Draw(Game.Skin.fScared, new Rectangle(108, 16, 24, 24), Color.White);
            if (faceSelected) batch.Draw(Game.Skin.faceSelect, new Rectangle(108, 16, 24, 24), Color.White);
        }

        void DrawFieldBorder(SpriteBatch batch)
        {
            batch.Draw(Game.Skin.borderTL, new Rectangle(0 - corner[0] * 16, 56 - corner[1] * 16, 8, 8), Color.White);
            batch.Draw(Game.Skin.borderT, new Rectangle(8 - corner[0] * 16, 56 - corner[1] * 16, 16 * width, 8), Color.White);
            batch.Draw(Game.Skin.borderTR, new Rectangle(8 + 16 * width - corner[0] * 16, 56 - corner[1] * 16, 8, 8), Color.White);
            batch.Draw(Game.Skin.borderL, new Rectangle(0 - corner[0] * 16, 64 - corner[1] * 16, 8, 16 * height), Color.White);
            batch.Draw(Game.Skin.borderR, new Rectangle(8 + 16 * width - corner[0] * 16, 64 - corner[1] * 16, 8, 16 * height), Color.White);
            batch.Draw(Game.Skin.borderBL, new Rectangle(0 - corner[0] * 16, 64 + 16 * height - corner[1] * 16, 8, 8), Color.White);
            batch.Draw(Game.Skin.borderB, new Rectangle(8 - corner[0] * 16, 64 + 16 * height - corner[1] * 16, 16 * width, 8), Color.White);
            batch.Draw(Game.Skin.borderBR, new Rectangle(8 + 16 * width - corner[0] * 16, 64 + 16 * height - corner[1] * 16, 8, 8), Color.White);
        }

        void DrawTileSelect(SpriteBatch batch)
        {
            if (!faceSelected) batch.Draw(Game.Skin.select, new Rectangle(8 + selectedTile[0] * 16 - corner[0] * 16, 64 + selectedTile[1] * 16 - corner[1] * 16, 16, 16), Color.White);
        }

        void DrawField(SpriteBatch batch)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Texture2D tile;
                    if (gameState == GameState.Playing || gameState == GameState.NotPlaying)
                    {
                        if (field.tiles[row, col].Flagged) tile = Game.Skin.tFlag;
                        else if (field.tiles[row, col].Hidden) tile = Game.Skin.tHidden;
                        else tile = Game.Skin.t[field.tiles[row, col].Number];
                    }
                    else
                    {
                        if (field.tiles[row, col].Flagged & !field.tiles[row, col].Mined) tile = Game.Skin.tNotMine;
                        else if (field.tiles[row, col].Flagged) tile = Game.Skin.tFlag;
                        else if (field.tiles[row, col].Hidden) tile = Game.Skin.tHidden;
                        else if (row == selectedMine[1] && col == selectedMine[0] && gameState == GameState.Lost) tile = Game.Skin.tClickedMine;
                        else if (field.tiles[row, col].Mined) tile = Game.Skin.tMine;
                        else tile = Game.Skin.t[field.tiles[row, col].Number];
                    }
                    batch.Draw(tile, new Rectangle(8 + col * 16 - corner[0] * 16, 64 + row * 16 - corner[1] * 16, 16, 16), Color.White);
                }
            }
        }
        #endregion
    }
}