using System;
using System.Collections.Generic;
using UnityEngine;

namespace workspace.YU__FFE.Scripts.Notation {
    public class GameRecord {
        private string _opponentName; // 대전 상대의 nickname
        private List<(int x, int y)> _points = new List<(int, int)>(); // 게임 중 발생한 점수 기록
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

        // 게임 기록 전체 반환 메서드
        public IReadOnlyList<(int x, int y)> GetPositions() {
            return _points.AsReadOnly(); // 읽기 전용 리스트로 반환
        }

        // 마지막 기록을 반환하는 메서드
        public (int x, int y) NextPosition() {
            if (_points.Count > 0) {
                return _points[_currentIndex++]; // 가장 마지막 기록 반환
            }
            return (0, 0); // 기록이 없으면 (0, 0) 반환
        }
        
        public (int x, int y) CurrentPosition() {
            if (_currentIndex > 0) {
                return _points[_currentIndex - 1]; // 현재 기록 반환
            }
            return (0, 0); // 기록이 없으면 (0, 0) 반환
        }
        
        public (int x, int y) PreviousPosition() {
            if (_currentIndex > 0) {
                return _points[--_currentIndex]; // 이전 기록 반환
            }
            return (0, 0); // 기록이 없으면 (0, 0) 반환
        }

        // 게임 결과 설정 메서드
        public void SetGameResult(GameResultType result) {
            _result = result;
        }

        // 게임 결과 가져오기
        public GameResultType GetGameResult() {
            return _result;
        }

    }

}
