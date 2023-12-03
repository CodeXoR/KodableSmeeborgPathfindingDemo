using System.Collections;
using CameraControls;
using MazeCreator;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;

public class SmeeborgPathfindingDemo : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileConfig tileConfig;
    
    // using an indirect reference here of the maze text layout as an addressable
    // to implement on demand loading leveraging the addressables api
    [SerializeField] private AssetReferenceT<TextAsset> mazeLayout;
    
    private IEnumerator Start()
    {
        // doing a check if maze layout config is set
        // AssetReference type will always have an object reference even when not set in the inspector
        // to check if AssetReference is valid, the runtime key associated is checked for validity instead
        // if config is not set a log is printed to display some helpful debugging info
        // in an actual live game, a message popup can be shown instead for better feedback
        if (mazeLayout.RuntimeKeyIsValid() == false)
        {
            Debug.Log("maze layout config is invalid.");
            yield break;
        }

        if (tilemap == null)
        {
            Debug.Log("tilemap is not set.");
            yield break;
        }
        
        if (tileConfig == null)
        {
            Debug.Log("tile config is not set.");
            yield break;
        }
        
        var mazeConfigLoadTask = mazeLayout.LoadAssetAsync<TextAsset>().Task;
        var tileConfigInitTask = tileConfig.Init();

        // coroutine will wait for both maze config load and tile config init to complete
        yield return new WaitUntil(() => mazeConfigLoadTask.IsCompleted && tileConfigInitTask.IsCompleted);
        
        // following the dependency inversion principle, demo depends on abstractions
        // instead of any concrete implementation reference. This enables us to easily and safely swap out 
        // different implementations, either for testing or extending the features of the default maze creator
        IMazeCreator mazeCreator = new AsciiMazeCreator(tilemap, tileConfig);

        mazeCreator.SetupMaze(mazeConfigLoadTask.Result);
        
        // release maze layout text asset once done with maze setup
        mazeLayout.ReleaseAsset();

        ICameraControls cameraControls = new TilemapOrthoCameraControls(gameCamera, tilemap);

        // implemented centering of maze world bounds for better viewing experience
        // added 10% padding along the longer world bound
        var cameraSetupTask = cameraControls.CenterToWorldBounds(viewScaleModifier: 1.1f);
        
        yield return new WaitUntil(() => cameraSetupTask.IsCompleted);
    }
}