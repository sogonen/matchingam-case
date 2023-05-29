using System.Collections.Generic;
using UnityEngine;

public class BridgeManager
{
    private List<Bridge> bridges = new List<Bridge>();

    public void DrawBridgeBetweenIslands(Island island1, Island island2)
    {
        Bridge bridge = new Bridge(island1, island2);
        bridge.DrawBridge();
        bridges.Add(bridge);
    }
    
    public List<Vector3> GetBridgePath(Island island1, Island island2)
    {
        foreach (Bridge bridge in bridges)
        {
            if (bridge.Island1 == island1 && bridge.Island2 == island2 || bridge.Island1 == island2 && bridge.Island2 == island1)
            {
                return bridge.BridgePath;
            }
        }
        return null;
    }
    
    public void RemoveBridge(Island island1, Island island2)
    {
        foreach (Bridge bridge in bridges)
        {
            if (bridge.Island1 == island1 && bridge.Island2 == island2 || bridge.Island1 == island2 && bridge.Island2 == island1)
            {
                bridge.DestroyBridge();
                bridges.Remove(bridge);
                island1.LowerIsland();
                island2.LowerIsland();
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