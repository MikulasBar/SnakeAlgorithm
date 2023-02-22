using System;
using System.Linq;
using System.Collections.Generic;

namespace SnakeAl
{
    class PrimsAlgorithm
    {
        bool SameDir(Direction dir1, Direction dir2)
        {
            return dir1.rowDir == dir2.rowDir && dir1.colDir == dir2.colDir;
        }
        bool SamePos(Position pos1, Position pos2)
        {
            return pos1.Row == pos2.Row && pos1.Col == pos2.Col;
        }
        bool IsActive(List<Edge> edges, Position nA, Position nB)
        {
            foreach(Edge e in edges)
            {
                if((SamePos(e.NodeA, nA) && SamePos(e.NodeB, nB)) 
                || (SamePos(e.NodeA,nB) && SamePos(e.NodeB, nA)))
                {
                    return e.active;
                }
            }
            return false;
        }
        bool CanStep(List<Edge> edges, Position pos, Direction dir)
        {
            Position newpos = new Position(pos.Row + dir.rowDir, pos.Col + dir.colDir);
            Position node = new Position((pos.Row - pos.Row%2)/2, (pos.Col - pos.Col%2)/2);
            Position newnode = new Position((newpos.Row - newpos.Row%2)/2, (newpos.Col - newpos.Col%2)/2);
            if(!SamePos(node, newnode))
            {
                return true;
            }
            if(pos.Row == newpos.Row)
            {
                if(pos.Row % 2 == 0)
                {
                    return !IsActive(edges, node, new Position(node.Row -1, node.Col));
                }
                else
                {
                    return !IsActive(edges, node, new Position(node.Row +1, node.Col));
                }
            }
            else
            {
                if(pos.Col % 2 == 0)
                {
                    return !IsActive(edges, node, new Position(node.Row, node.Col -1));
                }
                else
                {
                    return !IsActive(edges, node, new Position(node.Row, node.Col +1));
                }
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
                if((nodes[i.NodeA.Row, i.NodeA.Col] == 0 && nodes[i.NodeB.Row, i.NodeB.Col] == 0) 
                || i.active == true || (nodes[i.NodeA.Row, i.NodeA.Col] == 1 && nodes[i.NodeB.Row, i.NodeB.Col] == 1))
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
            for(int i = 1; i < rows*cols; i++)
            {
                order[pos.Row, pos.Col] = i;
                if(CanStep(edges, pos, dir.Rotate("right")))
                {
                    dir = dir.Rotate("right");
                }
                else if(CanStep(edges, pos, dir)) {}
                else if(CanStep(edges, pos, dir.Rotate("left")))
                {
                    dir = dir.Rotate("left");
                }
                dirs[pos.Row, pos.Col] = dir;
                pos = new Position(pos.Row + dir.rowDir, pos.Col + dir.colDir);
            }
            order[0,0] = 0; dirs[0,0] = new Direction(0,1);
            return (dirs, order);
        }
    }
}