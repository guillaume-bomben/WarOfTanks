using TMPro;
using UnityEngine;

namespace WarOfTanks.UI
{
    /// <summary>
    /// À attacher sur un GameObject "HUD" dans la scène de jeu.
    /// Références à brancher dans l'Inspector.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Score")]
        public TextMeshProUGUI scoreTeamAText;
        public TextMeshProUGUI scoreTeamBText;

        [Header("Chrono")]
        public TextMeshProUGUI timerText;

        [Header("Écran de fin")]
        public GameObject endScreen;
        public TextMeshProUGUI winnerText;
        public UnityEngine.UI.Button restartButton;
        public UnityEngine.UI.Button menuButton;

        void Start()
        {
            // Abonnement aux événements du GameManager
            if (GameManager.Instance == null) return;

            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnTimeChanged  += UpdateTimer;
            GameManager.Instance.OnMatchEnd     += ShowEndScreen;

            // Cache l'écran de fin au départ
            if (endScreen != null)
                endScreen.SetActive(false);

            // Boutons
            restartButton?.onClick.AddListener(GameManager.Instance.RestartMatch);
            menuButton?.onClick.AddListener(GameManager.Instance.LoadMainMenu);
        }

        void OnDestroy()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnTimeChanged  -= UpdateTimer;
            GameManager.Instance.OnMatchEnd     -= ShowEndScreen;
        }

        // ── Callbacks ────────────────────────────────────────────────────

        void UpdateScore(int teamId, int score)
        {
            if (teamId == 0 && scoreTeamAText != null)
                scoreTeamAText.text = $"Équipe A : {score}";
            else if (teamId == 1 && scoreTeamBText != null)
                scoreTeamBText.text = $"Équipe B : {score}";
        }

        void UpdateTimer(float timeRemaining)
        {
            if (timerText == null) return;

            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";

            // Rouge quand < 30 secondes
            timerText.color = timeRemaining < 30f ? Color.red : Color.white;
        }

        void ShowEndScreen(int winnerTeamId)
        {
            if (endScreen == null) return;
            endScreen.SetActive(true);

            if (winnerText != null)
            {
                winnerText.text = winnerTeamId switch
                {
                    0  => "Victoire — Équipe A !",
                    1  => "Victoire — Équipe B !",
                    _  => "Égalité !"
                };
            }
        }
    }
}