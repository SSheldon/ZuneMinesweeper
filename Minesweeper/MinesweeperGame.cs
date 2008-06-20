using System;

public class MinesweeperGame
{
    public int height;
    public int width;
    public int mines;
    public Tile[,] tile;
    
    //Game's constructor
    public MinesweeperGame(int gHeight, int gWidth, int gMines)
    {
        height = gHeight;
        width = gWidth;
        mines = gMines;
        tile = new Tile[height, width];
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                tile[row, col] = new Tile();
            }
        }
        GenerateMines();
        GenerateTileNums();
    }

    //randomly generates mines
    public void GenerateMines()
    {
        int[,] bomb = new int[mines, 3];
        Random RandomClass = new Random();
        for (int counter = 0; counter < mines; counter++)
        {
            bool exists = false;
            do
            {
                exists = false;
                bomb[counter, 0] = RandomClass.Next(0, height * width - 1);
                for (int counter2 = 0; counter2 < counter; counter2++)
                {
                    if (!(exists))
                    {
                        if (bomb[counter, 0] == bomb[counter2, 0])
                        {
                            exists = true;
                        }
                    }
                    else
                    {

                    }
                }
            } while (exists);
            bomb[counter, 2] = bomb[counter, 0] % width;
            bomb[counter, 1] = (bomb[counter, 0] - bomb[counter, 2]) / width;
            tile[bomb[counter, 1], bomb[counter, 2]].MineHere = true;
        }
    }

    //calculates tile numbers
    public void GenerateTileNums()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                int accumulator = 0;
                if (tile[row,col].MineHere == true)
                {
                    accumulator = 9;
                }
                else
                {
                    if (!(row == 0))
                    {
                        if (tile[(row - 1), col].MineHere == true)
                        {
                            accumulator++;
                        }
                    }
                    if (!(col == 0))
                    {
                        if (tile[row, (col - 1)].MineHere == true)
                        {
                            accumulator++;
                        }
                    }
                    if (!(row == 0) & !(col == 0))
                    {
                        if (tile[(row - 1), (col - 1)].MineHere == true)
                        {
                            accumulator++;
                        }
                    }
                    if (!(col == width - 1))
                    {
                        if (tile[row, (col + 1)].MineHere == true)
                        {
                            accumulator++;
                        }
                    }
                    if (!(row == 0) & !(col == width - 1))
                    {
                        if (tile[(row - 1), (col + 1)].MineHere == true)
                        {
                            accumulator++;
                        }
                    }
                    if (!(row == height - 1))
                    {
                        if (tile[(row + 1), col].MineHere == true)
                        {
                            accumulator++;
                        }
                    }
                    if (!(row == height - 1) & !(col == 0))
                    {
                        if (tile[(row + 1), (col - 1)].MineHere == true)
                        {
                            accumulator++;
                        }
                    }
                    if (!(row == height - 1) & !(col == width - 1))
                    {
                        if (tile[(row + 1), (col + 1)].MineHere == true)
                        {
                            accumulator++;
                        }
                    }
                }
                tile[row, col].TileNum = accumulator;
            }
        }
    }

    //carries out choice and returns 0 if game continues, 1 if it is won, and 2 if it is over
    public int SelectedAction(int rowPick, int colPick)
    {
        if (tile[rowPick, colPick].Flagged == true) return 0;
        if (tile[rowPick, colPick].MineHere == true)
        {
            //Game Over
            return 2;
        }
        else
        {
            tile[rowPick, colPick].Reveal();
            if (tile[rowPick, colPick].TileNum == 0)
            {
                RevealTouching(rowPick, colPick);
                bool moreZeros;
                do
                {
                    moreZeros = false;
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            if (tile[row, col].FieldValue == "0" & CheckHiddenTiles(row, col) == true)
                            {
                                RevealTouching(row, col);
                                moreZeros = true;
                            }
                        }
                    }
                } while (moreZeros == true);
            } //end reveal more on zeroes
        } //end tile revealing
        //check to see if the player has won
        bool moreTilesToReveal = false;
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (tile[row, col].MineHere == false & tile[row, col].Hidden == true)
                {
                    moreTilesToReveal = true;
                }
            }
        }
        if (moreTilesToReveal == false)
        {
            //Game won
            return 1;
        }
        else return 0;
    }

    //triggers the reveal method on all tiles touching the specified tile
    public void RevealTouching(int row, int col)
    {
        if (!(row == 0)) tile[(row - 1), col].Reveal();
        if (!(col == 0)) tile[row, (col - 1)].Reveal();
        if (!(row == 0) & !(col == 0)) tile[(row - 1), (col - 1)].Reveal();
        if (!(col == width - 1)) tile[row, (col + 1)].Reveal();
        if (!(row == 0) & !(col == width - 1)) tile[(row - 1), (col + 1)].Reveal();
        if (!(row == height - 1)) tile[(row + 1), col].Reveal();
        if (!(row == height - 1) & !(col == 0)) tile[(row + 1), (col - 1)].Reveal();
        if (!(row == height - 1) & !(col == width - 1)) tile[(row + 1), (col + 1)].Reveal();
    }

    //checkes to see if the specified tile is touching a hidden tile
    public bool CheckHiddenTiles(int row, int col)
    {
        bool anyHidden = false;
        if (!(row == 0)) 
            if (tile[(row - 1), col].Hidden == true & tile[(row - 1), col].Flagged == false) anyHidden = true;
        if (!(col == 0)) 
            if (tile[row, (col - 1)].Hidden == true & tile[row, (col - 1)].Flagged == false) anyHidden = true;
        if (!(row == 0) & !(col == 0)) 
            if (tile[(row - 1), (col - 1)].Hidden == true & tile[(row - 1), (col - 1)].Flagged == false) anyHidden = true;
        if (!(col == width - 1)) 
            if (tile[row, (col + 1)].Hidden == true & tile[row, (col + 1)].Flagged == false) anyHidden = true;
        if (!(row == 0) & !(col == width - 1)) 
            if (tile[(row - 1), (col + 1)].Hidden == true & tile[(row - 1), (col + 1)].Flagged == false) anyHidden = true;
        if (!(row == height - 1)) 
            if (tile[(row + 1), col].Hidden == true & tile[(row + 1), col].Flagged == false) anyHidden = true;
        if (!(row == height - 1) & !(col == 0)) 
            if (tile[(row + 1), (col - 1)].Hidden == true & tile[(row + 1), (col - 1)].Flagged == false) anyHidden = true;
        if (!(row == height - 1) & !(col == width - 1)) 
            if (tile[(row + 1), (col + 1)].Hidden == true & tile[(row + 1), (col + 1)].Flagged == false) anyHidden = true;
        return anyHidden;
    }
}