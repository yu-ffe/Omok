using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimHyeun {
    public class GradeChangeManager : Singleton<GradeChangeManager>
    {
        /*
         승패 반환 함수? - 게임 종료 시?
    승급 시스템 구현 - 임의의 승 패, 무승부면 승패 없음?
    - 포인트 시스템 (승리+1, 패배-1) -> 로그인 유저 포인트 변동
    - 급수 이동 (18급~1급) -> 급수 변동
    - 승급/강등 애니메이션? -> 형태만 구현? 게임종료 처리쪽?
         */


        public void GradeUpdate(UserSession userSession, GameResult gameResultType)
        {

        }
    }
}
