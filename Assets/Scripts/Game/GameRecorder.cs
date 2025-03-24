using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameRecorder
{
    private const int MaxRecords = 10; // 최대 저장 개수

    static GameResult gameResult;

    // 게임 결과 임시 저장
    public static void GameResultSave(GameResult result) // TODO 게임 종료 후 초기화 전 호출 필요
    {
        gameResult = result;
    }

    // 기보 저장하기
    public static void SaveGameRecord() // TODO 게임 저장 시 호출 필요
    {
        string nickname = PlayerManager.Instance.playerData.nickname;
        string grade = PlayerManager.Instance.playerData.grade.ToString();
        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // 한국시간 기준

        // 현재 저장된 기보 개수 확인
        int recordCount = PlayerPrefs.GetInt("RecordCount", 0);

        // 10개가 넘으면 가장 오래된 기록 삭제
        if (recordCount >= MaxRecords)
        {
            DeleteOldestRecord();
            recordCount--; // 삭제 후 개수 조정
        }

        // 새로운 기보를 저장할 인덱스
        int newIndex = recordCount;
        string recordKey = $"record_{newIndex}";

        // 기보 데이터를 JSON 문자열로 변환 후 저장
        RecordData record = new RecordData
        {
            Nickname = nickname,
            Grade = grade,
            Date = date,
            Result = gameResult.ToString(),
            Moves = GameManager.Instance.GameLogicInstance.moveList
        };

        string json = JsonUtility.ToJson(record);
        PlayerPrefs.SetString(recordKey, json);

        // 저장된 개수 업데이트
        PlayerPrefs.SetInt("RecordCount", recordCount + 1);
        PlayerPrefs.Save();

        Debug.Log($"게임 기록 저장 완료: {recordKey}");
    }


    // 저장된 기보 불러오기
    public static RecordData LoadGameRecord(int index)
    {
        string recordKey = $"record_{index}";

        if (PlayerPrefs.HasKey(recordKey))
        {
            string json = PlayerPrefs.GetString(recordKey);
            return JsonUtility.FromJson<RecordData>(json);
        }

        Debug.LogWarning($"기보를 찾을 수 없음: {recordKey}");
        return null;
    }

    // 저장된 모든 기보 리스트 불러오기
    public static List<RecordData> GetAllGameRecords()
    {
        int recordCount = PlayerPrefs.GetInt("RecordCount", 0);
        List<RecordData> records = new List<RecordData>();

        for (int i = 0; i < recordCount; i++)
        {
            RecordData record = LoadGameRecord(i);
            if (record != null)
            {
                records.Add(record);
            }
        }

        return records;
    }

    // 기보 삭제
    public static void DeleteGameRecord(int index)
    {
        int recordCount = PlayerPrefs.GetInt("RecordCount", 0);

        if (index < 0 || index >= recordCount) return;

        string recordKey = $"record_{index}";
        PlayerPrefs.DeleteKey(recordKey);

        // 이후 기록들을 한 칸씩 앞으로 당김
        for (int i = index + 1; i < recordCount; i++)
        {
            string oldKey = $"record_{i}";
            string newKey = $"record_{i - 1}";

            if (PlayerPrefs.HasKey(oldKey))
            {
                string data = PlayerPrefs.GetString(oldKey);
                PlayerPrefs.SetString(newKey, data);
                PlayerPrefs.DeleteKey(oldKey);
            }
        }

        // 저장된 개수 줄이기
        PlayerPrefs.SetInt("RecordCount", recordCount - 1);
        PlayerPrefs.Save();
    }

    // 가장 오래된 기보 삭제
    private static void DeleteOldestRecord()
    {
        int recordCount = PlayerPrefs.GetInt("RecordCount", 0);

        if (recordCount <= 0) return; // 저장된 기록이 없으면 종료

        // 가장 오래된 record_0 삭제
        string oldestKey = "record_0";
        PlayerPrefs.DeleteKey(oldestKey);

        // 남은 기록들을 앞으로 이동 (record_1 → record_0, record_2 → record_1 ...)
        for (int i = 1; i < recordCount; i++)
        {
            string oldKey = $"record_{i}";
            string newKey = $"record_{i - 1}";

            if (PlayerPrefs.HasKey(oldKey))
            {
                string data = PlayerPrefs.GetString(oldKey);
                PlayerPrefs.SetString(newKey, data);
                PlayerPrefs.DeleteKey(oldKey);
            }
        }

        PlayerPrefs.Save();
    }







}
