using System;

public struct Tile
{
    public int Number;
    public bool Mined;
    private bool revealed;
    private bool flagged;

    public bool Hidden
    {
        get { return !revealed; }
    }

    public bool Flagged
    {
        get { return flagged; }
    }

    public void Reveal()
    {
        if (!Flagged) revealed = true;
    }

    public void Hide()
    {
        revealed = false;
    }

    public void Flag()
    {
        if (Hidden) flagged = true;
    }

    public void Unflag()
    {
        flagged = false;
    }
}