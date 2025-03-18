using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using workspace.Ham6._03_Sctipts.Game;

namespace KimHyeun {
    public class OmokAI : MonoBehaviour
    {
        // 현재 보드 상태를 읽어서 (board) None인 곳에 둘수있음, 각 플레이어의 돌 위치에 따라 오목룰에 따라 최적의 좌표값 찾기
        // 띄워져 있는 곳도 체크 필요
        

        const int BOARD_SIZE = 15;

        Constants.PlayerType[,] board; // -> 배열 차체가 보드.ex) 플레이어A가 제일 좌하단에 두면 board[0, 0] 에 PlayerA 가 들어감
                                       // PlayerType의 값 : None, PlayerA, PlayerB

        
        int maxDepth = 4; // 최대 탐색 깊이
        int simulationCount = 100; // MCTS 시뮬레이션 횟수


        public OmokAI(Constants.PlayerType[,] board, int max)
        {
            Debug.Log($"깊이 설정: {max}");
            this.board = board;
            maxDepth = max;
        }





        public (int, int) GetBestMove()
        {
            int bestScore = int.MinValue;
            (int, int) bestMove = (-1, -1);

            // 가능한 모든 수를 구하기
            var possibleMoves = GetPotentialMoves(board);
            List<MCTSNode> nodes = new List<MCTSNode>();

            // 각 수에 대해 MCTS 노드 생성
            foreach (var move in possibleMoves)
            {
                MCTSNode node = new MCTSNode
                {
                    x = move.Item2,
                    y = move.Item1,
                    parent = null
                };
                nodes.Add(node);
            }

            // 시뮬레이션을 통해 가장 좋은 수 찾기
            foreach (var node in nodes)
            {
                int localSimulationCount = Mathf.Min(simulationCount, 100); // 시뮬레이션 횟수 제한

                for (int i = 0; i < localSimulationCount; i++)
                {
                    RunSimulation(node);
                }

                // 가장 많이 승리한 노드를 선택
                if (node.winCount > bestScore)
                {
                    bestScore = node.winCount;
                    bestMove = (node.x, node.y);
                }
            }

            return bestMove;
        }

        private void RunSimulation(MCTSNode node)
        {
            Constants.PlayerType[,] simulationBoard = (Constants.PlayerType[,])board.Clone(); // 보드 복사
            Constants.PlayerType currentPlayer = Constants.PlayerType.PlayerA;
            simulationBoard[node.y, node.x] = currentPlayer;

            currentPlayer = (currentPlayer == Constants.PlayerType.PlayerA) ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA;

            // 무작위 시뮬레이션
            while (!IsGameOver(simulationBoard, node.x, node.y))
            {
                var randomMoves = GetPotentialMoves(simulationBoard);
                if (randomMoves.Count == 0) break;

                var randomMove = randomMoves[Random.Range(0, randomMoves.Count)];
                simulationBoard[randomMove.Item1, randomMove.Item2] = currentPlayer;

                currentPlayer = (currentPlayer == Constants.PlayerType.PlayerA) ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA;
            }

            // 게임 종료 시 승리 기록
            if (IsGameOver(simulationBoard, node.x, node.y))
            {
                if (currentPlayer == Constants.PlayerType.PlayerA)
                    node.winCount++;
            }
        }

        private List<(int, int)> GetPotentialMoves(Constants.PlayerType[,] board)
        {
            List<(int, int)> potentialMoves = new List<(int, int)>();

            // 우선순위 높은 위치 먼저 탐색
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    if (board[y, x] == Constants.PlayerType.None)
                    {
                        if (IsNearEnemy(board, x, y)) // 상대 돌 근처를 우선 선택
                        {
                            potentialMoves.Add((y, x));
                        }
                    }
                }
            }

            // 우선순위 높은 곳 먼저 정렬
            potentialMoves.Sort((a, b) => CalculateMovePriority(a).CompareTo(CalculateMovePriority(b)));

            return potentialMoves;
        }

        private bool IsNearEnemy(Constants.PlayerType[,] board, int x, int y)
        {
            // 주어진 좌표 주변에 적이 있는지 확인
            int[] directions = { -1, 0, 1 };
            foreach (var dx in directions)
            {
                foreach (var dy in directions)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && nx < BOARD_SIZE && ny >= 0 && ny < BOARD_SIZE)
                    {
                        if (board[ny, nx] != Constants.PlayerType.None)
                            return true;
                    }
                }
            }
            return false;
        }

        private int CalculateMovePriority((int, int) move)
        {
            // 위치 우선순위 계산 예시
            return Mathf.Abs(move.Item1 - (BOARD_SIZE / 2)) + Mathf.Abs(move.Item2 - (BOARD_SIZE / 2));
        }

        private bool IsGameOver(Constants.PlayerType[,] simulationBoard, int lastX, int lastY)
        {
            // 최근 두었거나 변경된 곳 주변만 확인
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (CheckFiveInRow(simulationBoard, lastX + i, lastY + j))
                        return true;
                }
            }
            return false;
        }

        private bool CheckFiveInRow(Constants.PlayerType[,] simulationBoard, int x, int y)
        {
            Constants.PlayerType player = simulationBoard[y, x];
            if (player == Constants.PlayerType.None) return false;

            int[][] directions = new int[][]
            {
            new int[] {1, 0}, new int[] {0, 1}, new int[] {1, 1}, new int[] {1, -1}
            };

            foreach (var dir in directions)
            {
                int count = 1;
                for (int i = 1; i < 5; i++)
                {
                    int nx = x + dir[0] * i;
                    int ny = y + dir[1] * i;
                    if (nx < 0 || ny < 0 || nx >= BOARD_SIZE || ny >= BOARD_SIZE || simulationBoard[ny, nx] != player)
                        break;
                    count++;
                }
                for (int i = 1; i < 5; i++)
                {
                    int nx = x - dir[0] * i;
                    int ny = y - dir[1] * i;
                    if (nx < 0 || ny < 0 || nx >= BOARD_SIZE || ny >= BOARD_SIZE || simulationBoard[ny, nx] != player)
                        break;
                    count++;
                }
                if (count >= 5) return true;
            }
            return false;
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


    public class MCTSNode
    {
        public int x;
        public int y;
        public MCTSNode parent;
        public int winCount = 0;
        public int visitCount = 0;
    }
}

