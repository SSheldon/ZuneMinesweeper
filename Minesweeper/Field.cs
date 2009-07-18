using System;

public class Field
{
    int height, width, mines;
    public Tile[,] tiles;
    
    public Field(int height, int width, int mines)
    {
        this.height = height;
        this.width = width;
        this.mines = mines;
        tiles = new Tile[height, width];
        GenerateMines();
        AssignTileNumbers();
    }

    //randomly generates mines
    void GenerateMines()
    {
        int[] randomNumbers = new int[mines];
        Random rand = new Random();
        for (int counter = 0; counter < mines; counter++)
        {
            bool alreadyContained = false;
            do
            {
                alreadyContained = false;
                int randomNumber = rand.Next(0, height * width - 1);
                for (int counter2 = 0; counter2 < counter; counter2++)
                {
                    if (randomNumbers[counter2] == randomNumber) alreadyContained = true;
                }
                if (!alreadyContained) randomNumbers[counter] = randomNumber;
            } while (alreadyContained);
        }
        for (int counter = 0; counter < mines; counter++)
        {
            tiles[(int)Math.Floor(randomNumbers[counter] / width), randomNumbers[counter] % width].Mined = true;
        }
    }

    //calculates tile numbers
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
        if (tiles[row, col].Mined == true)
        {
            accumulator = 9;
        }
        else
        {
            if (row != 0)
            {
                if (tiles[(row - 1), col].Mined)
                {
                    accumulator++;
                }
            }
            if (col != 0)
            {
                if (tiles[row, (col - 1)].Mined)
                {
                    accumulator++;
                }
            }
            if (row != 0 && col != 0)
            {
                if (tiles[(row - 1), (col - 1)].Mined)
                {
                    accumulator++;
                }
            }
            if (col != width - 1)
            {
                if (tiles[row, (col + 1)].Mined)
                {
                    accumulator++;
                }
            }
            if (row != 0 && col != width - 1)
            {
                if (tiles[(row - 1), (col + 1)].Mined)
                {
                    accumulator++;
                }
            }
            if (row != height - 1)
            {
                if (tiles[(row + 1), col].Mined)
                {
                    accumulator++;
                }
            }
            if (row != height - 1 && col != 0)
            {
                if (tiles[(row + 1), (col - 1)].Mined)
                {
                    accumulator++;
                }
            }
            if (row != height - 1 && col != width - 1)
            {
                if (tiles[(row + 1), (col + 1)].Mined)
                {
                    accumulator++;
                }
            }
        }
        return accumulator;
    }

    /// <summary>
    /// Reveals all tiles touching the specified tile.
    /// </summary>
    public void RevealTouching(int row, int col)
    {
        if (row != 0) tiles[(row - 1), col].Reveal();
        if (col != 0) tiles[row, (col - 1)].Reveal();
        if (row != 0 && col != 0) tiles[(row - 1), (col - 1)].Reveal();
        if (col != width - 1) tiles[row, (col + 1)].Reveal();
        if (row != 0 && col != width - 1) tiles[(row - 1), (col + 1)].Reveal();
        if (row != height - 1) tiles[(row + 1), col].Reveal();
        if (row != height - 1 && col != 0) tiles[(row + 1), (col - 1)].Reveal();
        if (row != height - 1 && col != width - 1) tiles[(row + 1), (col + 1)].Reveal();
    }

    /// <summary>
    /// Returns a value indicating whether the specified tile is touching a hidden tile.
    /// </summary>
    public bool TouchingHiddenTile(int row, int col)
    {
        bool anyHidden = false;
        if (!(row == 0)) 
            if (tiles[(row - 1), col].Hidden == true & tiles[(row - 1), col].Flagged == false) anyHidden = true;
        if (!(col == 0)) 
            if (tiles[row, (col - 1)].Hidden == true & tiles[row, (col - 1)].Flagged == false) anyHidden = true;
        if (!(row == 0) & !(col == 0)) 
            if (tiles[(row - 1), (col - 1)].Hidden == true & tiles[(row - 1), (col - 1)].Flagged == false) anyHidden = true;
        if (!(col == width - 1)) 
            if (tiles[row, (col + 1)].Hidden == true & tiles[row, (col + 1)].Flagged == false) anyHidden = true;
        if (!(row == 0) & !(col == width - 1)) 
            if (tiles[(row - 1), (col + 1)].Hidden == true & tiles[(row - 1), (col + 1)].Flagged == false) anyHidden = true;
        if (!(row == height - 1)) 
            if (tiles[(row + 1), col].Hidden == true & tiles[(row + 1), col].Flagged == false) anyHidden = true;
        if (!(row == height - 1) & !(col == 0)) 
            if (tiles[(row + 1), (col - 1)].Hidden == true & tiles[(row + 1), (col - 1)].Flagged == false) anyHidden = true;
        if (!(row == height - 1) & !(col == width - 1)) 
            if (tiles[(row + 1), (col + 1)].Hidden == true & tiles[(row + 1), (col + 1)].Flagged == false) anyHidden = true;
        return anyHidden;
    }

    /// <summary>
    /// Gets a value indicating whether all the unmined tiles are revealed.
    /// </summary>
    public bool AllUnminedRevealed
    {
        get
        {
            bool moreTilesToReveal = false;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (tiles[row, col].Mined == false & tiles[row, col].Hidden == true) moreTilesToReveal = true;
                }
            }
            return !moreTilesToReveal;
        }
    }

    /// <summary>
    /// Reveals the specified tile and returns a value indicating if it was mined.
    /// </summary>
    public bool Click(int row, int col)
    {
        tiles[row, col].Reveal();
        bool checkAgain;
        //check to see if there are zero tiles touching hidden tiles
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
        } while (checkAgain == true);
        if (tiles[row, col].Mined && !tiles[row, col].Flagged) return true;
        else return false;
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
}