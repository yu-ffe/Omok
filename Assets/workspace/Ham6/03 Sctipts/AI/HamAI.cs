using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using workspace.Ham6._03_Sctipts.Game;

namespace workspace.Ham6.AI
{
    /// <summary>
    /// HamAI 클래스
    /// - GameLogic의 보드 상태(Constants.PlayerType[,] 형태)를 받아 AI가 착수할 위치를 결정합니다.
    /// - Alpha-Beta Pruning 알고리즘을 사용하여 최적의 수를 찾습니다.
    /// 
    /// 사용법:
    /// 1. HamAI ai = new HamAI(board);
    /// 2. (int row, int col) bestMove = ai.GetBestMove();
    ///    // 최적의 착수 좌표를 반환합니다.
    /// </summary>
    public class HamAI {
        private const int BOARD_SIZE = 15;
        
        private Constants.PlayerType[,] board;

        // 최대 탐색 깊이
        private int maxDepth = 4;

        public HamAI(Constants.PlayerType[,] board) {
            this.board = board;
        }

        /// <summary>
        /// Alpha-Beta Pruning 알고리즘을 재귀적으로 실행하여 평가값을 반환합니다.
        /// </summary>
        public int AlphaBetaPruning(int depth, int alpha, int beta) {
            if (depth == maxDepth) {
                return EvaluateBoard();
            }

            if (depth % 2 == 0) { // AI 턴 (최대화)
                int v = int.MinValue;
                bool pruning = false;

                for (int x = 0; x < BOARD_SIZE; x++) {
                    for (int y = 0; y < BOARD_SIZE; y++) {
                        if (board[x, y] == Constants.PlayerType.None) {
                            if (!HasNeighbor(x, y))
                                continue;

                            board[x, y] = Constants.PlayerType.PlayerB;
                            int temp = AlphaBetaPruning(depth + 1, alpha, beta);
                            v = Mathf.Max(v, temp);
                            board[x, y] = Constants.PlayerType.None;
                            alpha = Mathf.Max(alpha, v);
                            if (beta <= alpha) {
                                pruning = true;
                                break;
                            }
                        }
                    }
                    if (pruning)
                        break;
                }
                return v;
            } else { // 플레이어 턴 (최소화)
                int v = int.MaxValue;
                bool pruning = false;

                for (int x = 0; x < BOARD_SIZE; x++) {
                    for (int y = 0; y < BOARD_SIZE; y++) {
                        // CHANGED: EMPTY -> Constants.PlayerType.None
                        if (board[x, y] == Constants.PlayerType.None) {
                            if (!HasNeighbor(x, y))
                                continue;

                            // CHANGED: PLAYER -> Constants.PlayerType.PlayerA
                            board[x, y] = Constants.PlayerType.PlayerA;
                            int temp = AlphaBetaPruning(depth + 1, alpha, beta);
                            v = Mathf.Min(v, temp);
                            board[x, y] = Constants.PlayerType.None;
                            beta = Mathf.Min(beta, v);
                            if (beta <= alpha) {
                                pruning = true;
                                break;
                            }
                        }
                    }
                    if (pruning)
                        break;
                }
                return v;
            }
        }

        /// <summary>
        /// 현재 보드 상태를 평가하는 함수 (여기서는 간단히 0을 반환)
        /// 실제 평가 로직을 추가해야 합니다.
        /// </summary>
        private int EvaluateBoard()
        {
            int score = 0;

            // 각 연속 돌 개수에 대한 가중치 설정 (임의의 값, 필요에 따라 조정)
            int[] weights = new int[6];
            weights[0] = 0; // 0개는 0점
            weights[1] = 1; // 1개: 기본 점수
            weights[2] = 10; // 2개: 10점
            weights[3] = 100; // 3개: 100점
            weights[4] = 1000; // 4개: 1000점
            weights[5] = 100000; // 5개 이상: 승리 임계값

            // 평가할 방향: 가로, 세로, 대각선(↘), 역대각선(↙)
            int[][] directions = new int[][]
            {
                new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 1, -1 }
            };

            // 보드의 모든 셀 순회 (BOARD_SIZE는 15)
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    Constants.PlayerType cell = board[r, c];
                    // 비어 있는 셀은 건너뜁니다.
                    if (cell == Constants.PlayerType.None) continue;

                    // 4가지 방향에 대해 연속된 돌의 개수 평가
                    foreach (var dir in directions)
                    {
                        // 시작점인지 확인 (이전에 같은 돌이 있다면 이미 계산된 것으로 간주)
                        int prevR = r - dir[0];
                        int prevC = c - dir[1];
                        if (prevR >= 0 && prevR < BOARD_SIZE && prevC >= 0 && prevC < BOARD_SIZE &&
                            board[prevR, prevC] == cell)
                            continue;

                        int count = 1; // 현재 셀 포함
                        int nr = r + dir[0];
                        int nc = c + dir[1];
                        while (nr >= 0 && nr < BOARD_SIZE && nc >= 0 && nc < BOARD_SIZE && board[nr, nc] == cell)
                        {
                            count++;
                            nr += dir[0];
                            nc += dir[1];
                        }

                        // 5개 이상의 연속 돌이 있으면 즉시 승리 판단
                        if (count >= 5)
                        {
                            // AI가 승리하면 매우 큰 양수, 플레이어가 승리하면 매우 큰 음수 반환
                            return (cell == Constants.PlayerType.PlayerB) ? int.MaxValue : int.MinValue;
                        }

                        // 연속 돌 개수에 따른 가중치 점수를 추가합니다.
                        // AI의 돌이면 양수, 플레이어의 돌이면 음수로 계산하여 상대적 우위를 평가합니다.
                        if (cell == Constants.PlayerType.PlayerB)
                            score += weights[count];
                        else // Constants.PlayerType.PlayerA
                            score -= weights[count];
                    }
                }
            }

            return score;
        }
    


        /// <summary>
        /// (x,y) 위치에 주변 돌이 있는지 확인합니다.
        /// 주변 8방향 중 하나라도 돌이 있으면 true를 반환합니다.
        /// </summary>
        private bool HasNeighbor(int x, int y) {
            for (int dx = -1; dx <= 1; dx++) {
                for (int dy = -1; dy <= 1; dy++) {
                    if (dx == 0 && dy == 0)
                        continue;
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < BOARD_SIZE && ny >= 0 && ny < BOARD_SIZE) {
                        // CHANGED: EMPTY -> Constants.PlayerType.None
                        if (board[nx, ny] != Constants.PlayerType.None)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// AI가 착수할 위치를 결정하는 함수입니다.
        /// 모든 빈 칸(주변에 돌이 있는 곳) 중에서 Alpha-Beta Pruning을 통해 평가값이 가장 높은 수를 선택합니다.
        /// </summary>
        public (int, int) GetBestMove() {
            List<(int, int)> validMoves = new List<(int, int)>();
            for (int x = 0; x < BOARD_SIZE; x++) {
                for (int y = 0; y < BOARD_SIZE; y++) {
                    // CHANGED: EMPTY -> Constants.PlayerType.None
                    if (board[x, y] == Constants.PlayerType.None && HasNeighbor(x, y)) {
                        validMoves.Add((x, y));
                    }
                }
            }

            (int bestX, int bestY) = validMoves[0];
            int bestScore = int.MinValue;

            foreach (var move in validMoves) {
                // CHANGED: AI_PLAYER -> Constants.PlayerType.PlayerB
                board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
                int score = AlphaBetaPruning(1, int.MinValue, int.MaxValue);
                board[move.Item1, move.Item2] = Constants.PlayerType.None;

                if (score > bestScore) {
                    bestScore = score;
                    bestX = move.Item1;
                    bestY = move.Item2;
                }
            }

            return (bestX, bestY);
        }
    }
}
