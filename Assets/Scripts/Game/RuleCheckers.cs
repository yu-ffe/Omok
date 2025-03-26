using System.Collections.Generic;
using Commons;
using UnityEngine;

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

    readonly List<Vector2Int> InvalidPlaces = new List<Vector2Int>();
    readonly List<Vector2Int> Opened2ndPlaces = new();
    Vector2Int checkXY;

    public List<Vector2Int> CheckAllBoard()
    {
        InvalidPlaces.Clear();
        for (int x = 0; x < MAX; x++)
        {
            for (int y = 0; y < MAX; y++)
            {
                if (board[x, y] != Constants.PlayerType.None) // 빈 곳만 검사
                    continue;

                Opened2ndPlaces.Clear();// 거짓금수 체크를 위한 빈 자리 체크할 곳
                checkXY = new(x, y);    //**
                //* 임시로 검은돌로 위치
                board[x, y] = Constants.PlayerType.PlayerA;

                (bool isViolation, int v3, int v4, int v6, int sc3, int sbv3) = IsRuleViolation(x, y);
                if (isViolation)
                {
                    //! 2nd 다른 곳의 금수체크
                    bool isOtherPlaceViolation = false;
                    for (int z = 0; z < Opened2ndPlaces.Count; z++)
                    {
                        (bool isViolation2nd, int v3z, int v4z, int v6z, int sc3z, int sbv3z)
                             = IsRuleViolation(Opened2ndPlaces[z].x, Opened2ndPlaces[z].y, isFakeCheck: true);
                        isOtherPlaceViolation = isViolation2nd;
                        // Debug.Log($"<color=#2CC42CFF>{Opened2ndPlaces[z].x},{Opened2ndPlaces[z].y} is violation</color>3[{v3z}]4[{v4z}]6[{v6z}]s3[{sc3z}]sb3[{sbv3z}]");
                    }
                    if (!isOtherPlaceViolation)
                    {
                        InvalidPlaces.Add(new(x, y));
                        Debug.Log($"<color=#ff0000>{x},{y} is violation</color>3[{v3}]4[{v4}]6[{v6}]s3[{sc3}]sb3[{sbv3}]");
                    }
                }
                //* 임시로 검은돌 놓ㄹ고 검사한 것 원위치
                board[x, y] = Constants.PlayerType.None;
            }
        }

        return InvalidPlaces;
    }



    /// <summary> 현재 착수한 위치에서 열린 3, 열린 4, 6목 이상을 검사하는 함수 </summary>
    (bool isViolation, int v3, int v4, int v6, int sv3, int sbv3) IsRuleViolation(int x, int y, bool isFakeCheck = false)
    {
        int countOpened3 = 0;
        int countOpened4 = 0;
        int countOver6 = 0;
        int countSpaceThree = 0;
        int countSpaceBetweenThree = 0;

        foreach (Vector2Int dir in DIR)
        {
            var (openThree, openFour, overSix, spaceThree, spaceBetweenThree) = CheckRow(x, y, dir, isFakeCheck);

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
        bool hasDoubleSpaceBetweenThree = countSpaceBetweenThree >= 2;

        // 조합 조건을 따로 분리
        bool hasOpenThreeAndSpaceThree = countOpened3 + countSpaceThree >= 2;
        bool hasOpenThreeAndSpaceBetweenThree = countOpened3 + countSpaceBetweenThree >= 2;
        bool hasSpaceThreeAndSpaceBetweenThree = countSpaceThree + countSpaceBetweenThree >= 2;

        // 최종 위반 여부
        bool isViolation = hasDoubleOpenThree || hasDoubleOpenFour || hasSixOrMore || hasDoubleSpaceThree ||
                           hasDoubleSpaceBetweenThree || hasOpenThreeAndSpaceThree ||
                           hasOpenThreeAndSpaceBetweenThree || hasSpaceThreeAndSpaceBetweenThree;

        return (isViolation, countOpened3, countOpened4, countOver6, countSpaceThree, countSpaceBetweenThree);
    }

    /// <summary> 특정 위치에서 열린 3, 열린 4, 6목, 띄운 3목, 양쪽 돌 1개씩인 3목을 검사 </summary>
    (bool isOpenThree, bool isOpenFour, bool isOverSix, bool isSpaceThree, bool isSpaceBetweenThree) CheckRow(int x, int y, Vector2Int dir, bool isFakeCheck)
    {
        int count = 1; // 현재 착수한 돌 포함
        bool leftOpen = false, rightOpen = false;
        int px = x, py = y;

        while (IsValid(px - dir.x, py - dir.y) && (board[px - dir.x, py - dir.y] == Constants.PlayerType.PlayerA))
        {
            count++;
            px -= dir.x;
            py -= dir.y;
        }

        //! Open Check : L
        int leftEndX = px - dir.x, leftEndY = py - dir.y;
        if (IsOpened(leftEndX, leftEndX, Vector2Int.zero))
        {
            rightOpen = true;
            if (!isFakeCheck)
                Opened2ndPlaces.Add(new(leftEndX, leftEndY));
        }

        // 반대 방향 탐색
        px = x;
        py = y;
        while (IsValid(px + dir.x, py + dir.y) && (board[px + dir.x, py + dir.y] == Constants.PlayerType.PlayerA))
        {
            count++;
            px += dir.x;
            py += dir.y;
        }

        //! Open Check : R
        int rightEndX = px + dir.x, rightEndY = py + dir.y;
        if (IsOpened(rightEndX, rightEndY, Vector2Int.zero))
        {
            rightOpen = true;
            if (!isFakeCheck)
                Opened2ndPlaces.Add(new(rightEndX, rightEndY));
        }

        bool isOpenThree = (count == 3 && leftOpen && rightOpen);

        bool isOpenFour = (count == 4 && leftOpen && rightOpen);

        bool isOverSix = (count >= 6);



        bool isSpaceThree = false;
        bool isSpaceBetweenThree = false;

        px = x;
        py = y;
        int leftStoneCount = 0;
        bool leftEmptyFirst = false;


        //! LEFT > -
        if (IsValid(px - dir.x, py - dir.y) && board[px - dir.x, py - dir.y] == Constants.PlayerType.None)
        {
            if (!isFakeCheck)
                Opened2ndPlaces.Add(new(px - dir.x, py - dir.y));//*
            leftEmptyFirst = true;
            px -= dir.x;
            py -= dir.y;
            while (IsValid(px - dir.x, py - dir.y) && (board[px - dir.x, py - dir.y] == Constants.PlayerType.PlayerA))
            {
                leftStoneCount++;
                px -= dir.x;
                py -= dir.y;
            }
        }
        else
        {
            leftEmptyFirst = false;
            while (IsValid(px - dir.x, py - dir.y) && (board[px - dir.x, py - dir.y] == Constants.PlayerType.PlayerA))
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
        //!RIGHT > +
        if (IsValid(px + dir.x, py + dir.y) && board[px + dir.x, py + dir.y] == Constants.PlayerType.None)
        {
            if (!isFakeCheck)
                Opened2ndPlaces.Add(new(px + dir.x, py + dir.y));//*
            rightEmptyFirst = true;
            px += dir.x;
            py += dir.y;
            while (IsValid(px + dir.x, py + dir.y) && (board[px + dir.x, py + dir.y] == Constants.PlayerType.PlayerA))
            {
                rightStoneCount++;
                px += dir.x;
                py += dir.y;
            }
        }
        else
        {
            rightEmptyFirst = false;
            while (IsValid(px + dir.x, py + dir.y) && (board[px + dir.x, py + dir.y] == Constants.PlayerType.PlayerA))
            {
                rightStoneCount++;
                px += dir.x;
                py += dir.y;
            }
        }


        if ((leftEmptyFirst && leftStoneCount == 2 && rightStoneCount == 0) ||
            (rightEmptyFirst && rightStoneCount == 2 && leftStoneCount == 0))
        {
            isSpaceThree = true;
        }

        //! Opened Check Space3
        if (isSpaceThree)
        {
            bool isOpenedCheckSpace3 = true;
            // - - V X - O O V -
            // - 2 1 0 1 2 3 4 - 
            bool backL = IsOpened(x, y, dir);
            bool frontL = IsOpened(x, y, dir * -4);

            bool backR = IsOpened(x, y, dir * -1);
            bool frontR = IsOpened(x, y, dir * 4);

            if (leftEmptyFirst && leftStoneCount == 2)
                isOpenedCheckSpace3 = backL && frontL;
            else if (rightEmptyFirst && rightStoneCount == 2)
                isOpenedCheckSpace3 = backR && frontR;

            if (!isOpenedCheckSpace3)
            {
                isSpaceThree = false;
            }
        }

        if ((leftEmptyFirst && leftStoneCount == 1 && rightStoneCount == 1 && !rightEmptyFirst) ||
            (rightEmptyFirst && rightStoneCount == 1 && leftStoneCount == 1 && !leftEmptyFirst))
        {
            isSpaceBetweenThree = true;
        }

        //! Opened Check Between3
        if (isSpaceBetweenThree)
        {
            bool isOpenedCheckBetween3 = true;
            // - V O X - O V -
            // - 2 1 0 1 2 3 -
            bool backL = IsOpened(x, y, dir);
            bool frontL = IsOpened(x, y, dir * -3);

            bool backR = IsOpened(x, y, dir * -1);
            bool frontR = IsOpened(x, y, dir * 3);

            if (rightEmptyFirst)
                isOpenedCheckBetween3 = backL && frontL;
            else if (leftEmptyFirst)
                isOpenedCheckBetween3 = backR && frontR;

            if (!isOpenedCheckBetween3)
            {
                isSpaceBetweenThree = false;
            }
        }

        if (isOpenThree)
            isSpaceBetweenThree = false;


        return (isOpenThree, isOpenFour, isOverSix, isSpaceThree, isSpaceBetweenThree);
    }



    /// <summary> 보드 좌표 검사 </summary>
    bool IsValid(int x, int y)
    {
        return x >= MIN && x < MAX && y >= MIN && y < MAX;
    }

    /// <summary> 위치 열림 확인 </summary>
    bool IsOpened(int x, int y, Vector2Int dir)
    {
        return IsValid(x + dir.x, y + dir.y) && board[x + dir.x, y + dir.y] == Constants.PlayerType.None;
    }

}