using UnityEngine;
using WarOfTanks.Stats;

namespace WarOfTanks
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] Stats_SO stats;
        [SerializeField] new string name;
    }
}
