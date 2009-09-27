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
    public struct FieldLocation
    {
        public int row, col;

        public FieldLocation(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    public class GameplayScreen : GameScreen
    {
        #region FIELDS AND INITIALIZATION
        MinesweeperGame Game;
        Field field;
        int flags, time;
        double totalTime;
        FieldLocation selected, selectedMine, corner;
        enum Face { Happy, Win, Dead, Scared };
        Face faceValue;
        enum GameState { NotPlaying, Playing, Won, Lost };
        GameState gameState;
        bool faceSelected;
        bool Movable
        {
            get { return gameState == GameState.Playing || gameState == GameState.NotPlaying; }
        }
        public int Height
        {
            get { return field.Height; }
        }
        public int Width
        {
            get { return field.Width; }
        }
        public int Mines
        {
            get { return field.Mines; }
        }

        public GameplayScreen(MinesweeperGame game)
        {
            this.Game = game;
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

            SetGame(9, 9, 10);
        }

        public void SetGame(int height, int width, int mines)
        {
            field = new Field(height, width, mines);
            flags = Mines;
            gameState = GameState.NotPlaying;
            time = 0;
            totalTime = 0.0;
            faceValue = Face.Happy;
            selected = new FieldLocation(0, 0);
            corner = new FieldLocation(0, 0);
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

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        void UpdateCorner()
        {
            if (Height > 15)
            {
                if (selected.row - corner.row > 10)
                {
                    corner.row = selected.row - 10;
                    if (corner.row + 14 > Height - 1) corner.row = Height - 15;
                }
                else if (corner.row + 4 > selected.row)
                {
                    corner.row = selected.row - 4;
                    if (corner.row < 0) corner.row = 0;
                }
            }
            if (Width > 14)
            {
                if (selected.col - corner.col > 9)
                {
                    corner.col = selected.col - 9;
                    if (corner.col + 13 > Width - 1) corner.col = Width - 14;
                }
                else if (corner.col + 4 > selected.col)
                {
                    corner.col = selected.col - 4;
                    if (corner.col < 0) corner.col = 0;
                }
            }
        }

        /// <summary>
        /// Input helper method provided by GameScreen.  Packages up the various input
        /// values for ease of use.
        /// </summary>
        /// <param name="input">The state of the gamepads</param>
        public override void HandleInput(InputState input)
        {
            if (input.IsNewButtonPress(Buttons.Back)) Back();
            if (Movable)
            {
                #region DPAD CONTROLS
                if (input.IsNewButtonTick(Buttons.DPadUp))
                {
                    if (faceSelected)
                    {
                        selected.row = Height - 1;
                        faceSelected = false;
                    }
                    else if (selected.row == 0) faceSelected = true;
                    else selected.row--;
                }
                if (input.IsNewButtonTick(Buttons.DPadDown))
                {
                    if (faceSelected)
                    {
                        selected.row = 0;
                        faceSelected = false;
                    }
                    else if (selected.row < Height - 1) selected.row++;
                    else faceSelected = true;
                }
                if (input.IsNewButtonTick(Buttons.DPadLeft) && !faceSelected)
                {
                    if (selected.col == 0) selected.col = Width - 1;
                    else selected.col--;
                }
                if (input.IsNewButtonTick(Buttons.DPadRight) && !faceSelected)
                {
                    if (selected.col < Width - 1) selected.col++;
                    else selected.col = 0;
                }
                #endregion
                if (!faceSelected)
                    if ((Game.options.FlagWithPlay && input.IsButtonPressed(Buttons.A)) ||
                        (!Game.options.FlagWithPlay && input.IsButtonPressed(Buttons.B))) 
                        faceValue = Face.Scared;
                if (faceValue == Face.Scared)
                    if ((Game.options.FlagWithPlay && !input.IsButtonPressed(Buttons.A)) ||
                        (!Game.options.FlagWithPlay && !input.IsButtonPressed(Buttons.B)))
                        faceValue = Face.Happy;
                if (input.IsNewButtonRelease(Buttons.A))
                {
                    if (faceSelected) SetGame(Height, Width, Mines);
                    else if (Game.options.FlagWithPlay)
                    {
                        if (field[selected.row, selected.col].Hidden) TileClick();
                        else SurroundClick();
                    }
                    else TileFlag();
                }
                if (input.IsNewButtonRelease(Buttons.B) && !faceSelected)
                {
                    if (Game.options.FlagWithPlay) TileFlag();
                    else
                    {
                        if (field[selected.row, selected.col].Hidden) TileClick();
                        else SurroundClick();
                    }
                }
            }
            else if (input.IsNewButtonRelease(Buttons.A)) SetGame(Height, Width, Mines);
        }

        public void Back()
        {
            if (gameState == GameState.Playing) gameState = GameState.NotPlaying;
            ScreenManager.AddScreen(new MainMenuScreen(Game, true));
        }

        void TileClick()
        {
            if (field.AllHidden) field.MoveMine(selected.row, selected.col);
            if (gameState != GameState.Playing) gameState = GameState.Playing;
            if (field.Click(selected.row, selected.col)) //Game over
            {
                gameState = GameState.Lost;
                for (int row = 0; row < Height; row++)
                {
                    for (int col = 0; col < Width; col++)
                    {
                        if (field[row, col].Mined) field.Reveal(row, col);
                    }
                }
                selectedMine = selected;
                faceValue = Face.Dead;
                faceSelected = true;
            }
            else
            {
                if (field.AllUnminedRevealed) //Game won
                {
                    gameState = GameState.Won;
                    for (int row = 0; row < Height; row++)
                    {
                        for (int col = 0; col < Width; col++)
                        {
                            if (field[row, col].Mined) field.Flag(row, col);
                        }
                    }
                    flags = 0;
                    faceValue = Face.Win;
                    faceSelected = true;
                    if (Height == 9 && Width == 9 && Mines == 10 && time < Game.bestTimes[Difficulty.Beginner])
                    {
                        Game.bestTimes[Difficulty.Beginner] = time;
                        Game.UpdateBestTime(Difficulty.Beginner);
                    }
                    if (Height == 16 && Width == 16 && Mines == 40 && time < Game.bestTimes[Difficulty.Intermediate])
                    {
                        Game.bestTimes[Difficulty.Intermediate] = time;
                        Game.UpdateBestTime(Difficulty.Intermediate);
                    }
                    if (Height == 24 && Width == 30 && Mines == 99 && time < Game.bestTimes[Difficulty.Expert])
                    {
                        Game.bestTimes[Difficulty.Expert] = time;
                        Game.UpdateBestTime(Difficulty.Expert);
                    }
                    if (Height == 15 && Width == 14 && Mines == 30 && time < Game.bestTimes[Difficulty.Zune])
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
            if (field[selected.row, selected.col].Hidden)
            {
                if (!field[selected.row, selected.col].Flagged)
                {
                    field.Flag(selected.row, selected.col);
                    flags--;
                }
                else
                {
                    field.Unflag(selected.row, selected.col);
                    flags++;
                }
            }
        }

        void SurroundClick()
        {
            if (field.FlagsTouching(selected.row, selected.col) == field[selected.row, selected.col].Number)
            {
                FieldLocation originalSelected = selected;

                if (!(originalSelected.row == 0))
                {
                    selected = new FieldLocation(originalSelected.row - 1, originalSelected.col);
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelected.col == 0))
                {
                    selected = new FieldLocation(originalSelected.row, originalSelected.col - 1);
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelected.row == 0) & !(originalSelected.col == 0))
                {
                    selected = new FieldLocation(originalSelected.row - 1, originalSelected.col - 1);
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelected.col == Width - 1))
                {
                    selected = new FieldLocation(originalSelected.row, originalSelected.col + 1);
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelected.row == 0) & !(originalSelected.col == Width - 1))
                {
                    selected = new FieldLocation(originalSelected.row - 1, originalSelected.col + 1);
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelected.row == Height - 1))
                {
                    selected = new FieldLocation(originalSelected.row + 1, originalSelected.col);
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelected.row == Height - 1) & !(originalSelected.col == 0))
                {
                    selected = new FieldLocation(originalSelected.row + 1, originalSelected.col - 1);
                    TileClick();
                }
                if (gameState != GameState.Playing) return;
                if (!(originalSelected.row == Height - 1) & !(originalSelected.col == Width - 1))
                {
                    selected = new FieldLocation(originalSelected.row + 1, originalSelected.col + 1);
                    TileClick();
                }
                if (gameState != GameState.Playing) return;

                selected = originalSelected;
            }
            else
            {
                faceValue = Face.Happy;
            }
        }

        #region DRAWING
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            UpdateCorner();

            ScreenManager.GraphicsDevice.Clear(Game.Skin.background);

            ScreenManager.SpriteBatch.Begin();
            DrawFieldBorder(ScreenManager.SpriteBatch);
            DrawField(ScreenManager.SpriteBatch);
            //DrawBackground
            ScreenManager.SpriteBatch.Draw(Game.Skin.top, new Rectangle(0, 0, 240, 56), Color.White);
            MinesweeperGame.DrawNumbers(ScreenManager.SpriteBatch, Game.Skin, flags, 16, 16);
            MinesweeperGame.DrawNumbers(ScreenManager.SpriteBatch, Game.Skin, time, 185, 16);
            DrawFace(ScreenManager.SpriteBatch);
            //DrawTileSelect
            if (!faceSelected) ScreenManager.SpriteBatch.Draw(Game.Skin.select,
                new Rectangle(8 + selected.col * 16 - corner.col * 16, 64 + selected.row * 16 - corner.row * 16, 16, 16), Color.White);
            ScreenManager.SpriteBatch.End();
        }

        void DrawFace(SpriteBatch batch)
        {
            Texture2D face = null;
            switch (faceValue)
            {
                case Face.Happy:
                    face = Game.Skin.fHappy;
                    break;
                case Face.Win:
                    face = Game.Skin.fWin;
                    break;
                case Face.Dead:
                    face = Game.Skin.fDead;
                    break;
                case Face.Scared:
                    face = Game.Skin.fScared;
                    break;
            }
            batch.Draw(face, new Rectangle(108, 16, 24, 24), Color.White);
            face = null;
            if (faceSelected) batch.Draw(Game.Skin.faceSelect, new Rectangle(108, 16, 24, 24), Color.White);
        }

        void DrawFieldBorder(SpriteBatch batch)
        {
            batch.Draw(Game.Skin.borderTL, new Rectangle(0 - corner.col * 16, 56 - corner.row * 16, 8, 8), Color.White);
            batch.Draw(Game.Skin.borderT, new Rectangle(8 - corner.col * 16, 56 - corner.row * 16, 16 * Width, 8), Color.White);
            batch.Draw(Game.Skin.borderTR, new Rectangle(8 + 16 * Width - corner.col * 16, 56 - corner.row * 16, 8, 8), Color.White);
            batch.Draw(Game.Skin.borderL, new Rectangle(0 - corner.col * 16, 64 - corner.row * 16, 8, 16 * Height), Color.White);
            batch.Draw(Game.Skin.borderR, new Rectangle(8 + 16 * Width - corner.col * 16, 64 - corner.row * 16, 8, 16 * Height), Color.White);
            batch.Draw(Game.Skin.borderBL, new Rectangle(0 - corner.col * 16, 64 + 16 * Height - corner.row * 16, 8, 8), Color.White);
            batch.Draw(Game.Skin.borderB, new Rectangle(8 - corner.col * 16, 64 + 16 * Height - corner.row * 16, 16 * Width, 8), Color.White);
            batch.Draw(Game.Skin.borderBR, new Rectangle(8 + 16 * Width - corner.col * 16, 64 + 16 * Height - corner.row * 16, 8, 8), Color.White);
        }

        void DrawField(SpriteBatch batch)
        {
            Texture2D tile;
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {                    
                    if (gameState == GameState.Playing || gameState == GameState.NotPlaying)
                    {
                        if (field[row, col].Flagged) tile = Game.Skin.tFlag;
                        else if (field[row, col].Hidden) tile = Game.Skin.tHidden;
                        else tile = Game.Skin.t[field[row, col].Number];
                    }
                    else
                    {
                        if (field[row, col].Flagged & !field[row, col].Mined) tile = Game.Skin.tNotMine;
                        else if (field[row, col].Flagged) tile = Game.Skin.tFlag;
                        else if (field[row, col].Hidden) tile = Game.Skin.tHidden;
                        else if (row == selectedMine.row && col == selectedMine.col && gameState == GameState.Lost) tile = Game.Skin.tClickedMine;
                        else if (field[row, col].Mined) tile = Game.Skin.tMine;
                        else tile = Game.Skin.t[field[row, col].Number];
                    }
                    batch.Draw(tile, new Rectangle(8 + col * 16 - corner.col * 16, 64 + row * 16 - corner.row * 16, 16, 16), Color.White);
                }
            }
            tile = null;
        }
        #endregion
    }
}