using UnityEngine;
using UnityEngine.Tilemaps;

namespace WarOfTanks.MapGen
{
    [System.Serializable]
    public struct TerrainType
    {
        public string name;

        [Range(0f, 1f)]
        public float height;

        public TileBase tile;

        public bool isWalkable;
        public bool isHazard;

        public TerrainDataModifier modifier;
    }
}
