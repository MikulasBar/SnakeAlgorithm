using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Linq;

namespace SnakeAl
{
    class Algorithm
    {
        public int Distance(Position start, Position end, int rowO, int colO)
        {
            return (int)(Math.Abs(start.Row - end.Row + rowO) + Math.Abs(start.Col - end.Col + colO));
        }
        public bool WillHit(Border[,] cells, Position pos, int rowO, int colO)
        {
            /*if(cells.GetLength(0) <= pos.Row + rowO || pos.Row + rowO < 0 || cells.GetLength(1) <= pos.Col + colO || pos.Col + colO < 0)
            {
                return true;
            }*/
            return cells[pos.Row + rowO, pos.Col + colO].Background == Brushes.Lime || cells[pos.Row + rowO, pos.Col + colO].Background == Brushes.Black;
        }
        public Direction FindPath(Border[,] cells, Position start, Position end)
        {
            int min = int.MaxValue, i = 0;
            start.CreateNeighbours(cells);
            if(start.Row == end.Row && start.Col == end.Col)
                return new Direction(0,0);
            foreach(Position n in start.Neighbours)
            {
                if(!WillHit(cells, n, 0, 0) && min > Distance(n, end, 0, 0))
                {
                    min = Distance(n, end, 0, 0);
                    i = Array.IndexOf(start.Neighbours.ToArray(), n); 
                }
            }
            if(i == 0)
                return new Direction(1,0);
            if(i == 1)
                return new Direction(-1,0);
            if(i == 2)
                return new Direction(0,1);
            if(i == 3)
                return new Direction(0,-1);
            return new Direction(0,0);
        }
        Position FindLowestF(List<Position> poses)
        {
            int min = int.MaxValue;
            int j = 0;
            for(int i = 0; i < poses.Count; i++)
            {
                if(min > poses[i].Fcost)
                {
                    min = poses[i].Fcost;
                    j = i;
                }
            }
            return poses.ElementAt(j);
        }
        void FindAllF(List<Position> poses, Position start, Position end)
        {
            for(int i = 0; i < poses.Count; i++)
            {
                poses[i].SetFcost(start, end);
            }
        }
        bool Exists(List<Position> poses, Position pos)
        {
            for(int i = 0; i < poses.Count; i++)
            {
                if(poses[i].Row == pos.Row && poses[i].Col == pos.Col)
                {
                    return true;
                }
            }
            return false;
        }
        List<Position> RemoveCopies(List<Position> poses)
        {
            List<Position> originals = new List<Position>();
            for(int i = 0; i < poses.Count; i++)
            {
                if(!Exists(originals, poses.ElementAt(i)))
                {
                    originals.Add(poses.ElementAt(i));
                }
            }
            return originals;
        }
        public LinkedList<Position> AStar(Border[,] cells, Position start, Position end)
        {
            List<Position> open = new List<Position>();
            LinkedList<Position> closed = new LinkedList<Position>();
            
            open.Add(start);
            while(open.Count != 0)
            {  
                open = RemoveCopies(open);
                FindAllF(open, start, end);
                Position pos = FindLowestF(open);
                open.Remove(pos);
                closed.AddLast(pos);
                if(pos.Row == end.Row && pos.Col == end.Col)
                {
                    return closed;
                }
                pos.CreateNeighbours(cells);
                foreach(Position n in pos.Neighbours)
                {
                    if(Exists(closed.ToList(), n) || WillHit(cells, n, 0,0))
                    {
                        continue;
                    }
                    if(!Exists(open, n) || Distance(n, n.parent, 0, 0) > Distance(n, pos, 0, 0))
                    {
                        n.SetFcost(start, end);
                        n.parent.Neighbours.Remove(n);
                        pos.Neighbours.Add(n);
                        n.parent = pos;

                        if(!Exists(open, n))
                        {
                            open.Add(n);
                        }
                    }
                }
            }
            return closed;
        }
       /*List<Line> PrimsA(Node[,] nodes, Line[,] xL, Line[,] yL)
        {

        }
        Direction[,] GrafToPath(List<Line> graf)
        {

        }*/
    }
}