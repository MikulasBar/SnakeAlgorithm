using System;
namespace SnakeAl
{
    class Edge
    {
        public int value;
        public Position NodeA, NodeB;
        public bool active = false;
        public Edge(Position nodeA, Position nodeB)
        {
            Random rn = new Random();
            value = rn.Next(1,4);
            NodeA = nodeA; NodeB = nodeB;
        }
    }
}