using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MazeCreator
{
    public class AsciiMazeCreator : IMazeCreator
    {
        // maze creator uses Unity tilemap system for better overall performance
        private readonly Tilemap _tilemap;
        private readonly TileConfig _tileConfig;
        
        // cached char to string conversion to avoid creating new strings for the same characters 
        private readonly Dictionary<char, string> _charToStringMap = new();
        
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
                        continue;
                }

                _charToStringMap.TryAdd(mazeElement, mazeElement.ToString());

                // tilemap coordinates go bottom up, in order to render tiles top to bottom, row coordinate is negated
                _tilemap.SetTile(new Vector3Int(column, -row), _tileConfig.GetTile(_charToStringMap[mazeElement]));
                column++;
            }
        }
    } 
}