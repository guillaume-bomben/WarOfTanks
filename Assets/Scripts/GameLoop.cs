using UnityEngine;

namespace WarOfTanks
{
    public class GameLoop : MonoBehaviour
    {
        public float tickRate = 0.05f; // 20 ticks/s
        float accumulator = 0f;

        public static GameLoop Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            accumulator += Time.deltaTime;
            while (accumulator >= tickRate)
            {
                SimulateTick();
                accumulator -= tickRate;
            }
        }

        void SimulateTick()
        {
            UnitManager.Instance.TickAllUnits();
        }
    }
}
