using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TurnManager : MonoBehaviour
{
    private Grid _grid;
    public List<GridEntity> GridEntities;

    private int turnIndex = -1;

    public GridEntity TurnEntity { get; private set; }
    public System.Action<int> OnTurnChange;

    public TurnManagerDisplay turnDisplay;
    public EndGameDisplay endGameDisplay;

    bool _gameActive = true;

    public void Init(Grid grid)
    {
        _grid = grid;

        GridEntities = new List<GridEntity>();
        GridEntities.AddRange(FindObjectsOfType<GridEntity>());

        GridEntities.ForEach(x => { x.Init(grid); x.OnDeath += RemoveEntity; });
        SortEntities();
    }

    public void StartGame()
    {
        CreateUI();
        NextTurn();
    }

    private void CreateUI()
    {
        UIManager uiM = GameManager.Get().uiManager;

        turnDisplay = Instantiate(turnDisplay, uiM.transform);
        turnDisplay.Init(this);
    }

    private void NextTurn()
    {
        _grid.ClearTileColours();

        if (!_gameActive) return;

        turnIndex++;

        if (turnIndex >= GridEntities.Count) turnIndex = 0;

        if (TurnEntity) TurnEntity.OnTurnEnd -= NextTurn;

        TurnEntity = GridEntities[turnIndex];

        TurnEntity.StartTurn();
        TurnEntity.OnTurnEnd += NextTurn;

        OnTurnChange?.Invoke(turnIndex);
    }

    public void RemoveEntity(GridEntity entity)
    {
        GridEntities.Remove(entity);
        CheckIfOneTeamLeft();
    }

    private void CheckIfOneTeamLeft()
    {
        GridEntity entity = GridEntities[0];

        foreach (GridEntity gridEntity in GridEntities)
        {
            if (entity.team != gridEntity.team) return;
        }

        EndGame(entity.team);
    }

    private void EndGame(Team team)
    {
        _gameActive = false;

        UIManager uiM = GameManager.Get().uiManager;

        endGameDisplay = Instantiate(endGameDisplay, uiM.transform);
        endGameDisplay.Init(this, team);
    }

    public void SortEntities()
    {
        GridEntities = GridEntities.OrderBy(x => x.Initiative).ToList();
    }
}
