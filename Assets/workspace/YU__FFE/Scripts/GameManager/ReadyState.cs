using UnityEngine;

namespace workspace.YU__FFE.Scripts.i {
    public class ReadyState : IGameState
    {
        public void EnterState(GameManager gameManager)
        {
            Debug.Log("게임 준비 상태");
            gameManager.currentTime = gameManager.timer;
            gameManager.UpdateTimerUI();
        }

        public void UpdateState(GameManager gameManager)
        {
            // 대기 중 (사용자가 버튼을 눌러 게임을 시작해야 함)
        }
    }
}
