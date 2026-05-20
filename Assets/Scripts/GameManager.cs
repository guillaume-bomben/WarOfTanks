using UnityEngine;
using WarOfTanks.MapGen;

namespace WarOfTanks
{   
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public TilemapPerlinGenerator mapGenerator;
        [HideInInspector] public API.Player player;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}