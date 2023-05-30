using System.Collections.Generic;
using UnityEngine;

public class BridgeManager
{
    private List<Bridge> bridges = new List<Bridge>();

    public void DrawBridgeBetweenIslands(IslandPair islandPair)
    {
        Bridge bridge = new Bridge(islandPair);
        bridge.DrawBridge();
        bridges.Add(bridge);
    }
    
    public List<Vector3> GetBridgePath(IslandPair islandPair)
    {
        foreach (Bridge bridge in bridges)
        {
            if (bridge != null && bridge.IslandPair == islandPair)
            {
                return bridge.BridgePath;
            }
        }
        return null;
    }
    
    public void RemoveBridge(IslandPair islandPair)
    {
        foreach (Bridge bridge in bridges)
        {
            if (bridge.IslandPair == islandPair)
            {
                bridges.Remove(bridge);
                bridge.DestroyBridge();
                islandPair.FirstIsland.LowerIsland();
                islandPair.SecondIsland.LowerIsland();
                
                return;
            }
        }
    }

    public void ClearBridges()
    {
        foreach (Bridge bridge in bridges)
        {
            bridge.DestroyBridge();
        }
        bridges.Clear();
    }
}