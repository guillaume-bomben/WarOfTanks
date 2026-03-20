using UnityEngine;
using WarOfTanks.MapGen;
using WarOfTanks.Stats;

namespace WarOfTanks
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] public Stats_SO stats;
        public new string name;

        void ApplyTerrainEffect()
        {
            TerrainDataModifier mod = GameManager.Instance.mapGenerator.GetModifierAtWorldPos(transform.position);
            if (mod == null) 
                return;
            
            foreach (var statMod in mod.modifiers)
            {
                // stats.ApplyModifier(statMod);
            }
        }
    }
}
