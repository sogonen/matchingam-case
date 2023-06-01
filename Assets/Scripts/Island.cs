using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Island : MonoBehaviour, IPointerClickHandler
{
    public int size = 4; // The size of one side of the square island
    public float tileSize = 1f; // The size of one tile
    public Tile tilePrefab; // Assign this in the Inspector
    public int index;
    public bool isReadyToSelect;
    public List<Tile> tileList = new(); // List of tiles on the island
    public GameObject adIcon; // The ad icon game object

    private IslandManager islandManager;
    private bool isRaised; // Whether the island is raised or not
    private readonly float raisedHeight = 0.5f; // The height to raise the island when activated
    private readonly List<Stickman> stickmans = new(); // List of stickmans on the island
    public Tile[,] tiles; // 2D array to hold the tiles

    public bool IsAdIsland { get; set; }

    public bool IsCompleted { set; get; }

    public void OnPointerClick(PointerEventData eventData)
    {
        islandManager.HandleIslandClick(this);
    }

    public void Initialize(IslandManager manager)
    {
        islandManager = manager;
        tiles = new Tile[size, size];

        for (var x = 0; x < size; x++)
        for (var z = 0; z < size; z++)
        {
            var tileLocalPosition = new Vector3(x * tileSize - 1.5f, 0, z * tileSize - 1.5f);
            var newTile = Instantiate(tilePrefab, transform);
            newTile.transform.localPosition = tileLocalPosition;
            tiles[x, z] = newTile;
            tiles[x, z].name = "Tile (" + x + ", " + z + ")";
        }

        isReadyToSelect = true;
    }

    public void AddStickmanToTile(Stickman stickman, int x, int z)
    {
        if (IsTileValid(x, z))
        {
            var tile = tiles[x, z];
            tile.SetInitStickman(stickman, this); // Pass the 'island' reference to 'SetInitStickman' method
            stickmans.Add(stickman);
        }
    }

    public Tile GetTileAt(int x, int z)
    {
        if (IsTileValid(x, z)) return tiles[x, z];

        return null;
    }

    public List<Stickman> GetStickmansInColumn(int columnIndex)
    {
        var stickmansInColumn = new List<Stickman>();

        for (var rowIndex = 0; rowIndex < size; rowIndex++)
        {
            var stickman = tiles[columnIndex, rowIndex].GetStickman();
            if (stickman != null) stickmansInColumn.Add(stickman);
        }

        return stickmansInColumn;
    }

    public void RemoveStickman(Stickman stickman, Tile tile)
    {
        tile.SetOccupied(false);
        stickmans.Remove(stickman);
    }

    public List<Stickman> GetAllStickmen()
    {
        var stickmen = new List<Stickman>();

        foreach (var tile in tiles)
            if (tile.HasStickman)
            {
                var stickman = tile.GetStickman();
                stickmen.Add(stickman);
            }

        return stickmen;
    }

    public Stickman GetStickmanAtTile(int x, int z)
    {
        if (IsTileValid(x, z)) return tiles[x, z].GetStickman();

        return null;
    }

    public int GetHighestIndexedColumn()
    {
        for (var columnIndex = size - 1; columnIndex >= 0; columnIndex--)
        for (var rowIndex = 0; rowIndex < size; rowIndex++)
        {
            var currentTile = tiles[columnIndex, rowIndex];
            if (currentTile.IsOccupied || currentTile.HasStickman) return columnIndex;
        }

        return -1; // No stickmans found on the island
    }

    public int GetLowestIndexedColumn()
    {
        for (var columnIndex = 0; columnIndex < size; columnIndex++)
        for (var rowIndex = 0; rowIndex < size; rowIndex++)
        {
            var currentTile = tiles[columnIndex, rowIndex];
            if (!currentTile.IsOccupied && !currentTile.HasStickman) return columnIndex;
        }

        return -1;
    }

    public void RaiseIsland(float duration)
    {
        if (!isRaised) StartCoroutine(RaiseIslandCoroutine(duration));
    }

    private IEnumerator RaiseIslandCoroutine(float duration)
    {
        var startPos = transform.position;
        var targetPos = startPos + new Vector3(0, raisedHeight, 0);
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            var t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isRaised = true;
    }

    public void LowerIsland(float duration)
    {
        if (isRaised) StartCoroutine(LowerIslandCoroutine(duration));
    }

    public void Unlock()
    {
        IsAdIsland = false;
        adIcon.SetActive(false);
    }

    private IEnumerator LowerIslandCoroutine(float duration)
    {
        var startPos = transform.position;
        var targetPos = startPos - new Vector3(0, raisedHeight, 0);
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            var t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isRaised = false;
    }


    private bool IsTileValid(int x, int z)
    {
        return x >= 0 && x < size && z >= 0 && z < size;
    }
}