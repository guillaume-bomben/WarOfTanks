namespace WarOfTanks
{
    /// <summary>
    /// Équipe interne utilisée par la state machine de la zone de contrôle.
    /// Mapping avec le teamId (int) des tanks, via ControlZone :
    ///   teamId 0 = Équipe A = Team.Red
    ///   teamId 1 = Équipe B = Team.Blue
    /// </summary>
    public enum Team
    {
        None,
        Red,
        Blue
    }
}