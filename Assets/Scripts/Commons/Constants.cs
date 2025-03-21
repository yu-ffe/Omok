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
        }
        
        public const string ServerURL = "http://localhost"; // Express 서버 URL
    }
}
