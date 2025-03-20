
namespace WB {
    [System.Serializable]
    public class UserSession {
        public string Nickname;
        public int ProfileNum;
        public int Coins;
        public int Grade;
        public int RankPoint;
        public int WinCount; // 승리 횟수
        public int LoseCount; // 패배 횟수

        // 생성자
        public UserSession(string nickname, int profileNum, int coins, int grade,
                           int rankPoint, int winCount, int loseCount) {
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
