using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimHyeun {
    public class GradeChangeManager
    {
        /*
         승패 반환 함수? - 게임 종료 시?
    - 승급/강등 애니메이션? -> 형태만 구현? 게임종료 처리쪽?
         */


        public static void GradeUpdate(string userId, UserSession userSession, GameResult gameResultType) // 승패 결과를 받아서 유저 세션에 저장 (변경된 급수는 유저 세션에서 접근)
        {
            if (userSession != null)
            {
                switch (gameResultType)
                {
                    case GameResult.Win:
                        Debug.Log($"{userSession.Nickname} 플레이어 승리에 따른 승급 계산 실행");

                        if(userSession.Grade > 1) // 1급 보다 낮은 급수일때만 승급 포인트 조절
                        {
                            userSession.RankPoint = userSession.RankPoint + 1;

                            if (userSession.RankPoint >= 3)
                            {
                                userSession.RankPoint = 0; // 승점 포인트 초기화

                                userSession.Grade = Mathf.Clamp(userSession.Grade - 1, 1, 18); // 급수 상승
                            }

                            SessionManager.UpdateSession(userId, userSession.Coins, userSession.Grade, userSession.RankPoint);
                        }                       

                        break;

                    case GameResult.Lose:
                        Debug.Log($"{userSession.Nickname} 플레이어 패배에 따른 승급 계산 실행");

                        if (userSession.Grade < 18) // 18급 보다 높은 급수일때만 승급 포인트 조절
                        {
                            userSession.RankPoint = userSession.RankPoint - 1;

                            if (userSession.RankPoint <= -3)
                            {
                                userSession.RankPoint = 0; // 승점 포인트 초기화

                                userSession.Grade = Mathf.Clamp(userSession.Grade + 1, 1, 18); // 급수 감소
                            }

                            SessionManager.UpdateSession(userId, userSession.Coins, userSession.Grade, userSession.RankPoint);

                        }
                        break;

                    case GameResult.Draw:
                        Debug.Log($"{userSession.Nickname} 플레이어 무승부에 따른 승급 계산 실행");

                        break;

                    default:
                        Debug.LogError("대국 결과가 올바르지 않습니다.");

                        break;
                }
            }

            else
            {
                Debug.LogError("승급 계산에 필요한 유저 데이터를 받지 못했습니다.");
            }
        }
    }
}
