using System;

namespace Minesweeper
{
    public class Options
    {
        bool flagWithPlay, useTouch;
        string selectedSkin;

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

        public Options(bool flagWithPlay, bool useTouch, string selectedSkin)
        {
            FlagWithPlay = flagWithPlay;
            UseTouch = useTouch;
            SelectedSkin = selectedSkin;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Options)) return false;
            Options o = (Options)obj;
            if (o.FlagWithPlay != FlagWithPlay) return false;
            if (o.UseTouch != UseTouch) return false;
            if (o.SelectedSkin != SelectedSkin) return false;
            return true;
        }
    }
}