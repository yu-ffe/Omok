using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Popop : MonoBehaviour, IPopopUI
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
        UI_Manager.Get.popop = this;
    }

    //옵션. 공통 애니매이션
    DG.Tweening.Sequence sequenceShow;
    DG.Tweening.Sequence sequenceHide;

    public void Hide()
    {
        //닫을 때 효과
        if (sequenceHide == null)
        {
            //필요하면 만들기
        }
        else
            sequenceHide.Restart();
        // objPopup.SetActive(false); // 시퀀스 쓰려면 OnComplete에 해야함

        if (sequenceHide == null)
            objPopup.SetActive(false);
    }

    public void Refresh()
    {

    }

    public void Show(string msg, string okText, string cancelText, UnityAction okAction, UnityAction cancelAction)
    {
        textMsg.text = msg;
        textOk.text = okText;
        textCancel.text = cancelText;

        // Dotween Animation 효과
        objPopup.SetActive(true);
        if (sequenceShow == null)
        {
            //필요하면 만들기
        }
        else
            sequenceShow.Restart();

        // 확인 버튼에 콜백 함수 할당
        btnOk.onClick.RemoveAllListeners();
        btnOk.onClick.AddListener(Hide);
        if (okAction != null)
            btnOk.onClick.AddListener(okAction);

        // 확인 버튼에 콜백 함수 할당
        btnCancel.onClick.RemoveAllListeners();
        btnCancel.onClick.AddListener(Hide);
        if (cancelAction != null)
            btnCancel.onClick.AddListener(cancelAction);


    }
}
