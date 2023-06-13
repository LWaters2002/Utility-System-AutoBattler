using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionTask_Task", menuName = "ScriptObjects/Task/Action", order = 0)]
public class ActionTask : ATask
{
    public EntityAction actionPrefab;

    [HideInInspector]
    public EntityAction _action;

    public override void Entry()
    {
        _action.Use(End);
        _entity.UsedAction = true;
    }

    public override void Init(GridEntity entity)
    {
        base.Init(entity);
        _action = entity.AddAction<EntityAction>(actionPrefab);
    }

}
