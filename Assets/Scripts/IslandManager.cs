using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager
{
    private readonly Queue<IslandPair> activeIslandPairs = new();
    private readonly BridgeManager bridgeManager;
    private readonly LevelManager levelManager;
    private readonly Dictionary<IslandPair, int> movedStickManCount = new();

    public IslandManager(BridgeManager bridgeManager, LevelManager levelManager)
    {
        this.bridgeManager = bridgeManager;
        this.levelManager = levelManager;
    }

    public void HandleIslandClick(Island clickedIsland)
    {
        if (clickedIsland.IsAdIsland)
        {
            GameManager.Instance.ShowAdScreen();
            return;
        }

        if (activeIslandPairs.Count == 0)
        {
            // No active island pairs yet, create a new pair with the clicked island as the first island
            if (!clickedIsland.IsCompleted)
            {
                activeIslandPairs.Enqueue(new IslandPair(clickedIsland, null));
                RaiseIsland(clickedIsland);
            }
        }
        else
        {
            var firstPair = activeIslandPairs.Peek();
            if (firstPair.SecondIsland == null && firstPair.FirstIsland != clickedIsland &&
                IsValidBridgeConnection(firstPair.FirstIsland, clickedIsland))
            {
                // Valid bridge connection, create a new pair with the first selected island and the clicked island
                var bridgedPair = new IslandPair(firstPair.FirstIsland, clickedIsland);
                activeIslandPairs.Dequeue();
                RaiseIsland(clickedIsland);
                bridgeManager.DrawBridgeBetweenIslands(bridgedPair);
                MoveStickmansToNextIsland(bridgedPair);
            }
            else
            {
                // Invalid bridge connection
                activeIslandPairs.Dequeue();
                LowerIsland(firstPair.FirstIsland);
            }
        }
    }

    private bool IsValidBridgeConnection(Island firstIsland, Island secondIsland)
    {
        if (firstIsland.GetLowestIndexedColumn() == 0 || secondIsland.GetHighestIndexedColumn() == secondIsland.size - 1
                                                      || bridgeManager.CheckReverseBridge(firstIsland, secondIsland)
                                                      || !firstIsland.isReadyToSelect
                                                      || secondIsland.IsAdIsland)
            return false;

        var highestColumnIndex = firstIsland.GetHighestIndexedColumn();
        var lowestColumnIndex = secondIsland.GetLowestIndexedColumn();

        if (highestColumnIndex >= 0 && lowestColumnIndex > 0)
        {
            var highestColumnColor = firstIsland.GetStickmanAtTile(highestColumnIndex, 0).Color;
            var lowestColumnColor = secondIsland.GetStickmanAtTile(lowestColumnIndex - 1, 0).Color;
            if (highestColumnColor != lowestColumnColor)
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
        var bridgePath = bridgeManager.GetBridgePath(islandPair);
        // Get the highest indexed column in the first island
        var highestColumnIndex = islandPair.FirstIsland.GetHighestIndexedColumn();
        // Get the lowest indexed column in the second island
        var lowestColumnIndex = islandPair.SecondIsland.GetLowestIndexedColumn();
        // Calculate how many columns we need to move for the same colored stickmans to be on the same column
        var columnsToMove = CheckColumnsToMove(islandPair, highestColumnIndex, lowestColumnIndex);

        movedStickManCount.Add(islandPair, 0);

        float moveStartTime = 0;

        for (var i = 0; i < columnsToMove; i++)
        {
            // Move the stickmans in the column
            islandPair.FirstIsland.StartCoroutine(MoveColumn(islandPair, bridgePath, highestColumnIndex,
                lowestColumnIndex, moveStartTime));
            moveStartTime = i * 0.1f;
            highestColumnIndex--;
            lowestColumnIndex++;
        }
    }

    public IEnumerator MoveColumn(IslandPair islandPair, List<Vector3> bridgePath, int from, int to,
        float moveStartTime)
    {
        var stickmansToMove = islandPair.FirstIsland.GetStickmansInColumn(from);
        for (var i = 0; i < stickmansToMove.Count; i++)
        {
            var moveTime = moveStartTime + i * 0.05f;
            yield return new WaitForSeconds(moveTime);
            var stickman = stickmansToMove[i];
            var startTile = islandPair.FirstIsland.tiles[from, i];
            var endTile = islandPair.SecondIsland.tiles[to, i];

            // Calculate the path for the stickman to walk on the bridge
            var path = new List<Vector3>();
            path.Add(startTile.transform.position);
            path.AddRange(bridgePath);
            path.Add(endTile.transform.position);
            stickman.Run(path, this, levelManager, islandPair, to, i);
            islandPair.FirstIsland.RemoveStickman(stickman, startTile);
            endTile.SetStickMan(stickman);
            movedStickManCount[islandPair]++;
            islandPair.SecondIsland.isReadyToSelect = false;
        }
    }

    private int CheckColumnsToMove(IslandPair islandPair, int highestColumnIndex, int lowestColumnIndex)
    {
        var columnCount = 0;
        var colorToMove = islandPair.FirstIsland.GetStickmanAtTile(highestColumnIndex, 0).Color;

        var j = lowestColumnIndex;
        for (var i = highestColumnIndex; i >= 0; i--)
        {
            if (j == islandPair.SecondIsland.size) break;

            if (islandPair.FirstIsland.GetStickmanAtTile(i, 0).Color == colorToMove &&
                !islandPair.SecondIsland.tiles[j, 0].HasStickman)
            {
                columnCount++;
                j++;
            }
            else
            {
                break;
            }
        }

        return columnCount;
    }

    public void checkStickmanCount(IslandPair islandPair)
    {
        movedStickManCount[islandPair]--;
        if (movedStickManCount[islandPair] == 0)
        {
            bridgeManager.RemoveBridge(islandPair);
            movedStickManCount.Remove(islandPair);
            islandPair.SecondIsland.isReadyToSelect = true;
        }
    }

    public bool NoMovementInProgress()
    {
        return movedStickManCount.Count == 0;
    }

    public void RaiseIsland(Island island)
    {
        island.RaiseIsland(0.2f);
    }

    public void LowerIsland(Island island)
    {
        island.LowerIsland(0.2f);
    }

    public void Clear()
    {
        movedStickManCount.Clear();
        activeIslandPairs.Clear();
    }
}