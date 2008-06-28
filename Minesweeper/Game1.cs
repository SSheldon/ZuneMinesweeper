using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;

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
        StorageDevice storageDevice;
        StorageContainer container;

        protected Texture2D top;
        protected Texture2D[] numbers = new Texture2D[12];
        protected Texture2D fHappy, fWin, fDead, fScared;
        protected Texture2D faceSelect;
        protected Texture2D borderTL, borderT, borderTR, borderR, borderBR, borderB, borderBL, borderL;
        protected Texture2D[] t = new Texture2D[9];
        protected Texture2D tHidden, tFlag, tMine, tNotMine, tClickedMine;
        protected Texture2D select;
        protected Texture2D mTop, mSelect;
        protected Texture2D numBox, rightArrow, leftArrow;
        protected SpriteFont font, header, small;

        protected int height;
        protected int width;
        protected int mines;
        protected bool cantSelectRevealed;
        protected bool flagWithPlay;
        protected bool oldCantSelectRevealed, oldFlagWithPlay;
        protected int tempHeight, tempWidth, tempMines;
        protected int bestBeginner, bestIntermediate, bestExpert, bestZune;

        protected int flags;
        protected int time;
        protected double totalTime;
        protected int[] selectedTile = new int[2]; //tile in column selectedTile[0] of row selectedTile[1]
        protected int[] selectedMine = new int[2];
        protected int[] corner = new int[2];
        protected enum Face { Happy, Win, Dead, Scared };
        protected Face faceValue;
        protected GamePadState newPadState;
        protected enum GameState { NotPlaying, Playing, Won, Lost, Menu, NewGameMenu, CustomGameMenu, OptionsMenu, BestTimesMenu };
        protected GameState gameState;
        protected GameState oldGameState;
        protected enum Difficulty { Beginner, Intermediate, Expert, Zune }
        protected bool faceSelected;
        protected bool resumable;
        Menu mainMenu;
        MenuItem resume, newGame, music, bestTimes, options, exit;
        Menu newGameMenu;
        MenuItem beginner, intermediate, expert, zune, custom, back;
        Menu optionsMenu;
        MenuItem cantSelectRevealedMI, flagButton;
        Menu customGameMenu;
        MenuItem heightMI, widthMI, minesMI, ok;
        Menu bestTimesMenu;
        MenuItem bestBeginnerMI, bestIntermediateMI, bestExpertMI, bestZuneMI, resetMI;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Components.Add(new GamerServicesComponent(this));
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
            IAsyncResult syncResult = Guide.BeginShowStorageDeviceSelector(null, null);
            //while (!ar.IsCompleted) Thread.Sleep(10);
            storageDevice = Guide.EndShowStorageDeviceSelector(syncResult);
            //if (!storageDevice.IsConnected) Exit();
            container = storageDevice.OpenContainer("Minesweeper");
            bestBeginner = 999;
            bestIntermediate = 999;
            bestExpert = 999;
            bestZune = 999;
            GetBestTimes();

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
            flagWithPlay = true;
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
            resumable = false;

            GetOptions();
            
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
            mSelect = Content.Load<Texture2D>(@"Menu/select");
            numBox = Content.Load<Texture2D>(@"Menu/number back");
            rightArrow = Content.Load<Texture2D>(@"Menu/right arrow");
            leftArrow = Content.Load<Texture2D>(@"Menu/left arrow");
            font = Content.Load<SpriteFont>(@"fonts/normal");
            header = Content.Load<SpriteFont>(@"fonts/header");
            small = Content.Load<SpriteFont>(@"fonts/small");

            InitializeMenus();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected void InitializeMenus()
        {
            mainMenu = new Menu(mTop, mSelect);
            resume = new MenuItem("Resume", resumable, font);
            resume.itemClicked += new ItemClick(Resume);
            mainMenu.Add(0, ref resume);
            newGame = new MenuItem("New Game", font);
            newGame.itemClicked += new ItemClick(NewGameClick);
            mainMenu.Add(1, ref newGame);
            music = new MenuItem("Music", false, font);
            music.itemClicked += new ItemClick(DoNothing);
            mainMenu.Add(2, ref music);
            bestTimes = new MenuItem("Best Times", font);
            bestTimes.itemClicked += new ItemClick(BestTimesClick);
            mainMenu.Add(3, ref bestTimes);
            options = new MenuItem("Options", font);
            options.itemClicked +=new ItemClick(OptionsClick);
            mainMenu.Add(4, ref options);
            exit = new MenuItem("Exit", font);
            exit.itemClicked += new ItemClick(Exit);
            mainMenu.Add(5, ref exit);

            newGameMenu = new Menu("New Game:", header, mSelect);
            beginner = new MenuItem("Beginner", font);
            beginner.itemClicked += new ItemClick(BeginnerClick);
            newGameMenu.Add(0, ref beginner);
            intermediate = new MenuItem("Intermediate", font);
            intermediate.itemClicked += new ItemClick(IntermediateClick);
            newGameMenu.Add(1, ref intermediate);
            expert = new MenuItem("Expert", font);
            expert.itemClicked += new ItemClick(ExpertClick);
            newGameMenu.Add(2, ref expert);
            zune = new MenuItem("Zune Fit", font);
            zune.itemClicked += new ItemClick(ZuneClick);
            newGameMenu.Add(3, ref zune);
            custom = new MenuItem("Custom", font);
            custom.itemClicked += new ItemClick(CustomClick);
            newGameMenu.Add(4, ref custom);
            back = new MenuItem("Back", font);
            back.itemClicked += new ItemClick(Back);
            newGameMenu.Add(5, ref back);

            optionsMenu = new Menu("Options:", header, mSelect);
            cantSelectRevealedMI = new MenuItem("Revealed tiles can't be selected", small);
            cantSelectRevealedMI.smallFont = true;
            cantSelectRevealedMI.itemClicked += new ItemClick(CantSelectRevealedClick);
            optionsMenu.Add(0, ref cantSelectRevealedMI);
            flagButton = new MenuItem("Flag tiles with Play button", small);
            flagButton.smallFont = true;
            flagButton.itemClicked += new ItemClick(FlagButtonClick);
            optionsMenu.Add(1, ref flagButton);
            optionsMenu.Add(5, ref back);

            customGameMenu = new Menu("Custom Game:", header, mSelect);
            heightMI = new MenuItem("Height", font);
            heightMI.itemClicked += new ItemClick(DoNothing);
            customGameMenu.Add(0, ref heightMI);
            widthMI = new MenuItem("Width", font);
            widthMI.itemClicked += new ItemClick(DoNothing);
            customGameMenu.Add(1, ref widthMI);
            minesMI = new MenuItem("Mines", font);
            minesMI.itemClicked += new ItemClick(DoNothing);
            customGameMenu.Add(2, ref minesMI);
            ok = new MenuItem("OK", font);
            ok.itemClicked += new ItemClick(OKClick);
            customGameMenu.Add(4, ref ok);
            customGameMenu.Add(5, ref back);

            bestTimesMenu = new Menu("Best Times:", header, mSelect);
            bestBeginnerMI = new MenuItem("Beginner:", false, true, font);
            bestTimesMenu.Add(0, ref bestBeginnerMI);
            bestIntermediateMI = new MenuItem("Intermed.:", false, true, font);
            bestTimesMenu.Add(1, ref bestIntermediateMI);
            bestExpertMI = new MenuItem("Expert:", false, true, font);
            bestTimesMenu.Add(2, ref bestExpertMI);
            bestZuneMI = new MenuItem("Zune Fit:", false, true, font);
            bestTimesMenu.Add(3, ref bestZuneMI);
            resetMI = new MenuItem("Reset Times", font);
            resetMI.itemClicked += new ItemClick(ResetClick);
            bestTimesMenu.Add(4, ref resetMI);
            bestTimesMenu.Add(5, ref back);
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
            if (time > 999) time = 999;

            resume.selectable = resumable;
            resume.color = resumable;
            cantSelectRevealedMI.text = cantSelectRevealed ? "Revealed tiles can't be selected" : "Revealed tiles can be selected";
            flagButton.text = flagWithPlay ? "Flag tiles with Play button" : "Flag tiles with Center button";

            GamePadState oldPadState = newPadState;
            newPadState = GamePad.GetState(PlayerIndex.One);

            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
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
                        if (!faceSelected & flagWithPlay) faceValue = Face.Scared;
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        if (faceSelected) SetGame();
                        else
                            if (flagWithPlay) TileClick();
                            else TileFlag();
                    }
                    if (newPadState.Buttons.B == ButtonState.Pressed)
                    {
                        if (!faceSelected & !flagWithPlay) faceValue = Face.Scared;
                    }
                    if (newPadState.Buttons.B == ButtonState.Released && oldPadState.Buttons.B == ButtonState.Pressed)
                    {
                        if (!faceSelected)
                            if (flagWithPlay) TileFlag();
                            else TileClick();
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        oldGameState = gameState;
                        gameState = GameState.Menu;
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
                case GameState.Lost:
                case GameState.Won:
                    if (newPadState.Buttons.A == ButtonState.Pressed && oldPadState.Buttons.A == ButtonState.Released && faceSelected)
                    {
                        SetGame();
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        oldGameState = gameState;
                        gameState = GameState.Menu;
                    }
                    break;
                case GameState.Menu:
                    if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                    {
                        mainMenu.DownClick();
                    }
                    if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                    {
                        mainMenu.UpClick();
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        mainMenu.ClickItem();
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        if (resumable) gameState = oldGameState;
                    }
                    break;
                case GameState.NewGameMenu:
                    if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                    {
                        newGameMenu.DownClick();
                    }
                    if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                    {
                        newGameMenu.UpClick();
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        newGameMenu.ClickItem();
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        Back();
                    }
                    break;
                case GameState.CustomGameMenu:
                    if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                    {
                        customGameMenu.DownClick();
                    }
                    if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                    {
                        customGameMenu.UpClick();
                    }
                    if (newPadState.DPad.Right == ButtonState.Pressed && oldPadState.DPad.Right == ButtonState.Released)
                    {
                        switch (customGameMenu.selectedItem)
                        {
                            case 0:
                                tempHeight++;
                                if (tempHeight > 24) tempHeight = 9;
                                break;
                            case 1:
                                tempWidth++;
                                if (tempWidth > 30) tempWidth = 9;
                                break;
                            case 2:
                                tempMines++;
                                if (tempMines > (tempHeight - 1) * (tempWidth - 1)) tempMines = 10;
                                break;
                        }
                    }
                    if (newPadState.DPad.Left == ButtonState.Pressed && oldPadState.DPad.Left == ButtonState.Released)
                    {
                        switch (customGameMenu.selectedItem)
                        {
                            case 0:
                                tempHeight--;
                                if (tempHeight < 9) tempHeight = 24;
                                break;
                            case 1:
                                tempWidth--;
                                if (tempWidth < 9) tempWidth = 30;
                                break;
                            case 2:
                                tempMines--;
                                if (tempMines < 10) tempMines = (tempHeight - 1) * (tempWidth - 1);
                                break;
                        }
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        customGameMenu.ClickItem();
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        Back();
                    }
                    break;
                case GameState.OptionsMenu:
                    if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                    {
                        optionsMenu.DownClick();
                    }
                    if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                    {
                        optionsMenu.UpClick();
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        optionsMenu.ClickItem();
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        Back();
                    }
                    break;
                case GameState.BestTimesMenu:
                    if (newPadState.DPad.Down == ButtonState.Pressed && oldPadState.DPad.Down == ButtonState.Released)
                    {
                        bestTimesMenu.DownClick();
                    }
                    if (newPadState.DPad.Up == ButtonState.Pressed && oldPadState.DPad.Up == ButtonState.Released)
                    {
                        bestTimesMenu.UpClick();
                    }
                    if (newPadState.Buttons.A == ButtonState.Released && oldPadState.Buttons.A == ButtonState.Pressed)
                    {
                        bestTimesMenu.ClickItem();
                    }
                    if (newPadState.Buttons.Back == ButtonState.Pressed && oldPadState.Buttons.Back == ButtonState.Released)
                    {
                        Back();
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
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.Lost:
                case GameState.Won:
                case GameState.Playing: 
                case GameState.NotPlaying:
                    DrawFieldBorder(spriteBatch);
                    DrawField(spriteBatch);
                    DrawBackground(spriteBatch);
                    DrawNumbers(spriteBatch, flags, 16, 16);
                    DrawNumbers(spriteBatch, time, 185, 16);
                    DrawFace(spriteBatch);
                    DrawTileSelect(spriteBatch);
                    break;
                case GameState.Menu:
                    mainMenu.Draw(spriteBatch);
                    break;
                case GameState.NewGameMenu:
                    newGameMenu.Draw(spriteBatch);
                    break;
                case GameState.CustomGameMenu:
                    customGameMenu.Draw(spriteBatch);
                    spriteBatch.Draw(numBox, new Vector2(169, 83), Color.White);
                    DrawNumbers(spriteBatch, tempHeight, 170, 84);
                    spriteBatch.Draw(numBox, new Vector2(169, 123), Color.White);
                    DrawNumbers(spriteBatch, tempWidth, 170, 124);
                    spriteBatch.Draw(numBox, new Vector2(169, 163), Color.White);
                    DrawNumbers(spriteBatch, tempMines, 170, 164);
                    if (tempHeight > 9) spriteBatch.Draw(leftArrow, new Vector2(156, 85), Color.White);
                    if (tempHeight < 24) spriteBatch.Draw(rightArrow, new Vector2(212, 85), Color.White);
                    if (tempWidth > 9) spriteBatch.Draw(leftArrow, new Vector2(156, 125), Color.White);
                    if (tempWidth < 30) spriteBatch.Draw(rightArrow, new Vector2(212, 125), Color.White);
                    if (tempMines > 10) spriteBatch.Draw(leftArrow, new Vector2(156, 165), Color.White);
                    if (tempMines < (tempHeight - 1) * (tempWidth - 1)) spriteBatch.Draw(rightArrow, new Vector2(212, 165), Color.White);
                    break;
                case GameState.OptionsMenu:
                    optionsMenu.Draw(spriteBatch);
                    break;
                case GameState.BestTimesMenu:
                    bestTimesMenu.Draw(spriteBatch);
                    spriteBatch.DrawString(font, bestBeginner.ToString(), new Vector2(180, 72), Color.Black);
                    spriteBatch.DrawString(font, bestIntermediate.ToString(), new Vector2(180, 112), Color.Black);
                    spriteBatch.DrawString(font, bestExpert.ToString(), new Vector2(180, 152), Color.Black);
                    spriteBatch.DrawString(font, bestZune.ToString(), new Vector2(180, 192), Color.Black);
                    break;
            }
            spriteBatch.End();

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
                    if (height == 9 & width == 9 & mines == 10 & time < bestBeginner)
                    {
                        bestBeginner = time;
                        UpdateBestTime(Difficulty.Beginner);
                    }
                    if (height == 16 & width == 16 & mines == 40 & time < bestIntermediate)
                    {
                        bestIntermediate = time;
                        UpdateBestTime(Difficulty.Intermediate);
                    }
                    if (height == 24 & width == 30 & mines == 99 & time < bestExpert)
                    {
                        bestExpert = time;
                        UpdateBestTime(Difficulty.Expert);
                    }
                    if (height == 15 & width == 14 & mines == 30 & time < bestZune)
                    {
                        bestZune = time;
                        UpdateBestTime(Difficulty.Zune);
                    }
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

        protected void SetGame()
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
            gameState = GameState.NotPlaying;
            resumable = true;
        }

        protected void Resume()
        {
            gameState = oldGameState;
        }

        protected void NewGameClick()
        {
            gameState = GameState.NewGameMenu;
        }

        protected void OptionsClick()
        {
            gameState = GameState.OptionsMenu;
            oldCantSelectRevealed = cantSelectRevealed;
            oldFlagWithPlay = flagWithPlay;
        }

        protected void BestTimesClick()
        {
            gameState = GameState.BestTimesMenu;
        }

        protected void BeginnerClick()
        {
            height = 9;
            width = 9;
            mines = 10;
            SetGame();
        }

        protected void IntermediateClick()
        {
            height = 16;
            width = 16;
            mines = 40;
            SetGame();
        }

        protected void ExpertClick()
        {
            height = 24;
            width = 30;
            mines = 99;
            SetGame();
        }

        protected void ZuneClick()
        {
            height = 15;
            width = 14;
            mines = 30;
            SetGame();
        }

        protected void CustomClick()
        {
            gameState = GameState.CustomGameMenu;
            tempHeight = height;
            tempWidth = width;
            tempMines = mines;
        }

        protected void Back()
        {
            switch (gameState)
            {
                case GameState.NewGameMenu:
                    gameState = GameState.Menu;
                    break;
                case GameState.CustomGameMenu:
                    gameState = GameState.NewGameMenu;
                    break;
                case GameState.OptionsMenu:
                    if (oldCantSelectRevealed != cantSelectRevealed || oldFlagWithPlay != flagWithPlay) SetOptions();
                    gameState = GameState.Menu;
                    break;
                case GameState.BestTimesMenu:
                    gameState = GameState.Menu;
                    break;
            }
        }

        protected void CantSelectRevealedClick()
        {
            cantSelectRevealed = !cantSelectRevealed;
        }

        protected void FlagButtonClick()
        {
            flagWithPlay = !flagWithPlay;
        }

        protected void OKClick()
        {
            height = tempHeight;
            width = tempWidth;
            mines = tempMines;
            SetGame();
        }

        protected void ResetClick()
        {
            bestBeginner = 999;
            UpdateBestTime(Difficulty.Beginner);
            bestIntermediate = 999;
            UpdateBestTime(Difficulty.Intermediate);
            bestExpert = 999;
            UpdateBestTime(Difficulty.Expert);
            bestZune = 999;
            UpdateBestTime(Difficulty.Zune);
        }

        protected void DoNothing()
        {

        }

        protected void GetBestTimes()
        {
            string beginnerPath = Path.Combine(container.Path, "beginnertime.dat");
            string intermediatePath = Path.Combine(container.Path, "intermediatetime.dat");
            string expertPath = Path.Combine(container.Path, "experttime.dat");
            string zunePath = Path.Combine(container.Path, "zunetime.dat");

            BinaryReader dataFile;
            
            if (File.Exists(beginnerPath))
            {
                dataFile = new BinaryReader(new FileStream(beginnerPath, FileMode.Open));
                bestBeginner = dataFile.ReadInt32();
                dataFile.Close();
            }
            else UpdateBestTime(Difficulty.Beginner);
            
            if (File.Exists(intermediatePath))
            {
                dataFile = new BinaryReader(new FileStream(intermediatePath, FileMode.Open));
                bestIntermediate = dataFile.ReadInt32();
                dataFile.Close();
            }
            else UpdateBestTime(Difficulty.Intermediate);
            
            if (File.Exists(expertPath))
            {
                dataFile = new BinaryReader(new FileStream(expertPath, FileMode.Open));
                bestExpert = dataFile.ReadInt32();
                dataFile.Close();
            }
            else UpdateBestTime(Difficulty.Expert);
            
            if (File.Exists(zunePath))
            {
                dataFile = new BinaryReader(new FileStream(zunePath, FileMode.Open));
                bestZune = dataFile.ReadInt32();
                dataFile.Close();
            }
            else UpdateBestTime(Difficulty.Zune);
        }

        protected void UpdateBestTime(Difficulty difficulty)
        {
            string beginnerPath = Path.Combine(container.Path, "beginnertime.dat");
            string intermediatePath = Path.Combine(container.Path, "intermediatetime.dat");
            string expertPath = Path.Combine(container.Path, "experttime.dat");
            string zunePath = Path.Combine(container.Path, "zunetime.dat");

            BinaryWriter dataFile;
            switch (difficulty)
            {
                case Difficulty.Beginner:
                    dataFile = new BinaryWriter(new FileStream(beginnerPath, FileMode.Create));
                    dataFile.Write(bestBeginner);
                    dataFile.Close();
                    break;
                case Difficulty.Intermediate:
                    dataFile = new BinaryWriter(new FileStream(intermediatePath, FileMode.Create));
                    dataFile.Write(bestIntermediate);
                    dataFile.Close();
                    break;
                case Difficulty.Expert:
                    dataFile = new BinaryWriter(new FileStream(expertPath, FileMode.Create));
                    dataFile.Write(bestExpert);
                    dataFile.Close();
                    break;
                case Difficulty.Zune:
                    dataFile = new BinaryWriter(new FileStream(zunePath, FileMode.Create));
                    dataFile.Write(bestZune);
                    dataFile.Close();
                    break;
            }
            //container.Dispose();
        }

        protected void GetOptions()
        {
            if (File.Exists(Path.Combine(container.Path, "options.dat")))
            {
                BinaryReader dataFile;
                dataFile = new BinaryReader(new FileStream(Path.Combine(container.Path, "options.dat"), FileMode.Open));
                cantSelectRevealed = dataFile.ReadBoolean();
                flagWithPlay = dataFile.ReadBoolean();
                dataFile.Close();
            }
            else SetOptions();
        }

        protected void SetOptions()
        {
            BinaryWriter dataFile;
            dataFile = new BinaryWriter(new FileStream(Path.Combine(container.Path, "options.dat"), FileMode.Create));
            dataFile.Write(cantSelectRevealed);
            dataFile.Write(flagWithPlay);
            dataFile.Close();
        }
    }
}