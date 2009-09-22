using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;

namespace Minesweeper
{
    public enum Difficulty { Beginner, Intermediate, Expert, Zune }

    /// <summary>
    /// This is the main type for your game.  All of the logic for the game is
    /// handled inside of a GameScreen, so Game1 is just used to setup the 
    /// starting screens.
    /// </summary>
    public class MinesweeperGame : Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        StorageDevice storageDevice;
        StorageContainer container;
        SpriteFont normal, header, small;
        public Dictionary<string, Skin> skins;
        public Dictionary<Difficulty, int> bestTimes;
        public Skin Skin
        {
            get { return skins[options.SelectedSkin]; }
        }
        public Options options;

        public MinesweeperGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 240;
            graphics.PreferredBackBufferHeight = 320;

            Content.RootDirectory = "Content";

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
        }

        protected override void Initialize()
        {
            IAsyncResult syncResult = Guide.BeginShowStorageDeviceSelector(null, null);
            storageDevice = Guide.EndShowStorageDeviceSelector(syncResult);
            //if (!storageDevice.IsConnected) Exit();
            container = storageDevice.OpenContainer("Minesweeper");

            skins = new Dictionary<string, Skin>();
            bestTimes = new Dictionary<Difficulty, int>(4);
            bestTimes.Add(Difficulty.Beginner, 999);
            bestTimes.Add(Difficulty.Intermediate, 999);
            bestTimes.Add(Difficulty.Expert, 999);
            bestTimes.Add(Difficulty.Zune, 999);
            GetBestTimes();

            options = new Options(false, true, false, "Blue");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            normal = Content.Load<SpriteFont>("normal");
            header = Content.Load<SpriteFont>("header");
            small = Content.Load<SpriteFont>("small");
            foreach (String directory in Directory.GetDirectories(Path.Combine(StorageContainer.TitleLocation, Content.RootDirectory)))
            {
                Skin skin = Content.Load<Skin>(directory + "/skinfo");
                skin.InitializeTextures(directory, Content, normal, header, small);
                skins.Add(skin.name, skin);
            }

            GetOptions();
            if (!skins.ContainsKey(options.SelectedSkin)) options.SelectedSkin = "Blue";
        }

        protected override void BeginRun()
        {
            screenManager.AddScreen(new GameplayScreen(this));
            screenManager.AddScreen(new MainMenuScreen(this, false));
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
                bestTimes[Difficulty.Beginner] = dataFile.ReadInt32();
                dataFile.Close();
            }
            else UpdateBestTime(Difficulty.Beginner);

            if (File.Exists(intermediatePath))
            {
                dataFile = new BinaryReader(new FileStream(intermediatePath, FileMode.Open));
                bestTimes[Difficulty.Intermediate] = dataFile.ReadInt32();
                dataFile.Close();
            }
            else UpdateBestTime(Difficulty.Intermediate);

            if (File.Exists(expertPath))
            {
                dataFile = new BinaryReader(new FileStream(expertPath, FileMode.Open));
                bestTimes[Difficulty.Expert] = dataFile.ReadInt32();
                dataFile.Close();
            }
            else UpdateBestTime(Difficulty.Expert);

            if (File.Exists(zunePath))
            {
                dataFile = new BinaryReader(new FileStream(zunePath, FileMode.Open));
                bestTimes[Difficulty.Zune] = dataFile.ReadInt32();
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
                    dataFile.Write(bestTimes[Difficulty.Beginner]);
                    dataFile.Close();
                    break;
                case Difficulty.Intermediate:
                    dataFile = new BinaryWriter(new FileStream(intermediatePath, FileMode.Create));
                    dataFile.Write(bestTimes[Difficulty.Intermediate]);
                    dataFile.Close();
                    break;
                case Difficulty.Expert:
                    dataFile = new BinaryWriter(new FileStream(expertPath, FileMode.Create));
                    dataFile.Write(bestTimes[Difficulty.Expert]);
                    dataFile.Close();
                    break;
                case Difficulty.Zune:
                    dataFile = new BinaryWriter(new FileStream(zunePath, FileMode.Create));
                    dataFile.Write(bestTimes[Difficulty.Zune]);
                    dataFile.Close();
                    break;
            }
        }

        public void GetOptions()
        {
            if (File.Exists(Path.Combine(container.Path, "options.dat")))
                File.Delete(Path.Combine(container.Path, "options.dat"));
            if (File.Exists(Path.Combine(container.Path, "options.txt")))
            {
                StreamReader reader = new StreamReader(new FileStream(Path.Combine(container.Path, "options.txt"), FileMode.Open));
                try
                {
                    options.CantSelectRevealed = bool.Parse(reader.ReadLine());
                    options.FlagWithPlay = bool.Parse(reader.ReadLine());
                    options.SelectedSkin = reader.ReadLine();
                    options.UseTouch = bool.Parse(reader.ReadLine());
                    reader.Close();
                }
                catch
                {
                    reader.Close();
                    UpdateOptions();
                }
            }
            else UpdateOptions();
        }

        public void UpdateOptions()
        {
            StreamWriter writer = new StreamWriter(new FileStream(Path.Combine(container.Path, "options.txt"), FileMode.Create));
            writer.WriteLine(options.CantSelectRevealed);
            writer.WriteLine(options.FlagWithPlay);
            writer.WriteLine(options.SelectedSkin);
            writer.WriteLine(options.UseTouch);
            writer.Flush();
            writer.Close();
        }

        public void ExitAllMenuScreens()
        {
            GameScreen[] screens = screenManager.GetScreens();
            for (int i = screens.Length - 1; i >= 0; i--)
            {
                if (screens[i] is MenuScreen)
                {
                    screens[i].ExitScreen();
                }
            }
        }

        public static void DrawNumbers(SpriteBatch batch, Texture2D[] numberTextures, int amount, int x, int y)
        {
            int[] amountNums = new int[3];
            if (amount >= 0)
            {
                if (amount > 999) amount = 999;
                amountNums[0] = (amount - (amount % 100)) / 100;
                amountNums[1] = ((amount - amountNums[0] * 100) - ((amount - amountNums[0] * 100) % 10)) / 10;
                amountNums[2] = amount - (amountNums[0] * 100) - (amountNums[1] * 10);
                batch.Draw(numberTextures[amountNums[0]], new Rectangle(x, y, 13, 23), Color.White);
                batch.Draw(numberTextures[amountNums[1]], new Rectangle(x + 13, y, 13, 23), Color.White);
                batch.Draw(numberTextures[amountNums[2]], new Rectangle(x + 26, y, 13, 23), Color.White);
            }
            else
            {
                char[] amountParts = new char[10];
                amountParts = amount.ToString().ToCharArray();
                if (amount < 0 & amount > -10) amountNums[1] = 0;
                else amountNums[1] = int.Parse(amountParts[amountParts.GetUpperBound(0) - 1].ToString());
                amountNums[2] = int.Parse(amountParts[amountParts.GetUpperBound(0)].ToString());
                if (amount < 0 & amount > -10) amountNums[1] = 0;
                batch.Draw(numberTextures[10], new Rectangle(x, y, 13, 23), Color.White);
                batch.Draw(numberTextures[amountNums[1]], new Rectangle(x + 13, y, 13, 23), Color.White);
                batch.Draw(numberTextures[amountNums[2]], new Rectangle(x + 26, y, 13, 23), Color.White);
            }
        }
    }
}
