using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WarOfTanks.UI
{
    /// <summary>
    /// À attacher sur un GameObject "MainMenu" dans la scène "MainMenu".
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("Nom de la scène de jeu")]
        public string gameSceneName = "Map_01";

        [Header("Réglages (optionnel)")]
        public UnityEngine.UI.Slider matchDurationSlider;
        public TextMeshProUGUI matchDurationLabel;

        private float matchDuration = 180f;

        void Start()
        {
            if (matchDurationSlider != null)
            {
                matchDurationSlider.minValue = 60f;
                matchDurationSlider.maxValue = 600f;
                matchDurationSlider.value    = matchDuration;
                matchDurationSlider.onValueChanged.AddListener(OnDurationChanged);
                UpdateDurationLabel();
            }
        }

        public void PlayGame()
        {
            // Stocke la durée choisie en PlayerPrefs pour que GameManager la lise
            PlayerPrefs.SetFloat("MatchDuration", matchDuration);
            SceneManager.LoadScene(gameSceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        void OnDurationChanged(float value)
        {
            matchDuration = Mathf.Round(value / 30f) * 30f; // arrondi à 30s
            UpdateDurationLabel();
        }

        void UpdateDurationLabel()
        {
            if (matchDurationLabel == null) return;
            int minutes = Mathf.FloorToInt(matchDuration / 60f);
            int seconds = Mathf.FloorToInt(matchDuration % 60f);
            matchDurationLabel.text = $"Durée : {minutes:00}:{seconds:00}";
        }
    }
}