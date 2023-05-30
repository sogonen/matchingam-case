using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Stickman : MonoBehaviour
{
    public SkinnedMeshRenderer _renderer; // Cache reference to renderer
    public float walkingSpeed = 2f; // Walking speed of the stickman
    public Animator animator;
    private StickmanColor stickmanColor; // Private backing field
    private List<Vector3> currentPath; // Current walking path
    private int currentPathIndex; // Current index in the walking path

    private IslandPair islandPair;
    private BridgeManager bridgeManager;
    private LevelManager levelManager;
    private int[] currentTile;

    public StickmanColor Color
    {
        get => stickmanColor;
        set
        {
            stickmanColor = value;
            var material = ColorManager.Instance.GetMaterialForColor(value);
            if (material != null) _renderer.material = material;
        }
    }

    private void Start()
    {
        _renderer = GetComponent<SkinnedMeshRenderer>();
    }

    public void StartWalking(List<Vector3> path, BridgeManager bridgeManager, LevelManager levelManager, IslandPair islandPair, int tileX, int tileZ)
    {
        animator.SetBool("Walking", true);
        currentPath = path;
        currentTile = new int[2];
        currentTile[0] = tileX;
        currentTile[1] = tileZ;
        this.bridgeManager = bridgeManager;
        this.levelManager = levelManager;
        this.islandPair = islandPair;
        // Start the walking coroutine
        StartCoroutine(WalkAlongPath());
    }

    private System.Collections.IEnumerator WalkAlongPath()
    {
        while (currentPathIndex < currentPath.Count)
        {
            Vector3 targetPosition = currentPath[currentPathIndex] + Vector3.up * 0.7f;
            while (transform.position != targetPosition)
            {
                // Move the stickman towards the target position
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkingSpeed * Time.deltaTime);

                yield return null;
            }
            
            currentPathIndex++;
        }
        
        OnPathComplete();
    }

    private void OnPathComplete()
    {
        animator.SetBool("Walking", false);
        bridgeManager.RemoveBridge(islandPair);
        levelManager.IslandCompleted(islandPair.SecondIsland);
    }
}