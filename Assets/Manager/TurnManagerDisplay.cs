using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TurnManagerDisplay : MonoBehaviour
{

    public TextMeshProUGUI turnText;
    public TextMeshProUGUI taskText;

    public Toggle HeatmapAll;
    public Toggle HeatmapOffence;
    public Toggle HeatmapDefence;
    public Toggle HeatmapSafety;

    private TurnManager _turnManager;

    private bool _heatmapShown = false;

    public void Init(TurnManager turnManager)
    {
        _turnManager = turnManager;
        turnManager.OnTurnChange += UpdateTurnText;

        BindHeatmapToggles();
    }

    #region HeatmapToggles

    private void BindHeatmapToggles()
    {
        HeatmapAll.onValueChanged.AddListener(AllClicked);
        HeatmapOffence.onValueChanged.AddListener(OffenceClicked);
        HeatmapDefence.onValueChanged.AddListener(DefenceClicked);
        HeatmapSafety.onValueChanged.AddListener(SafetyClicked);
    }
    private void SafetyClicked(bool check)
    {
        if (!check) { _turnManager.TurnEntity.grid.ClearHeatmap(); return; }

        GameManager.HeatmapMode = 3;

        HeatmapAll.SetIsOnWithoutNotify(false);
        HeatmapOffence.SetIsOnWithoutNotify(false);
        HeatmapDefence.SetIsOnWithoutNotify(false);
    }

    private void DefenceClicked(bool check)
    {
        if (!check) { _turnManager.TurnEntity.grid.ClearHeatmap(); return; }

        GameManager.HeatmapMode = 2;

        HeatmapAll.SetIsOnWithoutNotify(false);
        HeatmapOffence.SetIsOnWithoutNotify(false);
        HeatmapSafety.SetIsOnWithoutNotify(false);
    }

    private void OffenceClicked(bool check)
    {
        if (!check) { _turnManager.TurnEntity.grid.ClearHeatmap(); return; }

        GameManager.HeatmapMode = 1;

        HeatmapAll.SetIsOnWithoutNotify(false);
        HeatmapDefence.SetIsOnWithoutNotify(false);
        HeatmapSafety.SetIsOnWithoutNotify(false);
    }

    private void AllClicked(bool check)
    {
        if (!check) { _turnManager.TurnEntity.grid.ClearHeatmap(); return; }

        GameManager.HeatmapMode = 0;

        HeatmapOffence.SetIsOnWithoutNotify(false);
        HeatmapDefence.SetIsOnWithoutNotify(false);
        HeatmapSafety.SetIsOnWithoutNotify(false);
    }

    #endregion

    private void UpdateTurnText(int turn)
    {
        turnText.text = "Turn " + turn;
    }

    private void Update() => UpdateTaskText();

    private void UpdateTaskText()
    {
        if (!_turnManager.TurnEntity) return;

        ATask task = _turnManager.TurnEntity.utility.ActiveTask;
        if (!task) return;

        taskText.text = "Task : " + task.label;
    }

}
