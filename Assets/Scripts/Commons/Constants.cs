using Commons.Models;

namespace Commons {
    public static class Constants {
        
        public const string ServerURL = "http://localhost"; // Express 서버 URL
        
        /// <summary>
        /// PlayerType → GameResult 변환 메서드
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static GameEnums.GameResult GetWinResultFromPlayerType(GameEnums.PlayerType player)
        {
            return player switch
            {
                GameEnums.PlayerType.PlayerA => GameEnums.GameResult.Player1Win,
                GameEnums.PlayerType.PlayerB => GameEnums.GameResult.Player2Win,
                _ => GameEnums.GameResult.None
            };
        }
    
        /// <summary>
        /// GameResult → PlayerType 변환 메서드
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static GameEnums.PlayerType GetWinnerPlayerFromGameResult(GameEnums.GameResult result)
        {
            return result switch
            {
                GameEnums.GameResult.Player1Win => GameEnums.PlayerType.PlayerA,
                GameEnums.GameResult.Player2Win => GameEnums.PlayerType.PlayerB,
                _ => GameEnums.PlayerType.None
            };
        }
    }
}
