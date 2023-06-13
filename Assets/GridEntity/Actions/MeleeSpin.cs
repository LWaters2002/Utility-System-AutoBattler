using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeSpin_Action", menuName = "ScriptObjects/Actions/MeleeSpin", order = 0)]
public class MeleeSpin : EntityAttack
{
    public Sword swordPrefab;

    public override void Use(Action callback)
    {
        Sword sword = Instantiate(swordPrefab, _entity.visual.transform);
        sword.PlayAnimation("SwordSpin", callback);

        DealDamageToTargets();
    }
}
