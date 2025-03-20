using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WB;

namespace Wb {
    
public class WBTest : MonoBehaviour
{

    public void AppStart()
    {
        SessionManager.LoadAllSessions();

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

}