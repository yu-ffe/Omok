using UnityEngine;

namespace workspace.Ham6._03_Sctipts.Game
{
    public class MyTurnState : IGameState
    {
        public void EnterState(GameManager gameManager)
        {
            Debug.Log("내 턴");
            gameManager.isMyTurn = true; // 내 턴 활성화
            gameManager.currentTime = gameManager.timer; // 타이머 초기화
            gameManager.UpdateTimerUI();
        }

        public void UpdateState(GameManager gameManager)
        {
            gameManager.currentTime = Mathf.Max(gameManager.currentTime - Time.deltaTime, 0.0f);
            gameManager.UpdateTimerUI();

            if (gameManager.currentTime <= 0.0f)
            {
                Debug.Log("시간 초과 패배");
                // TODO: 패배처리
            }
        }
    }
}