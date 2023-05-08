using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EarlyExit : MonoBehaviour
{
    public Tilemap tileMap;
    public float DelayTime;
    public TileBase TBase;

    public Vector3 Origin;
    public Vector3 Goal;
    private Queue<Vector3> _frontier = new Queue<Vector3>();
    private Dictionary<Vector3, Vector3> _cameFrom = new Dictionary<Vector3, Vector3>();
    public TileBase visitedTile, pathTile;

    private bool isEarlyExit = false;


    public void StartScan()
    {
        StartCoroutine(FloodField(DelayTime));
    }


    public IEnumerator FloodField(float time)
    {
        _frontier.Enqueue(Origin);
        _cameFrom[Origin] = Vector3.zero;

        while (_frontier.Count > 0 && !isEarlyExit)
        {
            Vector3 current = _frontier.Dequeue();
            foreach (Vector3 next in GetNeighbors(current))
            {
                if (next == Goal) { isEarlyExit = true; yield return null; }
                if (!_cameFrom.ContainsKey(next))
                {
                    yield return new WaitForSeconds(time);
                    _frontier.Enqueue(next);
                    _cameFrom[next] = current;
                }
            }
        }
        DrawPath(Goal);
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
        Vector3Int nextInt = new Vector3Int((int)next.x, (int)next.y, (int)next.z);
        if (_cameFrom.ContainsValue(next)) { return; }
        if (!tileMap.HasTile(nextInt)) { return; }
        if (_frontier.Contains(nextInt)) { return; }

        coordList.Add(nextInt);
        tileMap.SetTile(nextInt, TBase);
    }

    void DrawPath(Vector3 goal)
    {
        Vector3 current = goal;
        while (current != Origin)
        {
            Vector3Int currentInt = new Vector3Int((int)current.x, (int)current.y, (int)current.z);
            tileMap.SetTile(currentInt, pathTile);
            current = _cameFrom[current];
        }
    }
}
