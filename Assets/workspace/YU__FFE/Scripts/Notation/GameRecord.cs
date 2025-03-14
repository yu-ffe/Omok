using System;
using System.Collections.Generic;
using UnityEngine;

namespace workspace.YU__FFE.Scripts.Notation {
    public class GameRecord {
        private string _opponentName; // 대전 상대의 nickname
        private List<(int x, int y)> _points = new List<(int, int)>(); // 게임 중 대국 위치 기록
        private int _currentIndex; // 현재 기록 위치
        private GameResultType _result; // 게임 결과

        // 상대 이름을 설정하는 메서드
        public void SetOpponentName(string opponentName) {
            _opponentName = opponentName;
        }

        // 상대 이름을 가져오는 프로퍼티
        public string OpponentName => _opponentName;

        // 점수(위치) 추가 메서드
        public void AddPosition(int x, int y) {
            _points.Add((x, y)); // 위치를 리스트에 추가
        }
        
        public IReadOnlyList<(int x, int y)> GetPositions() {
            return _points.AsReadOnly();
        }

        // 게임 결과 설정 메서드
        public void SetGameResult(GameResultType result) {
            _result = result;
        }

    }

}
