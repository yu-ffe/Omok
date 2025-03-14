namespace workspace.YU__FFE.Scripts.Notation {
    public class MatchHistory {
        private GameRecord _gameRecord;
        private int _currentIndex;

        public void SetRecord(GameRecord gameRecord) {
            _gameRecord = gameRecord;
            _currentIndex = 0;
        }

        public (int x, int y) NextPosition() {
            var positions = _gameRecord.GetPositions();
            if (_currentIndex < positions.Count) {
                return positions[_currentIndex++];
            }
            return (0, 0);
        }

        public (int x, int y) CurrentPosition() {
            var positions = _gameRecord.GetPositions();
            if (_currentIndex > 0 && _currentIndex <= positions.Count) {
                return positions[_currentIndex - 1];
            }
            return (0, 0);
        }

        public (int x, int y) PreviousPosition() {
            var positions = _gameRecord.GetPositions();
            if (_currentIndex > 0) {
                return positions[--_currentIndex];
            }
            return (0, 0);
        }
    }
}
