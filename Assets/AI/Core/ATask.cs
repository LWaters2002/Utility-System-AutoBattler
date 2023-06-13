using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ATask : ScriptableObject
{

    public WeightNames category;

    #region Variables
    public string label;

    public AnimationCurve sampleCurve;
    public TaskCalculationMethod calculationMethod;

    public Consideration[] considerations;

    public System.Action OnEnd;

    public float Score { get; private set; }

    protected GridEntity _entity;
    #endregion

    public virtual void Init(GridEntity entity)
    {
        _entity = entity;
    }

    public abstract void Entry();
    public virtual void End() => OnEnd?.Invoke();

    public virtual void Tick(float deltaTime) { }

    public virtual float GetScore()
    {
        float score = 0.0f;

        switch (calculationMethod)
        {
            case TaskCalculationMethod.average:
                score = AverageConsiderations();
                break;
            case TaskCalculationMethod.multiply:
                score = MultiplyConsiderations();
                break;
        }

        Score = score;

        return score;
    }

    private float AverageConsiderations()
    {
        float score = 0.0f;

        foreach (Consideration consideration in considerations)
        {
            score += consideration.CalculateScore(_entity, this);
        }

        return score / considerations.Length;

    }

    private float MultiplyConsiderations()
    {
        float score = considerations[0].CalculateScore(_entity, this);

        for (int i = 1; i < considerations.Length; i++)
        {
            score *= considerations[i].CalculateScore(_entity, this);
        }

        return score;
    }
}


public enum TaskCalculationMethod
{
    average,
    multiply
}