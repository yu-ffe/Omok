namespace Commons.Models {
    public class PlayerDataResponse {
        public readonly string Nickname;
        public readonly int ProfileNum;
        public readonly int Coins;
        public readonly int Grade;
        public readonly int RankPoint;
        public readonly int WinCount;
        public readonly int LoseCount;

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
        public readonly bool Success;
        public readonly string Message;
        public CheckResponse(bool success, string message) {
            Success = success;
            Message = message;
        }
    }

    public class TokenResponse {
        public readonly string Message;
        public readonly string AccessToken;
        public readonly string RefreshToken;

        public TokenResponse(string message, string accessToken, string refreshToken) {
            this.Message = message;
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
        }
    }

    public class Ranking {
        public readonly string Nickname;
        public int ProfileNum;
        public readonly int Grade;
        public int RankPoint;
        public readonly int WinCount;
        public readonly int LoseCount;
        public Ranking(string nickname, int grade, int winCount, int loseCount) {
            Nickname = nickname;
            Grade = grade;
            WinCount = winCount;
            LoseCount = loseCount;
        }
    }
}
