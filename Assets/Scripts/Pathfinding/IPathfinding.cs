using System.Collections.Generic;

namespace Pathfinding
{
    public interface IPathfinding
    {
        IList<PathNode> FindPath(PathNode startNode, PathNode endNode);
    }
}