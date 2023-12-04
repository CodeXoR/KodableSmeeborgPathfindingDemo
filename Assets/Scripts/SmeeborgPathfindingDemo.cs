using System.Collections;
using CameraControls;
using MazeCreator;
using MazeCreator.Interfaces;
using MazeCreator.ScriptableObjects;
using Player;
using Player.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

public class SmeeborgPathfindingDemo : MonoBehaviour
{
    // some helpful map markers for visualization
    public GameObject startMarker;
    public GameObject endMarker;
    
    [SerializeField] private Camera gameCamera;
    // using a tilemap for the maze instead of 2D sprite objects for better overall performance
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileConfig tileConfig;
    // using an indirect reference here of the player prefab and maze text layout as addressables
    // to implement on demand loading leveraging the addressables api
    [SerializeField] private AssetReferenceT<GameObject> playerPrefab;
    [SerializeField] private AssetReferenceT<TextAsset> mazeLayout;

    private IEnumerator Start()
    {
        // the following code have some debug logs to display some helpful debugging info
        // in an actual live game, a message popup can be shown instead for better feedback
        
        if (gameCamera == null)
        {
            Debug.LogError("game camera is not set.");
            yield break;
        }
        
        // AssetReference type will always have an object reference even when not set in the inspector
        // to check if AssetReference is valid, the runtime key associated is checked for validity instead
        if (mazeLayout.RuntimeKeyIsValid() == false)
        {
            Debug.LogError("addressable maze layout config is invalid.");
            yield break;
        }
        
        if (playerPrefab.RuntimeKeyIsValid() == false)
        {
            Debug.LogError("addressable player prefab is invalid.");
            yield break;
        }

        if (tilemap == null)
        {
            Debug.LogError("tilemap is not set.");
            yield break;
        }
        
        if (tileConfig == null)
        {
            Debug.LogError("tile config is not set.");
            yield break;
        }
        
        var mazeConfigLoadTask = mazeLayout.LoadAssetAsync<TextAsset>().Task;
        var tileConfigInitTask = tileConfig.Init();

        // coroutine will wait for both maze config load and tile config init to complete
        yield return new WaitUntil(() => mazeConfigLoadTask.IsCompleted && tileConfigInitTask.IsCompleted);
        
        // for better modularity and testability,
        // the following code setup is following an inversion of control pattern through dependency injection
        // dependencies are set via constructor injection
        
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
        
        IGrid grid = (IGrid)mazeCreator;
        
        // tilemap coordinates go bottom up, y coordinate is inverted to get correct tilemap coordinates
        var playerStartPosition = tilemap.GetCellCenterWorld(new Vector3Int(grid.FirstWalkableNode.x, -grid.FirstWalkableNode.y));
        var playerEndPosition = tilemap.GetCellCenterWorld(new Vector3Int(grid.LastWalkableNode.x, -grid.LastWalkableNode.y));
        
        // set maze start marker
        if (startMarker != null)
        {
            startMarker.transform.position = playerStartPosition;
        }

        // set maze end marker
        if (endMarker != null)
        {
            endMarker.transform.position = playerEndPosition;
        }
        
        // player spawn
        var loadPlayerOp = Addressables.InstantiateAsync(playerPrefab, playerStartPosition, Quaternion.identity);
        yield return loadPlayerOp;

        if (loadPlayerOp.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("loading of addressable player prefab had an error");
            yield break;
        }
        
        var playerInstance = loadPlayerOp.Result;

        if (playerInstance == null)
        {
            Debug.LogError("player instance is null");
            yield break;
        }
        
        var player = playerInstance.GetComponent<PathfindingPlayer>();

        if (player == null)
        {
            Debug.LogError("player instance PathfindingPlayer component is null");
            yield break;
        }
        
        // player animation
        IPlayerAnimator animator = new AstroBlueAnimator(player.playerAnimator, player.playerRenderer);
        animator.StartAnimation();
        
        // pathfinding
        IPathfinding pathfinding = new AStarSearchPathfinding(grid, new DiagonalDistanceCalculator(), new EightDirectionalNeighborNodesFinder(grid));
        var path = pathfinding.FindPath(grid.FirstWalkableNode, grid.LastWalkableNode);
        
        if (path == null)
        {
            // showing player end animation whenever there is no valid path for feedback
            animator.EndAnimation();
            Debug.LogError("there is no valid path to destination.");
            yield break;
        }
        
        // player movement
        var playerTransform = player.transform;
        var startNode = grid.FirstWalkableNode;
        const float distanceCheckTolerance = 0.05f;
        
        // traversing path in reverse since path returned by pathfinding is in reverse
        for (var i = path.Count - 1; i >= 0; i--)
        {
            var targetNode = path[i];
            var targetPosition = tilemap.GetCellCenterWorld(new Vector3Int(targetNode.x, -targetNode.y));
            
            // player animation is updated based on 2D move direction and player speed
            // player will shows run animation if player speed is >= 4f, walk animation otherwise
            animator.UpdateAnimation(player.playerMoveSpeed, startNode.x - targetNode.x, startNode.y - targetNode.y);
            
            while (Vector3.Distance(playerTransform.position, targetPosition) > distanceCheckTolerance)
            {
                playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPosition, Time.deltaTime * player.playerMoveSpeed);

                yield return null;
            }

            startNode = targetNode;
        }
        animator.EndAnimation();
    }
}