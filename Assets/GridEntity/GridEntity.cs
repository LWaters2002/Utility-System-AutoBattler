using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UtilitySystem))]
public class GridEntity : MonoBehaviour
{
    #region Variables

    public EntityInformation info;
    public Team team;

    public System.Action OnTurnEnd;

    [Header("References")]
    public GameObject visual;
    public GameObject selectedIcon;
    public Healthbar healthbar;

    public GridTile ContainingTile { get; private set; }
    public int MoveEnergy { get; private set; }
    public List<EntityAction> Actions { get; private set; }

    public List<HeatmapTile> MovementHeatmap;

    private List<GridTile> _traversibleTiles;
    private Pathfinding_AStar _AStar;

    public float Initiative { get; private set; }
    public Grid grid { get; private set; }

    public UtilitySystem utility { get; private set; }
    private bool _active;

    public System.Action<GridEntity> OnDeath;

    [HideInInspector]
    public bool UsedAction = false;
    [HideInInspector]
    public bool HasMoved = false;

    public System.Action<float, float> OnHealthChange;
    public float Health { get; private set; }

    #endregion
    public T AddAction<T>(EntityAction actionPrefab) where T : EntityAction
    {
        T newAction = Instantiate(actionPrefab) as T;
        newAction.Init(this);

        Actions.Add(newAction);

        return newAction;
    }

    #region Functions
    public void Init(Grid grid)
    {
        this.grid = grid;

        ContainingTile = grid.GetNearestTile(transform.position);
        _AStar = new Pathfinding_AStar(false, false);

        Actions = new List<EntityAction>();

        utility = new UtilitySystem(this);

        Initiative = info.stats.speed + Random.Range(0.0f, 20.0f);
        Health = info.stats.health;


        healthbar.Init(this);

        SetTeamMaterial();

        UpdateTile();
    }

    private void SetTeamMaterial()
    {
        Renderer render = visual.GetComponentInChildren<Renderer>();
        Material _material = render.materials[1];

        float hue = 0;

        switch (team)
        {
            case Team.yellow:
                hue = 60;
                break;
            case Team.blue:
                hue = 195;
                break;
            case Team.purple:
                hue = 285;
                break;
            case Team.orange:
                hue = 30;
                break;
        }

        hue /= 360;

        _material.color = Color.HSVToRGB(hue, .6f, .6f);

    }

    private void UpdateTile()
    {
        if (ContainingTile) ContainingTile.SetEntity(null);
        ContainingTile = ContainingTile._grid.GetNearestTile(transform.position);
        ContainingTile.SetEntity(this);
    }

    public virtual void StartTurn()
    {
        selectedIcon.SetActive(true);
        MoveEnergy = info.stats.moveEnergy;
        UsedAction = false;
        HasMoved = false;

        utility.Run();
        _active = true;
    }
    
    public virtual void EndTurn()
    {
        selectedIcon.SetActive(false);
        OnTurnEnd?.Invoke();
        _active = false;
    }

    private void Update() { if (_active) utility.Tick(Time.deltaTime); }

    public void TakeDamage(GridEntity attacker, float damage)
    {
        Health -= damage;
        if (Health <= 0) Die();

        OnHealthChange?.Invoke(Health, info.stats.health);
    }

    public void Die()
    {
        ContainingTile.SetEntity(null);
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    public List<GridTile> GetTraversible()
    {
        _traversibleTiles = new List<GridTile>();
        _traversibleTiles.Clear();

        List<GridTile> tilesInRange = grid.TilesInRange(ContainingTile, MoveEnergy);

        foreach (GridTile checkTile in tilesInRange)
        {
            if (checkTile.tileType == TileType.unwalkable) continue;
            if (checkTile.TileEntity != null) continue;

            _traversibleTiles.Add(checkTile);
        }

        return _traversibleTiles;
    }

    public void MoveToTile(GridTile tile, System.Action callback = null)
    {
        _AStar.GeneratePath(ContainingTile, tile);
        if (_AStar.Path.Count <= 1) { callback?.Invoke(); return; }

        StartCoroutine(FollowPath(_AStar.Path, callback));
    }

    public void MoveToEntity(GridEntity entity, System.Action callback = null)
    {
        _AStar.GeneratePath(ContainingTile, entity.ContainingTile);
        _AStar.Path.RemoveAt(_AStar.Path.Count - 1);
        if (_AStar.Path.Count <= 1) { callback?.Invoke(); return; }

        StartCoroutine(FollowPath(_AStar.Path, callback));
    }

    public void RotateToDirection(Quaternion targetRotation, float timeToRotate, System.Action callback = null)
    {
        StartCoroutine(IERotateToDirection(targetRotation, timeToRotate, callback));
    }

    public void Wait(float time, System.Action callback = null)
    {
        StartCoroutine(IEWait(time, callback));
    }
    #endregion

    #region Coroutines
    IEnumerator FollowPath(List<Vector3> path, System.Action callback = null)
    {
        HasMoved = true;
        int index = 1;

        float alpha = 0f;

        if (Vector3.Dot(transform.forward, (path[index] - transform.position).normalized) < .7f)
        {
            Quaternion rotation = Quaternion.LookRotation(path[index] - transform.position);
            yield return StartCoroutine(IERotateToDirection(rotation, .3f));
        }

        while (index < path.Count && MoveEnergy > 0)
        {
            Vector3 tPos = Vector3.Lerp(path[index - 1], path[index], alpha);
            float hopOffset = Mathf.Sin(alpha * Mathf.PI) * 1.2f;

            transform.position = new Vector3(tPos.x, tPos.y + hopOffset, tPos.z);
            alpha += Time.deltaTime * info.stats.speed;

            alpha = Mathf.Clamp01(alpha);

            if (Vector3.Distance(path[index], transform.position) < .02f)
            {
                index++;
                MoveEnergy--;
                alpha = 0f;

                if (index == path.Count) continue;

                if (Vector3.Dot(transform.forward, (path[index] - transform.position).normalized) < .7f)
                {
                    Quaternion rotation = Quaternion.LookRotation(path[index] - transform.position);
                    yield return StartCoroutine(IERotateToDirection(rotation, .3f));
                }
            }

            yield return null;
        }

        UpdateTile();

        callback?.Invoke();
    }

    IEnumerator IERotateToDirection(Quaternion targetRotation, float timeToRotate, System.Action callback = null)
    {
        float alpha = 0;
        float alphaMultiplier = 1 / timeToRotate;

        Quaternion startingRotation = visual.transform.rotation;

        while (alpha < 1)
        {
            alpha += Time.deltaTime * alphaMultiplier;
            alpha = Mathf.Clamp01(alpha);

            visual.transform.rotation = Quaternion.Lerp(startingRotation, targetRotation, alpha);

            yield return null;
        }

        callback?.Invoke();
    }

    private IEnumerator IEWait(float time, System.Action callback = null)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }
    #endregion
}

public enum Team
{
    yellow, purple, orange, blue
}