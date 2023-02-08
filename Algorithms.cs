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
            int up = int.MaxValue, down = int.MaxValue, right = int.MaxValue, left = int.MaxValue;
            if(start == end)
                return new Direction(0,0);
            if(!WillHit(cells, start, 1,0))
                down = Distance(start, end, 1, 0);
            if(!WillHit(cells, start, 0,1))
                right = Distance(start, end, 0, 1);
            if(!WillHit(cells, start, -1,0))
                up = Distance(start, end, -1, 0);
            if(!WillHit(cells, start, 0,-1))
                left = Distance(start, end, 0, -1);

            int min = Math.Min(Math.Min(up, down) , Math.Min(right, left));

            if(min == up)
                return new Direction(-1,0);
            if(min == down)
                return new Direction(1,0);
            if(min == right)
                return new Direction(0,1);
            if(min == left)
                return new Direction(0,-1);
            return new Direction(0,0);
        }
        int[,] SetForAStar(Border[,] cells, Position start, Position end)
        {
            int[,] grid = new int[cells.GetLength(0),cells.GetLength(1)];
            for(int r = 0; r < cells.GetLength(0); r++)
            {
                for(int c = 0; c < cells.GetLength(1); c++)
                {

                    if(cells[r,c].Background == Brushes.Lime || cells[r,c].Background == Brushes.Black)
                    {
                        grid[r,c] = 5;
                    }
                    else
                    {
                        grid[r,c] = 2;
                    }
                }
            }
            grid[start.Row, start.Col] = 0;
            grid[end.Row, end.Col] = 1;
            return grid;
        }
        Position FindLowestF(int[,] grid, int[,] fcosts)
        {
            int min = int.MaxValue, j = 0, k = 0;
            for(int r = 0; r < grid.GetLength(0); r++)
            {
                for(int c = 0; c < grid.GetLength(1); c++)
                {
                    if(grid[r,c] == 3 || grid[r,c] == 0 || grid[r,c] == 1)
                    {
                        if(min > fcosts[r,c])
                        {
                            min = fcosts[r,c];
                            j = r; k = c;
                        }
                    }
                }
            }
            return new Position(j,k);
        }
        int DistanceOnObserved(Position start ,Position end, Position[,] parents)
        {
            int d = 0;
            Position pos = start;
            while(true)
            {
                if(pos == end)
                {
                    return d;
                }
                d++;
                pos = parents[pos.Row,pos.Col];
            }
        }
        LinkedList<Position> Path(int[,] grid, Position[,] parents, Position end, Position start)
        {
            LinkedList<Position> path = new LinkedList<Position>();
            Position pos = new Position(end.Row, end.Col);
            while(true)
            {
                if(pos.Row == start.Row && pos.Col == start.Col)
                {
                    return path;
                }
                path.AddFirst(pos);
                pos = parents[pos.Row, pos.Col];
            }
        }
        public LinkedList<Position> AStar(Border[,] cells, Position start, Position end)
        {
            int[,] grid = SetForAStar(cells, start, end);
            int[,] fcosts = new int[cells.GetLength(0),cells.GetLength(1)];
            Position[,] parents = new Position[cells.GetLength(0),cells.GetLength(1)];
            for(int r = 0; r < grid.GetLength(0); r++)
            {
                for(int c = 0; c < grid.GetLength(1); c++)
                {
                    fcosts[r,c] = int.MaxValue;
                }
            }
            fcosts[start.Row, start.Col] = Distance(start , end, 0, 0);
            
            //  2 = unrevealed, 3 = revealed, 4 = path, 5 = not traversable
            parents[start.Row, start.Col] = start;
            while(true)
            {
                Position pos = FindLowestF(grid, fcosts);
                if(pos.Row == end.Row && pos.Col == end.Col)
                {
                    return Path(grid, parents, end, start);
                }
                grid[pos.Row, pos.Col] = 4;
                for(int r = -1; r < 2; r++)
                {
                    for(int c = -1; c < 2; c++)
                    {
                        Position n = new Position(pos.Row + r, pos.Col + c);
                        if((r != 0 && c != 0) || (r == 0 && c == 0))
                        {
                            continue;
                        }
                        if(grid[n.Row,n.Col] == 4 || grid[n.Row,n.Col] == 5)
                        { 
                            continue;
                        }
                        if(!(grid[n.Row, n.Col] == 3) || DistanceOnObserved(parents[n.Row, n.Col], start, parents) > DistanceOnObserved(pos, start, parents))
                        {
                            parents[n.Row, n.Col] = pos;
                            fcosts[n.Row, n.Col] = Distance(n, end, 0, 0) + DistanceOnObserved(n, start, parents);
                            if(!(grid[n.Row,n.Col] == 3))
                            {
                                grid[n.Row, n.Col] = 3;
                            } 
                        }
                    }    
                }
            }
            return new LinkedList<Position>();
        }
       /*List<Line> PrimsA(Node[,] nodes, Line[,] xL, Line[,] yL)
        {

        }
        Direction[,] GrafToPath(List<Line> graf)
        {

        }*/
    }
}