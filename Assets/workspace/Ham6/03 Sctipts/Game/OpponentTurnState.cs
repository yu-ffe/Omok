using UnityEngine;

namespace workspace.Ham6._03_Sctipts.Game
{
    public class OpponentTurnState : IGameState
    {
        public void EnterState(GameManager gameManager)
        {
            Debug.Log("상대 턴");
            gameManager.isMyTurn = false; // 내 턴 비활성화
            gameManager.currentTime = gameManager.timer; // 타이머 초기화
            gameManager.UpdateTimerUI();
        }

        public void UpdateState(GameManager gameManager)
        {
            gameManager.currentTime = Mathf.Max(gameManager.currentTime - Time.deltaTime, 0.0f);
            gameManager.UpdateTimerUI();

            if (gameManager.currentTime <= 0.0f)
            {
                Debug.Log("상대의 시간이 초과");
                // TODO: 승리처리
            }
        }
    }
}