using System;
using System.Collections;

public class Field : IEnumerable
{
    int height, width, mines;
    Tile[,] tiles;
    public int Height
    {
        get { return height; }
    }
    public int Width
    {
        get { return width; }
    }
    public int Mines
    {
        get { return mines; }
    }

    public Tile this[int row, int col]
    {
        get { return tiles[row, col]; }
    }

    public void Flag(int row, int col)
    {
        if (tiles[row, col].Hidden) tiles[row, col].Flagged = true;
    }

    public void Unflag(int row, int col)
    {
        tiles[row, col].Flagged = false;
    }

    public Field(int height, int width, int mines)
    {
        this.height = height;
        this.width = width;
        this.mines = mines;
        tiles = new Tile[height, width];
        GenerateMines();
        AssignTileNumbers();
    }

    public IEnumerator GetEnumerator()
    {
        return new FieldEnumerator(this);
    }

    /// <summary>
    /// Randomly generates mines and assigns them to tiles.
    /// </summary>
    void GenerateMines()
    {
        int[] randomNumbers = new int[mines];
        Random rand = new Random();
        for (int i = 0; i < mines; i++)
        {
            bool alreadyContained;
            do
            {
                alreadyContained = false;
                int randomNumber = rand.Next(0, height * width - 1);
                for (int j = 0; j < i; j++)
                {
                    if (randomNumbers[j] == randomNumber) alreadyContained = true;
                }
                if (!alreadyContained) randomNumbers[i] = randomNumber;
            } while (alreadyContained);
        }
        foreach (int i in randomNumbers)
        {
            tiles[i / width, i % width].Mined = true;
        }
    }

    /// <summary>
    /// Calculates tile numbers and assigns them.
    /// </summary>
    void AssignTileNumbers()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                tiles[row, col].Number = TileNumber(row, col);
            }
        }
    }

    int TileNumber(int row, int col)
    {
        int accumulator = 0;
        for (AdjLoc l = new AdjLoc(this, row, col); l.Valid; l++)
        {
            if (tiles[l.Row, l.Col].Mined) accumulator++;
        }
        return accumulator;
    }

    /// <summary>
    /// Returns a value indicating the number of flagged tiles the specified tile is touching.
    /// </summary>
    public int FlagsTouching(int row, int col)
    {
        int surroundingFlags = 0;
        for (AdjLoc l = new AdjLoc(this, row, col); l.Valid; l++)
        {
            if (tiles[l.Row, l.Col].Flagged) surroundingFlags++;
        }
        return surroundingFlags;
    }

    /// <summary>
    /// Reveals all tiles touching the specified tile.
    /// </summary>
    void RevealTouching(int row, int col)
    {
        for (AdjLoc l = new AdjLoc(this, row, col); l.Valid; l++)
        {
            if (!tiles[l.Row, l.Col].Flagged) tiles[l.Row, l.Col].Hidden = false;
        }
    }

    /// <summary>
    /// Returns a value indicating whether the specified tile is touching a hidden tile.
    /// </summary>
    public bool TouchingHiddenTile(int row, int col)
    {
        for (AdjLoc l = new AdjLoc(this, row, col); l.Valid; l++)
        {
            if (tiles[l.Row, l.Col].Hidden && !tiles[l.Row, l.Col].Flagged) return true;
        }
        return false;
    }

    /// <summary>
    /// Gets a value indicating whether all the unmined tiles are revealed.
    /// </summary>
    public bool AllUnminedRevealed
    {
        get
        {
            foreach (Tile tile in this)
            {
                if (!tile.Mined && tile.Hidden) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Reveals the specified tile and returns a value indicating if it was mined.
    /// </summary>
    public bool Click(int row, int col)
    {
        if (!tiles[row, col].Flagged) tiles[row, col].Hidden = false;
        //check to see if there are zero tiles touching hidden tiles
        bool checkAgain;
        do
        {
            checkAgain = false;
            for (int rowCounter = 0; rowCounter < height; rowCounter++)
            {
                for (int colCounter = 0; colCounter < width; colCounter++)
                {
                    if (!tiles[rowCounter, colCounter].Hidden && !tiles[rowCounter, colCounter].Mined &&
                        tiles[rowCounter, colCounter].Number == 0 && TouchingHiddenTile(rowCounter, colCounter))
                    {
                        RevealTouching(rowCounter, colCounter);
                        checkAgain = true;
                    }
                }
            }
        } while (checkAgain);
         return tiles[row, col].Mined && !tiles[row, col].Flagged;
    }

    /// <summary>
    /// Reveals the tiles surrounding the specified tile and returns a value indicating if any were mined and unflagged.
    /// </summary>
    public bool ClickSurrounding(int row, int col)
    {
        bool mineClicked = false;
        for (AdjLoc l = new AdjLoc(this, row, col); l.Valid; l++)
        {
            if (Click(l.Row, l.Col)) mineClicked = true;
        }
        return mineClicked;
    }

    /// <summary>
    /// If the specified tile is mined, the mine will be moved to a different tile.
    /// </summary>
    public void MoveMine(int row, int col)
    {
        if (tiles[row, col].Mined)
        {
            for (int rowCounter = 0; rowCounter < height; rowCounter++)
            {
                for (int colCounter = 0; colCounter < width; colCounter++)
                {
                    if (!tiles[rowCounter, colCounter].Mined)
                    {
                        tiles[rowCounter, colCounter].Mined = true;
                        tiles[row, col].Mined = false;
                        break;
                    }
                }
                if (!tiles[row, col].Mined) break;
            }
            AssignTileNumbers();
        }
    }

    /// <summary>
    /// Gets a value indicating whether all the tiles are hidden.
    /// </summary>
    public bool AllHidden
    {
        get
        {
            foreach (Tile tile in this)
            {
                if (!tile.Hidden) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Represents a location on the field adjacent to the specified tile.
    /// </summary>
    private struct AdjLoc
    {
        private int r, c, i;
        private int height, width;

        public AdjLoc(Field field, int row, int col)
        {
            height = field.Height;
            width = field.Width;
            r = row;
            c = col;
            i = -1;
            this++;
        }

        /// <summary>
        /// Gets the row number of the adjacent location.
        /// </summary>
        public int Row
        {
            get
            {
                switch (i)
                {
                    case 0:
                    case 1:
                    case 2:
                        return r - 1;
                    case 3:
                    case 4:
                    default:
                        return r;
                    case 5:                        
                    case 6:
                    case 7:
                        return r + 1;
                }
            }
        }

        /// <summary>
        /// Gets the column number of the adjacent location.
        /// </summary>
        public int Col
        {
            get
            {
                switch (i)
                {
                    case 0:
                    case 3:
                    case 5:
                        return c - 1;
                    case 1:
                    case 6:
                    default:
                        return c;
                    case 2:
                    case 4:
                    case 7:
                        return c + 1;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if this is a valid location. 
        /// If it isn't, there are no more locations adjacent to the specified tile.
        /// </summary>
        public bool Valid
        {
            get { return i < 8 && i >= 0; }
        }

        public static AdjLoc operator ++(AdjLoc l)
        {
            for (l.i++; !l.PositionExists && l.i < 8; l.i++) ;
            return l;
        }

        private bool PositionExists
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return r != 0 && c != 0;
                    case 1:
                        return r != 0;
                    case 2:
                        return r != 0 && c != width - 1;
                    case 3:
                        return c != 0;
                    case 4:
                        return c != width - 1;
                    case 5:
                        return r != height - 1 && c != 0;
                    case 6:
                        return r != height - 1;
                    case 7:
                        return r != height - 1 && c != width - 1;
                    default:
                        return false;
                }
            }
        }
    }
}

public class FieldEnumerator : IEnumerator
{
    int position = -1;
    Field field;

    public FieldEnumerator(Field field)
    {
        this.field = field;
    }

    public object Current
    {
        get
        {
            return field[position / field.Width, position % field.Width];
        }
    }

    public bool MoveNext()
    {
        position++;
        return (position < field.Height * field.Width);
    }

    public void Reset()
    {
        position = -1;
    }
}
