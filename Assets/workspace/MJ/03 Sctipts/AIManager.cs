using System;

namespace MJ
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class MCTS 
    {
        private const int BoardSize = 15;
        private int[,] board;
        private int simulations = 1000;
        private double exploration = 1.4;
        private System.Random random = new System.Random();

        // 생성자
        public MCTS(int[,] board) 
        {
            this.board = board;
        }

        // 파라미터 설정
        public void SetParams(int simulations, double exploration) 
        {
            this.simulations = simulations;
            this.exploration = exploration;
        }
        
        //디버깅용
        public int GetSimulations() => simulations;
        public double GetExploration() => exploration;

        //ai의 판단, x, y 위치로 반환
        public (int, int) ai() 
        {
            // AI 턴 (-1)
            return MctsSearch(-1);
        }

        // 모든 빈칸 반환
        private List<(int, int)> GetValidMoves() 
        {
            List<(int, int)> validMoves = new List<(int, int)>();
            for (int r = 0; r < BoardSize; r++) 
                for (int c = 0; c < BoardSize; c++) 
                    if (board[r, c] == 0) 
                        validMoves.Add((r, c));
            return validMoves;
        }

        // 이동 가능 위치(본뜬 보드) 모으고
        /**
         *  1. 이동가능한 모든 위치 확인, 플레이어가 두는걸 예측
         */
        private int SimulateRandomGame(int[,] tempBoard, int player) 
        {
            List<(int, int)> moves = GetValidMoves();
            while (moves.Count > 0) 
            {
                var move = moves[random.Next(moves.Count)];
                tempBoard[move.Item1, move.Item2] = player;
                moves.Remove(move);

                if (CheckWinner(tempBoard, move))
                    return tempBoard[move.Item1, move.Item2];

                player = -player;
            }
            //무승부
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

            int x = move.Item1, y = move.Item2, player = tempBoard[x, y];
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

        private double UCT(int wins, int visits, int parentVisits) 
        {
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
        private (int, int) MctsSearch(int player)
        {
            var validMoves = GetValidMoves();
            Dictionary<(int, int), int> visits = validMoves.ToDictionary(m => m, _ => 0);
            Dictionary<(int, int), int> wins = validMoves.ToDictionary(m => m, _ => 0);

            for (int i = 0; i < simulations; i++)
            {
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

    public static class AIManager
    {
        public enum AILevel
        {
            Beginner,
            Intermediate,
            Advanced
        }
        
        //등급에 따른 AI 난이도 결정
        public static AILevel GetAIlevelByUser(UserSession userSession)
        {
            int grade = userSession.Grade;
            
            if(grade >= 10 && grade <= 18) return AILevel.Beginner;
            else if(grade >= 5 && grade <= 9) return AILevel.Intermediate;
            else if(grade >= 1 && grade <= 4) return AILevel.Advanced;
            else return AILevel.Beginner;
        }
        
        //MCTS에 AI 설정
        public static void SetupAIByUserGrade(MCTS ai, UserSession userSession)
        {
            AILevel aiLevel = GetAIlevelByUser(userSession);

            switch (aiLevel)
            {
                case AILevel.Beginner:
                    ai.SetParams(300, 0.5);
                    break;
                case AILevel.Intermediate:
                    ai.SetParams(800, 1.4);
                    break;
                case AILevel.Advanced:
                    ai.SetParams(2000, 2.0);
                    break;
            }
            Debug.Log($"[AI설정] 유저: {userSession.Nickname}, 등급: {userSession.Grade}, 레벨:{aiLevel}, "
                + $"시뮬레이션: {ai.GetSimulations()}, 탐색 계수 :{ai.GetExploration()}");
        }
    }
}
