using Commons.Models;
using Commons.Models.Enums;

namespace Commons {
    public static class Constants {
        
        public const string ServerURL = "http://localhost"; // Express 서버 URL
        
        /// <summary>
        /// PlayerType → GameResult 변환 메서드
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static GameResult GetWinResultFromPlayerType(PlayerType player)
        {
            return player switch
            {
                PlayerType.PlayerA => GameResult.Player1Win,
                PlayerType.PlayerB => GameResult.Player2Win,
                _ => GameResult.None
            };
        }
    
        /// <summary>
        /// GameResult → PlayerType 변환 메서드
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static PlayerType GetWinnerPlayerFromGameResult(GameResult result)
        {
            return result switch
            {
                GameResult.Player1Win => PlayerType.PlayerA,
                GameResult.Player2Win => PlayerType.PlayerB,
                _ => PlayerType.None
            };
        }
    }
}
