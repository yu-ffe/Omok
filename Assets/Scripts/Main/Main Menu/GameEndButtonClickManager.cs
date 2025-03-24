using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndButtonClickManager : Singleton<GameEndButtonClickManager>
{

    /// <summary>
    /// 확인 버튼 클릭 -> 메인 화면으로 이동
    /// </summary>
    public void OnClick_OkButton()
    {
        // 메인 프로필 화면으로 돌아가기
        GameManager.Instance.ChangeToMainScene();
    }

    /// <summary>
    /// 제대국 버튼 클릭 -> 모드에 따라 재경기 로직 분기
    /// </summary>
    public void OnClick_RestartButton()
    {
        Debug.Log("게임 종료 후 재대국 버튼 클릭");
        
        DORestart();

        // AI, 듀얼 -> 대국 재시작
        // TODO: (멀티 -> 재대국 요청)
    }

    /// <summary>
    /// 기보 저장 버튼 클릭 → 기보 저장 팝업 출력
    /// </summary>
    public void OnClick_RecordButton()
    {
        Debug.Log("게임 종료 후 [기보 저장] 버튼 클릭");

        UI_Manager.Instance.popup.Show(
            "기보를 저장하시겠습니까?",
            "저장", "취소",
            okAction: () =>
            {
                // TODO: 실제 기보 저장 로직 구현
                Debug.Log("기보 저장 완료 (예정)");
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
        Debug.Log("게임 재시작 실행");

        GameManager.Instance.RestartCurrentGame(); // 현재 모드로 재시작
    }

}