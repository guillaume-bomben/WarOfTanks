using System.Collections.Generic;
using UnityEngine;

namespace WarOfTanks
{
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance;
        private List<TankMovement> units = new List<TankMovement>();

        void Awake()
        {
            Instance = this;
        }

        public void Register(TankMovement unit)
        {
            units.Add(unit);
            Debug.Log($"Unit registered: {unit.name}");
        }

        public void TickAllUnits()
        {
            foreach (var unit in units)
            {
                unit.Tick();
            }
        }
    }
}