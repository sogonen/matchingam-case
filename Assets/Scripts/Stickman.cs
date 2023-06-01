using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stickman : MonoBehaviour
{
    public SkinnedMeshRenderer _renderer; // Cache reference to renderer
    public float walkingSpeed = 2f; // Walking speed of the stickman
    public Animator animator;
    private List<Vector3> currentPath; // Current walking path
    private int currentPathIndex; // Current index in the walking path
    private int[] currentTile;
    private IslandManager islandManager;

    private IslandPair islandPair;
    private LevelManager levelManager;
    private StickmanColor stickmanColor; // Private backing field

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

    public void Run(List<Vector3> path, IslandManager islandManager, LevelManager levelManager, IslandPair islandPair,
        int tileX, int tileZ)
    {
        animator.SetBool("run", true);
        currentPath = path;
        currentTile = new[] { tileX, tileZ };
        this.islandManager = islandManager;
        this.levelManager = levelManager;
        this.islandPair = islandPair;

        StartCoroutine(WalkAlongPath());
    }

    private IEnumerator WalkAlongPath()
    {
        while (currentPathIndex < currentPath.Count)
        {
            var targetPosition = currentPath[currentPathIndex] + Vector3.up * 1.2f;
            while (transform.position != targetPosition)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, targetPosition, walkingSpeed * Time.deltaTime);
                var direction = targetPosition - transform.position;
                if (direction != Vector3.zero)
                {
                    // Rotate the stickman to face the target position
                    var toRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 5f * Time.deltaTime);
                }

                yield return null;
            }

            currentPathIndex++;
        }

        OnPathComplete();
    }

    private void OnPathComplete()
    {
        currentPathIndex = 0;
        animator.SetBool("run", false);
        var targetForward = islandPair.SecondIsland.transform.right;
        transform.forward = targetForward;
        transform.localPosition = new Vector3(0, 1.2f, 0);
        islandManager.checkStickmanCount(islandPair);
        levelManager.IsIslandCompleted(islandPair.SecondIsland);
    }
}