using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WarOfTanks
{
    [RequireComponent(typeof(Collider))]
    public class ControlZone : MonoBehaviour
    {
        [Header("Capture Settings")]
        [Tooltip("Speed at which the gauge fills (units per second)")]
        public float captureSpeed = 0.1f; 
        
        [Tooltip("Speed at which the gauge empties when unattended (units per second)")]
        public float slowDecaySpeed = 0.05f;
        
        [Tooltip("Time in seconds before the gauge starts emptying when a captured zone is left unattended")]
        public float emptyTimeout = 5f;

        [Tooltip("Points generated per second when the zone is captured")]
        public float pointsPerSecond = 5f;

        [Header("State Info (Read Only)")]
        [Range(0f, 1f)]
        public float captureProgress = 0f;
        public Team capturingTeam = Team.None;

        [Header("Events")]
        // Events to hook UI or score managers
        public UnityEvent<float, Team> OnCaptureProgressChanged;
        public UnityEvent<Team> OnZoneCaptured;
        
        // Present tanks
        public HashSet<Tank> presentTanks = new HashSet<Tank>();

        // States
        public IState CurrentState { get; private set; }
        
        public readonly NeutralState neutralState = new NeutralState();
        public readonly CapturingState capturingState = new CapturingState();
        public readonly CapturedState capturedState = new CapturedState();
        public readonly ConflictState conflictState = new ConflictState();

        private void Start()
        {
            // Initialize trigger if not already
            GetComponent<Collider>().isTrigger = true;
            
            SwitchState(neutralState);
        }

        private void Update()
        {
            CurrentState?.Update(this);
            
            // Invoke visual update
            OnCaptureProgressChanged?.Invoke(captureProgress, capturingTeam);
        }

        public void SwitchState(IState newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit(this);
            }
            
            CurrentState = newState;
            CurrentState.Enter(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            Tank tank = other.GetComponentInParent<Tank>(); // Handle cases where collider is on a child object
            if (tank != null)
            {
                presentTanks.Add(tank);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Tank tank = other.GetComponentInParent<Tank>();
            if (tank != null)
            {
                presentTanks.Remove(tank);
            }
        }

        // Helper methods for States
        public int GetTeamCount(Team team)
        {
            int count = 0;
            // Clean up potentially destroyed tanks before counting
            presentTanks.RemoveWhere(t => t == null);
            
            foreach (var tank in presentTanks)
            {
                if (tank.team == team)
                    count++;
            }
            return count;
        }

        public bool IsConflict()
        {
            return GetTeamCount(Team.Team1) > 0 && GetTeamCount(Team.Team2) > 0;
        }

        public bool IsEmpty()
        {
            // Clean up
            presentTanks.RemoveWhere(t => t == null);
            return presentTanks.Count == 0;
        }

        public Team GetDominantTeam()
        {
            if (GetTeamCount(Team.Team1) > 0 && GetTeamCount(Team.Team2) == 0) return Team.Team1;
            if (GetTeamCount(Team.Team2) > 0 && GetTeamCount(Team.Team1) == 0) return Team.Team2;
            return Team.None;
        }
    }
}
