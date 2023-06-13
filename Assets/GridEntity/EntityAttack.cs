using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : EntityAction
{
    public float damageMultiplier = 1;

    public float GetPotentialDamage(GridTile tile)
    {
        int count = GetTargets(tile).Count;

        return count * _entity.info.baseDamage * damageMultiplier;
    }

    protected void DealDamageToTargets()
    {
        List<GridEntity> entities = GetTargets();

        foreach (GridEntity entity in entities)
        {
            entity.TakeDamage(_entity, _entity.info.baseDamage * damageMultiplier);
        }
    }
}
