
using System;
using System.Threading;
using mine_sweeper_cs;

class Program
{
    static void Main()
    {
        var gb = new GameBoard(9, 9, 10);
        gb.CuiGame();
    }
}
