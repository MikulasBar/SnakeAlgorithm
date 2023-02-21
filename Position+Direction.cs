using System.Collections.Generic;
using System.Windows.Controls;
namespace SnakeAl
{
    class Position
    {
        public int Row;
        public int Col;
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
    class Direction
    {
        public int rowDir;
        public int colDir;
        public Direction(int rowDir, int colDir)
        {
            this.rowDir = rowDir;
            this.colDir = colDir;
        }
    }
}