using UnityEngine;

namespace WarOfTanks
{
    public class NeutralState : IState
    {
        public void Enter(ControlZone zone)
        {
            // Zone was neutralized
            zone.capturingTeam = Team.None;
            zone.captureProgress = 0f;
        }

        public void Update(ControlZone zone)
        {
            // Check for conflict first
            if (zone.IsConflict())
            {
                zone.SwitchState(zone.conflictState);
                return;
            }

            // Check if one team is capturing
            Team dominantTeam = zone.GetDominantTeam();
            if (dominantTeam != Team.None)
            {
                zone.capturingTeam = dominantTeam;
                zone.SwitchState(zone.capturingState);
            }
        }

        public void Exit(ControlZone zone)
        {
            // Transitioning out of neutral
        }
    }
}