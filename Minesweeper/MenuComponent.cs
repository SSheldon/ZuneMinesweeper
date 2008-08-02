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

namespace Minesweeper
{
    class MenuComponent : DrawableGameComponent
    {
        Game1 game;
        SpriteBatch spriteBatch;
        public Menu currentMenu;
        GamePadState newPadState;
        public Skin s;
        public Menus menuState;
        int tempHeight, tempWidth, tempMines;
        int tempSelectedSkin, oldSelectedSkin;
        bool oldCantSelectRevealed, oldFlagWithPlay;

        Menu mainMenu;
        MenuItem resume, newGame, music, bestTimes, options, exit;
        Menu newGameMenu;
        MenuItem beginner, intermediate, expert, zune, custom, back;
        Menu optionsMenu;
        MenuItem cantSelectRevealedMI, flagButton, changeSkin;
        Menu customGameMenu;
        MenuItem heightMI, widthMI, minesMI, ok;
        Menu bestTimesMenu;
        MenuItem bestBeginnerMI, bestIntermediateMI, bestExpertMI, bestZuneMI, resetMI;
        Menu skinMenu;
        MenuItem skinMI, change;

        public MenuComponent(Game1 game, ref Skin skin) : base(game)
        {
            this.game = game;
            this.s = skin;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization logic here
            menuState = Menus.Main;
            InitializeMenus();
            tempSelectedSkin = game.selectedSkin;

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
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        void InitializeMenus()
        {
            mainMenu = new Menu(s.mTop);
            resume = new MenuItem("Resume", game.resumable);
            resume.itemClicked += new ItemClick(Resume);
            mainMenu.Add(0, ref resume);
            newGame = new MenuItem("New Game");
            newGame.itemClicked += new ItemClick(NewGameClick);
            mainMenu.Add(1, ref newGame);
            music = new MenuItem("Music", false);
            music.itemClicked += new ItemClick(DoNothing);
            mainMenu.Add(2, ref music);
            bestTimes = new MenuItem("Best Times");
            bestTimes.itemClicked += new ItemClick(BestTimesClick);
            mainMenu.Add(3, ref bestTimes);
            options = new MenuItem("Options");
            options.itemClicked += new ItemClick(OptionsClick);
            mainMenu.Add(4, ref options);
            exit = new MenuItem("Exit");
            exit.itemClicked += new ItemClick(game.Exit);
            mainMenu.Add(5, ref exit);

            newGameMenu = new Menu("New Game:");
            beginner = new MenuItem("Beginner");
            beginner.itemClicked += new ItemClick(BeginnerClick);
            newGameMenu.Add(0, ref beginner);
            intermediate = new MenuItem("Intermediate");
            intermediate.itemClicked += new ItemClick(IntermediateClick);
            newGameMenu.Add(1, ref intermediate);
            expert = new MenuItem("Expert");
            expert.itemClicked += new ItemClick(ExpertClick);
            newGameMenu.Add(2, ref expert);
            zune = new MenuItem("Zune Fit");
            zune.itemClicked += new ItemClick(ZuneClick);
            newGameMenu.Add(3, ref zune);
            custom = new MenuItem("Custom");
            custom.itemClicked += new ItemClick(CustomClick);
            newGameMenu.Add(4, ref custom);
            back = new MenuItem("Back");
            back.itemClicked += new ItemClick(Back);
            newGameMenu.Add(5, ref back);

            optionsMenu = new Menu("Options:");
            cantSelectRevealedMI = new MenuItem("Revealed tiles can't be selected", true, true, true);
            cantSelectRevealedMI.itemClicked += new ItemClick(CantSelectRevealedClick);
            optionsMenu.Add(0, ref cantSelectRevealedMI);
            flagButton = new MenuItem("Flag tiles with Play button", true, true, true);
            flagButton.itemClicked += new ItemClick(FlagButtonClick);
            optionsMenu.Add(1, ref flagButton);
            changeSkin = new MenuItem("Change Skin");
            changeSkin.itemClicked += new ItemClick(ChangeSkinClick);
            optionsMenu.Add(2, ref changeSkin);
            optionsMenu.Add(5, ref back);

            customGameMenu = new Menu("Custom Game:");
            heightMI = new MenuItem("Height");
            heightMI.itemClicked += new ItemClick(DoNothing);
            customGameMenu.Add(0, ref heightMI);
            widthMI = new MenuItem("Width");
            widthMI.itemClicked += new ItemClick(DoNothing);
            customGameMenu.Add(1, ref widthMI);
            minesMI = new MenuItem("Mines");
            minesMI.itemClicked += new ItemClick(DoNothing);
            customGameMenu.Add(2, ref minesMI);
            ok = new MenuItem("OK");
            ok.itemClicked += new ItemClick(OKClick);
            customGameMenu.Add(4, ref ok);
            customGameMenu.Add(5, ref back);

            bestTimesMenu = new Menu("Best Times:");
            bestBeginnerMI = new MenuItem("Beginner:", false, true);
            bestTimesMenu.Add(0, ref bestBeginnerMI);
            bestIntermediateMI = new MenuItem("Intermed.:", false, true);
            bestTimesMenu.Add(1, ref bestIntermediateMI);
            bestExpertMI = new MenuItem("Expert:", false, true);
            bestTimesMenu.Add(2, ref bestExpertMI);
            bestZuneMI = new MenuItem("Zune Fit:", false, true);
            bestTimesMenu.Add(3, ref bestZuneMI);
            resetMI = new MenuItem("Reset Times");
            resetMI.itemClicked += new ItemClick(ResetClick);
            bestTimesMenu.Add(4, ref resetMI);
            bestTimesMenu.Add(5, ref back);

            skinMenu = new Menu("Skin:");
            skinMI = new MenuItem("Classic");
            skinMI.itemClicked += new ItemClick(DoNothing);
            skinMenu.Add(0, ref skinMI);
            change = new MenuItem("Change");
            change.itemClicked += new ItemClick(ChangeClick);
            skinMenu.Add(4, ref change);
            skinMenu.Add(5, ref back);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            mainMenu.top = s.mTop;
            resume.selectable = game.resumable;
            resume.colored = game.resumable;
            cantSelectRevealedMI.text = game.cantSelectRevealed ? "Revealed tiles can't be selected" : "Revealed tiles can be selected";
            flagButton.text = game.flagWithPlay ? "Flag tiles with Play button" : "Flag tiles with Center button";
            skinMI.text = game.skins[tempSelectedSkin].name;

            if (menuState == Menus.CustomGame && tempMines > (tempHeight - 1) * (tempWidth - 1)) tempMines = (tempHeight - 1) * (tempWidth - 1);

            GamePadState oldPadState = newPadState;
            newPadState = GamePad.GetState(PlayerIndex.One);

            switch (menuState)
            {
                case Menus.Main:
                    currentMenu = mainMenu;
                    break;
                case Menus.NewGame:
                    currentMenu = newGameMenu;
                    break;
                case Menus.CustomGame:
                    currentMenu = customGameMenu;
                    break;
                case Menus.Options:
                    currentMenu = optionsMenu;
                    break;
                case Menus.BestTimes:
                    currentMenu = bestTimesMenu;
                    break;
                case Menus.Skin:
                    currentMenu = skinMenu;
                    break;
            }

            while (currentMenu.items[currentMenu.selectedItem] == null || !currentMenu.items[currentMenu.selectedItem].selectable)
            {
                if (currentMenu.selectedItem == 5) currentMenu.selectedItem = 0;
                else currentMenu.selectedItem++;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (s.headersBacked) spriteBatch.Draw(s.headerBack, new Vector2(0, 0), Color.White);
            if (currentMenu.top != null) spriteBatch.Draw(currentMenu.top, new Vector2(0, 0), Color.White);
            else spriteBatch.DrawString(s.header, currentMenu.title, new Vector2(4, 16), s.coloredText);
            for (int counter = 0; counter < 6; counter++)
            {
                if (currentMenu.items[counter] != null)
                {
                    if (s.itemsBacked)
                        if (!(menuState == Menus.CustomGame && (counter == 0 || counter == 1 || counter == 2)) && !(menuState == Menus.BestTimes && (counter == 0 || counter == 1 || counter == 2 || counter == 3)))
                            spriteBatch.Draw(s.itemBack, new Vector2(14, 78 + counter * 40), Color.White);
                    Color fontColor;
                    SpriteFont font;
                    fontColor = currentMenu.items[counter].colored ? s.coloredText : s.uncoloredText;
                    font = currentMenu.items[counter].smallFont ? s.small : s.normal;
                    int y;
                    if (currentMenu.items[counter].smallFont) y = 12;
                    else y = 0;
                    spriteBatch.DrawString(font, currentMenu.items[counter].text, new Vector2(16, 72 + counter * 40 + y), fontColor);
                }
            }

            if (menuState == Menus.CustomGame)
            {
                spriteBatch.Draw(s.numBox, new Vector2(162, 76), Color.White);
                game.DrawNumbers(spriteBatch, tempHeight, 170, 84);
                spriteBatch.Draw(s.numBox, new Vector2(162, 116), Color.White);
                game.DrawNumbers(spriteBatch, tempWidth, 170, 124);
                spriteBatch.Draw(s.numBox, new Vector2(162, 156), Color.White);
                game.DrawNumbers(spriteBatch, tempMines, 170, 164);
            }

            if (menuState == Menus.BestTimes)
            {
                spriteBatch.Draw(s.numBox, new Vector2(170, 76), Color.White);
                game.DrawNumbers(spriteBatch, game.bestBeginner, 178, 84);
                spriteBatch.Draw(s.numBox, new Vector2(170, 116), Color.White);
                game.DrawNumbers(spriteBatch, game.bestIntermediate, 178, 124);
                spriteBatch.Draw(s.numBox, new Vector2(170, 156), Color.White);
                game.DrawNumbers(spriteBatch, game.bestExpert, 178, 164);
                spriteBatch.Draw(s.numBox, new Vector2(170, 196), Color.White);
                game.DrawNumbers(spriteBatch, game.bestZune, 178, 204);
            }

            if (menuState == Menus.Skin)
            {
                spriteBatch.Draw(s.leftArrow, new Vector2(1, 85), Color.White);
                spriteBatch.Draw(s.rightArrow, new Vector2(14 + 212 + 2, 85), Color.White);
                spriteBatch.DrawString(s.normal, "By " + game.skins[tempSelectedSkin].creator, new Vector2(14, 118), s.coloredText);
                DrawSkinSample(spriteBatch);                 
            }

            if (menuState == Menus.CustomGame && (currentMenu.selectedItem == 0 || currentMenu.selectedItem == 1 || currentMenu.selectedItem == 2))
            {
                switch (currentMenu.selectedItem)
                {
                    case 0:
                        if (tempHeight > 9) spriteBatch.Draw(s.leftArrow, new Vector2(156, 85), Color.White);
                        if (tempHeight < 24) spriteBatch.Draw(s.rightArrow, new Vector2(212, 85), Color.White);
                        break;
                    case 1:
                        if (tempWidth > 9) spriteBatch.Draw(s.leftArrow, new Vector2(156, 125), Color.White);
                        if (tempWidth < 30) spriteBatch.Draw(s.rightArrow, new Vector2(212, 125), Color.White);
                        break;
                    case 2:
                        if (tempMines > 10) spriteBatch.Draw(s.leftArrow, new Vector2(156, 165), Color.White);
                        if (tempMines < (tempHeight - 1) * (tempWidth - 1)) spriteBatch.Draw(s.rightArrow, new Vector2(212, 165), Color.White);
                        break;
                }
            }
            else spriteBatch.Draw(s.mSelect, new Vector2(14, 78 + currentMenu.selectedItem * 40), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawSkinSample(SpriteBatch spriteBatch)
        {
            Skin s = game.skins[tempSelectedSkin];
            spriteBatch.Draw(game.blank, new Rectangle(22, 166, 196, 64), s.background);
            spriteBatch.Draw(s.numBox, new Vector2(30, 191), Color.White);
            spriteBatch.Draw(s.numbers[10], new Vector2(38, 199), Color.White);
            spriteBatch.Draw(s.numbers[4], new Vector2(51, 199), Color.White);
            spriteBatch.Draw(s.numbers[9], new Vector2(64, 199), Color.White);
            spriteBatch.Draw(s.tHidden, new Vector2(30, 174), Color.White);
            spriteBatch.Draw(s.tFlag, new Vector2(46, 174), Color.White);
            spriteBatch.Draw(s.tMine, new Vector2(62, 174), Color.White);
            spriteBatch.Draw(s.tClickedMine, new Vector2(78, 174), Color.White);
            spriteBatch.Draw(s.tNotMine, new Vector2(94, 174), Color.White);
            for (int counter = 0; counter < 7; counter++)
            {
                spriteBatch.Draw(s.t[counter], new Vector2(94 + 16 * counter, 174), Color.White);
            }
            spriteBatch.Draw(s.t[7], new Vector2(190, 190), Color.White);
            spriteBatch.Draw(s.t[8], new Vector2(190, 206), Color.White);
            spriteBatch.Draw(s.fHappy, new Vector2(110 - 24, 194), Color.White);
            spriteBatch.Draw(s.fScared, new Vector2(134 - 24, 194), Color.White);
            spriteBatch.Draw(s.fDead, new Vector2(158 - 24, 194), Color.White);
            spriteBatch.Draw(s.fWin, new Vector2(182 - 24, 194), Color.White);
        }

        void Resume()
        {
            game.gameState = game.oldGameState;
            this.Enabled = false;
            this.Visible = false;
        }

        void NewGameClick()
        {
            menuState = Menus.NewGame;
        }

        void OptionsClick()
        {
            menuState = Menus.Options;
            oldCantSelectRevealed = game.cantSelectRevealed;
            oldFlagWithPlay = game.flagWithPlay;
            oldSelectedSkin = game.selectedSkin;
        }

        void BestTimesClick()
        {
            menuState = Menus.BestTimes;
        }

        void BeginnerClick()
        {
            game.SetGame(9, 9, 10);
        }

        void IntermediateClick()
        {
            game.SetGame(16, 16, 40);
        }

        void ExpertClick()
        {
            game.SetGame(24, 30, 99);
        }

        void ZuneClick()
        {
            game.SetGame(15, 14, 30);
        }

        void CustomClick()
        {
            menuState = Menus.CustomGame;
            tempHeight = game.height;
            tempWidth = game.width;
            tempMines = game.mines;
        }

        public void Back()
        {
            switch (menuState)
            {
                case Menus.Main:
                    if (game.resumable) Resume();
                    break;
                case Menus.NewGame:
                    menuState = Menus.Main;
                    break;
                case Menus.CustomGame:
                    menuState = Menus.NewGame;
                    break;
                case Menus.Options:
                    if (oldCantSelectRevealed != game.cantSelectRevealed || oldFlagWithPlay != game.flagWithPlay || oldSelectedSkin != game.selectedSkin) game.SetOptions();
                    menuState = Menus.Main;
                    break;
                case Menus.BestTimes:
                    menuState = Menus.Main;
                    break;
                case Menus.Skin:
                    menuState = Menus.Options;
                    break;
            }
        }


        public void RightClick()
        {
            switch (menuState)
            {
                case Menus.CustomGame:
                    switch (currentMenu.selectedItem)
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
                    break;
                case Menus.Skin:
                    if (currentMenu.selectedItem == 0)
                    {
                        if (tempSelectedSkin == game.skins.Count - 1) tempSelectedSkin = 0;
                        else tempSelectedSkin++;
                    }
                    break;
            }
        }

        public void LeftClick()
        {
            switch (menuState)
            {
                case Menus.CustomGame:
                    switch (currentMenu.selectedItem)
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
                    break;
                case Menus.Skin:
                    if (currentMenu.selectedItem == 0)
                    {
                        if (tempSelectedSkin == 0) tempSelectedSkin = game.skins.Count - 1;
                        else tempSelectedSkin--;
                    }
                    break;
            }
        }

        void CantSelectRevealedClick()
        {
            game.cantSelectRevealed = !game.cantSelectRevealed;
        }

        void FlagButtonClick()
        {
            game.flagWithPlay = !game.flagWithPlay;
        }

        void ChangeSkinClick()
        {
            menuState = Menus.Skin;
            tempSelectedSkin = game.selectedSkin;
        }

        void OKClick()
        {
            game.SetGame(tempHeight, tempWidth, tempMines);
        }

        void ResetClick()
        {
            game.bestBeginner = 999;
            game.UpdateBestTime(Difficulty.Beginner);
            game.bestIntermediate = 999;
            game.UpdateBestTime(Difficulty.Intermediate);
            game.bestExpert = 999;
            game.UpdateBestTime(Difficulty.Expert);
            game.bestZune = 999;
            game.UpdateBestTime(Difficulty.Zune);
        }

        void ChangeClick()
        {
            game.selectedSkin = tempSelectedSkin;
        }

        void DoNothing()
        {

        }
    }
}