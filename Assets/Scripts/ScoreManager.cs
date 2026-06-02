using UnityEngine;
using UnityEngine.Events;

namespace WarOfTanks
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Score Settings")]
        public float scoreToWin = 1000f;

        [Header("Current Scores (Read Only)")]
        public float team1Score = 0f;
        public float team2Score = 0f;

        [Header("Events")]
        public UnityEvent<Team, float> OnScoreChanged;
        public UnityEvent<Team> OnTeamWin;

        private bool gameOver = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddScore(Team team, float points)
        {
            if (gameOver || team == Team.None) return;

            if (team == Team.Red)
            {
                team1Score += points;
                OnScoreChanged?.Invoke(team, team1Score);
                
                if (team1Score >= scoreToWin)
                {
                    TriggerWin(Team.Red);
                }
            }
            else if (team == Team.Blue)
            {
                team2Score += points;
                OnScoreChanged?.Invoke(team, team2Score);

                if (team2Score >= scoreToWin)
                {
                    TriggerWin(Team.Blue);
                }
            }
        }

        private void TriggerWin(Team winningTeam)
        {
            gameOver = true;
            Debug.Log($"<color=green>Game Over! {winningTeam} wins!</color>");
            OnTeamWin?.Invoke(winningTeam);
        }
    }
}
