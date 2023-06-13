using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consideration : ScriptableObject
{
    public bool oneMinus;
    public abstract float CalculateScore(GridEntity entity, ATask task);
}


