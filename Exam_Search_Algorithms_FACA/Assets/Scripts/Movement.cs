using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public Tilemap tilemap;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 newPosition = transform.position + new Vector3(horizontalInput, verticalInput, 0) * speed * Time.deltaTime;
        Vector3Int cellPosition = tilemap.WorldToCell(newPosition);
        TileBase tile = tilemap.GetTile(cellPosition);

        if (tile == null)
        {
            transform.position = newPosition;
        }
    }
}
