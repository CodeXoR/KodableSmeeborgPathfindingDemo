namespace MazeCreator.Interfaces
{
    public interface IPathNodeDistanceCalculator
    {
        int CalculateDiagonalDistance(PathNode startNode, PathNode endNode);
    }
}