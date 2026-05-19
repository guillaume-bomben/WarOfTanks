using UnityEngine;
using WarOfTanks.MapGen;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TilemapPerlinGenerator mapGenerator;
    public Player player;

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