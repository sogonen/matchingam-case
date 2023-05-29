using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IslandManager
{
    private List<Island> activeIslands = new List<Island>();
    private Island firstSelectedIsland;
    private BridgeManager bridgeManager;

    public IslandManager(BridgeManager bridgeManager)
    {
        this.bridgeManager = bridgeManager;
    }

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
                    bridgeManager.DrawBridgeBetweenIslands(firstSelectedIsland, secondSelectedIsland);
                    MoveStickmansToNextIsland(firstSelectedIsland, secondSelectedIsland);
                }

                // Deselect both islands
                //LowerIsland(firstSelectedIsland);
                //LowerIsland(secondSelectedIsland);
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

    private void MoveStickmansToNextIsland(Island firstIsland, Island secondIsland)
    {
        // Get the highest indexed column in the first island
        int highestColumnIndex = firstIsland.GetHighestIndexedColumn();
        int lowestColumnIndex = secondIsland.GetLowestIndexedColumn();

        // Get the stickmans from the highest column in the first island
        List<Stickman> stickmansToMove = firstIsland.GetStickmansInColumn(highestColumnIndex);

        for(int i = 0; i < stickmansToMove.Count; i++)
        {
            Stickman stickman = stickmansToMove[i];
            Tile startTile = firstIsland.tiles[highestColumnIndex, i];
            Tile endTile = secondIsland.tiles[lowestColumnIndex, i];
            
            // Calculate the path for the stickman to walk on the bridge
            List<Vector3> path = new List<Vector3>();
            path.Add(startTile.transform.position);
            path.AddRange(bridgeManager.GetBridgePath(firstIsland, secondIsland));
            path.Add(endTile.transform.position);

            // Start the stickman walking along the path
            stickman.StartWalking(path, bridgeManager,firstIsland, secondIsland, lowestColumnIndex , i);

            // Remove the stickman from the first island
            firstIsland.RemoveStickman(stickman, startTile);
            endTile.SetOccupied(true);
        }
    }

    private void RaiseIsland(Island island)
    {
        island.RaiseIsland();
    }

    private void LowerIsland(Island island)
    {
        island.LowerIsland();
    }
}
