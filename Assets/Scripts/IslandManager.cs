using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class IslandManager
{
    private Queue<IslandPair> activeIslandPairs = new Queue<IslandPair>();
    private BridgeManager bridgeManager;
    private LevelManager levelManager;
    public IslandManager(BridgeManager bridgeManager, LevelManager levelManager)
    {
        this.bridgeManager = bridgeManager;
        this.levelManager = levelManager;
    }

    public void HandleIslandClick(Island clickedIsland)
    {
        if (activeIslandPairs.Count == 0)
        {
            // No active island pairs yet, create a new pair with the clicked island as the first island
            if (!clickedIsland.IsCompleted)
            {
                activeIslandPairs.Enqueue(new IslandPair(clickedIsland, null));
            RaiseIsland(clickedIsland);
            }
        }

        else if (activeIslandPairs.Count > 0)
        {
            var firstPair = activeIslandPairs.Peek();
            if (firstPair.SecondIsland == null && IsValidBridgeConnection(firstPair.FirstIsland, clickedIsland))
            {
                // Valid bridge connection, create a new pair with the first selected island and the clicked island
                var bridgedPair = new IslandPair(firstPair.FirstIsland, clickedIsland);
                activeIslandPairs.Dequeue();
                RaiseIsland(clickedIsland);
                bridgeManager.DrawBridgeBetweenIslands(bridgedPair);
                MoveStickmansToNextIsland(bridgedPair);
            }
            else if (firstPair.FirstIsland == clickedIsland)
            {
                // Clicked the first island again, remove the pair from the queue
                activeIslandPairs.Dequeue();
                LowerIsland(clickedIsland);
            }
            else
            {
                // Invalid bridge connection, remove the first island from the queue and add the clicked island
                activeIslandPairs.Dequeue();
                LowerIsland(firstPair.FirstIsland);
            }
        }
    }

    private IslandPair GetActivePairContainingIsland(Island island)
    {
        foreach (var pair in activeIslandPairs)
        {
            if (pair.FirstIsland == island || pair.SecondIsland == island)
                return pair;
        }
        return null;
    }

    private bool IsValidBridgeConnection(Island firstIsland, Island secondIsland)
    {
        if (firstIsland.GetLowestIndexedColumn() == 0)
            return false;
        
        // Check if the highest filled column on the second island has the same color as the current column on the first island
        int highestColumnIndex = firstIsland.GetHighestIndexedColumn();
        int lowestColumnIndex = secondIsland.GetLowestIndexedColumn();

        if (highestColumnIndex >= 0 && lowestColumnIndex > 0)
        {
            StickmanColor highestColumnColor = firstIsland.GetStickmanAtTile(highestColumnIndex, 0).Color;
            StickmanColor lowestColumnColor = secondIsland.GetStickmanAtTile(lowestColumnIndex-1, 0).Color;
            if ((highestColumnColor != lowestColumnColor))
            {
                LowerIsland(firstIsland);
                LowerIsland(secondIsland);
                return false;
            }
        }
        return true;
    }

    private void MoveStickmansToNextIsland(IslandPair islandPair)
    {
        // Get the highest indexed column in the first island
        int highestColumnIndex = islandPair.FirstIsland.GetHighestIndexedColumn();
        int lowestColumnIndex = islandPair.SecondIsland.GetLowestIndexedColumn();

        // Get the stickmans from the highest column in the first island
        List<Stickman> stickmansToMove = islandPair.FirstIsland.GetStickmansInColumn(highestColumnIndex);

        List<Vector3> bridgePath = bridgeManager.GetBridgePath(islandPair);
        
        for(int i = 0; i < stickmansToMove.Count; i++)
        {
            Stickman stickman = stickmansToMove[i];
            Tile startTile = islandPair.FirstIsland.tiles[highestColumnIndex, i];
            Tile endTile = islandPair.SecondIsland.tiles[lowestColumnIndex, i];
            
            // Calculate the path for the stickman to walk on the bridge
            List<Vector3> path = new List<Vector3>();
            path.Add(startTile.transform.position);
            path.AddRange(bridgePath);
            path.Add(endTile.transform.position);
            stickman.StartWalking(path, bridgeManager, levelManager, islandPair, lowestColumnIndex , i);
            islandPair.FirstIsland.RemoveStickman(stickman, startTile);
            endTile.SetStickMan(stickman);
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
