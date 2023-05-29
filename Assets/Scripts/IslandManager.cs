using System.Collections.Generic;
using UnityEngine;

public class IslandManager
{
    private List<Island> activeIslands = new List<Island>();
    private Island firstSelectedIsland;

    public void HandleIslandClick(Island clickedIsland)
    {
        if (activeIslands.Count == 0)
        {
            // First island click
            activeIslands.Add(clickedIsland);
            firstSelectedIsland = clickedIsland;
            RaiseIsland(clickedIsland);
        }
        else if (activeIslands.Count == 1)
        {
            // Second island click
            var secondSelectedIsland = clickedIsland;

            if (secondSelectedIsland == firstSelectedIsland)
            {
                // Clicked the same island again, deselect the island
                activeIslands.Remove(clickedIsland);
                LowerIsland(clickedIsland);
            }
            else
            {
                // Clicked a different island
                if (IsValidBridgeConnection(firstSelectedIsland, secondSelectedIsland))
                {
                    activeIslands.Add(secondSelectedIsland);
                    RaiseIsland(secondSelectedIsland);
                    DrawBridgeBetweenIslands(firstSelectedIsland, secondSelectedIsland);
                    MoveStickmansToNextIsland(firstSelectedIsland, secondSelectedIsland);
                }

                // Deselect both islands
                LowerIsland(firstSelectedIsland);
                LowerIsland(secondSelectedIsland);
                activeIslands.Clear();
            }
        }
    }

    public void HandleScreenClick()
    {
        // Lower all active islands when clicking anywhere else on the screen
        foreach (var island in activeIslands)
        {
            LowerIsland(island);
        }
        activeIslands.Clear();
    }

    private bool IsValidBridgeConnection(Island firstIsland, Island secondIsland)
    {
        // Implement the logic to check if the bridge connection is valid
        // You can apply the specified conditions here
        // Return true if the connection is valid, false otherwise
        // Example condition: if (firstIsland.HasStickmansInHighestColumn() && secondIsland.IsEmpty())
        return true;
    }

    private void DrawBridgeBetweenIslands(Island firstIsland, Island secondIsland)
    {
        // Implement the logic to draw a bridge between the islands
        // You can create a bridge game object or modify the island visuals to indicate the bridge
    }

    private void MoveStickmansToNextIsland(Island firstIsland, Island secondIsland)
    {
        // Implement the logic to move stickmans from the first island to the second island
        // You can iterate through the tiles of the first island and check the highest indexed column
        // Move stickmans of the same color from the highest indexed column to the second island using the bridge/pathway
    }

    private void RaiseIsland(Island island)
    {
        // Implement the logic to raise the island
        // You can modify the Island class to include a method for raising the island
        island.RaiseIsland();
    }

    private void LowerIsland(Island island)
    {
        // Implement the logic to lower the island
        // You can modify the Island class to include a method for lowering the island
        island.LowerIsland();
    }
}
