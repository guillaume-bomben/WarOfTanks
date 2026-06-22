namespace WarOfTanks
{
    /// <summary>
    /// Zone en conflit : tanks des 2 équipes présents.
    /// Jauge FIGÉE, AUCUN point marqué (donc aucune modification de captureProgress).
    /// Quand le conflit cesse, on résout selon la valeur de la jauge :
    ///   >= 1 -> capturée ; <= 0 -> neutre ; sinon -> en cours de capture.
    /// </summary>
    public class ConflictState : IState
    {
        public void Enter(ControlZone zone) { }

        public void Update(ControlZone zone)
        {
            if (zone.IsConflict()) return; // toujours en conflit : on ne touche à rien

            if (zone.captureProgress >= 1f)
                zone.SwitchState(zone.capturedState);
            else if (zone.captureProgress <= 0f)
                zone.SwitchState(zone.neutralState);
            else
                zone.SwitchState(zone.capturingState);
        }

        public void Exit(ControlZone zone) { }
    }
}