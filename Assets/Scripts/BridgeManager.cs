using System.Collections.Generic;
using UnityEngine;

public class BridgeManager
{
    private List<Bridge> bridges = new List<Bridge>();

    public void DrawBridgeBetweenIslands(Island island1, Island island2)
    {
        Vector3 bridgePosition = (island1.transform.position + island2.transform.position) / 2f;

        // Calculate the scale and rotation of the bridge
        float bridgeLength = Vector3.Distance(island1.transform.position, island2.transform.position);
        Quaternion bridgeRotation = Quaternion.LookRotation(island2.transform.position - island1.transform.position);
        
        // Instantiate a bridge prefab at the calculated position and rotation
        //Bridge bridge = GameObject.Instantiate(BridgePrefab, bridgePosition, bridgeRotation);
        
        // Scale the bridge to match its length
        //Vector3 bridgeScale = bridge.transform.localScale;
        //bridgeScale.z = bridgeLength;
        //bridge.transform.localScale = bridgeScale;

        //bridges.Add(bridge);
    }

    public void MoveStickmansToNextIsland(Island clickedIsland, Island currentIsland)
    {
        if (clickedIsland == currentIsland || !currentIsland.HasStickmansInHighestColumn())
            return;

        int columnIndex = currentIsland.GetHighestIndexedColumn();
        List<Stickman> stickmansToMove = currentIsland.GetStickmansInColumn(columnIndex);

        if (stickmansToMove.Count == 0)
            return;

        int targetColumn = clickedIsland.GetFirstAvailableColumn();
        int targetRow = clickedIsland.GetFirstAvailableRow();

        foreach (Stickman stickman in stickmansToMove)
        {
            clickedIsland.AddStickmanToTile(stickman, targetColumn, targetRow);
            currentIsland.RemoveStickman(stickman);
            targetRow++;
        }

        DrawBridgeBetweenIslands(clickedIsland, currentIsland);
    }
}