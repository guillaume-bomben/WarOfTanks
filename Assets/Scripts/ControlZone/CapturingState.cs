using UnityEngine;

namespace WarOfTanks
{
    /// <summary>
    /// Zone en cours de capture : la jauge évolue, AUCUN point n'est marqué.
    /// - équipe qui capture seule -> jauge monte (captureSpeed)
    /// - équipe adverse seule      -> jauge descend (captureSpeed)
    /// - zone vide                 -> jauge descend lentement (slowDecaySpeed)
    /// - 2 équipes                 -> conflit
    /// Transitions : jauge >= 1 -> capturée ; jauge <= 0 -> neutre.
    /// </summary>
    public class CapturingState : IState
    {
        public void Enter(ControlZone zone) { }

        public void Update(ControlZone zone)
        {
            if (zone.IsConflict())
            {
                zone.SwitchState(zone.conflictState);
                return;
            }

            if (zone.IsEmpty())
            {
                zone.captureProgress -= zone.slowDecaySpeed * Time.deltaTime;
            }
            else
            {
                Team dominant = zone.GetDominantTeam();

                if (dominant == zone.capturingTeam)
                    zone.captureProgress += zone.captureSpeed * Time.deltaTime;
                else if (dominant != Team.None)
                    zone.captureProgress -= zone.captureSpeed * Time.deltaTime;
            }

            if (zone.captureProgress >= 1f)
            {
                zone.captureProgress = 1f;
                zone.SwitchState(zone.capturedState);
            }
            else if (zone.captureProgress <= 0f)
            {
                zone.captureProgress = 0f;
                zone.SwitchState(zone.neutralState);
            }
        }

        public void Exit(ControlZone zone) { }
    }
}