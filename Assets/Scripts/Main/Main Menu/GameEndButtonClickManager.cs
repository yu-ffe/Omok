using Commons.Patterns;
using Game;
using UnityEngine;

public class GameEndButtonClickManager : Singleton<GameEndButtonClickManager>
{
    
    
    /// <summary>
    /// [확인] 버튼 클릭 → 메인 화면으로 전환
    /// </summary>
    public void OnClick_OkButton()
    {
        Debug.Log("[GameEndButtonClickManager] 확인 버튼 클릭 → 메인 씬으로 이동");

        GameManager.Instance.ChangeToMainScene();
    }

    /// <summary>
    /// [재대국] 버튼 클릭 → 현재 게임 모드에 따라 재시작 처리
    /// </summary>
    public void OnClick_RestartButton()
    {
        Debug.Log("[GameEndButtonClickManager] 재대국 버튼 클릭");

        DORestart();

        // TODO: 멀티플레이 모드일 경우 상대에게 재대국 요청 보내기
    }

    /// <summary>
    /// [기보 저장] 버튼 클릭 → 팝업 표시 및 저장 처리
    /// </summary>
    public void OnClick_RecordButton()
    {
        Debug.Log("[GameEndButtonClickManager] 기보 저장 버튼 클릭");

        UI_Manager.Instance.popup.Show(
            "기보를 저장하시겠습니까?",
            "저장", "취소",
            okAction: () =>
            {
                // TODO: 실제 기보 저장 로직 구현
                Debug.Log("기보 저장 완료 (예정)");

                // 예시:
                // GameRecorder.SaveGameResult(PlayerManager.Instance.playerData);
            },
            cancelAction: () =>
            {
                Debug.Log("기보 저장 취소");
            }
        );
    }
    
    

    /// <summary>
    /// 실제 게임 재시작 로직
    /// </summary>
    public void DORestart()
    {
        Debug.Log("[GameEndButtonClickManager] 게임 재시작 실행");

        GameManager.Instance.RestartCurrentGame();
    }
}