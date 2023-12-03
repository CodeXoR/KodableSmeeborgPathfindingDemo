using MazeCreator.Interfaces;
using UnityEngine;

namespace MazeCreator
{
    public class DiagonalDistanceCalculator : IPathNodeDistanceCalculator
    {
        /// <summary>
        /// Calculates distance between two nodes with respect to diagonal movement
        /// </summary>
        /// <returns> approximate diagonal distance between start node and end node </returns>
        public int CalculateDiagonalDistance(PathNode startNode, PathNode endNode)
        {
            // the following const variables are scaled values by 10 for simplicity
            // unscaled horizontal move cost = 1
            // unscaled diagonal move cost = 1.414 or sqrt(2)
            // these are const variables declared in local scope to avoid heap memory allocation
            const int horizontalMoveCost = 10;
            const int diagonalMoveCost = 14;

            var horizontalDistance = Mathf.Abs(startNode.x - endNode.x);
            var verticalDistance = Mathf.Abs(startNode.y - endNode.y);

            var shorterDistance = Mathf.Min(horizontalDistance, verticalDistance);
            var longerDistance = Mathf.Max(horizontalDistance, verticalDistance);

            var numHorizontalMoves = longerDistance - shorterDistance;

            return shorterDistance * diagonalMoveCost + numHorizontalMoves * horizontalMoveCost;
        }
    }
}