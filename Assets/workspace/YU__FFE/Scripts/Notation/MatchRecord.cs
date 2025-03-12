using System.Collections.Generic;

namespace workspace.YU__FFE.Scripts.Notation {
    public class MatchRecord {
        private string _name; // 대전 상대의 nickname
        private bool _stone = false; // true: black, false: white
        private Queue<KeyValuePair<int, int>> _point = new Queue<KeyValuePair<int, int>>();
        public string Name {
            get { return _name; }
        }
    }
}
