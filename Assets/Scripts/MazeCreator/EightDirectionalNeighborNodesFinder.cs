using System.Collections.Generic;
using MazeCreator.Interfaces;

namespace MazeCreator
{
    public class EightDirectionalNeighborNodesFinder : IPathNodeNeighborFinder
    {
        private readonly IGrid _grid;
        // cached list container for holding current node neighbors
        private readonly List<PathNode> _currentNodeNeighborList = new();

        public EightDirectionalNeighborNodesFinder(IGrid grid)
        {
            _grid = grid;
        }
        
        public IList<PathNode> GetNodeNeighbors(PathNode pathNode)
        {
            _currentNodeNeighborList.Clear();

            var grid = _grid.Grid;
            var column = pathNode.x;
            var row = pathNode.y;

            var down = row - 1;
            var up = row + 1;
            var left = column - 1;
            var right = column + 1;
            
            if (left >= 0)
            {
                // left
                _currentNodeNeighborList.Add(grid[row][left]);
                
                // left down
                if (down >= 0 && left < grid[down].Count)
                {
                    _currentNodeNeighborList.Add(grid[down][left]);
                }
                
                // left up
                if (up < grid.Count && left < grid[up].Count)
                {
                    _currentNodeNeighborList.Add(grid[up][left]);
                }
            }
            
            if (right < grid[row].Count)
            {
                // right
                _currentNodeNeighborList.Add(grid[row][right]);
            }

            if (down >= 0 && right < grid[down].Count)
            {
                // right down
                _currentNodeNeighborList.Add(grid[down][right]);
            }

            if (up < grid.Count && right < grid[up].Count)
            {
                // right up
                _currentNodeNeighborList.Add(grid[up][right]);
            }

            if (down >= 0 && column < grid[down].Count)
            {
                // down
                _currentNodeNeighborList.Add(grid[down][column]);
            }

            if (up < grid.Count && column < grid[up].Count)
            {
                // up
                _currentNodeNeighborList.Add(grid[up][column]);
            }

            return _currentNodeNeighborList;
        }
    }
}