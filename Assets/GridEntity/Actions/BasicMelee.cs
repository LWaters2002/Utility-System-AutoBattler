using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicMelee_Action", menuName = "ScriptObjects/Actions/BasicMelee", order = 0)]
public class BasicMelee : EntityAttack
{
    public Sword swordPrefab;
    System.Action callbackCache;

    public override void Use(System.Action callback)
    {
        callbackCache = callback;

        Quaternion rotation = Quaternion.LookRotation(GetTargets()[0].transform.position - _entity.transform.position);
        _entity.RotateToDirection(rotation, .3f, Next);
    }

    private void Next()
    {
        Sword sword = Instantiate(swordPrefab, _entity.visual.transform);
        sword.PlayAnimation("SwordSlash", NextNext);
    }

    private void NextNext() // This naming here is intentional
    {
        DealDamageToTargets();
        callbackCache?.Invoke();
    }
}