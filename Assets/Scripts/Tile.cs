using UnityEngine;
public class Tile : MonoBehaviour
{
    public Vector3 position; // Position in world space
    public Stickman currentStickman;

    public void SetStickman(Stickman stickman, Island island)
    {
        // If there's already a stickman here, destroy it
        if (currentStickman != null)
        {
            PoolManager.Instance.ReturnStickmanToPool(currentStickman);
        }
        
        // Set the new stickman and parent it to this tile
        currentStickman = stickman;
        stickman.transform.parent = transform;

        // Position the stickman on this tile
        stickman.transform.localPosition = new Vector3(0, 1.2f, 0);

        // Rotate the stickman towards the island's forward direction
        Vector3 targetForward = island.transform.forward;
        stickman.transform.rotation = Quaternion.LookRotation(targetForward);
    }
    
    public Stickman GetStickman()
    {
        return currentStickman;
    }
    
    public bool HasStickman()
    {
        return currentStickman != null;
    }
}
