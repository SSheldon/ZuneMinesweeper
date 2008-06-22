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

        protected Texture2D top;
        protected Texture2D[] numbers = new Texture2D[12];
        protected Texture2D fHappy, fWin, fDead, fScared;
        protected Texture2D faceSelect;
        protected Texture2D borderTL, borderT, borderTR, borderR, borderBR, borderB, borderBL, borderL;
        protected Texture2D[] t = new Texture2D[9];
        protected Texture2D tHidden, tFlag, tMine, tNotMine, tClickedMine;
        protected Texture2D select;
        protected Texture2D mTop, mResume, mResumeGray, mNewGame, mMusic, mBestTimes, mOptions, mExit, mSelect;
        protected Texture2D newGameMenu;
        protected Texture2D customGameMenu, rightArrow, leftArrow;
        protected Texture2D optionsMenu, oCantSelectRevealed, oCanSelectRevealed;

        protected int height;
        protected int width;
        protected int mines;
        protected bool cantSelectRevealed;
        protected int tempHeight, tempWidth, tempMines;

        protected int flags;
        protected int time;
        protected double totalTime;
        protected int[] selectedTile = new int[2]; //tile in column selectedTile[0] of row selectedTile[1]
        protected int[] selectedMine = new int[2];
        protected int[] corner = new int[2];
        protected enum Face { Happy, Win, Dead, Scared };
        protected Face faceValue;
        protected GamePadState newPadState;
        protected enum GameState { NotPlaying, Playing, Won, Lost, Menu, NewGameMenu, CustomGameMenu, OptionsMenu };
        protected GameState gameState;
        protected GameState oldGameState;
        protected bool faceSelected;
        protected enum MenuItem { Resume, NewGame, Music, BestTimes, Options, Exit };
        protected MenuItem selectedMenuItem;
        protected bool resumable;
        protected enum NewGameMenuItem { Beginner, Intermediate, Expert, Zune, Custom, Back };
        protected NewGameMenuItem selectedNewGameItem;
        protected enum CustomGameMenuItem { Height, Width, Mines, OK, Back };
        protected CustomGameMenuItem selectedCustomGameItem;
        protected enum OptionsMenuItem { CantSelectRevealedTiles, Back };
        protected OptionsMenuItem selectedOptionsItem;

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
            height = 9;
            width = 9;
            mines = 10;
            if (height > 24) height = 24;
            if (height < 9) height = 9;
            if (width > 30) width = 30;
            if (width < 9) width = 9;
            if (mines > (height - 1) * (width - 1)) mines = (height - 1) * (width - 1);
            if (mines < 10) mines = 10;
            cantSelectRevealed = true;
            flags = mines;
            gameState = GameState.Menu;
            oldGameState = GameState.NotPlaying;
            time = 0;
            faceValue = Face.Happy;
            faceSelected = false;
            selectedTile[0] = 0;
            selectedTile[1] = 0;
            corner[0] = 0;
            corner[1] = 0;
            mGame = new MinesweeperGame(height, width, mines);
            totalTime = 0.0;
            selectedMenuItem = MenuItem.NewGame;
            resumable = false;
            selectedNewGameItem = NewGameMenuItem.Beginner;
            selectedCustomGameItem = CustomGameMenuItem.Height;
            selectedOptionsItem = OptionsMenuItem.CantSelectRevealedTiles;

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
            top = Content.Load<Texture2D>(@"top");
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
            mTop = Content.Load<Texture2D>(@"Menu/top");
            mResume = Content.Load<Texture2D>(@"Menu/resume");
            mResumeGray = Content.Load<Texture2D>(@"Menu/resume gray");
            mNewGame = Content.Load<Texture2D>(@"Menu/new game");
            mMusic = Content.Load<Texture2D>(@"Menu/music gray");
            mBestTimes = Content.Load<Texture2D>(@"Menu/best times gray");
            mOptions = Content.Load<Texture2D>(@"Menu/options");
            mExit = Content.Load<Texture2D>(@"Menu/exit");
            mSelect = Content.Load<Texture2D>(@"Menu/select");
            newGameMenu = Content.Load<Texture2D>(@"Menu/new game menu");
            customGameMenu = Content.Load<Texture2D>(@"Menu/custom game menu");
            rightArrow = Content.Load<Texture2D>(@"Menu/right arrow");
            leftArrow = Content.Load<Texture2D>(@"Menu/left arrow");
            optionsMenu = Content.Load<Texture2D>(@"Menu/options menu");
            oCantSelectRevealed = Content.Load<Texture2D>(@"Menu/cant select revealed");
            oCanSelectRevealed = Content.Load<Texture2D>(@"Menu/can select revealed");
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
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            //    this.Exit();

            // TODO: Add your update logic here
            if (gameState == GameState.Playing)
            {
                totalTime += gameTime.ElapsedGameTime.TotalSeconds;
                time = Convert.ToInt32(totalTime);
            }

            GamePadState oldPadState = newPadState;
            newPadState = GamePad.GetState(PlayerIndex.One);

            int gameStateNum = 0;
            if (gameState == GameState.Playing || gameState == GameState.NotPlaying) gameStateNum = 0;
            if (gameState == GameState.Lost || gameState == GameState.Won) gameStateNum = 1;
            if (gameState == GameState.Menu) gameStateNum = 2;
            if (gameState == GameState.NewGameMenu) gameStateNum = 3;
            if (gameState == GameState.CustomGameMenu) gameStateNum = 4;
            if (gameState == GameState.OptionsMenu) gameStateNum = 5;

            switch (gameStateNum)
            {
                case 0:
                    if (newPadState.DPad.Right == ButtonState.Pressed && oldPadState.DPad.Right == ButtonState.Released && !faceSelected)
                    {
                        if (selectedTile[0] < width - 1) selectedTile[0]++;
                        else selectedTile[0] = 0;
                        if (cantSelectRevealed && !mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
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
                    if (newPadState.DPad.Left == ButtonState.Pressed && oldPadState.DPad.Left == ButtonState.Released && !faceSelected)
                    {
                        if (selectedTile[0] == 0) selectedTile[0] = width - 1;
                        else selectedTile[0]--;
                        if (cantSelectRevealed && !mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
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
                        if (cantSelectRevealed && !mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
                        {
                            //what to do with a bad tile
                            bool foundGoodTile = false;
                            for (int row = selectedTile[1]; row < height; row++)
                            {
                                int distance = 100;
                                int bestTile = 0;
                                for (int col = 0; col < width; col++)
                                {
                                    if (mGame.tile[row, col].Hidden)
                                    {
                                        int tempDistance = Math.Abs(selectedTile[0] - col);
                                        if (tempDistance < distance)
                                        {
                                            bestTile = col;
                                            distance = tempDistance;
                                        }
                                        selectedTile[1] = row;
                                        foundGoodTile = true;
                                    }
                                }
                                if (foundGoodTile)
                                {
                                    selectedTile[0] = bestTile;
                                    break;
                                }
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
                        if (cantSelectRevealed && !mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
                        {
                            //what to do with a bad tile
                            bool foundGoodTile = false;
                            for (int row = selectedTile[1]; row >= 0; row--)
                            {
                                int distance = 100;
                                int bestTile = 0;
                                for (int col = 0; col < width; col++)
                                {
                                    if (mGame.tile[row, col].Hidden)
                                    {
                                        int tempDistance = Math.Abs(selectedTile[0] - col);
                                        if (tempDistance < distance)
                                        {
                                            bestTile = col;
                                            distance = tempDistance;
                                        }
                                        selectedTile[1] = row;
                                        foundGoodTile = true;
                                    }
                                }
                                if (foundGoodTile)
                                {
                                    selectedTile[0] = bestTile;
                                    break;
                                }
                            }
                            if (!foundGoodTile) faceSelected = true;
                        }
                    }
                    if (newPadState.Buttons.A == ButtonState.Pressed)
                    {
                        if (!faceSelected) faceValue = Face.Scared;
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        if (faceSelected)
                        {
                            NewGame();
                            //this.Initialize();
                        }
                        else TileClick();
                    }
                    if (newPadState.Buttons.B == ButtonState.Pressed && oldPadState.Buttons.B == ButtonState.Released)
                    {
                        if (!faceSelected) TileFlag();
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        oldGameState = gameState;
                        gameState = GameState.Menu;
                        if (resumable) selectedMenuItem = MenuItem.Resume;
                        else selectedMenuItem = MenuItem.NewGame;
                    }
                    if (height > 15)
                    {
                        if (selectedTile[1] - corner[1] > 10)
                        {
                            corner[1] = selectedTile[1] - 10;
                            if (corner[1] + 14 > height - 1) corner[1] = height - 15;
                        }
                        else
                        {
                            if (corner[1] + 4 > selectedTile[1])
                            {
                                corner[1] = selectedTile[1] - 4;
                                if (corner[1] < 0) corner[1] = 0;
                            }
                        }
                    }
                    if (width > 14)
                    {
                        if (selectedTile[0] - corner[0] > 9)
                        {
                            corner[0] = selectedTile[0] - 9;
                            if (corner[0] + 13 > width - 1) corner[0] = width - 14;
                        }
                        else
                        {
                            if (corner[0] + 4 > selectedTile[0])
                            {
                                corner[0] = selectedTile[0] - 4;
                                if (corner[0] < 0) corner[0] = 0;
                            }
                        }
                    }
                    break;
                case 1:
                    if (newPadState.Buttons.A == ButtonState.Pressed && oldPadState.Buttons.A == ButtonState.Released && faceSelected)
                    {
                        NewGame();
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        oldGameState = gameState;
                        gameState = GameState.Menu;
                        if (resumable) selectedMenuItem = MenuItem.Resume;
                        else selectedMenuItem = MenuItem.NewGame;
                    }
                    break;
                case 2:
                    if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                    {
                        if (selectedMenuItem == MenuItem.Resume) selectedMenuItem = MenuItem.NewGame;
                        else
                            if (selectedMenuItem == MenuItem.NewGame) selectedMenuItem = MenuItem.Options;
                            else
                                if (selectedMenuItem == MenuItem.Options) selectedMenuItem = MenuItem.Exit;
                                else
                                    if (selectedMenuItem == MenuItem.Exit)
                                    {
                                        if (resumable) selectedMenuItem = MenuItem.Resume;
                                        else selectedMenuItem = MenuItem.NewGame;
                                    }
                    }
                    if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                    {
                        if (selectedMenuItem == MenuItem.Resume) selectedMenuItem = MenuItem.Exit;
                        else
                            if (selectedMenuItem == MenuItem.Exit) selectedMenuItem = MenuItem.Options;
                            else
                                if (selectedMenuItem == MenuItem.Options) selectedMenuItem = MenuItem.NewGame;
                                else
                                    if (selectedMenuItem == MenuItem.NewGame)
                                    {
                                        if (resumable) selectedMenuItem = MenuItem.Resume;
                                        else selectedMenuItem = MenuItem.Exit;
                                    }
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        if (selectedMenuItem == MenuItem.Resume) gameState = oldGameState;
                        if (selectedMenuItem == MenuItem.NewGame)
                        {
                            gameState = GameState.NewGameMenu;
                        }
                        if (selectedMenuItem == MenuItem.Options)
                        {
                            gameState = GameState.OptionsMenu;
                        }
                        if (selectedMenuItem == MenuItem.Exit)
                        {
                            this.Exit();
                        }
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        if (resumable) gameState = oldGameState;
                    }
                    break;
                case 3:
                    if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                    {
                        if (selectedNewGameItem == NewGameMenuItem.Beginner) selectedNewGameItem = NewGameMenuItem.Intermediate;
                        else
                            if (selectedNewGameItem == NewGameMenuItem.Intermediate) selectedNewGameItem = NewGameMenuItem.Expert;
                            else
                                if (selectedNewGameItem == NewGameMenuItem.Expert) selectedNewGameItem = NewGameMenuItem.Zune;
                                else
                                    if (selectedNewGameItem == NewGameMenuItem.Zune) selectedNewGameItem = NewGameMenuItem.Custom;
                                    else
                                        if (selectedNewGameItem == NewGameMenuItem.Custom) selectedNewGameItem = NewGameMenuItem.Back;
                                        else selectedNewGameItem = NewGameMenuItem.Beginner;
                    }
                    if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                    {
                        if (selectedNewGameItem == NewGameMenuItem.Beginner) selectedNewGameItem = NewGameMenuItem.Back;
                        else
                            if (selectedNewGameItem == NewGameMenuItem.Intermediate) selectedNewGameItem = NewGameMenuItem.Beginner;
                            else
                                if (selectedNewGameItem == NewGameMenuItem.Expert) selectedNewGameItem = NewGameMenuItem.Intermediate;
                                else
                                    if (selectedNewGameItem == NewGameMenuItem.Zune) selectedNewGameItem = NewGameMenuItem.Expert;
                                    else
                                        if (selectedNewGameItem == NewGameMenuItem.Custom) selectedNewGameItem = NewGameMenuItem.Zune;
                                        else selectedNewGameItem = NewGameMenuItem.Custom;
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        if (selectedNewGameItem == NewGameMenuItem.Beginner)
                        {
                            height = 9;
                            width = 9;
                            mines = 10;
                            NewGame();
                            gameState = GameState.NotPlaying;
                            resumable = true;
                        }
                        if (selectedNewGameItem == NewGameMenuItem.Intermediate)
                        {
                            height = 16;
                            width = 16;
                            mines = 40;
                            NewGame();
                            gameState = GameState.NotPlaying;
                            resumable = true;
                        }
                        if (selectedNewGameItem == NewGameMenuItem.Expert)
                        {
                            height = 24;
                            width = 30;
                            mines = 99;
                            NewGame();
                            gameState = GameState.NotPlaying;
                            resumable = true;
                        }
                        if (selectedNewGameItem == NewGameMenuItem.Zune)
                        {
                            height = 15;
                            width = 14;
                            mines = 30;
                            NewGame();
                            gameState = GameState.NotPlaying;
                            resumable = true;
                        }
                        if (selectedNewGameItem == NewGameMenuItem.Custom)
                        {
                            gameState = GameState.CustomGameMenu;
                            tempHeight = height;
                            tempWidth = width;
                            tempMines = mines;
                        }
                        if (selectedNewGameItem == NewGameMenuItem.Back)
                        {
                            gameState = GameState.Menu;
                        }
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        gameState = GameState.Menu;
                    }
                    break;
                case 4:
                    if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                    {
                        if (selectedCustomGameItem == CustomGameMenuItem.Height) selectedCustomGameItem = CustomGameMenuItem.Width;
                        else
                            if (selectedCustomGameItem == CustomGameMenuItem.Width) selectedCustomGameItem = CustomGameMenuItem.Mines;
                            else
                                if (selectedCustomGameItem == CustomGameMenuItem.Mines) selectedCustomGameItem = CustomGameMenuItem.OK;
                                else
                                    if (selectedCustomGameItem == CustomGameMenuItem.OK) selectedCustomGameItem = CustomGameMenuItem.Back;
                                    else selectedCustomGameItem = CustomGameMenuItem.Height;
                    }
                    if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                    {
                        if (selectedCustomGameItem == CustomGameMenuItem.Height) selectedCustomGameItem = CustomGameMenuItem.Back;
                        else
                            if (selectedCustomGameItem == CustomGameMenuItem.Width) selectedCustomGameItem = CustomGameMenuItem.Height;
                            else
                                if (selectedCustomGameItem == CustomGameMenuItem.Mines) selectedCustomGameItem = CustomGameMenuItem.Width;
                                else
                                    if (selectedCustomGameItem == CustomGameMenuItem.OK) selectedCustomGameItem = CustomGameMenuItem.Mines;
                                    else selectedCustomGameItem = CustomGameMenuItem.OK;
                    }
                    if (newPadState.DPad.Right == ButtonState.Pressed && oldPadState.DPad.Right == ButtonState.Released)
                    {
                        if (selectedCustomGameItem == CustomGameMenuItem.Height)
                        {
                            tempHeight++;
                            if (tempHeight > 24) tempHeight = 9;
                        }
                        if (selectedCustomGameItem == CustomGameMenuItem.Width)
                        {
                            tempWidth++;
                            if (tempWidth > 30) tempWidth = 9;
                        }
                        if (selectedCustomGameItem == CustomGameMenuItem.Mines)
                        {
                            tempMines++;
                            if (tempMines > (tempHeight - 1) * (tempWidth - 1)) tempMines = 10;
                        }
                    }
                    if (newPadState.DPad.Left == ButtonState.Pressed && oldPadState.DPad.Left == ButtonState.Released)
                    {
                        if (selectedCustomGameItem == CustomGameMenuItem.Height)
                        {
                            tempHeight--;
                            if (tempHeight < 9) tempHeight = 24;
                        }
                        if (selectedCustomGameItem == CustomGameMenuItem.Width)
                        {
                            tempWidth--;
                            if (tempWidth < 9) tempWidth = 30;
                        }
                        if (selectedCustomGameItem == CustomGameMenuItem.Mines)
                        {
                            tempMines--;
                            if (tempMines < 10) tempMines = (tempHeight - 1) * (tempWidth - 1);
                        }
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        if (selectedCustomGameItem == CustomGameMenuItem.Back)
                        {
                            gameState = GameState.NewGameMenu;
                        }
                        if (selectedCustomGameItem == CustomGameMenuItem.OK)
                        {
                            height = tempHeight;
                            width = tempWidth;
                            mines = tempMines;
                            NewGame();
                            gameState = GameState.NotPlaying;
                            resumable = true;
                        }
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        gameState = GameState.NewGameMenu;
                    }
                    break;
                case 5:
                    if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                    {
                        if (selectedOptionsItem == OptionsMenuItem.CantSelectRevealedTiles) selectedOptionsItem = OptionsMenuItem.Back;
                        else selectedOptionsItem = OptionsMenuItem.CantSelectRevealedTiles;
                    }
                    if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                    {
                        if (selectedOptionsItem == OptionsMenuItem.CantSelectRevealedTiles) selectedOptionsItem = OptionsMenuItem.Back;
                        else selectedOptionsItem = OptionsMenuItem.CantSelectRevealedTiles;
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        if (selectedOptionsItem == OptionsMenuItem.Back)
                        {
                            gameState = GameState.Menu;
                        }
                        if (selectedOptionsItem == OptionsMenuItem.CantSelectRevealedTiles)
                        {
                            if (cantSelectRevealed) cantSelectRevealed = false;
                            else cantSelectRevealed = true;
                        }
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        gameState = GameState.Menu;
                    }
                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Silver);

            // TODO: Add your drawing code here
            if (gameState == GameState.Lost | gameState == GameState.Won | gameState == GameState.Playing | gameState == GameState.NotPlaying)
            {
                spriteBatch.Begin();
                DrawFieldBorder(spriteBatch);
                DrawField(spriteBatch);
                DrawBackground(spriteBatch);
                DrawNumbers(spriteBatch, flags, 16, 16);
                DrawNumbers(spriteBatch, time, 185, 16);
                DrawFace(spriteBatch);
                DrawTileSelect(spriteBatch);
                spriteBatch.End();
            }
            if (gameState == GameState.Menu)
            {
                spriteBatch.Begin();
                DrawMenu(spriteBatch);
                spriteBatch.End();
            }
            if (gameState == GameState.NewGameMenu)
            {
                spriteBatch.Begin();
                DrawNewGameMenu(spriteBatch);
                spriteBatch.End();
            }
            if (gameState == GameState.CustomGameMenu)
            {
                spriteBatch.Begin();
                DrawCustomGameMenu(spriteBatch);
                DrawNumbers(spriteBatch, tempHeight, 183, 82);
                DrawNumbers(spriteBatch, tempWidth, 183, 122);
                DrawNumbers(spriteBatch, tempMines, 183, 162);
                spriteBatch.End();
            }
            if (gameState == GameState.OptionsMenu)
            {
                spriteBatch.Begin();
                DrawOptionsMenu(spriteBatch);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        protected void DrawBackground(SpriteBatch batch)
        {
            batch.Draw(top, new Rectangle(0, 0, 240, 56), Color.White);
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
            batch.Draw(borderTL, new Rectangle(0 - corner[0] * 16, 56 - corner[1] * 16, 8, 8), Color.White);
            batch.Draw(borderT, new Rectangle(8 - corner[0] * 16, 56 - corner[1] * 16, 16 * width, 8), Color.White);
            batch.Draw(borderTR, new Rectangle(8 + 16 * width - corner[0] * 16, 56 - corner[1] * 16, 8, 8), Color.White);
            batch.Draw(borderL, new Rectangle(0 - corner[0] * 16, 64 - corner[1] * 16, 8, 16 * height), Color.White);
            batch.Draw(borderR, new Rectangle(8 + 16 * width - corner[0] * 16, 64 - corner[1] * 16, 8, 16 * height), Color.White);
            batch.Draw(borderBL, new Rectangle(0 - corner[0] * 16, 64 + 16 * height - corner[1] * 16, 8, 8), Color.White);
            batch.Draw(borderB, new Rectangle(8 - corner[0] * 16, 64 + 16 * height - corner[1] * 16, 16 * width, 8), Color.White);
            batch.Draw(borderBR, new Rectangle(8 + 16 * width - corner[0] * 16, 64 + 16 * height - corner[1] * 16, 8, 8), Color.White);
        }

        protected void DrawTileSelect(SpriteBatch batch)
        {
            if (!faceSelected) batch.Draw(select, new Rectangle(8 + selectedTile[0] * 16 - corner[0] * 16, 64 + selectedTile[1] * 16 - corner[1] * 16, 16, 16), Color.White);
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
                    batch.Draw(tile, new Rectangle(8 + col * 16 - corner[0] * 16, 64 + row * 16 - corner[1] * 16, 16, 16), Color.White);
                }
            }
        }

        protected void DrawMenu(SpriteBatch batch)
        {
            batch.Draw(mTop, new Rectangle(0, 0, 240, 72), Color.White);
            if (resumable) batch.Draw(mResume, new Rectangle(16, 80, 208, 32), Color.White);
            else batch.Draw(mResumeGray, new Rectangle(16, 80, 208, 32), Color.White);
            batch.Draw(mNewGame, new Rectangle(16, 120, 208, 32), Color.White);
            batch.Draw(mMusic, new Rectangle(16, 160, 208, 32), Color.White);
            batch.Draw(mBestTimes, new Rectangle(16, 200, 208, 32), Color.White);
            batch.Draw(mOptions, new Rectangle(16, 240, 208, 32), Color.White);
            batch.Draw(mExit, new Rectangle(16, 280, 208, 32), Color.White);
            int selectY;
            if (selectedMenuItem == MenuItem.Resume) selectY = 78;
            else
                if (selectedMenuItem == MenuItem.NewGame) selectY = 118;
                else
                    if (selectedMenuItem == MenuItem.Music) selectY = 158;
                    else
                        if (selectedMenuItem == MenuItem.BestTimes) selectY = 198;
                        else
                            if (selectedMenuItem == MenuItem.Options) selectY = 238;
                            else selectY = 278;
            batch.Draw(mSelect, new Rectangle(14, selectY, 212, 36), Color.White);
        }

        protected void DrawNewGameMenu(SpriteBatch batch)
        {
            batch.Draw(newGameMenu, new Rectangle(0, 0, 240, 320), Color.White);
            int selectY;
            if (selectedNewGameItem == NewGameMenuItem.Beginner) selectY = 78;
            else
                if (selectedNewGameItem == NewGameMenuItem.Intermediate) selectY = 118;
                else
                    if (selectedNewGameItem == NewGameMenuItem.Expert) selectY = 158;
                    else
                        if (selectedNewGameItem == NewGameMenuItem.Zune) selectY = 198;
                        else
                            if (selectedNewGameItem == NewGameMenuItem.Custom) selectY = 238;
                            else selectY = 278;
            batch.Draw(mSelect, new Rectangle(14, selectY, 212, 36), Color.White);
        }

        protected void DrawCustomGameMenu(SpriteBatch batch)
        {
            batch.Draw(customGameMenu, new Rectangle(0, 0, 240, 320), Color.White); // 183 82
            if (selectedCustomGameItem == CustomGameMenuItem.Height)
            {
                if (tempHeight > 9) batch.Draw(leftArrow, new Rectangle(175, 83, 6, 21), Color.White);
                if (tempHeight < 24) batch.Draw(rightArrow, new Rectangle(224, 83, 6, 21), Color.White);
            }
            if (selectedCustomGameItem == CustomGameMenuItem.Width)
            {
                if (tempWidth > 9) batch.Draw(leftArrow, new Rectangle(175, 123, 6, 21), Color.White);
                if (tempWidth < 30) batch.Draw(rightArrow, new Rectangle(224, 123, 6, 21), Color.White);
            }
            if (selectedCustomGameItem == CustomGameMenuItem.Mines)
            {
                if (tempMines > 10) batch.Draw(leftArrow, new Rectangle(175, 163, 6, 21), Color.White);
                if (tempMines < (tempHeight - 1) * (tempWidth - 1)) batch.Draw(rightArrow, new Rectangle(224, 163, 6, 21), Color.White);
            }
            if (selectedCustomGameItem == CustomGameMenuItem.OK) batch.Draw(mSelect, new Rectangle(14, 238, 212, 36), Color.White);
            if (selectedCustomGameItem == CustomGameMenuItem.Back) batch.Draw(mSelect, new Rectangle(14, 278, 212, 36), Color.White);
        }

        protected void DrawOptionsMenu(SpriteBatch batch)
        {
            batch.Draw(optionsMenu, new Rectangle(0, 0, 240, 320), Color.White);
            if (cantSelectRevealed) batch.Draw(oCantSelectRevealed, new Rectangle(16, 80, 208, 32), Color.White);
            else batch.Draw(oCanSelectRevealed, new Rectangle(16, 80, 208, 32), Color.White);
            if (selectedOptionsItem == OptionsMenuItem.CantSelectRevealedTiles) batch.Draw(mSelect, new Rectangle(14, 78, 212, 36), Color.White);
            else batch.Draw(mSelect, new Rectangle(14, 278, 212, 36), Color.White);
        }

        protected void TileClick()
        {
            if (gameState != GameState.Playing) gameState = GameState.Playing;
            switch (mGame.SelectedAction(selectedTile[1], selectedTile[0]))
            {
                case 0: //Game continues
                    gameState = GameState.Playing;
                    faceValue = Face.Happy;
                    if (cantSelectRevealed)
                    {
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

        protected void NewGame()
        {
            mGame = new MinesweeperGame(height, width, mines);
            flags = mines;
            gameState = GameState.NotPlaying;
            time = 0;
            totalTime = 0.0;
            faceValue = Face.Happy;
            selectedTile[0] = 0;
            selectedTile[1] = 0;
            corner[0] = 0;
            corner[1] = 0;
        }
    }
}