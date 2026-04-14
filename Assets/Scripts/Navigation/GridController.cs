using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WarOfTanks.MapGen;
using WarOfTanks.Nav;

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public FlowField currentflowField;
    public TilemapPerlinGenerator mapGen;

    public bool displayGrid = true;

    
    public void InitializeFlowField()
    {
        gridSize = new Vector2Int(mapGen.width, mapGen.height);
        currentflowField = new FlowField(cellRadius, gridSize);
        currentflowField.CreateGrid(mapGen);
    }

    void OnDrawGizmos()
    {
        if (!displayGrid) return;

        if (currentflowField != null)
        {
            DrawGrid(gridSize, Color.green, cellRadius);
        }
    }

    void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
    {
        if (currentflowField == null) return;
        
        Gizmos.color = drawColor;

        for (int x = 0; x < drawGridSize.x; x++)
        {
            for (int y = 0; y < drawGridSize.y; y++)
            {
                Cell c = currentflowField.grid[x, y];

                Vector3 center = new Vector3(
                    drawCellRadius * 2 * c.worldPos.x + drawCellRadius, 
                    drawCellRadius * 2 * c.worldPos.y + drawCellRadius, 
                    0
                );
                Vector3 size = Vector3.one * drawCellRadius * 2;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

            Cell dest = currentflowField.GetCellFromWorldPos(worldMousePos);
            currentflowField.CreateIntegrationField(dest);
            currentflowField.CreateFlowField();
        }
    }
}
