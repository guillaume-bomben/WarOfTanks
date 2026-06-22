using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WarOfTanks
{
    /// <summary>
    /// Zone de contrôle (Sujet Étape 8). 4 états gérés par une state machine.
    ///
    /// Conçue pour le projet 2D : Collider2D + OnTriggerEnter2D/Exit2D.
    /// Lit tank.teamId (int) et marque les points via GameManager.AddScore(teamId, amount).
    ///
    /// Représentation interne des équipes : enum Team (Red/Blue) pour la state machine.
    /// Mapping vers le score :
    ///   teamId 0 = Équipe A = Team.Red
    ///   teamId 1 = Équipe B = Team.Blue
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ControlZone : MonoBehaviour
    {
        [Header("Capture Settings")]
        [Tooltip("Vitesse de remplissage de la jauge (0..1 par seconde)")]
        public float captureSpeed = 0.1f;

        [Tooltip("Vitesse de décroissance lente quand la zone est laissée vide")]
        public float slowDecaySpeed = 0.05f;

        [Tooltip("Délai (s) avant qu'une zone capturée laissée vide ne décroisse")]
        public float emptyTimeout = 5f;

        [Tooltip("Points par seconde générés quand la zone est capturée et tenue")]
        public float pointsPerSecond = 5f;

        [Header("State Info (Read Only)")]
        [Range(0f, 1f)]
        public float captureProgress = 0f;
        public Team capturingTeam = Team.None;

        [Header("Events (UI / feedback)")]
        // (progress 0..1, équipe) — pour brancher le cercle de capture
        public UnityEvent<float, Team> OnCaptureProgressChanged;
        // (équipe) — quand la zone passe à 100%
        public UnityEvent<Team> OnZoneCaptured;

        // Tanks actuellement dans la zone
        public HashSet<Tank> presentTanks = new HashSet<Tank>();

        // Accumulateur : GameManager.AddScore prend un int, on cumule les fractions.
        private float scoreAccumulator = 0f;

        // ── States ──────────────────────────────────────────────────────
        public IState CurrentState { get; private set; }
        public readonly NeutralState neutralState = new NeutralState();
        public readonly CapturingState capturingState = new CapturingState();
        public readonly CapturedState capturedState = new CapturedState();
        public readonly ConflictState conflictState = new ConflictState();

        private void Start()
        {
            GetComponent<Collider2D>().isTrigger = true;
            SwitchState(neutralState);
        }

        private void Update()
        {
            CurrentState?.Update(this);
            OnCaptureProgressChanged?.Invoke(captureProgress, capturingTeam);
        }

        public void SwitchState(IState newState)
        {
            CurrentState?.Exit(this);
            CurrentState = newState;
            CurrentState.Enter(this);
        }

        // ── Détection 2D ────────────────────────────────────────────────
        private void OnTriggerEnter2D(Collider2D other)
        {
            Tank tank = other.GetComponentInParent<Tank>();
            if (tank != null)
                presentTanks.Add(tank);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Tank tank = other.GetComponentInParent<Tank>();
            if (tank != null)
                presentTanks.Remove(tank);
        }

        // ── Scoring ─────────────────────────────────────────────────────
        public void AddScoreForHoldingTeam(float deltaTime)
        {
            if (capturingTeam == Team.None) return;
            if (GameManager.Instance == null) return;

            scoreAccumulator += pointsPerSecond * deltaTime;

            int whole = Mathf.FloorToInt(scoreAccumulator);
            if (whole <= 0) return;

            scoreAccumulator -= whole;
            GameManager.Instance.AddScore(TeamToId(capturingTeam), whole);
        }

        public static int TeamToId(Team team) => team == Team.Blue ? 1 : 0;
        public static Team IdToTeam(int teamId) => teamId == 1 ? Team.Blue : Team.Red;

        // ── Helpers pour les états ──────────────────────────────────────
        public int GetTeamCount(Team team)
        {
            presentTanks.RemoveWhere(t => t == null);

            int count = 0;
            foreach (var tank in presentTanks)
                if (IdToTeam(tank.teamId) == team)
                    count++;
            return count;
        }

        public bool IsConflict()
            => GetTeamCount(Team.Red) > 0 && GetTeamCount(Team.Blue) > 0;

        public bool IsEmpty()
        {
            presentTanks.RemoveWhere(t => t == null);
            return presentTanks.Count == 0;
        }

        public Team GetDominantTeam()
        {
            if (GetTeamCount(Team.Red) > 0 && GetTeamCount(Team.Blue) == 0) return Team.Red;
            if (GetTeamCount(Team.Blue) > 0 && GetTeamCount(Team.Red) == 0) return Team.Blue;
            return Team.None;
        }
    }
}