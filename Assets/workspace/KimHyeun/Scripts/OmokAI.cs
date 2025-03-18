using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using workspace.Ham6._03_Sctipts.Game;

namespace KimHyeun {
    public class OmokAI : MonoBehaviour
    {
        // 미니맥스 알고리즘을 이용해서 승리에 가중치를 두는 방식, 탐색 깊이 적용해서 난이도 조절 예정

        // 현재 보드 상태를 읽어서 (board) None인 곳에 둘수있음, 각 플레이어의 돌 위치에 따라 오목룰에 따라 최적의 좌표값 찾기
        // 띄워져 있는 곳도 체크 필요
        // 공격 or 방어 지향 가능?


        const int BOARD_SIZE = 15;

        Constants.PlayerType[,] board; // -> 배열 차체가 보드.ex) 플레이어A가 제일 좌하단에 두면 board[0, 0] 에 PlayerA 가 들어감
                                       // PlayerType의 값 : None, PlayerA, PlayerB

        public OmokAI(Constants.PlayerType[,] board)
        {
            this.board = board;
        }

        // 최대 탐색 깊이
        int maxDepth = 4;




        public (int, int) GetBestMove()
        {
            int bestScore = int.MinValue;
            (int, int) bestMove = (-1, -1);

            // EvaluateBoard 결과를 이용하여 유망한 10~20개의 후보 위치 선택
            var possibleMoves = GetPotentialMoves();

            foreach (var move in possibleMoves)
            {
                int y = move.Item1;
                int x = move.Item2;
                board[y, x] = Constants.PlayerType.PlayerA;
                int score = Minimax(0, false, int.MinValue, int.MaxValue);
                board[y, x] = Constants.PlayerType.None;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = (x, y);
                }
            }
            return bestMove;
        }

        private List<(int, int)> GetPotentialMoves()
        {
            List<(int, int)> potentialMoves = new List<(int, int)>();
            List<(int, int)> candidates = new List<(int, int)>();

            // 보드 상태를 평가하여 유망한 후보 위치들만 추출
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    if (board[y, x] == Constants.PlayerType.None)
                    {
                        candidates.Add((y, x));
                    }
                }
            }

            // 후보들을 평가하고 높은 점수를 받은 10~20개 후보 선택
            candidates.Sort((a, b) => EvaluatePosition(b.Item2, b.Item1, Constants.PlayerType.PlayerA).CompareTo(EvaluatePosition(a.Item2, a.Item1, Constants.PlayerType.PlayerA)));

            int maxCandidates = Mathf.Min(candidates.Count, 20);
            for (int i = 0; i < maxCandidates; i++)
            {
                potentialMoves.Add(candidates[i]);
            }

            return potentialMoves;
        }

        private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
        {
            if (depth == maxDepth || IsGameOver())
            {
                return EvaluateBoard();
            }

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    for (int x = 0; x < BOARD_SIZE; x++)
                    {
                        if (board[y, x] == Constants.PlayerType.None)
                        {
                            board[y, x] = Constants.PlayerType.PlayerA;
                            int eval = Minimax(depth + 1, false, alpha, beta);
                            board[y, x] = Constants.PlayerType.None;
                            maxEval = Mathf.Max(maxEval, eval);
                            alpha = Mathf.Max(alpha, eval);
                            if (beta <= alpha)
                                return maxEval;
                        }
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    for (int x = 0; x < BOARD_SIZE; x++)
                    {
                        if (board[y, x] == Constants.PlayerType.None)
                        {
                            board[y, x] = Constants.PlayerType.PlayerB;
                            int eval = Minimax(depth + 1, true, alpha, beta);
                            board[y, x] = Constants.PlayerType.None;
                            minEval = Mathf.Min(minEval, eval);
                            beta = Mathf.Min(beta, eval);
                            if (beta <= alpha)
                                return minEval;
                        }
                    }
                }
                return minEval;
            }
        }

        private bool IsGameOver()
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    if (board[y, x] != Constants.PlayerType.None && CheckFiveInRow(x, y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckFiveInRow(int x, int y)
        {
            Constants.PlayerType player = board[y, x];
            if (player == Constants.PlayerType.None) return false;

            int[][] directions = new int[][] {
            new int[] {1, 0}, new int[] {0, 1},
            new int[] {1, 1}, new int[] {1, -1}
        };

            foreach (var dir in directions)
            {
                int count = 1;
                for (int i = 1; i < 5; i++)
                {
                    int nx = x + dir[0] * i;
                    int ny = y + dir[1] * i;
                    if (nx < 0 || ny < 0 || nx >= BOARD_SIZE || ny >= BOARD_SIZE || board[ny, nx] != player)
                        break;
                    count++;
                }
                for (int i = 1; i < 5; i++)
                {
                    int nx = x - dir[0] * i;
                    int ny = y - dir[1] * i;
                    if (nx < 0 || ny < 0 || nx >= BOARD_SIZE || ny >= BOARD_SIZE || board[ny, nx] != player)
                        break;
                    count++;
                }
                if (count >= 5) return true;
            }
            return false;
        }

        private int EvaluateBoard()
        {
            int score = 0;
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    if (board[y, x] == Constants.PlayerType.PlayerA)
                        score += EvaluatePosition(x, y, Constants.PlayerType.PlayerA);
                    else if (board[y, x] == Constants.PlayerType.PlayerB)
                        score -= EvaluatePosition(x, y, Constants.PlayerType.PlayerB);
                }
            }
            return score;
        }

        private int EvaluatePosition(int x, int y, Constants.PlayerType player)
        {
            int score = 0;
            int[][] directions = new int[][] {
            new int[] {1, 0}, new int[] {0, 1}, new int[] {1, 1}, new int[] {1, -1}
        };

            foreach (var dir in directions)
            {
                int count = 1;
                int emptyCount = 0; // 공백을 고려한 평가

                for (int i = 1; i < 5; i++)
                {
                    int nx = x + dir[0] * i;
                    int ny = y + dir[1] * i;
                    if (nx < 0 || ny < 0 || nx >= BOARD_SIZE || ny >= BOARD_SIZE || board[ny, nx] == Constants.PlayerType.None)
                    {
                        emptyCount++;
                        break;
                    }
                    if (board[ny, nx] != player)
                        break;
                    count++;
                }

                if (count == 2) score += 10;
                else if (count == 3) score += 100;
                else if (count == 4) score += 1000;
                else if (count >= 5) score += 10000;
            }
            return score;
        }






        // 이하 보류

        // bool enableForbiddenMoves = false; // 금수 적용 여부 (보류)

        /*
        private bool IsForbiddenMove(int x, int y)
        {
            if (!enableForbiddenMoves) return false;

            return IsDoubleThree(x, y) || IsDoubleFour(x, y) || IsOverFive(x, y);
        }

        private bool IsDoubleThree(int x, int y)
        {
            // 삼삼(33) 체크 로직
            return false;
        }

        private bool IsDoubleFour(int x, int y)
        {
            // 사사(44) 체크 로직
            return false;
        }

        private bool IsOverFive(int x, int y)
        {
            // 6목 이상(장목) 체크 로직
            return false;
        }
        */




    }
}

