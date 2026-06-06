using UnityEngine;
using UnityEngine.SceneManagement;
using WarOfTanks.MapGen;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Références")]
    public TilemapPerlinGenerator mapGenerator;

    [Header("Match")]
    public float matchDuration = 180f; // 3 minutes

    // Score
    public int ScoreTeamA { get; private set; }
    public int ScoreTeamB { get; private set; }

    // Chrono
    public float TimeRemaining { get; private set; }
    public bool MatchRunning { get; private set; }

    // Événements — la zone et l'UI s'y abonnent
    public event System.Action<int, int> OnScoreChanged;   // (teamId, newScore)
    public event System.Action<float>    OnTimeChanged;
    public event System.Action<int>      OnMatchEnd;        // teamId gagnant (-1 = égalité)

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
            return;
        }
    }

    void Start()
    {
        StartMatch();
    }

    void Update()
    {
        if (!MatchRunning) return;

        TimeRemaining -= Time.deltaTime;
        OnTimeChanged?.Invoke(TimeRemaining);

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            EndMatch();
        }
    }

    // ── API publique ─────────────────────────────────────────────────────

    public void StartMatch()
    {
        ScoreTeamA    = 0;
        ScoreTeamB    = 0;
        TimeRemaining = matchDuration;
        MatchRunning  = true;

        OnScoreChanged?.Invoke(0, 0);
        OnScoreChanged?.Invoke(1, 0);
        OnTimeChanged?.Invoke(TimeRemaining);
    }

    /// <summary>Appelé par la zone de contrôle chaque seconde qu'elle rapporte des points.</summary>
    public void AddScore(int teamId, int amount = 1)
    {
        if (!MatchRunning) return;

        if (teamId == 0)
        {
            ScoreTeamA += amount;
            OnScoreChanged?.Invoke(0, ScoreTeamA);
        }
        else
        {
            ScoreTeamB += amount;
            OnScoreChanged?.Invoke(1, ScoreTeamB);
        }
    }

    public void EndMatch()
    {
        MatchRunning = false;

        int winner;
        if (ScoreTeamA > ScoreTeamB)       winner = 0;
        else if (ScoreTeamB > ScoreTeamA)  winner = 1;
        else                               winner = -1; // égalité

        OnMatchEnd?.Invoke(winner);
    }

    public void LoadMainMenu()
    {
        // Détruit le GameManager persistant pour repartir proprement
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartMatch()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}