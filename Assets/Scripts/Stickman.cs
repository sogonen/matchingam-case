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

    private Island oldIsland;
    private Island newIsland;
    private BridgeManager bridgeManager;
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

    public void StartWalking(List<Vector3> path, BridgeManager bridgeManager, Island oldIsland, Island newIsland, int tileX, int tileZ)
    {
        animator.SetBool("Walking", true);
        currentPath = path;
        currentTile = new int[2];
        currentTile[0] = tileX;
        currentTile[1] = tileZ;
        this.bridgeManager = bridgeManager;
        this.oldIsland = oldIsland;
        this.newIsland = newIsland;
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
                // Adjust the rotation to face the direction of movement
                //Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, walkingSpeed * 5f * Time.deltaTime);

                yield return null;
            }

            // Move to the next point in the path
            currentPathIndex++;
        }

        // Path complete, do any necessary cleanup
        OnPathComplete();
    }

    private void OnPathComplete()
    {
        // Path is complete, handle any necessary logic
        animator.SetBool("Walking", false);
        newIsland.AddStickmanToTile(this, currentTile[0], currentTile[1]);
        bridgeManager.RemoveBridge(oldIsland, newIsland);
    }
}