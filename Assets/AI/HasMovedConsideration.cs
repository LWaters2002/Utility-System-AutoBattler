using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HasMoved_Consideration", menuName = "ScriptObjects/Considerations/HasMoved", order = 0)]
public class HasMovedConsideration : Consideration
{
    public override float CalculateScore(GridEntity entity, ATask task)
    {
        float score = entity.HasMoved ? 1f : 0f;

        if (oneMinus) score = 1 - score;
        return score;
    }

}