using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ScanArea : MonoBehaviour
{
    [SerializeField] Tilemap tile;
    public float DelayTime;
    public TileBase TBase;

    public Vector3 Origin;
    private Queue<Vector3> _frontier = new Queue<Vector3>();
    private Dictionary<Vector3, Vector3> _cameFrom = new Dictionary<Vector3, Vector3>();

    private void Start()
    {
        //StartCoroutine(FloodField(delayTime));
    }

    [ContextMenu("Start Scan")]
    public void StartScan()
    {
        StartCoroutine(FloodField(DelayTime));
    }


    public IEnumerator FloodField(float time)
    {
        _frontier.Enqueue(Origin);
        _cameFrom[Origin] = Vector3.zero;

        while(_frontier.Count > 0 )
        {
            Vector3 current = _frontier.Dequeue();
            Debug.Log(current);
            foreach (Vector3 next in GetNeighbors(current))
            {
                if (!_cameFrom.ContainsKey(next))
                {
                    yield return new WaitForSeconds(time);
                    _frontier.Enqueue(next);
                    _cameFrom[next] = current;
                }
            }
        }

    }


    private List<Vector3> GetNeighbors(Vector3 current)
    {
        Vector3Int currentInt = new Vector3Int((int)current.x, (int)current.y, (int)current.z);
        List<Vector3> neighbors = new List<Vector3>();
        Vector3Int neighborR = currentInt + Vector3Int.right;
        Vector3Int neighborL = currentInt + Vector3Int.left;
        Vector3Int neighborU = currentInt + Vector3Int.up;
        Vector3Int neighborD = currentInt + Vector3Int.down;
        
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
        if (!tile.HasTile(nextInt)) { return; }
        if (_frontier.Contains(nextInt)) { return; }

        coordList.Add(nextInt);
        tile.SetTile(nextInt, TBase);
    }
}
