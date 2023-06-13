using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Position_Consideration", menuName = "ScriptObjects/Considerations/Position", order = 0)]
public class PositionConsideration : Consideration
{
    public bool considerOffence;
    public bool considerDefence;
    public bool considerSafety;

    public override float CalculateScore(GridEntity entity, ATask task)
    {
        List<HeatmapTile> heatmap = entity.MovementHeatmap;
        if (heatmap.Count == 0) return 0.5f;

        HeatmapTile entityTile = heatmap[0];

        entityTile = heatmap.Find(x => x.tile == entity.ContainingTile);

        float score = 1.0f;

        if (considerOffence)
            score *= entityTile.offence;

        if (considerDefence)
            score *= entityTile.defence;

        if (considerSafety)
            score *= entityTile.safety;

        if (oneMinus) score = 1 - score;
        return score;
    }
}
