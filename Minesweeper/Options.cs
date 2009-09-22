using System;

namespace Minesweeper
{
    public class Options
    {
        bool cantSelectRevealed, flagWithPlay, useTouch;
        string selectedSkin;

        public bool CantSelectRevealed
        {
            get { return cantSelectRevealed; }
            set { cantSelectRevealed = value; }
        }

        public bool FlagWithPlay
        {
            get { return flagWithPlay; }
            set { flagWithPlay = value; }
        }

        public bool UseTouch
        {
            get { return useTouch; }
            set { useTouch = value; }
        }

        public string SelectedSkin
        {
            get { return selectedSkin; }
            set { selectedSkin = value; }
        }

        public Options(bool cantSelectRevealed, bool flagWithPlay, bool useTouch, string selectedSkin)
        {
            CantSelectRevealed = cantSelectRevealed;
            FlagWithPlay = flagWithPlay;
            UseTouch = useTouch;
            SelectedSkin = selectedSkin;
        }
    }
}