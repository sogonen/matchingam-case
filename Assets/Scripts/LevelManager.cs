using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelManager
{
    private GameManager gameManager;
    private int islandNumbers;
    private readonly List<LevelData> levels;
    private int stickmanSetNumbers;

    public LevelManager()
    {
        if (levels == null) levels = LoadLevelsFromJSON();
    }

    public int LevelCount => levels.Count;

    public int CompletedIslandCount { get; set; }

    private List<LevelData> LoadLevelsFromJSON()
    {
        var levelDataList = new List<LevelData>();
        var filePath = Path.Combine(Application.streamingAssetsPath, "levels.json");
        if (Application.platform == RuntimePlatform.Android)
            LoadLevelsFromAndroid(filePath, levelDataList);
        else
            LoadLevelsFromFile(filePath, levelDataList);

        return levelDataList;
    }

    private void LoadLevelsFromFile(string filePath, List<LevelData> levelDataList)
    {
        if (File.Exists(filePath))
        {
            var jsonData = File.ReadAllText(filePath);
            var levelDataListContainer = JsonUtility.FromJson<LevelDataList>(jsonData);

            if (levelDataListContainer != null) levelDataList.AddRange(levelDataListContainer.levels);
        }
    }

    public void LoadLevelsFromAndroid(string filePath, List<LevelData> levelDataList)
    {
        string jsonString;

        if (Application.platform == RuntimePlatform.Android)
        {
            var reader = new WWW(filePath);
            while (!reader.isDone)
            {
            }

            jsonString = reader.text;
        }
        else
        {
            jsonString = File.ReadAllText(filePath);
        }

        var levelDataListContainer = JsonUtility.FromJson<LevelDataList>(jsonString);

        if (levelDataListContainer != null) levelDataList.AddRange(levelDataListContainer.levels);
    }

    /* This part was the auto level generate which I first wanted to implement , but then I decided go with the prebuilt levels as in the actual game. I am just leaving it here as it can be improved */
    // public void GenerateNextLevel(GameManager gameManager,int levelIndex)
    // {
    //     this.gameManager = gameManager;
    //     
    //     gameManager.bridgeManager.ClearBridges();
    //     completedIslandCount = 0;
    //     islandNumbers = levels[levelIndex].islandNumbers;
    //     stickmanSetNumbers = levels[levelIndex].stickmanSetNumbers;
    //     // Create islands
    //     this.gameManager.SpawnIslands(islandNumbers);
    //
    //     // Set the stickman colors for this currentLevel
    //     var stickmanColors = ColorManager.Instance.GetStickmanColors(stickmanSetNumbers);
    //
    //     // Shuffle the islands randomly
    //     var shuffledIslands = gameManager.GetShuffledIslands();
    //
    //     // Determine the number of stickman colors used in this currentLevel
    //     var usedColorCount = Mathf.Min(stickmanSetNumbers, stickmanColors.Count);
    //
    //     // Determine the number of columns needed based on the number of stickman colors
    //     var columnsNeeded = Mathf.CeilToInt(4f * usedColorCount);
    //
    //     // Keep track of the stickman colors used in each column of each island
    //     var islandColumnColors = new Dictionary<Island, List<List<StickmanColor>>>();
    //
    //     // Loop through each column index
    //     for (var columnIndex = 0; columnIndex < columnsNeeded; columnIndex++)
    //     {
    //         // Select a stickman color for this column
    //         var colorIndex = columnIndex % usedColorCount;
    //         var columnColor = stickmanColors[colorIndex];
    //
    //         // Loop through each island
    //         foreach (var island in shuffledIslands)
    //         {
    //             // Determine the column index within the island
    //             var islandColumnIndex = columnIndex % island.size;
    //
    //             // Get the stickman colors used in this column of the island
    //             List<StickmanColor> columnColors;
    //             if (islandColumnColors.ContainsKey(island))
    //             {
    //                 columnColors = islandColumnColors[island][islandColumnIndex];
    //             }
    //             else
    //             {
    //                 columnColors = new List<StickmanColor>();
    //                 islandColumnColors[island] = new List<List<StickmanColor>>();
    //                 for (var i = 0; i < island.size; i++) islandColumnColors[island].Add(new List<StickmanColor>());
    //             }
    //
    //             // Check if the column already contains the selected color
    //             if (columnColors.Contains(columnColor))
    //             {
    //                 // Select a different color that hasn't been used in this column
    //                 var unusedColors = stickmanColors.Except(columnColors).ToList();
    //                 columnColor = unusedColors[Random.Range(0, unusedColors.Count)];
    //             }
    //
    //             // Add the selected color to the column colors
    //             columnColors.Add(columnColor);
    //
    //             // Loop through each row within the column
    //             for (var rowIndex = 0; rowIndex < island.size; rowIndex++)
    //             {
    //                 // Get a Stickman from the pool with the specified color
    //                 var stickman = PoolManager.Instance.GetStickmanFromPool(columnColor);
    //                 
    //                 // Add the Stickman to the tile in the island
    //                 if (stickman != null)
    //                     island.AddStickmanToTile(stickman, islandColumnIndex, rowIndex);
    //             }
    //         }
    //     }
    // }

    public void GenerateNextLevel(GameManager gameManager, int levelIndex)
    {
        this.gameManager = gameManager;

        var adIslandIndex = levels[levelIndex].ad;
        islandNumbers = levels[levelIndex].islandNumbers;
        stickmanSetNumbers = levels[levelIndex].stickmanSetNumbers;

        // Create islands
        this.gameManager.SpawnIslands(islandNumbers);

        // Set the stickman colors for this currentLevel
        var stickmanColors = ColorManager.Instance.GetStickmanColors(stickmanSetNumbers);

        // Get the island pattern for this level
        var islandPattern = levels[levelIndex].islands;

        // Loop through each island and set the stickman colors based on the island pattern
        for (var i = 0; i < islandNumbers; i++)
        {
            var islandData = islandPattern[i];
            var island = this.gameManager.islands[i];
            if (i == adIslandIndex)
            {
                island.IsAdIsland = true;
                island.adIcon = Object.Instantiate(PoolManager.Instance.adPrefab, island.transform);
            }

            for (var j = 0; j < islandData.islandPattern.Count; j++)
            {
                var colorIndex = islandData.islandPattern[j];
                if (colorIndex >= 0)
                    // Loop through each row within the column
                    for (var rowIndex = 0; rowIndex < island.size; rowIndex++)
                    {
                        // Get a Stickman from the pool with the specified color
                        var stickman =
                            PoolManager.Instance.GetStickmanFromPool(
                                ColorManager.Instance.GetStickmanColor(colorIndex));
                        // Add the Stickman to the tile in the island
                        if (stickman != null)
                            island.AddStickmanToTile(stickman, j, rowIndex);
                    }
            }
        }
    }

    public void IsIslandCompleted(Island currentIsland)
    {
        // Get the stickmen on the island
        var stickmen = currentIsland.GetAllStickmen();

        // Check if all stickmen on the island have the same color
        if (!currentIsland.IsCompleted && stickmen.Count == Mathf.Pow(currentIsland.size, 2) &&
            stickmen.All(s => s.Color == stickmen[0].Color))
        {
            currentIsland.IsCompleted = true;
            AddCompletedIslandCount();
        }
    }

    public void IsLevelCompleted()
    {
        if (CompletedIslandCount == stickmanSetNumbers) gameManager.LevelCompleted();
    }

    public void AddCompletedIslandCount()
    {
        CompletedIslandCount++;
        IsLevelCompleted();
    }
}

[Serializable]
public class IslandData
{
    public List<int> islandPattern;
}

[Serializable]
public class LevelData
{
    public List<IslandData> islands;
    public int ad;
    public int islandNumbers;
    public int stickmanSetNumbers;
}

[Serializable]
public class LevelDataList
{
    public List<LevelData> levels;
}