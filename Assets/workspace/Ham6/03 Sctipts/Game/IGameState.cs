namespace workspace.Ham6._03_Sctipts.Game
{
    public interface IGameState
    {
        void EnterState(GameManager gameManager);
        void UpdateState(GameManager gameManager);
    }
}