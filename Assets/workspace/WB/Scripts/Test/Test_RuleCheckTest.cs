using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT = Commons.Constants.PlayerType;
public class Test_RuleCheckTest : MonoBehaviour
{
    public Transform rootStones;
    public Transform rootPlacers;
    public GameObject oBlack, oWhite, oX, placer;


    RuleCheckers rule;
    TT[,] board;


    List<Test_Stone> xMarkers;
    Dictionary<Vector2Int, Test_Stone> onStones;
    Dictionary<TT, Stack<Test_Stone>> poolStone;


    Stack<RecData> prev, next;
    int nowActionIndex;
    RecData nowRecData;

    bool isShowX;

    void Start()
    {
        rule = new();
        board = new TT[15, 15];
        xMarkers = new();
        onStones = new();
        poolStone = new();
        poolStone.Add(TT.PlayerA, new Stack<Test_Stone>());
        poolStone.Add(TT.PlayerB, new Stack<Test_Stone>());
        poolStone.Add(TT.PlayerX, new Stack<Test_Stone>());

        nowActionIndex = -1;
        prev = new();
        next = new();

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                board[x, y] = TT.None;
                var pp = Instantiate(placer, rootPlacers).GetComponent<Test_Placer>();
                pp.Initialize(x, y, this);
            }
        }

        rule.Initialize(board);
    }


    public TT GetNextType(int x, int y)
    {
        var nowT = board[x, y];

        if (nowT == TT.None)
            return TT.PlayerA;

        if (nowT == TT.PlayerA)
            return TT.PlayerB;

        if (nowT == TT.PlayerB)
            return TT.None;

        return TT.None;
    }

    public void PlaceStone(Vector2Int XY, TT type, bool writeRec = true)
    {
        Debug.Log($"PlaceStone[{type}] : {XY}");
        if (writeRec)
        {
            if (nowActionIndex == -1)
            {
                nowActionIndex = 0;
            }
            else
            {
                prev.Push(nowRecData);
                nowActionIndex = prev.Peek().actionIndex + 1;
            }
            nowRecData = new RecData
            {
                actionIndex = nowActionIndex,
                XY = XY,
                typeAction = type
            };
            next.Clear();
        }

        if (type == TT.None)
        {
            if (onStones.ContainsKey(XY))
            {
                PushStone(onStones[XY]);
            }
            board[XY.x, XY.y] = type;
            return;
        }

        var stone = PopStone(type);
        stone.XY = XY;

        if (onStones.ContainsKey(XY))
        {
            PushStone(onStones[XY]);
            onStones[XY] = stone;
        }
        else
        {
            onStones.Add(XY, stone);
        }

        board[XY.x, XY.y] = type;

    }




    public void OnClick_RuleCheck()
    {
        if (isShowX)
        {
            for (int i = 0; i < xMarkers.Count; i++)
            {
                PushStone(xMarkers[i]);
            }

            xMarkers.Clear();
            isShowX = false;
            return;
        }

        var xList = rule.CheckAllBoard();

        for (int i = 0; i < xList.Count; i++)
        {
            var xMarker = PopStone(TT.PlayerX);
            xMarker.XY = xList[i];
            xMarkers.Add(xMarker);
        }
        isShowX = true;
    }

    public void OnClick_RemoveAll()
    {
        foreach (var xy in onStones.Keys)
        {
            PushStone(onStones[xy]);
        }
        onStones.Clear();

        for (int x = 0; x < 15; x++)
            for (int y = 0; y < 15; y++)
                board[x, y] = TT.None;
    }

    public void OnClick_Undo()
    {
        if (prev.Count == 0)
            return;

        if (nowRecData.typeAction != TT.None)
        {
            PushStone(onStones[nowRecData.XY]);
            onStones.Remove(nowRecData.XY);
            board[nowRecData.XY.x, nowRecData.XY.y] = TT.None;
        }
        next.Push(nowRecData);
        nowActionIndex--;
        nowRecData = prev.Pop();

        PlaceStone(nowRecData.XY, nowRecData.typeAction, false);
    }

    public void OnClick_Redo()
    {
        if (next.Count == 0)
            return;

    }




    Test_Stone PopStone(TT type)
    {
        Test_Stone stone;

        if (poolStone[type].Count > 0)
            stone = poolStone[type].Pop();
        else
        {
            switch (type)
            {
                case TT.PlayerA: stone = Instantiate(oBlack, rootStones).GetComponent<Test_Stone>(); break;
                case TT.PlayerB: stone = Instantiate(oWhite, rootStones).GetComponent<Test_Stone>(); break;
                case TT.PlayerX: stone = Instantiate(oX, rootStones).GetComponent<Test_Stone>(); break;
                default: return null;
            }
        }
        stone.gameObject.SetActive(true);
        stone.type = type;
        return stone;
    }

    void PushStone(Test_Stone stone)
    {
        stone.gameObject.SetActive(false);
        poolStone[stone.type].Push(stone);
    }





}







public struct RecData
{
    public int actionIndex;
    public Vector2Int XY;
    public TT typeAction;
}
