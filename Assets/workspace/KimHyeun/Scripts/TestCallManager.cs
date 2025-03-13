using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimHyeun {
    public class TestCallManager : Singleton<TestCallManager>
    {


        private void Start()
        {
            /// 테스트용 코드 호출 모음
            // SessionManager.AddSession("TestId1", "TestNickName1", 0, 1000, 18, 0, 0, 0);
            // SessionManager.AddSession("TestId2", "TestNickName2", 0, 500, 7, 0, 0, 0);
            // SessionManager.AddSession("TestId3", "TestNickName3", 0, 300, 10, 0, 0, 0);
            // SessionManager.AddSession("TestId4", "TestNickName4", 0, 100, 1, 0, 0, 0);


            SessionManager.currentUserId = SessionManager.GetAllUserIds()[0]; // 임의의 1번 유저를 로그인한 유저로 설정


            // TODO 랭킹 팝업 호출 시 실행 필요
            RankingManager.Instance.GetUserData(); 

            // TODO 상점 팝업 호출 시 실행 필요
            ShopManager.Instance.GetItemData(); 

            // TODO 로그인 시 호출, 상점 코인 구매 시 호출, 게임 종료 후 프로필 정보로 돌아가면 호출 -> 프로필 정보 변경 시 호출
            ProfileManager.Instance.UserInfoShow();

            // TODO 메인 화면 전환 시 호출 (로그인 시, 게임 종료 후)
            ProfileManager.Instance.ButtonInfoShow();


            /// 
        }




    }

}

