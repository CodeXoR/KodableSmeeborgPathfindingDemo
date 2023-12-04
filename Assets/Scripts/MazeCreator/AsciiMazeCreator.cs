using System.Collections.Generic;
using MazeCreator.Interfaces;
using MazeCreator.ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MazeCreator
{
    public class AsciiMazeCreator : IMazeCreator, IGrid
    {
        // maze creator uses Unity tilemap system for better overall performance
        private readonly Tilemap _tilemap;
        private readonly TileConfig _tileConfig;
        
        // cached char to string conversion to avoid creating new strings for the same characters 
        private readonly Dictionary<char, string> _charToStringMap = new();

        public IList<IList<PathNode>> Grid { get; } = new List<IList<PathNode>>();
        public PathNode FirstWalkableNode { get; private set; }
        public PathNode LastWalkableNode { get; private set; }

        public AsciiMazeCreator(Tilemap tilemap, TileConfig tileConfig)
        {
            _tilemap = tilemap;
            _tileConfig = tileConfig;
        }

        ~AsciiMazeCreator()
        {
            _charToStringMap.Clear();
        }
        
        public void SetupMaze(TextAsset mazeLayout)
        {
            // the way the demo is setup, mazeLayout will always be valid
            // keeping this check here still in case the maze creator is used without input validation
            if(mazeLayout == null) return;

            var mazeInfo = mazeLayout.text;

            var row = 0;
            var column = 0;
            
            Grid.Add(new List<PathNode>());

            for (var charIndex = 0; charIndex < mazeInfo.Length; charIndex++)
            {
                var mazeElement = mazeInfo[charIndex];

                switch (mazeElement)
                {
                    // checking for other version of newline at end of string to avoid extra walls rendered
                    case '\r':
                        continue;
                    case '\n':
                        row++;
                        column = 0;
                        Grid.Add(new List<PathNode>());
                        continue;
                }

                _charToStringMap.TryAdd(mazeElement, mazeElement.ToString());

                var tileId = _charToStringMap[mazeElement];
                var tile = _tileConfig.GetTile(tileId);
                var isTileWalkable = _tileConfig.IsTileWalkable(tileId);

                var pathNode = new PathNode(column, row, isTileWalkable);
                Grid[row].Add(pathNode);
                
                // tilemap coordinates go bottom up, in order to render tiles top to bottom, row coordinate is negated
                _tilemap.SetTile(new Vector3Int(column, -row), tile);
                
                // first walkable tile from the top-left corner would be the starting point of the maze
                // last walkable tile from the bottom-right corner would be the end point of the maze
                if (isTileWalkable)
                {
                    LastWalkableNode = pathNode;
                    FirstWalkableNode ??= pathNode;
                }
                
                column++;
            }
        }
    } 
}