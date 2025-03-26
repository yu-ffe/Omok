namespace Commons {
    public static class Constants {
        public enum PlayerType
        {
            None, 
            PlayerA, 
            PlayerB,
            PlayerX
        }

        public enum GameType
        {
            SinglePlayer,
            DualPlayer,
            MultiPlayer,
            Record,
        }
        
        //게임 결과 (승패 판정)
        public enum GameResult
        {
            None, // 게임 진행 중
            Win, // 플레이어 승
            Lose, // 플레이어 패
            Draw // 비김
        }
        
        
        public const string ServerURL = "http://localhost"; // Express 서버 URL
    }
}
