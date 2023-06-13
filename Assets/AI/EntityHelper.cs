using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntityHelper
{
    public static List<HeatmapTile> CalculateHeatmap(GridEntity entity)
    {
        List<GridTile> tiles = entity.grid.TilesInRange(entity.ContainingTile, entity.MoveEnergy);
        List<HeatmapTile> heatmapTiles = new List<HeatmapTile>();

        foreach (GridTile tile in tiles)
        {
            HeatmapTile hTile;
            hTile.tile = tile;
            hTile.offence = GetTileOffence(entity, tile);
            hTile.defence = GetTileDefence(entity, tile);
            hTile.safety = GetTileSafety(entity, tile);
            heatmapTiles.Add(hTile);
        }

        return heatmapTiles;
    }

    //Offense is measured by the damage that can be done
    private static float GetTileOffence(GridEntity entity, GridTile tile)
    {
        if (entity.UsedAction) return 0.0f;

        float highestDamage = 0;

        foreach (EntityAction action in entity.Actions)
        {
            EntityAttack attack = (EntityAttack)action;
            if (!attack) continue;

            float newDamage = attack.GetPotentialDamage(tile);

            if (newDamage > highestDamage) highestDamage = newDamage;
        }

        float result = Mathf.Clamp01(highestDamage / entity.info.baseDamage);
        return result;
    }

    //Defence is proximity to allies
    private static float GetTileDefence(GridEntity entity, GridTile tile)
    {
        List<GridEntity> entities = GameManager.Get().turnManager.GridEntities;

        float score = 0;

        foreach (GridEntity newEntity in entities)
        {
            if (newEntity == entity) continue;
            //Filters for friendlies
            if (newEntity.team != entity.team) continue;

            Vector2Int difference = newEntity.ContainingTile.GridPosition - tile.GridPosition;
            int x = Mathf.Abs(difference.x);
            int y = Mathf.Abs(difference.y);

            int dist = x + y;
            int allyMovement = newEntity.info.stats.moveEnergy;

            if (dist > allyMovement) continue;

            //Percent proximity to entity
            score += 1 - ((x + y) / (float)allyMovement);
        }

        score = Mathf.Clamp01(score);
        return score;
    }

    //Safety is the likelihood and vulnerability of the current position
    private static float GetTileSafety(GridEntity entity, GridTile tile)
    {
        List<GridEntity> entities = GameManager.Get().turnManager.GridEntities;

        float highestOffenceScore = 0;

        foreach (GridEntity newEntity in entities)
        {
            if (newEntity == entity) continue;

            //Filters for enemies
            if (newEntity.team == entity.team) continue;
            float newScore = GetTileOffence(newEntity, tile);

            if (newScore > highestOffenceScore) highestOffenceScore = newScore;
        }

        return 1 - highestOffenceScore;
    }

    public static int DistanceBetweenEntities(GridEntity entity0, GridEntity entity1)
    {
        Vector2Int difference = entity0.ContainingTile.GridPosition - entity1.ContainingTile.GridPosition;

        int x = Mathf.Abs(difference.x);
        int y = Mathf.Abs(difference.y);

        int dist = x + y;

        return dist;
    }
}

public struct HeatmapTile
{
    public GridTile tile;
    public float offence;
    public float defence;
    public float safety;
}
