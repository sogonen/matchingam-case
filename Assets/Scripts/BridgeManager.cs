using System.Collections.Generic;
using UnityEngine;

public class BridgeManager
{
    private readonly List<Bridge> bridges = new();

    public void DrawBridgeBetweenIslands(IslandPair islandPair)
    {
        var bridge = new Bridge(islandPair);
        bridge.DrawBridge();
        bridges.Add(bridge);
    }

    public List<Vector3> GetBridgePath(IslandPair islandPair)
    {
        foreach (var bridge in bridges)
            if (bridge != null && bridge.IslandPair == islandPair)
                return bridge.BridgePath;
        return null;
    }

    public bool CheckReverseBridge(Island firstIsland, Island secondIsland)
    {
        foreach (var bridge in bridges)
            if (bridge != null && bridge.IslandPair.FirstIsland == secondIsland &&
                bridge.IslandPair.SecondIsland == firstIsland)
                return true;
        return false;
    }

    public void RemoveBridge(IslandPair islandPair)
    {
        foreach (var bridge in bridges)
            if (bridge.IslandPair == islandPair)
            {
                bridges.Remove(bridge);
                bridge.DestroyBridge();
                GameManager.Instance.islandManager.LowerIsland(islandPair.FirstIsland);
                GameManager.Instance.islandManager.LowerIsland(islandPair.SecondIsland);
                return;
            }
    }

    public void ClearBridges()
    {
        foreach (var bridge in bridges) bridge.DestroyBridge();
        bridges.Clear();
    }
}