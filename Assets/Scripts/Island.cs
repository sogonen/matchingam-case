using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Island : MonoBehaviour, IPointerClickHandler
{
    public int size = 4; // The size of one side of the square island
    public float tileSize = 1f; // The size of one tile
    public Tile tilePrefab; // Assign this in the Inspector
    public int index;

    private readonly float raisedHeight = 0.5f; // The height to raise the island when activated
    private readonly List<Stickman> stickmans = new List<Stickman>(); // List of stickmans on the island
    public Tile[,] tiles; // 2D array to hold the tiles

    private IslandManager islandManager;

    public void Initialize(IslandManager manager)
    {
        islandManager = manager;
        tiles = new Tile[size, size];

        for (var x = 0; x < size; x++)
        {
            for (var z = 0; z < size; z++)
            {
                var tileLocalPosition = new Vector3(x * tileSize - 1.5f, 0, z * tileSize - 1.5f);
                var newTile = Instantiate(tilePrefab, transform);
                newTile.transform.localPosition = tileLocalPosition;
                tiles[x, z] = newTile;
                tiles[x, z].name = "Tile (" + x + ", " + z + ")";
            }
        }
    }

    public void AddStickmanToTile(Stickman stickman, int x, int z)
    {
        if (IsTileValid(x, z))
        {
            var tile = tiles[x, z];
            tile.SetStickman(stickman, this); // Pass the 'island' reference to 'SetStickman' method
            stickmans.Add(stickman);
        }
    }

    public bool HasStickmansInHighestColumn()
    {
        if (stickmans.Count > 0)
        {
            int highestColumnIndex = GetHighestIndexedColumn();
            return tiles[highestColumnIndex, 0].HasStickman();
        }

        return false;
    }
    
    public Stickman GetStickmanAtTile(int x, int z)
    {
        if (IsTileValid(x, z))
        {
            return tiles[x, z].GetStickman();
        }

        return null;
    }

    public List<Stickman> GetStickmansInColumn(int columnIndex)
    {
        List<Stickman> stickmansInColumn = new List<Stickman>();

        for (int rowIndex = 0; rowIndex < size; rowIndex++)
        {
            Stickman stickman = tiles[columnIndex, rowIndex].GetStickman();
            if (stickman != null)
            {
                stickmansInColumn.Add(stickman);
            }
        }

        return stickmansInColumn;
    }

    public void RemoveStickman(Stickman stickman)
    {
        stickmans.Remove(stickman);
    }

    public int GetHighestIndexedColumn()
    {
        for (int columnIndex = size - 1; columnIndex >= 0; columnIndex--)
        {
            for (int rowIndex = 0; rowIndex < size; rowIndex++)
            {
                if (tiles[columnIndex, rowIndex].HasStickman())
                {
                    return columnIndex;
                }
            }
        }

        return -1; // No stickmans found on the island
    }

    public int GetFirstAvailableColumn()
    {
        for (int columnIndex = 0; columnIndex < size; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < size; rowIndex++)
            {
                if (!tiles[columnIndex, rowIndex].HasStickman())
                {
                    return columnIndex;
                }
            }
        }

        return -1; // No available column found on the island
    }

    public int GetFirstAvailableRow()
    {
        for (int rowIndex = 0; rowIndex < size; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < size; columnIndex++)
            {
                if (!tiles[columnIndex, rowIndex].HasStickman())
                {
                    return rowIndex;
                }
            }
        }

        return -1; // No available row found on the island
    }

    public void RaiseIsland()
    {
        transform.position += new Vector3(0, raisedHeight, 0);
    }

    public void LowerIsland()
    {
        transform.position -= new Vector3(0, raisedHeight, 0);
    }

    private bool IsTileValid(int x, int z)
    {
        // Check if the given tile position is within the island's boundaries
        return x >= 0 && x < size && z >= 0 && z < size;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        islandManager.HandleIslandClick(this);
    }
}
