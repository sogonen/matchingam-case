using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Island islandPrefab; // Assign this in the Inspector
    public List<Island> islands = new();
    public UIScreenManager screenManager;
    public UIScreen levelCompleteScreen;
    public UIScreen adScreen;
    public UIScreen settingsScreen;
    public int currentLevel = 1;
    public BridgeManager bridgeManager;
    public IslandManager islandManager;
    public LevelManager levelManager;
    private Camera mainCamera;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        mainCamera = Camera.main;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", 1);

        bridgeManager = new BridgeManager();
        levelManager = new LevelManager();
        islandManager = new IslandManager(bridgeManager, levelManager);

        StartLevel();
    }

    public void SpawnIslands(int number)
    {
        // Calculate the position of the islands based on the camera size
        var halfCameraHeight = mainCamera.orthographicSize;
        var halfCameraWidth = mainCamera.aspect * halfCameraHeight;

        // Calculate the number of islands for each side
        var numberOfIslandsOnEachSide = number / 2;

        // Calculate the island spacing for each side
        var islandSpacing = 2f * halfCameraHeight / (numberOfIslandsOnEachSide + 1);

        for (var i = 0; i < number; i++)
        {
            // Calculate z position (depth)
            var zPosition = i / 2f * islandSpacing;

            // If it is odd, place on right side of screen; if even, place on left
            var xPosition = i % 2 != 0 ? halfCameraWidth / 1.5f : -halfCameraWidth / 1.5f;

            var islandPosition = new Vector3(xPosition, 0, zPosition);

            // Instantiate the island at the calculated position
            var newIsland = Instantiate(islandPrefab, islandPosition, Quaternion.identity);
            newIsland.name = "Island " + i;
            newIsland.index = i;
            newIsland.Initialize(islandManager);

            // Rotate the right island by 180 degrees
            newIsland.transform.Rotate(Vector3.up, i % 2 != 0 ? 180 : 0);

            islands.Add(newIsland);
        }
    }

    // public List<Island> GetShuffledIslands()
    // {
    //     var shuffledIslands = new List<Island>(islands);
    //     var islandCount = shuffledIslands.Count;
    //
    //     // Fisher-Yates shuffle algorithm
    //     for (var i = 0; i < islandCount - 1; i++)
    //     {
    //         var randomIndex = Random.Range(i, islandCount);
    //         var temp = shuffledIslands[randomIndex];
    //         shuffledIslands[randomIndex] = shuffledIslands[i];
    //         shuffledIslands[i] = temp;
    //     }
    //
    //     return shuffledIslands;
    // }

    public void NextLevel()
    {
        screenManager.DequeueScreen();
        StartLevel();
    }

    public void ShowAdScreen()
    {
        screenManager.EnqueueScreen(adScreen);
    }

    public void ShowSettingsScreen()
    {
        screenManager.EnqueueScreen(settingsScreen);
    }

    public void StartLevel()
    {
        screenManager.SetLevelText(currentLevel);
        ClearLevel();
        levelManager.GenerateNextLevel(this, currentLevel - 1);
    }

    public void RestartLevel()
    {
        if (!screenManager.IsScreenQueueEmpty())
            screenManager.DequeueScreen();
        StartLevel();
    }

    public void UnlockAdIsland()
    {
        var adIsland = islands.Where(t => t.IsAdIsland).FirstOrDefault();
        adIsland.Unlock();
        screenManager.DequeueScreen();
    }

    public void ResetGame()
    {
        PlayerPrefs.SetInt("currentLevel", 1);
        currentLevel = 1;
        RestartLevel();
    }

    public void LevelCompleted()
    {
        if (currentLevel < levelManager.LevelCount)
            currentLevel++;
        PlayerPrefs.SetInt("currentLevel", currentLevel);

        StartCoroutine(ShowLevelCompleteScreen());
    }

    private IEnumerator ShowLevelCompleteScreen()
    {
        yield return new WaitUntil(() => islandManager.NoMovementInProgress());
        screenManager.EnqueueScreen(levelCompleteScreen);
    }

    private void ClearLevel()
    {
        // Return Stickmans to the pool
        foreach (var island in islands)
            // Return stickmen to the pool
        foreach (var tile in island.tiles)
            if (tile.HasStickman)
            {
                var stickman = tile.GetStickman();
                PoolManager.Instance.ReturnStickmanToPool(stickman);
            }

        // Clear existing bridges
        bridgeManager.ClearBridges();
        // Reset the completed island count
        levelManager.CompletedIslandCount = 0;
        islandManager.Clear();
        // Clear existing islands
        foreach (var island in islands) Destroy(island.gameObject);
        islands.Clear();
    }
}