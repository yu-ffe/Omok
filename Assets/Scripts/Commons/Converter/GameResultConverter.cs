using Commons.Models.Enums;

namespace Commons.Converter {
    
    public static class GameResultConverter {
        /// <summary>
        /// GameResult → PlayerType 변환 메서드
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static PlayerType ToPlayerType(GameResult result) {
            return result switch {
                GameResult.Player1Win => PlayerType.PlayerA,
                GameResult.Player2Win => PlayerType.PlayerB,
                _ => PlayerType.None
            };
        }
    }
}
