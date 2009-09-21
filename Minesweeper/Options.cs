using System;

namespace Minesweeper
{
    public class Options
    {
        bool cantSelectRevealed, flagWithPlay, useTouch;
        int selectedSkin;

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

        public int SelectedSkin
        {
            get { return selectedSkin; }
            set { selectedSkin = value; }
        }

        public Options(bool cantSelectRevealed, bool flagWithPlay, bool useTouch, int selectedSkin)
        {
            CantSelectRevealed = cantSelectRevealed;
            FlagWithPlay = flagWithPlay;
            UseTouch = useTouch;
            SelectedSkin = selectedSkin;
        }
    }
}