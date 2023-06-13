using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    public bool GenerateGrid = false;

    [field: SerializeField]
    public Vector2Int Size { get; private set; }

    [field: SerializeField]
    public float TileSize { get; private set; }

    [Space]
    public GridTile tilePrefab;

    [SerializeField]
    private GridTile[] _tiles;
    public GridTile[] Tiles { get { return _tiles; } }

    public Pathfinding_AStar AStar { get; private set; }

    public void Init()
    {
        AStar = new Pathfinding_AStar(true, false);

        SetTileNeighbours();
    }

    private void Update()
    {
        if (GenerateGrid)
        {
            GenerateGrid = false;
            RegenerateTiles();
        }
    }

    private void UpdateGrid()
    {
        //Allocates space for new tiles.
        _tiles = new GridTile[Size.x * Size.y];

        Vector3 offset = -(transform.right * TileSize * Size.x) / 2 + -(transform.forward * TileSize * Size.y) / 2;

        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                Vector3 horizontalOffset = transform.right * TileSize * i;
                Vector3 verticalOffset = transform.forward * TileSize * j;

                int index = i + j * Size.y;

                _tiles[index] = Instantiate(tilePrefab, transform);
                _tiles[index].transform.position = transform.position + offset + verticalOffset + horizontalOffset;
                _tiles[index].transform.localScale = TileSize * .1f * Vector3.one;
                _tiles[index].transform.SetParent(this.transform);

            }
        }
    }

    private void SetTileNeighbours()
    {
        for (int i = 0; i < Size.x; i++)
        {
            for (int j = 0; j < Size.y; j++)
            {
                int index = i + j * Size.y;

                Vector2Int[] nOffsets = { Vector2Int.up, Vector2Int.up + Vector2Int.right, Vector2Int.right,
                    Vector2Int.right + Vector2Int.down, Vector2Int.down,Vector2Int.left + Vector2Int.down, Vector2Int.left, Vector2Int.up + Vector2Int.left };

                GridTile[] neighourTiles = new GridTile[8];

                for (int k = 0; k < nOffsets.Length; k++)
                {
                    Vector2Int nOffset = nOffsets[k];
                    int nIndex = i + nOffset.x + ((j + nOffset.y) * Size.y);

                    if (nOffset.x + i < 0 || nOffset.x + i >= Size.x) { neighourTiles[k] = null; continue; }
                    if (nOffset.y + j < 0 || nOffset.y + j >= Size.y) { neighourTiles[k] = null; continue; }


                    neighourTiles[k] = _tiles[nIndex];
                }

                _tiles[index].Init(this, new Vector2Int(i, j), neighourTiles);
            }
        }
    }

    public GridTile GetNearestTile(Vector3 position)
    {
        GridTile closestTile = _tiles[0];

        foreach (GridTile tile in _tiles)
        {
            if (Vector3.Distance(closestTile.transform.position, position) < Vector3.Distance(tile.transform.position, position)) continue;

            closestTile = tile;
        }

        return closestTile;
    }

    public GridTile GetTile(int x, int y)
    {
        return _tiles[x + y * Size.y];
    }

    private void RegenerateTiles()
    {
        for (int i = transform.childCount; i > 0; --i)
            DestroyImmediate(transform.GetChild(0).gameObject);

        UpdateGrid();
    }

    public void ClearTileColours()
    {
        foreach (GridTile tile in _tiles)
        {
            tile.SetColour(Color.white);
        }
    }

    public void PaintTileTypes()
    {
        foreach (GridTile tile in _tiles)
        {
            tile.SetColourToType();
        }
    }

    public void ClearHeatmap()
    {
        foreach (GridTile tile in _tiles)
        {
            tile.SetHeatmapColor(Color.white);
            tile.EnableHeatmap(false);
        }
    }

    public List<GridTile> TilesInRange(GridTile inTile, int range)
    {
        List<GridTile> tiles = new List<GridTile>();

        Vector2Int pos = inTile.GridPosition;

        foreach (GridTile t in Tiles)
        {
            Vector2Int otherPos = t.GridPosition;

            int xDif = otherPos.x - pos.x;
            int yDif = otherPos.y - pos.y;

            int difference = Mathf.Abs(xDif) + Mathf.Abs(yDif);

            if (difference <= range) tiles.Add(t);
        }

        return tiles;
    }

    //Uses Bresenham's line Algorithm
    public List<GridTile> GetTilesBetween(GridTile startTile, GridTile endTile)
    {
        List<GridTile> tiles = new List<GridTile>();

        int sX = startTile.GridPosition.x;
        int sY = startTile.GridPosition.y;

        int eX = endTile.GridPosition.x;
        int eY = endTile.GridPosition.y;

        int dX = Mathf.Abs(sX - eX);
        int dY = Mathf.Abs(sX - eX);

        int xDirection = (sX < eX) ? 1 : -1;
        int yDirection = (sY < eY) ? 1 : -1;

        int index = sY * Size.y + sX;

        if (dX > dY)
        {
            int error = dX / 2;

            for (int i = 0; i < dX; i++)
            {
                index += xDirection;
                if (index >= _tiles.Length || index < 0) continue;
                tiles.Add(_tiles[index]);

                error -= dY;

                if (error < 0)
                {
                    index += yDirection * Size.y;
                    if (index >= _tiles.Length || index < 0) continue;

                    tiles.Add(_tiles[index]);
                    error += dX;
                }
            }
        }
        else
        {
            int error = dY / 2;

            for (int i = 0; i < dY; i++)
            {
                index += yDirection * Size.y;
                if (index >= _tiles.Length || index < 0) continue;

                tiles.Add(_tiles[index]);
                error -= dX;

                if (error < 0)
                {
                    index += xDirection;
                    if (index >= _tiles.Length || index < 0) continue;
                    tiles.Add(_tiles[index]);
                    error += dY;
                }
            }
        }
        return tiles;
    }

}

