using Unity.Collections;
using UnityEngine;
using WarOfTanks.MapGen;

namespace WarOfTanks
{   
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public TilemapPerlinGenerator mapGenerator;
        [ReadOnly] public API.Player loggedPlayer;

        private SpawnManager spawnManager;

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

        private void Start()
        {
            TryGetComponent<SpawnManager>(out spawnManager);
            spawnManager?.Init();
        }
    }
}