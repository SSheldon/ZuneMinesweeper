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
using InputManagement;

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
        MenuComponent menuComponent;
        InputManager i;
        TouchInputManager tI;
        Skin s;
        public List<Skin> skins;
        public Texture2D blank;

        public int height;
        public int width;
        public int mines;
        public bool cantSelectRevealed;
        public bool flagWithPlay;
        public int bestBeginner, bestIntermediate, bestExpert, bestZune;
        public int selectedSkin;

        int flags;
        int time;
        double totalTime;
        int[] selectedTile = new int[2]; //tile in column selectedTile[0] of row selectedTile[1]
        int[] selectedMine = new int[2];
        int[] corner = new int[2];
        enum Face { Happy, Win, Dead, Scared };
        Face faceValue;
        public enum GameState { NotPlaying, Playing, Won, Lost, Menu };
        public GameState gameState;
        public GameState oldGameState;
        bool faceSelected;
        public bool resumable;
        bool scrollRight, scrollLeft, scrollUp, scrollDown;
        TimeSpan? lastScrollRight, lastScrollLeft, lastScrollUp, lastScrollDown;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            i = new InputManager(this);
            this.Components.Add(i);
            tI = new TouchInputManager(this);
            this.Components.Add(tI);
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
            storageDevice = Guide.EndShowStorageDeviceSelector(syncResult);
            //if (!storageDevice.IsConnected) Exit();
            container = storageDevice.OpenContainer("Minesweeper");
            bestBeginner = 999;
            bestIntermediate = 999;
            bestExpert = 999;
            bestZune = 999;
            GetBestTimes();

            tI.Enabled = tI.HasTouchpad;

            height = 9;
            width = 9;
            mines = 10;
            cantSelectRevealed = false;
            flagWithPlay = true;
            selectedSkin = 0;
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
            skins = new List<Skin>();
            scrollRight = false;
            lastScrollRight = null;

            GetOptions();
            InitializeInput();
            
            base.Initialize();
        }

        void InitializeInput()
        {
            i.RightPressed += new InputEventHandler(RightPress);
            i.LeftPressed += new InputEventHandler(LeftPress);
            i.DownPressed += new InputEventHandler(DownPress);
            i.UpPressed += new InputEventHandler(UpPress);
            i.CenterReleased += new InputEventHandler(CenterRelease);
            i.PlayReleased += new InputEventHandler(PlayRelease);
            i.BackReleased += new InputEventHandler(BackPress);
            i.HoldTime = 0.3F;
            i.RightHeld += new InputEventHandler(RightHold);
            i.RightHeldReleased += new InputEventHandler(RightUnhold);
            i.LeftHeld += new InputEventHandler(LeftHold);
            i.LeftHeldReleased += new InputEventHandler(LeftUnhold);
            i.UpHeld += new InputEventHandler(UpHold);
            i.UpHeldReleased += new InputEventHandler(UpUnhold);
            i.DownHeld += new InputEventHandler(DownHold);
            i.DownHeldReleased += new InputEventHandler(DownUnhold);
            tI.LeftToRightFlick += new FlickEventHandler(RightFlick);
            tI.RightToLeftFlick += new FlickEventHandler(LeftFlick);
            tI.DownToUpFlick += new FlickEventHandler(UpFlick);
            tI.UpToDownFlick += new FlickEventHandler(DownFlick);
        }

        void DownFlick(float duration, TouchpadPosition startPosition, TouchpadPosition endPosition)
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    if (!faceSelected)
                    {
                        selectedTile[1] = 0;
                        UpPress();
                        UpPress();
                    }
                    break;
            }
        }
        void UpFlick(float duration, TouchpadPosition startPosition, TouchpadPosition endPosition)
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    if (!faceSelected)
                    {
                        selectedTile[1] = height - 1;
                        DownPress();
                        DownPress();
                    }
                    break;
            }
        }
        void LeftFlick(float duration, TouchpadPosition startPosition, TouchpadPosition endPosition)
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    if (!faceSelected)
                    {
                        selectedTile[0] = width - 1;
                        RightPress();
                    }
                    break;
            }
        }
        void RightFlick(float duration, TouchpadPosition startPosition, TouchpadPosition endPosition)
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    if (!faceSelected)
                    {
                        selectedTile[0] = 0;
                        LeftPress();
                    }
                    break;
            }

        }
        void DownUnhold()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    scrollDown = false;
                    lastScrollDown = null;
                    break;
            }
        }
        void DownHold()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    scrollDown = true;
                    break;
            }
        }
        void UpUnhold()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    scrollUp = false;
                    lastScrollUp = null;
                    break;
            }
        }
        void UpHold()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    scrollUp = true;
                    break;
            }
        }
        void LeftUnhold()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    scrollLeft = false;
                    lastScrollLeft = null;
                    break;
            }
        }
        void LeftHold()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    //selectedTile[0] = 0;
                    scrollLeft = true;
                    break;
            }
        }
        void RightUnhold()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    scrollRight = false;
                    lastScrollRight = null;
                    break;
            }
        }
        void RightHold()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    //selectedTile[0] = width - 1;
                    scrollRight = true;
                    break;
            }
        }
        void BackPress()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                case GameState.Lost:
                case GameState.Won:
                    oldGameState = gameState;
                    gameState = GameState.Menu;
                    menuComponent.menuState = Menus.Main;
                    break;
                case GameState.Menu:
                    menuComponent.Back();
                    break;
            }
        }
        void PlayRelease()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    if (!faceSelected)
                        if (flagWithPlay) TileFlag();
                        else
                        {
                            if (mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
                            {
                                TileClick();
                            }
                            else
                            {
                                SurroundClick();
                            }
                        }
                    break;
            }
        }
        void CenterRelease()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
                    if (faceSelected) SetGame(height, width, mines);
                    else
                        if (flagWithPlay)
                        {
                            if (mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
                            {
                                TileClick();
                            }
                            else
                            {
                                SurroundClick();
                            }
                        }
                        else TileFlag();
                    break;
                case GameState.Lost:
                case GameState.Won:
                    SetGame(height, width, mines);
                    break;
                case GameState.Menu:
                    menuComponent.currentMenu.ClickItem();
                    break;
            }
        }
        void UpPress()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
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
                    break;
                case GameState.Menu:
                    menuComponent.currentMenu.UpClick();
                    break;
            }
                    
        }
        void DownPress()
        {
            switch (gameState)
            {
                case GameState.NotPlaying:
                case GameState.Playing:
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
                    break;
                case GameState.Menu:
                    menuComponent.currentMenu.DownClick();
                    break;
            }
        }
        void LeftPress()
        {
            switch (gameState)
            {
                case GameState.Playing:
                case GameState.NotPlaying:
                    if (!faceSelected)
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
                    break;
                case GameState.Menu:
                    menuComponent.LeftClick();
                    break;
            }
        }
        void RightPress()
        {
            switch (gameState)
            {
                case GameState.Playing:
                case GameState.NotPlaying:
                    if (!faceSelected)
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
                    break;
                case GameState.Menu:
                    menuComponent.RightClick();
                    break;
            }
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
            blank = Content.Load<Texture2D>("blank");
            foreach (String directory in Directory.GetDirectories(Path.Combine(StorageContainer.TitleLocation, Content.RootDirectory)))
            {
                Skin skin = Content.Load<Skin>(directory + "/skinfo");
                skin.InitializeTextures(directory, Content);
                skins.Add(skin);                
            }
            if (selectedSkin > skins.Count - 1) selectedSkin = 0;
            s = skins[selectedSkin];
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void BeginRun()
        {
            menuComponent = new MenuComponent(this, ref s);
            this.Components.Add(menuComponent);
            menuComponent.Enabled = false;
            menuComponent.Visible = false;
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

            s = skins[selectedSkin];
            menuComponent.Enabled = gameState == GameState.Menu;
            menuComponent.Visible = gameState == GameState.Menu;
            menuComponent.s = this.s;

            if (gameState == GameState.NotPlaying || gameState == GameState.Playing)
            {
                if (scrollRight && !i.RightIsPressed) RightUnhold();
                if (scrollLeft && !i.LeftIsPressed) LeftUnhold();
                if (scrollUp && !i.UpIsPressed) UpUnhold();
                if (scrollDown && !i.DownIsPressed) DownUnhold();

                if (scrollRight)
                {
                    if (!lastScrollRight.HasValue)
                    {
                        RightPress();
                        lastScrollRight = gameTime.TotalGameTime;
                    }
                    else
                    {
                        if (Convert.ToSingle(gameTime.TotalGameTime.Subtract(lastScrollRight.Value).TotalSeconds) >= 0.15F)
                        {
                            RightPress();
                            lastScrollRight = gameTime.TotalGameTime;
                        }
                    }
                }
                if (scrollLeft)
                {
                    if (!lastScrollLeft.HasValue)
                    {
                        LeftPress();
                        lastScrollLeft = gameTime.TotalGameTime;
                    }
                    else
                    {
                        if (Convert.ToSingle(gameTime.TotalGameTime.Subtract(lastScrollLeft.Value).TotalSeconds) >= 0.15F)
                        {
                            LeftPress();
                            lastScrollLeft = gameTime.TotalGameTime;
                        }
                    }
                }
                if (scrollUp)
                {
                    if (!lastScrollUp.HasValue)
                    {
                        UpPress();
                        lastScrollUp = gameTime.TotalGameTime;
                    }
                    else
                    {
                        if (Convert.ToSingle(gameTime.TotalGameTime.Subtract(lastScrollUp.Value).TotalSeconds) >= 0.15F)
                        {
                            UpPress();
                            lastScrollUp = gameTime.TotalGameTime;
                        }
                    }
                }
                if (scrollDown)
                {
                    if (!lastScrollDown.HasValue)
                    {
                        DownPress();
                        lastScrollDown = gameTime.TotalGameTime;
                    }
                    else
                    {
                        if (Convert.ToSingle(gameTime.TotalGameTime.Subtract(lastScrollDown.Value).TotalSeconds) >= 0.15F)
                        {
                            DownPress();
                            lastScrollDown = gameTime.TotalGameTime;
                        }
                    }
                }

                if (!faceSelected)
                {
                    if ((flagWithPlay && i.CenterIsPressed) || (!flagWithPlay && i.PlayIsPressed)) faceValue = Face.Scared;
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
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(s.background);

            // TODO: Add your drawing code here
            if (gameState != GameState.Menu)
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

            base.Draw(gameTime);
        }

        void DrawBackground(SpriteBatch batch)
        {
            batch.Draw(s.top, new Rectangle(0, 0, 240, 56), Color.White);
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
                batch.Draw(s.numbers[amountNums[0]], new Rectangle(x, y, 13, 23), Color.White);
                batch.Draw(s.numbers[amountNums[1]], new Rectangle(x + 13, y, 13, 23), Color.White);
                batch.Draw(s.numbers[amountNums[2]], new Rectangle(x + 26, y, 13, 23), Color.White);
            }
            else
            {
                char[] amountParts = new char[10];
                amountParts = amount.ToString().ToCharArray();
                if (amount < 0 & amount > -10) amountNums[1] = 0;
                else amountNums[1] = int.Parse(amountParts[amountParts.GetUpperBound(0) - 1].ToString());
                amountNums[2] = int.Parse(amountParts[amountParts.GetUpperBound(0)].ToString());
                if (amount < 0 & amount > -10) amountNums[1] = 0;
                batch.Draw(s.numbers[10], new Rectangle(x, y, 13, 23), Color.White);
                batch.Draw(s.numbers[amountNums[1]], new Rectangle(x + 13, y, 13, 23), Color.White);
                batch.Draw(s.numbers[amountNums[2]], new Rectangle(x + 26, y, 13, 23), Color.White);
            }
        }

        void DrawFace(SpriteBatch batch)
        {
            if (faceValue == Face.Happy) batch.Draw(s.fHappy, new Rectangle(108, 16, 24, 24), Color.White);
            else
                if (faceValue == Face.Win) batch.Draw(s.fWin, new Rectangle(108, 16, 24, 24), Color.White);
                else
                    if (faceValue == Face.Dead) batch.Draw(s.fDead, new Rectangle(108, 16, 24, 24), Color.White);
                    else batch.Draw(s.fScared, new Rectangle(108, 16, 24, 24), Color.White);
            if (faceSelected) batch.Draw(s.faceSelect, new Rectangle(108, 16, 24, 24), Color.White);
        }

        void DrawFieldBorder(SpriteBatch batch)
        {
            batch.Draw(s.borderTL, new Rectangle(0 - corner[0] * 16, 56 - corner[1] * 16, 8, 8), Color.White);
            batch.Draw(s.borderT, new Rectangle(8 - corner[0] * 16, 56 - corner[1] * 16, 16 * width, 8), Color.White);
            batch.Draw(s.borderTR, new Rectangle(8 + 16 * width - corner[0] * 16, 56 - corner[1] * 16, 8, 8), Color.White);
            batch.Draw(s.borderL, new Rectangle(0 - corner[0] * 16, 64 - corner[1] * 16, 8, 16 * height), Color.White);
            batch.Draw(s.borderR, new Rectangle(8 + 16 * width - corner[0] * 16, 64 - corner[1] * 16, 8, 16 * height), Color.White);
            batch.Draw(s.borderBL, new Rectangle(0 - corner[0] * 16, 64 + 16 * height - corner[1] * 16, 8, 8), Color.White);
            batch.Draw(s.borderB, new Rectangle(8 - corner[0] * 16, 64 + 16 * height - corner[1] * 16, 16 * width, 8), Color.White);
            batch.Draw(s.borderBR, new Rectangle(8 + 16 * width - corner[0] * 16, 64 + 16 * height - corner[1] * 16, 8, 8), Color.White);
        }

        void DrawTileSelect(SpriteBatch batch)
        {
            if (!faceSelected) batch.Draw(s.select, new Rectangle(8 + selectedTile[0] * 16 - corner[0] * 16, 64 + selectedTile[1] * 16 - corner[1] * 16, 16, 16), Color.White);
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
                        if (mGame.tile[row, col].Flagged) tile = s.tFlag;
                        else
                            if (mGame.tile[row, col].Hidden) tile = s.tHidden;
                            else tile = s.t[mGame.tile[row, col].TileNum];
                    }
                    else
                    {
                        if (mGame.tile[row, col].Flagged & !mGame.tile[row, col].MineHere) tile = s.tNotMine;
                        else
                            if (mGame.tile[row, col].Flagged) tile = s.tFlag;
                            else
                                if (mGame.tile[row, col].Hidden) tile = s.tHidden;
                                else
                                    if (row == selectedMine[1] && col == selectedMine[0] && gameState == GameState.Lost) tile = s.tClickedMine;
                                    else
                                        if (mGame.tile[row, col].MineHere) tile = s.tMine;
                                        else tile = s.t[mGame.tile[row, col].TileNum];
                    }
                    batch.Draw(tile, new Rectangle(8 + col * 16 - corner[0] * 16, 64 + row * 16 - corner[1] * 16, 16, 16), Color.White);
                }
            }
        }

        void TileClick()
        {
            if (gameState == GameState.NotPlaying)
            {
                if (mGame.tile[selectedTile[1], selectedTile[0]].MineHere)
                {
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            if (!mGame.tile[row, col].MineHere)
                            {
                                mGame.tile[row, col].MineHere = true;
                                mGame.tile[selectedTile[1], selectedTile[0]].MineHere = false;
                                break;
                            }
                        }
                        if (!mGame.tile[selectedTile[1], selectedTile[0]].MineHere) break;
                    }
                    mGame.GenerateTileNums();
                }
            }
            if (gameState != GameState.Playing) gameState = GameState.Playing;
            switch (mGame.SelectedAction(selectedTile[1], selectedTile[0]))
            {
                case 0: //Game continues
                    gameState = GameState.Playing;
                    faceValue = Face.Happy;
                    if (cantSelectRevealed)
                    {
                        bool foundGoodTile = false;
                        int[] tempSelectedTile = new int[2];
                        for (int counter = 1; counter < height | counter < width; counter++)
                        {
                            int row = selectedTile[1] - counter;
                            int col = selectedTile[0] - counter;
                            for (; col <= selectedTile[0] + counter; col++)
                            {
                                if (row >= 0 && row < height && col >= 0 && col < width)
                                    if (mGame.tile[row, col].Hidden)
                                    {
                                        tempSelectedTile[1] = row;
                                        tempSelectedTile[0] = col;
                                        if (!mGame.tile[tempSelectedTile[1], tempSelectedTile[0]].Flagged) foundGoodTile = true;
                                        if (foundGoodTile) break;
                                    }
                            }
                            if (foundGoodTile) break;
                            col--;
                            row++;
                            for (; row <= selectedTile[1] + counter; row++)
                            {
                                if (row >= 0 && row < height && col >= 0 && col < width)
                                    if (mGame.tile[row, col].Hidden)
                                    {
                                        tempSelectedTile[1] = row;
                                        tempSelectedTile[0] = col;
                                        if (!mGame.tile[tempSelectedTile[1], tempSelectedTile[0]].Flagged) foundGoodTile = true;
                                        if (foundGoodTile) break;
                                    }
                            }
                            if (foundGoodTile) break;
                            row--;
                            col--;
                            for (; col >= selectedTile[0] - counter; col--)
                            {
                                if (row >= 0 && row < height && col >= 0 && col < width)
                                    if (mGame.tile[row, col].Hidden)
                                    {
                                        tempSelectedTile[1] = row;
                                        tempSelectedTile[0] = col;
                                        if (!mGame.tile[tempSelectedTile[1], tempSelectedTile[0]].Flagged) foundGoodTile = true;
                                        if (foundGoodTile) break;
                                    }
                            }
                            if (foundGoodTile) break;
                            col++;
                            row--;
                            for (; row >= selectedTile[1] - counter + 1; row--)
                            {
                                if (row >= 0 && row < height && col >= 0 && col < width)
                                    if (mGame.tile[row, col].Hidden)
                                    {
                                        tempSelectedTile[1] = row;
                                        tempSelectedTile[0] = col;
                                        if (!mGame.tile[tempSelectedTile[1], tempSelectedTile[0]].Flagged) foundGoodTile = true;
                                        if (foundGoodTile) break;
                                    }
                            }
                            if (foundGoodTile) break;
                            row++;
                        }
                        selectedTile = tempSelectedTile;
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

        void TileFlag()
        {
            //if (gameState != GameState.Playing) gameState = GameState.Playing;
            if (mGame.tile[selectedTile[1], selectedTile[0]].Hidden)
            {
                if (!(mGame.tile[selectedTile[1], selectedTile[0]].Flagged))
                {
                    mGame.tile[selectedTile[1], selectedTile[0]].Flag();
                    flags--;
                }
                else
                {
                    mGame.tile[selectedTile[1], selectedTile[0]].Unflag();
                    flags++;
                }
            }
        }

        void SurroundClick()
        {
            int surroundingFlags = 0;

            if (!(selectedTile[1] == 0)) 
                if (mGame.tile[(selectedTile[1] - 1), selectedTile[0]].Flagged) surroundingFlags++;
            if (!(selectedTile[0] == 0)) 
                if (mGame.tile[selectedTile[1], (selectedTile[0] - 1)].Flagged) surroundingFlags++;
            if (!(selectedTile[1] == 0) & !(selectedTile[0] == 0)) 
                if (mGame.tile[(selectedTile[1] - 1), (selectedTile[0] - 1)].Flagged) surroundingFlags++;
            if (!(selectedTile[0] == width - 1)) 
                if (mGame.tile[selectedTile[1], (selectedTile[0] + 1)].Flagged) surroundingFlags++;
            if (!(selectedTile[1] == 0) & !(selectedTile[0] == width - 1)) 
                if (mGame.tile[(selectedTile[1] - 1), (selectedTile[0] + 1)].Flagged) surroundingFlags++;
            if (!(selectedTile[1] == height - 1)) 
                if (mGame.tile[(selectedTile[1] + 1), selectedTile[0]].Flagged) surroundingFlags++;
            if (!(selectedTile[1] == height - 1) & !(selectedTile[0] == 0)) 
                if (mGame.tile[(selectedTile[1] + 1), (selectedTile[0] - 1)].Flagged) surroundingFlags++;
            if (!(selectedTile[1] == height - 1) & !(selectedTile[0] == width - 1))
                if (mGame.tile[(selectedTile[1] + 1), (selectedTile[0] + 1)].Flagged) surroundingFlags++;

            if (surroundingFlags == mGame.tile[selectedTile[1], selectedTile[0]].TileNum)
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

        public void SetGame(int height, int width, int mines)
        {
            this.height = height;
            this.width = width;
            this.mines = mines;
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

        public void GetBestTimes()
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

        public void UpdateBestTime(Difficulty difficulty)
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
        }

        public void GetOptions()
        {
            if (File.Exists(Path.Combine(container.Path, "options.dat")))
            {
                BinaryReader dataFile;
                dataFile = new BinaryReader(new FileStream(Path.Combine(container.Path, "options.dat"), FileMode.Open));
                cantSelectRevealed = dataFile.ReadBoolean();
                flagWithPlay = dataFile.ReadBoolean();
                selectedSkin = dataFile.ReadInt32();
                dataFile.Close();
            }
            else SetOptions();
        }

        public void SetOptions()
        {
            BinaryWriter dataFile;
            dataFile = new BinaryWriter(new FileStream(Path.Combine(container.Path, "options.dat"), FileMode.Create));
            dataFile.Write(cantSelectRevealed);
            dataFile.Write(flagWithPlay);
            dataFile.Write(selectedSkin);
            dataFile.Close();
        }
    }
}