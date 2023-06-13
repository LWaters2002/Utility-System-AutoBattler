using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EvasiveMove_Task", menuName = "ScriptObjects/Task/EvasiveMove", order = 0)]
public class EvasiveMove_Task : ATask
{

    public override void Entry()
    {
        List<HeatmapTile> heatTiles = _entity.MovementHeatmap;
        _entity.HasMoved = true;

        //Gets best tile, if it's greater than 0 then move to it
        if (heatTiles.Count != 0)
        {
            HeatmapTile bestTile = heatTiles[0];

            foreach (HeatmapTile tile in heatTiles)
            {
                if (tile.safety > bestTile.safety) bestTile = tile;
                if (tile.safety == bestTile.safety && Random.Range(0, 2) == 1) bestTile = tile;
            }

            if (bestTile.safety != 0)
            {
                _entity.MoveToTile(bestTile.tile, End);
                return;
            }
        }

        // Otherwise move towards the nearest enemy.

        List<GridEntity> entities = GameManager.Get().turnManager.GridEntities;

        GridEntity nearestEntity = entities[0];
        int shortestDistance = 10000;

        foreach (GridEntity entity in entities)
        {
            if (entity.team != _entity.team) continue;
            if (entity == _entity) continue;

            int dist = EntityHelper.DistanceBetweenEntities(_entity, entity);

            if (dist >= shortestDistance) continue;
            shortestDistance = dist;
            nearestEntity = entity;
        }

        _entity.MoveToEntity(nearestEntity, End);
        return;
    }
}
