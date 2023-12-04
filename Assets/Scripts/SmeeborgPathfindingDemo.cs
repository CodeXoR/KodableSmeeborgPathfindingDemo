using System.Collections;
using CameraControls;
using MazeCreator;
using MazeCreator.Interfaces;
using MazeCreator.ScriptableObjects;
using PlayerAnimator;
using PlayerAnimator.Interfaces;
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

    public GameObject player;
    public SpriteRenderer playerRenderer;
    public Animator playerAnimator;
    public float playerMoveSpeed;
    
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
        
        // pathfinding
        var grid = (IGrid)mazeCreator;
        IPathfinding pathfinding = new AStarSearchPathfinding(grid, new DiagonalDistanceCalculator(), new EightDirectionalNeighborNodesFinder(grid));
        var path = pathfinding.FindPath(grid.FirstWalkableNode, grid.LastWalkableNode);

        var playerTransform = player.transform;
        // tilemap coordinates go bottom up, y coordinate is inverted to get correct tilemap coordinates
        playerTransform.position = tilemap.GetCellCenterWorld(new Vector3Int(grid.FirstWalkableNode.x, -grid.FirstWalkableNode.y));

        if(path == null) yield break;
        
        // player movement
        IPlayerAnimator animator = new AstroBlueAnimator(playerAnimator, playerRenderer);
        var startNode = grid.FirstWalkableNode;
        const float distanceCheckTolerance = 0.05f;
        for (var i = path.Count - 1; i >= 0; i--)
        {
            var targetNode = path[i];
            var targetPosition = tilemap.GetCellCenterWorld(new Vector3Int(targetNode.x, -targetNode.y));
            animator.UpdateAnimation(startNode.x - targetNode.x, startNode.y - targetNode.y);
            
            while (Vector3.Distance(playerTransform.position, targetPosition) > distanceCheckTolerance)
            {
                playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPosition, Time.deltaTime * playerMoveSpeed);

                yield return null;
            }

            startNode = targetNode;
        }
        animator.EndAnimation();
    }
}