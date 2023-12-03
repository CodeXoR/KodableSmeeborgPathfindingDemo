using UnityEngine;

namespace MazeCreator
{
    public interface IMazeCreator
    {
        void SetupMaze(TextAsset mazeLayout);
    }
}