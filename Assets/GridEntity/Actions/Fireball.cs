using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball_Action", menuName = "ScriptObjects/Actions/Fireball", order = 0)]
public class Fireball : EntityAttack
{
    public Projectile fireballPrefab;

    public override void Use(Action callback)
    {
        List<GridEntity> entities = GetTargets();
        _entity.StartCoroutine(LaunchFireball(entities[0], callback));
    }

    private IEnumerator LaunchFireball(GridEntity entity, Action callback)
    {
        Projectile projectile = Instantiate(fireballPrefab, _entity.transform.position + Vector3.up * 1.5f, Quaternion.identity);

        Vector3 startLocation = projectile.transform.position;
        Vector3 endLocation = entity.transform.position;

        float t = 0;
        float length = 1;

        while (t < length)
        {
            projectile.transform.position = Vector3.Lerp(startLocation, endLocation, t / length);
            projectile.transform.position += Mathf.Sin((t / length) * Mathf.PI) * Vector3.up;

            t += Time.deltaTime;

            yield return null;
        }

        entity.TakeDamage(_entity, damageMultiplier * _entity.info.baseDamage);

        projectile.Hit();
        Destroy(projectile, 1f);

        callback?.Invoke();
    }
}
