using System.Collections.Generic;
using UnityEngine;

namespace WarOfTanks.MapGen
{
    [CreateAssetMenu(fileName = "New TerrainDataModifier", menuName = "War Of Tanks/Create new Terrain Data Modifier")]
    public class TerrainDataModifier : ScriptableObject
    {
        public StatMod[] modifiers;
    }

    [System.Serializable]
    public struct StatMod
    {
        public Stats.StatsEnum statType;
        public Stats.StatsModifier modifier;
        public float value;
    }
}