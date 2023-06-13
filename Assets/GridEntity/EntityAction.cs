using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAction : ScriptableObject
{
    public int minRange = 0;
    public int range;

    public bool isSelf;
    public bool straightLine;
    public bool requiresLOS;
    public bool singleTarget;

    public bool isFriendly;

    protected GridEntity _entity;

    public virtual void Init(GridEntity entity)
    {
        _entity = entity;
    }

    public virtual void Use(System.Action callback)
    {
        callback?.Invoke();
    }

    public List<GridEntity> GetTargets()
    {
        if (isSelf) return new List<GridEntity> { _entity };

        List<GridTile> tilesInRange = _entity.grid.TilesInRange(_entity.ContainingTile, range);
        List<GridEntity> validEntities = new List<GridEntity>();

        foreach (GridTile gridTile in tilesInRange)
        {
            GridEntity gridEntity = gridTile.TileEntity;
            if (!gridEntity) continue;
            if (gridEntity.team == _entity.team && !isFriendly) continue;
            if (gridEntity.team != _entity.team && isFriendly) continue;
            if (requiresLOS)
                if (CheckLineOfSight(_entity.ContainingTile, gridTile)) continue;

            validEntities.Add(gridEntity);
        }

        if (validEntities.Count == 0) return null;

        if (singleTarget)
        {
            GridEntity bestTarget = GetBestTarget(validEntities);
            return new List<GridEntity> { bestTarget };
        }

        return validEntities;
    }

    public List<GridEntity> GetTargets(GridTile startTile)
    {
        if (isSelf) return new List<GridEntity> { _entity };

        List<GridTile> tilesInRange = _entity.grid.TilesInRange(startTile, range);
        List<GridEntity> validEntities = new List<GridEntity>();

        foreach (GridTile gridTile in tilesInRange)
        {
            GridEntity gridEntity = gridTile.TileEntity;
            if (!gridEntity) continue;
            if (gridEntity.team == _entity.team && !isFriendly) continue;
            if (gridEntity.team != _entity.team && isFriendly) continue;
            if (requiresLOS)
                if (CheckLineOfSight(_entity.ContainingTile, gridTile)) continue;

            validEntities.Add(gridEntity);
        }

        if (singleTarget)
        {
            GridEntity bestTarget = GetBestTarget(validEntities);
            if (bestTarget)
            {
                return new List<GridEntity> { bestTarget };
            }
        }

        return validEntities;
    }

    private bool CheckLineOfSight(GridTile startTile, GridTile endTile)
    {
        List<GridTile> tiles = _entity.grid.GetTilesBetween(startTile, endTile);

        bool lineValid = true;

        foreach (GridTile tile in tiles)
        {
            if (tile == startTile) continue;
            if (tile == endTile) continue;

            if (tile.tileType == TileType.unwalkable) return false;
            if (tile.TileEntity) return false;
        }

        return lineValid;
    }

    public virtual GridEntity GetBestTarget(List<GridEntity> entities)
    {
        if (entities.Count == 0) return null;
        return entities[0];
    }

}
