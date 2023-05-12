using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] Camera main;
    [SerializeField] Grid grid;
    public TileBase originTile, destinyTile;
    public Tilemap tileMap;
    [Header("Characters")]
    public Vector3 offset = new Vector3(0f, 0.02f, 0f);
    [SerializeField] Character scanArea;
    public Tilemap infantery;
    public Tilemap tank;
    public Tilemap reconocimiento;
    public Tilemap Caballeria;
    [SerializeField] TileBase playerSprite, treeSprite, bearSprite, slimeSprite;

    [HideInInspector]
    public TileBase tb;

    private Dictionary<Tilemap, Vector3Int> _previousPosition = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _origin = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _goal = new Dictionary<Tilemap, Vector3Int>();

    [SerializeField] private enum CharacterType { Player, Tree, Slime, Bear }
    [SerializeField] private CharacterType _characterType;

    bool _isPlayerSelected = false;
    Vector3Int tilePosition;

    private void Start()
    {
        _previousPosition[tileMap] = new Vector3Int(-1, -1, 0);
    }


    private void Update()
    {
        if (!_isPlayerSelected) SelectTile();

        if (Input.GetMouseButtonDown(0))
        {
            switch (_characterType)
            {
                case CharacterType.Player:
                    if (infantery.HasTile(tilePosition) && !_isPlayerSelected)
                    {
                        scanArea.playerSprite = playerSprite;
                        scanArea.playerTile = infantery;
                        scanArea.snowCost = 10;
                        scanArea.lavaCost = 1000;
                        scanArea.waterCost = 50;
                        scanArea.rockCost = 2;
                        scanArea.iceCost = 200;
                        GetCharacterData(80);
                    }
                    break;
                case CharacterType.Tree:
                    if (tank.HasTile(tilePosition) && !_isPlayerSelected)
                    {
                        scanArea.playerSprite = playerSprite;
                        scanArea.playerTile = infantery;
                        scanArea.snowCost = 3;
                        scanArea.lavaCost = 1000;
                        scanArea.waterCost = 2;
                        scanArea.rockCost = 20;
                        scanArea.iceCost = 20;
                        scanArea.playerSprite = treeSprite;
                        scanArea.playerTile = tank;
                        GetCharacterData(50);
                    }
                    break;
                case CharacterType.Slime:
                    if (Caballeria.HasTile(tilePosition) && !_isPlayerSelected)
                    {
                        scanArea.playerSprite = slimeSprite;
                        scanArea.snowCost = 1;
                        scanArea.lavaCost = 1;
                        scanArea.waterCost = 1;
                        scanArea.rockCost = 1;
                        scanArea.iceCost = 1;
                        scanArea.playerTile = Caballeria;
                        GetCharacterData(70);
                    }
                    break;
                case CharacterType.Bear:
                    if (reconocimiento.HasTile(tilePosition) && !_isPlayerSelected)
                    {
                        scanArea.playerSprite = bearSprite;
                        scanArea.snowCost = 300;
                        scanArea.lavaCost = 10;
                        scanArea.waterCost = 200;
                        scanArea.rockCost = 2;
                        scanArea.iceCost = 2000;
                        scanArea.playerTile = reconocimiento;
                        GetCharacterData(100);
                    }
                    break;
            }
        }
        if (Input.GetMouseButtonDown(1) && _isPlayerSelected)
        {
            DetectTileClick(isOrigin: false);
            scanArea.DrawPath(tilePosition);
        }

        if (_isPlayerSelected && Input.GetKeyDown(KeyCode.Return)) { scanArea.MovePlayer(); }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isPlayerSelected = false;
            scanArea.ClearTiles();
        }

    }

    private void GetCharacterData(int maxSteps)
    {
        _isPlayerSelected = true;
        scanArea.maxSteps = maxSteps;
        DetectTileClick(isOrigin: true);
        ShowMovementArea();
        DetectTileClick(isOrigin: false);
    }


    private void SelectTile()
    {
        Vector3 mousePosition = main.ScreenToWorldPoint(Input.mousePosition);
        tilePosition = tileMap.WorldToCell(mousePosition);
        tilePosition.z = 0;

        if (tilePosition != _previousPosition[tileMap])
        {
            if (tileMap.HasTile(tilePosition))
            {
                tileMap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(offset, Quaternion.Euler(0, 0, 0), Vector3.one));
            }
            if (tileMap.HasTile(_previousPosition[tileMap]))
            {
                tileMap.SetTransformMatrix(_previousPosition[tileMap], Matrix4x4.identity);
            }
            _previousPosition[tileMap] = tilePosition;
        }
    }


    private void DetectTileClick(bool isOrigin)
    {
        Vector3 mousePos = main.ScreenToWorldPoint(Input.mousePosition);
        tilePosition = tileMap.WorldToCell(mousePos);
        tilePosition.z = 0;

        TileBase newTile = isOrigin ? originTile : destinyTile;
        Dictionary<Tilemap, Vector3Int> selectedDictionary = isOrigin ? _origin : _goal;

        if (tileMap.HasTile(tilePosition))
        {
            var oldTile = tileMap.GetTile(tilePosition);
            selectedDictionary[tileMap] = tilePosition;
        }
    }

    private void ShowMovementArea()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = grid.WorldToCell(new Vector3(mousePosition.x, mousePosition.y, 0f));
        Vector3 worldPosition = grid.CellToWorld(cellPosition) + new Vector3(1 / 2f, 1 / 2f, 0f);
        Debug.Log("Clicked on cell " + cellPosition + " at position " + worldPosition);
        scanArea.Origin = cellPosition;
        scanArea.StartScan();
    }
}
