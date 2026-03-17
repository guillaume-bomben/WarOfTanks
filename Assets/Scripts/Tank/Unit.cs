using UnityEngine;
using WarOfTanks.Stats;

namespace WarOfTanks
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] public Stats_SO stats;
        public new string name;
    }
}
