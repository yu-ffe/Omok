namespace Commons.Models {
    public class PlayerDataResponse {
        public string Nickname;
        public int ProfileNum;
        public int Coins;
        public int Grade;
        public int RankPoint;
        public int WinCount;
        public int LoseCount;

        public PlayerDataResponse(string nickname, int profileNum, int coins, int grade, int rankPoint, int winCount, int loseCount) {
            this.Nickname = nickname;
            this.ProfileNum = profileNum;
            this.Coins = coins;
            this.Grade = grade;
            this.RankPoint = rankPoint;
            this.WinCount = winCount;
            this.LoseCount = loseCount;
        }
    }

    public class CheckResponse {
        public bool Success;
        public string Message;
    }

    public class TokenResponse {
        public string Message;
        public string AccessToken;
        public string RefreshToken;

        public TokenResponse(string message, string accessToken, string refreshToken) {
            this.Message = message;
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
        }
    }

    public class Ranking {
        public string Nickname;
        public int ProfileNum;
        public int Grade;
        public int RankPoint;
        public int WinCount;
        public int LoseCount;
    }
}
