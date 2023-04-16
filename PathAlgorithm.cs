using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System;

namespace SnakeAl
{
    struct PathAlgorithm
    {
        public int Distance(Position start, Position end, int rowO, int colO) // Find difference of Row and Col of start and end, then add it together, 
        {                                                                     // start node has offset
            return (int)(Math.Abs(start.Row - end.Row + rowO) + Math.Abs(start.Col - end.Col + colO));
        }
        public bool WillHit(Border[,] cells, Position pos, int rowO, int colO) // Return True if node with some offset hits snake or is not in grid
        {
            Position newpos = new(pos.Row + rowO, pos.Col + colO);
            return newpos.Row == -1 || newpos.Col == -1 || newpos.Row == cells.GetLength(0) || newpos.Col == cells.GetLength(1) || cells[newpos.Row, newpos.Col].Background == Brushes.Lime;
        }
        public Direction NextMove(Border[,] cells, Position start, Position target) // Find closest node to target node, that is also next to start
        {
            (int r, int c) = start - target;
            if(r > 0 && !WillHit(cells, start, -1, 0))
                return new(-1,0);
            else if(r < 0 && !WillHit(cells, start, 1, 0))
                return new(1,0);
            else if(c > 0 && !WillHit(cells, start, 0, -1))
                return new(0,-1);
            else if(c < 0 && !WillHit(cells, start, 0, 1))
                return new(0,1);
            return new(0,0);
        }
        public int[,] SetForAStar(Border[,] cells, Position start, int[,] order, Position end) // Setup grid for AStar
        {
            int[,] grid = new int[cells.GetLength(0),cells.GetLength(1)];
            for(int r = 0; r < cells.GetLength(0); r++)
            {
                for(int c = 0; c < cells.GetLength(1); c++) // If order is greater than target than its not traversable
                {
                    grid[r,c] = order[r,c] > order[end.Row, end.Col] ? 5 : 2;
                }
            }
            grid[start.Row, start.Col] = 3;
            return grid;
        }
        Position FindLowestF(int[,] grid, int[,] fcosts) // Find node with lowest Fcost 
        {
            int min = int.MaxValue, j = int.MinValue, k = int.MinValue;
            for(int r = 0; r < grid.GetLength(0); r++)
            {
                for(int c = 0; c < grid.GetLength(1); c++)
                {
                    if(min > fcosts[r,c] && grid[r,c] == 3)
                    {
                        min = fcosts[r,c];
                        j = r; k = c;
                    }
                }
            }
            return new(j,k);
        }
        int DistanceOnObserved(Position start ,Position end, Position[,] parents) // Number of hops needed to get end to start node
        {
            int d = 0;
            Position pos = start;
            while(true)
            {
                if(pos == end)
                    return d;
                d++;
                pos = parents[pos.Row,pos.Col];
            }
        }
        LinkedList<Position> Path(int[,] grid, Position[,] parents, Position end, Position start) // Find List of nodes in path using parents
        {
            LinkedList<Position> path = new();
            Position pos = new(end.Row, end.Col); // Steping on parents on nodes
            while(true)
            {
                if(pos == start)
                    return path;
                path.AddFirst(pos);
                pos = parents[pos.Row, pos.Col];
            }
        }
        public LinkedList<Position> AStar(Border[,] cells, Position start, Position end, int[,] order) // AStar Algorithm, finding shortest path
        {
            int[,] grid = SetForAStar(cells, start, order, end);            // Variables
            int[,] fcosts = new int[cells.GetLength(0),cells.GetLength(1)]; // distance between node and target + number of hops needed to get node to start node
            Position[,] parents = new Position[cells.GetLength(0),cells.GetLength(1)]; // all nodes refers to another node

            for(int r = 0; r < grid.GetLength(0); r++) // Setup Fcosts
            {
                for(int c = 0; c < grid.GetLength(1); c++)
                    fcosts[r,c] = int.MaxValue;
            }
            fcosts[start.Row, start.Col] = Distance(start , end, 0, 0); // Setup start node
            parents[start.Row, start.Col] = start;
            
            while(true) // Nodes: 2 = UnExplored, 3 = Explored, 4 = Already closed, 5 = Snake / Wall
            {
                Position pos = FindLowestF(grid, fcosts); // Find lowest F Value in list of Explored nodes
                grid[pos.Row, pos.Col] = 4;

                if(pos == end) // Hits the target, find path from target to start
                    return Path(grid, parents, end, start);
                for(int r = -1; r < 2; r++) // Visit all neighbours
                {
                    for(int c = -1; c < 2; c++)
                    {
                        Position n = new(pos.Row + r, pos.Col + c);

                        if((r != 0 && c != 0) || (r == 0 && c == 0) || WillHit(cells, n, 0, 0)  // Filter diagonal nodes, the current node, already closed nodes and smaller order nodes
                           || grid[n.Row,n.Col] == 4 || order[n.Row, n.Col] < order[pos.Row, pos.Col])
                        {
                            continue;
                        }

                        if(grid[n.Row, n.Col] != 3 || DistanceOnObserved(parents[n.Row, n.Col], start, parents) > DistanceOnObserved(pos, start, parents))
                        { // Changing parent of neighbour to current node, update Fcost of it
                            parents[n.Row, n.Col] = pos;
                            fcosts[n.Row, n.Col] = Distance(n, end, 0, 0) + DistanceOnObserved(n, start, parents);
                            grid[n.Row, n.Col] = grid[n.Row,n.Col] != 3 
                            ? 3 : grid[n.Row, n.Col]; // Add neighbour to Explored
                        }
                    }    
                }
            }
        }
    }
}