using System;
using System.Collections.Generic;

namespace SnakeAl
{
    class PrimsAlgorithm
    {
        bool CanStep(List<Edge> edges, Position pos, Position newpos)
        {
            Position node = new Position((pos.Row - pos.Row%2)/2, (pos.Col - pos.Col%2)/2);
            Position newnode = new Position((newpos.Row - newpos.Row%2)/2, (newpos.Col - newpos.Col%2)/2);
            if(node != newnode)
            {
                return true;
            }
        }
        bool AllVisited(int[,] nodes)
        {
            for(int r = 0; r < nodes.GetLength(0); r++)
            {
                for(int c = 0; c < nodes.GetLength(1); c++)
                {
                    if(nodes[r,c] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        int MinimalEdge(int[,] nodes, List<Edge> edges)
        {
            int index = 0, min = int.MaxValue;
            foreach(Edge i in edges)
            {
                if(nodes[i.NodeA.Row, i.NodeA.Col] == 0 && nodes[i.NodeB.Row, i.NodeB.Col] == 0 || i.active == true)
                {
                    continue;
                }
                if(min > i.value)
                {
                    min = i.value;
                    index = edges.IndexOf(i);
                }
            }
            return index;
        }
        List<Edge> SetEdges(int[,] nodes)
        {
            List<Edge> edges = new List<Edge>();
            for(int r = 0; r < nodes.GetLength(0); r++)
            {
                for(int c = 0; c < nodes.GetLength(1)-1; c++)
                {
                    edges.Add(new Edge(new Position(r,c), new Position(r,c + 1)));
                    edges.Add(new Edge(new Position(c,r), new Position(c + 1,r)));
                }
            }
            return edges;
        }
        List<Edge> MST(int rows,int cols)
        {
            int[,] nodes = new int[rows/2,cols/2];
            List<Edge> edges = new List<Edge>();
            edges = SetEdges(nodes);
            nodes[0,0] = 1;
            while(!AllVisited(nodes))
            {
                Edge edge = edges[MinimalEdge(nodes, edges)];
                edges[MinimalEdge(nodes, edges)].active = true;
                if(nodes[edge.NodeA.Row, edge.NodeA.Col] == 0)
                {
                    nodes[edge.NodeA.Row, edge.NodeA.Col] = 1;
                }
                else
                {
                    nodes[edge.NodeB.Row, edge.NodeB.Col] = 1;
                }
            }
            return edges;
        }
        public (Direction[,], int[,]) HamiltonsCycle(int rows, int cols)
        {
            Direction[,] dirs = new Direction[rows,cols];
            int[,] order = new int[rows,cols];
            List<Edge> edges = MST(rows, cols);
            Direction dir = new Direction(0,1);
            Position pos = new Position(0,1);
            Dictionary<Direction, Direction> rotate = new ()
            {
                {new Direction(0,1), new Direction(1,0)}, {new Direction(1,0), new Direction(0,-1)},
                {new Direction(0,-1), new Direction(-1,0)}, {new Direction(-1,0), new Direction(0,1)}
            };
            for(int i = 1; i < rows*cols; i++)
            {
                order[pos.Row, pos.Col] = i;
                if()
                {
                    dir = rotate[dir];
                }
                else if()
                {
                    dir = rotate[dir];
                    dir.rowDir *= -1;
                    dir.colDir *= -1;
                }
                pos = new Position(pos.Row + dir.rowDir, pos.Col + dir.colDir);
            }
            return (dirs, order);
        }
    }
}