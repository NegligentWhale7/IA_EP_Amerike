using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldPosition : MonoBehaviour
{
    [SerializeField] BreadthSearchFirst Scan;
    [SerializeField] Grid grid;
    [SerializeField] Tilemap tilemap;
    [SerializeField] float cellSize;
    [SerializeField] float scanTime;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = grid.WorldToCell(new Vector3(mousePosition.x, mousePosition.y, 0f));
            Vector3 worldPosition = grid.CellToWorld(cellPosition) + new Vector3(cellSize / 2f, cellSize / 2f, 0f);
            Debug.Log("Clicked on cell " + cellPosition + " at position " + worldPosition);
            Scan.Origin = cellPosition;
            Scan.StartCoroutine(Scan.FloodField(0.01f));
        }
    }
}
