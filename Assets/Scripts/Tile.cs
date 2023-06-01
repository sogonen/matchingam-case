using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3 position; // Position in world space
    public Stickman currentStickman;
    public bool isOccupied;

    public bool IsOccupied => isOccupied;

    public bool HasStickman => currentStickman != null;

    public void SetInitStickman(Stickman stickman, Island island)
    {
        SetStickMan(stickman);
        stickman.transform.localPosition = new Vector3(0, 1.2f, 0);
        var targetForward = island.transform.right;
        stickman.transform.forward = targetForward;
    }

    public void SetStickMan(Stickman stickman)
    {
        isOccupied = true;
        currentStickman = stickman;
        stickman.transform.parent = transform;
    }

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
        if (!occupied)
            currentStickman = null;
    }

    public Stickman GetStickman()
    {
        return currentStickman;
    }
}