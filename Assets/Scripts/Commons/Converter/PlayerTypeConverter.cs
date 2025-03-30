using Commons.Models.Enums;

namespace Commons.Converter {
    public static class PlayerTypeConverter {
        /// <summary>
        /// PlayerType → GameResult 변환 메서드
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static GameResult ToGameResult(PlayerType player) {
            return player switch {
                PlayerType.PlayerA => GameResult.Player1Win,
                PlayerType.PlayerB => GameResult.Player2Win,
                _ => GameResult.None
            };
        }
    }
}
