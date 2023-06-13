using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveEnergy_Consideration", menuName = "ScriptObjects/Considerations/MoveEnergy", order = 0)]
public class MoveEnergy_Consideration : Consideration
{
    public override float CalculateScore(GridEntity entity, ATask task)
    {
        float score = (float)entity.MoveEnergy / entity.info.stats.moveEnergy;
        
        score = Mathf.Clamp01(score);
        if (oneMinus) score = 1-score;
        
        return score;
    }
}
