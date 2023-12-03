using System.Collections.Generic;

namespace MazeCreator.Interfaces
{
    public interface IPathNodeNeighborFinder
    {
        IList<PathNode> GetNodeNeighbors(PathNode pathNode);
    }
}