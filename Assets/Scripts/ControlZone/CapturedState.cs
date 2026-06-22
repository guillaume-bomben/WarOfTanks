using UnityEngine;

namespace WarOfTanks
{
    /// <summary>
    /// Zone capturée (jauge à 100%) : l'équipe marque des points tant qu'elle tient.
    /// - équipe présente   -> marque des points, reset du timeout
    /// - zone vide         -> timeout puis décroissance lente
    /// - équipe adverse seule -> décroissance à vitesse de capture (ignore le timeout)
    /// Dès que la jauge passe sous 100% -> retour en cours de capture.
    /// </summary>
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

            Team dominant = zone.GetDominantTeam();

            if (dominant == zone.capturingTeam)
            {
                emptyTimer = 0f;
                zone.AddScoreForHoldingTeam(Time.deltaTime);
            }
            else if (zone.IsEmpty())
            {
                emptyTimer += Time.deltaTime;
                if (emptyTimer >= zone.emptyTimeout)
                    zone.captureProgress -= zone.slowDecaySpeed * Time.deltaTime;
            }
            else if (dominant != Team.None && dominant != zone.capturingTeam)
            {
                zone.captureProgress -= zone.captureSpeed * Time.deltaTime;
            }

            if (zone.captureProgress < 1f)
                zone.SwitchState(zone.capturingState);
        }

        public void Exit(ControlZone zone) { }
    }
}