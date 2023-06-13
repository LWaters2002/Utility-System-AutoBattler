using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DealableDamage_Consideration", menuName = "ScriptObjects/Considerations/DealDamage", order = 0)]
public class DealableDamageConsideration : Consideration
{
    public override float CalculateScore(GridEntity entity, ATask task)
    {
        ActionTask actionTask = (ActionTask)task;

        if (!actionTask) return 0f;
        EntityAttack attack = (EntityAttack)actionTask._action;
        if (!attack) return 0f;

        float damagePercent = attack.GetPotentialDamage(entity.ContainingTile) / entity.info.baseDamage;

        damagePercent = Mathf.Clamp01(damagePercent);

        if (oneMinus) damagePercent = 1 - damagePercent;
        return damagePercent;
    }

}
