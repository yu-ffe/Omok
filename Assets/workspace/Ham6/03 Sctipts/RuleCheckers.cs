using System.Collections.Generic;
using UnityEngine;
using workspace.Ham6._03_Sctipts.Game;

namespace workspace.Ham6._03_Sctipts
{
    public class RuleCheckers
    {
        const int MAX = 15;
        const int MIN = 0;
        Constants.PlayerType[,] board;
        readonly Vector2Int[] DIR = { new(1, 0), new(1, 1), new(0, 1), new(-1, 1) };

        public void Initialize(Constants.PlayerType[,] playingBoard)
        {
            board = playingBoard;
        }

        public RuleCheckers()
        {
            board = new Constants.PlayerType[MAX, MAX];
        }

        int minX, minY, maxX, maxY;
        readonly List<Vector2Int> InvalidPlaces = new List<Vector2Int>();

        public List<Vector2Int> CheckAllBoard()
        {
            InvalidPlaces.Clear();
            // GameManager.Instance.Logic.OmokBoard.ClearXMarker();
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] != Constants.PlayerType.None) // 빈 곳만 검사
                        continue;

                    (bool isViolation, int v3, int v4, int v6, int sc3, int sbv3) = IsRuleViolation(x, y);
                    if (isViolation)
                    {
                        InvalidPlaces.Add(new(x, y));
                        
                        Debug.Log($"<color=#ff0000>{x},{y} is violation</color>");
                        // Debug.Log($"금수 위치: {x},{y} ( 3x3:{v3} / 4x4:{v4} / 6x:{v6} / s3:{sc3} / sbv3:{sbv3} )");
                        // GameManager.Instance.Logic.OmokBoard.PlaceX(x, y);
                    }
                }
            }

            return InvalidPlaces;
        }

        /// <summary> 현재 착수한 위치에서 열린 3, 열린 4, 6목 이상을 검사하는 함수 </summary>
        (bool isViolation, int v3, int v4, int v6, int sv3, int sbv3) IsRuleViolation(int x, int y)
        {
            // 🔹 검사할 4가지 방향 (반대 방향도 검사하므로 4개만 필요)
            Vector2Int[] directions = new Vector2Int[]
            {
                new(1, 0), // 가로 (→)
                new(0, 1), // 세로 (↓)
                new(1, 1), // 대각선 (↘)
                new(1, -1) // 대각선 (↗)
            };

            int countOpened3 = 0;
            int countOpened4 = 0;
            int countOver6 = 0;
            int countSpaceThree = 0;
            int countSpaceBetweenThree = 0;

            foreach (Vector2Int dir in directions)
            {
                var (openThree, openFour, overSix, spaceThree, spaceBetweenThree) = CheckRow(x, y, dir);

                if (openFour)
                    countOpened4++;
                else if (openThree)
                    countOpened3++;
                else if (overSix) countOver6++;
                if (spaceThree) countSpaceThree++;
                if (spaceBetweenThree) countSpaceBetweenThree++;
            }

            bool hasDoubleOpenThree = countOpened3 >= 2;
            bool hasDoubleOpenFour = countOpened4 >= 2;
            bool hasSixOrMore = countOver6 > 0;
            bool hasDoubleSpaceThree = countSpaceThree >= 2;
            // bool hasDoubleBetweenThree = countBetweenThree >= 2;
            bool hasDoubleSpaceBetweenThree = countSpaceBetweenThree >= 2;

            // 조합 조건을 따로 분리
            bool hasOpenThreeAndSpaceThree = countOpened3 + countSpaceThree >= 2;
            // bool hasOpenThreeAndBetweenThree = countOpened3 + countBetweenThree >= 2;
            bool hasOpenThreeAndSpaceBetweenThree = countOpened3 + countSpaceBetweenThree >= 2;
            // bool hasSpaceThreeAndBetweenThree = countSpaceThree + countBetweenThree >= 2;
            bool hasSpaceThreeAndSpaceBetweenThree = countSpaceThree + countSpaceBetweenThree >= 2;
            // bool hasBetweenThreeAndSpaceBetweenThree = countBetweenThree + countSpaceBetweenThree >= 2;

            // 최종 위반 여부
            bool isViolation = hasDoubleOpenThree || hasDoubleOpenFour || hasSixOrMore || hasDoubleSpaceThree ||
                               hasDoubleSpaceBetweenThree || hasOpenThreeAndSpaceThree ||
                               hasOpenThreeAndSpaceBetweenThree || hasSpaceThreeAndSpaceBetweenThree;

            return (isViolation, countOpened3, countOpened4, countOver6, countSpaceThree, countSpaceBetweenThree);
        }

        /// <summary> 특정 위치에서 열린 3, 열린 4, 6목, 띄운 3목, 양쪽 돌 1개씩인 3목을 검사 </summary>
        (bool isOpenThree, bool isOpenFour, bool isOverSix, bool isSpaceThree, bool isSpaceBetweenThree) CheckRow(int x,
            int y, Vector2Int dir)
        {
            int count = 1; // 현재 착수한 돌 포함
            bool leftOpen = false, rightOpen = false;
            int px = x, py = y;

            while (IsValid(px - dir.x, py - dir.y) && board[px - dir.x, py - dir.y] == Constants.PlayerType.PlayerA)
            {
                count++;
                px -= dir.x;
                py -= dir.y;
            }

            int leftEndX = px - dir.x, leftEndY = py - dir.y;
            if (IsValid(leftEndX, leftEndY) && board[leftEndX, leftEndY] == Constants.PlayerType.None) leftOpen = true;

            // 반대 방향 탐색
            px = x;
            py = y;
            while (IsValid(px + dir.x, py + dir.y) && board[px + dir.x, py + dir.y] == Constants.PlayerType.PlayerA)
            {
                count++;
                px += dir.x;
                py += dir.y;
            }

            int rightEndX = px + dir.x, rightEndY = py + dir.y;
            if (IsValid(rightEndX, rightEndY) && board[rightEndX, rightEndY] == Constants.PlayerType.None)
                rightOpen = true;

            bool isOpenThree = (count == 3 && leftOpen && rightOpen);

            bool isOpenFour = (count == 4 && leftOpen && rightOpen);

            bool isOverSix = (count >= 6);

            bool isSpaceThree = false;
            bool isSpaceBetweenThree = false;

            px = x;
            py = y;
            int leftStoneCount = 0;
            bool leftEmptyFirst = false;

            if (IsValid(px - dir.x, py - dir.y) && board[px - dir.x, py - dir.y] == Constants.PlayerType.None)
            {
                leftEmptyFirst = true;
                px -= dir.x;
                py -= dir.y;
                while (IsValid(px - dir.x, py - dir.y) && board[px - dir.x, py - dir.y] == Constants.PlayerType.PlayerA)
                {
                    leftStoneCount++;
                    px -= dir.x;
                    py -= dir.y;
                }
            }
            else
            {
                leftEmptyFirst = false;
                // px -= dir.x;
                // py -= dir.y;
                while (IsValid(px - dir.x, py - dir.y) && board[px - dir.x, py - dir.y] == Constants.PlayerType.PlayerA)
                {
                    leftStoneCount++;
                    px -= dir.x;
                    py -= dir.y;
                }
            }

            px = x;
            py = y;
            int rightStoneCount = 0;
            bool rightEmptyFirst = false;

            if (IsValid(px + dir.x, py + dir.y) && board[px + dir.x, py + dir.y] == Constants.PlayerType.None)
            {
                rightEmptyFirst = true;
                px += dir.x;
                py += dir.y;
                while (IsValid(px + dir.x, py + dir.y) && board[px + dir.x, py + dir.y] == Constants.PlayerType.PlayerA)
                {
                    rightStoneCount++;
                    px += dir.x;
                    py += dir.y;
                }
            }
            else
            {
                rightEmptyFirst = false;
                // px += dir.x;
                // py += dir.y;
                while (IsValid(px + dir.x, py + dir.y) && board[px + dir.x, py + dir.y] == Constants.PlayerType.PlayerA)
                {
                    rightStoneCount++;
                    px += dir.x;
                    py += dir.y;
                }
            }

            if ((leftEmptyFirst && leftStoneCount == 2) || (rightEmptyFirst && rightStoneCount == 2))
                isSpaceThree = true;

            // bool betweenThree = (leftStoneCount == 1 && rightStoneCount == 1);
            // if (isSpaceBetweenThree)
            //     betweenThree = false;

            if ((leftEmptyFirst && leftStoneCount == 1 && rightStoneCount == 1 && !rightEmptyFirst) ||
                (rightEmptyFirst && rightStoneCount == 1 && leftStoneCount == 1 && !leftEmptyFirst))
            {
                isSpaceBetweenThree = true;
            }

            if (isOpenThree) isSpaceBetweenThree = false;

            return (isOpenThree, isOpenFour, isOverSix, isSpaceThree, isSpaceBetweenThree);
        }

        /// <summary> 보드 좌표 검사 </summary>
        bool IsValid(int x, int y)
        {
            return x >= MIN && x < MAX && y >= MIN && y < MAX;
        }
    }
}