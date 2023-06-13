using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HasAction_Consideration", menuName = "ScriptObjects/Considerations/HasAction", order = 0)]
public class HasActionConsideration : Consideration
{
    public override float CalculateScore(GridEntity entity, ATask task)
    {
        float score = entity.UsedAction ? 0f : 1f;

        if (oneMinus) score = 1 - score;
        return score;
    }

}