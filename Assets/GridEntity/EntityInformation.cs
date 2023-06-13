using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EntityInformation", menuName = "ScriptObjects/EntityInformation", order = 0)]
public class EntityInformation : ScriptableObject
{
    public string entityName;
    public float baseDamage;
    public EntityStat stats;

    [Header("AI")]
    public EntityWeight[] weights;
    public List<ATask> Tasks;

    public float GetWeight(WeightNames weightName)
    {
        foreach (EntityWeight w in weights)
        {
            if (w.name != weightName) continue;
            return w.weight;
        }

        return 1f;
    }
}

[System.Serializable]
public struct EntityStat
{
    // The amount of tiles that can be traversed in a turn; This determines range.
    public int moveEnergy;
    public float speed;
    public float health;
}

[System.Serializable]
public struct EntityWeight
{
    public WeightNames name;
    public float weight;
}

public enum WeightNames
{
    offensive, // Rash moves, focus on high risk and reward - prefers damage
    defensive, // Reliable and simple, focuses on survival.
    evasive, // preference to evade attacks and be out of range
    intelligence // Controls utility random threshold
}