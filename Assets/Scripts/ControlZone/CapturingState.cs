using UnityEngine;

namespace WarOfTanks
{
    public class CapturingState : IState
    {
        public void Enter(ControlZone zone)
        {
            // Transitioned into capturing state
        }

        public void Update(ControlZone zone)
        {
            if (zone.IsConflict())
            {
                zone.SwitchState(zone.conflictState);
                return;
            }

            if (zone.IsEmpty())
            {
                // No tanks = slow decay
                zone.captureProgress -= zone.slowDecaySpeed * Time.deltaTime;
            }
            else
            {
                Team dominantTeam = zone.GetDominantTeam();
                
                if (dominantTeam == zone.capturingTeam)
                {
                    // Same team progressing capture
                    zone.captureProgress += zone.captureSpeed * Time.deltaTime;
                }
                else if (dominantTeam != Team.None)
                {
                    // Enemy team reverting capture (decreases at same capture speed)
                    zone.captureProgress -= zone.captureSpeed * Time.deltaTime;
                }
            }

            // Clamp progress and Check Transitions
            if (zone.captureProgress >= 1f)
            {
                zone.captureProgress = 1f;
                zone.SwitchState(zone.capturedState);
            }
            else if (zone.captureProgress <= 0f)
            {
                zone.captureProgress = 0f;
                // Once depleted, go back to Neutral (it might go to enemy capturing immediately)
                zone.SwitchState(zone.neutralState);
            }
        }

        public void Exit(ControlZone zone)
        {
            
        }
    }
}