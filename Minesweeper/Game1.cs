using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace Minesweeper
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MinesweeperGame mGame;

        protected Texture2D background = null;
        protected Texture2D[] numbers = new Texture2D[12];
        protected Texture2D fHappy, fWin, fDead, fScared;
        protected Texture2D faceSelect;
        protected Texture2D borderTL, borderT, borderTR, borderR, borderBR, borderB, borderBL, borderL;
        protected Texture2D[] t = new Texture2D[9];
        protected Texture2D tHidden, tFlag, tMine, tNotMine, tClickedMine;
        protected Texture2D select;

        protected int height;
        protected int width;
        protected int mines;

        protected int flags;
        protected int time;
        protected double totalTime;

        protected int[] selectedTile = new int[2];
        protected int[] selectedMine = new int[2];
        protected enum Face { Happy, Win, Dead, Scared };
        protected Face faceValue;
        protected GamePadState newPadState;
        protected enum GameState { NotPlaying, Playing, Won, Lost };
        protected GameState gameState;
        protected bool faceSelected;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            height = 15;
            width = 14;
            mines = 30;
            if (height > 24) height = 24;
            if (height < 9) height = 9;
            if (width > 30) width = 30;
            if (width < 9) width = 9;
            if (mines > (height - 1) * (width - 1)) mines = (height - 1) * (width - 1);
            if (mines < 10) mines = 10;
            flags = mines;
            gameState = GameState.NotPlaying;
            time = 0;
            faceValue = Face.Happy;
            faceSelected = false;
            selectedTile[0] = 0;
            selectedTile[1] = 0;
            mGame = new MinesweeperGame(height, width, mines);
            totalTime = 0.0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>(@"background");
            for (int counter = 0; counter < 10; counter++)
            {
                numbers[counter] = Content.Load<Texture2D>(@"Numbers/" + counter);
            }
            numbers[10] = Content.Load<Texture2D>(@"Numbers/-");
            numbers[11] = Content.Load<Texture2D>(@"Numbers/_");
            fHappy = Content.Load<Texture2D>(@"Faces/happy");
            fWin = Content.Load<Texture2D>(@"Faces/win");
            fDead = Content.Load<Texture2D>(@"Faces/dead");
            fScared = Content.Load<Texture2D>(@"Faces/scared");
            faceSelect = Content.Load<Texture2D>(@"faceselect");
            borderB = Content.Load<Texture2D>(@"Borders/B");
            borderBL = Content.Load<Texture2D>(@"Borders/BL");
            borderBR = Content.Load<Texture2D>(@"Borders/BR");
            borderL = Content.Load<Texture2D>(@"Borders/L");
            borderR = Content.Load<Texture2D>(@"Borders/R");
            borderT = Content.Load<Texture2D>(@"Borders/T");
            borderTL = Content.Load<Texture2D>(@"Borders/TL");
            borderTR = Content.Load<Texture2D>(@"Borders/TR");
            for (int counter = 0; counter < 9; counter++)
            {
                t[counter] = Content.Load<Texture2D>(@"Tiles/" + counter);
            }
            tHidden = Content.Load<Texture2D>(@"Tiles/hidden");
            tFlag = Content.Load<Texture2D>(@"Tiles/flag");
            tMine = Content.Load<Texture2D>(@"Tiles/bomb");
            tNotMine = Content.Load<Texture2D>(@"Tiles/notbomb");
            tClickedMine = Content.Load<Texture2D>(@"Tiles/clickedbomb");
            select = Content.Load<Texture2D>(@"select");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            if (gameState == GameState.Playing)
            {
                totalTime += gameTime.ElapsedGameTime.TotalSeconds;
                time = Convert.ToInt32(totalTime);
            }

            GamePadState oldPadState = newPadState;
            newPadState = GamePad.GetState(PlayerIndex.One);
            if (gameState != GameState.Lost && gameState != GameState.Won)
            {
                if (newPadState.DPad.Right == ButtonState.Pressed && oldPadState.DPad.Right == ButtonState.Released)
                {
                    if (selectedTile[0] < width - 1) selectedTile[0]++;
                    else selectedTile[0] = 0;
                    if (!mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
                    {
                        bool foundGoodTile = false;
                        for (int col = selectedTile[0] + 1; col < width; col++)
                        {
                            if (mGame.tile[selectedTile[1], col].Hidden)
                            {
                                selectedTile[0] = col;
                                foundGoodTile = true;
                                break;
                            }
                        }
                        if (!foundGoodTile)
                        {
                            for (int col = 0; col <= selectedTile[0]; col++)
                            {
                                if (mGame.tile[selectedTile[1], col].Hidden)
                                {
                                    selectedTile[0] = col;
                                    foundGoodTile = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (newPadState.DPad.Left == ButtonState.Pressed && oldPadState.DPad.Left == ButtonState.Released)
                {
                    if (selectedTile[0] == 0) selectedTile[0] = width - 1;
                    else selectedTile[0]--;
                    if (!mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
                    {
                        bool foundGoodTile = false;
                        for (int col = selectedTile[0] - 1; col >= 0; col--)
                        {
                            if (mGame.tile[selectedTile[1], col].Hidden)
                            {
                                selectedTile[0] = col;
                                foundGoodTile = true;
                                break;
                            }
                        }
                        if (!foundGoodTile)
                        {
                            for (int col = width - 1; col >= selectedTile[0]; col--)
                            {
                                if (mGame.tile[selectedTile[1], col].Hidden)
                                {
                                    selectedTile[0] = col;
                                    foundGoodTile = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                {
                    if (faceSelected)
                    {
                        selectedTile[1] = 0;
                        faceSelected = false;
                    }
                    else
                        if (selectedTile[1] < height - 1) selectedTile[1]++;
                        else faceSelected = true;
                    if (!mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
                    {
                        //what to do with a bad tile
                        bool foundGoodTile = false;
                        for (int row = selectedTile[1]; row < height; row++)
                        {
                            for (int col = 0; col < width; col++)
                            {
                                if (mGame.tile[row, col].Hidden)
                                {
                                    selectedTile[1] = row;
                                    selectedTile[0] = col;
                                    foundGoodTile = true;
                                    break;
                                }
                            }
                            if (foundGoodTile) break;
                        }
                        if (!foundGoodTile) faceSelected = true;
                    }
                }
                if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                {
                    if (faceSelected)
                    {
                        selectedTile[1] = height - 1;
                        faceSelected = false;
                    }
                    else
                        if (selectedTile[1] == 0) faceSelected = true;
                        else selectedTile[1]--;
                    if (!mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
                    {
                        //what to do with a bad tile
                        bool foundGoodTile = false;
                        for (int row = selectedTile[1]; row >= 0; row--)
                        {
                            for (int col = 0; col < width; col++)
                            {
                                if (mGame.tile[row, col].Hidden)
                                {
                                    selectedTile[1] = row;
                                    selectedTile[0] = col;
                                    foundGoodTile = true;
                                    break;
                                }
                            }
                            if (foundGoodTile) break;
                        }
                        if (!foundGoodTile) faceSelected = true;
                    }
                }
                if (newPadState.Buttons.A == ButtonState.Pressed)
                {
                    if (!faceSelected) faceValue = Face.Scared;
                }
                if (newPadState.Buttons.B == ButtonState.Pressed && oldPadState.Buttons.B == ButtonState.Released)
                {
                    if (!faceSelected) TileFlag();
                }
            }
            if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
            {
                if (faceSelected)
                {
                    mGame = new MinesweeperGame(height, width, mines);
                    flags = mines;
                    gameState = GameState.NotPlaying;
                    time = 0;
                    totalTime = 0.0;
                    faceValue = Face.Happy;
                    //this.Initialize();
                }
                else TileClick();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            DrawBackground(spriteBatch);
            DrawNumbers(spriteBatch, flags, 16, 16);
            DrawNumbers(spriteBatch, time, 185, 16);
            DrawFace(spriteBatch);
            DrawFieldBorder(spriteBatch);
            DrawField(spriteBatch);
            DrawTileSelect(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawBackground(SpriteBatch batch)
        {
            batch.Draw(background, new Rectangle(0, 0, 240, 320), Color.White);
        }

        protected void DrawNumbers(SpriteBatch batch, int amount, int x, int y)
        {
            int[] amountNums = new int[3];
            if (amount >= 0)
            {
                if (amount > 999) amount = 999;
                amountNums[0] = (amount - (amount % 100)) / 100;
                amountNums[1] = ((amount - amountNums[0] * 100) - ((amount - amountNums[0] * 100) % 10)) / 10;
                amountNums[2] = amount - (amountNums[0] * 100) - (amountNums[1] * 10);
                batch.Draw(numbers[amountNums[0]], new Rectangle(x, y, 13, 23), Color.White);
                batch.Draw(numbers[amountNums[1]], new Rectangle(x + 13, y, 13, 23), Color.White);
                batch.Draw(numbers[amountNums[2]], new Rectangle(x + 26, y, 13, 23), Color.White);
            }
            else
            {
                char[] amountParts = new char[10];
                amountParts = amount.ToString().ToCharArray();
                if (amount < 0 & amount > -10) amountNums[1] = 0;
                else amountNums[1] = int.Parse(amountParts[amountParts.GetUpperBound(0) - 1].ToString());
                amountNums[2] = int.Parse(amountParts[amountParts.GetUpperBound(0)].ToString());
                if (amount < 0 & amount > -10) amountNums[1] = 0;
                batch.Draw(numbers[10], new Rectangle(x, y, 13, 23), Color.White);
                batch.Draw(numbers[amountNums[1]], new Rectangle(x + 13, y, 13, 23), Color.White);
                batch.Draw(numbers[amountNums[2]], new Rectangle(x + 26, y, 13, 23), Color.White);
            }
        }

        protected void DrawFace(SpriteBatch batch)
        {
            if (faceValue == Face.Happy) batch.Draw(fHappy, new Rectangle(108, 16, 24, 24), Color.White);
            else
                if (faceValue == Face.Win) batch.Draw(fWin, new Rectangle(108, 16, 24, 24), Color.White);
                else
                    if (faceValue == Face.Dead) batch.Draw(fDead, new Rectangle(108, 16, 24, 24), Color.White);
                    else batch.Draw(fScared, new Rectangle(108, 16, 24, 24), Color.White);
            if (faceSelected) batch.Draw(faceSelect, new Rectangle(108, 16, 24, 24), Color.White);
        }

        protected void DrawFieldBorder(SpriteBatch batch)
        {
            batch.Draw(borderTL, new Rectangle(0, 56, 8, 8), Color.White);
            batch.Draw(borderT, new Rectangle(8, 56, 16 * width, 8), Color.White);
            batch.Draw(borderTR, new Rectangle(8 + 16 * width, 56, 8, 8), Color.White);
            batch.Draw(borderL, new Rectangle(0, 64, 8, 16 * height), Color.White);
            batch.Draw(borderR, new Rectangle(8 + 16 * width, 64, 8, 16 * height), Color.White);
            batch.Draw(borderBL, new Rectangle(0, 64 + 16 * height, 8, 8), Color.White);
            batch.Draw(borderB, new Rectangle(8, 64 + 16 * height, 16 * width, 8), Color.White);
            batch.Draw(borderBR, new Rectangle(8 + 16 * width, 64 + 16 * height, 8, 8), Color.White);
        }

        protected void DrawTileSelect(SpriteBatch batch)
        {
            if (!faceSelected) batch.Draw(select, new Rectangle(8 + selectedTile[0] * 16, 64 + selectedTile[1] * 16, 16, 16), Color.White);
        }

        protected void DrawField(SpriteBatch batch)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Texture2D tile;
                    if (gameState == GameState.Playing)
                    {
                        if (mGame.tile[row, col].Flagged) tile = tFlag;
                        else
                            if (mGame.tile[row, col].Hidden) tile = tHidden;
                            else tile = t[mGame.tile[row, col].TileNum];
                    }
                    else
                    {
                        if (mGame.tile[row, col].Flagged & !mGame.tile[row, col].MineHere) tile = tNotMine;
                        else
                            if (mGame.tile[row, col].Flagged) tile = tFlag;
                            else
                                if (mGame.tile[row, col].Hidden) tile = tHidden;
                                else
                                    if (row == selectedMine[1] && col == selectedMine[0] && gameState == GameState.Lost) tile = tClickedMine;
                                    else
                                        if (mGame.tile[row, col].MineHere) tile = tMine;
                                        else tile = t[mGame.tile[row, col].TileNum];
                    }
                    batch.Draw(tile, new Rectangle(8 + col * 16, 64 + row * 16, 16, 16), Color.White);
                }
            }
        }

        protected void TileClick()
        {
            if (gameState != GameState.Playing) gameState = GameState.Playing;
            switch (mGame.SelectedAction(selectedTile[1], selectedTile[0]))
            {
                case 0: //Game continues
                    gameState = GameState.Playing;
                    faceValue = Face.Happy;
                    for (int counter = 1; counter < width - 1 | counter < height - 1; counter++)
                    {
                        if (selectedTile[1] >= counter)
                            if (mGame.tile[selectedTile[1] - counter, selectedTile[0]].Hidden)
                            {
                                selectedTile[1] -= counter;
                                //selectedTile[0] = selectedTile[0];
                                break;
                            }
                        if (selectedTile[0] >= counter)
                            if (mGame.tile[selectedTile[1], selectedTile[0] - counter].Hidden)
                            {
                                //selectedTile[1] = selectedTile[1];
                                selectedTile[0] -= counter;
                                break;
                            }
                        if (selectedTile[1] >= counter & selectedTile[0] >= counter)
                            if (mGame.tile[selectedTile[1] - counter, selectedTile[0] - counter].Hidden)
                            {
                                selectedTile[1] -= counter;
                                selectedTile[0] -= counter;
                                break;
                            }
                        if (selectedTile[0] <= width - 1 - counter)
                            if (mGame.tile[selectedTile[1], selectedTile[0] + counter].Hidden)
                            {
                                //selectedTile[1] = selectedTile[1];
                                selectedTile[0] += counter;
                                break;
                            }
                        if (selectedTile[1] >= counter & selectedTile[0] <= width - 1 - counter)
                            if (mGame.tile[selectedTile[1] - counter, selectedTile[0] + counter].Hidden)
                            {
                                selectedTile[1] -= counter;
                                selectedTile[0] += counter;
                                break;
                            }
                        if (selectedTile[1] <= height - 1 - counter)
                            if (mGame.tile[selectedTile[1] + counter, selectedTile[0]].Hidden)
                            {
                                selectedTile[1] += counter;
                                //selectedTile[0] = selectedTile[0];
                                break;
                            }
                        if (selectedTile[1] <= height - 1 - counter & selectedTile[0] >= counter)
                            if (mGame.tile[selectedTile[1] + counter, selectedTile[0] - counter].Hidden)
                            {
                                selectedTile[1] += counter;
                                selectedTile[0] -= counter;
                                break;
                            }
                        if (selectedTile[1] <= height - 1 - counter & selectedTile[0] <= width - 1 - counter)
                            if (mGame.tile[selectedTile[1] + counter, selectedTile[0] + counter].Hidden)
                            {
                                selectedTile[1] += counter;
                                selectedTile[0] += counter;
                                break;
                            }
                    }
                    break;
                case 1: //Game won
                    gameState = GameState.Won;
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            if (mGame.tile[row, col].MineHere == true) mGame.tile[row, col].Flag();
                        }
                    }
                    flags = 0;
                    faceValue = Face.Win;
                    faceSelected = true;
                    break;
                case 2: //Game over
                    gameState = GameState.Lost;
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            if (mGame.tile[row, col].MineHere == true) mGame.tile[row, col].Reveal();
                        }
                    }
                    selectedMine = selectedTile;
                    faceValue = Face.Dead;
                    faceSelected = true;
                    break;
            }
        }

        protected void TileFlag()
        {
            if (gameState != GameState.Playing) gameState = GameState.Playing;
            if (!(mGame.tile[selectedTile[1], selectedTile[0]].Flagged))
            {
                if (mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
                {
                    mGame.tile[selectedTile[1], selectedTile[0]].Flag();
                    flags--;
                }
            }
            else
            {
                mGame.tile[selectedTile[1], selectedTile[0]].Unflag();
                flags++;
            }
        }
    }
}
