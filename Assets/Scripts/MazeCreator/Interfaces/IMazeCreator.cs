using UnityEngine;

namespace MazeCreator.Interfaces
{
    public interface IMazeCreator
    {
        void SetupMaze(TextAsset mazeLayout);
    }
}