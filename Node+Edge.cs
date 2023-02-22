using System;
namespace SnakeAl
{
    class Edge
    {
        public int value;
        public Position NodeA, NodeB;
        public bool active;
        public Edge(Position nodeA, Position nodeB)
        {
            Random rn = new Random();
            value = rn.Next(1,6);
            NodeA = nodeA; NodeB = nodeB;
            active = false;
        }
    }
}