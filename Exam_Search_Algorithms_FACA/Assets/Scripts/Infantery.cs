using ESarkis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

public class Infantery : MonoBehaviour
{
    public float maxSteps;
    [SerializeField] Tilemap deMap;
    [SerializeField] Vector3 offset;
    public Tilemap tileMap;
    public float DelayTime;
    public TileBase TBase, playerSprite;
    public TileBase visitedTile, pathTile;
    public bool IsPlayerSelected = false;
    public Vector3 Goal { get; set; }
    public Vector3 Origin { get; set; }
    public TileManager ts;

    private PriorityQueue<Vector3> _frontier = new PriorityQueue<Vector3>();
    private Dictionary<Vector3, Vector3> _cameFrom = new Dictionary<Vector3, Vector3>();
    private Dictionary<Vector3, double> _costSoFar = new Dictionary<Vector3, double>();
    private Dictionary<Vector3Int, TileBase> _oldTile = new Dictionary<Vector3Int, TileBase>();
    private float _steps;
    public List<Vector3Int> _pathNodes = new List<Vector3Int>();

    public void StartScan()
    {
        StartCoroutine(FloodField(DelayTime));
    }


    public IEnumerator FloodField(float time)
    {
        _frontier.Enqueue(Origin, 0);
        _cameFrom[Origin] = Vector3.zero;
        _costSoFar[Origin] = 0;

        while (_frontier.Count > 0 && _steps < maxSteps)
        {
            Vector3 current = _frontier.Dequeue();
            foreach (Vector3 next in GetNeighbors(current))
            {
                double newCost = _costSoFar[current] + GetCost(next);
                if (_steps >= maxSteps) { yield return null; }
                if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                {
                    yield return new WaitForSeconds(time);
                    _costSoFar[next] = newCost;
                    _frontier.Enqueue(next, newCost);
                    _cameFrom[next] = current;
                    _steps++;
                }
            }
        }
    }


    private double GetCost(Vector3 next)
    {
        var nextTile = tileMap.GetTile(new Vector3Int((int)next.x, (int)next.y, (int)next.z));
        double cost = nextTile.name switch
        {
            "ground_grass" => 1,
            "ground_asphalt" => 400,
            "road_t_a" => 500,
            "road_straight_a" => 500,
            "road_straight_b" => 500,
            "road_end_c" => 500,
            "road_t_c" => 500,
            "road_end_d" => 500,
            "road_end_a" => 500,
            "road_end_b" => 500,
            "crops_plowed_growth" => 200,
            "ground_water" => 300,
            
            _ => 1
        };

        return cost;
    }
    private List<Vector3> GetNeighbors(Vector3 current)
    {
        Vector3Int currentInt = new Vector3Int((int)current.x, (int)current.y, (int)current.z);
        List<Vector3> neighbors = new List<Vector3>();
        Vector3Int neighborD = currentInt + Vector3Int.down;
        Vector3Int neighborU = currentInt + Vector3Int.up;
        Vector3Int neighborL = currentInt + Vector3Int.left;
        Vector3Int neighborR = currentInt + Vector3Int.right;

        ValidateCoord(neighborR, neighbors);
        ValidateCoord(neighborL, neighbors);
        ValidateCoord(neighborU, neighbors);
        ValidateCoord(neighborD, neighbors);
        return neighbors;
    }


    private void ValidateCoord(Vector3 next, List<Vector3> coordList)
    {
        Dictionary<Vector3Int, TileBase> _oldT = new Dictionary<Vector3Int, TileBase>();
        Vector3Int nextInt = new Vector3Int((int)next.x, (int)next.y, (int)next.z);
        if (_cameFrom.ContainsValue(next)) { return; }
        if (!tileMap.HasTile(nextInt)) { return; }
        if (_frontier.Contains(nextInt)) { return; }

        coordList.Add(nextInt);
        deMap.SetTile(nextInt, TBase);
        //tileMap.SetTile(nextInt, TBase);
    }


    public void DrawPath(Vector3 goal)
    {

        if (_oldTile.Count > 0)
        {
            _pathNodes.Clear();
            Debug.Log("0");
            Vector3 currentReturn = Origin;

            for (int i = _oldTile.Count - 1; i >= 0; i--)
            {
                Vector3Int currentint = _oldTile.ElementAt(i).Key;
                TileBase tile = _oldTile.ElementAt(i).Value;
                deMap.SetTile(currentint, tile);
            }
        }

        Vector3 current = goal;

        while (current != Origin)
        {
            Vector3Int currentInt = new Vector3Int((int)current.x, (int)current.y, (int)current.z);
            if (!_oldTile.ContainsKey(currentInt))
            {
                _oldTile.Add(currentInt, deMap.GetTile(currentInt));
            }
            if (!deMap.HasTile(currentInt)) { return; }
            deMap.SetTile(currentInt, pathTile);
            // playerMap.SetTile(currentInt, playerSprite);
            _pathNodes.Add(currentInt);
            current = _cameFrom[current];

        }


    }

    public void MovePlayer()
    {
        Vector3 current = Goal;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = ts.infantery.WorldToCell(new Vector3(mousePosition.x, mousePosition.y, 0f));
        Goal = cellPosition;
        // current = _cameFrom[Goal];
        Vector3Int currentInt = new Vector3Int((int)current.x, (int)current.y, (int)current.z);

        int indexNodes = _pathNodes.Count - 1;
        Vector3Int newVi = _pathNodes[indexNodes];
        //  playerMap.SetTile(newVi, playerSprite);

        Move();
        //playerMap.SetTile(cellPosition, playerSprite);

    }


    async void Move()
    {
        for (int i = _pathNodes.Count - 1; i >= 0; i--)
        {
            ts.infantery.SetTile(_pathNodes[i], playerSprite);
            ts.infantery.SetTransformMatrix(_pathNodes[i], Matrix4x4.TRS(offset, Quaternion.Euler(0, 0, 0), Vector3.one));
            await Task.Delay(1000);
            if (i != 0)
            {
                ts.infantery.ClearAllTiles();
            }
        }
    }


    public void ClearTiles()
    {
        StopAllCoroutines();
        deMap.ClearAllTiles();
        _steps = 0;
        _frontier.Clear();
        _cameFrom.Clear();
        _costSoFar.Clear();
        _oldTile.Clear();
        _pathNodes.Clear();
    }
}
