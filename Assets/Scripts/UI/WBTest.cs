using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WBTest : MonoBehaviour
{

    public void AppStart()
    {
        // Todo: 서버에서 로드로 변경
        // SessionManager.LoadAllSessions();

        UI_Manager.Instance.Show(UI_Manager.PanelType.Login);

        gameObject.SetActive(false);
    }


    // public BasicPopup basicPopup;
    public string testPopupMsg;
    [ContextMenu("Popup Test")]
    void PopupTest()
    {
        UI_Manager.Instance.popup.Show(
            msg: testPopupMsg,
            okText: "okok",
            cancelText: "nono",
            () => { Debug.Log("Click OK"); },
            () => { Debug.Log("Click Cancel"); });
    }

    public UI_Manager.PanelType panelType;
    [ContextMenu("Show Test")]
    void ShowTest()
    {
        UI_Manager.Instance.Show(panelType);
    }
}
