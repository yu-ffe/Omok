using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndButtonClickManager : Singleton<GameEndButtonClickManager>
{

    // TODO: 각 버튼 기능 추가
    public void OnClick_OkButton()
    {
        Debug.Log("게임 종료 후 확인 버튼 클릭");

        // 메인 프로필 화면으로 돌아가기
        GameManager.Instance.ChangeToMainScene();
    }

    public void OnClick_RestartButton()
    {
        Debug.Log("게임 종료 후 재대국 버튼 클릭");

        // AI, 듀얼 -> 대국 재시작
        // (멀티 -> 재대국 요청)
    }

    public void OnClick_RecordButton()
    {
        Debug.Log("게임 종료 후 기보 저장 버튼 클릭");

        // 기보 저장 여부 팝업 출력

        GameRecorder.SaveGameRecord(); // 기보 저장
    }

    public void DORestart()
    {
        // 게임 재시작 로직
        //GameManager.Instance.RestartCurrentGame();

    }

}