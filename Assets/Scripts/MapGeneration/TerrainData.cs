using UnityEngine;

namespace WarOfTanks.MapGen
{
    [System.Serializable]
    public struct TerrainData
    {
        public string name;

        public TerrainRuleTile ruleTile;
        public TerrainDataModifier modifier;
        [Range(0f, 1f)]
        public float height;


        public bool isWalkable;
        public bool isHazard;


        Vector3 worldPos;

        public void SetWorldPos(Vector3 pos)
        {
            worldPos = pos;
        }
    }
}
