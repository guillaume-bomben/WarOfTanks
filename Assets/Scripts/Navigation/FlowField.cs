using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using WarOfTanks.MapGen;

namespace WarOfTanks.Nav
{
    public class FlowField
    {
        public Cell[,] grid { get; private set; }
        public Vector2Int gridSize { get; private set; }
        public float cellRadius { get; private set; }

        public Cell destinationCell;

        float cellDiameter;


        public FlowField(float cellRadius, Vector2Int gridSize, TilemapPerlinGenerator mapGen = null)
        {
            this.cellRadius = cellRadius;
            this.gridSize = mapGen ? new Vector2Int(mapGen.width, mapGen.height) : gridSize;
            this.cellDiameter = cellRadius * 2f;
        }

        public void CreateFlowField()
        {
            foreach (Cell c in grid)
            {
                List<Cell> currNeighbors = GetNeighborCells(c.gridIndex, GridDirection.AllDirections);
                int bestCost = c.bestCost;

                foreach (Cell neighbor in currNeighbors)
                {
                    if (neighbor.bestCost < bestCost)
                    {
                        bestCost = neighbor.bestCost;
                        c.bestDirection = GridDirection.GetDirectionFromV2I(neighbor.gridIndex - c.gridIndex);
                    }
                }
            }
        }

        public void CreateGrid(TilemapPerlinGenerator mapGen)
        {
            grid = new Cell[gridSize.x, gridSize.y];
            Vector3 worldOffset = new Vector3(
                -gridSize.x * cellDiameter / 2f, 
                -gridSize.y * cellDiameter / 2f, 
                0
            );

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 worldPos = new Vector3(cellDiameter * x + cellRadius, cellDiameter * y + cellRadius, 0) + worldOffset;
                    Cell currentCell = new Cell(worldPos, new Vector2Int(x, y));
                    grid[x, y] = currentCell;
                    Vector3Int tilePos = mapGen.worldMap.WorldToCell(worldPos);

                    if (mapGen.unwalkableMap.HasTile(tilePos) || mapGen.waterMap.HasTile(tilePos))
                        currentCell.IncreaseCost(byte.MaxValue);
                    else
                    {
                        currentCell.IncreaseCost(1);
                        if (mapGen.hazardMap.HasTile(tilePos))
                        {
                            TerrainDataModifier modifier = mapGen.GetModifierAtWorldPos(worldPos);
                            if (modifier != null)
                            {
                                // Apply modifier
                                currentCell.IncreaseCost(10);
                            }
                        }
                    }
                }
            }
        }

        public void CreateIntegrationField(Cell destinationCell)
        {
            this.destinationCell = destinationCell;
            this.destinationCell.cost = 0;
            this.destinationCell.bestCost = 0;

            Queue<Cell> cellsToCheck = new Queue<Cell>();
            cellsToCheck.Enqueue(this.destinationCell);

            while (cellsToCheck.Count > 0)
            {
                Cell c = cellsToCheck.Dequeue();
                List<Cell> neighbors = GetNeighborCells(c.gridIndex, GridDirection.CardinalDirections);
                foreach (Cell n in neighbors)
                {
                    if (n.cost == byte.MaxValue)
                        continue;
                    
                    if (n.cost + c.bestCost < n.bestCost)
                    {
                        n.bestCost = (ushort)(n.cost + c.bestCost);
                        cellsToCheck.Enqueue(n);
                    }
                }
            }
        }

        List<Cell> GetNeighborCells(Vector2Int nodeIndex, GridDirection.Directions directions)
        {
            List<Cell> neighbors = new List<Cell>();

            foreach (Vector2Int currDir in directions)
            {
                Cell newNeighbor = GetCellAtRelativePos(nodeIndex, currDir);
                if (newNeighbor != null)
                    neighbors.Add(newNeighbor);
            }

            return neighbors;
        }

        Cell GetCellAtRelativePos(Vector2Int origin, Vector2Int relativePos)
        {
            Vector2Int finalPos = origin + relativePos;

            if (finalPos.x < 0 || finalPos.x >= gridSize.x ||
                finalPos.y < 0 || finalPos.y >= gridSize.y
            )   return null;
            else
                return grid[finalPos.x, finalPos.y];
        }

        public Cell GetCellFromWorldPos(Vector3 worldPos)
        {
            float percentX = worldPos.x / (gridSize.x * cellDiameter);
            float percentY = worldPos.z / (gridSize.y * cellDiameter);
    
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);
    
            int x = Mathf.Clamp(Mathf.FloorToInt((gridSize.x) * percentX), 0, gridSize.x - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt((gridSize.y) * percentY), 0, gridSize.y - 1);
            return grid[x, y];
        }
    }
}
