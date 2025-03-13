using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WB;

public class WBTest : MonoBehaviour
{
    public UI_Manager.PanelType pnl;

    [ContextMenu("ShowPanel")]
    void ShowPanel()
    {
        UI_Manager.Instance.Show(pnl);
    }

    [ContextMenu("HidePanel")]
    void HidePanel()
    {
        UI_Manager.Instance.Hide(pnl);
    }


    public BasicPopup basicPopup;
    public string popupMsg;
    [ContextMenu("Popup Test")]
    void PopupTest()
    {
        basicPopup.Show(
            msg: popupMsg,
            okText: "okok",
            cancelText: "nono",
            () => { Debug.Log("Click OK"); },
            () => { Debug.Log("Click Cancel"); });
    }
}
