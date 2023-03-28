using System;

namespace SnakeAl
{
    class Position
    {
        public int Row, Col;
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
            return  a.Row != b.Row || a.Col != b.Col;
        }
        public static (int,int) operator -(Position a, Position b)
        {
            return (a.Row - b.Row, a.Col - b.Col);
        }
    }
    class Direction
    {
        public int rowDir, colDir;
        public Direction(int rowDir, int colDir)
        {
            this.rowDir = rowDir;
            this.colDir = colDir;
        }
        public Direction Rotate(string d)
        {
            if(d == "right")
                return new(colDir,-rowDir);
            if(d == "left")
                return new(-colDir,rowDir);
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
            Random rn = new();
            value = rn.Next(1,4);
            NodeA = nodeA; NodeB = nodeB;
            active = false;
        }
    }
}