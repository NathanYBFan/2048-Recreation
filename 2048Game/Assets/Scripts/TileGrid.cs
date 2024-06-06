using System.Collections.Generic;
using UnityEngine;

public sealed class TileGrid : MonoBehaviour
{
    [SerializeField]
    private TileRow[] rows;

    public TileRow[] Rows => rows;

    public int size => rows.Length * rows[0].Cells.Length;
    public int height => rows.Length;
    public int width => size / height;

    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[0].Cells.Length; x++)
            {
                rows[y].Cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }


    public TileCell GetRandomEmptyCell()
    {
        List<TileCell> availableTiles = new List<TileCell>();

        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[0].Cells.Length; x++)
            {
                if (rows[y].Cells[x].occupied) continue;
                availableTiles.Add(rows[y].Cells[x]);
            }
        }

        if (availableTiles.Count <= 0) return null;

        int index = Random.Range(0, availableTiles.Count);

        return availableTiles[index];
    }

    public TileCell GetCell(int x, int y) { return x >= 0 && x < width && y >= 0 && y < height ? rows[y].Cells[x] : null; }
    public TileCell GetCell(Vector2Int coordinate) { return coordinate.x >= 0 && coordinate.x < width && coordinate.y >= 0 && coordinate.y < height ? rows[coordinate.y].Cells[coordinate.x] : null; }
    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }
}
