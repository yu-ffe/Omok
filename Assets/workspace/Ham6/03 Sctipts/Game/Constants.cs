namespace workspace.Ham6._03_Sctipts.Game
{
    public class Constants
    {
        public enum PlayerType
        {
            None, 
            PlayerA, 
            PlayerB
        }

        public enum StoneType
        {
            Normal,
            Hint,
            Last,
            XMark,
            PositionSelecor
        }

        public enum GameType
        {
            SinglePlayer,
            DualPlayer,
            MultiPlayer,
        }
    }
}