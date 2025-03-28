using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordUIManager : MonoBehaviour
{
    [Header("조정될 UI")]
    [SerializeField] GameObject recordUIObj; // 기보 버튼들
    [SerializeField] GameObject surrenderObj; // 기권 버튼
    [SerializeField] GameObject goMainObj; // 뒤로가기 버튼
    [SerializeField] GameObject timerObj; // 타이머
    [SerializeField] GameObject fireObj; // 착수
    [SerializeField] GameObject turnObj; // 흑백패널

    [Header("수 이동 버튼 오브젝트들")]
    [SerializeField] GameObject buttonObj_First;
    [SerializeField] GameObject buttonObj_Before;
    [SerializeField] GameObject buttonObj_After;
    [SerializeField] GameObject buttonObj_Last;
    

    private void Awake()
    {
        GameManager.Instance.recordUIManager = this;
    }

    public void RecordUISet(bool isRecord)
    {
        // 기보 수이동 버튼 표기
        recordUIObj.SetActive(isRecord);

        // 기권 버튼 뒤로가기 버튼 변경
        surrenderObj.SetActive(!isRecord);
        goMainObj.SetActive(isRecord);

        // 타이머, 착수 버튼 표기
        timerObj.SetActive(!isRecord);
        fireObj.SetActive(!isRecord);
        turnObj.SetActive(!isRecord);
    }

    private void Start()
    {
        ButtonSet();
    }

    void ButtonSet()
    {
        AddSafeButton(buttonObj_First, () => { RecordSaveManager.Instance.TurnBack(RecordSaveManager.Instance.GetBeforeLocation, true); });
        AddSafeButton(buttonObj_Before, () => { RecordSaveManager.Instance.TurnBack(RecordSaveManager.Instance.GetBeforeLocation, false); });
        AddSafeButton(buttonObj_After, () => { RecordSaveManager.Instance.TurnGo(RecordSaveManager.Instance.GetAfterLocation, false); });
        AddSafeButton(buttonObj_Last, () => { RecordSaveManager.Instance.TurnGo(RecordSaveManager.Instance.GetAfterLocation, true); });

        AddSafeButton(goMainObj, () => { GameManager.Instance.ChangeToMainScene(); });
        
     
    }

    void AddSafeButton(GameObject buttonObj, UnityEngine.Events.UnityAction action)
    {
        if (buttonObj == null) return;

        Button btn = buttonObj.GetComponent<Button>();
        if (btn == null) btn = buttonObj.AddComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }
}
