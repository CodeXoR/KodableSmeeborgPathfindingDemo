using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CameraControls
{
    public class TilemapOrthoCameraControls : ICameraControls
    {
        private readonly Camera _gameCamera;
        private readonly Tilemap _tilemap;
        
        public TilemapOrthoCameraControls(Camera gameCamera, Tilemap tilemap)
        {
            _gameCamera = gameCamera;
            _tilemap = tilemap;
        }
        
        public async Task CenterToWorldBounds(float viewScaleModifier = 1f)
        {
            if (_gameCamera == null)
            {
                Debug.LogError("tilemap camera controls reference camera is null.");
                return;
            }
            
            if (_tilemap == null)
            {
                Debug.LogError("tilemap camera controls reference tilemap is null.");
                return;
            }
            
            _tilemap.CompressBounds();
        
            var bounds = _tilemap.localBounds;
            var vertical = bounds.size.y;
            var horizontal = bounds.size.x * _gameCamera.pixelHeight / _gameCamera.pixelWidth;
            var size = Mathf.Max(horizontal, vertical) * 0.5f;
            var center = bounds.center + new Vector3(0f, 0f, -10f);

            _gameCamera.transform.position = center;
            _gameCamera.orthographicSize = size * viewScaleModifier;

            await Task.CompletedTask;
        }
    }
}