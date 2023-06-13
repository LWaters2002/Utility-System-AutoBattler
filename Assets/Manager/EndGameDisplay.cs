using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class EndGameDisplay : MonoBehaviour
{

    public Button restartButton;
    public TextMeshProUGUI winText;

    private TurnManager _turnManager;

    public void Init(TurnManager turnManager, Team winningTeam)
    {
        _turnManager = turnManager;
        SetWinText(winningTeam);
        restartButton.onClick.AddListener(ReloadLevel);
    }

    private void SetWinText(Team winningTeam)
    {
        string teamName = winningTeam.ToString();
        winText.text = teamName + " Team\nWins!";
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
