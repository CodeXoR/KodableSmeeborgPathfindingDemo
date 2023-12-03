using System.Collections.Generic;
using MazeCreator.Interfaces;

namespace MazeCreator
{
    public class AStarSearchPathfinding : IPathfinding
    {
        private readonly IGrid _grid;
        private readonly IPathNodeDistanceCalculator _distanceCalculator;
        private readonly IPathNodeNeighborFinder _pathNodeNeighborFinder;
        
        // used hashset for open and closed list for better search and remove performance
        private HashSet<PathNode> _openList;
        private HashSet<PathNode> _closedList;
        
        public AStarSearchPathfinding(IGrid grid, IPathNodeDistanceCalculator distanceCalculator, 
            IPathNodeNeighborFinder pathNodeNeighborFinder)
        {
            _grid = grid;
            
            // separated distance calculator following a strategy pattern approach
            // distance calculation is setup to be interchangeable
            _distanceCalculator = distanceCalculator;

            // neighbor finder is setup to be interchangeable depending on what movements are allowed
            // e.g. if diagonals are not allowed, a neighbor finder that only takes into account 
            //      neighbors at the down, top, left, right of current node will be more applicable
            _pathNodeNeighborFinder = pathNodeNeighborFinder;
        }
        
        public IList<PathNode> FindPath(PathNode startNode, PathNode endNode)
        {
            _openList = new HashSet<PathNode> { startNode };
            _closedList = new HashSet<PathNode>();

            // reset grid nodes
            var grid = _grid.Grid;
            for (var row = 0; row < grid.Count; row++)
            {
                for (var column = 0; column < grid[row].Count; column++)
                {
                    var pathNode = grid[row][column];
                    pathNode.g = int.MaxValue;
                    pathNode.ConnectedNode = null;
                }
            }

            startNode.g = 0;
            startNode.h = _distanceCalculator.CalculateDiagonalDistance(startNode, endNode);

            while (_openList.Count > 0)
            {
                var currentNode = GetLowestFCostNode();
                if (currentNode == endNode)
                {
                    return CalculatePath(endNode);
                }

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);
                
                var currentNodeNeighbors = _pathNodeNeighborFinder.GetNodeNeighbors(currentNode);

                for (var i = 0; i < currentNodeNeighbors.Count; i++)
                {
                    var neighborNode = currentNodeNeighbors[i];
                    
                    if(neighborNode.walkable == false || _closedList.Contains(neighborNode)) continue;

                    var gCostToNeighbor = currentNode.g +
                                          _distanceCalculator.CalculateDiagonalDistance(currentNode, neighborNode);
                    
                    if (gCostToNeighbor >= neighborNode.g) continue;
                    
                    neighborNode.g = gCostToNeighbor;
                    neighborNode.h = _distanceCalculator.CalculateDiagonalDistance(neighborNode, endNode);
                    neighborNode.ConnectedNode = currentNode;

                    // since we're using a hashset this call already checks
                    // if the container already contains the neighbor node 
                    _openList.Add(neighborNode);
                }
            }

            return null;
        }

        /// <summary>
        /// Calculates a path from start node to end node
        /// Note: the path returned will be in reverse
        /// opted to not reverse the path to save time,
        /// any consumer of the path can simply iterate through the node list in reverse
        /// </summary>
        private IList<PathNode> CalculatePath(PathNode endNode)
        {
            var path = new List<PathNode> { endNode };
            var currentNode = endNode;
            while (currentNode.ConnectedNode != null)
            {
                var pathNode = currentNode.ConnectedNode;
                path.Add(pathNode);
                currentNode = pathNode;
            }
            return path;
        }

        /// <summary>
        /// Finds the a node in current open list with the lowest F cost
        /// Note: search can be improved by using a separate container from the open list
        ///       in .Net6 using a priority queue would be best in this scenario
        /// </summary>
        private PathNode GetLowestFCostNode()
        {
            PathNode lowestFCostNode = null;
            
            foreach (var pathNode in _openList)
            {
                if (lowestFCostNode == null || pathNode.F < lowestFCostNode.F || 
                    // fallback check for shorter heuristic cost whenever f cost is the same
                    pathNode.F == lowestFCostNode.F && pathNode.h < lowestFCostNode.h)
                {
                    lowestFCostNode = pathNode;
                }
            }

            return lowestFCostNode;
        }
    }
}