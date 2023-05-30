using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class LevelManager
{
    private List<LevelData> levels;
    private GameManager gameManager;
    private int completedIslandCount = 0;
    private int islandNumbers;
    private int stickmanSetNumbers;
    public LevelManager()
    {
        if (levels == null)
        {
            levels = LoadLevelsFromJSON();
        }
    }
    
    private List<LevelData> LoadLevelsFromJSON()
    {
        List<LevelData> levelDataList = new List<LevelData>();

        string filePath = Path.Combine(Application.streamingAssetsPath, "levels.json");

        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            LevelDataList levelDataListContainer = JsonUtility.FromJson<LevelDataList>(jsonData);

            if (levelDataListContainer != null)
            {
                levelDataList = levelDataListContainer.levels;
            }
            else
            {
                Debug.LogError("Failed to parse currentLevel data from JSON: " + filePath);
            }
        }
        else
        {
            Debug.LogError("Level data file not found: " + filePath);
        }

        return levelDataList;
    }

    public void GenerateNextLevel(GameManager gameManager,int levelIndex)
    {
        islandNumbers = levels[levelIndex].islandNumbers;
        stickmanSetNumbers = levels[levelIndex].stickmanSetNumbers;
        
        this.gameManager = gameManager;
        // Create islands
        this.gameManager.SpawnIslands(islandNumbers);

        // Set the stickman colors for this currentLevel
        var stickmanColors = ColorManager.Instance.GetStickmanColors(stickmanSetNumbers);

        // Shuffle the islands randomly
        var shuffledIslands = gameManager.GetShuffledIslands();

        // Determine the number of stickman colors used in this currentLevel
        var usedColorCount = Mathf.Min(stickmanSetNumbers, stickmanColors.Count);

        // Determine the number of columns needed based on the number of stickman colors
        var columnsNeeded = Mathf.CeilToInt(4f * usedColorCount);

        // Keep track of the stickman colors used in each column of each island
        var islandColumnColors = new Dictionary<Island, List<List<StickmanColor>>>();

        // Loop through each column index
        for (var columnIndex = 0; columnIndex < columnsNeeded; columnIndex++)
        {
            // Select a stickman color for this column
            var colorIndex = columnIndex % usedColorCount;
            var columnColor = stickmanColors[colorIndex];

            // Loop through each island
            foreach (var island in shuffledIslands)
            {
                // Determine the column index within the island
                var islandColumnIndex = columnIndex % island.size;

                // Get the stickman colors used in this column of the island
                List<StickmanColor> columnColors;
                if (islandColumnColors.ContainsKey(island))
                {
                    columnColors = islandColumnColors[island][islandColumnIndex];
                }
                else
                {
                    columnColors = new List<StickmanColor>();
                    islandColumnColors[island] = new List<List<StickmanColor>>();
                    for (var i = 0; i < island.size; i++) islandColumnColors[island].Add(new List<StickmanColor>());
                }

                // Check if the column already contains the selected color
                if (columnColors.Contains(columnColor))
                {
                    // Select a different color that hasn't been used in this column
                    var unusedColors = stickmanColors.Except(columnColors).ToList();
                    columnColor = unusedColors[Random.Range(0, unusedColors.Count)];
                }

                // Add the selected color to the column colors
                columnColors.Add(columnColor);

                // Loop through each row within the column
                for (var rowIndex = 0; rowIndex < island.size; rowIndex++)
                {
                    // Get a Stickman from the pool with the specified color
                    var stickman = PoolManager.Instance.GetStickmanFromPool(columnColor);
                    
                    // Add the Stickman to the tile in the island
                    if (stickman != null)
                        island.AddStickmanToTile(stickman, islandColumnIndex, rowIndex);
                }
            }
        }
    }
    
    public void IslandCompleted(Island currentIsland)
    {
        // Get the stickmen on the island
        List<Stickman> stickmen = currentIsland.GetAllStickmen();

        // Check if all stickmen on the island have the same color
        if (stickmen.Count == 16 && stickmen.All(s => s.Color == stickmen[0].Color))
        {
            AddCompletedIslandCount();
        }
    }

    public void IsLevelCompleted()
    {
        if(completedIslandCount == stickmanSetNumbers)
        {
            gameManager.LevelCompleted();
        }
    }
    
    public void AddCompletedIslandCount()
    {
        completedIslandCount++;
        IsLevelCompleted();
    }
    
    public int CompletedIslandCount
    {
        get { return completedIslandCount; }
    }
}

[System.Serializable]
public class LevelData
{
    public int islandNumbers;
    public int stickmanSetNumbers;
}

[System.Serializable]
public class LevelDataList
{
    public List<LevelData> levels;
}