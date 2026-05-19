using System.Collections.Generic;
using UnityEngine;
using WarOfTanks.MapGen;

namespace WarOfTanks.Nav
{
    public class FlowField
    {
        public Cell[,] grid { get; private set; }
        public Vector2Int gridSize { get; private set; }
        public float cellRadius { get; private set; }
        public Cell destinationCell;
        public TilemapPerlinGenerator map;

        public GridDirection.DirectionMode directionMode;

        private float cellDiameter;
        private Vector3 worldOffset;


        public FlowField(float _cellRadius, Vector2Int _gridSize, TilemapPerlinGenerator _map)
        {
            cellRadius = _cellRadius;
            cellDiameter = cellRadius * 2f;
            map = _map;
            gridSize = _map != null ? new Vector2Int(_map.width, _map.height) : _gridSize;
        
            worldOffset = new Vector3(
                -gridSize.x * cellDiameter / 2f, 
                -gridSize.y * cellDiameter / 2f, 
                0
            );
        }

        public void CreateGrid()
        {
            grid = new Cell[gridSize.x, gridSize.y];

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 worldPos = new Vector3(
                        cellDiameter * x + cellRadius, 
                        cellDiameter * y + cellRadius,
                        0 
                    ) + worldOffset;
                    grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
                }
            }
        }

        public void CreateCostField()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 worldPos = new Vector3(
                        cellDiameter * x + cellRadius,
                        cellDiameter * y + cellRadius,
                        0
                    ) + worldOffset;
                    Vector3Int tilePos = map.worldMap.WorldToCell(worldPos);
                    Cell currentCell = grid[x, y];

                    if (map.unwalkableMap.HasTile(tilePos) || map.waterMap.HasTile(tilePos))
                    {
                        currentCell.cost = byte.MaxValue;
                    }
                    else
                    {
                        currentCell.IncreaseCost(1);
                        
                        // Si c'est une hazardTile, on augmente le coût
                        if (map.hazardMap.HasTile(tilePos))
                        {
                            currentCell.IncreaseCost(2);
                            var mods = ((TerrainRuleTile)map.hazardMap.GetTile(tilePos)).modifier.modifiers;
                            foreach (var mod in mods)
                            {
                                if (mod.statType == Stats.StatsEnum.Health && mod.value < 0)
                                    currentCell.IncreaseCost(1);
                                
                                if (mod.statType == Stats.StatsEnum.MoveSpeed && mod.value < 0)
                                    currentCell.IncreaseCost(1);
                            }
                        }
                    }
                }
            }
        }

        public void CreateIntegrationField(Cell destination)
        {
            destinationCell = destination;

            destinationCell.cost = 0;
            destinationCell.bestCost = 0;

            Queue<Cell> cellsToCheck = new Queue<Cell>();

            cellsToCheck.Enqueue(destinationCell);

            while(cellsToCheck.Count > 0)
            {
                Cell currentCell = cellsToCheck.Dequeue();

                GridDirection.Directions gridDirections;
                switch (directionMode)
                {
                    case GridDirection.DirectionMode.CardinalDirections:
                        gridDirections = GridDirection.CardinalDirections;
                        break;
                    
                    case GridDirection.DirectionMode.CardinalAndIntercardinalDirections:
                        gridDirections = GridDirection.CardinalAndIntercardinalDirections;
                        break;
                    
                    case GridDirection.DirectionMode.AllDirections:
                        gridDirections = GridDirection.AllDirections;
                        break;
                    
                    default:
                        gridDirections = GridDirection.AllDirections;
                        break;
                }

                List<Cell> currentNeighbors = GetNeighborCells(currentCell.gridIndex, gridDirections);
                foreach (Cell neighbor in currentNeighbors)
                {
                    if (neighbor.cost == byte.MaxValue) { continue; }
                    if (neighbor.cost + currentCell.bestCost < neighbor.bestCost)
                    {
                        neighbor.bestCost = (ushort)(neighbor.cost + currentCell.bestCost);
                        cellsToCheck.Enqueue(neighbor);
                    }
                }
            }
        }

        public void CreateFlowField()
        {
            foreach(Cell curCell in grid)
            {
                List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.AllDirections);

                int bestCost = curCell.bestCost;

                foreach(Cell curNeighbor in curNeighbors)
                {
                    if(curNeighbor.bestCost < bestCost)
                    {
                        bestCost = curNeighbor.bestCost;
                        curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridIndex - curCell.gridIndex);
                        // Debug.Log($"Setting best direction. Neighbor = {curNeighbor.gridIndex} ; Current = {curCell.gridIndex}\nBest Dir = {curCell.bestDirection}");
                    }
                }
            }
        }

        private List<Cell> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
        {
            List<Cell> neighborCells = new List<Cell>();

            foreach (Vector2Int curDirection in directions)
            {
                Cell newNeighbor = GetCellAtRelativePos(nodeIndex, curDirection);
                if (newNeighbor != null)
                {
                    neighborCells.Add(newNeighbor);
                }
            }
            return neighborCells;
        }

        private Cell GetCellAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
        {
            Vector2Int finalPos = orignPos + relativePos;

            if (finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
            {
                return null;
            }

            else { return grid[finalPos.x, finalPos.y]; }
        }

        public Cell GetCellFromWorldPos(Vector3 worldPos)
        {
            Vector3 localPos = worldPos - worldOffset;

            float percentX = localPos.x / (gridSize.x * cellDiameter);
            float percentY = localPos.y / (gridSize.y * cellDiameter);

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.Clamp(Mathf.FloorToInt(gridSize.x * percentX), 0, gridSize.x - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(gridSize.y * percentY), 0, gridSize.y - 1);
            return grid[x, y];
        }
    }
}