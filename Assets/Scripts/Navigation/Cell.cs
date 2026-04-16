using System;
using UnityEngine;

namespace WarOfTanks.Nav
{
    [Serializable]
    public class Cell
    {
        public Vector3 worldPos;
        public Vector2Int gridIndex;
        
        public byte cost;
        public ushort bestCost;

        public GridDirection bestDirection;


        public Cell(Vector3 worldPos, Vector2Int gridIndex)
        {
            this.worldPos = worldPos;
            this.gridIndex = gridIndex;
            this.cost = 1;
            this.bestCost = ushort.MaxValue;
            this.bestDirection = GridDirection.None;
        }

        public void IncreaseCost(int amount)
        {
            if (cost == byte.MaxValue) return;

            if (amount + cost >= byte.MaxValue)
                cost = byte.MaxValue;
            else
                cost += (byte)amount;

            // Debug.Log($"Cell at {gridIndex} bestDir = {bestDirection}");
        }
    }
}