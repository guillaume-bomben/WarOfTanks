using UnityEngine;

namespace WarOfTanks.MapGen
{
    [System.Serializable]
    public struct TerrainData
    {
        public string name;

        public TerrainRuleTile ruleTile;
        [Range(0f, 1f)]
        public float height;

        Vector3 worldPos;

        public void SetWorldPos(Vector3 pos)
        {
            worldPos = pos;
        }
    }
}
