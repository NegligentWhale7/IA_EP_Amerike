using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using ESarkis;
using UnityEngine.Events;
using System.Collections;
using System.Linq;

public class NewPath : MonoBehaviour
{
    public Vector3 Origin { get; set; }
    public Vector3 Goal { get; set; }
    [Header("Settings")]
    public Tilemap TileMap;
    public TileBase visitedTile;
    public TileBase pathTile, originalTile;
    public float time = .01f;
    [Header("Event")]
    public UnityEvent Completed;
    public bool finish;
    [Header("Tipo de pieza")]
    public Behaviour mode;
    public enum Behaviour
    {
        peon,
        caballero,
    }


    private PriorityQueue<Vector3> _frontier = new PriorityQueue<Vector3>();
    private Dictionary<Vector3, Vector3> _cameFrom = new Dictionary<Vector3, Vector3>();
    private Dictionary<Vector3, double> _costSoFar = new Dictionary<Vector3, double>();
    private Dictionary<Vector3Int, TileBase> _oldTile = new Dictionary<Vector3Int, TileBase>();

    public IEnumerator FloodFill2D()
    {
        _frontier.Enqueue(Origin, 0);
        _cameFrom[Origin] = Vector3.zero;
        _costSoFar[Origin] = 0;
        Debug.Log("Se inicializa");
        Debug.Log(_frontier.Count + " Frontier:Count");
        while (_frontier.Count > 0)
        {
            Vector3 current = _frontier.Dequeue();
            foreach (Vector3 next in GetNeighbours(current))
            {
                Debug.Log("Busca a los vecinos");
                //if (next == Goal) //yield return finish = true;
                var newCost = _costSoFar[current] + GetCost(next);
                //Esta condicion es la que determina el camino mas optimo dentro de nuestros tiles. 
                if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                {
                    Debug.Log("Busqueda");
                    _costSoFar[next] = newCost;
                    var priority = newCost + Heuristic(Goal, next);
                    _frontier.Enqueue(next, priority);
                    _cameFrom[next] = current;
                }
            }
        }
        yield return _frontier;
        DrawPath(Goal);
    }
    private float Heuristic(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private double GetCost(Vector3 next)
    {
        var nextTile = TileMap.GetTile(new Vector3Int((int)next.x, (int)next.y, (int)next.z));
        //Debug.Log(nextTile.name + "name");
        double cost = nextTile.name switch
        {
            "isometric_angled_pixel_0000" => 1,
            "isometric_angled_pixel_0007" => 2,
            "isometric_angled_pixel_0002" => 3,
            "isometric_angled_pixel_0013" => 4,
            "isometric_angled_pixel_0010" => 5,
            _ => 0
        };
        //Debug.Log(cost + "Cost");
        return cost;
        //return _weightTile[nextTile]; 
    }


    public void DrawPath(Vector3 goal)
    {
        if (_oldTile.Count > 0)
        {
            Debug.Log("0");
            Vector3 currentReturn = Origin;

            for (int i = _oldTile.Count - 1; i >= 0; i--)
            {
                Vector3Int currentint = _oldTile.ElementAt(i).Key;
                TileBase tile = _oldTile.ElementAt(i).Value;
                TileMap.SetTile(currentint, tile); // usar el tile original en lugar de pathtile
            }
        }

        Debug.Log("Draw");

        Vector3 current = goal;

        while (current != Origin)
        {
            Vector3Int currentInt = new Vector3Int((int)current.x, (int)current.y, (int)current.z);
            if (!_oldTile.ContainsKey(currentInt))
            {
                _oldTile.Add(currentInt, TileMap.GetTile(currentInt));
            }
            TileMap.SetTile(currentInt, originalTile); // Usar la variable originalTile en lugar de pathTile
            
            current = _cameFrom[current];
        }
        Completed.Invoke();
        //_oldTile.Clear();
    }

    //Esto valida las posiciones del codigo, y checa las casillas circundantes.
    private List<Vector3> GetNeighbours(Vector3 Current)
    {
        List<Vector3> neighbours = new List<Vector3>();
        ValidateCoordinate(Current + Vector3.right, neighbours);
        ValidateCoordinate(Current + Vector3.left, neighbours);
        ValidateCoordinate(Current + Vector3.up, neighbours);
        ValidateCoordinate(Current + Vector3.down, neighbours);

        return neighbours;
    }

    private void ValidateCoordinate(Vector3 neighbour, List<Vector3> neighbours)
    {
        Vector3Int neighbourInt = new Vector3Int((int)neighbour.x, (int)neighbour.y, (int)neighbour.z);
        if (!TileMap.HasTile(neighbourInt)) return;
        if (!_frontier.Contains(neighbour))
        {
            TileFlags flags = TileMap.GetTileFlags(neighbourInt);

            neighbours.Add(neighbour);
            //TileMap.SetTile(neighbourInt, visitedTile);
            //TileMap.SetTileFlags(neighbourInt, flags);
        }
    }
}
