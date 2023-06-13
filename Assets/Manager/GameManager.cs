using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public TurnManager turnManager;

    private static GameManager _instance;
    public static GameManager Get() { return _instance; }

    public static int HeatmapMode = 0;

    void Start() => InitialisationChain();

    public void InitialisationChain()
    {
        _instance = this;

        uiManager = Instantiate(uiManager, this.transform);
        Grid grid = FindObjectOfType<Grid>();
        turnManager = Instantiate(turnManager, this.transform);

        grid.Init();
        turnManager.Init(grid);
        FindObjectOfType<Character>().Init();
    }

}
