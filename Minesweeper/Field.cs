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

    public void Reveal(int row, int col)
    {
        if (!tiles[row, col].Flagged) tiles[row, col].Hidden = false;
    }

    public void Hide(int row, int col)
    {
        tiles[row, col].Hidden = true;
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
        if (row != 0 && tiles[(row - 1), col].Mined)
            accumulator++;
        if (col != 0 && tiles[row, (col - 1)].Mined)
            accumulator++;
        if (row != 0 && col != 0 && tiles[(row - 1), (col - 1)].Mined)
            accumulator++;
        if (col != width - 1 && tiles[row, (col + 1)].Mined)
            accumulator++;
        if (row != 0 && col != width - 1 && tiles[(row - 1), (col + 1)].Mined)
            accumulator++;
        if (row != height - 1 && tiles[(row + 1), col].Mined)
            accumulator++;
        if (row != height - 1 && col != 0 && tiles[(row + 1), (col - 1)].Mined)
            accumulator++;
        if (row != height - 1 && col != width - 1 && tiles[(row + 1), (col + 1)].Mined)
            accumulator++;
        return accumulator;
    }

    /// <summary>
    /// Returns a value indicating the number of flagged tiles the specified tile is touching.
    /// </summary>
    public int FlagsTouching(int row, int col)
    {
        int surroundingFlags = 0;

        if (row != 0 && tiles[(row - 1), col].Flagged)
            surroundingFlags++;
        if (col != 0 && tiles[row, (col - 1)].Flagged)
            surroundingFlags++;
        if (row != 0 && col != 0 && tiles[(row - 1), (col - 1)].Flagged)
            surroundingFlags++;
        if (col != width - 1 && tiles[row, (col + 1)].Flagged)
            surroundingFlags++;
        if (row != 0 && col != width - 1 && tiles[(row - 1), (col + 1)].Flagged)
            surroundingFlags++;
        if (row != height - 1 && tiles[(row + 1), col].Flagged)
            surroundingFlags++;
        if (row != height - 1 && col != 0 && tiles[(row + 1), (col - 1)].Flagged)
            surroundingFlags++;
        if (row != height - 1 && col != width - 1 && tiles[(row + 1), (col + 1)].Flagged)
            surroundingFlags++;

        return surroundingFlags;
    }

    /// <summary>
    /// Reveals all tiles touching the specified tile.
    /// </summary>
    void RevealTouching(int row, int col)
    {
        if (row != 0)
            Reveal((row - 1), col);
        if (col != 0)
            Reveal(row, (col - 1));
        if (row != 0 && col != 0)
            Reveal((row - 1), (col - 1));
        if (col != width - 1)
            Reveal(row, (col + 1));
        if (row != 0 && col != width - 1)
            Reveal((row - 1), (col + 1));
        if (row != height - 1)
            Reveal((row + 1), col);
        if (row != height - 1 && col != 0)
            Reveal((row + 1), (col - 1));
        if (row != height - 1 && col != width - 1)
            Reveal((row + 1), (col + 1));
    }

    /// <summary>
    /// Returns a value indicating whether the specified tile is touching a hidden tile.
    /// </summary>
    public bool TouchingHiddenTile(int row, int col)
    {
        if (row != 0 && tiles[(row - 1), col].Hidden && !tiles[(row - 1), col].Flagged)
            return true;
        if (col != 0 && tiles[row, (col - 1)].Hidden && !tiles[row, (col - 1)].Flagged)
            return true;
        if (row != 0 && col != 0 && tiles[(row - 1), (col - 1)].Hidden && !tiles[(row - 1), (col - 1)].Flagged)
            return true;
        if (col != width - 1 && tiles[row, (col + 1)].Hidden && !tiles[row, (col + 1)].Flagged)
            return true;
        if (row != 0 && col != width - 1 && tiles[(row - 1), (col + 1)].Hidden && !tiles[(row - 1), (col + 1)].Flagged)
            return true;
        if (row != height - 1 && tiles[(row + 1), col].Hidden && !tiles[(row + 1), col].Flagged)
            return true;
        if (row != height - 1 && col != 0 && tiles[(row + 1), (col - 1)].Hidden && !tiles[(row + 1), (col - 1)].Flagged)
            return true;
        if (row != height - 1 && col != width - 1 && tiles[(row + 1), (col + 1)].Hidden && !tiles[(row + 1), (col + 1)].Flagged)
            return true;
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
        Reveal(row, col);
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
