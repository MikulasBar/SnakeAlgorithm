using System;

namespace SnakeAl
{
    struct Position
    {
        public int Row, Col; // Y and X coordinates
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
        // Operators to compare and subtract positions
        public static bool operator ==(Position a, Position b) => a.Row == b.Row && a.Col == b.Col;
        public static bool operator !=(Position a, Position b) => a.Row != b.Row || a.Col != b.Col;
        public static (int,int) operator -(Position a, Position b) => (a.Row - b.Row, a.Col - b.Col);
    }
    struct Direction
    {
        public int rowDir, colDir; // Y and X offset, representing vector without origin
        public Direction(int rowDir, int colDir)
        {
            this.rowDir = rowDir;
            this.colDir = colDir;
        }
        public Direction Rotate(string d) // Rotating direction by 90° right or left
        {
            return d == "right" ? new(colDir, -rowDir) : d == "left" ? new(-colDir, rowDir) : this;
        }
    }
    class Edge
    {
        public int value; // Value of edge
        public Position NodeA, NodeB; // Two nodes connected to edge
        public bool active; // Is Active
        public Edge(Position nodeA, Position nodeB)
        {
            Random rn = new();
            value = rn.Next(1,4); // Value is random 
            NodeA = nodeA; NodeB = nodeB;
            active = false;
        }
    }
}