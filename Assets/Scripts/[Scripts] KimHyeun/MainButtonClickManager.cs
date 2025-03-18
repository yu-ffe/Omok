using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtonClickManager : Singleton<MainButtonClickManager>
{
    public void OnClick_GameStartButton()
    {
        Debug.Log("게임 시작 버튼 클릭");

        // 게임 실행
    }

    public void OnClick_RecordButton()
    {
        Debug.Log("내 기보 버튼 클릭");

        // 기보 팝업 출력
        // 기보 데이터 로드
    }

    public void OnClick_RankingButton()
    {
        Debug.Log("랭킹 버튼 클릭");

        WB.UI_Manager.Instance.Show(WB.UI_Manager.PanelType.Ranking);
        // 랭킹 팝업 출력
        // 랭킹 데이터 로드
    }

    public void OnClick_ShopButton()
    {
        Debug.Log("상점 버튼 클릭");

        // 상점 팝업 출력
        // 상점 데이터 로드
    }

    public void OnClick_SettingButton()
    {
        Debug.Log("설정 버튼 클릭");

        // 설정 팝업 출력
    }
}
