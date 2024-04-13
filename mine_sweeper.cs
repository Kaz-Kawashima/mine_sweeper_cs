using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace mine_sweeper_cs
{
    enum KeyAction
    {
        up, 
        down, 
        left, 
        right, 
        open, 
        flag,
        no_action
    }

    internal class GameBoard
    {
        private List<List<Panel>> field;
        private int sizeX;
        private int sizeY;
        private int fieldSizeX;
        private int fieldSizeY;
        private int numBomb;
        private int cursorRow;
        private int cursorCol;


        public GameBoard(int y, int x, int numBomb)
        {
            sizeX = x;
            sizeY = y;
            fieldSizeX = x + 2;
            fieldSizeY = y + 2;
            field = new List<List<Panel>>();
            this.numBomb = numBomb;
            cursorCol = 1;
            cursorRow = 1;
            //FillPanel
            for (int row = 0; row < fieldSizeY; row++)
            {
                var panelRow = new List<Panel>();
                for (int col = 0; col < fieldSizeX; col++)
                {
                    panelRow.Add(new BlankPanel());
                }
                field.Add(panelRow);
            }
            //FillBorder
            for (int row=0; row < fieldSizeY; row++)
            {
                field[row][0] = new BorderPanel();
                field[row][fieldSizeX - 1] = new BorderPanel();
            }
            for (int col = 0;col < fieldSizeX; col++)
            {
                field[0][col] = new BorderPanel();
                field[fieldSizeY - 1][col] = new BorderPanel();
            }
            setBomb();
            calcFieldBombValue();
        }

        private void setBomb()
        {
            var rand  = new Random();
            int bombCounter = 0;
            while (bombCounter < numBomb)
            {
                int row = rand.Next(1, sizeY);
                int col = rand.Next(1, sizeX);
                var panel = field[row][col];
                if (panel is not BombPanel)
                {
                    field[row][col] = new BombPanel();
                    bombCounter++;
                }
            }
        }

        private void calcFieldBombValue()
        {
            for (int row = 1; row <= sizeY; row++) {
                for (int col = 1; col <= sizeX; col++)
                {
                    var panel = field[row][col];
                    if (panel is BlankPanel)
                    {
                        ((BlankPanel)panel).bombValue = calcPanelBombValue(row, col);
                    }
                }
            }
        }

        int calcPanelBombValue(int y, int x)
        {
            int bombCounter = 0;
            for (int row = y - 1; row <= y + 1; row++)
            {
                for (int col =  x - 1; col <= x + 1; col++)
                {
                    var panel = field[row][col];
                    if (panel is BombPanel)
                    {
                        bombCounter++;
                    }
                }
            }
            return bombCounter;
        }

        public void Print()
        {
            string output = "";
            for(int row = 0; row < fieldSizeY; row++)
            {
                for(int col = 0; col < fieldSizeX; col++)   
                {
                    string panel_string;
                    if (row == 0 && col == cursorCol)
                    {
                        panel_string = "v";
                    }
                    else if (row == fieldSizeY - 1 && col == cursorCol)
                    {
                        panel_string = "^";
                    }
                    else if (row == cursorRow && col == 0)
                    {
                        panel_string = ">";
                    }
                    else if (row == cursorRow && col == fieldSizeX - 1)
                    {
                        panel_string = "<";
                    }
                    else if (row == cursorRow && col == cursorCol)
                    {
                        panel_string = "@";
                    }
                    else
                    {
                        var panel = field[row][col];
                        panel_string = panel.ToString();
                    }
                    output += panel_string;
                    output += " ";
                }
                output += "\n";
            }
            var flagCount = CountFlag();
            output += $"\ninput <- ^v -> / O open / F flag ({flagCount})";
            Console.Clear();
            Console.Write(output);
        }

        public int CountFlag()
        {
            int flagCount = 0;
            foreach(var panelRow in field)
            {
                foreach(var panel in panelRow)
                {
                    if (panel.isFlagged)
                    {
                        flagCount++;
                    }
                }
            }
            return flagCount;
        }

        public OpenResult Open(int row, int col)
        {
            var panel = field[row][col];
            return panel.Open();
        }

        public void Flag(int row, int col) {
            field[row][col].Flag();
        }

        public int OpenAround(int y, int x)
        {
            int newOpen = 0;
            for (int row = y - 1; row <= y + 1; row++)
            {
                for (int col = x - 1; col <= x + 1; col++)
                {
                    var panel = field[row][col];
                    if (!panel.isOpen)
                    {
                        panel.Open();
                        newOpen++;
                    }
                }
            }
            return newOpen;
        }

        public void CascadeOpen()
        {
            int newOpen = 1;
            while(newOpen > 0)
            {
                newOpen = 0;
                for (int row = 1; row <= sizeY; row++)
                {
                    for(int col = 1; col <= sizeX; col++)
                    {
                        var panel = field[row][col];
                        if (panel is BlankPanel && panel.isOpen && ((BlankPanel)panel).bombValue == 0)
                        {
                            newOpen += OpenAround(row, col);
                        }
                    }
                }
            }
        }

        public void BombOpen()
        {
            foreach(var panelRow in field)
            {
                foreach(var panel in panelRow)
                {
                    if(panel is BombPanel)
                    {
                        panel.Open();
                    }
                }
            }
        }

        public bool IsFinished()
        {
            foreach (var panelRow in field)
            {
                foreach (var panel in panelRow)
                {
                    if(!panel.isOpen && panel is BlankPanel){
                        return false;
                    }
                }
            }
            return true;
        }

        private void up()
        {
            cursorRow -= 1;
            if (cursorRow < 1)
            {
                cursorRow = 1;
            }
        }

        private void down() 
        {
            cursorRow += 1;
            if (cursorRow > sizeY)
            {
                cursorRow = sizeY;
            }
        }

        private void left() {
            cursorCol -= 1;
            if(cursorCol < 1)
            {
                cursorCol = 1;
            }
        } 
        private void right() {
            cursorCol += 1;
            if(cursorCol > sizeX)
            {
                cursorCol = sizeX;
            }
        }

        public KeyAction GetKey()
        {
            var keyInfo = Console.ReadKey();
            if(keyInfo.Key == ConsoleKey.UpArrow)
            {
                //Console.WriteLine("up");
                return KeyAction.up;
            }
            else if(keyInfo.Key == ConsoleKey.DownArrow)
            {
                //Console.WriteLine("down");
                return KeyAction.down;
            }
            else if(keyInfo.Key == ConsoleKey.LeftArrow)
            {
                //Console.WriteLine("left");
                return KeyAction.left;
            }
            else if(keyInfo.Key == ConsoleKey.RightArrow)
            {
                //Console.WriteLine("right");
                return KeyAction.right;
            }
            else if(keyInfo.KeyChar == 'o')
            {
                //Console.WriteLine("o");
                return KeyAction.open;
            }
            else if(keyInfo.KeyChar == 'f')
            {
                //Console.WriteLine("f");
                return KeyAction.flag;
            } else
            {
                return KeyAction.no_action;
            }
        }

        public void CuiGame()
        {
            bool finished = false;
            OpenResult result = OpenResult.Safe;
            while (!finished)
            {
                Print();
                var key = GetKey();
                switch (key)
                {
                    case KeyAction.up:
                        up();
                        break;
                    case KeyAction.down:
                        down(); 
                        break;
                    case KeyAction.left:
                        left();
                        break;
                    case KeyAction.right:
                        right();
                        break;
                    case KeyAction.flag:
                        Flag(cursorRow, cursorCol); 
                        break;
                    case KeyAction.open:
                        result = Open(cursorRow, cursorCol);
                        if (result == OpenResult.Safe)
                        {
                            CascadeOpen();
                            finished = IsFinished();
                        } else
                        {
                            BombOpen();
                            finished = true;
                        }
                        break;
                }
            }
            Print();
            switch (result)
            {
                case OpenResult.Safe:
                    Console.WriteLine("\n\nYou Win!");
                    break;
                case OpenResult.Explosion:
                    Console.WriteLine("\n\nGame Over");
                    break;
            }
            Console.WriteLine("\nHit any Key.");
            Console.ReadKey();
        }
    }
}
