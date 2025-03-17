using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using workspace.Ham6._03_Sctipts.Game;
using Random = System.Random;

namespace workspace.Ham6.AI {
    /// <summary>
    /// 사용법
    /// 1. 선언: GomokuMCTS ai = new GomokuMCTS(board);
    /// 2. 파라미터 설정: ai.SetParams(simulations, exploration); // 시뮬레이션 횟수, 탐색 상수
    /// 3. var (x, y) = ai.ai(); // AI의 판단 좌표 반환
    /// 4. 플레이어 입력: board[x, y] = Constants.PlayerType.PlayerB; // 예시
    /// </summary>
    class MCTS 
    {
        private const int BoardSize = 15;
        private Constants.PlayerType[,] board; 

        private int simulations = 1000;
        private double exploration = 1.4;
        private Random random = new Random();

        public MCTS(Constants.PlayerType[,] board) 
        {
            this.board = board;
        }

        public void SetParams(int simulations, double exploration) 
        {
            this.simulations = simulations;
            this.exploration = exploration;
        }

        // AI의 판단, AI는 플레이어 B로 가정
        public (int, int) ai() 
        {
            return MctsSearch(Constants.PlayerType.PlayerB); 
        }

        // 빈 칸(이동 가능한 위치) 목록 반환, 빈 칸은 Constants.PlayerType.None으로 간주
        private List<(int, int)> GetValidMoves() 
        {
            List<(int, int)> validMoves = new List<(int, int)>();
            
            for (int r = 0; r < BoardSize; r++) 
            {
                for (int c = 0; c < BoardSize; c++) 
                {
                    if (board[r, c] == Constants.PlayerType.None) 
                    {
                        validMoves.Add((r, c));
                    }
                }
            }
            return validMoves;
        }

        // 플레이어 전환 헬퍼 함수
        private Constants.PlayerType TogglePlayer(Constants.PlayerType player) 
        {
            if (player == Constants.PlayerType.PlayerA)
                return Constants.PlayerType.PlayerB;
            else if (player == Constants.PlayerType.PlayerB)
                return Constants.PlayerType.PlayerA;
            else
                return Constants.PlayerType.None; // 예외 상황
        }

        // 임시 보드(tempBoard)에서 랜덤 시뮬레이션 진행하여 승리한 플레이어 반환
        private Constants.PlayerType SimulateRandomGame(Constants.PlayerType[,] tempBoard, Constants.PlayerType player) 
        {
            List<(int, int)> moves = GetValidMovesFromBoard(tempBoard);
            
            while (moves.Count > 0) 
            {
                var move = moves[random.Next(moves.Count)];
                tempBoard[move.Item1, move.Item2] = player;
                moves.Remove(move);

                if (CheckWinner(tempBoard, move))
                    return tempBoard[move.Item1, move.Item2];

                player = TogglePlayer(player); 
            }
            
            return Constants.PlayerType.None;
        }

        // 현재 tempBoard에서 빈 칸만 모으는 헬퍼 함수
        private List<(int, int)> GetValidMovesFromBoard(Constants.PlayerType[,] tempBoard) 
        { 
            List<(int, int)> validMoves = new List<(int, int)>();
            for (int r = 0; r < BoardSize; r++) {
                for (int c = 0; c < BoardSize; c++) {
                    if (tempBoard[r, c] == Constants.PlayerType.None) 
                    {
                        validMoves.Add((r, c));
                    }
                }
            }
            return validMoves;
        }

        // 승리 조건 확인: 마지막 착수 위치(move)를 기준으로 해당 플레이어의 연속 돌 개수가 5 이상이면 승리
        public bool CheckWinner(Constants.PlayerType[,] tempBoard, (int, int) move) 
        {
            int[][] directions = new int[][] 
            {
                new int[] { 1, 0 }, // 수직
                new int[] { 0, 1 }, // 수평
                new int[] { 1, 1 }, // 대각선 ↘
                new int[] { 1, -1 } // 대각선 ↙
            };

            int x = move.Item1, y = move.Item2;
            Constants.PlayerType player = tempBoard[x, y];

            foreach (var dir in directions) 
            {
                int count = 1;
                for (int d = -1; d <= 1; d += 2) 
                {
                    int nx = x + dir[0] * d, ny = y + dir[1] * d;
                    while (nx >= 0 && nx < BoardSize && ny >= 0 && ny < BoardSize && tempBoard[nx, ny] == player) 
                    {
                        count++;
                        nx += dir[0] * d;
                        ny += dir[1] * d;
                    }
                }
                if (count >= 5) return true;
            }
            return false;
        }
        
        public void DebugPrintBoard()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // CHANGED: 행을 BoardSize-1부터 0까지 내림차순으로 출력 (즉, (14,0)부터 시작)
            for (int y = BoardSize - 1; y >= 0; y--)
            {
                // x: 0부터 14까지 (왼쪽에서 오른쪽으로 출력)
                for (int x = 0; x < BoardSize; x++)
                {
                    if (board[x, y] == Constants.PlayerType.PlayerA)
                        sb.Append("A ");
                    else if (board[x, y] == Constants.PlayerType.PlayerB)
                        sb.Append("B ");
                    else
                        sb.Append($"({x},{y}) ");
                }
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }

        private double UCT(int wins, int visits, int parentVisits) 
        {
            if (visits == 0) return double.MaxValue;
            return (double)wins / visits + exploration * Math.Sqrt(Math.Log(parentVisits) / visits);
        }

        // MCTS 알고리즘 Search: 현재 플레이어(player)를 기준으로 가장 좋은 이동 위치 반환
        private (int, int) MctsSearch(Constants.PlayerType player) 
        {
            var validMoves = GetValidMoves();
            Dictionary<(int, int), int> visits = validMoves.ToDictionary(m => m, _ => 0);
            Dictionary<(int, int), int> wins = validMoves.ToDictionary(m => m, _ => 0);

            for (int i = 0; i < simulations; i++) 
            {
                var move = validMoves[random.Next(validMoves.Count)];
                Constants.PlayerType[,] tempBoard = (Constants.PlayerType[,])board.Clone();
                tempBoard[move.Item1, move.Item2] = player;

                Constants.PlayerType result = SimulateRandomGame(tempBoard, TogglePlayer(player));
                visits[move]++;
                if (result == player)
                    wins[move]++;
            }

            // UCT 값을 기준으로 가장 높은 이동 선택
            return validMoves.OrderByDescending(m => UCT(wins[m], visits[m], visits.Values.Sum())).First();
        }
    }
}