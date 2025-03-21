using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackUp : MonoBehaviour
{
    #region AI 백업

    // 최신
    //public class OmokAI
    //{
    //    private const int BOARD_SIZE = 15;
    //    private Constants.PlayerType[,] board;
    //    const int maxDepth = 4;

    //    // 이동 정렬을 위한 히스토리 테이블
    //    private int[,] historyTable = new int[BOARD_SIZE, BOARD_SIZE];

    //    // 트랜스포지션 테이블 최적화 (해시 충돌 감소를 위한 Zobrist 해싱 사용)
    //    private Dictionary<long, TranspositionEntry> transpositionTable = new Dictionary<long, TranspositionEntry>();
    //    private long[,,] zobristTable;

    //    // 방향 배열을 클래스 멤버로 이동하여 반복 생성 방지
    //    private readonly int[][] directions = new int[][] {
    //    new int[] { 0, 1 }, new int[] { 1, 0 },
    //    new int[] { 1, 1 }, new int[] { 1, -1 }
    //    };

    //    // 평가 가중치를 상수로 정의
    //    private const int WIN_SCORE = 2000000;
    //    private const int FOUR_SCORE = 200000;
    //    private const int OPEN_THREE_SCORE = 50000;
    //    private readonly int[] weights = new int[] { 0, 1, 10, 100, 1000, 100000 };

    //    public OmokAI(Constants.PlayerType[,] board)
    //    {
    //        this.board = board;
    //        InitializeZobristTable();
    //    }

    //    // Zobrist 해싱을 위한 테이블 초기화 (NextInt64 대신 Next로 구현)
    //    private void InitializeZobristTable()
    //    {
    //        zobristTable = new long[BOARD_SIZE, BOARD_SIZE, 3]; // 3은 None, PlayerA, PlayerB
    //        System.Random rand = new System.Random(42); // 고정된 시드로 일관성 유지

    //        for (int i = 0; i < BOARD_SIZE; i++)
    //        {
    //            for (int j = 0; j < BOARD_SIZE; j++)
    //            {
    //                for (int k = 0; k < 3; k++)
    //                {
    //                    // 64비트 난수 생성 (NextInt64 대신 Next 메서드 두 번 사용)
    //                    long r1 = rand.Next();
    //                    long r2 = rand.Next();
    //                    zobristTable[i, j, k] = (r1 << 32) | (uint)r2;
    //                }
    //            }
    //        }
    //    }

    //    // 현재 보드 상태의 Zobrist 해시 계산
    //    private long ComputeZobristHash()
    //    {
    //        long hash = 0;
    //        for (int i = 0; i < BOARD_SIZE; i++)
    //        {
    //            for (int j = 0; j < BOARD_SIZE; j++)
    //            {
    //                if (board[i, j] != Constants.PlayerType.None)
    //                {
    //                    int pieceIndex = board[i, j] == Constants.PlayerType.PlayerA ? 1 : 2;
    //                    hash ^= zobristTable[i, j, pieceIndex];
    //                }
    //            }
    //        }
    //        return hash;
    //    }

    //    // 트랜스포지션 테이블 항목
    //    private struct TranspositionEntry
    //    {
    //        public int Score;
    //        public int Depth;
    //        public const int EXACT = 0, LOWERBOUND = 1, UPPERBOUND = 2;
    //        public int Flag;
    //    }






    //    // 이기는 수인지 체크 - 훨씬 더 효율적인 구현
    //    private bool IsWinningMove(int r, int c, Constants.PlayerType player)
    //    {
    //        board[r, c] = player;
    //        bool isWinning = false;

    //        foreach (var dir in directions)
    //        {
    //            int count = 1;

    //            // 정방향 체크
    //            int nr = r + dir[0];
    //            int nc = c + dir[1];
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == player)
    //            {
    //                count++;
    //                nr += dir[0];
    //                nc += dir[1];
    //            }

    //            // 역방향 체크
    //            nr = r - dir[0];
    //            nc = c - dir[1];
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == player)
    //            {
    //                count++;
    //                nr -= dir[0];
    //                nc -= dir[1];
    //            }

    //            if (count >= 5)
    //            {
    //                isWinning = true;
    //                break;
    //            }
    //        }

    //        board[r, c] = Constants.PlayerType.None;
    //        return isWinning;
    //    }

    //    // 현재 보드 상태에서 승패가 결정되었는지 체크
    //    private int CheckWinningPosition()
    //    {
    //        // 가로, 세로, 대각선 5목 체크 (효율성을 위해 통합됨)
    //        for (int r = 0; r < BOARD_SIZE; r++)
    //        {
    //            for (int c = 0; c < BOARD_SIZE; c++)
    //            {
    //                Constants.PlayerType cell = board[r, c];
    //                if (cell == Constants.PlayerType.None) continue;

    //                foreach (var dir in directions)
    //                {
    //                    // 연속된 5개의 돌만 체크 (이미 체크된 돌은 건너뜀)
    //                    if (IsValidPosition(r - dir[0], c - dir[1]) &&
    //                        board[r - dir[0], c - dir[1]] == cell) continue;

    //                    int count = 1;
    //                    int nr = r + dir[0];
    //                    int nc = c + dir[1];

    //                    while (IsValidPosition(nr, nc) && board[nr, nc] == cell)
    //                    {
    //                        count++;
    //                        nr += dir[0];
    //                        nc += dir[1];
    //                    }

    //                    if (count >= 5)
    //                    {
    //                        return cell == Constants.PlayerType.PlayerB ? WIN_SCORE : -WIN_SCORE;
    //                    }
    //                }
    //            }
    //        }

    //        return 0; // 승패 결정되지 않음
    //    }

    //    // 열린 3인지 체크 (양쪽이 열려있는 3목)
    //    private bool HasOpenThree(int r, int c, Constants.PlayerType player)
    //    {
    //        foreach (var dir in directions)
    //        {
    //            int count = 1; // 현재 돌 포함
    //            bool leftOpen = false;
    //            bool rightOpen = false;

    //            // 왼쪽 체크
    //            int nr = r - dir[0];
    //            int nc = c - dir[1];
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == player)
    //            {
    //                count++;
    //                nr -= dir[0];
    //                nc -= dir[1];
    //            }

    //            // 왼쪽 끝이 열려있는지 체크
    //            if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
    //            {
    //                leftOpen = true;
    //            }

    //            // 오른쪽 체크
    //            nr = r + dir[0];
    //            nc = c + dir[1];
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == player)
    //            {
    //                count++;
    //                nr += dir[0];
    //                nc += dir[1];
    //            }

    //            // 오른쪽 끝이 열려있는지 체크
    //            if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
    //            {
    //                rightOpen = true;
    //            }

    //            // count가 3이고, 양쪽 끝이 열린 상태에서, 중간에 빈 칸이 있어도 열린 3목을 찾음
    //            if (count == 3 && leftOpen && rightOpen)
    //            {
    //                return true;
    //            }
    //        }

    //        return false;
    //    }

    //    private bool IsValidPosition(int r, int c)
    //    {
    //        return r >= 0 && r < BOARD_SIZE && c >= 0 && c < BOARD_SIZE;
    //    }

    //    // 최적화된 평가 함수
    //    private int EvaluateBoard()
    //    {
    //        int score = 0;
    //        bool[,] counted = new bool[BOARD_SIZE, BOARD_SIZE];

    //        // 각 셀에서 시작하는 연속된 돌만 평가 (중복 카운트 방지)
    //        for (int r = 0; r < BOARD_SIZE; r++)
    //        {
    //            for (int c = 0; c < BOARD_SIZE; c++)
    //            {
    //                if (counted[r, c]) continue;

    //                Constants.PlayerType cell = board[r, c];
    //                if (cell == Constants.PlayerType.None) continue;

    //                foreach (var dir in directions)
    //                {
    //                    // 이미 체크된 방향인지 확인
    //                    if (IsValidPosition(r - dir[0], c - dir[1]) &&
    //                        board[r - dir[0], c - dir[1]] == cell) continue;

    //                    int count = 1;
    //                    bool leftOpen = false, rightOpen = false;

    //                    // 왼쪽이 비어있는지 체크
    //                    if (IsValidPosition(r - dir[0], c - dir[1]) &&
    //                        board[r - dir[0], c - dir[1]] == Constants.PlayerType.None)
    //                    {
    //                        leftOpen = true;
    //                    }

    //                    // 연속된 돌 카운트 및 표시
    //                    int nr = r + dir[0];
    //                    int nc = c + dir[1];
    //                    while (IsValidPosition(nr, nc) && board[nr, nc] == cell)
    //                    {
    //                        counted[nr, nc] = true;
    //                        count++;
    //                        nr += dir[0];
    //                        nc += dir[1];
    //                    }

    //                    // 오른쪽이 비어있는지 체크
    //                    if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
    //                    {
    //                        rightOpen = true;
    //                    }

    //                    // 열린 상태에 따라 점수 부여
    //                    int openFactor = (leftOpen ? 1 : 0) + (rightOpen ? 1 : 0);
    //                    int weightFactor = weights[count] * (1 + openFactor);

    //                    if (cell == Constants.PlayerType.PlayerB)
    //                    {
    //                        score += weightFactor;
    //                        if (count == 4)
    //                        {
    //                            score += (openFactor > 0) ? FOUR_SCORE * openFactor : FOUR_SCORE / 10;
    //                        }
    //                        else if (count == 3 && openFactor == 2)
    //                        {
    //                            score += OPEN_THREE_SCORE;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        score -= weightFactor;
    //                        if (count == 4)
    //                        {
    //                            score -= (openFactor > 0) ? FOUR_SCORE * openFactor : FOUR_SCORE / 10;
    //                        }
    //                        else if (count == 3 && openFactor == 2)
    //                        {
    //                            score -= OPEN_THREE_SCORE;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        return score;
    //    }

    //    private bool HasNeighbor(int x, int y)
    //    {
    //        const int NEIGHBOR_DISTANCE = 2; // 2칸 이내에 돌이 있는지 확인

    //        for (int dx = -NEIGHBOR_DISTANCE; dx <= NEIGHBOR_DISTANCE; dx++)
    //        {
    //            for (int dy = -NEIGHBOR_DISTANCE; dy <= NEIGHBOR_DISTANCE; dy++)
    //            {
    //                if (dx == 0 && dy == 0)
    //                    continue;

    //                int nx = x + dx;
    //                int ny = y + dy;
    //                if (IsValidPosition(nx, ny) && board[nx, ny] != Constants.PlayerType.None)
    //                    return true;
    //            }
    //        }
    //        return false;
    //    }


    //    private (int, int) CheckForWinningMove(Constants.PlayerType player)
    //    {
    //        for (int r = 0; r < BOARD_SIZE; r++)
    //        {
    //            for (int c = 0; c < BOARD_SIZE; c++)
    //            {
    //                if (board[r, c] != Constants.PlayerType.None) continue;
    //                if (!HasNeighbor(r, c)) continue; // 주변에 돌이 없으면 건너뛰기

    //                if (IsWinningMove(r, c, player))
    //                {
    //                    return (r, c);
    //                }
    //            }
    //        }
    //        return (-1, -1);
    //    }









    //    // 3000 노말 (최대 시간 3초)
    //    public (int, int) GetBestMove(int timeLimit = 3000)
    //    {
    //        // Stopwatch를 사용하여 시간 추적
    //        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    //        stopwatch.Start();

    //        // 즉시 승리 가능한 수를 먼저 확인
    //        (int, int) winningMove = CheckForWinningMove(Constants.PlayerType.PlayerB);
    //        if (winningMove.Item1 != -1)
    //            return winningMove;

    //        // 즉시 패배를 막는 수를 확인
    //        (int, int) blockingMove = CheckForWinningMove(Constants.PlayerType.PlayerA);
    //        if (blockingMove.Item1 != -1)
    //            return blockingMove;

    //        // 트랜스포지션 테이블 초기화
    //        transpositionTable.Clear();

    //        // 유효한 이동 가져오기 (히스토리 테이블로 정렬)
    //        List<(int, int)> validMoves = GetSortedValidMoves(true);

    //        if (validMoves.Count == 0)
    //        {
    //            UnityEngine.Debug.LogError("착수 가능한 위치가 없습니다.");
    //            return (-1, -1);
    //        }

    //        // 최선의 이동 계산
    //        (int, int) bestMove = validMoves[0];
    //        int bestScore = int.MinValue;
    //        List<((int, int) move, int score)> bestMoves = new List<((int, int), int)>();
    //        System.Random random = new System.Random();
    //        bool allMovesLosing = true; // 모든 수가 지는 수인지 체크

    //        foreach (var move in validMoves)
    //        {
    //            board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
    //            int score = AlphaBetaPruningWithTimeLimit(1, int.MinValue, int.MaxValue, false, stopwatch, timeLimit);

    //            // 지는 수가 아닌 것이 있는지 확인
    //            if (score > -WIN_SCORE)
    //                allMovesLosing = false;

    //            board[move.Item1, move.Item2] = Constants.PlayerType.None;

    //            // 히스토리 테이블 업데이트
    //            historyTable[move.Item1, move.Item2] += score > 0 ? score : 0;

    //            // 모든 수가 지는 수라면 방어 점수를 계산
    //            int defensiveScore = 0;
    //            if (score <= -WIN_SCORE)
    //            {
    //                defensiveScore = EvaluateDefensiveMove(move.Item1, move.Item2);
    //            }


    //            if (score > bestScore)
    //            {
    //                bestScore = score;
    //                bestMoves.Clear();
    //                bestMoves.Add((move, score)); // 점수도 함께 저장
    //            }
    //            else if (score == bestScore)
    //            {
    //                bestMoves.Add((move, score));
    //            }

    //            // 시간 체크
    //            if (stopwatch.ElapsedMilliseconds >= timeLimit)
    //            {
    //                break; // 시간이 초과되면 탐색 종료
    //            }
    //        }

    //        // 모든 수가 지는 수라면 방어 점수에 따라 선택
    //        if (allMovesLosing && validMoves.Count > 0)
    //        {
    //            // StringBuilder tempShow = new StringBuilder();
    //            // tempShow.Append("모든 수가 지는 수입니다. 방어 점수에 따라 선택: ");

    //            // 각 수에 대한 방어 점수 계산 및 저장
    //            bestMoves.Clear();
    //            foreach (var move in validMoves)
    //            {
    //                int defensiveScore = EvaluateDefensiveMove(move.Item1, move.Item2);
    //                bestMoves.Add((move, defensiveScore));
    //            }

    //            // 방어 점수 기준으로 내림차순 정렬
    //            bestMoves = bestMoves.OrderByDescending(m => m.score).ToList();

    //            // 방어 점수 로깅
    //            //  foreach (var item in bestMoves)
    //            //  {
    //            //      tempShow.Append($"({item.move.Item1}, {item.move.Item2}) - 점수: {item.score}, ");
    //            //  }
    //            // Debug.Log(tempShow.ToString());

    //            // 동일한 최고 방어 점수를 가진 수들 목록
    //            if (bestMoves.Count > 0)
    //            {
    //                int highestDefScore = bestMoves[0].score;
    //                List<(int, int)> bestDefensiveMoves = new List<(int, int)>();

    //                foreach (var item in bestMoves)
    //                {
    //                    if (item.score == highestDefScore)
    //                        bestDefensiveMoves.Add(item.move);
    //                    else
    //                        break;
    //                }

    //                // 최고 방어 점수를 가진 수들 중 랜덤 선택
    //                bestMove = bestDefensiveMoves[random.Next(bestDefensiveMoves.Count)];
    //                // Debug.Log($"선택된 방어 좌표: ({bestMove.Item1}, {bestMove.Item2}) - 방어 점수: {highestDefScore}");
    //            }
    //            else
    //            {
    //                // 방어 점수가 계산되지 않은 경우 (예외 상황)
    //                bestMove = validMoves[random.Next(validMoves.Count)];
    //                // Debug.Log($"방어 점수가 계산되지 않아 무작위 선택된 좌표: ({bestMove.Item1}, {bestMove.Item2})");
    //            }
    //        }

    //        else
    //        {
    //            // 기존 방식대로 최고 점수를 가진 좌표들 중 랜덤하게 선택
    //            // StringBuilder tempShow = new StringBuilder();
    //            // foreach (var (move, score) in bestMoves)
    //            // {
    //            //     tempShow.Append($"({move.Item1}, {move.Item2}) - 점수: {score}, ");
    //            // }
    //            // Debug.Log(tempShow.ToString());

    //            // 최고 점수를 가진 좌표들 중 랜덤하게 선택
    //            bestMove = bestMoves[random.Next(bestMoves.Count)].move;
    //            // Debug.Log($"선택된 좌표: ({bestMove.Item1}, {bestMove.Item2}) - 점수: {bestScore}");
    //        }

    //        stopwatch.Stop();
    //        return bestMove;
    //    }

    //    private int AlphaBetaPruningWithTimeLimit(int depth, int alpha, int beta, bool isMaximizing, System.Diagnostics.Stopwatch stopwatch, int timeLimit)
    //    {
    //        // 시간이 초과하면 즉시 반환
    //        if (stopwatch.ElapsedMilliseconds >= timeLimit)
    //        {
    //            Debug.LogWarning("시간 초과! 평가 함수로 즉시 종료");
    //            return EvaluateBoard(); // 평가 함수로 즉시 종료
    //        }

    //        int winCheck = CheckWinningPosition();
    //        if (winCheck != 0)
    //        {
    //            return winCheck * (maxDepth - depth + 1);
    //        }

    //        if (depth >= 4) // 깊이는 고정 4
    //        {
    //            return EvaluateBoard();
    //        }

    //        long hash = ComputeZobristHash();
    //        if (transpositionTable.TryGetValue(hash, out TranspositionEntry entry) && entry.Depth >= 4 - depth)
    //        {
    //            if (entry.Flag == TranspositionEntry.EXACT)
    //                return entry.Score;
    //            else if (entry.Flag == TranspositionEntry.LOWERBOUND)
    //                alpha = Mathf.Max(alpha, entry.Score);
    //            else if (entry.Flag == TranspositionEntry.UPPERBOUND)
    //                beta = Mathf.Min(beta, entry.Score);

    //            if (alpha >= beta)
    //                return entry.Score;
    //        }

    //        List<(int, int)> validMoves = GetSortedValidMoves(isMaximizing);

    //        int bestScore;
    //        int flag = TranspositionEntry.UPPERBOUND;

    //        if (isMaximizing)
    //        {
    //            bestScore = int.MinValue;

    //            foreach (var move in validMoves)
    //            {
    //                // 현재 상태 저장 (좌표, 기존 값)
    //                var prevState = board[move.Item1, move.Item2];
    //                board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
    //                int score = AlphaBetaPruningWithTimeLimit(depth + 1, alpha, beta, false, stopwatch, timeLimit);

    //                board[move.Item1, move.Item2] = prevState;


    //                if (score > bestScore)
    //                {
    //                    bestScore = score;
    //                    if (depth == 0) historyTable[move.Item1, move.Item2] += 1 << (4 - depth);
    //                }

    //                alpha = Mathf.Max(alpha, bestScore);
    //                if (beta <= alpha)
    //                {
    //                    historyTable[move.Item1, move.Item2] += 1 << (4 - depth + 2);
    //                    break;
    //                }

    //                // 시간 체크
    //                if (stopwatch.ElapsedMilliseconds >= timeLimit)
    //                {
    //                    break;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            bestScore = int.MaxValue;

    //            foreach (var move in validMoves)
    //            {
    //                var prevState = board[move.Item1, move.Item2];
    //                board[move.Item1, move.Item2] = Constants.PlayerType.PlayerA;
    //                int score = AlphaBetaPruningWithTimeLimit(depth + 1, alpha, beta, true, stopwatch, timeLimit);

    //                board[move.Item1, move.Item2] = prevState;

    //                if (score < bestScore)
    //                {
    //                    bestScore = score;
    //                    if (depth == 0) historyTable[move.Item1, move.Item2] += 1 << (4 - depth);
    //                }

    //                beta = Mathf.Min(beta, bestScore);
    //                if (beta <= alpha)
    //                {
    //                    historyTable[move.Item1, move.Item2] += 1 << (4 - depth + 2);
    //                    break;
    //                }

    //                // 시간 체크
    //                if (stopwatch.ElapsedMilliseconds >= timeLimit)
    //                {
    //                    break;
    //                }
    //            }
    //        }

    //        if (bestScore <= alpha)
    //            flag = TranspositionEntry.UPPERBOUND;
    //        else if (bestScore >= beta)
    //            flag = TranspositionEntry.LOWERBOUND;
    //        else
    //            flag = TranspositionEntry.EXACT;

    //        transpositionTable[hash] = new TranspositionEntry
    //        {
    //            Score = bestScore,
    //            Depth = 4 - depth,
    //            Flag = flag
    //        };

    //        return bestScore;
    //    }

    //    // 이동을 히스토리 테이블 점수로 정렬 (좋은 이동부터 탐색)
    //    private List<(int, int)> GetSortedValidMoves(bool isMaximizing)
    //    {
    //        List<(int, int, int)> scoredMoves = new List<(int, int, int)>();

    //        for (int x = 0; x < BOARD_SIZE; x++)
    //        {
    //            for (int y = 0; y < BOARD_SIZE; y++)
    //            {
    //                if (board[x, y] == Constants.PlayerType.None && HasNeighbor(x, y))
    //                {
    //                    int score = historyTable[x, y];

    //                    // 간단한 휴리스틱으로 좋은 이동 먼저 평가
    //                    Constants.PlayerType player = isMaximizing ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA;
    //                    if (IsWinningMove(x, y, player))
    //                    {
    //                        score += 10000000; // 이기는 수에 가장 높은 우선순위
    //                    }
    //                    else if (IsWinningMove(x, y, isMaximizing ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB))
    //                    {
    //                        score += 5000000;  // 방어 수에 다음 우선순위
    //                    }
    //                    else
    //                    {
    //                        // 열린 3 체크
    //                        board[x, y] = player;
    //                        if (HasOpenThree(x, y, player))
    //                        {
    //                            score += 1000000;
    //                        }
    //                        board[x, y] = Constants.PlayerType.None;
    //                    }

    //                    scoredMoves.Add((x, y, score));
    //                }
    //            }
    //        }

    //        // 점수 기준으로 내림차순 정렬
    //        scoredMoves.Sort((a, b) => b.Item3.CompareTo(a.Item3));


    //        List<(int, int)> result = new List<(int, int)>(scoredMoves.Count);
    //        foreach (var move in scoredMoves)
    //        {
    //            result.Add((move.Item1, move.Item2));
    //        }
    //        return result;
    //    }





    //    // 방어 점수 계산 함수 수정
    //    private int EvaluateDefensiveMove(int r, int c)
    //    {
    //        int defensiveScore = 0;

    //        // 임시로 우리 돌 놓기
    //        board[r, c] = Constants.PlayerType.PlayerB;

    //        // 방어 점수 계산 - 이 위치에 돌을 놓음으로써 상대의 공격을 얼마나 방해하는지 평가

    //        // 1. 상대방의 열린 4 막기 (가장 높은 우선순위)
    //        int openFoursBlocked = CountBlockedOpenFours(r, c);
    //        int openFoursScore = openFoursBlocked * 10000;

    //        // 2. 상대방의 열린 3 막기 (높은 우선순위)
    //        int openThreesBlocked = CountBlockedOpenThrees(r, c);
    //        int openThreesScore = openThreesBlocked * 5000;

    //        // 3. 자신의 연결된 돌 수에 따른 점수
    //        int connectedStones = CountOwnConnectedStones(r, c);
    //        int connectedScore = connectedStones * 100;

    //        // 4. 보드 중앙 근처에 두는 것이 일반적으로 유리함
    //        int positionValue = EvaluatePositionValue(r, c);

    //        // 5. 이 위치가 향후 좋은 수를 만들 수 있는지 평가
    //        int futureValue = EvaluateFutureValue(r, c);

    //        // 방어 점수 합산
    //        defensiveScore += openFoursScore;
    //        defensiveScore += openThreesScore;
    //        defensiveScore += connectedScore;
    //        defensiveScore += positionValue;
    //        defensiveScore += futureValue;

    //        /*
    //        // 디버그 로그 추가
    //        if (openFoursBlocked > 0 || openThreesBlocked > 0 || connectedStones >= 2)
    //        {
    //            Debug.Log($"위치 ({r},{c}): 막은 열린4={openFoursBlocked}(점수:{openFoursScore}), " +
    //                      $"막은 열린3={openThreesBlocked}(점수:{openThreesScore}), " +
    //                      $"연결된 돌={connectedStones}(점수:{connectedScore}), " +
    //                      $"위치가치={positionValue}, 미래가치={futureValue}, " +
    //                      $"총점:{defensiveScore}");
    //        }*/

    //        // 원래 상태로 복구
    //        board[r, c] = Constants.PlayerType.None;

    //        return defensiveScore;
    //    }


    //    // 상대방의 열린 4와 열린 3을 막는 수의 개수 계산 함수 수정
    //    private int CountBlockedOpenFours(int r, int c)
    //    {
    //        int count = 0;
    //        Constants.PlayerType opponent = Constants.PlayerType.PlayerA;

    //        // 원래 상태 저장
    //        Constants.PlayerType original = board[r, c];

    //        // 이 위치에 돌이 없다고 가정
    //        board[r, c] = Constants.PlayerType.None;

    //        // 방향별 체크
    //        foreach (var dir in directions)
    //        {
    //            // 이 위치에 상대방 돌 놓기
    //            board[r, c] = opponent;

    //            // 이 방향으로 돌 세기
    //            int stoneCount = 1; // 현재 위치 포함
    //            bool openEnd1 = false;
    //            bool openEnd2 = false;

    //            // 정방향 확인
    //            int nr = r + dir[0];
    //            int nc = c + dir[1];
    //            int dirStones = 0;
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == opponent)
    //            {
    //                stoneCount++;
    //                dirStones++;
    //                nr += dir[0];
    //                nc += dir[1];
    //            }
    //            openEnd1 = IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None;

    //            // 역방향 확인
    //            nr = r - dir[0];
    //            nc = c - dir[1];
    //            int revDirStones = 0;
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == opponent)
    //            {
    //                stoneCount++;
    //                revDirStones++;
    //                nr -= dir[0];
    //                nc -= dir[1];
    //            }
    //            openEnd2 = IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None;

    //            // 디버그 로그 추가
    //            if (stoneCount >= 3)
    //            {
    //                // Debug.Log($"위치 ({r},{c}) - 방향 [{dir[0]},{dir[1]}]: 돌 수={stoneCount}, 정방향={dirStones}, " +
    //                //           $"역방향={revDirStones}, 정방향열림={openEnd1}, 역방향열림={openEnd2}");
    //            }

    //            // 양쪽이 열린 4 (매우 중요)
    //            if (stoneCount == 4 && openEnd1 && openEnd2)
    //            {
    //                count += 3;
    //                // Debug.Log($"위치 ({r},{c}) - 양쪽 열린 4 감지!");
    //            }
    //            // 한쪽만 열린 4
    //            else if (stoneCount == 4 && (openEnd1 || openEnd2))
    //            {
    //                count += 1;
    //                //  Debug.Log($"위치 ({r},{c}) - 한쪽 열린 4 감지!");
    //            }
    //            // 이 위치가 5목을 만드는 경우 (최우선)
    //            else if (stoneCount >= 5)
    //            {
    //                count += 5; // 가장 높은 가중치
    //                            // Debug.Log($"위치 ({r},{c}) - 5목 방지 위치!");
    //            }

    //            // 상태 원복
    //            board[r, c] = Constants.PlayerType.None;
    //        }

    //        // 원래 상태로 복구
    //        board[r, c] = original;

    //        return count;
    //    }

    //    // 상대방의 열린 3을 막는 수의 개수 계산 수정
    //    private int CountBlockedOpenThrees(int r, int c)
    //    {
    //        int count = 0;
    //        Constants.PlayerType opponent = Constants.PlayerType.PlayerA;

    //        // 원래 상태 저장
    //        Constants.PlayerType original = board[r, c];

    //        // 이 위치에 돌이 없다고 가정
    //        board[r, c] = Constants.PlayerType.None;

    //        // 방향별 체크
    //        foreach (var dir in directions)
    //        {
    //            // 이 위치에 상대방 돌 놓기
    //            board[r, c] = opponent;

    //            // 이 방향으로 돌 세기
    //            int stoneCount = 1; // 현재 위치 포함
    //            bool openEnd1 = false;
    //            bool openEnd2 = false;

    //            // 정방향 확인
    //            int nr = r + dir[0];
    //            int nc = c + dir[1];
    //            int dirStones = 0;
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == opponent)
    //            {
    //                stoneCount++;
    //                dirStones++;
    //                nr += dir[0];
    //                nc += dir[1];
    //            }
    //            openEnd1 = IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None;

    //            // 역방향 확인
    //            nr = r - dir[0];
    //            nc = c - dir[1];
    //            int revDirStones = 0;
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == opponent)
    //            {
    //                stoneCount++;
    //                revDirStones++;
    //                nr -= dir[0];
    //                nc -= dir[1];
    //            }
    //            openEnd2 = IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None;

    //            // 디버그 로그 추가
    //            if (stoneCount >= 2 && openEnd1 && openEnd2)
    //            {
    //                //  Debug.Log($"위치 ({r},{c}) - 3 체크 방향 [{dir[0]},{dir[1]}]: 돌 수={stoneCount}, " +
    //                //           $"정방향열림={openEnd1}, 역방향열림={openEnd2}");
    //            }

    //            // 양쪽이 열린 3 체크 (양쪽이 모두 열려있고 돌이 3개인 경우)
    //            if (stoneCount == 3 && openEnd1 && openEnd2)
    //            {
    //                count += 2;
    //                //   Debug.Log($"위치 ({r},{c}) - 양쪽 열린 3 감지!");
    //            }
    //            // 한쪽만 열린 3은 낮은 가중치
    //            else if (stoneCount == 3 && (openEnd1 || openEnd2))
    //            {
    //                count += 1;
    //                //  Debug.Log($"위치 ({r},{c}) - 한쪽 열린 3 감지!");
    //            }

    //            // 상태 원복
    //            board[r, c] = Constants.PlayerType.None;
    //        }

    //        // 원래 상태로 복구
    //        board[r, c] = original;

    //        return count;
    //    }


    //    // 자신의 연결된 돌 수 계산
    //    private int CountOwnConnectedStones(int r, int c)
    //    {
    //        int maxCount = 0;
    //        Constants.PlayerType player = Constants.PlayerType.PlayerB;

    //        foreach (var dir in directions)
    //        {
    //            int count = 1; // 현재 위치 포함

    //            // 정방향 확인
    //            int nr = r + dir[0];
    //            int nc = c + dir[1];
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == player)
    //            {
    //                count++;
    //                nr += dir[0];
    //                nc += dir[1];
    //            }

    //            // 역방향 확인
    //            nr = r - dir[0];
    //            nc = c - dir[1];
    //            while (IsValidPosition(nr, nc) && board[nr, nc] == player)
    //            {
    //                count++;
    //                nr -= dir[0];
    //                nc -= dir[1];
    //            }

    //            maxCount = Mathf.Max(maxCount, count);
    //        }

    //        return maxCount;
    //    }

    //    // 위치 가치 평가 (보드 중앙에 가까울수록 높은 점수)
    //    private int EvaluatePositionValue(int r, int c)
    //    {
    //        int centerR = BOARD_SIZE / 2;
    //        int centerC = BOARD_SIZE / 2;

    //        // 중앙에서의 거리 계산 (맨해튼 거리)
    //        int distanceToCenter = Mathf.Abs(r - centerR) + Mathf.Abs(c - centerC);

    //        // 중앙에 가까울수록 높은 점수 (최대 100점)
    //        return Mathf.Max(0, 100 - distanceToCenter * 10);
    //    }

    //    // 미래 가치 평가 (이 자리가 미래에 좋은 수를 만들 수 있는지)
    //    private int EvaluateFutureValue(int r, int c)
    //    {
    //        int score = 0;
    //        Constants.PlayerType player = Constants.PlayerType.PlayerB;

    //        // 이 자리에 두었을 때 다음 수에서 어떤 효과가 있는지 평가
    //        foreach (var dir in directions)
    //        {
    //            // 각 방향에서 1칸, 2칸 떨어진 곳에 빈 공간이 있고 유리한 형태를 만들 수 있는지 체크
    //            for (int dist = 1; dist <= 2; dist++)
    //            {
    //                int nr = r + dir[0] * dist;
    //                int nc = c + dir[1] * dist;

    //                if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
    //                {
    //                    // 이 위치에 돌을 놓아보고 효과 평가
    //                    board[nr, nc] = player;
    //                    if (HasOpenThree(nr, nc, player))
    //                    {
    //                        score += 50;
    //                    }
    //                    board[nr, nc] = Constants.PlayerType.None;
    //                }

    //                // 반대 방향도 체크
    //                nr = r - dir[0] * dist;
    //                nc = c - dir[1] * dist;

    //                if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
    //                {
    //                    board[nr, nc] = player;
    //                    if (HasOpenThree(nr, nc, player))
    //                    {
    //                        score += 50;
    //                    }
    //                    board[nr, nc] = Constants.PlayerType.None;
    //                }
    //            }
    //        }

    //        return score;
    //    }
    //}


    /*
    public class OmokAI
    {
        private const int BOARD_SIZE = 15;
        private Constants.PlayerType[,] board;
        public int maxDepth = 4;

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

        public OmokAI(Constants.PlayerType[,] board, int max = 4)
        {
            this.board = board;
            maxDepth = max;
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
                int count = 1;
                bool leftOpen = false;
                bool rightOpen = false;

                // 왼쪽 체크
                int nr = r - dir[0];
                int nc = c - dir[1];
                if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
                {
                    leftOpen = true;

                    // 추가 체크 (빈 칸 뒤에 또 돌이 있는지)
                    nr -= dir[0];
                    nc -= dir[1];
                    if (IsValidPosition(nr, nc) && board[nr, nc] == player)
                    {
                        count++;
                        nr -= dir[0];
                        nc -= dir[1];
                        if (IsValidPosition(nr, nc) && board[nr, nc] == player)
                        {
                            count++;
                        }
                    }
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

                // 오른쪽 끝이 열려있는지
                if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
                {
                    rightOpen = true;
                }

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










        public (int, int) GetBestMove()
        {
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
                Debug.LogError("착수 가능한 위치가 없습니다.");
                return (-1, -1);
            }

            // 최선의 이동 계산
            (int, int) bestMove = validMoves[0];
            int bestScore = int.MinValue;


            foreach (var move in validMoves)
            {
                board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
                int score = AlphaBetaPruning(1, int.MinValue, int.MaxValue, false);
                board[move.Item1, move.Item2] = Constants.PlayerType.None;

                // 히스토리 테이블 업데이트
                historyTable[move.Item1, move.Item2] += score > 0 ? score : 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int AlphaBetaPruning(int depth, int alpha, int beta, bool isMaximizing)
        {
            int winCheck = CheckWinningPosition();
            if (winCheck != 0)
            {
                return winCheck * (maxDepth - depth + 1);
            }

            if (depth >= maxDepth)
            {
                return EvaluateBoard();
            }

            long hash = ComputeZobristHash();
            if (transpositionTable.TryGetValue(hash, out TranspositionEntry entry) && entry.Depth >= maxDepth - depth)
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
                    int score = AlphaBetaPruning(depth + 1, alpha, beta, false);

                    board[move.Item1, move.Item2] = prevState;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        if (depth == 0) historyTable[move.Item1, move.Item2] += 1 << (maxDepth - depth);
                    }

                    alpha = Mathf.Max(alpha, bestScore);
                    if (beta <= alpha)
                    {
                        historyTable[move.Item1, move.Item2] += 1 << (maxDepth - depth + 2);
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
                    int score = AlphaBetaPruning(depth + 1, alpha, beta, true);

                    board[move.Item1, move.Item2] = prevState;

                    if (score < bestScore)
                    {
                        bestScore = score;
                        if (depth == 0) historyTable[move.Item1, move.Item2] += 1 << (maxDepth - depth);
                    }

                    beta = Mathf.Min(beta, bestScore);
                    if (beta <= alpha)
                    {
                        historyTable[move.Item1, move.Item2] += 1 << (maxDepth - depth + 2);
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
                Depth = maxDepth - depth,
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

            // 좌표만 반환
           // return scoredMoves.Select(m => (m.Item1, m.Item2)).ToList();
        }

    }
    */


    /*
    public class OmokAI
    {
        private const int BOARD_SIZE = 15;
        private Constants.PlayerType[,] board;
        public int maxDepth = 4;

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

        public OmokAI(Constants.PlayerType[,] board, int max = 4)
        {
            this.board = board;
            maxDepth = max;
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

        public int AlphaBetaPruning(int depth, int alpha, int beta, bool isMaximizing)
        {
            // 승리 여부 빠르게 확인
            int winCheck = CheckWinningPosition();
            if (winCheck != 0)
            {
                return winCheck * (maxDepth - depth + 1); // 빨리 승리할수록 더 높은 점수
            }

            // 최대 깊이 도달
            if (depth >= maxDepth)
            {
                return EvaluateBoard();
            }

            // 트랜스포지션 테이블 확인
            long hash = ComputeZobristHash();
            if (transpositionTable.TryGetValue(hash, out TranspositionEntry entry) && entry.Depth >= maxDepth - depth)
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

            // 유효한 이동 가져오기
            List<(int, int)> validMoves = GetSortedValidMoves(isMaximizing);

            int originalAlpha = alpha;
            int bestScore;
            int flag = TranspositionEntry.UPPERBOUND;

            if (isMaximizing)
            {
                bestScore = int.MinValue;

                foreach (var move in validMoves)
                {
                    board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
                    int score = AlphaBetaPruning(depth + 1, alpha, beta, false);
                    board[move.Item1, move.Item2] = Constants.PlayerType.None;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        // 히스토리 테이블 업데이트 - 좋은 수를 기억
                        if (depth == 0) historyTable[move.Item1, move.Item2] += 1 << (maxDepth - depth);
                    }

                    alpha = Mathf.Max(alpha, bestScore);
                    if (beta <= alpha)
                    {
                        // 히스토리 테이블 더 크게 업데이트 - 컷오프를 일으킨 수
                        historyTable[move.Item1, move.Item2] += 1 << (maxDepth - depth + 2);
                        break;
                    }
                }
            }
            else
            {
                bestScore = int.MaxValue;

                foreach (var move in validMoves)
                {
                    board[move.Item1, move.Item2] = Constants.PlayerType.PlayerA;
                    int score = AlphaBetaPruning(depth + 1, alpha, beta, true);
                    board[move.Item1, move.Item2] = Constants.PlayerType.None;

                    if (score < bestScore)
                    {
                        bestScore = score;
                        // 히스토리 테이블 업데이트
                        if (depth == 0) historyTable[move.Item1, move.Item2] += 1 << (maxDepth - depth);
                    }

                    beta = Mathf.Min(beta, bestScore);
                    if (beta <= alpha)
                    {
                        // 히스토리 테이블 더 크게 업데이트
                        historyTable[move.Item1, move.Item2] += 1 << (maxDepth - depth + 2);
                        break;
                    }
                }
            }

            // 결과를 트랜스포지션 테이블에 저장
            if (bestScore <= originalAlpha)
                flag = TranspositionEntry.UPPERBOUND;
            else if (bestScore >= beta)
                flag = TranspositionEntry.LOWERBOUND;
            else
                flag = TranspositionEntry.EXACT;

            transpositionTable[hash] = new TranspositionEntry
            {
                Score = bestScore,
                Depth = maxDepth - depth,
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

            // 좌표만 반환
            return scoredMoves.Select(m => (m.Item1, m.Item2)).ToList();
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
                int count = 1;
                bool leftOpen = false;
                bool rightOpen = false;

                // 왼쪽 체크
                int nr = r - dir[0];
                int nc = c - dir[1];
                if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
                {
                    leftOpen = true;

                    // 추가 체크 (빈 칸 뒤에 또 돌이 있는지)
                    nr -= dir[0];
                    nc -= dir[1];
                    if (IsValidPosition(nr, nc) && board[nr, nc] == player)
                    {
                        count++;
                        nr -= dir[0];
                        nc -= dir[1];
                        if (IsValidPosition(nr, nc) && board[nr, nc] == player)
                        {
                            count++;
                        }
                    }
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

                // 오른쪽 끝이 열려있는지
                if (IsValidPosition(nr, nc) && board[nr, nc] == Constants.PlayerType.None)
                {
                    rightOpen = true;
                }

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

        public (int, int) GetBestMove()
        {
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
                Debug.LogError("착수 가능한 위치가 없습니다.");
                return (-1, -1);
            }

            // 최선의 이동 계산
            (int, int) bestMove = validMoves[0];
            int bestScore = int.MinValue;

            foreach (var move in validMoves)
            {
                board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
                int score = AlphaBetaPruning(1, int.MinValue, int.MaxValue, false);
                board[move.Item1, move.Item2] = Constants.PlayerType.None;

                // 히스토리 테이블 업데이트
                historyTable[move.Item1, move.Item2] += score > 0 ? score : 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        // 즉시 승리 또는 패배 가능성 먼저 확인 (기존 함수와 동일하지만 최적화)
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
    }
    
    */

    /*
    public class OmokAI
    {
        private const int BOARD_SIZE = 15;
        private Constants.PlayerType[,] board;
        public int maxDepth = 4;

        public OmokAI(Constants.PlayerType[,] board, int max = 4)
        {
            this.board = board;
            maxDepth = max;
        }

        // 이전 결과 캐싱
        private Dictionary<string, int> transpositionTable = new Dictionary<string, int>();


        public int AlphaBetaPruning(int depth, int alpha, int beta)
        {
            // 이전에 계산된 결과가 있으면 재사용
            string boardKey = GetBoardKey();
            string stateKey = $"{boardKey}_{depth}_{alpha}_{beta}";

            if (transpositionTable.TryGetValue(stateKey, out int cachedScore))
            {
                return cachedScore;
            }


            if (depth == maxDepth)
            {
                return EvaluateBoard();
            }

            List<(int, int)> validMoves = GetValidMoves();

            // 탐색 순서를 랜덤으로 섞음
            validMoves = validMoves.OrderBy(_ => UnityEngine.Random.value).ToList();

            if (depth % 2 == 0)
            { // AI 턴 (최대화)
                int v = int.MinValue;
                bool pruning = false;

                foreach (var move in validMoves)
                {
                    if (board[move.Item1, move.Item2] == Constants.PlayerType.None)
                    {
                        board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
                        int temp = AlphaBetaPruning(depth + 1, alpha, beta);
                        v = Mathf.Max(v, temp);
                        board[move.Item1, move.Item2] = Constants.PlayerType.None;
                        alpha = Mathf.Max(alpha, v);
                        if (beta <= alpha)
                        {
                            pruning = true;
                            break;
                        }
                    }
                }

                transpositionTable[stateKey] = v;
                return v;
            }
            else
            { // 플레이어 턴 (최소화)
                int v = int.MaxValue;
                bool pruning = false;

                foreach (var move in validMoves)
                {
                    if (board[move.Item1, move.Item2] == Constants.PlayerType.None)
                    {
                        board[move.Item1, move.Item2] = Constants.PlayerType.PlayerA;
                        int temp = AlphaBetaPruning(depth + 1, alpha, beta);
                        v = Mathf.Min(v, temp);
                        board[move.Item1, move.Item2] = Constants.PlayerType.None;
                        beta = Mathf.Min(beta, v);
                        if (beta <= alpha)
                        {
                            pruning = true;
                            break;
                        }
                    }
                }

                transpositionTable[stateKey] = v;
                return v;
            }
        }

        private string GetBoardKey()
        {
            // 보드 상태를 문자열로 변환
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    sb.Append((int)board[i, j]);
                }
            }
            return sb.ToString();
        }

        private List<(int, int)> GetValidMoves()
        {
            List<(int, int)> validMoves = new List<(int, int)>();
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    if (board[x, y] == Constants.PlayerType.None && HasNeighbor(x, y))
                    {
                        validMoves.Add((x, y));
                    }
                }
            }
            return validMoves;
        }

        private int EvaluateBoard()
        { 
            int score = 0;
            int[] weights = new int[] { 0, 1, 10, 100, 1000, 100000 };
            int[][] directions = new int[][] { new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 1, -1 } };

            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    Constants.PlayerType cell = board[r, c];
                    if (cell == Constants.PlayerType.None) continue;

                    foreach (var dir in directions)
                    {
                        int count = 1;
                        int nr = r + dir[0];
                        int nc = c + dir[1];
                        while (nr >= 0 && nr < BOARD_SIZE && nc >= 0 && nc < BOARD_SIZE && board[nr, nc] == cell)
                        {
                            count++;
                            nr += dir[0];
                            nc += dir[1];
                        }

                        if (count >= 5)
                        {
                            return (cell == Constants.PlayerType.PlayerB) ? 2000000 : -2000000;
                        }

                        if (cell == Constants.PlayerType.PlayerB)
                        {
                            if (count == 4)
                            {
                                score += 200000; 
                            }
                            score += weights[count];
                        }
                        else if (cell == Constants.PlayerType.PlayerA)
                        {
                            if (count == 4)
                            {
                                score -= 200000; 
                            }
                            score -= weights[count];
                        }
                    }
                }
            }


            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    if (board[r, c] != Constants.PlayerType.None)
                        continue;

                    foreach (var dir in directions)
                    {
                        int countB = 0; // AI(PlayerB)의 연속 돌 수
                        int countA = 0; // 플레이어(PlayerA)의 연속 돌 수
                        int nrB = r + dir[0], ncB = c + dir[1];
                        int nrA = r + dir[0], ncA = c + dir[1];

                        while (nrB >= 0 && nrB < BOARD_SIZE && ncB >= 0 && ncB < BOARD_SIZE && board[nrB, ncB] == Constants.PlayerType.PlayerB)
                        {
                            countB++;
                            nrB += dir[0];
                            ncB += dir[1];
                        }

                        while (nrA >= 0 && nrA < BOARD_SIZE && ncA >= 0 && ncA < BOARD_SIZE && board[nrA, ncA] == Constants.PlayerType.PlayerA)
                        {
                            countA++;
                            nrA += dir[0];
                            ncA += dir[1];
                        }

                        // AI가 승리할 수 있는 자리가 있으면 높은 점수를 부여
                        if (countB == 4)
                        {
                            score += 100000; // AI 승리 점수
                        }

                        // 상대가 승리할 수 있는 자리를 발견하면 즉시 막기
                        if (countA == 4)
                        {
                            score -= 100000; // 플레이어 승리 방지 점수
                        }

                        // AI가 승리할 수 있는 연속된 돌을 놓을 자리를 찾은 경우에도 높은 점수를 부여
                        if (countB == 3)
                        {
                            score += 50000; // AI가 3개 연속을 만들 수 있는 위치를 우선적으로 평가
                        }

                        // 플레이어가 3개 연속을 만들 수 있는 자리를 발견하면 이를 방어하기 위한 점수 추가
                        if (countA == 3)
                        {
                            score -= 50000; // 플레이어가 3개 연속을 만들지 못하도록 방어하는 점수
                        }
                    }
                }
            }

            return score;
        }
       

        private bool HasNeighbor(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < BOARD_SIZE && ny >= 0 && ny < BOARD_SIZE)
                    {
                        if (board[nx, ny] != Constants.PlayerType.None)
                            return true;
                    }
                }
            }
            return false;
        }


        public (int, int) GetBestMove()
        {
            // 즉시 승리 가능한 수를 먼저 확인
            (int, int) winningMove = CheckForWinningMove(Constants.PlayerType.PlayerB);
            if (winningMove.Item1 != -1)
            {
                return winningMove;
            }

            // 즉시 패배를 막는 수를 확인
            (int, int) blockingMove = CheckForWinningMove(Constants.PlayerType.PlayerA);
            if (blockingMove.Item1 != -1)
            {
                return blockingMove;
            }

            List<(int, int)> validMoves = GetValidMoves();

            if (validMoves.Count == 0)
            {
                Debug.LogError("착수 가능한 위치가 없습니다.");
                return (-1, -1);
            }

            validMoves.Sort((a, b) => {
                int scoreA = EvaluateMove(a.Item1, a.Item2);
                int scoreB = EvaluateMove(b.Item1, b.Item2);
                return scoreB.CompareTo(scoreA);  // 점수가 높은 순으로 정렬
            });

            List<(int, int)> bestMoves = new List<(int, int)>();
            int bestScore = int.MinValue;

            foreach (var move in validMoves)
            {
                board[move.Item1, move.Item2] = Constants.PlayerType.PlayerB;
                int score = AlphaBetaPruning(1, int.MinValue, int.MaxValue);
                board[move.Item1, move.Item2] = Constants.PlayerType.None;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(move);
                }
            }

            return bestMoves[0]; // 가장 높은 점수를 가진 첫 번째 수를 선택
        }

        private int EvaluateMove(int x, int y)
        {
            // 해당 위치에 돌을 놓고 점수를 계산
            Constants.PlayerType original = board[x, y];
            board[x, y] = Constants.PlayerType.PlayerB; // AI 돌 놓기

            int score = EvaluateBoard(); // 보드 평가

            board[x, y] = original; // 원래 상태로 돌려놓기

            return score;
        }

        // 즉시 승리 또는 패배 가능성 먼저 확인
        private (int, int) CheckForWinningMove(Constants.PlayerType player)
        {
            int[][] directions = new int[][] { new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 1, -1 } };

            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    if (board[r, c] != Constants.PlayerType.None) continue;

                    board[r, c] = player;

                    foreach (var dir in directions)
                    {
                        int count = 1;
                        int nr = r + dir[0];
                        int nc = c + dir[1];
                        while (nr >= 0 && nr < BOARD_SIZE && nc >= 0 && nc < BOARD_SIZE && board[nr, nc] == player)
                        {
                            count++;
                            nr += dir[0];
                            nc += dir[1];
                        }

                        // 반대 방향 확인
                        nr = r - dir[0];
                        nc = c - dir[1];
                        while (nr >= 0 && nr < BOARD_SIZE && nc >= 0 && nc < BOARD_SIZE && board[nr, nc] == player)
                        {
                            count++;
                            nr -= dir[0];
                            nc -= dir[1];
                        }

                        if (count >= 5)
                        {
                            board[r, c] = Constants.PlayerType.None;
                            return (r, c);
                        }
                    }

                    board[r, c] = Constants.PlayerType.None;
                }
            }

            return (-1, -1);
        }
    }
    */
    #endregion
}
