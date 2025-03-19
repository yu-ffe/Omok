namespace workspace.YU__FFE.Scripts.User {
    [System.Serializable]
    //UserSession -> UserSessionData
    public class PlayerData {
        /// <summary>
        /// id, password는 저장 X
        /// </summary>
        public string id;
        public string nickname;
        public string password;
        public int profileNum;
        public int coins;
        public int grade;
        public int rankPoint;
        public int winCount; // 승리 횟수
        public int loseCount; // 패배 횟수

        public PlayerData() { }
        
        // 생성자
        public PlayerData(string id, string nickname, string password, int profileNum, int coins, int grade,
                          int rankPoint, int winCount, int loseCount) {
            this.id = id;
            this.nickname = nickname;
            this.password = password;
            this.profileNum = profileNum;
            this.coins = coins;
            this.grade = grade;
            this.rankPoint = rankPoint;
            this.winCount = winCount;
            this.loseCount = loseCount;
        }
        public PlayerData(string nickname, int profileNum, int coins, int grade,
                          int rankPoint, int winCount, int loseCount) {
            this.nickname = nickname;
            this.profileNum = profileNum;
            this.coins = coins;
            this.grade = grade;
            this.rankPoint = rankPoint;
            this.winCount = winCount;
            this.loseCount = loseCount;
        }

        public void SetPrivateData(string id, string password) {
            this.id = id;
            this.password = password;
        }
        
        public void ClearPrivateData() {
            this.id = null;
            this.password = null;
        }

        public void SetPrivateData(string id, string nickname, string password, int profile) {
            this.id = id;
            this.nickname = nickname;
            this.password = password;
            this.profileNum = profile;
        }
        
    }

}
