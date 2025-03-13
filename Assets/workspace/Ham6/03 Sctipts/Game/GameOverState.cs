using UnityEngine;

namespace workspace.Ham6._03_Sctipts.Game
{
    public class GameOverState : IGameState
    {
        public void EnterState(GameManager gameManager)
        {
            Debug.Log("게임 종료");
            gameManager.currentTime = 0;
            gameManager.UpdateTimerUI();
        }

        public void UpdateState(GameManager gameManager)
        {
            // 게임이 종료된 상태에서 아무것도 하지 않음.
        }
    }
}