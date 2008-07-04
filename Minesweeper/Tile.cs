using System;

public class Tile
{
    protected string fieldValue = " ";
    protected int tileNum = 0;
    protected bool mineHere = false;
    protected bool hidden = true;
    protected bool flagged = false;

    public string FieldValue
    {
        get
        {
            if (Flagged) fieldValue = ">";
            else
            {
                if (Hidden) fieldValue = " ";
                else
                {
                    if (MineHere == true) fieldValue = "X";
                    else fieldValue = TileNum.ToString();
                }
            }
            return fieldValue;
        }
    }

    public int TileNum
    {
        get
        {
            return tileNum;
        }
        set
        {
            tileNum = value;
        }
    }

    public bool MineHere
    {
        get
        {
            return mineHere;
        }
        set
        {
            mineHere = value;
        }
    }

    public bool Hidden
    {
        get
        {
            return hidden;
        }
    }

    public bool Flagged
    {
        get
        {
            return flagged;
        }
    }

    public void Reveal()
    {
        if (!(Flagged))
        {
            hidden = false;
        }
    }

    public void Hide()
    {
        hidden = true;
    }

    public void Flag()
    {
        if (Hidden)
        {
            flagged = true;
        }

    }

    public void Unflag()
    {
        flagged = false;
    }
}