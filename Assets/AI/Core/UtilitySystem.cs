using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilitySystem
{
    private List<ATask> _tasks;
    private GridEntity _entity;

    private float _intelligence;

    public ATask ActiveTask { get; private set; }

    public UtilitySystem(GridEntity entity)
    {
        _entity = entity;
        _intelligence = entity.info.GetWeight(WeightNames.intelligence);

        _tasks = new List<ATask>();

        foreach (ATask task in entity.info.Tasks)
        {
            ATask instancedTask = GameObject.Instantiate(task);
            instancedTask.Init(entity);

            _tasks.Add(instancedTask);
        }
    }

    public void Run()
    {
        _entity.MovementHeatmap = EntityHelper.CalculateHeatmap(_entity);
        _entity.MovementHeatmap.ShowHeatmap(GameManager.HeatmapMode);

        _entity.grid.PaintTileTypes();

        ATask bestTask = _tasks[0]; bestTask.GetScore();
        List<ATask> viableTasks = new List<ATask>();

        foreach (ATask task in _tasks)
        {
            float score = task.GetScore();

            if (score > bestTask.Score) bestTask = task;

            if (task.Score > _intelligence) viableTasks.Add(task);
        }

        if (viableTasks.Count == 0) { ExecuteTask(bestTask); return; }

        ATask randomTask = viableTasks[Random.Range(0, viableTasks.Count)];

        ExecuteTask(randomTask);
    }

    private void ExecuteTask(ATask task)
    {
        if (ActiveTask) ActiveTask.OnEnd -= TaskFinished;

        ActiveTask = task;
        ActiveTask.OnEnd += TaskFinished;

        task.Entry();
    }

    private void TaskFinished() => Run();

    public void Tick(float deltaTime) => ActiveTask?.Tick(deltaTime);
}
