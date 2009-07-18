using System;

public struct Tile
{
    public int Number = 0;
    public bool Mined = false;
    protected bool hidden = true;
    protected bool flagged = false;

    public bool Hidden
    {
        get { return hidden; }
    }

    public bool Flagged
    {
        get { return flagged; }
    }

    public void Reveal()
    {
        if (!Flagged) hidden = false;
    }

    public void Hide()
    {
        hidden = true;
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