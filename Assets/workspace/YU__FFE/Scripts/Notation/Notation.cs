

using System.Collections.Generic;

namespace workspace.YU__FFE.Scripts.Notation {
    public class Notation : Singleton<Notation> {
        // 기록 보존은 어떻게? 일단 넣지 말자
        // 로컬에 저장 + (서버에 저장 + 불러오기)
        List<GameRecord> _records = new List<GameRecord>();
        MatchHistory _matchHistory = new MatchHistory();
        
        /**
         * 사용법
         * 1. matchHistory에 record 하나 등록
         */
        
        /// <summary>
        /// 중요 - 게임매니저에 GameRecord 클래스를 추가해서 기록
        /// 대국 시작 종료 후 Notation에 Record 추가
        /// </summary>
        public void AddRecord() {
            _records.Add(new GameRecord());
        }

        public void OnUpdate() {
            // 제작 X -> 서버/로컬에 아직 저장할 필요성은 X
            // 로컬에 저장할떄 데이터 양이나 관리가 어려울것같으므로 일단은 보류
        }
        
    }
}
