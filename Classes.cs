using System.Collections.Generic;
using System;
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
            int rA = a.Row, rB = b.Row, cA = a.Col, cB = b.Col;
            return (rA-rB,cA-cB);
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
            if(d == "right")
                return new Direction(colDir, -rowDir);
            if(d == "left")
                return new Direction(-colDir, rowDir);
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
            value = rn.Next(1,4);
            NodeA = nodeA; NodeB = nodeB;
            active = false;
        }
    }
}
