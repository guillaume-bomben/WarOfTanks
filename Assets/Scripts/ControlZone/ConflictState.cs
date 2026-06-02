using UnityEngine;

namespace WarOfTanks
{
    public class ConflictState : IState
    {
        public void Enter(ControlZone zone)
        {
            // Transitioned into conflict state
        }

        public void Update(ControlZone zone)
        {
            // If the zone is no longer in conflict
            if (!zone.IsConflict())
            {
                // Resolve conflict by switching to the correct state based on the gauge
                if (zone.captureProgress >= 1f)
                {
                    zone.SwitchState(zone.capturedState);
                }
                else if (zone.captureProgress <= 0f)
                {
                    zone.SwitchState(zone.neutralState);
                }
                else
                {
                    zone.SwitchState(zone.capturingState);
                }
            }
            
            // Jauge figée : Aucun code pour modifier zone.captureProgress
            // Aucun point marqué : Aucun code attribuant des points
        }

        public void Exit(ControlZone zone)
        {
            // Leaving conflict state
        }
    }
}