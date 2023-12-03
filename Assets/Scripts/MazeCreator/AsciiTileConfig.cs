using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MazeCreator
{
    [CreateAssetMenu(fileName = "AsciiTileConfig", menuName = "TileConfig/AsciiTileConfig", order = 0)]
    public class AsciiTileConfig : TileConfig
    {
        [Serializable]
        public class TileIdData
        {
            public List<string> ids;
            public string tileKeyMap;
        }
        
        // implemented an inspector editable data object here for mapping ascii tile ids
        // did it this way because Unity doesn't serialize dictionaries in the inspector
        // and I was aiming to have an editable object in the inspector for ease of testing and editing
        // alternatively, time and space can be saved if inspector editable values is not needed but that
        // would require editing this script if some values needed to be updated
        public List<TileIdData> tileIdData;
        private readonly Dictionary<string, string> _tileKeyMap = new();

        public override async Task Init()
        {
            for (var i = 0; i < tileIdData.Count; i++)
            {
                var tileData = tileIdData[i];
                var tileIds = tileData.ids;
                var tileKeyMap = tileData.tileKeyMap;
                
                for (var j = 0; j < tileIds.Count; j++)
                {
                    var id = tileIds[j];
                    _tileKeyMap.TryAdd(id, tileKeyMap);
                }
            }
            
            await base.Init();
        }
        
        protected override string GetTileKey(string tileId)
        {
            return _tileKeyMap.TryGetValue(tileId, out var tileKey) ? tileKey : default;
        }
    }
}