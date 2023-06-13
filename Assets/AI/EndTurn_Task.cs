using UnityEngine;

[CreateAssetMenu(fileName = "EndTurn_Task", menuName = "ScriptObjects/Task/EndTurn", order = 0)]
public class EndTurn_Task : ATask
{
    [Header("END TURN SCORE")]
    public float baseScore;

    public override void Entry()
    {
        _entity.Wait(1f, EndTurn);
    }

    private void EndTurn()
    {
        _entity.EndTurn();
    }

    public override float GetScore()
    {
        return baseScore;
    }

}
