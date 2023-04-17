using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseReference : MonoBehaviour
{
    public Grid AGrid;
    [SerializeField] float cellSize = 1f; 

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            GetMousePosition();
        }
    }


    private void GetMousePosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = AGrid.WorldToCell(mousePos);
        Vector3 worldPosition = AGrid.CellToWorld(gridPos) + new Vector3(cellSize / 2f, cellSize / 2f, 0f);

        Debug.Log("Posición del mundo: " + worldPosition);
    }
}
