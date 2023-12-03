using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;

namespace MazeCreator
{
    /// <summary>
    /// This is an abstract scriptable object that holds data about what tiles are used by the program
    /// </summary>
    public abstract class TileConfig : ScriptableObject
    {
        // added quick hint here about what this tile config is about
        // would improve this with a custom inspector help box if time permits
        [Header("[  Note: set tile group addressable labels  ]")]
        [Space]
        public List<string> tileGroups;
        
        // using concurrent dictionary here for safe setting of entries across multiple asynchronous calls
        protected ConcurrentDictionary<string, Tile> TileSetMap;

        public virtual async Task Init()
        {
            TileSetMap = new ConcurrentDictionary<string, Tile>();
            
            // enforced loading of only one tile family at a time here for simplicity
            // but this can be extended easily to multiple tile groups as needed
            await Addressables.LoadAssetsAsync<Tile>(tileGroups,
                tile =>
                {
                    if(tile == null) return;
                    TileSetMap.TryAdd(tile.name, tile);
                },
                Addressables.MergeMode.Union,
                false).Task;
        }
        
        /// <summary>
        /// Retrieves a tile with string id from loaded tileset
        /// </summary>
        /// <param name="tileId">
        /// opted to use a string id here for flexibility
        /// </param>
        /// <returns></returns>
        public Tile GetTile(string tileId)
        {
            return TileSetMap.TryGetValue(GetTileKey(tileId), out var mappedTile) ? mappedTile : default;
        }

        /// <summary>
        /// Converts a tile id to a tile key mapped in loaded tileset
        /// </summary>
        /// <param name="tileId">
        /// the string tile id to be converted to a tile key
        /// </param>
        /// <returns></returns>
        protected abstract string GetTileKey(string tileId);
    }
}