using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveRandom_Task", menuName = "ScriptObjects/Task/MoveRandom", order = 0)]
public class MoveRandom_Task : ATask
{
    public override void Entry()
    {
        _entity.Wait(1f, Next);
        List<GridTile> travelTiles = _entity.GetTraversible();
        travelTiles.PaintTiles(Color.green);
    }

    private void Next()
    {
        _entity.grid.PaintTileTypes();

        List<GridTile> travelTiles = _entity.GetTraversible();
        GridTile targetTile = travelTiles[Random.Range(0, travelTiles.Count)];
        _entity.MoveToTile(targetTile, End);
    }


}