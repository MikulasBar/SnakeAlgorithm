using System.Collections.Generic;
using System.Windows.Controls;
namespace SnakeAl
{
    class Position
    {
        public int Row;
        public int Col;
        public Position parent;
        public int Fcost;
        public List<Position> Neighbours = new List<Position>();
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
        static public Position NewPos(int row, int col, Position parent)
        {
            Position pos = new Position(row, col);
            pos.parent = parent;
            return pos;
        }
        public void CreateNeighbours(Border[,] cells)
        {
            Algorithm al = new Algorithm();
            if(!al.WillHit(cells, new Position(Row,Col),-1,0))
            {
                Position up = Position.NewPos(Row -1, Col, this);
                Neighbours.Add(up);
            }
            if(!al.WillHit(cells, new Position(Row,Col),1,0))
            {
                Position down = Position.NewPos(Row +1, Col, this);
                Neighbours.Add(down);
            }
            if(!al.WillHit(cells, new Position(Row,Col),0,1))
            {
                Position right = Position.NewPos(Row, Col +1, this);
                Neighbours.Add(right);
            }
            if(!al.WillHit(cells, new Position(Row,Col),0,-1))
            {
                Position left = Position.NewPos(Row, Col -1, this);
                Neighbours.Add(left);
            }
        }
        public void SetFcost(Position start, Position end)
        {
            Algorithm al = new Algorithm();
            Fcost = al.Distance(this, start, 0, 0) + al.Distance(this, end, 0, 0);
        }
    }
}