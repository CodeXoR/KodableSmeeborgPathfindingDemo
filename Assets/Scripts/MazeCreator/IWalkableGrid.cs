using System.Collections.Generic;
using UnityEngine;

namespace MazeCreator
{
    public interface IWalkableGrid
    {
        // using list of lists here to accommodate maze rows of different lengths
        IList<IList<bool>> WalkableMap { get; }
        
        Vector2Int StartCoords { get; }
        
        Vector2Int EndCoords { get; }
    }
}