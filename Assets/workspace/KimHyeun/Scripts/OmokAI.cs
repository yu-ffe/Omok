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


        bool enableForbiddenMoves = true; // 금수 적용 여부


        public (int, int) GetBestMove() // 호출 시 현재 보드값에 따라 알고리즘을 작동시켜 최종적으로 계산된 위치 좌표 리턴
        {
            int bestScore = int.MinValue;
            (int, int) bestMove = (-1, -1);

            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    if (board[y, x] == Constants.PlayerType.None && (!enableForbiddenMoves || !IsForbiddenMove(x, y)))
                    {
                        board[y, x] = Constants.PlayerType.PlayerA;
                        int score = Minimax(0, false, int.MinValue, int.MaxValue);
                        board[y, x] = Constants.PlayerType.None;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = (x, y);
                        }
                    }
                }
            }
            return bestMove;
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
                        if (board[y, x] == Constants.PlayerType.None && (!enableForbiddenMoves || !IsForbiddenMove(x, y)))
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
                        if (board[y, x] == Constants.PlayerType.None && (!enableForbiddenMoves || !IsForbiddenMove(x, y)))
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
            // TODO: 오목 승리 조건 체크 함수
            return false;
        }

        private int EvaluateBoard()
        {
            // TODO: 휴리스틱 평가 함수 구현 (공격, 방어 패턴 분석)
            return 0;
        }

        private bool IsForbiddenMove(int x, int y)
        {
            // TODO: 금수(33, 44, 장목) 체크 함수
            return false;
        }
    }
}
