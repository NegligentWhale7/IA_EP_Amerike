using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldPosition : MonoBehaviour
{
    [Header("Cells")]
    public Grid AGrid;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] ScanArea scan;

    Vector3 mousePos, worldPosition;
    Vector3Int gridPos;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            GetMousePosition();
        }
    }


    private void GetMousePosition()
    {
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f));
        gridPos = AGrid.WorldToCell(mousePos);
        Vector3Int gridPosInt= new Vector3Int (gridPos.x, gridPos.y, 0); 
        worldPosition = AGrid.CellToWorld(gridPos) + new Vector3(cellSize / 2f, cellSize / 2f, 0f);
        Vector3Int worldPosInt = new Vector3Int ((int)worldPosition.x, (int)worldPosition.y, 0); 

        Debug.Log("Posición del mundo: " + worldPosition);
        scan.Origin = worldPosInt;
        scan.StartScan();
    }
}
