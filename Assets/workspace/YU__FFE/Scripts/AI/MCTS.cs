using System;
using System.Collections.Generic;
using System.Linq;

namespace workspace.YU__FFE.Scripts.AI {
    /// <summary>
    /// 사용법
    /// 1. 선언: GomokuMCTS ai = new GomokuMCTS(board);
    /// 2. 파라미터 설정 ai.setParams(simulations, exploration); // 시뮬레이션, 가중치
    /// 3. var (x, y) = ai.ai(); // ai의 판단
    /// 4. 플레이어 입력 board[x, y] = 1;
    /// </summary>
    class MCTS {
        private const int BoardSize = 15;
        private int[,] board;
        private int simulations = 1000;
        private double exploration = 1.4;
        private Random random = new Random();

        // 얕은 복사, 안되면 아쉽.. 테스트는 안해봤고 코드 읽어보기만함
        public MCTS(int[,] board) {
            this.board = board;
        }

        public void SetParams(int simulations, double exploration) {
            this.simulations = simulations;
            this.exploration = exploration;
        }

        //ai의 판단, x, y 위치로 반환
        public (int, int) ai() {
            return MctsSearch(-1);
        }

        private List<(int, int)> GetValidMoves() {
            List<(int, int)> validMoves = new List<(int, int)>();
            for (int r = 0; r < BoardSize; r++) {
                for (int c = 0; c < BoardSize; c++) {
                    if (board[r, c] == 0) {
                        validMoves.Add((r, c));
                    }
                }
            }
            return validMoves;
        }

        // 이동 가능 위치(본뜬 보드) 모으고
        /**
         *  1. 이동가능한 모든 위치 확인, 플레이어가 두는걸 예측
         */
        private int SimulateRandomGame(int[,] tempBoard, int player) {
            List<(int, int)> moves = GetValidMoves();
            while (moves.Count > 0) {
                var move = moves[random.Next(moves.Count)];
                tempBoard[move.Item1, move.Item2] = player;
                moves.Remove(move);

                if (CheckWinner(tempBoard, move))
                    return tempBoard[move.Item1, move.Item2];

                player = -player;
            }
            return 0;
        }

        // 승패 파악
        public bool CheckWinner(int[,] tempBoard, (int, int) move) {
            int[][] directions = new int[][] {
                new int[] { 1, 0 }, // 수직
                new int[] { 0, 1 }, // 수평
                new int[] { 1, 1 }, // 대각선 ↘
                new int[] { 1, -1 } // 대각선 ↙
            };

            int x = move.Item1, y = move.Item2;
            int player = tempBoard[x, y];

            foreach (var dir in directions) {
                int count = 1;
                for (int d = -1; d <= 1; d += 2) {
                    int nx = x + dir[0] * d, ny = y + dir[1] * d;
                    while (nx >= 0 && nx < BoardSize && ny >= 0 && ny < BoardSize && tempBoard[nx, ny] == player) {
                        count++;
                        nx += dir[0] * d;
                        ny += dir[1] * d;
                    }
                }
                if (count >= 5) return true;
            }
            return false;
        }

        private double UCT(int wins, int visits, int parentVisits) {
            if (visits == 0) return double.MaxValue;
            return (double)wins / visits + exploration * Math.Sqrt(Math.Log(parentVisits) / visits);
        }

        // MCTS 알고리즘 Search
        /**
         * 1. 이동 가능 위치 전부 가져옴
         * 2. 방문, 승리 횟수 저장
         * 3. simulation 횟수만큼 반복
         * 3-2. 보드 본뜨고
         * 3-3. 시뮬레이션
         */
        private (int, int) MctsSearch(int player) {
            var validMoves = GetValidMoves();
            Dictionary<(int, int), int> visits = validMoves.ToDictionary(m => m, _ => 0);
            Dictionary<(int, int), int> wins = validMoves.ToDictionary(m => m, _ => 0);

            for (int i = 0; i < simulations; i++) {
                var move = validMoves[random.Next(validMoves.Count)];
                int[,] tempBoard = (int[,])board.Clone();
                tempBoard[move.Item1, move.Item2] = player;

                int result = SimulateRandomGame(tempBoard, -player);
                visits[move]++;
                if (result == player)
                    wins[move]++;
            }

            return validMoves.OrderByDescending(m => UCT(wins[m], visits[m], visits.Values.Sum())).First();
        }
    }
}