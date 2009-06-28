using System;

public class Tile
{
    protected int number = 0;
    protected bool mined = false;
    protected bool hidden = true;
    protected bool flagged = false;

    public int Number
    {
        get { return number; }
        set { number = value; }
    }

    public bool Mined
    {
        get { return mined; }
        set { mined = value; }
    }

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
        if (!(Flagged)) hidden = false;
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