using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimHyeun {
    public class MainButtonClickManager : Singleton<MainButtonClickManager>
    {
        public void OnClick_GameStartButton()
        {
            Debug.Log("게임 시작 버튼 클릭");
        }

        public void OnClick_RecordButton()
        {
            Debug.Log("내 기보 버튼 클릭");
        }

        public void OnClick_RankingButton()
        {
            Debug.Log("랭킹 버튼 클릭");
        }

        public void OnClick_ShopButton()
        {
            Debug.Log("상점 버튼 클릭");
        }

        public void OnClick_SettingButton()
        {
            Debug.Log("설정 버튼 클릭");
        }
    }
}

