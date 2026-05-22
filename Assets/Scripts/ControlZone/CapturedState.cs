using UnityEngine;

namespace WarOfTanks
{
    public class CapturedState : IState
    {
        private float emptyTimer;

        public void Enter(ControlZone zone)
        {
            zone.captureProgress = 1f;
            emptyTimer = 0f;
            zone.OnZoneCaptured?.Invoke(zone.capturingTeam);
        }

        public void Update(ControlZone zone)
        {
            if (zone.IsConflict())
            {
                zone.SwitchState(zone.conflictState);
                return;
            }

            Team dominantTeam = zone.GetDominantTeam();

            if (dominantTeam == zone.capturingTeam)
            {
                // Reset timeout timer
                emptyTimer = 0f;
                // Marquer des points pour l'équipe (zone.capturingTeam)
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(zone.capturingTeam, zone.pointsPerSecond * Time.deltaTime);
                }
            }
            else if (zone.IsEmpty())
            {
                // No tanks in the zone, wait for timeout
                emptyTimer += Time.deltaTime;
                if (emptyTimer >= zone.emptyTimeout)
                {
                    zone.captureProgress -= zone.slowDecaySpeed * Time.deltaTime;
                }
            }
            else if (dominantTeam != Team.None && dominantTeam != zone.capturingTeam)
            {
                // Enemy team is alone, decrease at normal capture speed. Ignore timeout.
                zone.captureProgress -= zone.captureSpeed * Time.deltaTime;
            }

            // Check if captured state is lost
            if (zone.captureProgress < 1f)
            {
                zone.SwitchState(zone.capturingState);
            }
        }

        public void Exit(ControlZone zone)
        {
            // Leaving captured state
        }
    }
}