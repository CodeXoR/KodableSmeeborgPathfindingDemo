namespace MazeCreator
{
    public class PathNode
    {
        /// <summary>
        /// the x coordinate of this node in a grid
        /// </summary>
        public readonly int x;
        
        /// <summary>
        /// the y coordinate of this node in a grid
        /// </summary>
        public readonly int y;
        
        /// <summary>
        /// a movement cost from start node to this node
        /// </summary>
        public int g;
        
        /// <summary>
        /// a heuristic cost from this node to end node
        /// </summary>
        public int h;
        
        /// <summary>
        /// the total movement cost to go to this node
        /// made this into a property to save memory allocation of the cost of int * number of nodes
        /// g + h
        /// </summary>
        public int F => g + h;

        public bool walkable;
        
        public PathNode ConnectedNode { get; set; }

        public PathNode(int xCoord, int yCoord, bool isWalkable)
        {
            x = xCoord;
            y = yCoord;
            walkable = isWalkable;
        }
    }
}