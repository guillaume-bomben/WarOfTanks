namespace WarOfTanks
{
    public interface IState
    {
        void Enter(ControlZone zone);
        void Update(ControlZone zone);
        void Exit(ControlZone zone);
    }
}