namespace Commons {
    public static class Constants {
        public const string ServerURL = "http://localhost"; // Express 서버 URL

        public class UserDataResponse {
            public string Nickname;
            public int ProfileNum;
            public int Coins;
            public int Grade;
            public int RankPoint;
            public int WinCount;
            public int LoseCount;

            public UserDataResponse(string nickname, int profileNum, int coins, int grade, int rankPoint, int winCount,
                                    int loseCount) {
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
            public bool success;
            public string message;
        }

        public class TokenResponse {
            public string message;
            public string accessToken;
            public string refreshToken;
            public TokenResponse(string message, string accessToken, string refreshToken) {
                this.message = message;
                this.accessToken = accessToken;
                this.refreshToken = refreshToken;
            }
        }

    }
}
