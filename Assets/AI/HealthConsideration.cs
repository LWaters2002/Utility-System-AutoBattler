using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthConsideration", menuName = "ScriptObjects/Considerations/Health", order = 0)]
public class HealthConsideration : Consideration
{
    public override float CalculateScore(GridEntity entity, ATask task)
    {
        float percent = entity.Health / entity.info.stats.health;

        if (oneMinus) percent = 1 - percent;
        return percent;
    }
}

