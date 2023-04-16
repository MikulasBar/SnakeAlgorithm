using System.Collections.Generic;

namespace SnakeAl
{
    struct PrimsAlgorithm
    {
        bool IsActive(List<Edge> edges, Position nA, Position nB) // Returns true if edge is active
        {
            foreach(Edge e in edges)
            {
                if((e.NodeA == nA && e.NodeB == nB) 
                || (e.NodeA == nB && e.NodeB == nA))
                    return e.active;
            }
            return false;
        }
        bool CanStep(List<Edge> edges, Position pos, Direction dir) // Returns true if between start node and target node is not edge or if the edge is inactive
        {
            Position newpos = new(pos.Row + dir.rowDir, pos.Col + dir.colDir);
            Position node = new((pos.Row - pos.Row%2)/2, (pos.Col - pos.Col%2)/2);
            Position newnode = new((newpos.Row - newpos.Row%2)/2, (newpos.Col - newpos.Col%2)/2);
            if(node != newnode)
                return true;
            if(pos.Row == newpos.Row)
                return pos.Row % 2 == 0 ? !IsActive(edges, node, new(node.Row -1, node.Col)) : !IsActive(edges, node, new(node.Row +1, node.Col));
            else
                return pos.Col % 2 == 0 ? !IsActive(edges, node, new(node.Row, node.Col -1)) : !IsActive(edges, node, new(node.Row, node.Col +1));
        }
        bool AllVisited(int[,] nodes) // Returns true if all nodes are visited
        {
            for(int r = 0; r < nodes.GetLength(0); r++)
            {
                for(int c = 0; c < nodes.GetLength(1); c++)
                {
                    if(nodes[r,c] == 0)
                        return false;
                }
            }
            return true;
        }
        int MinimalEdge(int[,] nodes, List<Edge> edges) // Find edge with 1 node visited and 1 not, with minimal value
        {
            int index = 0, min = int.MaxValue;
            foreach(Edge i in edges)
            {
                if((nodes[i.NodeA.Row, i.NodeA.Col] == 0 && nodes[i.NodeB.Row, i.NodeB.Col] == 0) 
                || (nodes[i.NodeA.Row, i.NodeA.Col] == 1 && nodes[i.NodeB.Row, i.NodeB.Col] == 1))
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
        List<Edge> SetEdges(int[,] nodes) // Set all vertical and horizontal edges between nodes 
        {
            List<Edge> edges = new();
            for(int r = 0; r < nodes.GetLength(0); r++)
            {
                for(int c = 0; c < nodes.GetLength(1)-1; c++)
                {
                    edges.Add(new(new(r,c), new(r,c + 1)));
                    edges.Add(new(new(c,r), new(c + 1,r)));
                }
            }
            return edges;
        }
        List<Edge> MST(int rows,int cols) // Get Edges of Minimal spanning three of a graph
        {
            int[,] nodes = new int[rows/2,cols/2];
            List<Edge> edges = SetEdges(nodes);
            nodes[0,0] = 1;
            while(!AllVisited(nodes)) // Until all nodes are visited
            {
                Edge edge = edges[MinimalEdge(nodes, edges)]; // Find best edge
                edges[MinimalEdge(nodes, edges)].active = true; // Activate the edge
                if(nodes[edge.NodeA.Row, edge.NodeA.Col] == 0) // Switch recently visited node to visited
                    nodes[edge.NodeA.Row, edge.NodeA.Col] = 1;
                else
                    nodes[edge.NodeB.Row, edge.NodeB.Col] = 1;
            }
            return edges;
        }
        public (Direction[,], int[,]) HamiltonsCycle(int rows, int cols) // Creates grid of vectors from minimal spanning three, also create numeric order of cycle 
        {
            Direction[,] dirs = new Direction[rows,cols];
            int[,] order = new int[rows,cols];
            List<Edge> edges = MST(rows, cols); // Get Minimal spanning three
            Direction dir = new(0,1);
            Position pos = new(0,1);
            for(int i = 1; i < rows*cols; i++)
            {   // Loop tries to rotate as much as possible to the right
                if(CanStep(edges, pos, dir.Rotate("right"))) // First select right
                    dir = dir.Rotate("right");
                else if(CanStep(edges, pos, dir)) {} // If right is not option than straight
                else if(CanStep(edges, pos, dir.Rotate("left"))) // Left is last option
                    dir = dir.Rotate("left");
                
                order[pos.Row, pos.Col] = i; // Set order
                dirs[pos.Row, pos.Col] = dir; // Set vector in grid to selected direction
                pos = new(pos.Row + dir.rowDir, pos.Col + dir.colDir); // Change current positon to current position + selected vector as offset
            }
            dirs[0,0] = new(0,1);
            return (dirs, order);
        }
    }
}