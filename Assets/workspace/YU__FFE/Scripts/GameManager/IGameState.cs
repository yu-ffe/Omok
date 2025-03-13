namespace workspace.YU__FFE.Scripts.i
{
    public interface IGameState
    {
        void EnterState(GameManager gameManager);
        void UpdateState(GameManager gameManager);
    }
}