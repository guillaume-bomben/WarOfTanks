using UnityEngine;

namespace WarOfTanks.MapGen
{
    [System.Serializable]
    public struct TerrainType
    {
        public string name;

        public TerrainRuleTile ruleTile;
        public TerrainDataModifier modifier;
        [Range(0f, 1f)]
        public float height;


        public bool isWalkable;
        public bool isHazard;

    }
}
