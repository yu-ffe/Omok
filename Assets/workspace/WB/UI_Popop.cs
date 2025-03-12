using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Popup : MonoBehaviour, IPopopUI, IObserverUI
{
    public GameObject objPopup;     //root
    public TextMeshProUGUI textMsg; //child 0
    public Button btnOk;            //child 1
    public Button btnCancel;        //chidl 2
    public TextMeshProUGUI textOk;  //child 1 - child 0
    public TextMeshProUGUI textCancel; //chidl 2 - child 0


    private void OnValidate()
    {
        if (textMsg == null)
            textMsg = objPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (btnOk == null)
            btnOk = objPopup.transform.GetChild(1).GetComponent<Button>();
        if (btnCancel == null)
            btnCancel = objPopup.transform.GetChild(2).GetComponent<Button>();
        if (textOk == null)
            textOk = btnOk.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (textCancel == null)
            textCancel = btnCancel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine(AddPopupToManager());
    }

    IEnumerator AddPopupToManager()
    {
        while (UI_Manager.Get == null)
            yield return null;

        //메인 Popup 등록
        UI_Manager.Get.popup = this;
    }

    public void Show(string msg, string okText = null, string cancelText = null, UnityAction okAction = null, UnityAction cancelAction = null)
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        throw new System.NotImplementedException();
    }

    public void Refresh()
    {
        //?? tpfhrhcxla

    }

    public void OnNotify()
    {
        throw new System.NotImplementedException();
    }

    public void OnNotify(string msg)
    {
        Refresh();
    }

    //옵션. 공통 애니매이션
    DG.Tweening.Sequence sequenceShow;
    DG.Tweening.Sequence sequenceHide;

    public string NotifyKey { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}
