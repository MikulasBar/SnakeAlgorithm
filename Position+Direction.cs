using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
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
        public Direction((int,int) i)
        {
            rowDir = i.Item1;
            colDir = i.Item2;
        }
        public Direction Rotate(string d)
        {
            Dictionary<(int,int),(int,int)> dict = new ()
            {
                {(0,1), (1,0)},{(1,0), (0,-1)},
                {(0,-1), (-1,0)},{(-1,0), (0,1)}
            };
            Direction dir = new Direction(dict[(this.rowDir,this.colDir)].Item1, dict[(this.rowDir,this.colDir)].Item2);
            if(d == "right")
            {
                return dir;
            }
            if(d == "left")
            {
                dir.rowDir *= -1;
                dir.colDir *= -1;
                return dir;
            }
            return this;
        }
    }
}