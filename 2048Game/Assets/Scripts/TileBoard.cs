using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TileBoard : MonoBehaviour
{
    [SerializeField]
    private TileGrid grid;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private List<Tile> tiles = new List<Tile>(16);

    [SerializeField]
    private TileState[] tileStates;

    [SerializeField]
    private Tile tilePrefab;

    private bool waiting;

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(grid.GetRandomEmptyCell());

        tiles.Add(tile);
    }

    public void ResetBoard()
    {
        for (int y = 0; y < grid.height; y++)
            for (int x = 0; x < grid.width; x++)
                grid.Rows[y].Cells[x].tile = null;

        for (int i = 0; i < tiles.Count; i++)
            Destroy(tiles[i].gameObject);

        tiles.Clear();
    }

    private void Update()
    {
        if (waiting) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            MoveTiles(Vector2Int.up, 0, 1, 1, 1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveTiles(Vector2Int.left, 1, 1, 0, 1);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
    }

    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;
        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.empty) continue;
                changed |= MoveTile(cell.tile, direction);
            }
        }

        if (changed)
        {
            AudioManager._Instance.PlayAudio(2, 0.3f);
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.Cell, direction);
        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    AudioManager._Instance.PlayAudio(3, 0.5f);
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveToCell(newCell);
            return true;
        }
        return false;
    }

    private bool CanMerge(Tile A, Tile B)
    {
        return A.Number == B.Number && !B.Locked;
    }

    private void Merge(Tile A, Tile B)
    {
        tiles.Remove(A);
        A.Merge(B.Cell);

        int index = Mathf.Clamp(IndexOfTileState(B.TileState) + 1, 0, tileStates.Length - 1);
        int number = B.Number * 2;

        B.SetState(tileStates[index], number);

        gameManager.IncreaseScore(number);
    }

    private int IndexOfTileState(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
            if (state == tileStates[i]) return i;

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        if (tiles.Count != grid.size)
            CreateTile();

        foreach(var tile in tiles)
            tile.Locked = false;

        if (CheckForGameOver()) gameManager.GameOver();
    }

    private bool CheckForGameOver()
    {
        if (tiles.Count != grid.size) return false;

        foreach (var tile in tiles)
        {
            TileCell[] adjacentBoxes =
            {
                grid.GetAdjacentCell(tile.Cell, Vector2Int.up),
                grid.GetAdjacentCell(tile.Cell, Vector2Int.down),
                grid.GetAdjacentCell(tile.Cell, Vector2Int.left),
                grid.GetAdjacentCell(tile.Cell, Vector2Int.right)
            };

            foreach(TileCell cell in adjacentBoxes)
                if (cell != null && CanMerge(tile, cell.tile)) return false;
        }

        return true;
    }
}
