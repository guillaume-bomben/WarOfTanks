namespace WarOfTanks
{
    /// <summary>
    /// Interface générique d'un état de la state machine de la zone de contrôle.
    /// (Sujet Étape 8 : "system de state machine générique" réutilisé pour la zone.)
    /// </summary>
    public interface IState
    {
        void Enter(ControlZone zone);
        void Update(ControlZone zone);
        void Exit(ControlZone zone);
    }
}