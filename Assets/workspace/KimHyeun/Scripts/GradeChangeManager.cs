using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimHyeun {
    public class GradeChangeManager
    {
        const int rankPointRange = 30;

        public static int GetRankPointRange()
        {
            return rankPointRange;
        }
        

        public static void GradeUpdate(string userId, UserSession userSession, GameResult gameResultType) // 승패 결과를 받아서 유저 세션에 저장 (변경된 급수는 유저 세션에서 접근)
        {
            if (userSession != null)
            {
                switch (gameResultType)
                {
                    case GameResult.Win:

                        int winPoint = 3; // 기본 승리 포인트

                        if (userSession.Grade >= 10) winPoint = 10; // 10급~18급: 10점 증가
                        else if (userSession.Grade >= 5) winPoint = 6; // 5급~9급: 6점 증가
                        else winPoint = 3; // 1급~4급: 3점 증가

                        if (userSession.Grade > 1) // 1급은 승점 제외
                        {
                            userSession.RankPoint += winPoint; // 급수에 따라 승급 포인트 증가

                            if (userSession.RankPoint >= rankPointRange) // 30점 도달 시 승급
                            {
                                userSession.RankPoint = 0;
                                userSession.Grade = Mathf.Clamp(userSession.Grade - 1, 1, 18); // 급수 상승
                            }

                            SessionManager.UpdateSession(userId, userSession.Coins, userSession.Grade, userSession.RankPoint);
                        }

                        break;

                    case GameResult.Lose:

                        userSession.RankPoint -= 10; // 패배 시 승급 포인트 감소

                        if (userSession.RankPoint <= -rankPointRange) // -30점 도달 시 강등
                        {
                            userSession.RankPoint = 0;
                            userSession.Grade = Mathf.Clamp(userSession.Grade + 1, 1, 18); // 급수 감소
                        }

                        SessionManager.UpdateSession(userId, userSession.Coins, userSession.Grade, userSession.RankPoint);
                        
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
