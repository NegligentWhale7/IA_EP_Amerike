using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    [SerializeField] Camera main;
    public Tilemap tileMap;
    public Vector3 offset = new Vector3(0f, 0.03f, 0f);
    public TileBase originTile, destinyTile;

    private Dictionary<Tilemap, Vector3Int> _previousPosition = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _origin = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _goal = new Dictionary<Tilemap, Vector3Int>();

    public ScanArea scanArea;

    //Project Settings: (0,1,-1)
    //TileMap - Mode: Individual
    private void Start()
    {
        _previousPosition[tileMap] = new Vector3Int(-1, -1, 0); 
    }


    private void Update()
    {
        SelectTile();

        if (Input.GetMouseButtonDown(0))
        {
            DetectTileClick(isOrigin : true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            DetectTileClick(isOrigin : false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartFlooFill();
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
        scanArea.Goal= _goal[tileMap];
        scanArea.tileMap = tileMap;
        scanArea.visitedTile = originTile;
        scanArea.pathTile = destinyTile;

        StartCoroutine(scanArea.FloodField(0.0001f));
    }
}
