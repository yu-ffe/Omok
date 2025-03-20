using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Diagnostics;
using workspace.Ham6._03_Sctipts.Game;

namespace KimHyeun {



    public class OmokAI
    {
        private const int BOARD_SIZE = 15;
        private Constants.PlayerType[,] board;
        const int maxDepth = 4;

        // 이동 정렬을 위한 히스토리 테이블
        private int[,] historyTable = new int[BOARD_SIZE, BOARD_SIZE];

        // 트랜스포지션 테이블 최적화 (해시 충돌 감소를 위한 Zobrist 해싱 사용)
        private Dictionary<long, TranspositionEntry> transpositionTable = new Dictionary<long, TranspositionEntry>();
        private long[,,] zobristTable;

        // 방향 배열을 클래스 멤버로 이동하여 반복 생성 방지
        private readonly int[][] directions = new int[][] {
        new int[] { 0, 1 }, new int[] { 1, 0 },
        new int[] { 1, 1 }, new int[] { 1, -1 }
        };

        // 평가 가중치를 상수로 정의
        private const int WIN_SCORE = 2000000;
        private const int FOUR_SCORE = 200000;
        private const int OPEN_THREE_SCORE = 50000;
        private readonly int[] weights = new int[] { 0, 1, 10, 100, 1000, 100000 };

        public OmokAI(Constants.PlayerType[,] board)
        {
            this.board = board;
            InitializeZobristTable();
        }

        // Zobrist 해싱을 위한 테이블 초기화 (NextInt64 대신 Next로 구현)
        private void InitializeZobristTable()
        {
            zobristTable = new long[BOARD_SIZE, BOARD_SIZE, 3]; // 3은 None, PlayerA, PlayerB
            System.Random rand = new System.Random(42); // 고정된 시드로 일관성 유지

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        // 64비트 난수 생성 (NextInt64 대신 Next 메서드 두 번 사용)
                        long r1 = rand.Next();
                        long r2 = rand.Next();
                        zobristTable[i, j, k] = (r1 << 32) | (uint)r2;
                    }
                }
            }
        }

        // 현재 보드 상태의 Zobrist 해시 계산
        private long ComputeZobristHash()
        {
            long hash = 0;
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    if (board[i, j] != Constants.PlayerType.None)
                    {
                        int pieceIndex = board[i, j] == Constants.PlayerType.PlayerA ? 1 : 2;
                        hash ^= zobristTable[i, j, pieceIndex];
                    }
                }
            }
            return hash;
        }

        // 트랜스포지션 테이블 항목
        private struct TranspositionEntry
        {
            public int Score;
            public int Depth;
            public const int EXACT = 0, LOWERBOUND = 1, UPPERBOUND = 2;
            public int Flag;
        }






        // 이기는 수인지 체크 - 훨씬 더 효율적인 구현
        private bool IsWinningMove(int r, int c, Constants.PlayerType player)
        {
            board[r, c] = player;
            bool isWinning = false;

            foreach (var dir in directions)
            {
                int count = 1;

                // 정방향 체크
                int nr = r + dir[0];
                int nc = c + dir[1];
                while (IsValidPosition(nr, nc) && board[nr, nc] == player)
                {
                    count++;
                    nr += dir[0];
                    nc += dir[1];
                }

                // 역방향 체크
                nr = r - dir[0];
                nc = c - dir[1];
                while (IsValidPosition(nr, nc) && board[nr, nc] == player)
                {
                    count++;
                    nr -= dir[0];
                    nc -= dir[1];
                }

                if (count >= 5)
                {
                    isWinning = true;
                    break;
                }
            }

            board[r, c] = Constants.PlayerType.None;
            return isWinning;
        }

        // 현재 보드 상태에서 승패가 결정되었는지 체크
        private int CheckWinningPosition()
        {
            // 가로, 세로, 대각선 5목 체크 (효율성을 위해 통합됨)
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    Constants.PlayerType cell = board[r, c];
                    if (cell == Constants.PlayerType.None) continue;

                    foreach (var dir in directions)
                    {
                        // 연속된 5개의 돌만 체크 (이미 체크된 돌은 건너뜀)
                        if (IsValidPosition(r - dir[0], c - dir[1]) &&
                            board[r - dir[0], c - dir[1]] == cell) continue;

                        int count = 1;
                        int nr = r + dir[0];
                        int nc = c + dir[1];

                        while (IsValidPosition(nr, nc) && board[nr, nc] == cell)
                        {
                            count++;
                            nr += dir[0];
                            nc += dir[1];
                        }

                        if (count >= 5)
                        {
                            return cell == Constants.PlayerType.PlayerB ? WIN_SCORE : -WIN_SCORE;
                        }
                    }
                }
            }

            return 0; // 승패 결정되지 않음
        }

        // 열린 3인지 체크 (양쪽이 열려있는 3목)
        private bool HasOpenThree(int r, int c, Constants.PlayerType player)
        {
            foreach (var dir in directions)
            {
                int count = 1; // 현재 돌 포함
                bool leftOpen = false;
                bool rightOpen = false;

                // 왼쪽 체크
                int nr = r - dir[0];
                int nc = c - dir[1];
                while (IsValidPosition(nr, nc) && board[nr, nc] == player)
                {
                    count++;
                    nr -= dir[0];
                    nc -= dir[1];
                }

                // 왼쪽 끝이 열려있는지 체크
                if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
                {
                    leftOpen = true;
                }

                // 오른쪽 체크
                nr = r + dir[0];
                nc = c + dir[1];
                while (IsValidPosition(nr, nc) && board[nr, nc] == player)
                {
                    count++;
                    nr += dir[0];
                    nc += dir[1];
                }

                // 오른쪽 끝이 열려있는지 체크
                if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
                {
                    rightOpen = true;
                }

                // count가 3이고, 양쪽 끝이 열린 상태에서, 중간에 빈 칸이 있어도 열린 3목을 찾음
                if (count == 3 && leftOpen && rightOpen)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsValidPosition(int r, int c)
        {
            return r >= 0 && r < BOARD_SIZE && c >= 0 && c < BOARD_SIZE;
        }

        // 최적화된 평가 함수
        private int EvaluateBoard()
        {
            int score = 0;
            bool[,] counted = new bool[BOARD_SIZE, BOARD_SIZE];

            // 각 셀에서 시작하는 연속된 돌만 평가 (중복 카운트 방지)
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    if (counted[r, c]) continue;

                    Constants.PlayerType cell = board[r, c];
                    if (cell == Constants.PlayerType.None) continue;

                    foreach (var dir in directions)
                    {
                        // 이미 체크된 방향인지 확인
                        if (IsValidPosition(r - dir[0], c - dir[1]) &&
                            board[r - dir[0], c - dir[1]] == cell) continue;

                        int count = 1;
                        bool leftOpen = false, rightOpen = false;

                        // 왼쪽이 비어있는지 체크
                        if (IsValidPosition(r - dir[0], c - dir[1]) &&
                            board[r - dir[0], c - dir[1]] == Constants.PlayerType.None)
                        {
                            leftOpen = true;
                        }

                        // 연속된 돌 카운트 및 표시
                        int nr = r + dir[0];
                        int nc = c + dir[1];
                        while (IsValidPosition(nr, nc) && board[nr, nc] == cell)
                        {
                            counted[nr, nc] = true;
                            count++;
                            nr += dir[0];
                            nc += dir[1];
                        }

                        // 오른쪽이 비어있는지 체크
                        if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
                        {
                            rightOpen = true;
                        }

                        // 열린 상태에 따라 점수 부여
                        int openFactor = (leftOpen ? 1 : 0) + (rightOpen ? 1 : 0);
                        int weightFactor = weights[count] * (1 + openFactor);

                        if (cell == Constants.PlayerType.PlayerB)
                        {
                            score += weightFactor;
                            if (count == 4)
                            {
                                score += (openFactor > 0) ? FOUR_SCORE * openFactor : FOUR_SCORE / 10;
                            }
                            else if (count == 3 && openFactor == 2)
                            {
                                score += OPEN_THREE_SCORE;
                            }
                        }
                        else
                        {
                            score -= weightFactor;
                            if (count == 4)
                            {
                                score -= (openFactor > 0) ? FOUR_SCORE * openFactor : FOUR_SCORE / 10;
                            }
                            else if (count == 3 && openFactor == 2)
                            {
                                score -= OPEN_THREE_SCORE;
                            }
                        }
                    }
                }
            }

            return score;
        }

        private bool HasNeighbor(int x, int y)
        {
            const int NEIGHBOR_DISTANCE = 2; // 2칸 이내에 돌이 있는지 확인

            for (int dx = -NEIGHBOR_DISTANCE; dx <= NEIGHBOR_DISTANCE; dx++)
            {
                for (int dy = -NEIGHBOR_DISTANCE; dy <= NEIGHBOR_DISTANCE; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    int nx = x + dx;
                    int ny = y + dy;
                    if (IsValidPosition(nx, ny) && board[nx, ny] != Constants.PlayerType.None)
                        return true;
                }
            }
            return false;
        }


        private (int, int) CheckForWinningMove(Constants.PlayerType player)
        {
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    if (board[r, c] != Constants.PlayerType.None) continue;
                    if (!HasNeighbor(r, c)) continue; // 주변에 돌이 없으면 건너뛰기

                    if (IsWinningMove(r, c, player))
                    {
                        return (r, c);
                    }
                }
            }
            return (-1, -1);
        }









        // 3000 노말 (최대 시간 3초)
        public (int, int) GetBestMove(int timeLimit = 3000)
        {
            // Stopwatch를 사용하여 시간 추적
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // 즉시 승리 가능한 수를 먼저 확인
            (int, int) winningMove = CheckForWinningMove(Constants.PlayerType.PlayerB);
            if (winningMove.Item1 != -1)
                return winningMove;

            // 즉시 패배를 막는 수를 확인
            (int, int) blockingMove = CheckForWinningMove(Constants.PlayerType.PlayerA);
            if (blockingMove.Item1 != -1)
                return blockingMove;

            // 트랜스포지션 테이블 초기화
            transpositionTable.Clear();

            // 유효한 이동 가져오기 (히스토리 테이블로 정렬)
            List<(int, int)> validMoves = GetSortedValidMoves(true);

            if (validMoves.Count == 0)
            {
                UnityEngine.Debug.LogError("착수 가능한 위치가 없습니다.");
                return (-1, -1);
            }

            // 최선의 이동 계산
            (int, int) bestMove = validMoves[0];
            int bestScore = int.MinValue;

            foreach (var move in validMoves)
            {
                board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
                int score = AlphaBetaPruningWithTimeLimit(1, int.MinValue, int.MaxValue, false, stopwatch, timeLimit);
                board[move.Item1, move.Item2] = Constants.PlayerType.None;

                // 히스토리 테이블 업데이트
                historyTable[move.Item1, move.Item2] += score > 0 ? score : 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }

                // 시간 체크
                if (stopwatch.ElapsedMilliseconds >= timeLimit)
                {
                    break; // 시간이 초과되면 탐색 종료
                }
            }

            stopwatch.Stop();
            return bestMove;
        }

        private int AlphaBetaPruningWithTimeLimit(int depth, int alpha, int beta, bool isMaximizing, Stopwatch stopwatch, int timeLimit)
        {
            // 시간이 초과하면 즉시 반환
            if (stopwatch.ElapsedMilliseconds >= timeLimit)
            {
                return EvaluateBoard(); // 평가 함수로 즉시 종료
            }

            int winCheck = CheckWinningPosition();
            if (winCheck != 0)
            {
                return winCheck * (maxDepth - depth + 1);
            }

            if (depth >= 4) // 깊이는 고정 4
            {
                return EvaluateBoard();
            }

            long hash = ComputeZobristHash();
            if (transpositionTable.TryGetValue(hash, out TranspositionEntry entry) && entry.Depth >= 4 - depth)
            {
                if (entry.Flag == TranspositionEntry.EXACT)
                    return entry.Score;
                else if (entry.Flag == TranspositionEntry.LOWERBOUND)
                    alpha = Mathf.Max(alpha, entry.Score);
                else if (entry.Flag == TranspositionEntry.UPPERBOUND)
                    beta = Mathf.Min(beta, entry.Score);

                if (alpha >= beta)
                    return entry.Score;
            }

            List<(int, int)> validMoves = GetSortedValidMoves(isMaximizing);

            int bestScore;
            int flag = TranspositionEntry.UPPERBOUND;

            if (isMaximizing)
            {
                bestScore = int.MinValue;

                foreach (var move in validMoves)
                {
                    // 현재 상태 저장 (좌표, 기존 값)
                    var prevState = board[move.Item1, move.Item2];
                    board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
                    int score = AlphaBetaPruningWithTimeLimit(depth + 1, alpha, beta, false, stopwatch, timeLimit);

                    board[move.Item1, move.Item2] = prevState;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        if (depth == 0) historyTable[move.Item1, move.Item2] += 1 << (4 - depth);
                    }

                    alpha = Mathf.Max(alpha, bestScore);
                    if (beta <= alpha)
                    {
                        historyTable[move.Item1, move.Item2] += 1 << (4 - depth + 2);
                        break;
                    }

                    // 시간 체크
                    if (stopwatch.ElapsedMilliseconds >= timeLimit)
                    {
                        break;
                    }
                }
            }
            else
            {
                bestScore = int.MaxValue;

                foreach (var move in validMoves)
                {
                    var prevState = board[move.Item1, move.Item2];
                    board[move.Item1, move.Item2] = Constants.PlayerType.PlayerA;
                    int score = AlphaBetaPruningWithTimeLimit(depth + 1, alpha, beta, true, stopwatch, timeLimit);

                    board[move.Item1, move.Item2] = prevState;

                    if (score < bestScore)
                    {
                        bestScore = score;
                        if (depth == 0) historyTable[move.Item1, move.Item2] += 1 << (4 - depth);
                    }

                    beta = Mathf.Min(beta, bestScore);
                    if (beta <= alpha)
                    {
                        historyTable[move.Item1, move.Item2] += 1 << (4 - depth + 2);
                        break;
                    }

                    // 시간 체크
                    if (stopwatch.ElapsedMilliseconds >= timeLimit)
                    {
                        break;
                    }
                }
            }

            if (bestScore <= alpha)
                flag = TranspositionEntry.UPPERBOUND;
            else if (bestScore >= beta)
                flag = TranspositionEntry.LOWERBOUND;
            else
                flag = TranspositionEntry.EXACT;

            transpositionTable[hash] = new TranspositionEntry
            {
                Score = bestScore,
                Depth = 4 - depth,
                Flag = flag
            };

            return bestScore;
        }

        // 이동을 히스토리 테이블 점수로 정렬 (좋은 이동부터 탐색)
        private List<(int, int)> GetSortedValidMoves(bool isMaximizing)
        {
            List<(int, int, int)> scoredMoves = new List<(int, int, int)>();

            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    if (board[x, y] == Constants.PlayerType.None && HasNeighbor(x, y))
                    {
                        int score = historyTable[x, y];

                        // 간단한 휴리스틱으로 좋은 이동 먼저 평가
                        Constants.PlayerType player = isMaximizing ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA;
                        if (IsWinningMove(x, y, player))
                        {
                            score += 10000000; // 이기는 수에 가장 높은 우선순위
                        }
                        else if (IsWinningMove(x, y, isMaximizing ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB))
                        {
                            score += 5000000;  // 방어 수에 다음 우선순위
                        }
                        else
                        {
                            // 열린 3 체크
                            board[x, y] = player;
                            if (HasOpenThree(x, y, player))
                            {
                                score += 1000000;
                            }
                            board[x, y] = Constants.PlayerType.None;
                        }

                        scoredMoves.Add((x, y, score));
                    }
                }
            }

            // 점수 기준으로 내림차순 정렬
            scoredMoves.Sort((a, b) => b.Item3.CompareTo(a.Item3));


            List<(int, int)> result = new List<(int, int)>(scoredMoves.Count);
            foreach (var move in scoredMoves)
            {
                result.Add((move.Item1, move.Item2));
            }
            return result;
        }

    }
}


