using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commons;
using Commons.Models;
using Commons.Models.Enums;
using Commons.Patterns;
using Game;
using System;
using System.Linq;

public class RecordSaveManager : MonoSingleton<RecordSaveManager>
{
    RecordData loadedRecord;
    int loadIndex;

    public void SetLoadRecord(RecordData recordData) // 재생 기보 선택
    {
        loadedRecord = recordData;
        loadIndex = -1;


        if (loadedRecord != null)
        {
            string movesLog = loadedRecord.Moves.Count > 0
                ? string.Join(", ", loadedRecord.Moves.Select(m => $"({m.player}, {m.x}, {m.y})"))
                : "No moves recorded";

            Debug.Log($"불러온 기록: {loadedRecord.Nickname} / {loadedRecord.Date} / {loadedRecord.Result} / Moves: {movesLog}");
        }
    }






    public void ReplayShow()
    {
        // 기보 재생 함수-기보플레이용 UI 표기
        GameManager.Instance.ChangeToGameScene(GameType.Record);
    }

    public (PlayerType player, int x, int y) GetBeforeLocation() // 현재 기보 이전 수 좌표
    {
        if (loadIndex > 0)
        {
            loadIndex--;
            return (loadedRecord.Moves[loadIndex].player, loadedRecord.Moves[loadIndex].x, loadedRecord.Moves[loadIndex].y);
        }
        else
        {
            return (PlayerType.None, 0, 0); // 되돌아 갈 값 없음
        }
    }

    public (PlayerType player, int x, int y) GetAfterLocation() // 현재 기보 다음 수 좌표
    {
        if (loadIndex < loadedRecord.Moves.Count - 1) // 다음 수가 존재할 때만 증가
        {
            loadIndex++;
            return (loadedRecord.Moves[loadIndex].player, loadedRecord.Moves[loadIndex].x, loadedRecord.Moves[loadIndex].y);
        }
        else
        {
            return (PlayerType.None, 0, 0); // 끝
        }
    }

    #region 기보 착수 기능

    public void TurnGo(Func<(PlayerType, int, int)> getLocationFunc, bool isContinuous)
    {
        if (isContinuous)
            StartCoroutine(GoToTargetLocation(getLocationFunc));
        else
            PlaceStone(getLocationFunc());
    }
    
    public void TurnBack(Func<(PlayerType, int, int)> getLocationFunc, bool isContinuous)
    {
        if (isContinuous)
            StartCoroutine(RemoveToTargetLocation(getLocationFunc));
        else
            ReMoveStone(getLocationFunc());
    }

    void ReMoveStone((PlayerType, int, int) location)
    {
        if (location.Item1 == PlayerType.None) return;

        Debug.Log($"({location.Item2}, {location.Item3}) 좌표");
        // TODO: location 좌표에 착수 기능 추가
        GameManager.Instance.omokBoard.RemoveStone();
    }
    
    void PlaceStone((PlayerType, int, int) location)
    {
        if (location.Item1 == PlayerType.None) return;

        Debug.Log($"({location.Item2}, {location.Item3}) 좌표");
        // TODO: location 좌표에 착수 기능 추가
        GameManager.Instance.omokBoard.PlaceStone(location.Item1,location.Item2,location.Item3);
    }

    IEnumerator GoToTargetLocation(Func<(PlayerType, int, int)> getLocationFunc)
    {
        (PlayerType, int, int) location = getLocationFunc();

        while (location.Item1 != PlayerType.None) // 이동 가능할 때만 실행
        {
            // TODO: location 좌표에 착수 기능
            GameManager.Instance.omokBoard.PlaceStone(location.Item1,location.Item2,location.Item3);
            Debug.Log($"({location.Item2}, {location.Item3}) 좌표");

            yield return new WaitForSeconds(0.2f);

            location = getLocationFunc(); // 다음 목표 좌표 이동
        }
    }
    
    IEnumerator RemoveToTargetLocation(Func<(PlayerType, int, int)> getLocationFunc)
    {
        (PlayerType, int, int) location = getLocationFunc();

        while (location.Item1 != PlayerType.None) // 이동 가능할 때만 실행
        {
            // TODO: location 좌표에 착수 기능
            GameManager.Instance.omokBoard.RemoveStone();
            Debug.Log($"({location.Item2}, {location.Item3}) 좌표");

            yield return new WaitForSeconds(0.2f);

            location = getLocationFunc(); // 다음 목표 좌표 이동
        }
    }

    #endregion








    public RecordData GetLoadedRecordData() // 불러온 기보에 대한 정보
    {
        return loadedRecord;
    }


}
