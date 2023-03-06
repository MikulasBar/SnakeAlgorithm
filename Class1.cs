using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
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
        public static bool operator ==(Position a, Position b)
        {
            return a.Row == b.Row && a.Col == b.Col;
        }
        public static bool operator !=(Position a, Position b)
        {
            return a.Row != b.Row || a.Col != b.Col;
        }
        public static (int, int) operator -(Position a, Position b)
        {
            int rA = a.Row, rB = b.Row, cA = a.Col, cB = b.Col;
            return (rA - rB, cA - cB);
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
        public Direction Rotate(string d)
        {
            Dictionary<(int, int), (int, int)> dict = new  Dictionary<(int, int), (int, int)>()
            {
                {(0,1), (1,0)},{(1,0), (0,-1)},
                {(0,-1), (-1,0)},{(-1,0), (0,1)}
            };
            Direction dir = new Direction(dict[(this.rowDir, this.colDir)].Item1, dict[(this.rowDir, this.colDir)].Item2);
            if (d == "right")
                return dir;
            if (d == "left")
            {
                dir.rowDir *= -1;
                dir.colDir *= -1;
                return dir;
            }
            return this;
        }
    }
    class Edge
    {
        public int value;
        public Position NodeA, NodeB;
        public bool active;
        public Edge(Position nodeA, Position nodeB)
        {
            Random rn = new Random();
            value = rn.Next(1, 4);
            NodeA = nodeA; NodeB = nodeB;
            active = false;
        }
    }
}
