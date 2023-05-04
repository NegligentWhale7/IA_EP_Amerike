using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] Camera main;
    public Tilemap tileMap;
    public Vector3 offset = new Vector3(0f, 0.03f, 0f);
    public TileBase originTile, destinyTile;

    private Dictionary<Tilemap, Vector3Int> _previousPosition = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _origin = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _goal = new Dictionary<Tilemap, Vector3Int>();

    [SerializeField] BreadthSearchFirst scanArea;
    [SerializeField] EarlyExit earlyE;
    [SerializeField] Dijkstra dijkstraArea;
    [SerializeField] HeuristicSearch heuristicSearch;
    [SerializeField] Star starArea;

    private enum SelectorType { FloodField, EarlyExit, Dijkstra, Heuristic, Star }
    [SerializeField] private SelectorType _selectorType;

    private void Start()
    {
        _previousPosition[tileMap] = new Vector3Int(-1, -1, 0);
    }


    private void Update()
    {
        SelectTile();

        if (Input.GetMouseButtonDown(0))
        {
            DetectTileClick(isOrigin: true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            DetectTileClick(isOrigin: false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (_selectorType)
            {
                case SelectorType.FloodField:
                    StartFlooFill();
                    break;
                case SelectorType.EarlyExit:
                    StartEarlyExitScan();
                    break;
                case SelectorType.Dijkstra: 
                    StartDijkstra();
                    break;
                case SelectorType.Heuristic:
                    StartHeuristic();
                    break;
                case SelectorType.Star:
                    StartStar();
                    break;
            }
        }
    }


    private void SelectTile()
    {
        Vector3 mousePos = main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePosition = tileMap.WorldToCell(mousePos);
        tilePosition.z = 0;

        //Debug.Log($"Tile Selected {tilePosition}");

        if (tileMap.HasTile(tilePosition))
        {
            tileMap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(offset, Quaternion.Euler(0, 0, 0), Vector3.one));
            tileMap.SetTransformMatrix(_previousPosition[tileMap], Matrix4x4.identity);

            _previousPosition[tileMap] = tilePosition;
        }
    }


    private void DetectTileClick(bool isOrigin)
    {
        Vector3 mousePos = main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePosition = tileMap.WorldToCell(mousePos);
        tilePosition.z = 0;

        TileBase newTile = isOrigin ? originTile : destinyTile;
        Dictionary<Tilemap, Vector3Int> selectedDictionary = isOrigin ? _origin : _goal;

        if (tileMap.HasTile(tilePosition))
        {
            var oldTile = tileMap.GetTile(tilePosition);
            tileMap.SetTile(tilePosition, newTile);

            if (selectedDictionary.ContainsKey(tileMap))
            {
                tileMap.SetTile(selectedDictionary[tileMap], oldTile);

            }
            selectedDictionary[tileMap] = tilePosition;
        }
    }

    private void StartFlooFill()
    {
        scanArea.Origin = _origin[tileMap];
        scanArea.Goal = _goal[tileMap];
        scanArea.tileMap = tileMap;
        scanArea.visitedTile = originTile;
        scanArea.pathTile = destinyTile;

        StartCoroutine(scanArea.FloodField(0.0001f));
    }

    private void StartEarlyExitScan()
    {
        earlyE.Origin = _origin[tileMap];
        earlyE.Goal = _goal[tileMap];
        earlyE.tileMap = tileMap;
        earlyE.visitedTile = originTile;
        earlyE.pathTile = destinyTile;

        StartCoroutine(earlyE.FloodField(0.0001f));
    }

    private void StartDijkstra()
    {
        dijkstraArea.Origin = _origin[tileMap];
        dijkstraArea.Goal = _goal[tileMap];
        dijkstraArea.tileMap = tileMap;
        dijkstraArea.visitedTile = originTile;
        dijkstraArea.pathTile = destinyTile;

        StartCoroutine(dijkstraArea.FloodField(0.0001f));
    }


    private void StartHeuristic()
    {
        heuristicSearch.Origin = _origin[tileMap];
        heuristicSearch.Goal = _goal[tileMap];
        heuristicSearch.tileMap = tileMap;
        heuristicSearch.visitedTile = originTile;
        heuristicSearch.pathTile = destinyTile;

        StartCoroutine(heuristicSearch.FloodField(0.0001f));
    }


    private void StartStar()
    {
        starArea.Origin = _origin[tileMap];
        starArea.Goal = _goal[tileMap];
        starArea.tileMap = tileMap;
        starArea.visitedTile = originTile;
        starArea.pathTile = destinyTile;

        StartCoroutine(starArea.FloodField(0.0001f));
    }
}
