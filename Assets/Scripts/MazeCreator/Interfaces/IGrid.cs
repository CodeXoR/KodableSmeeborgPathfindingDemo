using System.Collections.Generic;

namespace MazeCreator.Interfaces
{
    public interface IGrid
    {
        // using list of lists here to accommodate maze rows of different lengths
        IList<IList<PathNode>> Grid { get; }
        
        PathNode FirstWalkableNode { get; }
        
        PathNode LastWalkableNode { get; }
    }
}