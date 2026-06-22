namespace WarOfTanks
{
    /// <summary>
    /// Zone neutre : aucune équipe ne contrôle, jauge vide, aucun point.
    /// - 2 équipes présentes -> conflit
    /// - 1 seule équipe présente -> en cours de capture
    /// </summary>
    public class NeutralState : IState
    {
        public void Enter(ControlZone zone)
        {
            zone.capturingTeam = Team.None;
            zone.captureProgress = 0f;
        }

        public void Update(ControlZone zone)
        {
            if (zone.IsConflict())
            {
                zone.SwitchState(zone.conflictState);
                return;
            }

            Team dominant = zone.GetDominantTeam();
            if (dominant != Team.None)
            {
                zone.capturingTeam = dominant;
                zone.SwitchState(zone.capturingState);
            }
        }

        public void Exit(ControlZone zone) { }
    }
}