using System;
using UnityEngine;

namespace WarOfTanks.MapGen
{
    [Serializable]
    public class HazardLayer
    {
        public TerrainRuleTile ruleTile;
        public float scale = 15f;
        [Range(0f, 1f)] public float threshold = 0.6f;
        public Vector2 offset;
    }
}