using System;

public struct Tile
{
    public int Number;
    public bool Mined;
    private bool revealed;
    public bool Flagged;

    public bool Hidden
    {
        get { return !revealed; }
        set { revealed = !value; }
    }
}