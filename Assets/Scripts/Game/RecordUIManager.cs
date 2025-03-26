using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordUIManager : Singleton<RecordUIManager>
{
    [SerializeField] GameObject recordUIObj;

    [SerializeField] GameObject buttonObj_First;
    [SerializeField] GameObject buttonObj_Before;
    [SerializeField] GameObject buttonObj_After;
    [SerializeField] GameObject buttonObj_Last;

    public void RecordUISet(bool on)
    {
        recordUIObj.SetActive(on);
    }

    private void Start()
    {
        ButtonSet();
    }

    void ButtonSet()
    {
        AddSafeButton(buttonObj_First, () => { Debug.Log("기보 처음 수까지 두기 함수 실행"); });
        AddSafeButton(buttonObj_Before, () => { Debug.Log("기보 이전 수 두기 함수 실행"); });
        AddSafeButton(buttonObj_After, () => { Debug.Log("기보 다음 수 두기 함수 실행"); });
        AddSafeButton(buttonObj_Last, () => { Debug.Log("기보 마지막 수까지 두기 함수 실행"); });
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
