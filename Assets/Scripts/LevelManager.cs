using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager
{
    private readonly int islandNumbers;
    private readonly int stickmanSetNumbers;

    public LevelManager(int islandNumbers, int stickmanSetNumbers)
    {
        this.islandNumbers = islandNumbers;
        this.stickmanSetNumbers = stickmanSetNumbers;
    }

    public void GenerateLevel(GameManager gameManager, int level)
    {
        // Create islands
        gameManager.SpawnIslands(islandNumbers);

        // Set the stickman colors for this level
        var stickmanColors = ColorManager.Instance.GetStickmanColors(stickmanSetNumbers);

        // Shuffle the islands randomly
        var shuffledIslands = gameManager.GetShuffledIslands();

        // Determine the number of stickman colors used in this level
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
}