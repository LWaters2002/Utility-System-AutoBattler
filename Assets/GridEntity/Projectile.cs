using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public UnityEvent OnHit;

    public void Hit()
    {
        OnHit?.Invoke();
    }
}
