using System.Collections.Generic;

namespace MazeCreator.Interfaces
{
    public interface IPathfinding
    {
        IList<PathNode> FindPath(PathNode startNode, PathNode endNode);
    }
}