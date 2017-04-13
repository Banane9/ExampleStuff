using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSolver
{
    internal class Node
    {
        private readonly Dictionary<Direction, Node> surroundingNodes = new Dictionary<Direction, Node>();
        public ElementType ElementType { get; private set; }

        public Node this[Direction direction]
        {
            get { return surroundingNodes[direction]; }
            set { surroundingNodes[direction] = value; }
        }

        public Node(ElementType elementType, Node up = null, Node right = null, Node down = null, Node left = null)
        {
            ElementType = elementType;

            surroundingNodes[Direction.Up] = up;
            surroundingNodes[Direction.Right] = right;
            surroundingNodes[Direction.Down] = down;
            surroundingNodes[Direction.Left] = left;
        }

        public bool HasNeighborFor(Direction direction)
        {
            return surroundingNodes[direction] != null;
        }
    }
}