// 1. 단일 책임 원칙: SRP(Single Responsibility Principle)
// 2. 개방-폐쇄 원칙: OCP(Open-Closed Principle)
// 3. 의존성 주입: DIP(Dependency Inversion Principle)

namespace Commons.Models.Response {
    public struct PlayerDataResponse {
        // 플레이어 닉네임
        public readonly string Nickname;

        // 플레이어 프로필 번호
        public readonly int ProfileNum;

        // 플레이어 코인
        public readonly int Coins;

        // 플레이어 등급
        public readonly int Grade;

        // 랭크 포인트
        public readonly int RankPoint;

        // 승리 횟수
        public readonly int WinCount;

        // 패배 횟수
        public readonly int LoseCount;
        
        /// <summary>
        /// 플레이어 데이터 응답
        /// </summary>
        public PlayerDataResponse(string nickname, int profileNum, int coins, int grade, int rankPoint, int winCount, int loseCount) {
            Nickname = nickname;
            ProfileNum = profileNum;
            Coins = coins;
            Grade = grade;
            RankPoint = rankPoint;
            WinCount = winCount;
            LoseCount = loseCount;
        }
    }
}
