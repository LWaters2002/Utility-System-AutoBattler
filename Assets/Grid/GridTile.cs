using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridTile : MonoBehaviour, IClickable
{
    public TextMeshPro text;
    public LayerMask obstacleMask;
    public Grid _grid { get; private set; }
    private Vector2Int _gridPosition;
    public Vector2Int GridPosition { get { return _gridPosition; } }

    public TileType tileType;

    [Header("References")]
    public GameObject heatmap;
    public Renderer heatmapRenderer;
    public Renderer tileRenderer;

    public GridEntity TileEntity;

    [SerializeField]
    private GridTile[] _neighbours;

    public GridTile[] Neighbours { get { return _neighbours; } }

    private Material _material;
    private Material _heatmapMaterial;

    public Color[] typeColours;

    public void Init(Grid grid, Vector2Int position, GridTile[] tiles)
    {
        TileEntity = null;

        _grid = grid;
        _gridPosition = position;

        text.text = "";
        _neighbours = tiles;

        _material = tileRenderer.material;
        _heatmapMaterial = heatmapRenderer.material;

        bool blocked = Physics.CheckBox(transform.position, transform.localScale * 5f, Quaternion.identity, obstacleMask);

        if (blocked)
        {
            SetTileType(TileType.unwalkable);
        }
    }

    public void SetEntity(GridEntity _entity)
    {
        TileEntity = _entity;
    }

    private void SetTileType(TileType newTileType)
    {
        if (tileType == newTileType) return;

        tileType = newTileType;

        switch (tileType)
        {
            case TileType.unwalkable:
                break;
            case TileType.ground:
                break;
            case TileType.aerial:
                break;
        }

        SetColourToType();
    }

    public void SetColourToType()
    {
        int colourIndex = (int)tileType;
        SetColour(typeColours[colourIndex]);
    }

    public void EnableHeatmap(bool isEnabled)
    {
        heatmap.SetActive(isEnabled);
    }

    public void SetHeatmapColor(Color colour)
    {
        if (!_heatmapMaterial) return;
        _heatmapMaterial.color = colour;
    }

    #region clickable
    public bool LeftClick()
    {
        if (tileType == TileType.unwalkable) return false;

        return true;
    }

    public bool RightClick()
    {
        if (tileType == TileType.unwalkable) return false;

        return true;
    }

    public void SetColour(Color colour)
    {
        if (!_material) return;
        _material.SetColor("_Colour", colour);
    }

    #endregion
}

public static class TileUtil
{
    public static void PaintTiles(this List<GridTile> tiles, Color color)
    {
        foreach (GridTile tile in tiles)
        {
            tile.SetColour(color);
        }
    }

    public static void ShowHeatmap(this List<HeatmapTile> tiles, int type)
    {
        if (tiles.Count == 0) return;

        tiles[0].tile._grid.ClearHeatmap();

        foreach (HeatmapTile heatTile in tiles)
        {
            GridTile tile = heatTile.tile;

            float r, g, b;

            r = heatTile.offence;
            g = heatTile.defence;
            b = heatTile.safety;

            Color color = new Color(r, g, b);

            switch (type)
            {
                case 1: // Offence
                    color = new Color(r, 0, 0);
                    break;
                case 2: // Defence
                    color = new Color(0, g, 0);
                    break;
                case 3: // Safety
                    color = new Color(0, 0, b);
                    break;
            }

            color.a = .65f;

            tile.EnableHeatmap(true);
            tile.SetHeatmapColor(color);
        }
    }

}

public enum TileType
{
    unwalkable,
    ground,
    aerial,
    water
}
